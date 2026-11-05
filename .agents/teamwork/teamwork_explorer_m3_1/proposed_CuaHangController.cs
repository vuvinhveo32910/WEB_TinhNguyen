using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Data;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

namespace WEB_TinhNguyen.Areas.CuaHang.Controllers
{
    /// <summary>
    /// Controller phục vụ Storefront (Giao diện người dùng) của Cửa Hàng Gây Quỹ Tình Nguyện.
    /// Hỗ trợ xem danh mục sản phẩm, tìm kiếm, lọc theo phân loại, sắp xếp giá/đóng góp,
    /// xem chi tiết sản phẩm kèm thông điệp gây quỹ và các sản phẩm liên quan.
    /// Hoạt động an toàn ở chế độ kép: Tự động truy vấn SQL Server và fallback In-Memory Store khi offline.
    /// </summary>
    [Area("CuaHang")]
    public class CuaHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuaHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. DANH MỤC SẢN PHẨM STOREFRONT (INDEX / CATALOG)
        // =========================================================================
        /// <summary>
        /// Hiển thị danh sách sản phẩm gây quỹ đang hoạt động (TrangThai == true)
        /// Hỗ trợ tìm kiếm theo từ khóa (q), lọc theo danh mục (danhMuc) và sắp xếp (sort).
        /// </summary>
        /// <param name="q">Từ khóa tìm kiếm (theo tên, mô tả ngắn, thông điệp gây quỹ, danh mục)</param>
        /// <param name="danhMuc">Tên danh mục cần lọc (hoặc "Tất cả")</param>
        /// <param name="sort">Kiểu sắp xếp: gia-tang, gia-giam, dong-gop, moi-nhat, mac-dinh</param>
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? danhMuc, string? sort)
        {
            List<SanPhamGayQuy> allProducts;

            try
            {
                if (_context.Database.CanConnect())
                {
                    allProducts = await _context.SanPhamGayQuys
                        .Where(p => p.TrangThai == true)
                        .OrderBy(p => p.ThuTu)
                        .ThenByDescending(p => p.IDSanPham)
                        .ToListAsync();

                    // Tự động seed dữ liệu ban đầu nếu database mới tạo và chưa có sản phẩm
                    if (!allProducts.Any())
                    {
                        DbInitializer.Seed(_context);
                        allProducts = await _context.SanPhamGayQuys
                            .Where(p => p.TrangThai == true)
                            .OrderBy(p => p.ThuTu)
                            .ThenByDescending(p => p.IDSanPham)
                            .ToListAsync();
                    }
                }
                else
                {
                    // Fallback sang dữ liệu mẫu In-Memory khi SQL Server offline
                    allProducts = DbInitializer.GetInMemoryProducts()
                        .Where(p => p.TrangThai == true)
                        .OrderBy(p => p.ThuTu)
                        .ThenByDescending(p => p.IDSanPham)
                        .ToList();
                }
            }
            catch
            {
                // Phòng vệ toàn diện trước mọi ngoại lệ database
                allProducts = DbInitializer.GetInMemoryProducts()
                    .Where(p => p.TrangThai == true)
                    .OrderBy(p => p.ThuTu)
                    .ThenByDescending(p => p.IDSanPham)
                    .ToList();
            }

            // Trích xuất danh sách danh mục khả dụng duy nhất cho các tab/nút lọc
            var categories = allProducts
                .Select(p => p.DanhMuc)
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d)
                .ToList();

            ViewBag.Categories = categories;

            // 1. Lọc theo danh mục nếu được chỉ định
            var filtered = allProducts.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(danhMuc) && !danhMuc.Equals("Tất cả", StringComparison.OrdinalIgnoreCase) && !danhMuc.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filtered = filtered.Where(p => string.Equals(p.DanhMuc, danhMuc.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            // 2. Lọc theo từ khóa tìm kiếm (q)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var kw = q.Trim();
                filtered = filtered.Where(p =>
                    (!string.IsNullOrEmpty(p.TenSanPham) && p.TenSanPham.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(p.MoTaNgan) && p.MoTaNgan.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(p.ThongDiepGayQuy) && p.ThongDiepGayQuy.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(p.DanhMuc) && p.DanhMuc.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            // 3. Sắp xếp kết quả
            var sortMode = (sort ?? string.Empty).Trim().ToLowerInvariant();
            filtered = sortMode switch
            {
                "gia-tang" or "price-asc" or "gia_tang" or "asc" => filtered.OrderBy(p => p.GiaBan),
                "gia-giam" or "price-desc" or "gia_giam" or "desc" => filtered.OrderByDescending(p => p.GiaBan),
                "dong-gop" or "donation" or "quy" or "fund" => filtered.OrderByDescending(p => p.PhanTramDongGop).ThenByDescending(p => p.GiaBan),
                "moi-nhat" or "newest" or "new" or "date-desc" => filtered.OrderByDescending(p => p.NgayTao),
                _ => filtered.OrderBy(p => p.ThuTu).ThenByDescending(p => p.IDSanPham)
            };

            var resultList = filtered.ToList();

            // Truyền trạng thái lọc/tìm kiếm về View
            ViewBag.CurrentSearch = q ?? string.Empty;
            ViewBag.CurrentCategory = string.IsNullOrWhiteSpace(danhMuc) ? "Tất cả" : danhMuc.Trim();
            ViewBag.CurrentSort = sortMode;
            ViewBag.TotalCount = resultList.Count;

            return View(resultList);
        }

        // =========================================================================
        // 2. CHI TIẾT SẢN PHẨM STOREFRONT (DETAIL)
        // =========================================================================
        /// <summary>
        /// Hiển thị thông tin chi tiết một sản phẩm gây quỹ (giá, thông điệp thiện nguyện,
        /// bộ chọn số lượng, nút thêm vào giỏ và danh sách sản phẩm liên quan).
        /// </summary>
        /// <param name="id">Mã định danh sản phẩm (IDSanPham)</param>
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Mã sản phẩm không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            SanPhamGayQuy? product = null;
            List<SanPhamGayQuy> relatedProducts = new List<SanPhamGayQuy>();

            try
            {
                if (_context.Database.CanConnect())
                {
                    product = await _context.SanPhamGayQuys
                        .FirstOrDefaultAsync(p => p.IDSanPham == id && p.TrangThai == true);

                    if (product != null)
                    {
                        // Lấy các sản phẩm liên quan cùng danh mục
                        relatedProducts = await _context.SanPhamGayQuys
                            .Where(p => p.TrangThai == true && p.IDSanPham != id && p.DanhMuc == product.DanhMuc)
                            .OrderBy(p => p.ThuTu)
                            .Take(4)
                            .ToListAsync();

                        // Nếu cùng danh mục chưa đủ 4 sản phẩm, bổ sung thêm các sản phẩm khác
                        if (relatedProducts.Count < 4)
                        {
                            var existingIds = relatedProducts.Select(r => r.IDSanPham).ToList();
                            existingIds.Add(id);

                            var more = await _context.SanPhamGayQuys
                                .Where(p => p.TrangThai == true && !existingIds.Contains(p.IDSanPham))
                                .OrderBy(p => p.ThuTu)
                                .Take(4 - relatedProducts.Count)
                                .ToListAsync();

                            relatedProducts.AddRange(more);
                        }
                    }
                }
                else
                {
                    // Fallback sang In-Memory Store
                    product = DbInitializer.GetInMemoryProductById(id);
                    if (product != null && product.TrangThai)
                    {
                        relatedProducts = DbInitializer.GetInMemoryProducts()
                            .Where(p => p.TrangThai == true && p.IDSanPham != id)
                            .Take(4)
                            .ToList();
                    }
                }
            }
            catch
            {
                // Bắt ngoại lệ và truy xuất từ In-Memory Store
                product = DbInitializer.GetInMemoryProductById(id);
                if (product != null && product.TrangThai)
                {
                    relatedProducts = DbInitializer.GetInMemoryProducts()
                        .Where(p => p.TrangThai == true && p.IDSanPham != id)
                        .Take(4)
                        .ToList();
                }
            }

            // Xử lý khi không tìm thấy hoặc sản phẩm đã bị ẩn/ngừng hoạt động
            if (product == null || !product.TrangThai)
            {
                TempData["ErrorMessage"] = "Sản phẩm gây quỹ không tồn tại hoặc đã tạm dừng lưu hành.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
        }
    }
}
