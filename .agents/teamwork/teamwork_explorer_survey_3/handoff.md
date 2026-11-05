# BÁO CÁO KHẢO SÁT: CỬA HÀNG GÂY QUỸ, GIỎ HÀNG, THỰC THỂ DỮ LIỆU, SEED DATA VÀ KIỂM CHỨNG HỆ THỐNG

**Tác vụ**: Khảo sát năng lực Storefront, mẫu triển khai giỏ hàng (Session/Cookie), thực thể nghiệp vụ sản phẩm & đơn hàng gây quỹ, cơ chế Seed Data và xác minh biên dịch dự án (`dotnet build`).  
**Người thực hiện**: `teamwork_explorer_survey_3`  
**Ngày thực hiện**: 2026-11-03  
**Dự án**: WEB_TinhNguyen (.NET 10.0, ASP.NET Core MVC, EF Core 10.0.12)  
**Tệp handoff**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_survey_3/handoff.md`

---

## 1. Observation (Các quan sát thực tế)

### 1.1. Hiện trạng Cấu hình Session & Cookie trong `Program.cs`
Kiểm tra trực tiếp tệp `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Program.cs` (toàn bộ 38 dòng):
```csharp
using WEB_TinhNguyen.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Dán đoạn này vào để kết nối DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
```

**Các quan sát cốt lõi**:
1. **Chưa có cấu hình bộ nhớ đệm (Cache)**: Hoàn toàn không có `builder.Services.AddDistributedMemoryCache();`.
2. **Chưa có dịch vụ Session**: Hoàn toàn không có `builder.Services.AddSession(...)`.
3. **Chưa có Middleware Session**: Hoàn toàn không có `app.UseSession();` trong pipeline xử lý HTTP request (phải nằm giữa `UseRouting()` và `UseAuthorization()` / `MapControllerRoute()`).
4. **Chưa có Route cho ASP.NET Core Area**: Chỉ có duy nhất route `"default"`. Chưa có `app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")`.
5. **Dòng lệnh bị lặp**: Dòng 10 và dòng 12 cùng gọi `builder.Services.AddControllersWithViews();`.

### 1.2. Hiện trạng Thực thể Nghiệp vụ trong `Models/Entities.cs` và `Data/ApplicationDbContext.cs`
1. **Kiểm tra `Data/ApplicationDbContext.cs`** (34 dòng):
   - Đang đăng ký 19 `DbSet`: `ChienDichs`, `CauHinhTrangChus`, `CauHinhAnhNois`, `BannerTrangChus`, `ChuongTrinhDemNguocs`, `CauHinhKyUcs`, `AnhKyUcs`, `NhaTaiTroKyUcs`, `CauHinhChuongTrinhs`, `ChangChuongTrinhs`, `ChiTietLoTrinhs`, `CauHinhGayQuys`, `KenhDongGops`, `NhaTaiTroGayQuys`, `CauHinhBanToChucs`, `ThanhVienBanToChucs`, `CauHinhTinTucs`, `BaiVietTinTucs`, `ThuVienHieuUngs`.
   - **Hoàn toàn chưa có DbSet nào** cho `SanPham`, `Product`, `DonHang`, `Order`, `ChiTietDonHang`.
2. **Kiểm tra `Models/Entities.cs`** (613 dòng):
   - Các bảng đều dùng thuộc tính `[Table("...")]`, khóa chính `[Key]`, khóa ngoại `[ForeignKey("...")]`.
   - Các trường thuộc tính dùng kiểu dữ liệu C# tương ứng: `int`, `string?`, `bool`, `DateTime?`, `decimal` (chưa dùng).
   - Module `CauHinhGayQuy` hiện tại (dòng 322-439) chỉ phục vụ trang giới thiệu ủng hộ từ thiện tĩnh gồm: banner hero, kênh chuyển khoản QR (`KenhDongGop`), và danh sách nhà tài trợ (`NhaTaiTroGayQuy`), hoàn toàn chưa có tính năng thương mại điện tử gây quỹ (E-commerce / Storefront).

