using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class AdminChuongTrinhController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminChuongTrinhController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;
            string uploadsFolder = Path.Combine(_env.WebRootPath, "gallery");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/gallery/" + uniqueFileName;
        }

        public async Task<IActionResult> Index()
        {
            var chienDichs = await _context.ChienDichs.OrderByDescending(c => c.Nam).ToListAsync();
            return View(chienDichs);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cd = await _context.ChienDichs.FindAsync(id);
            if (cd == null) return NotFound();

            // QUAN TRỌNG: Phải Include sâu vào ChiTietLoTrinhs
            var cauHinh = await _context.CauHinhChuongTrinhs
                .Include(c => c.ChangChuongTrinhs)
                    .ThenInclude(ch => ch.ChiTietLoTrinhs)
                .FirstOrDefaultAsync(c => c.IDChienDich == id);

            if (cauHinh == null)
            {
                cauHinh = new CauHinhChuongTrinh { IDChienDich = id };
                _context.CauHinhChuongTrinhs.Add(cauHinh);
                await _context.SaveChangesAsync();
            }

            var vm = new CauHinhChuongTrinhVM
            {
                IDCauHinhCT = cauHinh.IDCauHinhCT,
                IDChienDich = cauHinh.IDChienDich,
                NamChienDich = cd.Nam.ToString(),
                TieuDeChinh = cauHinh.TieuDeChinh,
                TieuDeNhanManh = cauHinh.TieuDeNhanManh,
                DoanVanMoTa = cauHinh.DoanVanMoTa,
                LinkAnhHero1Cu = cauHinh.LinkAnhHero1,
                FontTieuDe = cauHinh.KieuChuTieuDe,

                // MAPPING DỮ LIỆU CON
                Changs = cauHinh.ChangChuongTrinhs?.OrderBy(x => x.ThuTu).Select(c => new ChangChuongTrinhVM
                {
                    NhanThoiGian = c.NhanThoiGian,
                    TieuDeChang = c.TieuDeChang,
                    MoTaChang = c.MoTaChang,
                    DiaDiem = c.DiaDiem,
                    LinkAnhCu = c.LinkAnh,

                    // NẾU BỊ NULL, SẼ TRẢ VỀ LIST RỖNG
                    LoTrinhs = c.ChiTietLoTrinhs?.OrderBy(l => l.ThuTu).Select(l => new ChiTietLoTrinhVM
                    {
                        ThoiGian = l.ThoiGian,
                        NoiDungHoatDong = l.NoiDungHoatDong
                    }).ToList() ?? new List<ChiTietLoTrinhVM>()
                }).ToList() ?? new List<ChangChuongTrinhVM>()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinhChuongTrinhVM vm)
        {
            var keys = ModelState.Keys.ToList();
            foreach (var key in keys) { ModelState.Remove(key); }

            var cauHinh = await _context.CauHinhChuongTrinhs.FindAsync(vm.IDCauHinhCT);
            if (cauHinh == null) return NotFound();

            cauHinh.TieuDeChinh = vm.TieuDeChinh;
            cauHinh.TieuDeNhanManh = vm.TieuDeNhanManh;
            cauHinh.DoanVanMoTa = vm.DoanVanMoTa;
            cauHinh.KieuChuTieuDe = vm.FontTieuDe;
            cauHinh.LinkAnhHero1 = vm.FileAnhHero1 != null ? await UploadFileAsync(vm.FileAnhHero1) : vm.LinkAnhHero1Cu;
            cauHinh.LinkAnhHero2 = vm.FileAnhHero2 != null ? await UploadFileAsync(vm.FileAnhHero2) : vm.LinkAnhHero2Cu;

            // 1. Lấy danh sách ID các chặng cũ để xóa chi tiết lộ trình bên trong
            var oldChangIds = _context.ChangChuongTrinhs.Where(x => x.IDCauHinhCT == vm.IDCauHinhCT).Select(x => x.IDChang).ToList();

            // 2. Xóa tất cả ChiTietLoTrinh cũ thuộc các chặng này
            _context.ChiTietLoTrinhs.RemoveRange(_context.ChiTietLoTrinhs.Where(x => oldChangIds.Contains(x.IDChang)));

            // 3. Xóa các chặng cũ
            _context.ChangChuongTrinhs.RemoveRange(_context.ChangChuongTrinhs.Where(x => x.IDCauHinhCT == vm.IDCauHinhCT));

            await _context.SaveChangesAsync();

            // 4. Thêm chặng và lộ trình mới
            if (vm.Changs != null && vm.Changs.Any())
            {
                int sttChang = 0;
                foreach (var item in vm.Changs)
                {
                    string img = item.FileAnh != null ? await UploadFileAsync(item.FileAnh) : item.LinkAnhCu;
                    if (!string.IsNullOrEmpty(item.TieuDeChang))
                    {
                        var newChang = new ChangChuongTrinh
                        {
                            IDCauHinhCT = vm.IDCauHinhCT,
                            NhanThoiGian = item.NhanThoiGian,
                            TieuDeChang = item.TieuDeChang,
                            MoTaChang = item.MoTaChang,
                            DiaDiem = item.DiaDiem,
                            LinkAnh = img,
                            ThuTu = sttChang++
                        };

                        _context.ChangChuongTrinhs.Add(newChang);
                        await _context.SaveChangesAsync(); // Lưu để lấy IDChang mới sinh ra

                        // Thêm chi tiết lộ trình cho chặng này
                        if (item.LoTrinhs != null && item.LoTrinhs.Any())
                        {
                            int sttLoTrinh = 0;
                            foreach (var lotrinh in item.LoTrinhs)
                            {
                                if (!string.IsNullOrEmpty(lotrinh.ThoiGian))
                                {
                                    _context.ChiTietLoTrinhs.Add(new ChiTietLoTrinh
                                    {
                                        IDChang = newChang.IDChang,
                                        ThoiGian = lotrinh.ThoiGian,
                                        NoiDungHoatDong = lotrinh.NoiDungHoatDong,
                                        ThuTu = sttLoTrinh++
                                    });
                                }
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã lưu cấu hình Chương trình thành công!";
            return RedirectToAction("Index");
        }
    }
}