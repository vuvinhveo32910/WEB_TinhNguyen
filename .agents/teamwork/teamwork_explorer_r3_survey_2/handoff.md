# BÁO CÁO KHẢO SÁT & THIẾT KẾ KIẾN TRÚC VIỆT HÓA MVC (YÊU CẦU R2)
**Vai trò**: C# MVC Architecture Explorer (Lập trình viên C# - .NET 10 MVC Structure & Routing)  
**Mã đặc tả nhiệm vụ**: Requirement R2 — Việt hóa cấu trúc thư mục và cấu hình MVC  
**Môi trường thực thi**: .NET SDK 10.0.114 | TargetFramework `net10.0` | Linux x86_64  

---

## 1. Observation (Quan Sát Thực Nghiệm & Bằng Chứng Mã Nguồn)

### 1.1. Hiện trạng cấu trúc thư mục & kiểm kê tệp tin (File Inventory)
Dự án web ASP.NET Core MVC nằm tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen`. Kết quả quét toàn bộ cây thư mục cho 4 phân vùng chính như sau:

1. **`Controllers/` (9 tệp tin C#)**:
   - `Controllers/AdminBanToChucController.cs` (Line 13: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminChuongTrinhController.cs` (Line 13: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminController.cs` (Line 3: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminGayQuyController.cs` (Line 12: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminHieuUngController.cs` (Line 12: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminKyUcController.cs` (Line 12: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminTinTucController.cs` (Line 13: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/AdminTrangChuController.cs` (Line 13: `namespace WEB_TinhNguyen.Controllers`)
   - `Controllers/HomeController.cs` (Line 11: `namespace WEB_TinhNguyen.Controllers`)

2. **`Models/` (3 tệp tin C#)**:
   - `Models/CauHinhTrangChuVM.cs` (Line 5: `namespace WEB_TinhNguyen.Models`)
   - `Models/Entities.cs` (Line 6: `namespace WEB_TinhNguyen.Models`)
   - `Models/ErrorViewModel.cs` (Line 1: `namespace WEB_TinhNguyen.Models`)

3. **`Data/` (1 tệp tin C#)**:
   - `Data/ApplicationDbContext.cs` (Line 5: `namespace WEB_TinhNguyen.Data`)

4. **`Views/` (29 tệp tin `.cshtml`, `.css`, `.js` trải dài trên 10 thư mục con)**:
   - Root: `_ViewImports.cshtml`, `_ViewStart.cshtml`
   - `Admin/`: `Index.cshtml`
   - `AdminBanToChuc/`: `Index.cshtml`, `Edit.cshtml`
   - `AdminChuongTrinh/`: `Index.cshtml`, `Edit.cshtml`
   - `AdminGayQuy/`: `Index.cshtml`, `Edit.cshtml`
   - `AdminHieuUng/`: `Index.cshtml`
   - `AdminKyUc/`: `Index.cshtml`, `Edit.cshtml`
   - `AdminTinTuc/`: `Index.cshtml`, `Edit.cshtml`
   - `AdminTrangChu/`: `Index.cshtml`, `Edit.cshtml`
   - `Home/`: `Index.cshtml`, `BanToChuc.cshtml`, `ChuongTrinh.cshtml`, `ComingSoon.cshtml`, `GayQuy.cshtml`, `KyUc.cshtml`, `LoiChuc.cshtml`, `Privacy.cshtml`, `TinTuc.cshtml`
   - `Shared/`: `Error.cshtml`, `_AdminLayout.cshtml`, `_Layout.cshtml`, `_Layout.cshtml.css`, `_ValidationScriptsPartial.cshtml`, `hoamai.js`

5. **`Areas/CuaHang/` (13 tệp tin mã nguồn & giao diện)**:
   - `Controllers/` (4 tệp): `AdminDonHangController.cs`, `AdminSanPhamController.cs`, `CuaHangController.cs`, `GioHangController.cs` (đều có `[Area("CuaHang")]`).
   - `Helpers/` (2 tệp): `DbInitializer.cs`, `SessionExtensions.cs`.
   - `Models/` (7 tệp): `CartItem.cs`, `CartViewModel.cs`, `CheckoutViewModel.cs`, `ChiTietDonHangGayQuy.cs`, `DonHangGayQuy.cs`, `SanPhamCrudViewModel.cs`, `SanPhamGayQuy.cs`.
   - `Views/` (10 tệp view + imports): `AdminDonHang/` (ChiTiet, Index), `AdminSanPham/` (Create, Edit, Index), `CuaHang/` (ChiTiet, Index), `GioHang/` (DatHangThanhCong, Index), `_ViewImports.cshtml`, `_ViewStart.cshtml`.

### 1.2. Môi trường biên dịch .NET 10 & Lỗi Sandbox Đặc thù
- Tệp `WEB_TinhNguyen.csproj`:
  ```xml
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.12">
  ```
- **Lỗi Sandbox khi chạy `dotnet build` thông thường**:
  Lệnh `dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj` trả về lỗi thoát mã 1:
  ```text
  /usr/lib/dotnet/sdk/10.0.114/NuGet.targets(196,5): error MSB4018: System.IO.IOException: The system cannot open the device or file specified. : 'NuGet-Migrations'. One or more system calls failed: lstat("/run/user/1000", ...) == -1; errno == ENOENT;
  ```
- **Giải pháp kiểm chứng**: Chạy với tiền tố môi trường `XDG_RUNTIME_DIR=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj` thành công 100% với 0 lỗi (0 Errors, 4 Warnings từ gói transitive `System.Security.Cryptography.Pkcs`).

### 1.3. Cạm bẫy đường dẫn Layout tuyệt đối (22 vị trí hardcoded)
Quét toàn bộ codebase phát hiện **22 tệp View** đang gán layout thông qua chuỗi ký tự ảo bắt đầu bằng `~/Views/Shared/`:
- **7 tệp trong `Areas/CuaHang/Views/`**:
  - `Areas/CuaHang/Views/_ViewStart.cshtml`:
    - Line 10: `Layout = "~/Views/Shared/_AdminLayout.cshtml";`
    - Line 14: `Layout = "~/Views/Shared/_Layout.cshtml";`
  - `Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml` (Line 4)
  - `Areas/CuaHang/Views/AdminDonHang/Index.cshtml` (Line 4)
  - `Areas/CuaHang/Views/AdminSanPham/Create.cshtml` (Line 4)
  - `Areas/CuaHang/Views/AdminSanPham/Edit.cshtml` (Line 4)
  - `Areas/CuaHang/Views/AdminSanPham/Index.cshtml` (Line 4)
- **15 tệp trong `Views/`**:
  - `Views/Home/Index.cshtml` (Line 4: `Layout = "~/Views/Shared/_Layout.cshtml";`)
  - `Views/Admin/Index.cshtml` (Line 3: `Layout = "~/Views/Shared/_AdminLayout.cshtml";`)
  - `Views/AdminBanToChuc/Edit.cshtml`, `Index.cshtml`
  - `Views/AdminChuongTrinh/Edit.cshtml`, `Index.cshtml`
  - `Views/AdminGayQuy/Edit.cshtml`, `Index.cshtml`
  - `Views/AdminHieuUng/Index.cshtml`
  - `Views/AdminKyUc/Edit.cshtml`, `Index.cshtml`
  - `Views/AdminTinTuc/Edit.cshtml`, `Index.cshtml`
  - `Views/AdminTrangChu/Edit.cshtml`, `Index.cshtml`

*Hậu quả nếu không sửa*: Khi đổi tên thư mục `Views` -> `GiaoDien`, Razor compiler tại thời điểm build không báo lỗi nhưng khi người dùng truy cập web tại runtime, hệ thống sẽ quăng `InvalidOperationException: The layout view '~/Views/Shared/_Layout.cshtml' could not be found` làm crash 500 toàn trang.

### 1.4. Cạm bẫy quét nội dung Tailwind CSS (`tailwind.config.js`)
Trong `tailwind.config.js`:
```javascript
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./WEB_TinhNguyen/Views/**/*.cshtml",
    "./WEB_TinhNguyen/Areas/**/*.cshtml",
    "./WEB_TinhNguyen/wwwroot/js/**/*.js",
    "./wwwroot/js/**/*.js"
  ],
```
*Hậu quả nếu không sửa*: `WEB_TinhNguyen.csproj` có target tự động `TailwindCSS` chạy `npm run build:css` trước mỗi lần build. Nếu `tailwind.config.js` chỉ lắng nghe `Views/`, sau khi đổi sang `GiaoDien/`, Tailwind CLI sẽ không tìm thấy bất kỳ view nào để biên dịch CSS utility, khiến toàn bộ giao diện bị vỡ style!

---

## 2. Logic Chain (Lập Luận Phân Tích & Giải Pháp Kỹ Thuật)

### 2.1. Phân định cơ chế định tuyến Controller vs Phân giải Razor View
1. **Cơ chế khám phá Controller trong ASP.NET Core MVC**:
   - `AddControllersWithViews()` sử dụng `DefaultApplicationPartManager` cùng `ControllerFeatureProvider`.
   - Cơ chế này nạp tất cả các kiểu dữ liệu công khai (public class) có hậu tố `Controller` kế thừa từ `ControllerBase` hoặc `Controller` trong Assembly `WEB_TinhNguyen.dll`.
   - **Lập luận**: ASP.NET Core hoàn toàn không phụ thuộc vào tên thư mục vật lý chứa Controller. Dù Controller nằm ở `Controllers/` hay `BoDieuKhien/`, MVC đều tự động nhận diện.
   - Định tuyến `app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")` ánh xạ token `{controller}` theo tên class `HomeController` -> `"Home"`. Do đó, giữ nguyên tên class `HomeController`, `AdminTinTucController`,... đảm bảo mọi URL và thẻ TagHelper `<a asp-controller="Home" asp-action="Index">` tiếp tục hoạt động 100% không bị gãy link.

2. **Cơ chế phân giải Razor ViewEngine**:
   - Khác với Controller, Razor ViewEngine phân giải file `.cshtml` dựa trên các mẫu chuỗi định vị đường dẫn (`ViewLocationFormats` và `AreaViewLocationFormats`).
   - Mặc định ViewEngine chỉ tìm trong `/Views/{1}/{0}.cshtml` và `/Views/Shared/{0}.cshtml`.
   - **Lập luận**: Khi đổi tên thư mục vật lý thành `GiaoDien`, ViewEngine mặc định sẽ tìm thất bại. Bắt buộc phải cấu hình lại `RazorViewEngineOptions` và bổ sung custom `IViewLocationExpander` để ViewEngine tự động ánh xạ `/GiaoDien/{1}/{0}.cshtml`.

### 2.2. Thiết kế cấu hình Razor ViewEngine tối ưu
Để đảm bảo khả năng tương thích cao nhất, triển khai cả 2 tầng:

1. **Tầng 1: Cấu hình tĩnh `RazorViewEngineOptions` trong `Program.cs`**:
   ```csharp
   builder.Services.Configure<RazorViewEngineOptions>(options =>
   {
       // Định dạng View cho phân hệ MVC gốc (Root)
       options.ViewLocationFormats.Clear();
       options.ViewLocationFormats.Add("/GiaoDien/{1}/{0}.cshtml");
       options.ViewLocationFormats.Add("/GiaoDien/Shared/{0}.cshtml");
       options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");       // Fallback an toàn
       options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml"); // Fallback an toàn

       // Định dạng View cho Phân hệ (Areas: CuaHang)
       options.AreaViewLocationFormats.Clear();
       options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
       options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
       options.AreaViewLocationFormats.Add("/Areas/{2}/GiaoDien/{1}/{0}.cshtml");
       options.AreaViewLocationFormats.Add("/Areas/{2}/GiaoDien/Shared/{0}.cshtml");
       options.AreaViewLocationFormats.Add("/GiaoDien/Shared/{0}.cshtml"); // Dùng chung layout từ GiaoDien/Shared
       options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
   });
   ```

2. **Tầng 2: Custom `VietnameseViewLocationExpander : IViewLocationExpander`**:
   Tạo tệp `DuLieu/VietnameseViewLocationExpander.cs`:
   ```csharp
   using Microsoft.AspNetCore.Mvc.Razor;

   namespace WEB_TinhNguyen.DuLieu
   {
       public class VietnameseViewLocationExpander : IViewLocationExpander
       {
           public void PopulateValues(ViewLocationExpanderContext context)
           {
           }

           public IEnumerable<string> ExpandViewLocations(
               ViewLocationExpanderContext context,
               IEnumerable<string> viewLocations)
           {
               if (!string.IsNullOrEmpty(context.AreaName))
               {
                   return new[]
                   {
                       "/Areas/{2}/Views/{1}/{0}.cshtml",
                       "/Areas/{2}/Views/Shared/{0}.cshtml",
                       "/Areas/{2}/GiaoDien/{1}/{0}.cshtml",
                       "/Areas/{2}/GiaoDien/Shared/{0}.cshtml",
                       "/GiaoDien/Shared/{0}.cshtml",
                       "/Views/Shared/{0}.cshtml"
                   };
               }

               return new[]
               {
                   "/GiaoDien/{1}/{0}.cshtml",
                   "/GiaoDien/Shared/{0}.cshtml",
                   "/Views/{1}/{0}.cshtml",
                   "/Views/Shared/{0}.cshtml"
               };
           }
       }
   }
   ```
   Đăng ký vào DI trong `Program.cs`:
   ```csharp
   builder.Services.Configure<RazorViewEngineOptions>(options =>
   {
       options.ViewLocationExpanders.Add(new VietnameseViewLocationExpander());
   });
   ```

### 2.3. Chiến lược Namespace và Khả năng tương thích ngược Migration
1. **Đổi tên namespace đồng bộ theo thư mục**:
   - `Controllers/` -> `BoDieuKhien/`: Sử dụng `namespace WEB_TinhNguyen.BoDieuKhien;`
   - `Models/` -> `MoHinh/`: Sử dụng `namespace WEB_TinhNguyen.MoHinh;`
   - `Data/` -> `DuLieu/`: Sử dụng `namespace WEB_TinhNguyen.DuLieu;`

2. **Bảo toàn 14 Migration EF Core hiện có**:
   Các tệp trong `Migrations/` có khai báo `using WEB_TinhNguyen.Data;` và `[DbContext(typeof(ApplicationDbContext))]`. Để không làm hỏng lịch sử migration, bổ sung tệp tương thích ngược `DuLieu/Compatibility.cs`:
   ```csharp
   namespace WEB_TinhNguyen.Data
   {
       [System.Obsolete("Sử dụng WEB_TinhNguyen.DuLieu.ApplicationDbContext")]
       public class ApplicationDbContext : WEB_TinhNguyen.DuLieu.ApplicationDbContext
       {
           public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<WEB_TinhNguyen.DuLieu.ApplicationDbContext> options)
               : base(options) { }
       }
   }
   ```
   Hoặc khai báo `global using WEB_TinhNguyen.DuLieu;` và `global using WEB_TinhNguyen.MoHinh;` trong `DuLieu/GlobalUsings.cs`.

### 2.4. Tương tác và Bảo toàn phân hệ Bán hàng (`Areas/CuaHang/`)
- **Theo yêu cầu R3**: Phân hệ `Areas/CuaHang/` phải được "bảo toàn 100% cấu trúc MVC/DOM và giữ nguyên toàn bộ giao diện hiện tại, cô lập hoàn toàn vùng này".
- Do đó:
  1. Thư mục nội bộ của `Areas/CuaHang/` **GIỮ NGUYÊN**: `Areas/CuaHang/Controllers`, `Areas/CuaHang/Models`, `Areas/CuaHang/Views`, `Areas/CuaHang/Helpers`. Tuyệt đối không đổi tên thành `Areas/CuaHang/BoDieuKhien`.
  2. Tại 7 vị trí tham chiếu layout trong `Areas/CuaHang/Views/`: Cập nhật chuỗi đường dẫn từ `~/Views/Shared/...` sang `~/GiaoDien/Shared/...`.
  3. Trong `Areas/CuaHang/Views/_ViewImports.cshtml`: Cập nhật `@using WEB_TinhNguyen.Models` -> `@using WEB_TinhNguyen.MoHinh`.
  4. Trong `Areas/CuaHang/Models/SanPhamGayQuy.cs`: Cập nhật `using WEB_TinhNguyen.Models;` -> `using WEB_TinhNguyen.MoHinh;`.
  5. Trong 4 Area Controller và `DbInitializer.cs`: Cập nhật `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`.

---

## 3. Caveats (Các Điểm Lưu Ý & Rủi Ro Cần Quản Trị)

1. **Hệ điều hành Linux phân biệt chữ hoa / chữ thường (Case Sensitivity)**:
   Trên môi trường Linux, `/GiaoDien/` khác `/giaodien/`. Mọi chuỗi định nghĩa trong `ViewLocationFormats`, tệp layout, và tên thư mục vật lý phải tuân thủ chính xác chuẩn PascalCase: `BoDieuKhien`, `MoHinh`, `GiaoDien`, `DuLieu`.
2. **Kế thừa tệp cấu hình Razor (`_ViewImports.cshtml`, `_ViewStart.cshtml`)**:
   Khi chuyển `Views/` sang `GiaoDien/`, 2 tệp `_ViewImports.cshtml` và `_ViewStart.cshtml` phải nằm trực tiếp dưới thư mục gốc `GiaoDien/`. Nếu để sót ở thư mục cũ, toàn bộ view trong `GiaoDien/` sẽ mất định nghĩa layout và TagHelper của ASP.NET Core.
3. **Thứ tự thực thi Git (Git Renaming)**:
   Để bảo toàn lịch sử commit trên Git, bước đổi tên thư mục nên được thực hiện bằng `git mv WEB_TinhNguyen/Controllers WEB_TinhNguyen/BoDieuKhien` (hoặc di chuyển file có giám sát git status).
4. **Tương thích R1 (Refactor Database & DbContext)**:
   Đội ngũ thực hiện R1 sẽ chỉnh sửa các model và DbContext (xóa các cột màu sắc, đổi tên bảng/cột tiếng Việt). Cần phối hợp để R2 thực hiện đổi tên cấu trúc thư mục trước hoặc cùng lúc với R1 nhằm tránh xung đột namespace `WEB_TinhNguyen.MoHinh` và `WEB_TinhNguyen.DuLieu`.

---

## 4. Conclusion (Kết Luận & Kế Hoạch Triển Khai Chi Tiết)

### Bảng Kế Hoạch Đổi Tên Thư Mục Chuẩn Mực

| Thư mục hiện tại | Thư mục tiếng Việt chuẩn | Số lượng tệp | Namespace mới | Ghi chú tương thích |
|---|---|---|---|---|
| `WEB_TinhNguyen/Controllers` | `WEB_TinhNguyen/BoDieuKhien` | 9 tệp | `WEB_TinhNguyen.BoDieuKhien` | ASP.NET Core tự động nhận diện qua hậu tố `*Controller` |
| `WEB_TinhNguyen/Models` | `WEB_TinhNguyen/MoHinh` | 3 tệp | `WEB_TinhNguyen.MoHinh` | Cập nhật using tại Controller, DbContext, ViewImports |
| `WEB_TinhNguyen/Views` | `WEB_TinhNguyen/GiaoDien` | 29 tệp (10 dir) | *N/A (Razor Views)* | Cấu hình `ViewLocationFormats` và `IViewLocationExpander` |
| `WEB_TinhNguyen/Data` | `WEB_TinhNguyen/DuLieu` | 1 tệp | `WEB_TinhNguyen.DuLieu` | Bổ sung alias tương thích ngược cho Migration |
| `WEB_TinhNguyen/Areas/CuaHang` | `WEB_TinhNguyen/Areas/CuaHang` | 13 tệp | `WEB_TinhNguyen.Areas.CuaHang.*` | **Giữ nguyên 100%** theo yêu cầu bảo toàn R3 |

### Danh mục các bước thực hiện mã nguồn (Action Items)
1. **Di chuyển thư mục**:
   - `git mv WEB_TinhNguyen/Controllers WEB_TinhNguyen/BoDieuKhien`
   - `git mv WEB_TinhNguyen/Models WEB_TinhNguyen/MoHinh`
   - `git mv WEB_TinhNguyen/Views WEB_TinhNguyen/GiaoDien`
   - `git mv WEB_TinhNguyen/Data WEB_TinhNguyen/DuLieu`
2. **Cập nhật `tailwind.config.js`**:
   Thêm `./WEB_TinhNguyen/GiaoDien/**/*.cshtml` và `./GiaoDien/**/*.cshtml` vào mảng `content`.
3. **Tạo `VietnameseViewLocationExpander.cs` và `GlobalUsings.cs`** trong `DuLieu/`.
4. **Cập nhật `Program.cs`**:
   - Đổi `using WEB_TinhNguyen.Data;` -> `using WEB_TinhNguyen.DuLieu;`.
   - Cấu hình `RazorViewEngineOptions.ViewLocationFormats` và `AreaViewLocationFormats`.
   - Đăng ký `VietnameseViewLocationExpander`.
5. **Cập nhật 22 tệp View & Layout**:
   - Thay thế toàn bộ `~/Views/Shared/_AdminLayout.cshtml` thành `~/GiaoDien/Shared/_AdminLayout.cshtml`.
   - Thay thế toàn bộ `~/Views/Shared/_Layout.cshtml` thành `~/GiaoDien/Shared/_Layout.cshtml`.
6. **Cập nhật `_ViewImports.cshtml`** (cả ở `GiaoDien/` và `Areas/CuaHang/Views/`):
   Đổi `@using WEB_TinhNguyen.Models` -> `@using WEB_TinhNguyen.MoHinh`.
7. **Cập nhật `_Layout.cshtml`**:
   Đổi `@using WEB_TinhNguyen.Data` -> `@using WEB_TinhNguyen.DuLieu`.

---

## 5. Verification Method (Quy Trình Kiểm Tra & Thẩm Định Độc Lập)

Người kiểm thử hoặc Challenger có thể thẩm định độc lập theo các bước sau:

1. **Kiểm tra biên dịch dự án**:
   ```bash
   cd /home/hutech/Vinh_Offline/WEB_TinhNguyen
   XDG_RUNTIME_DIR=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Điều kiện đạt*: Quá trình biên dịch kết thúc thành công với mã 0 (0 Error), Tailwind CSS biên dịch hoàn tất.

2. **Kiểm tra phân giải View tại Runtime (Không bị lỗi 500 do thiếu View/Layout)**:
   Khởi chạy ứng dụng:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet run --project WEB_TinhNguyen/WEB_TinhNguyen.csproj --urls "http://localhost:5000"
   ```
   Thực hiện các lệnh `curl` kiểm tra mã phản hồi HTTP 200:
   - `curl -I http://localhost:5000/` (Phân giải `GiaoDien/Home/Index.cshtml` + Layout)
   - `curl -I http://localhost:5000/Home/TinTuc` (Phân giải `GiaoDien/Home/TinTuc.cshtml`)
   - `curl -I http://localhost:5000/Admin` (Phân giải `GiaoDien/Admin/Index.cshtml` + AdminLayout)
   - `curl -I http://localhost:5000/CuaHang` (Phân giải `Areas/CuaHang/Views/CuaHang/Index.cshtml`)
   - `curl -I http://localhost:5000/CuaHang/GioHang` (Phân giải `Areas/CuaHang/Views/GioHang/Index.cshtml`)
   - `curl -I http://localhost:5000/CuaHang/AdminSanPham` (Phân giải `Areas/CuaHang/Views/AdminSanPham/Index.cshtml`)
   *Điều kiện đạt*: Toàn bộ các endpoint đều trả về HTTP 200 OK với đầy đủ mã HTML và không có ngoại lệ thiếu view.

3. **Điều kiện hủy bỏ (Invalidation Conditions)**:
   - Nếu xuất hiện lỗi `InvalidOperationException: The view 'X' was not found. The following locations were searched...` -> Cấu hình `ViewLocationFormats` chưa phủ đủ thư mục.
   - Nếu xuất hiện lỗi `InvalidOperationException: The layout view '~/Views/Shared/...' could not be found` -> Còn sót chuỗi path tuyệt đối cũ chưa được thay thế sang `~/GiaoDien/`.
   - Nếu lệnh build báo `error MSB4018 ... lstat("/run/user/1000") == -1` -> Chưa thiết lập biến môi trường `XDG_RUNTIME_DIR=/tmp`.