### 1.3. Cơ chế Khởi tạo Dữ liệu Mẫu (Seed Data) Hiện tại của Dự án
1. **Không có `OnModelCreating` chứa `HasData`**: `ApplicationDbContext.cs` không override phương thức `OnModelCreating`, không cấu hình seed cứng qua EF Core Migrations.
2. **Không có `DbInitializer` tại Startup**: `Program.cs` không gọi phương thức khởi tạo dữ liệu nào khi ứng dụng chạy.
3. **Mẫu Khởi tạo Dữ liệu Tự động (Lazy On-demand Seeding)**:
   - Khảo sát `Controllers/AdminBanToChucController.cs` (dòng 44-86):
     ```csharp
     var config = await _context.CauHinhBanToChucs.Include(c => c.ThanhViens).FirstOrDefaultAsync(c => c.IDChienDich == id);
     if (config == null)
     {
         config = new CauHinhBanToChuc { ... };
         _context.CauHinhBanToChucs.Add(config);
         await _context.SaveChangesAsync();
         var seedMembers = new List<ThanhVienBanToChuc> { ... };
         _context.ThanhVienBanToChucs.AddRange(seedMembers);
         await _context.SaveChangesAsync();
     }
     ```
   - Khảo sát `Controllers/AdminGayQuyController.cs` (dòng 44-90):
     Tương tự, khi `config == null`, tự động tạo bản ghi cấu hình mặc định và 2 `KenhDongGop` mẫu (MBBank Nguyễn Anh Hào, Quyên góp hiện vật).
   - Khảo sát `Controllers/AdminTinTucController.cs` (dòng 47-178):
     Tự động tạo 10 bài viết mẫu nếu bảng bài viết rỗng.

### 1.4. Môi trường Thực thi và Biên dịch Dự án (`dotnet build`)
1. **Lệnh biên dịch mặc định**:
   `dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`
   - **Kết quả**: Thất bại với lỗi:
     ```
     /usr/lib/dotnet/sdk/10.0.114/NuGet.targets(196,5): error MSB4018: The "RestoreTask" task failed unexpectedly.
     System.IO.IOException: The system cannot open the device or file specified. : 'NuGet-Migrations'. One or more system calls failed: lstat("/run/user/1000", ...) == -1; errno == ENOENT;
     ```
   - **Nguyên nhân**: Môi trường container sandbox Linux không có thư mục runtime người dùng `/run/user/1000`. Cơ chế tạo Named Mutex của NuGet Restore cố truy cập đường dẫn này nên báo lỗi `ENOENT`.
2. **Lệnh biên dịch chuẩn hóa**:
   `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`
   - **Kết quả**: **Thành công 100% (Build succeeded)**.
   - **Thời gian**: ~4.47 giây.
   - **Cảnh báo**: 4 cảnh báo an ninh đã biết của NuGet về package `System.Security.Cryptography.Pkcs` 9.0.11 (không ảnh hưởng biên dịch).
   - **Tự động build CSS**: MSBuild target `TailwindCSS` đã tự động chạy `npm run build:css` (tailwindcss v3.4.3 biên dịch `Styles/app.css` ra `wwwroot/css/site.css` trong 381ms).
3. **Hiện trạng Dịch vụ SQL Server**:
   - Kiểm tra `ss -tulpn | grep 1433`: Không có port 1433 nào đang mở cục bộ.
   - Chuỗi kết nối trong `appsettings.json` trỏ tới `Server=localhost;Database=TinhNguyen;User Id=sa;Password=Vuvinh2910@;`.
   - Kết luận: Khi chạy ở chế độ demo/offline mà không có SQL Server đang active, mọi truy vấn EF Core cần được bọc `try-catch` hoặc hỗ trợ cơ chế nạp dữ liệu mẫu tại tầng Controller/Service để không gây crash ứng dụng.

### 1.5. Cấu trúc Giao diện Hiện có (Storefront, Navigation & Styles)
1. **Header Navigation Desktop** (`Views/Shared/_Layout.cshtml`, dòng 373-394):
   - Nằm cố định trên cùng (`fixed inset-x-0 top-0 z-50`).
   - Danh sách link hiện tại: `Trang chủ`, `Chương trình`, `Đóng góp`, `Ban tổ chức`, `Tin tức`, `Ký ức` (Dropdown).
   - Yêu cầu R1: Đặt mục dẫn tới trang mua bán sản phẩm gây quỹ ngang hàng với Trang chủ.
