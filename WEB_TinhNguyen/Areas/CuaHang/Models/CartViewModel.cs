using System.Collections.Generic;
using System.Linq;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// ViewModel cho màn hình xem và cập nhật giỏ hàng
    /// </summary>
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public int TongSoLuong => Items?.Sum(i => i.SoLuong) ?? 0;
        public decimal TongTienHang => Items?.Sum(i => i.ThanhTien) ?? 0;
        public decimal TongTienDongGop => Items?.Sum(i => i.TienDongGop) ?? 0;
        public decimal PhiVanChuyen { get; set; } = 0;
        public decimal TienUngHoThem { get; set; } = 0;
        public decimal TongThanhToan => TongTienHang + PhiVanChuyen + TienUngHoThem;
        public bool CoSanPham => Items != null && Items.Count > 0;
    }
}
