# BÁO CÁO THIẾT KẾ CHI TIẾT THỰC THỂ DỮ LIỆU & VIEWMODELS CHO PHÂN HỆ CỬA HÀNG GÂY QUỸ
**Milestone 1 — Task M1.2: Entities & ViewModels Specification**  
**Agent thực hiện**: `teamwork_explorer_m1_2`  
**Thư mục làm việc**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2`  
**Người nhận báo cáo**: Project Orchestrator (`108b9d81-1dd6-495f-84eb-99dbb197aebe`)  
**Ngày báo cáo**: 2026-11-03  

---

## 1. Observation (Các quan sát thực tế)

1. **Hiện trạng cấu trúc dự án và mô hình dữ liệu**:
   - Vị trí tệp định nghĩa thực thể: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/Entities.cs` (613 dòng).
   - Vị trí tệp DbContext: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Data/ApplicationDbContext.cs` (34 dòng).
   - `ApplicationDbContext` hiện đăng ký 19 `DbSet` cho các module Trang chủ, Ký ức, Chương trình, Ban tổ chức, Tin tức, Kênh đóng góp trực tiếp (`KenhDongGop`), nhưng **hoàn toàn chưa có các bảng cho tính năng thương mại điện tử gây quỹ** (`SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`).
   - Các thực thể hiện tại trong `Entities.cs` tuân thủ quy ước DataAnnotations: `[Table("...")]`, `[Key]`, `[ForeignKey("...")]`.

2. **Cấu hình Trình biên dịch & Ngôn ngữ C#**:
   - Tệp dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     <Nullable>enable</Nullable>
     <ImplicitUsings>enable</ImplicitUsings>
     ```
   - EF Core phiên bản `10.0.12` (`Microsoft.EntityFrameworkCore.SqlServer`).
   - Do `<Nullable>enable</Nullable>`, các thuộc tính kiểu tham chiếu (`string`) không được gán giá trị mặc định sẽ phát sinh cảnh báo `CS8618: Non-nullable property ... must contain a non-null value when exiting constructor`. Vì vậy, tất cả thuộc tính chuỗi không nullable cần được khởi tạo `= string.Empty;`, và các danh sách cần `= new List<...>();`.
   - Với các cột kiểu `decimal`, EF Core yêu cầu chỉ định thuộc tính `[Column(TypeName = "decimal(18,2)")]` để tránh cảnh báo mất độ chính xác tiền tệ khi tạo cột trong SQL Server.

3. **Mô hình Xử lý Tải ảnh (Upload) trong Codebase**:
   - Khảo sát `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/CauHinhTrangChuVM.cs` (dòng 437-438) và `Controllers/AdminTinTucController.cs` (dòng 270-320):
   - Dự án sử dụng mô hình kết hợp:
     - `LinkHinhAnh` (kiểu `string?`): Lưu đường dẫn ảnh cũ hoặc URL trực tiếp.
     - `FileHinhAnh` (kiểu `IFormFile?`): Nhận file upload nhị phân từ form multipart/form-data trên giao diện Admin.
     - Hàm lưu file lưu trữ vào `wwwroot/uploads/...` và gán URL dạng `/uploads/{subFolder}/{guid}{ext}`.

4. **Yêu cầu Nghiệp vụ từ ORIGINAL_REQUEST.md & PROJECT.md**:
   - **R1 (Storefront)**: Hiển thị sản phẩm gây quỹ với hình ảnh, giá bán, mô tả, thông tin gây quỹ tình nguyện; xem chi tiết, chọn số lượng, thêm vào giỏ hàng; form thanh toán/đặt mua ghi nhận thông tin người nhận, lời nhắn, số tiền ủng hộ thêm tùy tâm, phương thức thanh toán (VietQR hoặc COD).
   - **R2 (Admin)**: Quản lý danh sách, thêm, sửa, xóa sản phẩm đầy đủ các trường (tên, giá bán, giá niêm yết cũ, ảnh, mô tả ngắn, mô tả chi tiết, thông điệp gây quỹ, tỷ lệ trích quỹ, số lượng tồn kho, danh mục, trạng thái).
   - **R3 (Area Architecture)**: Toàn bộ Controllers, Views, Models/ViewModels đặt trong thư mục riêng biệt `Areas/CuaHang/`.
   - **Interface Contract**:
     - `CartItem`: `{ IDSanPham, TenSanPham, LinkHinhAnh, DonGia, SoLuong, ThanhTien, ThongDiepGayQuy }`
     - `CartViewModel`: `{ Items, TongSoLuong, TongTienHang, TongTienDongGop, PhiVanChuyen, TienUngHoThem, TongThanhToan }`
     - `CheckoutViewModel`: `{ HoTen, SoDienThoai, Email, DiaChi, GhiChu, TienUngHoThem, PhuongThucThanhToan, Items, TongTien }`
     - `SanPhamCrudViewModel`: Hỗ trợ form Admin Create/Edit, tích hợp file upload và mapping 2 chiều sang thực thể.

---