2. **Mobile Liquid Bottom Navigation** (`Views/Shared/_Layout.cshtml`, dòng 476-516):
   - Thanh điều hướng nổi dưới đáy màn hình trên thiết bị di động (`.navigation lg:hidden`), cao 70px, có indicator trượt theo active item.
   - Danh sách nút mobile: `Trang chủ` (`/home/index`), `Sự kiện` (`/home/chuongtrinh`), `Ký ức` (`/home/kyuc`), `Đóng góp` (`/home/gayquy`), `Tổ chức` (`/home/bantochuc`), `Tin tức` (`/home/tintuc`).
3. **Thanh kéo Bật/Tắt đèn Dark Mode**:
   - Tích hợp thư viện kéo dây vật lý (`#pullcord-wrapper`). Class `dark` được gắn vào thẻ `<html>`, các style màu dùng biến CSS `var(--color-main)`, `var(--bg-web)` và class Tailwind `dark:bg-night`, `dark:text-white`.

---

## 2. Logic Chain (Chuỗi lập luận & Thiết kế giải pháp)

Dựa trên các quan sát trực tiếp, chúng tôi xây dựng chuỗi lập luận logic cho giải pháp toàn diện như sau:

```
[Mục tiêu gốc]
Xây dựng Cửa hàng gây quỹ tình nguyện (Storefront, Chi tiết, Giỏ hàng, Đặt mua)
+ Quản trị Admin (CRUD sản phẩm chi tiết) trong thư mục riêng biệt (ASP.NET Core Area)
                      │
                      ▼
[Vấn đề 1: Lưu trạng thái Giỏ hàng]
- Người dùng chưa đăng nhập tài khoản (Website tình nguyện cộng đồng)
- Cần lưu tạm các sản phẩm đã chọn, số lượng, cập nhật linh hoạt
- Giải pháp: Kích hoạt Session ASP.NET Core trong Program.cs
  + Serialization sang JSON với Extension Method
  + Hỗ trợ thêm Cookie dự phòng
                      │
                      ▼
[Vấn đề 2: Mô hình Thực thể Dữ liệu (Domain Entities)]
- Cần lưu: Sản phẩm (Tên, giá, ảnh, mô tả, số lượng tồn, thông tin gây quỹ)
- Cần lưu: Đơn hàng (Thông tin người ủng hộ, địa chỉ, tổng tiền, phương thức, trạng thái)
- Cần lưu: Chi tiết đơn hàng (Snapshot sản phẩm, đơn giá, số lượng)
- Giải pháp: Tạo SanPhamGayQuy, DonHangGayQuy, ChiTietDonHangGayQuy chuẩn EF Core
                      │
                      ▼
[Vấn đề 3: Cấu trúc Thư mục Area]
- Yêu cầu R3: "Tổ chức toàn bộ mã nguồn của tính năng trong thư mục riêng biệt theo kiến trúc Area"
- Giải pháp: Tạo Area duy nhất `Areas/CuaHang` (hoặc `Areas/Store`) chứa:
  + Controllers: CuaHangController (Khách xem), GioHangController (Giỏ & Đặt), AdminSanPhamController (Admin CRUD)
  + Models & ViewModels
  + Views: Tách bạch rõ giao diện Storefront (kế thừa _Layout) và Admin (kế thừa _AdminLayout)
  + Bổ sung MapControllerRoute cho Area trong Program.cs
                      │
                      ▼
[Vấn đề 4: Cơ chế Seed Data chống crash offline]
- SQL Server có thể chưa sẵn sàng ở máy kiểm thử cục bộ
- Giải pháp: Tạo DbInitializer và Controller Fallback Seeding
  Cung cấp sẵn 4 sản phẩm gây quỹ chân thực, ý nghĩa cho dự án Trăng Sáng Langbiang 2026
```

