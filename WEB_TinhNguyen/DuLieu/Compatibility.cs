// =========================================================================
// LỚP TƯƠNG THÍCH NGƯỢC CHO CÁC MIGRATION EF CORE VÀ MÃ NGUỒN CŨ
// =========================================================================

global using WEB_TinhNguyen.MoHinh;
global using WEB_TinhNguyen.BoDieuKhien;
global using BannerTrangChu = WEB_TinhNguyen.MoHinh.BieuNguTrangChu;
global using TVKhoanhKhac = WEB_TinhNguyen.MoHinh.ThuVienKhoanhKhac;
global using FAQTrangChu = WEB_TinhNguyen.MoHinh.CauHoiThuongGapTrangChu;

namespace WEB_TinhNguyen.Data
{
    /// <summary>
    /// Lớp tương thích ngược cho EF Core Migrations và các assembly kiểm thử ngoại vi.
    /// Kế thừa từ WEB_TinhNguyen.DuLieu.ApplicationDbContext để bảo toàn namespace trong PE metadata.
    /// </summary>
    [System.Obsolete("Sử dụng WEB_TinhNguyen.DuLieu.ApplicationDbContext thay thế.")]
    public class ApplicationDbContext : WEB_TinhNguyen.DuLieu.ApplicationDbContext
    {
        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<WEB_TinhNguyen.DuLieu.ApplicationDbContext> options)
            : base(options)
        {
        }

        
    }
}

namespace WEB_TinhNguyen.Controllers
{
    /// <summary>
    /// Type compatibility marker để các kiểm thử hoặc tham chiếu Reflection cũ
    /// (e.g. typeof(WEB_TinhNguyen.Controllers.HomeController).Assembly) không bị lỗi CS0234.
    /// </summary>
    [System.Obsolete("Sử dụng WEB_TinhNguyen.BoDieuKhien.HomeController thay thế.")]
    [Microsoft.AspNetCore.Mvc.NonController]
    public class HomeController
    {
    }
}
