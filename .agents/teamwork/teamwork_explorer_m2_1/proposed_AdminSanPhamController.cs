using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Data;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

namespace WEB_TinhNguyen.Areas.CuaHang.Controllers
{
    /// <summary>
    /// Phân hệ Quản trị Sản phẩm Gây quỹ Tình nguyện (Admin Product Management)
    /// Đảm nhận toàn bộ nghiệp vụ CRUD sản phẩm, upload hình ảnh lên máy chủ,
    /// bảo vệ dữ liệu với Anti-Forgery Token và tự động fallback sang In-Memory Store khi chạy offline.
    /// </summary>
    [Area("CuaHang")]
    public class AdminSanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminSanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================================================================
        // 1. DANH SÁCH SẢN PHẨM (INDEX)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<SanPhamGayQuy> products;

            try
            {
                if (_context.Database.CanConnect())
                {
                    products = await _context.SanPhamGayQuys
                        .OrderBy(p => p.ThuTu)
                        .ThenByDescending(p => p.IDSanPham)
                        .ToListAsync();

                    // Nếu database mới kết nối lần đầu và chưa có dữ liệu, tự động seed 4 sản phẩm mẫu
                    if (!products.Any())
                    {
                        DbInitializer.Seed(_context);
                        products = await _context.SanPhamGayQuys
                            .OrderBy(p => p.ThuTu)
                            .ThenByDescending(p => p.IDSanPham)
                            .ToListAsync();
                    }
                }
                else
                {
                    // Fallback In-Memory khi database server offline trong sandbox
                    products = DbInitializer.GetInMemoryProducts()
                        .OrderBy(p => p.ThuTu)
                        .ThenByDescending(p => p.IDSanPham)
                        .ToList();
                }
            }
            catch
            {
                // Bắt toàn bộ ngoại lệ kết nối/cơ sở dữ liệu để hệ thống không bao giờ bị gián đoạn
                products = DbInitializer.GetInMemoryProducts()
                    .OrderBy(p => p.ThuTu)
                    .ThenByDescending(p => p.IDSanPham)
                    .ToList();
            }