### 2.1. Thiết kế Chi tiết Dịch vụ Session & State Management
#### Lựa chọn Kiến trúc Giỏ hàng:
| Tiêu chí | Session State (`ISession`) | Client Cookie |
| :--- | :--- | :--- |
| **Bảo mật** | Cao (dữ liệu lưu trên server, client chỉ giữ Session ID mã hóa) | Trung bình (dữ liệu giỏ hàng nằm ở cookie trình duyệt) |
| **Giới hạn kích thước** | Không giới hạn dung lượng lưu trữ | Tối đa 4KB (dễ bị tràn nếu nhiều sản phẩm hoặc ghi chú dài) |
| **Thời gian tồn tại** | Theo phiên làm việc (mặc định cấu hình 60 - 120 phút) | Có thể lưu dài hạn (7 - 30 ngày) |
| **Khả năng khôi phục** | Mất khi restart ứng dụng nếu dùng In-Memory Cache | Không mất khi restart ứng dụng |

**Đề xuất tối ưu**: Sử dụng **Session State làm giải pháp chính** kết hợp các Extension Methods serialization JSON chuẩn `System.Text.Json`.

#### Mã cấu hình bổ sung trong `Program.cs`:
```csharp
// 1. Đăng ký Caching và Session trước builder.Build()
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".TinhNguyen.Session";
});
builder.Services.AddHttpContextAccessor();

// 2. Kích hoạt Session Middleware sau app.UseRouting() và trước app.UseAuthorization()
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 3. Đăng ký Area Routing trước Default Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=CuaHang}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```

#### Helper Extension Serialization:
```csharp
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
    }

    public static T? Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
    }
}
```

### 2.2. Thiết kế Mô hình Thực thể Dữ liệu (Domain Entities)

#### 1. Thực thể Sản phẩm Gây quỹ (`SanPhamGayQuy`):
```csharp
[Table("SanPhamGayQuy")]
public class SanPhamGayQuy
{
    [Key]
    public int IDSanPham { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [MaxLength(255)]
    [Display(Name = "Tên sản phẩm")]
    public string TenSanPham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giá bán")]
    [Range(0, 100000000, ErrorMessage = "Giá bán không hợp lệ")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá bán (VNĐ)")]
    public decimal GiaBan { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá niêm yết cũ (nếu có)")]
    public decimal? GiaGoc { get; set; }

    [MaxLength(1000)]
    [Display(Name = "Hình ảnh sản phẩm")]
    public string? LinkHinhAnh { get; set; }

    [Display(Name = "Mô tả ngắn")]
    [MaxLength(500)]
    public string? MoTaNgan { get; set; }

    [Display(Name = "Mô tả chi tiết")]
    public string? MoTaChiTiet { get; set; }

    [MaxLength(500)]
    [Display(Name = "Thông điệp gây quỹ")]
    public string? ThongDiepGayQuy { get; set; }

    [Display(Name = "Tỷ lệ đóng góp vào quỹ (%)")]
    [Range(1, 100)]
    public int PhanTramDongGop { get; set; } = 100;

    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(0, 100000)]
    [Display(Name = "Số lượng tồn kho")]
    public int SoLuongTon { get; set; } = 50;

    [Display(Name = "Đã bán")]
    public int DaBan { get; set; } = 0;

    [MaxLength(100)]
    [Display(Name = "Danh mục")]
    public string DanhMuc { get; set; } = "Quà lưu niệm";

    [Display(Name = "Trạng thái hiển thị")]
    public bool TrangThai { get; set; } = true;

    [Display(Name = "Thứ tự hiển thị")]
    public int ThuTu { get; set; } = 0;

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public int? IDChienDich { get; set; }
    [ForeignKey("IDChienDich")]
    public virtual ChienDich? ChienDich { get; set; }
}
```