## 2. Logic Chain (Chuỗi lập luận & Thiết kế kỹ thuật)

1. **Từ Quan sát 1 & 4 (Kiến trúc Area và Tổ chức Namespace)**:
   - Tất cả 7 tệp Model & ViewModel mới sẽ được đặt tại thư mục: `WEB_TinhNguyen/Areas/CuaHang/Models/`.
   - Namespace chuẩn hóa thống nhất: `WEB_TinhNguyen.Areas.CuaHang.Models`.
   - `ApplicationDbContext` chỉ cần bổ sung `using WEB_TinhNguyen.Areas.CuaHang.Models;` để khai báo 3 `DbSet`.

2. **Từ Quan sát 1 & 2 (Thiết kế Thực thể SanPhamGayQuy)**:
   - Cần phản ánh đầy đủ thông tin bán hàng thương mại điện tử lẫn giá trị gây quỹ nhân văn:
     - Khóa chính: `IDSanPham` (int, Identity).
     - Định danh & Trưng bày: `TenSanPham`, `DanhMuc`, `LinkHinhAnh`, `ThuTu`, `TrangThai`.
     - Tài chính & Đóng góp: `GiaBan`, `GiaGoc` (để hiển thị giá ưu đãi gạch ngang), `PhanTramDongGop` (mặc định 100%, hoặc từ 1-100%).
     - Quản lý kho: `SoLuongTon` (tự động kiểm tra còn hàng / hết hàng), `DaBan` (số lượt đã ủng hộ để hiển thị thanh tiến độ).
     - Nội dung truyền thông: `MoTaNgan`, `MoTaChiTiet`, `ThongDiepGayQuy` (thể hiện cụ thể bao nhiêu tiền / hiện vật sẽ đến tay các em nhỏ Langbiang).
     - Quan hệ điều hướng: `ChiTietDonHangs` (liên kết 1-nhiều tới các chi tiết đơn hàng).

3. **Từ Quan sát 4 (Thiết kế Thực thể DonHangGayQuy & ChiTietDonHangGayQuy)**:
   - Đơn hàng ủng hộ không yêu cầu người dùng phải đăng nhập tài khoản (đặc trưng website từ thiện cộng đồng), do đó cần lưu đầy đủ thông tin liên hệ và nhận hàng tại thời điểm đặt:
     - `MaDonHang`: Sinh tự động theo định dạng thời gian + mã ngẫu nhiên (ví dụ: `GQ20261103-X89B`).
     - Người nhận: `HoTenNguoiMua`, `SoDienThoai`, `Email`, `DiaChiNhanHang`, `GhiChu`.
     - Tài chính: `TongTienHang`, `TienUngHoThem` (người mua có thể tự nguyện quyên góp thêm ngoài tiền hàng), `PhiVanChuyen` (mặc định 0đ), `TongThanhToan`.
     - Trạng thái: `PhuongThucThanhToan` ("ChuyenKhoanQR" hoặc "COD"), `TrangThaiDonHang` ("MoiDat", "DaXacNhan", "DangGiao", "HoanThanh", "DaHuy"), `TrangThaiThanhToan` ("ChuaThanhToan", "DaThanhToan").
   - Chi tiết đơn hàng (`ChiTietDonHangGayQuy`):
     - Lưu "Snapshot" tại thời điểm đặt (`TenSanPham`, `DonGia`, `SoLuong`, `ThanhTien`, `LinkHinhAnh`, `ThongDiepGayQuy`) để đảm bảo lịch sử đơn hàng luôn chính xác ngay cả khi Admin thay đổi giá bán hoặc tên sản phẩm sau này.
     - Khóa ngoại `IDDonHang` liên kết `DonHangGayQuy` và `IDSanPham` (nullable) liên kết `SanPhamGayQuy`.

4. **Từ Quan sát 3 (Thiết kế ViewModels cho Storefront & Admin)**:
   - `CartItem`: Model lưu gọn nhẹ trong `ISession["GioHangGayQuy"]` dưới định dạng JSON, có sẵn các property tính toán `ThanhTien` và `TienDongGop`.
   - `CartViewModel`: Tổng hợp giỏ hàng cho View `GioHang/Index.cshtml`, tính tổng tiền, tổng số lượng và tổng tiền đóng góp từ thiện.
   - `CheckoutViewModel`: Binding form đặt hàng, chứa validation tin cậy (DataAnnotations) bằng tiếng Việt, bao gồm cả danh sách các mặt hàng đang đặt để render bảng tóm tắt bên cạnh form.
   - `SanPhamCrudViewModel`: Phục vụ các trang Admin Create và Edit. Hỗ trợ trường `IFormFile? FileHinhAnh` cho upload tệp cục bộ và `string? LinkHinhAnh` cho nhập link trực tiếp. Tích hợp sẵn 3 phương thức tĩnh/tiện ích `FromEntity()`, `ToEntity()`, và `UpdateEntity()` giúp giảm thiểu boilerplate code và triệt tiêu lỗi copy thuộc tính trong Controller.

---

## 3. Thiết kế Chi tiết & Mã nguồn Triển khai (Code Specifications)

