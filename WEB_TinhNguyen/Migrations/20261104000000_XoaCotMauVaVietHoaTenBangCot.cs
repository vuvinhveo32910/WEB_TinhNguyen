using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_TinhNguyen.Migrations
{
    public partial class XoaCotMauVaVietHoaTenBangCot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. XÓA 9 CỘT MÀU TRONG BẢNG CauHinhTrangChu
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNhan", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenStart", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenMid", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauTieuDe", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauChu", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauNenTag", table: "CauHinhTrangChu");

            // 2. XÓA 1 CỘT MÀU TRONG BẢNG TinTucTrangChu
            migrationBuilder.DropColumn(name: "MauTieuDe", table: "TinTucTrangChu");

            // 3. XÓA 3 CỘT MÀU TRONG BẢNG ChuongTrinhDemNguoc
            migrationBuilder.DropColumn(name: "MauNenStart", table: "ChuongTrinhDemNguoc");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "ChuongTrinhDemNguoc");
            migrationBuilder.DropColumn(name: "MauChu", table: "ChuongTrinhDemNguoc");

            // 4. XÓA 4 CỘT MÀU TRONG BẢNG CauHinhKyUc
            migrationBuilder.DropColumn(name: "MauTieuDeTongKet", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauNenHienTai", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhKyUc");

            // 5. XÓA 3 CỘT MÀU TRONG BẢNG CauHinhChuongTrinh
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhChuongTrinh");
            migrationBuilder.DropColumn(name: "MauNenHienTai", table: "CauHinhChuongTrinh");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhChuongTrinh");

            // 6. XÓA 16 CỘT MÀU TRONG BẢNG CauHinhGayQuy
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauChuNhan", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenNhan", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNutStart", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNutEnd", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenStart", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenMid", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenEnd", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "DongGop_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "DongGop_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauNen", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "TaiTro_MauTieuDe", table: "CauHinhGayQuy");

            // 7. XÓA 5 CỘT MÀU TRONG BẢNG KenhDongGop
            migrationBuilder.DropColumn(name: "MauIcon", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauNenIcon", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauTieuDe", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauMoTa", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauNenCard", table: "KenhDongGop");

            // 8. XÓA 9 CỘT MÀU TRONG BẢNG CauHinhBanToChuc
            migrationBuilder.DropColumn(name: "MauNenStart", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenMid", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SangLap_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SoDo_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SoDo_MauDuongKe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "BTC_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "CTA_MauNutStart", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "CTA_MauNutEnd", table: "CauHinhBanToChuc");

            // 9. XÓA 2 CỘT MÀU TRONG BẢNG ThanhVienBanToChuc
            migrationBuilder.DropColumn(name: "MauChuDaoThe", table: "ThanhVienBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenThe", table: "ThanhVienBanToChuc");

            // 10. XÓA 10 CỘT MÀU TRONG BẢNG CauHinhTinTuc
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauChuNhan", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenNhan", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauTieuDe", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauMoTa", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenStart", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenMid", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenEnd", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "DanhSach_MauNenStart", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "DanhSach_MauNenEnd", table: "CauHinhTinTuc");

            // 11. XÓA 4 CỘT MÀU TRONG BẢNG BaiVietTinTuc
            migrationBuilder.DropColumn(name: "MauNenTag", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauChuTag", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauTieuDeThe", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauTieuDePopup", table: "BaiVietTinTuc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