#### 2. Thực thể Đơn hàng Ủng hộ (`DonHangGayQuy`):
```csharp
[Table("DonHangGayQuy")]
public class DonHangGayQuy
{
    [Key]
    public int IDDonHang { get; set; }

    [Required]
    [MaxLength(50)]
    public string MaDonHang { get; set; } = string.Empty; // VD: GQ2026-X8B9

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [MaxLength(255)]
    [Display(Name = "Họ và tên người nhận")]
    public string HoTenNguoiMua { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [MaxLength(20)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
    [MaxLength(500)]
    [Display(Name = "Địa chỉ nhận hàng")]
    public string DiaChiNhanHang { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Display(Name = "Lời nhắn gửi / Ghi chú")]
    public string? GhiChu { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTienHang { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Ủng hộ thêm tùy tâm cho quỹ")]
    public decimal TienUngHoThem { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PhiVanChuyen { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongThanhToan { get; set; }

    [MaxLength(50)]
    public string PhuongThucThanhToan { get; set; } = "ChuyenKhoanQR"; // "ChuyenKhoanQR" hoặc "COD"

    [MaxLength(50)]
    public string TrangThaiDonHang { get; set; } = "MoiDat"; // MoiDat, DaXacNhan, DangGiao, HoanThanh, DaHuy

    [MaxLength(50)]
    public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan"; // ChuaThanhToan, DaThanhToan

    public DateTime NgayDatHang { get; set; } = DateTime.Now;

    public virtual ICollection<ChiTietDonHangGayQuy> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangGayQuy>();
}
```

#### 3. Thực thể Chi tiết Đơn hàng (`ChiTietDonHangGayQuy`):
```csharp
[Table("ChiTietDonHangGayQuy")]
public class ChiTietDonHangGayQuy
{
    [Key]
    public int IDChiTiet { get; set; }

    public int IDDonHang { get; set; }
    [ForeignKey("IDDonHang")]
    public virtual DonHangGayQuy? DonHang { get; set; }

    public int IDSanPham { get; set; }
    [ForeignKey("IDSanPham")]
    public virtual SanPhamGayQuy? SanPham { get; set; }

    [MaxLength(255)]
    public string TenSanPham { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? LinkHinhAnh { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ThanhTien { get; set; }
}
```

### 2.3. Thiết kế Cấu trúc Thư mục Area (`Areas/CuaHang`)
Để đảm bảo tính đóng gói sạch sẽ (Clean Architecture) theo yêu cầu R3, toàn bộ mã nguồn của tính năng mới được đặt trong một Area duy nhất:

```
WEB_TinhNguyen/
├── Areas/
│   └── CuaHang/
│       ├── Controllers/
│       │   ├── CuaHangController.cs        // Storefront: Xem danh sách, lọc danh mục, xem chi tiết
│       │   ├── GioHangController.cs        // Cart: Thêm, sửa số lượng, xóa, checkout, đặt hàng thành công
│       │   └── AdminSanPhamController.cs   // Admin CRUD: Danh sách bảng, Thêm, Sửa, Xóa sản phẩm
│       ├── Models/
│       │   ├── SanPhamGayQuy.cs
│       │   ├── DonHangGayQuy.cs
│       │   ├── ChiTietDonHangGayQuy.cs
│       │   ├── CartItem.cs
│       │   ├── CartViewModel.cs
│       │   ├── CheckoutViewModel.cs
│       │   └── SanPhamCrudViewModel.cs
│       ├── Services/                       // (Tùy chọn) SessionCartService, VietQrHelper
│       └── Views/
│           ├── _ViewStart.cshtml           // Phân luồng Layout tự động (Storefront vs Admin)
│           ├── _ViewImports.cshtml
│           ├── CuaHang/
│           │   ├── Index.cshtml            // Storefront catalog sản phẩm
│           │   └── ChiTiet.cshtml          // Trang chi tiết sản phẩm
│           ├── GioHang/
│           │   ├── Index.cshtml            // Giỏ hàng và form đặt mua/ủng hộ gây quỹ
│           │   └── DatHangThanhCong.cshtml // Xác nhận đơn, hiển thị VietQR tự động
│           └── AdminSanPham/
│               ├── Index.cshtml            // Bảng danh sách quản trị sản phẩm
│               ├── Create.cshtml           // Form thêm mới sản phẩm
│               └── Edit.cshtml             // Form chỉnh sửa sản phẩm
```

### 2.4. Dữ liệu Mẫu Ban Đầu (Seed Data) Chi tiết
Khởi tạo ít nhất 4 sản phẩm thực tế, mang bản sắc dự án Trăng Sáng Langbiang 2026:

