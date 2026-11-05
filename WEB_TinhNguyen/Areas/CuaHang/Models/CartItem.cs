namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// Model đại diện cho một sản phẩm trong giỏ hàng (lưu trong ASP.NET Core Session)
    /// </summary>
    public class CartItem
    {
        public int IDSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? LinkHinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; } = 1;
        public string? ThongDiepGayQuy { get; set; }
        public int PhanTramDongGop { get; set; } = 100;
        public int SoLuongTon { get; set; } = 100;

        public decimal ThanhTien => DonGia * SoLuong;
        public decimal TienDongGop => ThanhTien * PhanTramDongGop / 100m;
    }
}