Dưới đây là mã nguồn C# chi tiết 100%, sẵn sàng áp dụng ngay cho từng tệp.

### 3.1. `Areas/CuaHang/Models/SanPhamGayQuy.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WEB_TinhNguyen.Models;

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
```

---

### 3.2. `Areas/CuaHang/Models/DonHangGayQuy.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/DonHangGayQuy.cs`

```csharp
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
```

---

### 3.3. `Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`

```csharp
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
```

---

### 3.4. `Areas/CuaHang/Models/CartItem.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/CartItem.cs`

```csharp
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
```

---

### 3.5. `Areas/CuaHang/Models/CartViewModel.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/CartViewModel.cs`

```csharp
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
```

---

### 3.6. `Areas/CuaHang/Models/CheckoutViewModel.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/CheckoutViewModel.cs`

```csharp
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
```

---

### 3.7. `Areas/CuaHang/Models/SanPhamCrudViewModel.cs`
**Đường dẫn**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`

```csharp
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
```

---

## 4. Caveats (Các điểm cần lưu ý)

1. **Khóa ngoại với `ChienDich`**:
   - `IDChienDich` trong `SanPhamGayQuy` là `int?` (nullable). Nếu trong database chưa có bản ghi `ChienDich` nào, sản phẩm vẫn có thể được tạo bình thường với giá trị `null` mà không bị ràng buộc Referential Integrity ngăn chặn.
2. **Xóa Sản phẩm có trong Đơn hàng cũ**:
   - Khi Admin xóa một sản phẩm đã có đơn hàng trong bảng `ChiTietDonHangGayQuy`, trong Controller xóa cần kiểm tra hoặc `ChiTietDonHangGayQuy.IDSanPham` phải là nullable (`int?`) để khi xóa sản phẩm không làm vỡ quan hệ toàn vẹn dữ liệu đơn hàng cũ.
3. **Mã đơn hàng**:
   - `MaDonHang` được sinh ở tầng controller đặt hàng theo quy tắc `$"GQ{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}"`. Thuộc tính này là bắt buộc (`[Required]`) và giới hạn tối đa 50 ký tự.
4. **Session Key**:
   - Giỏ hàng sử dụng khóa phiên `"GioHangGayQuy"` để lưu `List<CartItem>` tương ứng với giao ước trong `PROJECT.md` (Dòng 87).

---

## 5. Conclusion (Kết luận)

1. Đã thiết kế hoàn chỉnh và chi tiết 100% cho 3 Thực thể dữ liệu (`SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`) và 4 ViewModels (`CartItem`, `CartViewModel`, `CheckoutViewModel`, `SanPhamCrudViewModel`).
2. Toàn bộ mã nguồn tuân thủ nghiêm ngặt:
   - Framework .NET 10.0 và EF Core 10.0.12.
   - Cơ chế Nullable Reference Types (`<Nullable>enable</Nullable>`) sạch sẽ, không gây cảnh báo biên dịch CS8618.
   - Chỉ định kiểu cột SQL Server tường minh `[Column(TypeName = "decimal(18,2)")]` cho các trường tiền tệ.
   - Đầy đủ DataAnnotations kiểm tra tính hợp lệ dữ liệu (Validation) kèm thông điệp tiếng Việt thân thiện, chuyên nghiệp.
   - Kiến trúc Area đóng gói trong `WEB_TinhNguyen.Areas.CuaHang.Models`.
3. Cung cấp sẵn các hàm mapping 2 chiều thông minh trong `SanPhamCrudViewModel` giúp cho việc viết Controller của Milestone 2 (`AdminSanPhamController`) trở nên nhanh chóng, an toàn và dễ kiểm thử.

---

## 6. Verification Method (Phương pháp kiểm chứng độc lập)

1. **Kiểm tra cú pháp và tích hợp biên dịch**:
   - Sau khi tạo các file tại `WEB_TinhNguyen/Areas/CuaHang/Models/` và cập nhật `ApplicationDbContext.cs`, thực thi lệnh:
     ```bash
     XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
     ```
   - **Kỳ vọng**: Trạng thái `Build succeeded: 0 Error(s)`.
2. **Kiểm tra tính toàn vẹn thuộc tính & kiểu dữ liệu**:
   - Kiểm tra xem 3 thực thể có đầy đủ các trường yêu cầu trong `PROJECT.md` và `ORIGINAL_REQUEST.md`: Tên, Giá, Số lượng tồn, Hình ảnh, Mô tả, Thông điệp gây quỹ, Tỷ lệ trích quỹ.
   - Kiểm tra xem `CartItem` và `CartViewModel` có đầy đủ các thuộc tính tính toán thành tiền và số tiền đóng góp.
3. **Điều kiện vô hiệu hóa (Invalidation conditions)**:
   - Xuất hiện lỗi biên dịch CS8618 nếu thiếu giá trị khởi tạo mặc định.
   - Xuất hiện cảnh báo EF Core về `decimal` precision nếu thiếu `TypeName = "decimal(18,2)"`.