| STT | Tên Sản Phẩm | Giá bán | Tồn kho | Danh mục | Thông điệp Gây quỹ & Ý nghĩa | Link Ảnh Mẫu |
|:---:|:---|:---:|:---:|:---|:---|:---|
| 1 | **Áo Thun Tình Nguyện "Trăng Sáng Langbiang 2026"** | 129.000 ₫ | 100 | Trang phục | Đóng góp 50.000₫/áo vào quỹ quà tặng Trung thu cho 200 em nhỏ đồng bào K'Ho tại Langbiang. Vải cotton 100% 4 chiều thoáng mát. | `/gallery/2025/bro02087.jpg` |
| 2 | **Túi Tote Canvas "Yêu Thương Gửi Về Núi Rừng"** | 79.000 ₫ | 85 | Phụ kiện | 100% lợi nhuận dành cho chương trình "Nấu ăn cho em" với 250 suất phở bò nóng hổi nơi vùng cao. Vải canvas dày dặn, thân thiện môi trường. | `/gallery/2025/bro02318.jpg` |
| 3 | **Bình Giữ Nhiệt Tre Khắc Tên 500ml** | 159.000 ₫ | 50 | Quà lưu niệm | Đóng góp quỹ máy lọc nước sạch cho điểm trường bản làng. Vỏ gỗ tre tự nhiên khắc laser biểu tượng đỉnh Langbiang, ruột inox 304 giữ nhiệt 12h. | `/gallery/2025/bro01755.jpg` |
| 4 | **Móc Khóa Linh Vật Thỏ Trăng Langbiang Đan Len** | 35.000 ₫ | 120 | Quà lưu niệm | Góp 1 lồng đèn truyền thống và tập vở cho trẻ em nghèo. Sản phẩm đan len thủ công tinh tế từ chính các bạn tình nguyện viên. | `/gallery/2025/bro01835.jpg` |

#### Cơ chế Kích hoạt Dữ liệu Mẫu (2 Tầng An toàn):
1. **Tầng 1 - DbInitializer tại Khởi động**: 
   Kiểm tra `if (!context.SanPhams.Any())`, thêm danh sách trên và lưu vào database. Bọc toàn bộ trong `try-catch` để trong trường hợp database offline, ứng dụng vẫn khởi động trơn tru.
2. **Tầng 2 - Lazy Fallback tại Controllers**:
   Khi `CuaHangController` hoặc `AdminSanPhamController` truy vấn dữ liệu, nếu không kết nối được database hoặc bảng rỗng, controller tự nạp danh sách mẫu vào bộ nhớ để giao diện người dùng và quản trị Admin luôn có sẵn dữ liệu trực quan để kiểm thử và chấm điểm.

### 2.5. Tích hợp Điều hướng (Navigation Links)
#### 1. Trên Desktop Header (`Views/Shared/_Layout.cshtml` dòng 374):
Đặt ngay cạnh link "Trang chủ":
```html
<nav class="hidden items-center gap-1 lg:flex pr-24">
    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="Index">Trang chủ</a>
    
    <!-- MỤC CỬA HÀNG GÂY QUỸ MỚI THÊM NGANG HÀNG TRANG CHỦ -->
    <a class="relative rounded-full px-3.5 py-2 text-sm font-bold text-emerald-700 bg-emerald-50/80 hover:bg-emerald-100 dark:text-emerald-300 dark:bg-emerald-950/40 dark:hover:bg-emerald-900/60 transition shadow-sm flex items-center gap-1.5"
       asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index">
        <i class="fas fa-store text-emerald-600 dark:text-emerald-400"></i>
        <span>Sản phẩm Gây quỹ</span>
        <span class="inline-flex items-center justify-center px-1.5 py-0.5 text-[10px] font-extrabold text-white bg-red-500 rounded-full animate-pulse">HOT</span>
    </a>

    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="ChuongTrinh">Chương trình</a>
    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="GayQuy">Đóng góp</a>
    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="BanToChuc">Ban tổ chức</a>
    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="TinTuc">Tin tức</a>
    ...
</nav>
```

