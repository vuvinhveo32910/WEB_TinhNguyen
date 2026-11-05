using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.MoHinh;
using WEB_TinhNguyen.Areas.CuaHang.Models;

namespace WEB_TinhNguyen.DuLieu
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

       

        // Khai báo các bảng để Entity Framework tạo trong Database
        public DbSet<ChienDich> ChienDichs { get; set; }
        public DbSet<CauHinhTrangChu> CauHinhTrangChus { get; set; }
        public DbSet<CauHinhAnhNoi> CauHinhAnhNois { get; set; }
        public DbSet<BieuNguTrangChu> BieuNguTrangChus { get; set; }
        public DbSet<BieuNguTrangChu> BannerTrangChus { get => BieuNguTrangChus; set => BieuNguTrangChus = value; }
        public DbSet<ChuongTrinhDemNguoc> ChuongTrinhDemNguocs { get; set; }
        public DbSet<CauHinhKyUc> CauHinhKyUcs { get; set; }
        public DbSet<AnhKyUc> AnhKyUcs { get; set; }
        public DbSet<NhaTaiTroKyUc> NhaTaiTroKyUcs { get; set; }
        public DbSet<CauHinhChuongTrinh> CauHinhChuongTrinhs { get; set; }
        public DbSet<ChangChuongTrinh> ChangChuongTrinhs { get; set; }
        public DbSet<ChiTietLoTrinh> ChiTietLoTrinhs { get; set; }
        public DbSet<CauHinhGayQuy> CauHinhGayQuys { get; set; }
        public DbSet<KenhDongGop> KenhDongGops { get; set; }
        public DbSet<NhaTaiTroGayQuy> NhaTaiTroGayQuys { get; set; }
        public DbSet<CauHinhBanToChuc> CauHinhBanToChucs { get; set; }
        public DbSet<ThanhVienBanToChuc> ThanhVienBanToChucs { get; set; }
        public DbSet<CauHinhTinTuc> CauHinhTinTucs { get; set; }
        public DbSet<BaiVietTinTuc> BaiVietTinTucs { get; set; }
        public DbSet<ThuVienHieuUng> ThuVienHieuUngs { get; set; }
        public DbSet<ThuVienKhoanhKhac> ThuVienKhoanhKhacs { get; set; }
        public DbSet<ThuVienKhoanhKhac> TVKhoanhKhacs { get => ThuVienKhoanhKhacs; set => ThuVienKhoanhKhacs = value; }
        public DbSet<CauHoiThuongGapTrangChu> CauHoiThuongGapTrangChus { get; set; }
        public DbSet<CauHoiThuongGapTrangChu> FAQTrangChus { get => CauHoiThuongGapTrangChus; set => CauHoiThuongGapTrangChus = value; }

        // =========================================================================
        // PHÂN HỆ: CỬA HÀNG SẢN PHẨM GÂY QUỸ (AREA: CuaHang)
        // =========================================================================
        public DbSet<SanPhamGayQuy> SanPhamGayQuys { get; set; }
        public DbSet<DonHangGayQuy> DonHangGayQuys { get; set; }
        public DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ánh xạ tên bảng thực tế trong SQL Server cho BieuNguTrangChu, ThuVienKhoanhKhac, CauHoiThuongGapTrangChu
            modelBuilder.Entity<BieuNguTrangChu>(entity =>
            {
                entity.ToTable("BannerTrangChu");
                entity.Ignore(e => e.IDBanner);
                entity.Ignore(e => e.LinkAnh);
                entity.Ignore(e => e.TieuDeAlt);
            });

            modelBuilder.Entity<ThuVienKhoanhKhac>(entity =>
            {
                entity.ToTable("TVKhoanhKhac");
                entity.Ignore(e => e.LinkAnh);
            });

            modelBuilder.Entity<CauHoiThuongGapTrangChu>(entity =>
            {
                entity.ToTable("FAQTrangChu");
                entity.Ignore(e => e.IDFAQ);
            });

            modelBuilder.Entity<NhaTaiTroTrangChu>(entity =>
            {
                entity.ToTable("NhaTaiTroTrangChu");
                entity.Ignore(e => e.LinkLogo);
                entity.Ignore(e => e.DuongDanTrangWeb);
                entity.Ignore(e => e.LinkWebsite);
            });

            modelBuilder.Entity<NhaTaiTroKyUc>(entity =>
            {
                entity.ToTable("NhaTaiTroKyUc");
                entity.Ignore(e => e.LinkLogo);
                entity.Ignore(e => e.DuongDanTrangWeb);
                entity.Ignore(e => e.LinkWebsite);
            });

            // Cấu hình bảng CauHinhTrangChu và loại trừ các cột không tồn tại trong CSDL
            modelBuilder.Entity<CauHinhTrangChu>(entity =>
            {
                entity.ToTable("CauHinhTrangChu");
                entity.Ignore(e => e.DuongDanAnhBia);
                entity.Ignore(e => e.DuongDanAnhLinhVat);
                entity.Ignore(e => e.HienThiHieuUng3D);
                entity.Ignore(e => e.HienThiLinhVat);
                entity.Ignore(e => e.KieuChuNoiDung);
                entity.Ignore(e => e.KieuChuTieuDe);
                entity.Ignore(e => e.VeDuAn_DuongDanAnh);
                entity.Ignore(e => e.ViTriLinhVat);

                entity.Property(e => e.LinkAnhBia);
                entity.Property(e => e.FontTieuDe);
                entity.Property(e => e.FontNoiDung);
                entity.Property(e => e.HienThiCanvas3D);
                entity.Property(e => e.VeDuAn_LinkAnh);
                entity.Property(e => e.HienThiMascot);
                entity.Property(e => e.LinkAnhMascot);
                entity.Property(e => e.ViTriMascot);
            });

            // Cấu hình bảng CauHinhKyUc
            modelBuilder.Entity<CauHinhKyUc>(entity =>
            {
                entity.ToTable("CauHinhKyUc");
                entity.Ignore(e => e.DuongDanAnhBia);
                entity.Ignore(e => e.KieuChuTieuDe);
                entity.Ignore(e => e.KieuChuTieuDeTongKet);

                entity.Property(e => e.LinkAnhBia);
                entity.Property(e => e.FontTieuDe);
                entity.Property(e => e.FontTieuDeTongKet);
            });

            // Cấu hình bảng CauHinhChuongTrinh
            modelBuilder.Entity<CauHinhChuongTrinh>(entity =>
            {
                entity.ToTable("CauHinhChuongTrinh");
                entity.Ignore(e => e.KieuChuTieuDe);

                entity.Property(e => e.FontTieuDe);
            });

            // Cấu hình bảng CauHinhGayQuy
            modelBuilder.Entity<CauHinhGayQuy>(entity =>
            {
                entity.ToTable("CauHinhGayQuy");
                entity.Ignore(e => e.KieuChuTieuDe);
                entity.Ignore(e => e.KieuChuNoiDung);

                entity.Property(e => e.FontTieuDe);
                entity.Property(e => e.FontNoiDung);
            });

            // Cấu hình bảng CauHinhBanToChuc
            modelBuilder.Entity<CauHinhBanToChuc>(entity =>
            {
                entity.ToTable("CauHinhBanToChuc");
                entity.Ignore(e => e.KieuChuTieuDe);
                entity.Ignore(e => e.KieuChuNoiDung);

                entity.Property(e => e.FontTieuDe);
                entity.Property(e => e.FontNoiDung);
            });

            // Cấu hình bảng CauHinhTinTuc
            modelBuilder.Entity<CauHinhTinTuc>(entity =>
            {
                entity.ToTable("CauHinhTinTuc");
                entity.Ignore(e => e.KieuChuTieuDe);
                entity.Ignore(e => e.KieuChuNoiDung);

                entity.Property(e => e.FontTieuDe);
                entity.Property(e => e.FontNoiDung);
            });

            // Cấu hình bảng và trường tiền tệ cho SanPhamGayQuy
            modelBuilder.Entity<SanPhamGayQuy>(entity =>
            {
                entity.ToTable("SanPhamGayQuy");
                entity.Property(e => e.GiaBan).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GiaGoc).HasColumnType("decimal(18,2)");
            });

            // Cấu hình bảng và trường tiền tệ cho DonHangGayQuy
            modelBuilder.Entity<DonHangGayQuy>(entity =>
            {
                entity.ToTable("DonHangGayQuy");
                entity.Property(e => e.TongTienHang).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TienUngHoThem).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TongThanhToan).HasColumnType("decimal(18,2)");
            });

            // Cấu hình bảng, trường tiền tệ và khóa ngoại cho ChiTietDonHangGayQuy
            modelBuilder.Entity<ChiTietDonHangGayQuy>(entity =>
            {
                entity.ToTable("ChiTietDonHangGayQuy");
                entity.Property(e => e.DonGia).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.DonHang)
                      .WithMany(p => p.ChiTietDonHangs)
                      .HasForeignKey(d => d.IDDonHang)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SanPham)
                      .WithMany(p => p.ChiTietDonHangs)
                      .HasForeignKey(d => d.IDSanPham)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}