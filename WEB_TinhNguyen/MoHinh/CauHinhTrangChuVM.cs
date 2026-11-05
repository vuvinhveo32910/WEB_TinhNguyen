using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace WEB_TinhNguyen.MoHinh
{
    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE TRANG CHỦ
    // =========================================================================
    public class CauHinhTrangChuVM
    {
        public int IDCauHinh { get; set; }
        public int IDChienDich { get; set; }
        public string? TenChienDich { get; set; }

        public int TocDoChuyenSlide { get; set; } = 4000;

        public string? TieuDePhu { get; set; }
        public string? TieuDeChinh { get; set; }
        public string? DoanVanMoTa { get; set; }

        public string? KieuChuTieuDe { get; set; } = "Dancing Script";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }
        public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";
        public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }

        public bool HienThiHieuUng3D { get; set; } = true;
        public bool HienThiCanvas3D { get => HienThiHieuUng3D; set => HienThiHieuUng3D = value; }
        public bool HienThiLaRoi { get; set; } = true;

        public string? DuongDanAnhBiaCu { get; set; }
        public string? LinkAnhBiaCu { get => DuongDanAnhBiaCu; set => DuongDanAnhBiaCu = value; }
        public IFormFile? FileAnhBia { get; set; }

        public string? VeDuAn_TieuDe { get; set; }
        public string? VeDuAn_NoiDung { get; set; }
        public string? VeDuAn_DuongDanAnhCu { get; set; }
        public string? VeDuAn_LinkAnhCu { get => VeDuAn_DuongDanAnhCu; set => VeDuAn_DuongDanAnhCu = value; }
        public IFormFile? VeDuAn_FileAnh { get; set; }

        public int? IDHieuUng { get; set; }
        // --- LINH VẬT TRANG TRÍ ---
        public bool HienThiLinhVat { get; set; } = true;
        public bool HienThiMascot { get => HienThiLinhVat; set => HienThiLinhVat = value; }
        public string? DuongDanAnhLinhVatCu { get; set; }
        public string? LinkAnhMascotCu { get => DuongDanAnhLinhVatCu; set => DuongDanAnhLinhVatCu = value; }
        public IFormFile? FileAnhMascot { get; set; }
        public string? ViTriLinhVat { get; set; } = "right";
        public string? ViTriMascot { get => ViTriLinhVat; set => ViTriLinhVat = value; }

        public List<BieuNguTrangChuVM> BieuNgus { get; set; } = new List<BieuNguTrangChuVM>();
        public List<BieuNguTrangChuVM> Banners { get => BieuNgus; set => BieuNgus = value; }

        public List<ChuongTrinhDemNguocVM> ChuongTrinhs { get; set; } = new List<ChuongTrinhDemNguocVM>();
        public List<CauHinhAnhNoiVM> AnhNois { get; set; } = new List<CauHinhAnhNoiVM>();

        public List<ThuVienKhoanhKhacVM> ThuVienKhoanhKhacs { get; set; } = new List<ThuVienKhoanhKhacVM>();
        public List<ThuVienKhoanhKhacVM> TVKhoanhKhacs { get => ThuVienKhoanhKhacs; set => ThuVienKhoanhKhacs = value; }

        public List<TinTucTrangChuVM> TinTucs { get; set; } = new List<TinTucTrangChuVM>();
        public List<NhaTaiTroTrangChuVM> NhaTaiTros { get; set; } = new List<NhaTaiTroTrangChuVM>();

        public List<CauHoiThuongGapTrangChuVM> CauHoiThuongGaps { get; set; } = new List<CauHoiThuongGapTrangChuVM>();
        public List<CauHoiThuongGapTrangChuVM> FAQs { get => CauHoiThuongGaps; set => CauHoiThuongGaps = value; }
    }

    public class BieuNguTrangChuVM
    {
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? TieuDeThayThe { get; set; }
        public string? TieuDeAlt { get => TieuDeThayThe; set => TieuDeThayThe = value; }
    }
    public class BannerTrangChuVM : BieuNguTrangChuVM { }

    public class CauHinhAnhNoiVM
    {
        public int IDAnhNoi { get; set; }
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? TieuDeThayThe { get; set; }
        public string? TieuDeAlt { get => TieuDeThayThe; set => TieuDeThayThe = value; }
        public int ChieuSau { get; set; } = -50;
        public string? ViTriTren { get; set; }
        public string? ViTriTop { get => ViTriTren; set => ViTriTren = value; }
        public string? CanLe { get; set; } = "left";
        public string? KhoangCachLe { get; set; }
        public string? KichThuoc { get; set; }
        public string? DoTreHoatHoa { get; set; } = "0s";
        public string? DoTreAnimation { get => DoTreHoatHoa; set => DoTreHoatHoa = value; }
        public string? GocXoay { get; set; } = "0deg";
    }

    public class ThuVienKhoanhKhacVM
    {
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? TieuDe { get; set; }
        public string? MoTa { get; set; }
    }
    public class TVKhoanhKhacVM : ThuVienKhoanhKhacVM { }

    public class TinTucTrangChuVM
    {
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? TieuDe { get; set; }
        public string? TrichDan { get; set; }
        public string? ThePhanLoai { get; set; }
        public string? LinkChuyenHuong { get; set; }
        public int LuotThich { get; set; }
        public string? NoiDungChiTiet { get; set; }
        public string? KichCoTieuDe { get; set; } = "18px";
    }

    public class NhaTaiTroTrangChuVM
    {
        public string? DuongDanLogoCu { get; set; }
        public string? LinkLogoCu { get => DuongDanLogoCu; set => DuongDanLogoCu = value; }
        public IFormFile? FileLogo { get; set; }
        public string? NhomTaiTro { get; set; }
        public string? TenDonVi { get; set; }
    }

    public class CauHoiThuongGapTrangChuVM
    {
        public string? CauHoi { get; set; }
        public string? CauTraLoi { get; set; }
    }
    public class FAQTrangChuVM : CauHoiThuongGapTrangChuVM { }

    public class ChuongTrinhDemNguocVM
    {
        public string? TieuDePhu { get; set; }
        public string? TieuDeChinh { get; set; }
        public string? MoTa { get; set; }
        public string? ThoiGianHienThi { get; set; }
        public DateTime ThoiGianBatDau { get; set; } = DateTime.Now;
        public DateTime ThoiGianKetThuc { get; set; } = DateTime.Now.AddDays(1);
        public bool EpKetThuc { get; set; }
    }

    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE KÝ ỨC
    // =========================================================================

    public class CauHinhKyUcVM
    {
        public int IDCauHinhKyUc { get; set; }
        public int IDChienDich { get; set; }
        public string? NamChienDich { get; set; }

        public string? TieuDeChinh { get; set; }
        public string? DoanVanMoTa { get; set; }
        public string? NoiDungTongKetHtml { get; set; }

        public string? KieuChuTieuDe { get; set; } = "Dancing Script";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }

        public string? DuongDanAnhBiaCu { get; set; }
        public string? LinkAnhBiaCu { get => DuongDanAnhBiaCu; set => DuongDanAnhBiaCu = value; }
        public IFormFile? FileAnhBia { get; set; }
        public string? TieuDeTongKet { get; set; }
        public string? KieuChuTieuDeTongKet { get; set; }
        public string? FontTieuDeTongKet { get => KieuChuTieuDeTongKet; set => KieuChuTieuDeTongKet = value; }

        public List<AnhKyUcVM> ThuVienAnhs { get; set; } = new List<AnhKyUcVM>();
        public List<NhaTaiTroKyUcVM> NhaTaiTros { get; set; } = new List<NhaTaiTroKyUcVM>();
    }

    public class AnhKyUcVM
    {
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? TieuDeAnh { get; set; }
        public bool LaAnhDoc { get; set; }
    }

    public class NhaTaiTroKyUcVM
    {
        public string? DuongDanLogoCu { get; set; }
        public string? LinkLogoCu { get => DuongDanLogoCu; set => DuongDanLogoCu = value; }
        public IFormFile? FileLogo { get; set; }
        public string? TenDonVi { get; set; }
        public string? PhanLoai { get; set; }
    }

    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE CHƯƠNG TRÌNH
    // =========================================================================

    public class CauHinhChuongTrinhVM
    {
        public int IDCauHinhCT { get; set; }
        public int IDChienDich { get; set; }
        public string? NamChienDich { get; set; }

        public string? TieuDeChinh { get; set; }
        public string? TieuDeNhanManh { get; set; }
        public string? DoanVanMoTa { get; set; }

        public string? KieuChuTieuDe { get; set; } = "Be Vietnam Pro";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }

        public string? DuongDanAnhHero1Cu { get; set; }
        public string? LinkAnhHero1Cu { get => DuongDanAnhHero1Cu; set => DuongDanAnhHero1Cu = value; }
        public IFormFile? FileAnhHero1 { get; set; }
        public string? DuongDanAnhHero2Cu { get; set; }
        public string? LinkAnhHero2Cu { get => DuongDanAnhHero2Cu; set => DuongDanAnhHero2Cu = value; }
        public IFormFile? FileAnhHero2 { get; set; }

        public List<ChangChuongTrinhVM> Changs { get; set; } = new List<ChangChuongTrinhVM>();
    }

    public class ChangChuongTrinhVM
    {
        public string? NhanThoiGian { get; set; }
        public string? TieuDeChang { get; set; }
        public string? MoTaChang { get; set; }
        public string? DiaDiem { get; set; }
        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public List<ChiTietLoTrinhVM> LoTrinhs { get; set; } = new List<ChiTietLoTrinhVM>();
    }

    public class ChiTietLoTrinhVM
    {
        public string? ThoiGian { get; set; }
        public string? NoiDungHoatDong { get; set; }
    }

    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE ĐÓNG GÓP / GÂY QUỸ (GAY QUY)
    // =========================================================================
    public class CauHinhGayQuyVM
    {
        public int IDCauHinhGQ { get; set; }
        public int IDChienDich { get; set; }
        public string? TenChienDich { get; set; }
        public int NamChienDich { get; set; }

        // --- CẤU HÌNH CHUNG ---
        public string? KieuChuTieuDe { get; set; } = "Be Vietnam Pro";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }
        public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";
        public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }

        // --- SECTION 1: HERO (NỔI BẬT) ---
        public string? NoiBat_Nhan { get; set; } = "KHÔNG GIAN YÊU THƯƠNG";
        public string? Hero_Nhan { get => NoiBat_Nhan; set => NoiBat_Nhan = value; }

        public string? NoiBat_TieuDe { get; set; } = "HÌNH THỨC TRAO GỬI YÊU THƯƠNG";
        public string? Hero_TieuDe { get => NoiBat_TieuDe; set => NoiBat_TieuDe = value; }
        public string? NoiBat_CoChuTieuDe { get; set; } = "48px";
        public string? Hero_CoChuTieuDe { get => NoiBat_CoChuTieuDe; set => NoiBat_CoChuTieuDe = value; }

        public string? NoiBat_MoTa { get; set; } = "Có nhiều cách để bạn kề vai, sát cánh cùng dự án. Mọi sự ủng hộ thông qua các \"Gói yêu thương\", hiện kim, nhu yếu phẩm hay chỉ một lời chúc tốt đẹp đều là những mảnh ghép làm nên một mùa Trung thu diệu kỳ cho trẻ em nhỏ có hoàn cảnh khó khăn!";
        public string? Hero_MoTa { get => NoiBat_MoTa; set => NoiBat_MoTa = value; }

        public string? NoiBat_ChuNut { get; set; } = "Đóng góp ngay";
        public string? Hero_TextNut { get => NoiBat_ChuNut; set => NoiBat_ChuNut = value; }
        public string? NoiBat_DuongDanNut { get; set; } = "#ways-to-contribute";
        public string? Hero_LinkNut { get => NoiBat_DuongDanNut; set => NoiBat_DuongDanNut = value; }

        public string? NoiBat_DuongDanAnhCu { get; set; }
        public string? Hero_LinkAnhCu { get => NoiBat_DuongDanAnhCu; set => NoiBat_DuongDanAnhCu = value; }
        public IFormFile? Hero_FileAnh { get; set; }

        // --- SECTION 2: CÁCH THỨC ĐÓNG GÓP ---
        public string? DongGop_Nhan { get; set; } = "Kênh ủng hộ";
        public string? DongGop_TieuDe { get; set; } = "Cách thức đóng góp";
        public string? DongGop_MoTa { get; set; } = "Cùng chung tay lan tỏa yêu thương bằng phương thức phù hợp nhất với bạn.";
        public string? DongGop_GhiChuChanTrang { get; set; } = "Mọi đóng góp đều được ghi nhận và công khai minh bạch trên Fanpage của dự án.";

        // --- SECTION 3: BÁO CÁO THU CHI ---
        public string? BaoCao_Nhan { get; set; } = "Minh bạch";
        public string? BaoCao_TieuDe { get; set; } = "Báo cáo thu – chi";
        public string? BaoCao_MoTa { get; set; } = "Toàn bộ khoản thu – chi mùa 2026 được cập nhật công khai trên các kênh thông tin của Dự án.";
        public string? BaoCao_ChuNut { get; set; } = "Xem báo cáo chi tiết";
        public string? BaoCao_TextNut { get => BaoCao_ChuNut; set => BaoCao_ChuNut = value; }
        public string? BaoCao_DuongDanNut { get; set; } = "https://drive.google.com/drive/folders/1atXjIPlv9Q6k45q5CQ-s9joMiOGtI9YQ?usp=drive_link";
        public string? BaoCao_LinkNut { get => BaoCao_DuongDanNut; set => BaoCao_DuongDanNut = value; }

        // --- SECTION 4: ĐƠN VỊ TÀI TRỢ ---
        public string? TaiTro_Nhan { get; set; } = "Những mảnh ghép tạo nên Trăng sáng Langbiang 2026";
        public string? TaiTro_TieuDe { get; set; } = "ĐƠN VỊ TỔ CHỨC - TÀI TRỢ";
        public string? TaiTro_TieuDeNoiBat { get; set; } = "NĂM 2026";
        public string? TaiTro_MoTa { get; set; } = "Mỗi đơn vị là một mảnh ghép quan trọng, cùng nhau thắp sáng và làm nên một mùa trăng 2026 thật trọn vẹn!";

        public List<KenhDongGopVM> KenhDongGops { get; set; } = new List<KenhDongGopVM>();
        public List<NhaTaiTroGayQuyVM> NhaTaiTros { get; set; } = new List<NhaTaiTroGayQuyVM>();
    }

    public class KenhDongGopVM
    {
        public string? TenKenh { get; set; }
        public string? MoTa { get; set; }
        public string? LoaiBieuTuong { get; set; } = "bank";
        public string? LoaiIcon { get => LoaiBieuTuong; set => LoaiBieuTuong = value; }

        public string? ChuNut { get; set; } = "QR Tài khoản";
        public string? TextNut { get => ChuNut; set => ChuNut = value; }
        public string? LoaiHanhDong { get; set; } = "qr";
        public string? DuongDanNgoai { get; set; }
        public string? LinkNgoai { get => DuongDanNgoai; set => DuongDanNgoai = value; }

        public string? DuongDanAnhQRCu { get; set; }
        public string? LinkAnhQRCu { get => DuongDanAnhQRCu; set => DuongDanAnhQRCu = value; }
        public IFormFile? FileAnhQR { get; set; }
        public string? TenNganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTaiKhoan { get; set; }
        public string? NoiDungChuyenKhoan { get; set; }
        public string? GhiChuThem { get; set; }
    }

    public class NhaTaiTroGayQuyVM
    {
        public string? NhomTaiTro { get; set; }
        public string? TenDonVi { get; set; }
        public string? DuongDanLogoCu { get; set; }
        public string? LinkLogoCu { get => DuongDanLogoCu; set => DuongDanLogoCu = value; }
        public IFormFile? FileLogo { get; set; }
        public string? DuongDanTrangWeb { get; set; }
        public string? LinkWebsite { get => DuongDanTrangWeb; set => DuongDanTrangWeb = value; }
        public string? GioiThieu { get; set; }
    }

    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE BAN TỔ CHỨC
    // =========================================================================
    public class CauHinhBanToChucVM
    {
        public int IDCauHinhBTC { get; set; }
        public int IDChienDich { get; set; }
        public string? TenChienDich { get; set; }
        public int NamChienDich { get; set; }

        public string? KieuChuTieuDe { get; set; } = "Dancing Script";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }
        public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";
        public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }

        // Section 1: Ban Sáng Lập
        public bool HienThiBanSangLap { get; set; } = true;
        public string? SangLap_Nhan { get; set; } = "Đội ngũ";
        public string? SangLap_TieuDe { get; set; } = "Ban sáng lập";
        public string? SangLap_CoChuTieuDe { get; set; } = "54px";

        // Section 2: Sơ đồ Tổ chức (Giống ảnh mẫu)
        public bool HienThiSoDoToChuc { get; set; } = true;
        public string? SoDo_Nhan { get; set; } = "Cơ cấu tổ chức";
        public string? SoDo_TieuDe { get; set; } = "Ban Thường Trực & Điều Hành";
        public string? SoDo_DoanVanMoTa { get; set; }

        // Section 3: Thành viên BTC
        public bool HienThiDanhSachBTC { get; set; } = true;
        public string? BTC_Nhan { get; set; } = "Đội ngũ";
        public string? BTC_TieuDe { get; set; } = "Thành viên Ban tổ chức";
        public int SoLuongHienThiBanDau { get; set; } = 9;

        // Section 4: Kêu gọi (CTA)
        public string? KeuGoi_MoTa { get; set; } = "Muốn trở thành một phần của đội ngũ mùa 2026?";
        public string? CTA_MoTa { get => KeuGoi_MoTa; set => KeuGoi_MoTa = value; }
        public string? KeuGoi_ChuNut { get; set; } = "Đăng ký làm tình nguyện viên";
        public string? CTA_TextNut { get => KeuGoi_ChuNut; set => KeuGoi_ChuNut = value; }
        public string? KeuGoi_DuongDanNut { get; set; } = "/#register";
        public string? CTA_LinkNut { get => KeuGoi_DuongDanNut; set => KeuGoi_DuongDanNut = value; }

        public List<ThanhVienBTC_VM> BanSangLaps { get; set; } = new List<ThanhVienBTC_VM>();
        public List<ThanhVienBTC_VM> SoDoThuongTrucs { get; set; } = new List<ThanhVienBTC_VM>();
        public List<ThanhVienBTC_VM> ThanhVienBTCs { get; set; } = new List<ThanhVienBTC_VM>();
    }

    public class ThanhVienBTC_VM
    {
        public string? NhomHienThi { get; set; } = "Tree_Branch";
        public string? HoTen { get; set; }
        public string? ChucVuChinh { get; set; }
        public string? DonViCongTac { get; set; }
        public string? ChucVuPhu { get; set; }
        public string? MoTaChiTiet { get; set; }

        public string? DuongDanAnhCu { get; set; }
        public string? LinkAnhCu { get => DuongDanAnhCu; set => DuongDanAnhCu = value; }
        public IFormFile? FileAnh { get; set; }
        public string? LinkFacebook { get; set; }
        public bool CoHuyHieuSao { get; set; } = false;
    }

    // =========================================================================
    // CÁC VIEWMODELS CHO MODULE TRANG BẢN TIN / TIN TỨC
    // =========================================================================
    public class CauHinhTinTucVM
    {
        public int IDCauHinhTT { get; set; }
        public int IDChienDich { get; set; }
        public string? TenChienDich { get; set; }
        public int NamChienDich { get; set; }

        // Cấu hình chung
        public string? KieuChuTieuDe { get; set; } = "Dancing Script";
        public string? FontTieuDe { get => KieuChuTieuDe; set => KieuChuTieuDe = value; }
        public string? KieuChuNoiDung { get; set; } = "Be Vietnam Pro";
        public string? FontNoiDung { get => KieuChuNoiDung; set => KieuChuNoiDung = value; }

        // Hero (Nổi bật) Section
        public string? NoiBat_Nhan { get; set; } = "Bản tin";
        public string? Hero_Nhan { get => NoiBat_Nhan; set => NoiBat_Nhan = value; }

        public string? NoiBat_TieuDe { get; set; } = "Câu chuyện từ hành trình";
        public string? Hero_TieuDe { get => NoiBat_TieuDe; set => NoiBat_TieuDe = value; }
        public string? NoiBat_CoChuTieuDe { get; set; } = "64px";
        public string? Hero_CoChuTieuDe { get => NoiBat_CoChuTieuDe; set => NoiBat_CoChuTieuDe = value; }

        public string? NoiBat_MoTa { get; set; } = "Những dòng nhật ký, hình ảnh và tin tức mới nhất về Trăng Sáng Langbiang. Đăng ký bản tin để không bỏ lỡ mùa 2026!";
        public string? Hero_MoTa { get => NoiBat_MoTa; set => NoiBat_MoTa = value; }

        public List<BaiVietTinTucVM> BaiViets { get; set; } = new List<BaiVietTinTucVM>();
    }

    public class BaiVietTinTucVM
    {
        public int IDBaiViet { get; set; }
        public string? TieuDe { get; set; }
        public string? ThePhanLoai { get; set; } = "THÔNG BÁO 2026";

        public string? TrichDan { get; set; }
        public string? NoiDungChiTiet { get; set; }

        public string? DuongDanAnhDaiDienCu { get; set; }
        public string? LinkAnhDaiDienCu { get => DuongDanAnhDaiDienCu; set => DuongDanAnhDaiDienCu = value; }
        public IFormFile? FileAnhDaiDien { get; set; }

        public string? DuongDanAnhTrongBaiCu { get; set; }
        public string? LinkAnhTrongBaiCu { get => DuongDanAnhTrongBaiCu; set => DuongDanAnhTrongBaiCu = value; }
        public IFormFile? FileAnhTrongBai { get; set; }

        public string? DuongDanBaiVietGoc { get; set; }
        public string? LinkBaiVietGoc { get => DuongDanBaiVietGoc; set => DuongDanBaiVietGoc = value; }

        public string? CoChuTieuDe { get; set; } = "18px";

        public int LuotThich { get; set; } = 0;
        public string? NgayDang { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public int? IDHieuUng { get; set; }
    }
}