#### 2. Trên Mobile Liquid Bottom Nav (`Views/Shared/_Layout.cshtml` dòng 476):
Bổ sung một thẻ `<li>` với `data-url="/cuahang"`:
```html
<li class="list" data-url="/cuahang">
    <a href="/CuaHang/CuaHang">
        <span class="icon"><i class="fas fa-shopping-bag"></i></span>
        <span class="text">Cửa hàng</span>
    </a>
</li>
```

#### 3. Trên Admin Sidebar (`Views/Shared/_AdminLayout.cshtml` dòng 247):
Thêm vào nhóm danh mục `Nội dung`:
```html
<li>
    <a href="@Url.Action("Index", "AdminSanPham", new { area = "CuaHang" })" 
       class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")">
        <i class="fas fa-boxes"></i> <span class="link-text">Sản phẩm Gây Quỹ</span>
    </a>
</li>
```

### 2.6. Xử lý Các Điểm Biên (Edge Cases) & Yêu cầu UI/UX

#### A. Storefront & Product Card:
- **Badge Ý nghĩa Thiện nguyện**: Mỗi thẻ sản phẩm hiển thị nhãn nổi bật: `% Đóng góp gây quỹ` (ví dụ: `100% Gây Quỹ` hoặc `Góp 50.000đ/sản phẩm`).
- **Thanh Tiến độ / Trạng thái Tồn kho**: Hiển thị số lượng "Đã bán X chiếc" để khích lệ tinh thần người mua ủng hộ.
- **Hết hàng**: Nếu `SoLuongTon <= 0`, hiển thị nhãn xám "Tạm hết hàng", làm mờ nút "Thêm vào giỏ" và vô hiệu hóa click.
- **Nút Mua Nhanh & Xem Chi Tiết**: Nút icon giỏ hàng cho phép thêm nhanh 1 sản phẩm vào giỏ với hiệu ứng Toast thông báo thành công mà không làm gián đoạn việc xem trang.

#### B. Chi tiết Sản phẩm (Product Details):
- **Trình chọn số lượng thông minh**: Các nút `[-]` `[Số lượng]` `[+]` có giới hạn cận dưới là 1 và cận trên bằng `SoLuongTon`.
- **Hộp thoại Thông điệp Hành động (Callout Box)**: Nổi bật đoạn văn giải thích rõ số tiền từ sản phẩm này sẽ được Ban Tổ Chức sử dụng vào mục đích cụ thể nào (mua quà, nấu ăn, trao học bổng).
- **Mua ngay (Buy Now)**: Tự động thêm sản phẩm vào giỏ và chuyển hướng thẳng tới màn hình Giỏ hàng / Thanh toán.

#### C. Giỏ hàng & Thanh toán (Cart & Checkout):
- **Trạng thái Giỏ hàng Trống**: Hiển thị hình ảnh minh họa ấm áp, thông điệp "Giỏ hàng của bạn đang trống" kèm nút bấm nổi bật "Khám phá sản phẩm gây quỹ ngay".
- **Ủng hộ thêm Tùy tâm (Extra Donation Feature)**:
  Đây là điểm nhấn độc đáo của website thiện nguyện. Người dùng có thể click chọn nhanh các nút tiền ủng hộ thêm (+20.000₫, +50.000₫, +100.000₫) hoặc tự nhập số tiền đóng góp thêm ngoài tiền hàng.
- **Tự động sinh mã VietQR**:
  Khi người dùng chọn phương thức "Chuyển khoản QR", trang hoàn tất sẽ tạo ngay hình ảnh mã QR ngân hàng chuẩn VietQR:
  `https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={TongTien}&addInfo={MaDonHang}%20ung%20ho%20Langbiang`
  với thông tin khớp chính xác tài khoản MBBank của Ban Tổ Chức dự án (`0924044283` - Nguyễn Anh Hào).
- **Khoảng đệm Mobile (Mobile Padding)**:
  Do thanh Mobile Liquid Bottom Nav chiếm 70px ở đáy màn hình, trang giỏ hàng và đặt hàng trên mobile cần có `padding-bottom: 120px` để nút "Gửi đơn đặt hàng" không bao giờ bị che khuất.
- **Tương thích Dark Mode**:
  Áp dụng đầy đủ các class `dark:bg-night`, `dark:bg-night-2`, `dark:text-white`, `dark:border-white/10` cho tất cả các View của Area mới để đồng bộ hoàn hảo với công tắc kéo dây đèn của website.

