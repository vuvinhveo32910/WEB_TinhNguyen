using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// Thực thể Đơn đặt mua / Ủng hộ gây quỹ
    /// </summary>
    [Table("DonHangGayQuy")]
    public class DonHangGayQuy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã ID")]
        public int IDDonHang { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Mã đơn hàng")]
        public string MaDonHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự")]
        [Display(Name = "Họ và tên người nhận")]
        public string HoTenNguoiMua { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        [StringLength(500, ErrorMessage = "Địa chỉ nhận hàng không được vượt quá 500 ký tự")]
        [Display(Name = "Địa chỉ nhận hàng")]
        public string DiaChiNhanHang { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        [Display(Name = "Lời nhắn gửi / Ghi chú đơn hàng")]
        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền hàng")]
        public decimal TongTienHang { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tiền ủng hộ thêm")]
        public decimal TienUngHoThem { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Phí giao hàng")]
        public decimal PhiVanChuyen { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng số tiền thanh toán")]
        public decimal TongThanhToan { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "ChuyenKhoanQR"; // "ChuyenKhoanQR" hoặc "COD"

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái đơn hàng")]
        public string TrangThaiDonHang { get; set; } = "MoiDat"; // "MoiDat", "DaXacNhan", "DangGiao", "HoanThanh", "DaHuy"

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan"; // "ChuaThanhToan", "DaThanhToan"

        [Display(Name = "Thời gian đặt hàng")]
        public DateTime NgayDatHang { get; set; } = DateTime.Now;

        [Display(Name = "Thời gian cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        public virtual ICollection<ChiTietDonHangGayQuy> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangGayQuy>();

        // --- CÁC THUỘC TÍNH TIỆN ÍCH ---
        [NotMapped]
        public int TongSoLuongSanPham => ChiTietDonHangs?.Sum(c => c.SoLuong) ?? 0;

        [NotMapped]
        public string TenPhuongThucHienThi => PhuongThucThanhToan == "COD" ? "Thanh toán khi nhận hàng (COD)" : "Chuyển khoản VietQR";

        [NotMapped]
        public string TenTrangThaiHienThi => TrangThaiDonHang switch
        {
            "MoiDat" => "Mới đặt hàng",
            "DaXacNhan" => "Đã xác nhận",
            "DangGiao" => "Đang giao hàng",
            "HoanThanh" => "Đã hoàn thành",
            "DaHuy" => "Đã hủy đơn",
            _ => TrangThaiDonHang
        };
    }
}
