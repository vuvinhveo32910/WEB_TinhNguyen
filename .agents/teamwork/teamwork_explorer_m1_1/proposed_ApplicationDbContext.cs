using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Models;
using WEB_TinhNguyen.Areas.CuaHang.Models;

namespace WEB_TinhNguyen.Data
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
        public DbSet<BannerTrangChu> BannerTrangChus { get; set; }
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

        // =========================================================================
        // PHÂN HỆ: CỬA HÀNG SẢN PHẨM GÂY QUỸ (AREA: CuaHang)
        // =========================================================================
        public DbSet<SanPhamGayQuy> SanPhamGayQuys { get; set; }
        public DbSet<DonHangGayQuy> DonHangGayQuys { get; set; }
        public DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                      .WithMany()
                      .HasForeignKey(d => d.IDSanPham)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
