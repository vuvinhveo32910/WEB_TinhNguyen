using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_TinhNguyen.MoHinh
{
    // =========================================================================
    // MODULE: TRANG CHỦ
    // =========================================================================
    [Table("ChienDich")]
    public class ChienDich
    {
        [Key]
        public int IDChienDich { get; set; }
        public int Nam { get; set; }
        public string? TieuDe { get; set; }
        public string? TenHienThiMenu { get; set; }
        public bool TrangThaiHoatDong { get; set; }

        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("CauHinhTrangChu")]
    public class CauHinhTrangChu
    {
        [Key]
        public int IDCauHinh { get; set; }
        public int IDChienDich { get; set; }

        public string? TieuDePhu { get; set; }
        public string? TieuDeChinh { get; set; }
        public string? LinkAnhBia { get; set; }
        [NotMapped]
        public string? DuongDanAnhBia { get => LinkAnhBia; set => LinkAnhBia = value; }
        public string? DoanVanMoTa { get; set; }

        public int TocDoChuyenSlide { get; set; } = 4000;

        public string? FontTieuDe { get; set; }
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }
        public string? FontNoiDung { get; set; }
        [NotMapped]
        public string? KieuChuNoiDung { get => FontNoiDung; set => FontNoiDung = value; }

        public bool HienThiCanvas3D { get; set; }
        [NotMapped]
        public bool HienThiHieuUng3D { get => HienThiCanvas3D; set => HienThiCanvas3D = value; }
        public bool HienThiLaRoi { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public string? VeDuAn_TieuDe { get; set; }
        public string? VeDuAn_NoiDung { get; set; }
        public string? VeDuAn_LinkAnh { get; set; }
        [NotMapped]
        public string? VeDuAn_DuongDanAnh { get => VeDuAn_LinkAnh; set => VeDuAn_LinkAnh = value; }
        // Khóa ngoại liên kết tới Kho Hiệu Ứng
        public int? IDHieuUng { get; set; }
        [ForeignKey("IDHieuUng")]
        public virtual ThuVienHieuUng? ThuVienHieuUng { get; set; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }
        // --- CẤU HÌNH LINH VẬT TẠI FOOTER ---
        public bool HienThiMascot { get; set; } = true;
        [NotMapped]
        public bool HienThiLinhVat { get => HienThiMascot; set => HienThiMascot = value; }
        public string? LinkAnhMascot { get; set; }
        [NotMapped]
        public string? DuongDanAnhLinhVat { get => LinkAnhMascot; set => LinkAnhMascot = value; }
        public string? ViTriMascot { get; set; } = "right"; // Giá trị: left, right, center
        [NotMapped]
        public string? ViTriLinhVat { get => ViTriMascot; set => ViTriMascot = value; }
        public virtual ICollection<BieuNguTrangChu>? BieuNguTrangChus { get; set; }
        [NotMapped]
        public virtual ICollection<BieuNguTrangChu>? BannerTrangChus { get => BieuNguTrangChus; set => BieuNguTrangChus = value; }
        public virtual ICollection<ChuongTrinhDemNguoc>? ChuongTrinhDemNguocs { get; set; }
        public virtual ICollection<CauHinhAnhNoi>? CauHinhAnhNois { get; set; }
        public virtual ICollection<ThuVienKhoanhKhac>? ThuVienKhoanhKhacs { get; set; }
        [NotMapped]
        public virtual ICollection<ThuVienKhoanhKhac>? TVKhoanhKhacs { get => ThuVienKhoanhKhacs; set => ThuVienKhoanhKhacs = value; }
        public virtual ICollection<TinTucTrangChu>? TinTucs { get; set; }
        public virtual ICollection<NhaTaiTroTrangChu>? NhaTaiTros { get; set; }
        public virtual ICollection<CauHoiThuongGapTrangChu>? CauHoiThuongGaps { get; set; }
        [NotMapped]
        public virtual ICollection<CauHoiThuongGapTrangChu>? FAQs { get => CauHoiThuongGaps; set => CauHoiThuongGaps = value; }
    }

    [Table("BannerTrangChu")]
    public class BieuNguTrangChu
    {
        [Key]
        [Column("IDBanner")]
        public int IDBieuNgu { get; set; }
        [NotMapped]
        public int IDBanner { get => IDBieuNgu; set => IDBieuNgu = value; }
        public int IDCauHinh { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        [Column("TieuDeAlt")]
        public string? TieuDeThayThe { get; set; }
        [NotMapped]
        public string? TieuDeAlt { get => TieuDeThayThe; set => TieuDeThayThe = value; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("CauHinhAnhNoi")]
    public class CauHinhAnhNoi
    {
        [Key]
        public int IDAnhNoi { get; set; }
        public int IDCauHinh { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        [Column("TieuDeAlt")]
        public string? TieuDeThayThe { get; set; }
        [NotMapped]
        public string? TieuDeAlt { get => TieuDeThayThe; set => TieuDeThayThe = value; }
        public int ChieuSau { get; set; }
        [Column("ViTriTop")]
        public string? ViTriTren { get; set; }
        [NotMapped]
        public string? ViTriTop { get => ViTriTren; set => ViTriTren = value; }
        public string? CanLe { get; set; }
        public string? KhoangCachLe { get; set; }
        public string? KichThuoc { get; set; }
        [Column("DoTreAnimation")]
        public string? DoTreHoatHoa { get; set; }
        [NotMapped]
        public string? DoTreAnimation { get => DoTreHoatHoa; set => DoTreHoatHoa = value; }
        public string? GocXoay { get; set; }
        public bool TrangThai { get; set; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("TVKhoanhKhac")]
    public class ThuVienKhoanhKhac
    {
        [Key]
        public int IDKhoanhKhac { get; set; }
        public int IDCauHinh { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        public string? TieuDe { get; set; }
        public string? MoTa { get; set; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("TinTucTrangChu")]
    public class TinTucTrangChu
    {
        [Key]
        public int IDTinTuc { get; set; }
        public int IDCauHinh { get; set; }
        public string? TieuDe { get; set; }
        public string? TrichDan { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        public string? ThePhanLoai { get; set; }
        public string? LinkChuyenHuong { get; set; }
        public int LuotThich { get; set; }
        public string? NoiDungChiTiet { get; set; }
        public string? KichCoTieuDe { get; set; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("NhaTaiTroTrangChu")]
    public class NhaTaiTroTrangChu
    {
        [Key]
        public int IDNhaTaiTro { get; set; }
        public int IDCauHinh { get; set; }
        public string? NhomTaiTro { get; set; }
        public string? TenDonVi { get; set; }
        [Column("LinkLogo")]
        public string? DuongDanLogo { get; set; }
        [NotMapped]
        public string? LinkLogo { get => DuongDanLogo; set => DuongDanLogo = value; }
        [NotMapped]
        public string? DuongDanTrangWeb { get; set; }
        [NotMapped]
        public string? LinkWebsite { get => DuongDanTrangWeb; set => DuongDanTrangWeb = value; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("FAQTrangChu")]
    public class CauHoiThuongGapTrangChu
    {
        [Key]
        [Column("IDFAQ")]
        public int IDCauHoi { get; set; }
        [NotMapped]
        public int IDFAQ { get => IDCauHoi; set => IDCauHoi = value; }
        public int IDCauHinh { get; set; }
        public string? CauHoi { get; set; }
        public string? CauTraLoi { get; set; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    [Table("ChuongTrinhDemNguoc")]
    public class ChuongTrinhDemNguoc
    {
        [Key]
        public int IDChuongTrinh { get; set; }
        public int IDCauHinh { get; set; }

        public string? TieuDePhu { get; set; }
        public string? TieuDeChinh { get; set; }
        public string? MoTa { get; set; }
        public string? ThoiGianHienThi { get; set; }

        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }

        public bool EpKetThuc { get; set; }

        [ForeignKey("IDCauHinh")]
        public virtual CauHinhTrangChu? CauHinhTrangChu { get; set; }
    }

    // =========================================================================
    // MODULE: KÝ ỨC (NHÌN LẠI CÁC MÙA DỰ ÁN)
    // =========================================================================
    [Table("CauHinhKyUc")]
    public class CauHinhKyUc
    {
        [Key]
        public int IDCauHinhKyUc { get; set; }
        public int IDChienDich { get; set; }

        public string? TieuDeChinh { get; set; } = "Nhìn lại mùa dự án";
        public string? DoanVanMoTa { get; set; }
        public string? NoiDungTongKetHtml { get; set; }
        public string? LinkAnhBia { get; set; }
        [NotMapped]
        public string? DuongDanAnhBia { get => LinkAnhBia; set => LinkAnhBia = value; }
        public string? TieuDeTongKet { get; set; }
        public string? FontTieuDeTongKet { get; set; }
        [NotMapped]
        public string? KieuChuTieuDeTongKet { get => FontTieuDeTongKet; set => FontTieuDeTongKet = value; }

        public string? FontTieuDe { get; set; } = "Dancing Script";
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }

        public virtual ICollection<AnhKyUc>? AnhKyUcs { get; set; }
        public virtual ICollection<NhaTaiTroKyUc>? NhaTaiTroKyUcs { get; set; }
    }

    [Table("AnhKyUc")]
    public class AnhKyUc
    {
        [Key]
        public int IDAnh { get; set; }
        public int IDCauHinhKyUc { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        public string? TieuDeAnh { get; set; }
        public bool LaAnhDoc { get; set; } = false;
        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhKyUc")]
        public virtual CauHinhKyUc? CauHinhKyUc { get; set; }
    }

    [Table("NhaTaiTroKyUc")]
    public class NhaTaiTroKyUc
    {
        [Key]
        public int IDNhaTaiTro { get; set; }
        public int IDCauHinhKyUc { get; set; }
        public string? TenDonVi { get; set; }
        [Column("LinkLogo")]
        public string? DuongDanLogo { get; set; }
        [NotMapped]
        public string? LinkLogo { get => DuongDanLogo; set => DuongDanLogo = value; }
        public string? PhanLoai { get; set; }
        [NotMapped]
        public string? DuongDanTrangWeb { get; set; }
        [NotMapped]
        public string? LinkWebsite { get => DuongDanTrangWeb; set => DuongDanTrangWeb = value; }

        [ForeignKey("IDCauHinhKyUc")]
        public virtual CauHinhKyUc? CauHinhKyUc { get; set; }
    }

    // =========================================================================
    // MODULE: CHƯƠNG TRÌNH HOẠT ĐỘNG
    // =========================================================================
    [Table("CauHinhChuongTrinh")]
    public class CauHinhChuongTrinh
    {
        [Key]
        public int IDCauHinhCT { get; set; }
        public int IDChienDich { get; set; }

        // HERO SECTION
        public string? TieuDeChinh { get; set; } = "Hành Trình Lan Tỏa";
        public string? TieuDeNhanManh { get; set; } = "Yêu Thương và Hy Vọng";
        public string? DoanVanMoTa { get; set; }
        [Column("LinkAnhHero1")]
        public string? DuongDanAnhHero1 { get; set; }
        [NotMapped]
        public string? LinkAnhHero1 { get => DuongDanAnhHero1; set => DuongDanAnhHero1 = value; }
        [Column("LinkAnhHero2")]
        public string? DuongDanAnhHero2 { get; set; }
        [NotMapped]
        public string? LinkAnhHero2 { get => DuongDanAnhHero2; set => DuongDanAnhHero2 = value; }

        // GIAO DIỆN (CSS)
        public string? FontTieuDe { get; set; } = "Be Vietnam Pro";
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }
        public virtual ICollection<ChangChuongTrinh>? ChangChuongTrinhs { get; set; }
    }

    [Table("ChangChuongTrinh")]
    public class ChangChuongTrinh
    {
        [Key]
        public int IDChang { get; set; }
        public int IDCauHinhCT { get; set; }

        public string? NhanThoiGian { get; set; }
        public string? TieuDeChang { get; set; }
        public string? MoTaChang { get; set; }
        public string? DiaDiem { get; set; }
        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        public virtual ICollection<ChiTietLoTrinh>? ChiTietLoTrinhs { get; set; }

        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhCT")]
        public virtual CauHinhChuongTrinh? CauHinhChuongTrinh { get; set; }
    }

    [Table("ChiTietLoTrinh")]
    public class ChiTietLoTrinh
    {
        [Key]
        public int IDLoTrinh { get; set; }
        public int IDChang { get; set; }

        public string? ThoiGian { get; set; } // VD: "13g30"
        public string? NoiDungHoatDong { get; set; } // VD: "Công tác chuẩn bị, trang trí gian hàng..."
        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDChang")]
        public virtual ChangChuongTrinh? ChangChuongTrinh { get; set; }
    }

    // =========================================================================
    // MODULE: ĐÓNG GÓP / GÂY QUỸ (GAY QUY)
    // =========================================================================
    [Table("CauHinhGayQuy")]
    public class CauHinhGayQuy
    {
        [Key]
        public int IDCauHinhGQ { get; set; }
        public int IDChienDich { get; set; }

        // --- CẤU HÌNH CHUNG ---
        public string? FontTieuDe { get; set; } = "Be Vietnam Pro";
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }
        public string? FontNoiDung { get; set; } = "Be Vietnam Pro";
        [NotMapped]
        public string? KieuChuNoiDung { get => FontNoiDung; set => FontNoiDung = value; }

        // --- SECTION 1: HERO (NỔI BẬT) ---
        [Column("Hero_Nhan")]
        public string? NoiBat_Nhan { get; set; } = "KHÔNG GIAN YÊU THƯƠNG";
        [NotMapped]
        public string? Hero_Nhan { get => NoiBat_Nhan; set => NoiBat_Nhan = value; }

        [Column("Hero_TieuDe")]
        public string? NoiBat_TieuDe { get; set; } = "HÌNH THỨC TRAO GỬI YÊU THƯƠNG";
        [NotMapped]
        public string? Hero_TieuDe { get => NoiBat_TieuDe; set => NoiBat_TieuDe = value; }
        [Column("Hero_CoChuTieuDe")]
        public string? NoiBat_CoChuTieuDe { get; set; } = "48px";
        [NotMapped]
        public string? Hero_CoChuTieuDe { get => NoiBat_CoChuTieuDe; set => NoiBat_CoChuTieuDe = value; }

        [Column("Hero_MoTa")]
        public string? NoiBat_MoTa { get; set; }
        [NotMapped]
        public string? Hero_MoTa { get => NoiBat_MoTa; set => NoiBat_MoTa = value; }

        [Column("Hero_TextNut")]
        public string? NoiBat_ChuNut { get; set; } = "Đóng góp ngay";
        [NotMapped]
        public string? Hero_TextNut { get => NoiBat_ChuNut; set => NoiBat_ChuNut = value; }
        [Column("Hero_LinkNut")]
        public string? NoiBat_DuongDanNut { get; set; } = "#ways-to-contribute";
        [NotMapped]
        public string? Hero_LinkNut { get => NoiBat_DuongDanNut; set => NoiBat_DuongDanNut = value; }

        [Column("Hero_LinkAnh")]
        public string? NoiBat_DuongDanAnh { get; set; }
        [NotMapped]
        public string? Hero_LinkAnh { get => NoiBat_DuongDanAnh; set => NoiBat_DuongDanAnh = value; }

        // --- SECTION 2: CÁCH THỨC ĐÓNG GÓP ---
        public string? DongGop_Nhan { get; set; } = "Kênh ủng hộ";
        public string? DongGop_TieuDe { get; set; } = "Cách thức đóng góp";
        public string? DongGop_MoTa { get; set; } = "Cùng chung tay lan tỏa yêu thương bằng phương thức phù hợp nhất với bạn.";
        public string? DongGop_GhiChuChanTrang { get; set; } = "Mọi đóng góp đều được ghi nhận và công khai minh bạch trên Fanpage của dự án.";

        // --- SECTION 3: BÁO CÁO THU CHI ---
        public string? BaoCao_Nhan { get; set; } = "Minh bạch";
        public string? BaoCao_TieuDe { get; set; } = "Báo cáo thu – chi";
        public string? BaoCao_MoTa { get; set; } = "Toàn bộ khoản thu – chi mùa 2026 được cập nhật công khai trên các kênh thông tin của Dự án.";
        [Column("BaoCao_TextNut")]
        public string? BaoCao_ChuNut { get; set; } = "Xem báo cáo chi tiết";
        [NotMapped]
        public string? BaoCao_TextNut { get => BaoCao_ChuNut; set => BaoCao_ChuNut = value; }
        [Column("BaoCao_LinkNut")]
        public string? BaoCao_DuongDanNut { get; set; }
        [NotMapped]
        public string? BaoCao_LinkNut { get => BaoCao_DuongDanNut; set => BaoCao_DuongDanNut = value; }

        // --- SECTION 4: ĐƠN VỊ TÀI TRỢ & TỔ CHỨC ---
        public string? TaiTro_Nhan { get; set; } = "Những mảnh ghép tạo nên Trăng sáng Langbiang 2026";
        public string? TaiTro_TieuDe { get; set; } = "ĐƠN VỊ TỔ CHỨC - TÀI TRỢ";
        public string? TaiTro_TieuDeNoiBat { get; set; } = "NĂM 2026";
        public string? TaiTro_MoTa { get; set; } = "Mỗi đơn vị là một mảnh ghép quan trọng, cùng nhau thắp sáng và làm nên một mùa trăng 2026 thật trọn vẹn!";

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }

        public virtual ICollection<KenhDongGop>? KenhDongGops { get; set; }
        public virtual ICollection<NhaTaiTroGayQuy>? NhaTaiTroGayQuys { get; set; }
    }

    [Table("KenhDongGop")]
    public class KenhDongGop
    {
        [Key]
        public int IDKenh { get; set; }
        public int IDCauHinhGQ { get; set; }

        public string? TenKenh { get; set; } // VD: Quyên góp trực tiếp
        public string? MoTa { get; set; }
        [Column("LoaiIcon")]
        public string? LoaiBieuTuong { get; set; } = "bank"; // bank, gift, heart, star
        [NotMapped]
        public string? LoaiIcon { get => LoaiBieuTuong; set => LoaiBieuTuong = value; }

        [Column("TextNut")]
        public string? ChuNut { get; set; } = "QR Tài khoản";
        [NotMapped]
        public string? TextNut { get => ChuNut; set => ChuNut = value; }
        public string? LoaiHanhDong { get; set; } = "qr"; // "qr" = Mở Popup QR, "link" = Mở link ngoài
        [Column("LinkNgoai")]
        public string? DuongDanNgoai { get; set; }
        [NotMapped]
        public string? LinkNgoai { get => DuongDanNgoai; set => DuongDanNgoai = value; }

        // Dữ liệu cho Popup QR
        [Column("LinkAnhQR")]
        public string? DuongDanAnhQR { get; set; }
        [NotMapped]
        public string? LinkAnhQR { get => DuongDanAnhQR; set => DuongDanAnhQR = value; }
        public string? TenNganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTaiKhoan { get; set; }
        public string? NoiDungChuyenKhoan { get; set; }
        public string? GhiChuThem { get; set; }

        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhGQ")]
        public virtual CauHinhGayQuy? CauHinhGayQuy { get; set; }
    }

    [Table("NhaTaiTroGayQuy")]
    public class NhaTaiTroGayQuy
    {
        [Key]
        public int IDNhaTaiTroGQ { get; set; }
        public int IDCauHinhGQ { get; set; }

        public string? NhomTaiTro { get; set; } // VD: Đơn vị tổ chức năm 2026
        public string? TenDonVi { get; set; }
        [Column("LinkLogo")]
        public string? DuongDanLogo { get; set; }
        [NotMapped]
        public string? LinkLogo { get => DuongDanLogo; set => DuongDanLogo = value; }
        [Column("LinkWebsite")]
        public string? DuongDanTrangWeb { get; set; }
        [NotMapped]
        public string? LinkWebsite { get => DuongDanTrangWeb; set => DuongDanTrangWeb = value; }
        public string? GioiThieu { get; set; } // Nội dung hiện lên khi click vào logo
        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhGQ")]
        public virtual CauHinhGayQuy? CauHinhGayQuy { get; set; }
    }

    // =========================================================================
    // MODULE: BAN TỔ CHỨC (BAN TO CHUC & SO DO TO CHUC)
    // =========================================================================
    [Table("CauHinhBanToChuc")]
    public class CauHinhBanToChuc
    {
        [Key]
        public int IDCauHinhBTC { get; set; }
        public int IDChienDich { get; set; }

        // --- CẤU HÌNH CHUNG ---
        public string? FontTieuDe { get; set; } = "Dancing Script";
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }
        public string? FontNoiDung { get; set; } = "Be Vietnam Pro";
        [NotMapped]
        public string? KieuChuNoiDung { get => FontNoiDung; set => FontNoiDung = value; }

        // --- SECTION 1: BAN SÁNG LẬP ---
        public bool HienThiBanSangLap { get; set; } = true;
        public string? SangLap_Nhan { get; set; } = "Đội ngũ";
        public string? SangLap_TieuDe { get; set; } = "Ban sáng lập";
        public string? SangLap_CoChuTieuDe { get; set; } = "54px";

        // --- SECTION 2: SƠ ĐỒ BAN THƯỜNG TRỰC / TRƯỞNG BAN (GIỐNG ẢNH MẪU) ---
        public bool HienThiSoDoToChuc { get; set; } = true;
        public string? SoDo_Nhan { get; set; } = "Cơ cấu tổ chức";
        public string? SoDo_TieuDe { get; set; } = "Ban Thường Trực & Điều Hành";
        public string? SoDo_DoanVanMoTa { get; set; } = "Ban Tổ chức dự án là lực lượng nòng cốt trong việc xây dựng kế hoạch, kết nối nguồn lực và trực tiếp điều phối các hoạt động tình nguyện. Với mô hình hoạt động thống nhất giữa các đơn vị đồng hành, Ban Thường trực dự án gồm:";

        // --- SECTION 3: DANH SÁCH BAN TỔ CHỨC & THÀNH VIÊN ---
        public bool HienThiDanhSachBTC { get; set; } = true;
        public string? BTC_Nhan { get; set; } = "Đội ngũ";
        public string? BTC_TieuDe { get; set; } = "Thành viên Ban tổ chức";
        public int SoLuongHienThiBanDau { get; set; } = 9; // Số lượng hiện trước khi bấm "Xem thêm"

        // --- SECTION 4: KÊU GỌI ĐĂNG KÝ TNV (CTA) ---
        [Column("CTA_MoTa")]
        public string? KeuGoi_MoTa { get; set; } = "Muốn trở thành một phần của đội ngũ mùa 2026?";
        [NotMapped]
        public string? CTA_MoTa { get => KeuGoi_MoTa; set => KeuGoi_MoTa = value; }
        [Column("CTA_TextNut")]
        public string? KeuGoi_ChuNut { get; set; } = "Đăng ký làm tình nguyện viên";
        [NotMapped]
        public string? CTA_TextNut { get => KeuGoi_ChuNut; set => KeuGoi_ChuNut = value; }
        [Column("CTA_LinkNut")]
        public string? KeuGoi_DuongDanNut { get; set; } = "/#register";
        [NotMapped]
        public string? CTA_LinkNut { get => KeuGoi_DuongDanNut; set => KeuGoi_DuongDanNut = value; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }

        public virtual ICollection<ThanhVienBanToChuc>? ThanhViens { get; set; }
    }

    [Table("ThanhVienBanToChuc")]
    public class ThanhVienBanToChuc
    {
        [Key]
        public int IDThanhVien { get; set; }
        public int IDCauHinhBTC { get; set; }

        public string? NhomHienThi { get; set; } = "Tree_Branch";

        public string? HoTen { get; set; } // VD: ANH PHẠM ĐỨC TUYÊN
        public string? ChucVuChinh { get; set; } // Chữ to in đậm
        public string? DonViCongTac { get; set; } // Dòng dưới chức vụ chính
        public string? ChucVuPhu { get; set; } // Các gạch đầu dòng
        public string? MoTaChiTiet { get; set; } // Đoạn giới thiệu dài

        [Column("LinkAnh")]
        public string? DuongDanAnh { get; set; }
        [NotMapped]
        public string? LinkAnh { get => DuongDanAnh; set => DuongDanAnh = value; }
        public string? LinkFacebook { get; set; }
        public bool CoHuyHieuSao { get; set; } = false; // Icon ngôi sao góc trái ảnh

        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhBTC")]
        public virtual CauHinhBanToChuc? CauHinhBanToChuc { get; set; }
    }

    // =========================================================================
    // MODULE: TRANG BẢN TIN / TIN TỨC (TIN TUC)
    // =========================================================================
    [Table("CauHinhTinTuc")]
    public class CauHinhTinTuc
    {
        [Key]
        public int IDCauHinhTT { get; set; }
        public int IDChienDich { get; set; }

        // --- CẤU HÌNH CHUNG ---
        public string? FontTieuDe { get; set; } = "Dancing Script";
        [NotMapped]
        public string? KieuChuTieuDe { get => FontTieuDe; set => FontTieuDe = value; }
        public string? FontNoiDung { get; set; } = "Be Vietnam Pro";
        [NotMapped]
        public string? KieuChuNoiDung { get => FontNoiDung; set => FontNoiDung = value; }

        // --- SECTION 1: HERO (NỔI BẬT) ---
        [Column("Hero_Nhan")]
        public string? NoiBat_Nhan { get; set; } = "Bản tin";
        [NotMapped]
        public string? Hero_Nhan { get => NoiBat_Nhan; set => NoiBat_Nhan = value; }

        [Column("Hero_TieuDe")]
        public string? NoiBat_TieuDe { get; set; } = "Câu chuyện từ hành trình";
        [NotMapped]
        public string? Hero_TieuDe { get => NoiBat_TieuDe; set => NoiBat_TieuDe = value; }
        [Column("Hero_CoChuTieuDe")]
        public string? NoiBat_CoChuTieuDe { get; set; } = "64px";
        [NotMapped]
        public string? Hero_CoChuTieuDe { get => NoiBat_CoChuTieuDe; set => NoiBat_CoChuTieuDe = value; }

        [Column("Hero_MoTa")]
        public string? NoiBat_MoTa { get; set; } = "Những dòng nhật ký, hình ảnh và tin tức mới nhất về Trăng Sáng Langbiang. Đăng ký bản tin để không bỏ lỡ mùa 2026!";
        [NotMapped]
        public string? Hero_MoTa { get => NoiBat_MoTa; set => NoiBat_MoTa = value; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }

        public virtual ICollection<BaiVietTinTuc>? BaiViets { get; set; }
    }

    [Table("BaiVietTinTuc")]
    public class BaiVietTinTuc
    {
        [Key]
        public int IDBaiViet { get; set; }
        public int IDCauHinhTT { get; set; }

        public string? TieuDe { get; set; }
        public string? ThePhanLoai { get; set; } = "THÔNG BÁO 2026"; // Tag góc trái trên ảnh

        public string? TrichDan { get; set; } // Đoạn mô tả ngắn ngoài thẻ
        public string? NoiDungChiTiet { get; set; } // Nội dung HTML hiện trong Popup khi bấm "Đọc tiếp"

        [Column("LinkAnhDaiDien")]
        public string? DuongDanAnhDaiDien { get; set; } // Ảnh bìa của thẻ
        [NotMapped]
        public string? LinkAnhDaiDien { get => DuongDanAnhDaiDien; set => DuongDanAnhDaiDien = value; }
        [Column("LinkAnhTrongBai")]
        public string? DuongDanAnhTrongBai { get; set; } // Ảnh đính kèm cuối bài trong Popup
        [NotMapped]
        public string? LinkAnhTrongBai { get => DuongDanAnhTrongBai; set => DuongDanAnhTrongBai = value; }
        [Column("LinkBaiVietGoc")]
        public string? DuongDanBaiVietGoc { get; set; } // Link bài Facebook gốc (tùy chọn)
        [NotMapped]
        public string? LinkBaiVietGoc { get => DuongDanBaiVietGoc; set => DuongDanBaiVietGoc = value; }

        public string? CoChuTieuDe { get; set; } = "18px";

        public int LuotThich { get; set; } = 0;
        public string? NgayDang { get; set; } = "2026-08-22";
        public int ThuTu { get; set; } = 0;

        [ForeignKey("IDCauHinhTT")]
        public virtual CauHinhTinTuc? CauHinhTinTuc { get; set; }
    }

    // =========================================================================
    // MODULE: KHO HIỆU ỨNG ĐỘNG TOÀN TRANG (TẾT, NOEL, TRUNG THU...)
    // =========================================================================
    [Table("ThuVienHieuUng")]
    public class ThuVienHieuUng
    {
        [Key]
        public int IDHieuUng { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hiệu ứng")]
        public string? TenHieuUng { get; set; } // VD: Hoa Mai Rơi, Tuyết Rơi...

        public string? MoTa { get; set; }
        [Column("LinkFileJS")]
        public string? DuongDanTepJS { get; set; } // Đường dẫn file Script chứa hiệu ứng
        [NotMapped]
        public string? LinkFileJS { get => DuongDanTepJS; set => DuongDanTepJS = value; }
        public bool TrangThai { get; set; } = true;
    }
}