            return View(products);
        }

        // =========================================================================
        // 2. THÊM MỚI SẢN PHẨM (CREATE GET & POST)
        // =========================================================================
        [HttpGet]
        public IActionResult Create()
        {
            var model = new SanPhamCrudViewModel
            {
                GiaBan = 0,
                PhanTramDongGop = 100,
                SoLuongTon = 50,
                TrangThai = true,
                DanhMuc = "Quà lưu niệm",
                ThuTu = 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamCrudViewModel model)
        {
            // Kiểm tra định dạng tệp tải lên nếu có
            if (model.FileHinhAnh != null && model.FileHinhAnh.Length > 0)
            {
                var ext = Path.GetExtension(model.FileHinhAnh.FileName).ToLowerInvariant();
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };
                if (!allowedExts.Contains(ext))
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), "Định dạng hình ảnh không hợp lệ. Chỉ chấp nhận các định dạng .jpg, .jpeg, .png, .webp, .gif, .svg.");
                }
                else if (model.FileHinhAnh.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), "Dung lượng tệp hình ảnh không được vượt quá 10MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? uploadedImagePath = null;
            if (model.FileHinhAnh != null && model.FileHinhAnh.Length > 0)
            {
                try
                {
                    uploadedImagePath = await SaveFileAsync(model.FileHinhAnh, "sanpham");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), $"Không thể lưu tệp hình ảnh: {ex.Message}");
                    return View(model);
                }
            }

            var entity = model.ToEntity(uploadedImagePath);
            bool dbSaved = false;

            try
            {
                if (_context.Database.CanConnect())
                {
                    _context.SanPhamGayQuys.Add(entity);
                    await _context.SaveChangesAsync();
                    dbSaved = true;
                }
            }
            catch
            {
                dbSaved = false;
            }

            // Nếu DB offline hoặc thất bại, đồng bộ vào In-Memory Store
            if (!dbSaved)
            {
                DbInitializer.AddInMemoryProduct(entity);
            }

            TempData["Success"] = $"Đã thêm mới sản phẩm \"{entity.TenSanPham}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 3. CHỈNH SỬA SẢN PHẨM (EDIT GET & POST)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Mã sản phẩm không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            SanPhamGayQuy? entity = null;

            try
            {
                if (_context.Database.CanConnect())
                {
                    entity = await _context.SanPhamGayQuys.FindAsync(id);
                }
            }
            catch
            {
                entity = null;
            }

            // Fallback sang In-Memory Store nếu không tìm thấy trong DB
            if (entity == null)
            {
                entity = DbInitializer.GetInMemoryProductById(id);
            }

            if (entity == null)
            {
                TempData["Error"] = $"Không tìm thấy sản phẩm có mã #{id}.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = SanPhamCrudViewModel.FromEntity(entity);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPhamCrudViewModel model)
        {
            if (id != model.IDSanPham)
            {
                TempData["Error"] = "Mã sản phẩm không hợp lệ hoặc đã bị thay đổi.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra tệp tải lên nếu có
            if (model.FileHinhAnh != null && model.FileHinhAnh.Length > 0)
            {
                var ext = Path.GetExtension(model.FileHinhAnh.FileName).ToLowerInvariant();
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };
                if (!allowedExts.Contains(ext))
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), "Định dạng hình ảnh không hợp lệ. Chỉ chấp nhận các định dạng .jpg, .jpeg, .png, .webp, .gif, .svg.");
                }
                else if (model.FileHinhAnh.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), "Dung lượng tệp hình ảnh không được vượt quá 10MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? uploadedImagePath = null;
            if (model.FileHinhAnh != null && model.FileHinhAnh.Length > 0)
            {
                try
                {
                    uploadedImagePath = await SaveFileAsync(model.FileHinhAnh, "sanpham");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(model.FileHinhAnh), $"Không thể lưu tệp hình ảnh mới: {ex.Message}");
                    return View(model);
                }
            }

            bool dbUpdated = false;

            try
            {
                if (_context.Database.CanConnect())
                {
                    var existing = await _context.SanPhamGayQuys.FindAsync(id);
                    if (existing != null)
                    {
                        model.UpdateEntity(existing, uploadedImagePath);
                        await _context.SaveChangesAsync();
                        dbUpdated = true;
                    }
                }
            }
            catch
            {
                dbUpdated = false;
            }

            // Luôn đồng bộ / fallback cập nhật trên In-Memory Store
            var inMemItem = DbInitializer.GetInMemoryProductById(id);
            if (inMemItem != null)
            {
                model.UpdateEntity(inMemItem, uploadedImagePath);
                DbInitializer.UpdateInMemoryProduct(inMemItem);
            }
            else if (!dbUpdated)
            {
                TempData["Error"] = $"Không tìm thấy sản phẩm #{id} để cập nhật.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = $"Đã cập nhật thông tin sản phẩm \"{model.TenSanPham}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 4. XÓA SẢN PHẨM (DELETE POST)
        // =========================================================================
        /// <summary>
        /// Xóa sản phẩm theo ID. Chỉ chấp nhận phương thức POST có kèm Anti-Forgery Token.
        /// Chú ý: Giữ duy nhất 1 phương thức tên Delete(int id) để tương thích phản chiếu GetMethod("Delete").
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Mã sản phẩm không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            string productName = "";
            bool dbDeleted = false;

            try
            {
                if (_context.Database.CanConnect())
                {
                    var entity = await _context.SanPhamGayQuys
                        .Include(p => p.ChiTietDonHangs)
                        .FirstOrDefaultAsync(p => p.IDSanPham == id);

                    if (entity != null)
                    {
                        productName = entity.TenSanPham;

                        // Kiểm tra ràng buộc khóa ngoại: nếu sản phẩm đã phát sinh chi tiết đơn hàng
                        if (entity.ChiTietDonHangs != null && entity.ChiTietDonHangs.Any())
                        {
                            // Chuyển sang ẩn sản phẩm để không vi phạm ràng buộc dữ liệu lịch sử đơn hàng
                            entity.TrangThai = false;
                            await _context.SaveChangesAsync();
                            dbDeleted = true;
                            TempData["Success"] = $"Sản phẩm \"{productName}\" đã có trong đơn hàng nên được chuyển sang trạng thái Ẩn để bảo toàn lịch sử giao dịch.";
                        }
                        else
                        {
                            _context.SanPhamGayQuys.Remove(entity);
                            await _context.SaveChangesAsync();
                            dbDeleted = true;
                        }
                    }
                }
            }
            catch (DbUpdateException)
            {
                // Xử lý an toàn khi xảy ra ràng buộc FK từ SQL Server
                try
                {
                    var entity = await _context.SanPhamGayQuys.FindAsync(id);
                    if (entity != null)
                    {
                        productName = entity.TenSanPham;
                        entity.TrangThai = false;
                        await _context.SaveChangesAsync();
                        dbDeleted = true;
                        TempData["Success"] = $"Sản phẩm \"{productName}\" đã được chuyển sang trạng thái Ẩn để bảo đảm tính toàn vẹn dữ liệu.";
                    }
                }
                catch
                {
                    dbDeleted = false;
                }
            }
            catch
            {
                dbDeleted = false;
            }

            // Đồng bộ với In-Memory Store
            var inMemItem = DbInitializer.GetInMemoryProductById(id);
            if (inMemItem != null)
            {
                if (string.IsNullOrEmpty(productName))
                {
                    productName = inMemItem.TenSanPham;
                }
                DbInitializer.DeleteInMemoryProduct(id);
            }

            if (!dbDeleted && inMemItem == null)
            {
                TempData["Error"] = $"Không tìm thấy sản phẩm #{id} hoặc sản phẩm không tồn tại.";
            }
            else if (TempData["Success"] == null)
            {
                TempData["Success"] = $"Đã xóa sản phẩm \"{(string.IsNullOrEmpty(productName) ? $"#{id}" : productName)}\" thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 5. TIỆN ÍCH LƯU TỆP HÌNH ẢNH (UPLOAD HELPER)
        // =========================================================================
        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            var webRoot = _env?.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(_env?.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
            }

            string uploadsFolder = Path.Combine(webRoot, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string uniqueName = $"{Guid.NewGuid():N}{ext}";
            string filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{subFolder}/{uniqueName}";
        }
    }
}
