using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class AdminKyUcController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminKyUcController(ApplicationDbContext context, IWebHostEnvironment env)
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

        // Danh sách các năm để Edit
        public async Task<IActionResult> Index()
        {
            var chienDichs = await _context.ChienDichs.OrderByDescending(c => c.Nam).ToListAsync();
            return View(chienDichs);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) // id ở đây là IDChienDich
        {
            var cd = await _context.ChienDichs.FindAsync(id);
            if (cd == null) return NotFound();

            var cauHinh = await _context.CauHinhKyUcs
                .Include(c => c.AnhKyUcs)
                .Include(c => c.NhaTaiTroKyUcs)
                .FirstOrDefaultAsync(c => c.IDChienDich == id);

            if (cauHinh == null)
            {
                cauHinh = new CauHinhKyUc { IDChienDich = id };
                _context.CauHinhKyUcs.Add(cauHinh);
                await _context.SaveChangesAsync();
            }

            var vm = new CauHinhKyUcVM
            {
                IDCauHinhKyUc = cauHinh.IDCauHinhKyUc,
                IDChienDich = cauHinh.IDChienDich,
                NamChienDich = cd.Nam.ToString(),
                TieuDeChinh = cauHinh.TieuDeChinh,
                DoanVanMoTa = cauHinh.DoanVanMoTa,
                NoiDungTongKetHtml = cauHinh.NoiDungTongKetHtml,
                FontTieuDe = cauHinh.KieuChuTieuDe,
                LinkAnhBiaCu = cauHinh.DuongDanAnhBia,
                TieuDeTongKet = cauHinh.TieuDeTongKet ?? ("Trăng Sáng Langbiang " + cd.Nam),
                FontTieuDeTongKet = cauHinh.KieuChuTieuDeTongKet ?? "Be Vietnam Pro",

                // DÙNG TOÁN TỬ ? ĐỂ CHỐNG LỖI NULL
                ThuVienAnhs = cauHinh.AnhKyUcs?.Select(a => new AnhKyUcVM
                {
                    LinkAnhCu = a.LinkAnh,
                    TieuDeAnh = a.TieuDeAnh,
                    LaAnhDoc = a.LaAnhDoc
                }).ToList() ?? new List<AnhKyUcVM>(),

                NhaTaiTros = cauHinh.NhaTaiTroKyUcs?.Select(n => new NhaTaiTroKyUcVM
                {
                    TenDonVi = n.TenDonVi,
                    PhanLoai = n.PhanLoai,
                    LinkLogoCu = n.LinkLogo
                }).ToList() ?? new List<NhaTaiTroKyUcVM>()
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinhKyUcVM vm)
        {
            // Bỏ qua validate các trường file và trường text không bắt buộc
            var keys = ModelState.Keys.ToList();
            foreach (var key in keys) { ModelState.Remove(key); }

            var cauHinh = await _context.CauHinhKyUcs.FindAsync(vm.IDCauHinhKyUc);
            if (cauHinh == null) return NotFound();

            cauHinh.TieuDeChinh = vm.TieuDeChinh;
            cauHinh.DoanVanMoTa = vm.DoanVanMoTa;
            cauHinh.NoiDungTongKetHtml = vm.NoiDungTongKetHtml;
            // Thêm 3 dòng này để lưu vào Database
            cauHinh.TieuDeTongKet = vm.TieuDeTongKet;
            cauHinh.FontTieuDeTongKet = vm.FontTieuDeTongKet;
            cauHinh.FontTieuDe = vm.FontTieuDe;
            cauHinh.LinkAnhBia = vm.FileAnhBia != null ? await UploadFileAsync(vm.FileAnhBia) : vm.LinkAnhBiaCu;

            // Xóa dữ liệu cũ để thay mới
            _context.AnhKyUcs.RemoveRange(_context.AnhKyUcs.Where(x => x.IDCauHinhKyUc == vm.IDCauHinhKyUc));
            _context.NhaTaiTroKyUcs.RemoveRange(_context.NhaTaiTroKyUcs.Where(x => x.IDCauHinhKyUc == vm.IDCauHinhKyUc));
            await _context.SaveChangesAsync();

            // Lưu dữ liệu mới (chỉ thêm khi list không null)
            if (vm.ThuVienAnhs != null && vm.ThuVienAnhs.Any())
            {
                // Biến đếm thứ tự hiển thị
                int thuTu = 0;
                foreach (var item in vm.ThuVienAnhs)
                {
                    string img = item.FileAnh != null ? await UploadFileAsync(item.FileAnh) : item.LinkAnhCu;
                    if (!string.IsNullOrEmpty(img))
                    {
                        _context.AnhKyUcs.Add(new AnhKyUc
                        {
                            IDCauHinhKyUc = vm.IDCauHinhKyUc,
                            LinkAnh = img,
                            TieuDeAnh = item.TieuDeAnh,
                            LaAnhDoc = item.LaAnhDoc,
                            ThuTu = thuTu++
                        });
                    }
                }
            }

            if (vm.NhaTaiTros != null && vm.NhaTaiTros.Any())
            {
                foreach (var item in vm.NhaTaiTros)
                {
                    string logo = item.FileLogo != null ? await UploadFileAsync(item.FileLogo) : item.LinkLogoCu;
                    if (!string.IsNullOrEmpty(logo))
                    {
                        _context.NhaTaiTroKyUcs.Add(new NhaTaiTroKyUc
                        {
                            IDCauHinhKyUc = vm.IDCauHinhKyUc,
                            LinkLogo = logo,
                            TenDonVi = item.TenDonVi,
                            PhanLoai = item.PhanLoai
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã lưu giao diện Ký ức thành công!";
            return RedirectToAction("Index");
        }
    }
}