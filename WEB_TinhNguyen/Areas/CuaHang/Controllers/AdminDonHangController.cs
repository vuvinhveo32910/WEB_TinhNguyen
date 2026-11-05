using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

namespace WEB_TinhNguyen.Areas.CuaHang.Controllers
{
    /// <summary>
    /// Phân hệ Quản trị Đơn đặt mua / Ủng hộ Gây quỹ Tình nguyện (Admin Order Management)
    /// Hỗ trợ tra cứu, lọc trạng thái, cập nhật quy trình xử lý đơn hàng, hoàn trả tồn kho khi hủy đơn,
    /// thống kê KPI doanh thu và đóng góp quỹ thiện nguyện với cơ chế tự động fallback In-Memory.
    /// </summary>
    [Area("CuaHang")]
    public class AdminDonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAjaxRequest()
        {
            return Request?.Headers != null &&
                   (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    Request.Headers["Accept"].ToString().Contains("application/json"));
        }

        // =========================================================================
        // 1. DANH SÁCH ĐƠN HÀNG & THỐNG KÊ KPI (INDEX)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? trangThai, string? thanhToan, string? sort)
        {
            List<DonHangGayQuy> allOrders;

            try
            {
                if (_context.Database.CanConnect())
                {
                    allOrders = await _context.DonHangGayQuys
                        .Include(o => o.ChiTietDonHangs)
                            .ThenInclude(c => c.SanPham)
                        .OrderByDescending(o => o.NgayDatHang)
                        .ToListAsync();

                    // Nếu database mới kết nối lần đầu và chưa có đơn hàng, tự động seed 2 đơn hàng mẫu
                    if (!allOrders.Any())
                    {
                        DbInitializer.Seed(_context);
                        allOrders = await _context.DonHangGayQuys
                            .Include(o => o.ChiTietDonHangs)
                                .ThenInclude(c => c.SanPham)
                            .OrderByDescending(o => o.NgayDatHang)
                            .ToListAsync();
                    }
                }
                else
                {
                    allOrders = DbInitializer.GetInMemoryOrders();
                }
            }
            catch
            {
                allOrders = DbInitializer.GetInMemoryOrders();
            }

            // Tính toán và truyền các chỉ số KPI thống kê vào ViewBag
            ViewBag.TongDonHang = allOrders.Count;
            ViewBag.ChoXacNhan = allOrders.Count(o => string.Equals(o.TrangThaiDonHang, "MoiDat", StringComparison.OrdinalIgnoreCase));
            var activeOrders = allOrders.Where(o => !string.Equals(o.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.TongDoanhThu = activeOrders.Sum(o => o.TongThanhToan);
            ViewBag.TongTienQuy = activeOrders.Sum(o => o.TienUngHoThem + (o.ChiTietDonHangs?.Sum(c => c.TienDongGop) ?? 0m));

            // Lưu các tham số lọc để hiển thị trên View
            ViewBag.CurrentSearch = q;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentThanhToan = thanhToan;
            ViewBag.CurrentSort = sort;

            // Xử lý tìm kiếm và lọc dữ liệu
            var query = allOrders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLower();
                query = query.Where(o =>
                    (!string.IsNullOrEmpty(o.MaDonHang) && o.MaDonHang.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(o.HoTenNguoiMua) && o.HoTenNguoiMua.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(o.SoDienThoai) && o.SoDienThoai.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(o.Email) && o.Email.ToLower().Contains(keyword))
                );
            }

            if (!string.IsNullOrWhiteSpace(trangThai) && !string.Equals(trangThai, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => string.Equals(o.TrangThaiDonHang, trangThai, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(thanhToan) && !string.Equals(thanhToan, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => string.Equals(o.TrangThaiThanhToan, thanhToan, StringComparison.OrdinalIgnoreCase));
            }

            query = sort switch
            {
                "date_asc" => query.OrderBy(o => o.NgayDatHang),
                "total_desc" => query.OrderByDescending(o => o.TongThanhToan),
                "total_asc" => query.OrderBy(o => o.TongThanhToan),
                _ => query.OrderByDescending(o => o.NgayDatHang)
            };

            var filteredOrders = query.ToList();
            return View(filteredOrders);
        }

        // =========================================================================
        // 2. CHI TIẾT ĐƠN HÀNG (CHI TIET)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            DonHangGayQuy? order = null;

            try
            {
                if (_context.Database.CanConnect())
                {
                    order = await _context.DonHangGayQuys
                        .Include(o => o.ChiTietDonHangs)
                            .ThenInclude(c => c.SanPham)
                        .FirstOrDefaultAsync(o => o.IDDonHang == id);
                }
                else
                {
                    order = DbInitializer.GetInMemoryOrderById(id);
                }
            }
            catch
            {
                order = DbInitializer.GetInMemoryOrderById(id);
            }

            if (order == null)
            {
                order = DbInitializer.GetInMemoryOrderById(id);
            }

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // =========================================================================
        // 3. CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG & RESTOCK (CAP NHAT TRANG THAI)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiDonHang, string? trangThaiThanhToan)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(trangThaiDonHang))
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Dữ liệu trạng thái đơn hàng không hợp lệ." });
                }
                TempData["ErrorMessage"] = "Dữ liệu trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            bool updated = false;

            try
            {
                if (_context.Database.CanConnect())
                {
                    var order = await _context.DonHangGayQuys
                        .Include(o => o.ChiTietDonHangs)
                        .FirstOrDefaultAsync(o => o.IDDonHang == id);

                    if (order != null)
                    {
                        var oldStatus = order.TrangThaiDonHang;
                        bool wasCancelled = string.Equals(oldStatus, "DaHuy", StringComparison.OrdinalIgnoreCase);
                        bool isCancelled = string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase);

                        if (wasCancelled && !isCancelled)
                        {
                            if (IsAjaxRequest())
                            {
                                return Json(new { success = false, message = "Không thể thay đổi trạng thái của đơn hàng đã hủy." });
                            }
                            TempData["ErrorMessage"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy.";
                            return RedirectToAction(nameof(ChiTiet), new { id = id });
                        }

                        bool wasNotCancelled = !wasCancelled;

                        order.TrangThaiDonHang = trangThaiDonHang;
                        if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
                        {
                            order.TrangThaiThanhToan = trangThaiThanhToan;
                        }
                        order.NgayCapNhat = DateTime.Now;

                        // Restock logic: Nếu chuyển sang "DaHuy", hoàn trả tồn kho và giảm số lượng đã bán
                        if (wasNotCancelled && isCancelled && order.ChiTietDonHangs != null)
                        {
                            foreach (var detail in order.ChiTietDonHangs)
                            {
                                if (detail.IDSanPham.HasValue)
                                {
                                    var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
                                    if (prod != null)
                                    {
                                        prod.SoLuongTon += detail.SoLuong;
                                        prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);
                                    }
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        updated = true;
                    }
                }
                else
                {
                    var inMemOrder = DbInitializer.GetInMemoryOrderById(id);
                    if (inMemOrder != null && string.Equals(inMemOrder.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase) && !string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsAjaxRequest())
                        {
                            return Json(new { success = false, message = "Không thể thay đổi trạng thái của đơn hàng đã hủy." });
                        }
                        TempData["ErrorMessage"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy.";
                        return RedirectToAction(nameof(ChiTiet), new { id = id });
                    }
                    updated = DbInitializer.UpdateInMemoryOrderStatus(id, trangThaiDonHang, trangThaiThanhToan);
                }
            }
            catch
            {
                var inMemOrder = DbInitializer.GetInMemoryOrderById(id);
                if (inMemOrder != null && string.Equals(inMemOrder.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase) && !string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase))
                {
                    if (IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Không thể thay đổi trạng thái của đơn hàng đã hủy." });
                    }
                    TempData["ErrorMessage"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy.";
                    return RedirectToAction(nameof(ChiTiet), new { id = id });
                }
                updated = DbInitializer.UpdateInMemoryOrderStatus(id, trangThaiDonHang, trangThaiThanhToan);
            }

            if (!updated)
            {
                var inMemOrder = DbInitializer.GetInMemoryOrderById(id);
                if (inMemOrder != null && string.Equals(inMemOrder.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase) && !string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase))
                {
                    if (IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Không thể thay đổi trạng thái của đơn hàng đã hủy." });
                    }
                    TempData["ErrorMessage"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy.";
                    return RedirectToAction(nameof(ChiTiet), new { id = id });
                }
                updated = DbInitializer.UpdateInMemoryOrderStatus(id, trangThaiDonHang, trangThaiThanhToan);
            }

            if (!updated)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Không tìm thấy đơn hàng #{id} để cập nhật." });
                }
                TempData["ErrorMessage"] = $"Không tìm thấy đơn hàng #{id}.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái đơn hàng thành công!",
                    orderId = id,
                    trangThaiDonHang = trangThaiDonHang,
                    trangThaiThanhToan = trangThaiThanhToan
                });
            }

