using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WEB_TinhNguyen.MoHinh;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// Thực thể Sản phẩm Gây quỹ Tình nguyện
    /// </summary>
    [Table("SanPhamGayQuy")]
    public class SanPhamGayQuy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã ID")]
        public int IDSanPham { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, 1000000000, ErrorMessage = "Giá bán phải nằm trong khoảng từ 0 đến 1.000.000.000 VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal GiaBan { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá niêm yết cũ phải nằm trong khoảng từ 0 đến 1.000.000.000 VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá niêm yết cũ (VNĐ)")]
        public decimal? GiaGoc { get; set; }

        [StringLength(1000, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 1000 ký tự")]
        [Display(Name = "Hình ảnh sản phẩm")]
        public string? LinkHinhAnh { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả ngắn gọn")]
        public string? MoTaNgan { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        public string? MoTaChiTiet { get; set; }

        [StringLength(500, ErrorMessage = "Thông điệp gây quỹ không được vượt quá 500 ký tự")]
        [Display(Name = "Thông điệp gây quỹ / Ý nghĩa nhân văn")]
        public string? ThongDiepGayQuy { get; set; }

        [Range(1, 100, ErrorMessage = "Tỷ lệ đóng góp vào quỹ từ 1% đến 100%")]
        [Display(Name = "Tỷ lệ đóng góp quỹ (%)")]
        public int PhanTramDongGop { get; set; } = 100;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, 1000000, ErrorMessage = "Số lượng tồn phải nằm trong khoảng từ 0 đến 1.000.000")]
        [Display(Name = "Số lượng tồn kho")]
        public int SoLuongTon { get; set; } = 100;

        [Display(Name = "Số lượng đã bán")]
        public int DaBan { get; set; } = 0;

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        [Display(Name = "Danh mục")]
        public string DanhMuc { get; set; } = "Quà lưu niệm";

        [Display(Name = "Trạng thái hiển thị")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Thứ tự sắp xếp")]
        public int ThuTu { get; set; } = 0;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Chiến dịch liên kết")]
        public int? IDChienDich { get; set; }

        [ForeignKey("IDChienDich")]
        public virtual ChienDich? ChienDich { get; set; }

        public virtual ICollection<ChiTietDonHangGayQuy> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangGayQuy>();

        // --- CÁC THUỘC TÍNH TIỆN ÍCH KHÔNG ÁNH XẠ DATABASE ---
        [NotMapped]
        public bool ConHang => SoLuongTon > 0;

        [NotMapped]
        public decimal TienDongGopTrenMoiSp => GiaBan * PhanTramDongGop / 100m;
    }
}
