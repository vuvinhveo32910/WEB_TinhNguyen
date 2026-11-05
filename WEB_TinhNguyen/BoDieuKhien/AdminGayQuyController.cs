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
    public class AdminGayQuyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminGayQuyController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. DANH SÁCH CHIẾN DỊCH THEO NĂM
        public async Task<IActionResult> Index()
        {
            var list = await _context.ChienDichs.OrderByDescending(c => c.Nam).ToListAsync();
            return View(list);
        }

        // 2. MỞ TRANG CẤU HÌNH (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var chienDich = await _context.ChienDichs.FindAsync(id);
            if (chienDich == null) return NotFound();

            var config = await _context.CauHinhGayQuys
                .Include(c => c.KenhDongGops)
                .Include(c => c.NhaTaiTroGayQuys)
                .FirstOrDefaultAsync(c => c.IDChienDich == id);

            // Nếu chưa có cấu hình cho năm này thì khởi tạo dữ liệu mẫu mặc định giống hệt trang gốc
            if (config == null)
            {
                config = new CauHinhGayQuy
                {
                    IDChienDich = id,
                    Hero_LinkAnh = "/gallery/2025/bro02087.jpg",
                    Hero_MoTa = "Có nhiều cách để bạn kề vai, sát cánh cùng dự án. Mọi sự ủng hộ thông qua các \"Gói yêu thương\", hiện kim, nhu yếu phẩm hay chỉ một lời chúc tốt đẹp đều là những mảnh ghép làm nên một mùa Trung thu diệu kỳ cho trẻ em nhỏ có hoàn cảnh khó khăn!",
                    BaoCao_LinkNut = "https://drive.google.com/drive/folders/1atXjIPlv9Q6k45q5CQ-s9joMiOGtI9YQ?usp=drive_link"
                };
                _context.CauHinhGayQuys.Add(config);
                await _context.SaveChangesAsync();

                // Thêm 2 kênh đóng góp mặc định
                _context.KenhDongGops.AddRange(
                    new KenhDongGop
                    {
                        IDCauHinhGQ = config.IDCauHinhGQ,
                        TenKenh = "Quyên góp trực tiếp",
                        MoTa = $"BTC chỉ sử dụng DUY NHẤT một tài khoản này để tiếp nhận quyên góp trực tiếp từ 10/8/{chienDich.Nam} đến hết 25/9/{chienDich.Nam}",
                        LoaiIcon = "bank",
                        TextNut = "QR Tài khoản",
                        LoaiHanhDong = "qr",
                        TenNganHang = "MBBank (Ngân hàng Quân Đội)",
                        SoTaiKhoan = "0924044283",
                        TenChuTaiKhoan = "NGUYỄN ANH HÀO",
                        NoiDungChuyenKhoan = "[Họ và tên]_[Đơn vị]_gopLangbiang",
                        LinkAnhQR = "https://ik.imagekit.io/ij9ocagrc/kho/QRCK-A.Ha-o-636KHTNdal43FQMQOVKE0khAuEiX4h_OMj08ZkCK.webp",
                        ThuTu = 1
                    },
                    new KenhDongGop
                    {
                        IDCauHinhGQ = config.IDCauHinhGQ,
                        TenKenh = "Quyên góp hiện vật",
                        MoTa = "Quà Trung thu, nhu yếu phẩm,... Vui lòng liên hệ Zalo 0827.564.956 (Anh Phạm Minh Phát - đại diện Ban sáng lập Dự án) hoặc Fanpage",
                        LoaiIcon = "gift",
                        TextNut = "Liên hệ Fanpage",
                        LoaiHanhDong = "link",
                        LinkNgoai = "https://www.facebook.com/profile.php?id=61580211752903",
                        ThuTu = 2
                    }
                );
                await _context.SaveChangesAsync();

                // Load lại
                config = await _context.CauHinhGayQuys
                    .Include(c => c.KenhDongGops)
                    .Include(c => c.NhaTaiTroGayQuys)
                    .FirstOrDefaultAsync(c => c.IDChienDich == id);
            }

            var vm = new CauHinhGayQuyVM
            {
                IDCauHinhGQ = config.IDCauHinhGQ,
                IDChienDich = config.IDChienDich,
                TenChienDich = chienDich.TieuDe,
                NamChienDich = chienDich.Nam,

                FontTieuDe = config.KieuChuTieuDe,
                FontNoiDung = config.KieuChuNoiDung,

                Hero_Nhan = config.NoiBat_Nhan,
                Hero_TieuDe = config.NoiBat_TieuDe,
                Hero_CoChuTieuDe = config.NoiBat_CoChuTieuDe,
                Hero_MoTa = config.NoiBat_MoTa,
                Hero_TextNut = config.NoiBat_ChuNut,
                Hero_LinkNut = config.NoiBat_DuongDanNut,
                Hero_LinkAnhCu = config.NoiBat_DuongDanAnh,

                DongGop_Nhan = config.DongGop_Nhan,
                DongGop_TieuDe = config.DongGop_TieuDe,
                DongGop_MoTa = config.DongGop_MoTa,
                DongGop_GhiChuChanTrang = config.DongGop_GhiChuChanTrang,

                BaoCao_Nhan = config.BaoCao_Nhan,
                BaoCao_TieuDe = config.BaoCao_TieuDe,
                BaoCao_MoTa = config.BaoCao_MoTa,
                BaoCao_TextNut = config.BaoCao_ChuNut,
                BaoCao_LinkNut = config.BaoCao_DuongDanNut,

                TaiTro_Nhan = config.TaiTro_Nhan,
                TaiTro_TieuDe = config.TaiTro_TieuDe,
                TaiTro_TieuDeNoiBat = config.TaiTro_TieuDeNoiBat,
                TaiTro_MoTa = config.TaiTro_MoTa,

                KenhDongGops = config.KenhDongGops?.OrderBy(k => k.ThuTu).Select(k => new KenhDongGopVM
                {
                    TenKenh = k.TenKenh,
                    MoTa = k.MoTa,
                    LoaiIcon = k.LoaiBieuTuong,
                    TextNut = k.ChuNut,
                    LoaiHanhDong = k.LoaiHanhDong,
                    LinkNgoai = k.DuongDanNgoai,
                    LinkAnhQRCu = k.DuongDanAnhQR,
                    TenNganHang = k.TenNganHang,
                    SoTaiKhoan = k.SoTaiKhoan,
                    TenChuTaiKhoan = k.TenChuTaiKhoan,
                    NoiDungChuyenKhoan = k.NoiDungChuyenKhoan,
                    GhiChuThem = k.GhiChuThem
                }).ToList() ?? new(),

                NhaTaiTros = config.NhaTaiTroGayQuys?.OrderBy(n => n.ThuTu).Select(n => new NhaTaiTroGayQuyVM
                {
                    NhomTaiTro = n.NhomTaiTro,
                    TenDonVi = n.TenDonVi,
                    LinkLogoCu = n.DuongDanLogo,
                    LinkWebsite = n.DuongDanTrangWeb,
                    GioiThieu = n.GioiThieu
                }).ToList() ?? new()
            };

            return View(vm);
        }

        // 3. LƯU CẤU HÌNH (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinhGayQuyVM vm)
        {
            var config = await _context.CauHinhGayQuys
                .Include(c => c.KenhDongGops)
                .Include(c => c.NhaTaiTroGayQuys)
                .FirstOrDefaultAsync(c => c.IDCauHinhGQ == vm.IDCauHinhGQ);

            if (config == null) return NotFound();

            // Cập nhật thông tin chung & Hero
            config.KieuChuTieuDe = vm.FontTieuDe;
            config.KieuChuNoiDung = vm.FontNoiDung;

            config.NoiBat_Nhan = vm.Hero_Nhan;
            config.NoiBat_TieuDe = vm.Hero_TieuDe;
            config.NoiBat_CoChuTieuDe = vm.Hero_CoChuTieuDe;
            config.NoiBat_MoTa = vm.Hero_MoTa;
            config.NoiBat_ChuNut = vm.Hero_TextNut;
            config.NoiBat_DuongDanNut = vm.Hero_LinkNut;

            if (vm.Hero_FileAnh != null)
                config.NoiBat_DuongDanAnh = await SaveFileAsync(vm.Hero_FileAnh, "gayquy");
            else
                config.NoiBat_DuongDanAnh = vm.Hero_LinkAnhCu;

            // Cập nhật Section 2: Cách thức đóng góp
            config.DongGop_Nhan = vm.DongGop_Nhan;
            config.DongGop_TieuDe = vm.DongGop_TieuDe;
            config.DongGop_MoTa = vm.DongGop_MoTa;
            config.DongGop_GhiChuChanTrang = vm.DongGop_GhiChuChanTrang;

            // Cập nhật Section 3: Báo cáo thu chi
            config.BaoCao_Nhan = vm.BaoCao_Nhan;
            config.BaoCao_TieuDe = vm.BaoCao_TieuDe;
            config.BaoCao_MoTa = vm.BaoCao_MoTa;
            config.BaoCao_ChuNut = vm.BaoCao_TextNut;
            config.BaoCao_DuongDanNut = vm.BaoCao_LinkNut;

            // Cập nhật Section 4: Đơn vị tài trợ
            config.TaiTro_Nhan = vm.TaiTro_Nhan;
            config.TaiTro_TieuDe = vm.TaiTro_TieuDe;
            config.TaiTro_TieuDeNoiBat = vm.TaiTro_TieuDeNoiBat;
            config.TaiTro_MoTa = vm.TaiTro_MoTa;

            // Xử lý danh sách Kênh Đóng Góp (Repeater)
            _context.KenhDongGops.RemoveRange(config.KenhDongGops);
            if (vm.KenhDongGops != null)
            {
                int order = 1;
                foreach (var item in vm.KenhDongGops)
                {
                    if (string.IsNullOrWhiteSpace(item.TenKenh)) continue;

                    string qrLink = item.LinkAnhQRCu;
                    if (item.FileAnhQR != null)
                        qrLink = await SaveFileAsync(item.FileAnhQR, "gayquy_qr");

                    _context.KenhDongGops.Add(new KenhDongGop
                    {
                        IDCauHinhGQ = config.IDCauHinhGQ,
                        TenKenh = item.TenKenh,
                        MoTa = item.MoTa,
                        LoaiBieuTuong = item.LoaiIcon,
                        ChuNut = item.TextNut,
                        LoaiHanhDong = item.LoaiHanhDong,
                        DuongDanNgoai = item.LinkNgoai,
                        DuongDanAnhQR = qrLink,
                        TenNganHang = item.TenNganHang,
                        SoTaiKhoan = item.SoTaiKhoan,
                        TenChuTaiKhoan = item.TenChuTaiKhoan,
                        NoiDungChuyenKhoan = item.NoiDungChuyenKhoan,
                        GhiChuThem = item.GhiChuThem,
                        ThuTu = order++
                    });
                }
            }

            // Xử lý danh sách Nhà Tài Trợ (Repeater)
            _context.NhaTaiTroGayQuys.RemoveRange(config.NhaTaiTroGayQuys);
            if (vm.NhaTaiTros != null)
            {
                int order = 1;
                foreach (var item in vm.NhaTaiTros)
                {
                    if (string.IsNullOrWhiteSpace(item.TenDonVi)) continue;

                    string logoLink = item.LinkLogoCu;
                    if (item.FileLogo != null)
                        logoLink = await SaveFileAsync(item.FileLogo, "sponsors");

                    _context.NhaTaiTroGayQuys.Add(new NhaTaiTroGayQuy
                    {
                        IDCauHinhGQ = config.IDCauHinhGQ,
                        NhomTaiTro = string.IsNullOrWhiteSpace(item.NhomTaiTro) ? "Đơn vị đồng hành" : item.NhomTaiTro.Trim(),
                        TenDonVi = item.TenDonVi,
                        LinkLogo = logoLink,
                        LinkWebsite = item.LinkWebsite,
                        GioiThieu = item.GioiThieu,
                        ThuTu = order++
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã lưu cấu hình trang Đóng góp - Gây quỹ cho chiến dịch {vm.TenChienDich} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // HÀM HỖ TRỢ LƯU FILE ẢNH
        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string ext = Path.GetExtension(file.FileName);
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