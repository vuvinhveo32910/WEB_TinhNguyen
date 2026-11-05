using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class AdminHieuUngController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminHieuUngController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. DANH SÁCH HIỆU ỨNG
        public async Task<IActionResult> Index()
        {
            var list = await _context.ThuVienHieuUngs.ToListAsync();
            return View(list);
        }

        // 2. THÊM / SỬA HIỆU ỨNG (Lưu file JS)
        [HttpPost]
        public async Task<IActionResult> Save(int IDHieuUng, string TenHieuUng, string MoTa, IFormFile FileJS)
        {
            var hieuUng = await _context.ThuVienHieuUngs.FindAsync(IDHieuUng);
            if (hieuUng == null)
            {
                hieuUng = new ThuVienHieuUng();
                _context.ThuVienHieuUngs.Add(hieuUng);
            }

            hieuUng.TenHieuUng = TenHieuUng;
            hieuUng.MoTa = MoTa;

            // Xử lý upload file JS
            if (FileJS != null && FileJS.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "effects");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + FileJS.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FileJS.CopyToAsync(fileStream);
                }
                hieuUng.LinkFileJS = "/effects/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã lưu hiệu ứng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 3. XÓA HIỆU ỨNG
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var hieuUng = await _context.ThuVienHieuUngs.FindAsync(id);
            if (hieuUng != null)
            {
                // Nếu file còn tồn tại trên server thì xóa luôn file cho nhẹ máy
                if (!string.IsNullOrEmpty(hieuUng.LinkFileJS))
                {
                    string filePath = Path.Combine(_env.WebRootPath, hieuUng.LinkFileJS.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _context.ThuVienHieuUngs.Remove(hieuUng);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa hiệu ứng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}