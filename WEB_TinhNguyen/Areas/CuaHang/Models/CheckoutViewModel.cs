using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// ViewModel biểu mẫu thanh toán & đặt mua / ủng hộ gây quỹ
    /// </summary>
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn")]
        [StringLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự")]
        [Display(Name = "Họ và tên người nhận")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên hệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Địa chỉ email (nhận thông tin đơn hàng)")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        [StringLength(500, ErrorMessage = "Địa chỉ nhận hàng không được vượt quá 500 ký tự")]
        [Display(Name = "Địa chỉ nhận hàng chi tiết")]
        public string DiaChi { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Lời nhắn gửi không được vượt quá 1000 ký tự")]
        [Display(Name = "Lời nhắn gửi / Lời chúc gửi tới các em nhỏ và ban tổ chức")]
        public string? GhiChu { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Số tiền ủng hộ thêm không hợp lệ")]
        [Display(Name = "Ủng hộ thêm tùy tâm cho quỹ (VNĐ)")]
        public decimal TienUngHoThem { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "ChuyenKhoanQR"; // "ChuyenKhoanQR" hoặc "COD"

        // Danh sách mặt hàng kèm theo để hiển thị tóm tắt giỏ hàng
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public int TongSoLuong => Items?.Sum(i => i.SoLuong) ?? 0;
        public decimal TongTienHang => Items?.Sum(i => i.ThanhTien) ?? 0;
        public decimal PhiVanChuyen { get; set; } = 0;
        public decimal TongThanhToan => TongTienHang + PhiVanChuyen + TienUngHoThem;
        public decimal TongTienDongGop => (Items?.Sum(i => i.TienDongGop) ?? 0) + TienUngHoThem;
    }
}