            return RedirectToAction(nameof(ChiTiet), new { id = id });
        }

        // =========================================================================
        // 4. XÓA ĐƠN HÀNG (XOA DON HANG)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> XoaDonHang(int id)
        {
            if (id <= 0)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Mã đơn hàng không hợp lệ." });
                }
                TempData["ErrorMessage"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            bool deleted = false;

            try
            {
                if (_context.Database.CanConnect())
                {
                    var order = await _context.DonHangGayQuys
                        .Include(o => o.ChiTietDonHangs)
                        .FirstOrDefaultAsync(o => o.IDDonHang == id);

                    if (order != null)
                    {
                        // Hoàn lại số lượng tồn kho nếu đơn hàng chưa hủy trước khi xóa
                        if (!string.Equals(order.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase) && order.ChiTietDonHangs != null)
                        {
                            foreach (var detail in order.ChiTietDonHangs)
                            {
                                if (detail.IDSanPham.HasValue)
                                {
                                    var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
                                    if (prod != null)
                                    {
                                        prod.SoLuongTon += detail.SoLuong;
                                        prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);
                                    }
                                }
                            }
                        }

                        _context.DonHangGayQuys.Remove(order);
                        await _context.SaveChangesAsync();
                        deleted = true;
                    }
                }
                else
                {
                    deleted = DbInitializer.DeleteInMemoryOrder(id);
                }
            }
            catch
            {
                deleted = DbInitializer.DeleteInMemoryOrder(id);
            }

            if (!deleted)
            {
                deleted = DbInitializer.DeleteInMemoryOrder(id);
            }

            if (!deleted)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Không tìm thấy đơn hàng #{id} để xóa." });
                }
                TempData["ErrorMessage"] = $"Không tìm thấy đơn hàng #{id}.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Đã xóa đơn hàng thành công!";

            if (IsAjaxRequest())
            {
                return Json(new { success = true, message = "Đã xóa đơn hàng thành công!" });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
