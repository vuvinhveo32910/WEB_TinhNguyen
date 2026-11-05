using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WEB_TinhNguyen.Areas.CuaHang.Models
{
    /// <summary>
    /// ViewModel biểu mẫu Thêm mới / Chỉnh sửa Sản phẩm trong phân hệ Admin
    /// </summary>
    public class SanPhamCrudViewModel
    {
        [Display(Name = "Mã sản phẩm")]
        public int IDSanPham { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, 1000000000, ErrorMessage = "Giá bán phải nằm trong khoảng từ 0 đến 1.000.000.000 VNĐ")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal GiaBan { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá niêm yết cũ phải nằm trong khoảng từ 0 đến 1.000.000.000 VNĐ")]
        [Display(Name = "Giá niêm yết cũ (nếu có để hiển thị gạch ngang)")]
        public decimal? GiaGoc { get; set; }

        [StringLength(1000, ErrorMessage = "Đường dẫn ảnh không được vượt quá 1000 ký tự")]
        [Display(Name = "Đường dẫn hình ảnh (URL hoặc đường dẫn sẵn có)")]
        public string? LinkHinhAnh { get; set; }

        [Display(Name = "Tải tệp hình ảnh từ thiết bị (Ưu tiên tệp tải lên)")]
        public IFormFile? FileHinhAnh { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả ngắn gọn (hiển thị ở thẻ sản phẩm)")]
        public string? MoTaNgan { get; set; }

        [Display(Name = "Mô tả chi tiết sản phẩm")]
        public string? MoTaChiTiet { get; set; }

        [StringLength(500, ErrorMessage = "Thông điệp gây quỹ không được vượt quá 500 ký tự")]
        [Display(Name = "Thông điệp gây quỹ / Ý nghĩa nhân văn")]
        public string? ThongDiepGayQuy { get; set; }

        [Range(1, 100, ErrorMessage = "Tỷ lệ trích quỹ phải từ 1% đến 100%")]
        [Display(Name = "Tỷ lệ đóng góp vào quỹ (%)")]
        public int PhanTramDongGop { get; set; } = 100;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, 1000000, ErrorMessage = "Số lượng tồn phải nằm trong khoảng từ 0 đến 1.000.000")]
        [Display(Name = "Số lượng tồn kho")]
        public int SoLuongTon { get; set; } = 50;

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        [Display(Name = "Danh mục")]
        public string DanhMuc { get; set; } = "Quà lưu niệm";

        [Display(Name = "Hiển thị lên cửa hàng")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Thứ tự hiển thị")]
        public int ThuTu { get; set; } = 0;

        [Display(Name = "Chiến dịch")]
        public int? IDChienDich { get; set; }

        // --- CÁC PHƯƠNG THỨC MAPPING TIỆN ÍCH CHO CONTROLLER ---

        /// <summary>
        /// Tạo ViewModel từ thực thể Entity trong cơ sở dữ liệu (Dùng cho action GET Edit)
        /// </summary>
        public static SanPhamCrudViewModel FromEntity(SanPhamGayQuy entity)
        {
            return new SanPhamCrudViewModel
            {
                IDSanPham = entity.IDSanPham,
                TenSanPham = entity.TenSanPham,
                GiaBan = entity.GiaBan,
                GiaGoc = entity.GiaGoc,
                LinkHinhAnh = entity.LinkHinhAnh,
                MoTaNgan = entity.MoTaNgan,
                MoTaChiTiet = entity.MoTaChiTiet,
                ThongDiepGayQuy = entity.ThongDiepGayQuy,
                PhanTramDongGop = entity.PhanTramDongGop,
                SoLuongTon = entity.SoLuongTon,
                DanhMuc = entity.DanhMuc,
                TrangThai = entity.TrangThai,
                ThuTu = entity.ThuTu,
                IDChienDich = entity.IDChienDich
            };
        }

        /// <summary>
        /// Chuyển đổi ViewModel thành thực thể Entity mới (Dùng cho action POST Create)
        /// </summary>
        public SanPhamGayQuy ToEntity(string? uploadedImagePath = null)
        {
            return new SanPhamGayQuy
            {
                TenSanPham = this.TenSanPham.Trim(),
                GiaBan = this.GiaBan,
                GiaGoc = this.GiaGoc,
                LinkHinhAnh = !string.IsNullOrEmpty(uploadedImagePath) ? uploadedImagePath : this.LinkHinhAnh,
                MoTaNgan = this.MoTaNgan?.Trim(),
                MoTaChiTiet = this.MoTaChiTiet?.Trim(),
                ThongDiepGayQuy = this.ThongDiepGayQuy?.Trim(),
                PhanTramDongGop = this.PhanTramDongGop,
                SoLuongTon = this.SoLuongTon,
                DanhMuc = string.IsNullOrWhiteSpace(this.DanhMuc) ? "Quà lưu niệm" : this.DanhMuc.Trim(),
                TrangThai = this.TrangThai,
                ThuTu = this.ThuTu,
                IDChienDich = this.IDChienDich,
                NgayTao = DateTime.Now
            };
        }

        /// <summary>
        /// Cập nhật thông tin từ ViewModel vào thực thể Entity đã tồn tại (Dùng cho action POST Edit)
        /// </summary>
        public void UpdateEntity(SanPhamGayQuy entity, string? uploadedImagePath = null)
        {
            entity.TenSanPham = this.TenSanPham.Trim();
            entity.GiaBan = this.GiaBan;
            entity.GiaGoc = this.GiaGoc;
            
            if (!string.IsNullOrEmpty(uploadedImagePath))
            {
                entity.LinkHinhAnh = uploadedImagePath;
            }
            else if (!string.IsNullOrEmpty(this.LinkHinhAnh))
            {
                entity.LinkHinhAnh = this.LinkHinhAnh.Trim();
            }

            entity.MoTaNgan = this.MoTaNgan?.Trim();
            entity.MoTaChiTiet = this.MoTaChiTiet?.Trim();
            entity.ThongDiepGayQuy = this.ThongDiepGayQuy?.Trim();
            entity.PhanTramDongGop = this.PhanTramDongGop;
            entity.SoLuongTon = this.SoLuongTon;
            entity.DanhMuc = string.IsNullOrWhiteSpace(this.DanhMuc) ? "Quà lưu niệm" : this.DanhMuc.Trim();
            entity.TrangThai = this.TrangThai;
            entity.ThuTu = this.ThuTu;
            entity.IDChienDich = this.IDChienDich;
            entity.NgayCapNhat = DateTime.Now;
        }
    }
}
