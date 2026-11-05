using Microsoft.AspNetCore.Mvc;
using WEB_TinhNguyen.MoHinh;
using System.Diagnostics;
using WEB_TinhNguyen.DuLieu;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace WEB_TinhNguyen.BoDieuKhien
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ==========================================
        // TRANG CHỦ
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var activeConfig = await _context.CauHinhTrangChus
                .Include(c => c.ChienDich)
                .Include(c => c.BieuNguTrangChus)
                .Include(c => c.ChuongTrinhDemNguocs)
                .Include(c => c.CauHinhAnhNois)
                .Include(c => c.ThuVienKhoanhKhacs)
                .Include(c => c.TinTucs)
                .Include(c => c.NhaTaiTros)
                .Include(c => c.CauHoiThuongGaps)
                .Where(c => c.ChienDich.TrangThaiHoatDong == true)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            // NẾU ADMIN ĐÃ TẮT HẾT CHIẾN DỊCH -> HIỂN THỊ TRANG "ĐANG CẬP NHẬT"
            if (activeConfig == null)
            {
                return View("ComingSoon");
            }

            return View(activeConfig);
        }

        // ==========================================
        // CÁC TRANG CON: NẾU KHÔNG CÓ CHIẾN DỊCH ACTIVE CŨNG HIỂN THỊ COMING SOON
        // ==========================================
        public async Task<IActionResult> ChuongTrinh(int? year)
        {
            ChienDich? cd = null;
            if (year.HasValue) cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.Nam == year.Value);
            else cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.TrangThaiHoatDong == true);

            // NẾU KHÔNG TÌM THẤY -> HIỂN THỊ COMING SOON
            if (cd == null) return View("ComingSoon");

            var config = await _context.CauHinhChuongTrinhs
                .Include(c => c.ChangChuongTrinhs).ThenInclude(ch => ch.ChiTietLoTrinhs)
                .FirstOrDefaultAsync(c => c.IDChienDich == cd.IDChienDich);

            ViewBag.Nam = cd.Nam;
            return View(config);
        }

        public async Task<IActionResult> GayQuy(int? year)
        {
            ChienDich? cd = null;
            if (year.HasValue) cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.Nam == year.Value);
            else cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.TrangThaiHoatDong == true);

            if (cd == null) return View("ComingSoon");

            var config = await _context.CauHinhGayQuys
                .Include(c => c.KenhDongGops).Include(c => c.NhaTaiTroGayQuys)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.IDChienDich == cd.IDChienDich);

            ViewBag.Nam = cd.Nam;
            return View(config ?? new CauHinhGayQuy());
        }

        public async Task<IActionResult> BanToChuc(int? year)
        {
            ChienDich? cd = null;
            if (year.HasValue) cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.Nam == year.Value);
            else cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.TrangThaiHoatDong == true);

            if (cd == null) return View("ComingSoon");

            var config = await _context.CauHinhBanToChucs
                .Include(c => c.ThanhViens)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.IDChienDich == cd.IDChienDich);

            ViewBag.Nam = cd.Nam;
            return View(config ?? new CauHinhBanToChuc());
        }

        public async Task<IActionResult> TinTuc(int? year)
        {
            ChienDich? cd = null;
            if (year.HasValue) cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.Nam == year.Value);
            else cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.TrangThaiHoatDong == true);

            if (cd == null) return View("ComingSoon");

            var config = await _context.CauHinhTinTucs
                .Include(c => c.BaiViets)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.IDChienDich == cd.IDChienDich);

            ViewBag.Nam = cd.Nam;
            return View(config ?? new CauHinhTinTuc());
        }

        // ==========================================
        // CÁC TRANG VÀ API KHÁC GIỮ NGUYÊN
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> LikeBaiViet(int id)
        {
            var post = await _context.BaiVietTinTucs.FindAsync(id);
            if (post == null) return NotFound();

            post.LuotThich += 1;
            await _context.SaveChangesAsync();

            return Json(new { success = true, likes = post.LuotThich });
        }

        public IActionResult LoiChuc() => View();

        public async Task<IActionResult> KyUc(int year)
        {
            var cd = await _context.ChienDichs.FirstOrDefaultAsync(c => c.Nam == year);
            if (cd == null) return View("ComingSoon");

            var config = await _context.CauHinhKyUcs
                .Include(c => c.AnhKyUcs)
                .Include(c => c.NhaTaiTroKyUcs)
                .FirstOrDefaultAsync(c => c.IDChienDich == cd.IDChienDich);

            ViewBag.Nam = year;
            return View(config);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}