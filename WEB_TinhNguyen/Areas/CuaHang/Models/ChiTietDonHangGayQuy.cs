using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// Thực thể Chi tiết Đơn hàng Gây quỹ (Từng dòng sản phẩm trong đơn)
    /// </summary>
    [Table("ChiTietDonHangGayQuy")]
    public class ChiTietDonHangGayQuy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã ID")]
        public int IDChiTiet { get; set; }

        [Required]
        public int IDDonHang { get; set; }

        [ForeignKey("IDDonHang")]
        public virtual DonHangGayQuy? DonHang { get; set; }

        public int? IDSanPham { get; set; }

        [ForeignKey("IDSanPham")]
        public virtual SanPhamGayQuy? SanPham { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Hình ảnh sản phẩm")]
        public string? LinkHinhAnh { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Số lượng phải ít nhất là 1")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }

        [StringLength(500)]
        [Display(Name = "Thông điệp gây quỹ")]
        public string? ThongDiepGayQuy { get; set; }

        [Range(0, 100)]
        [Display(Name = "Tỷ lệ đóng góp quỹ (%)")]
        public int PhanTramDongGop { get; set; } = 100;

        // --- CÁC THUỘC TÍNH TIỆN ÍCH ---
        [NotMapped]
        public decimal TienDongGop => ThanhTien * PhanTramDongGop / 100m;
    }
}