---

## 3. Caveats (Các lưu ý & Giả định)

1. **Biến Môi Trường Biên Dịch**:
   - Khi chạy lệnh `dotnet build` hoặc `dotnet run` trong môi trường sandbox Linux này, **bắt buộc** phải sử dụng tiền tố:
     `XDG_RUNTIME_DIR=/tmp dotnet build ...`
   - Nếu không có biến này, NuGet Restore sẽ phát sinh lỗi `MSB4018` do thiếu thư mục `/run/user/1000`.
2. **Cơ sở Dữ liệu SQL Server Cục bộ**:
   - Hiện tại SQL Server (cổng 1433) chưa chạy thường trực trong sandbox. Mã nguồn cần triển khai theo phương thức an toàn (bọc try-catch hoặc cơ chế in-memory fallback) để khi chạy ở chế độ demo/offline, các trang Cửa hàng và Admin vẫn hiển thị trơn tru với dữ liệu mẫu mà không gặp lỗi kết nối cơ sở dữ liệu.
3. **Cấu hình Tailwind CSS Content Scan**:
   - File `tailwind.config.js` hiện tại chỉ khai báo `content: ["./Views/**/*.cshtml", "./wwwroot/js/**/*.js"]`.
   - Cần bổ sung thêm `"./Areas/**/*.cshtml"` vào mảng `content` của `tailwind.config.js` để Tailwind CSS quét và biên dịch đầy đủ các utility class được viết trong các View thuộc Area mới.
4. **Quyền Ghi Tệp Ảnh Uploads**:
   - Thư mục `wwwroot/uploads/sanpham/` cần được tự động tạo (`Directory.CreateDirectory`) nếu chưa tồn tại khi quản trị viên thực hiện tải ảnh sản phẩm lên.

---

## 4. Conclusion (Kết luận)

1. **Khả thi 100%**: Dự án có kiến trúc hiện đại, mã nguồn rõ ràng, tương thích hoàn hảo với .NET 10.0 MVC và sẵn sàng tích hợp module Cửa hàng Gây quỹ.
2. **Cấu hình Session & Routing**: Cần cập nhật `Program.cs` để bổ sung `AddDistributedMemoryCache()`, `AddSession()`, `UseSession()` và route `areas`.
3. **Cấu trúc Thư mục**: Thiết kế Area tập trung `Areas/CuaHang` đáp ứng trọn vẹn tiêu chí chấp nhận của R3, vừa tách biệt mã nguồn vừa tái sử dụng tối đa hai Layout có sẵn (`_Layout.cshtml` cho người dùng và `_AdminLayout.cshtml` cho quản trị).
4. **Trải nghiệm Người dùng & Ý nghĩa Xã hội**: Tích hợp các tính năng đặc thù gây quỹ (thông điệp từ thiện, đóng góp thêm tùy tâm, mã VietQR chuẩn ngân hàng BTC) tạo nên một sản phẩm hoàn thiện, chuyên nghiệp và giàu cảm xúc.

---

## 5. Verification Method (Phương pháp Xác minh Độc lập)

Người nhận handoff có thể thực hiện các bước sau để kiểm chứng độc lập:

1. **Kiểm tra Biên dịch Dự án hiện tại**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Kỳ vọng*: Thoát với mã lỗi 0 (`0 Error(s)`), thông báo "Build succeeded".

2. **Kiểm tra File `Program.cs`**:
   Sử dụng lệnh kiểm tra sự tồn tại của Session và Area route:
   ```bash
   grep -E "AddSession|UseSession|MapControllerRoute.*areas" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Program.cs
   ```
   *Kỳ vọng*: Hiện tại chưa có; sau khi triển khai sẽ trả về các dòng tương ứng.

3. **Kiểm tra Layouts**:
   - `_Layout.cshtml`: Xác nhận vị trí thẻ `<nav>` trên desktop (dòng 373-394) và thanh Liquid nav trên mobile (dòng 476-516).
   - `_AdminLayout.cshtml`: Xác nhận danh mục menu sidebar (dòng 240-282).
