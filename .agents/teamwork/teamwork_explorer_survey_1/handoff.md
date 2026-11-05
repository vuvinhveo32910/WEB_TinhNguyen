# Báo Cáo Khảo Sát Hệ Thống & Kiến Trúc Dự Án (System Survey Report)

## Tóm tắt cốt lõi (Executive Summary)
Dự án là ứng dụng Web ASP.NET Core (.NET 10.0) sử dụng Entity Framework Core 10.0.12 với cơ sở dữ liệu SQL Server. Giao diện người dùng sử dụng Tailwind CSS kết hợp FontAwesome và Google Fonts, hỗ trợ Responsive hoàn chỉnh với thanh điều hướng Desktop cố định và thanh Liquid Bottom Navigation trên thiết bị di động; giao diện Admin sở hữu Sidebar co giãn mượt mà. Dự án biên dịch thành công 100% (0 lỗi) khi cấu hình biến môi trường `XDG_RUNTIME_DIR=/tmp`. Tính năng cửa hàng gây quỹ mới sẽ được tích hợp thuận lợi theo mô hình ASP.NET Core Area với DbContext mở rộng, hỗ trợ giỏ hàng qua Session và cơ chế tự động Seed dữ liệu mẫu.

---

## 1. Quan sát trực tiếp (Direct Observations)

### 1.1 Cấu trúc Solution & Phiên bản .NET
- **File Solution**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
  ```xml
  <Solution>
    <Project Path="WEB_TinhNguyen/WEB_TinhNguyen.csproj" />
  </Solution>
  ```
- **Project File**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`
  - Target Framework: `net10.0` (SDK phiên bản 10.0.114 trên Ubuntu 24.04).
  - Implicit Usings: `enable`, Nullable: `enable`.
  - NuGet Packages:
    - `Microsoft.EntityFrameworkCore` v10.0.12
    - `Microsoft.EntityFrameworkCore.SqlServer` v10.0.12
    - `Microsoft.EntityFrameworkCore.Tools` v10.0.12
  - Pre-build Target:
    ```xml
    <Target Name="TailwindCSS" BeforeTargets="Build">
        <Exec Command="npm run build:css" />
    </Target>
    ```

### 1.2 Môi trường thực thi & Lệnh biên dịch (`dotnet build`)
- Khi thực thi lệnh mặc định `dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`, xuất hiện lỗi:
  ```
  /usr/lib/dotnet/sdk/10.0.114/NuGet.targets(196,5): error MSB4018: System.IO.IOException: The system cannot open the device or file specified. : 'NuGet-Migrations'. One or more system calls failed: lstat("/run/user/1000", ...) == -1; errno == ENOENT;
  ```
- Nguyên nhân: Trong môi trường sandbox Linux, thư mục `/run/user/1000` không tồn tại, khiến cơ chế Named Mutex của NuGet Restore trên .NET bị nghẽn.
- Giải pháp xác minh: Thêm `XDG_RUNTIME_DIR=/tmp`:
  ```bash
  XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
  ```
- Kết quả: **Build succeeded (0 Error(s), 4 Warning(s) về vulnerability đã biết của package Pkcs)**. Quá trình chạy `npm run build:css` tự động hoàn tất trong 424ms.

### 1.3 Database Context, Provider & Migrations
- **DbContext Path**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Data/ApplicationDbContext.cs`
  - Đang kế thừa `Microsoft.EntityFrameworkCore.DbContext`.
  - Khai báo các `DbSet`: `ChienDichs`, `CauHinhTrangChus`, `CauHinhAnhNois`, `BannerTrangChus`, `ChuongTrinhDemNguocs`, `CauHinhKyUcs`, `AnhKyUcs`, `NhaTaiTroKyUcs`, `CauHinhChuongTrinhs`, `ChangChuongTrinhs`, `ChiTietLoTrinhs`, `CauHinhGayQuys`, `KenhDongGops`, `NhaTaiTroGayQuys`, `CauHinhBanToChucs`, `ThanhVienBanToChucs`, `CauHinhTinTucs`, `BaiVietTinTucs`, `ThuVienHieuUngs`.
  - Chưa có DbSet cho `SanPham`, `DonHang`, hay `ChiTietDonHang`.
- **Connection Strings** (`appsettings.json`):
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TinhNguyen;User Id=sa;Password=Vuvinh2910@;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
  ```
- **Migrations**: Thư mục `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/` chứa 27 file migrations (từ `20261006032230_AddBannerTable` đến `20261014042058_AddMascot` và `ApplicationDbContextModelSnapshot.cs`). Provider mục tiêu là `SqlServerModelBuilderExtensions.UseIdentityColumns`.
- Hiện tại cổng 1433 local không mở trong môi trường sandbox (không có service mssql chạy nền trong sandbox).

### 1.4 Thành phần Giao diện & Điều hướng (Layouts & Navigation)
- **Storefront Client Layout** (`/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Views/Shared/_Layout.cshtml`):
  - **Desktop Navigation** (dòng 373-394):
    ```html
    <nav class="hidden items-center gap-1 lg:flex pr-24">
        <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="Index">Trang chủ</a>
        <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="ChuongTrinh">Chương trình</a>
        <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="GayQuy">Đóng góp</a>
        <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="BanToChuc">Ban tổ chức</a>
        <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="TinTuc">Tin tức</a>
        ...
    </nav>
    ```
  - **Mobile Liquid Bottom Navigation** (dòng 476-516):
    Thanh điều hướng nổi dưới chân trang (`.navigation lg:hidden`) có JavaScript tính toán vị trí vòng tròn chỉ báo (`indicator`) theo thuộc tính `data-url`.
  - **Footer Links** (dòng 454-458): Khối danh mục "Khám phá" chứa danh sách các trang chính.
  - **Thư viện UI/CSS**: Nhúng Tailwind CSS qua CDN `<script src="https://cdn.tailwindcss.com"></script>` cùng file CSS cục bộ `~/css/site.css`, FontAwesome 6.4, Google Fonts (Be Vietnam Pro, Dancing Script). Hỗ trợ Dark Mode qua cơ chế dây kéo tương tác (`#pullcord-wrapper`).

- **Admin Layout** (`/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`):
  - Sidebar dạng Boys-Tech thu nhỏ 85px, hover nở rộng 280px (dòng 23-45).
  - Danh sách Menu quản trị chia làm 3 nhóm: "Nội dung" (Trang chủ, Chương trình, Đóng góp, Ký ức, Ban Tổ chức, Tin tức), "Tương tác" (Đăng ký TNV), "Hệ thống" (Xem trang ngoài).
  - Style đồng bộ nền xanh xám/tối `#161a2d`, màu nhấn xanh lá cây `#4caf50`.
  - Sử dụng Tailwind CSS CDN và FontAwesome 6.0.

### 1.5 Cấu hình Middleware & Routing trong `Program.cs`
- `Program.cs` hiện tại (38 dòng):
  ```csharp
  var builder = WebApplication.CreateBuilder(args);
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  builder.Services.AddControllersWithViews();
  builder.Services.AddControllersWithViews(); // Bị lặp dòng

  var app = builder.Build();
  ...
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
- **Hiện trạng quan sát**:
  1. Chưa có route cho Area: `app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");`.
  2. Chưa đăng ký Session (`builder.Services.AddSession()`, `app.UseSession()`) để lưu giỏ hàng của người dùng.
  3. Dự án hiện chưa có thư mục `Areas/`.

---

## 2. Chuỗi lập luận & Đề xuất kiến trúc (Logic Chain & Architecture Proposal)

Từ các quan sát thực tế trên, quá trình phân tích dẫn đến giải pháp kỹ thuật chi tiết như sau:

```
[Khảo sát hiện trạng]
 ├── .NET 10 Web App + EF Core SqlServer
 ├── Chưa có Area, Chưa có Session, Chưa có Entity Sản phẩm/Đơn hàng
 ├── _Layout.cshtml có Desktop Navbar + Mobile Liquid Bottom Nav
 └── _AdminLayout.cshtml có Sidebar mở rộng với các module quản trị
         │
         ▼
[Yêu cầu từ ORIGINAL_REQUEST.md]
 ├── R1: Điều hướng Desktop & Mobile, Storefront danh sách, chi tiết, giỏ hàng, đặt mua
 ├── R2: Admin CRUD Sản phẩm (Thêm, Sửa, Xóa, hiển thị bảng, upload ảnh)
 └── R3: Lưu trữ dữ liệu DB, cấu trúc trong thư mục riêng biệt theo ASP.NET Core Area, Seed Data >= 3 sản phẩm
         │
         ▼
[Thiết kế giải pháp chi tiết]
 ├── 1. Routing & Session: Bổ sung MapControllerRoute cho Area và AddSession vào Program.cs
 ├── 2. Cấu trúc Area: Tạo thư mục Areas/Admin (hoặc Areas/CuaHang) cho tính năng gây quỹ
 ├── 3. Data Models & DbContext: Tạo SanPham, DonHang, ChiTietDonHang + DbSets + Seed Data tự phục hồi
 ├── 4. Storefront UI: Tích hợp menu trên _Layout.cshtml (ngang hàng Trang chủ), View danh sách, chi tiết, giỏ hàng, xác nhận
 └── 5. Admin UI: Thêm mục menu trên _AdminLayout.cshtml, giao diện quản lý CRUD sản phẩm đồng bộ phong cách
```

### 2.1 Cấu trúc Thư mục Area (ASP.NET Core Area)
Để đáp ứng triệt để R3 và Tiêu chí chấp nhận ("Toàn bộ Controllers, Views, Models/ViewModels liên quan đến tính năng mới được đặt trong thư mục riêng biệt theo cấu trúc ASP.NET Core Area"):

```
WEB_TinhNguyen/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   └── SanPhamGayQuyController.cs       (CRUD Sản phẩm Admin: Index, Create, Edit, Delete)
│   │   │   └── DonHangGayQuyController.cs       (Xem danh sách đơn hàng ủng hộ)
│   │   ├── Models/
│   │   │   └── SanPhamVM.cs                    (ViewModels cho form Thêm/Sửa/Danh sách)
│   │   └── Views/
│   │       ├── _ViewImports.cshtml
│   │       ├── _ViewStart.cshtml               (Chỉ định Layout = "~/Views/Shared/_AdminLayout.cshtml")
│   │       ├── SanPhamGayQuy/
│   │       │   ├── Index.cshtml                (Bảng danh sách sản phẩm)
│   │       │   ├── Create.cshtml               (Form thêm mới)
│   │       │   └── Edit.cshtml                 (Form chỉnh sửa)
│   │       └── DonHangGayQuy/
│   │           └── Index.cshtml                (Danh sách đơn đặt hàng)
│   │
│   └── CuaHang/ (hoặc tích hợp Storefront trong Controllers/CuaHangController)
│       ├── Controllers/
│       │   ├── CuaHangController.cs            (Danh sách sản phẩm, chi tiết sản phẩm)
│       │   └── GioHangController.cs            (Thêm vào giỏ, xem giỏ, thanh toán/đặt mua)
│       ├── Models/
│       │   ├── CartItem.cs
│       │   ├── CartViewModel.cs
│       │   └── CheckoutViewModel.cs
│       └── Views/
│           ├── _ViewImports.cshtml
│           ├── _ViewStart.cshtml               (Chỉ định Layout = "~/Views/Shared/_Layout.cshtml")
│           ├── CuaHang/
│           │   ├── Index.cshtml                (Giao diện cửa hàng gây quỹ)
│           │   └── ChiTiet.cshtml              (Chi tiết sản phẩm)
│           └── GioHang/
│               ├── Index.cshtml                (Giỏ hàng & biểu mẫu thông tin đặt mua)
│               └── DatHangThanhCong.cshtml     (Trang cảm ơn & xác nhận đơn)
```

> **Lưu ý định tuyến trong Program.cs**: Cần thêm route Area trước default route:
> ```csharp
> app.MapControllerRoute(
>     name: "areas",
>     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
> ```

### 2.2 Thiết kế Thực thể Dữ liệu (Entities & DbContext)
Trong `ApplicationDbContext.cs` hoặc file model riêng biệt:

1. **`SanPhamGayQuy`**:
   - `int IDSanPham` (Khóa chính, Identity)
   - `string TenSanPham` (Tên sản phẩm, ví dụ: "Áo thun Tình nguyện Trăng Sáng", "Móc khóa Gấu bông Langbiang")
   - `decimal GiaBan` (Giá bán / Mức ủng hộ, ví dụ: 120.000 VNĐ)
   - `int SoLuongTon` (Số lượng có sẵn trong kho)
   - `string? MoTaNgan` (Mô tả tóm tắt)
   - `string? MoTaChiTiet` (Mô tả chi tiết sản phẩm và ý nghĩa gây quỹ)
   - `string? LinkHinhAnh` (Đường dẫn ảnh trong `/uploads/sanpham/` hoặc URL ngoài)
   - `string? ThongTinGayQuy` (Ví dụ: "100% lợi nhuận dùng mua bánh Trung thu & lồng đèn cho trẻ em Langbiang")
   - `bool TrangThai` (Còn hàng / Mở bán)
   - `bool DaXoa` (Hỗ trợ xóa mềm hoặc xóa trực tiếp)
   - `DateTime NgayTao` (Ngày tạo)

2. **`DonHangGayQuy`**:
   - `int IDDonHang` (Khóa chính)
   - `string MaDonHang` (Mã đơn, ví dụ: "LB-2026-XXXX")
   - `string HoTenNguoiMua` (Họ và tên người ủng hộ)
   - `string SoDienThoai` (Số điện thoại liên hệ)
   - `string? Email`
   - `string DiaChiNhanHang` (Địa chỉ giao hàng)
   - `string? LoiNhanUngHo` (Lời nhắn gửi tới các em nhỏ vùng cao)
   - `string PhuongThucThanhToan` ("Chuyển khoản QR", "Tiền mặt khi nhận hàng - COD")
   - `decimal TongTien` (Tổng giá trị đơn hàng)
   - `string TrangThaiDonHang` ("Chờ xác nhận", "Đã thanh toán", "Đang giao", "Hoàn thành")
   - `DateTime NgayDatHang`
   - Navigation property: `ICollection<ChiTietDonHangGayQuy> ChiTietDonHangs`

3. **`ChiTietDonHangGayQuy`**:
   - `int IDChiTiet` (Khóa chính)
   - `int IDDonHang` (Khóa ngoại)
   - `int IDSanPham` (Khóa ngoại)
   - `string TenSanPham` (Snapshot tên sản phẩm tại thời điểm mua)
   - `decimal DonGia`
   - `int SoLuong`
   - `decimal ThanhTien`

### 2.3 Cơ chế Dữ liệu mẫu ban đầu (Seed Data Pattern)
Dự án có sẵn cơ chế tự động khởi tạo dữ liệu (tương tự như `AdminTinTucController` hay `AdminBanToChucController` khi dữ liệu rỗng). Có thể tạo lớp trợ giúp `DbInitializer.Seed(ApplicationDbContext context)` hoặc kích hoạt khi controller truy cập lần đầu:
- **Sản phẩm mẫu 1**: Áo Thun Tình Nguyện "Trăng Sáng Langbiang 2026" - 150.000 VNĐ - Tồn kho: 100 - Gây quỹ: Góp 1 phần quà trung thu trị giá 50.000đ.
- **Sản phẩm mẫu 2**: Móc Khóa Gỗ Linh Vật Langbiang Khắc Tên - 35.000 VNĐ - Tồn kho: 200 - Gây quỹ: Góp 1 lồng đèn truyền thống cho em nhỏ.
- **Sản phẩm mẫu 3**: Túi Tote Vải Canvas "Nụ Cười Vùng Cao" - 85.000 VNĐ - Tồn kho: 80 - Gây quỹ: Góp 2 bữa ăn dinh dưỡng "Nấu ăn cho em".

### 2.4 Tích hợp Điều hướng trên `Views/Shared/_Layout.cshtml`
- **Thanh Menu Desktop** (ngang hàng Trang chủ):
  ```html
  <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright"
     asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index">
     <i class="fas fa-store mr-1 text-emerald-600"></i> Cửa hàng gây quỹ
  </a>
  ```
- **Thanh Liquid Bottom Navigation trên Mobile**:
  Bổ sung một mục item `data-url="/cuahang"` với icon `<i class="fas fa-shopping-bag"></i>` và text `Cửa hàng`.
- **Icon Giỏ hàng góc trên Header**:
  Hiển thị badge số lượng sản phẩm trong giỏ hàng để người dùng truy cập nhanh.

### 2.5 Tích hợp Menu Sidebar Admin trên `Views/Shared/_AdminLayout.cshtml`
- Trong danh mục `<h4><span>Nội dung</span>...</h4>`:
  ```html
  <li>
      <a href="@Url.Action("Index", "SanPhamGayQuy", new { area = "Admin" })" 
         class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "SanPhamGayQuy" ? "active" : "")">
          <i class="fas fa-box-open"></i> <span class="link-text">Sản phẩm gây quỹ</span>
      </a>
  </li>
  ```
- Trong danh mục `<h4><span>Tương tác</span>...</h4>`:
  ```html
  <li>
      <a href="@Url.Action("Index", "DonHangGayQuy", new { area = "Admin" })" 
         class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "DonHangGayQuy" ? "active" : "")">
          <i class="fas fa-shopping-cart"></i> <span class="link-text">Đơn hàng gây quỹ</span>
      </a>
  </li>
  ```

---

## 3. Các lưu ý & Giả định (Caveats & Assumptions)

1. **Môi trường Database SQL Server**:
   - Máy chủ SQL Server không mở cổng 1433 trực tiếp trong môi trường sandbox hiện tại. 
   - Đảm bảo các mô hình Entity sử dụng chuẩn EF Core DataAnnotations/Fluent API tương thích hoàn toàn với SQL Server. Khi chạy kiểm thử hoặc chạy demo offline, có thể linh hoạt cấu hình kết nối hoặc có cơ chế fallback thích hợp.
2. **Biến môi trường Build**:
   - Luôn sử dụng tiền tố `XDG_RUNTIME_DIR=/tmp` khi chạy `dotnet build` hoặc `dotnet run` trên máy trạm Ubuntu này để tránh lỗi NuGet Mutex.
3. **Session State**:
   - `Program.cs` cần bổ sung `builder.Services.AddDistributedMemoryCache();` và `builder.Services.AddSession(...)` cùng `app.UseSession()` để hỗ trợ lưu trữ giỏ hàng nếu sử dụng Session. Có thể hỗ trợ đồng thời Cookie/LocalStorage dự phòng.
4. **Quyền ghi tệp tải lên (Uploads)**:
   - Thư mục `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/wwwroot/uploads/sanpham` cần được tạo nếu chưa tồn tại khi tải ảnh lên.

---

## 4. Kết luận (Conclusion)

- Kiến trúc hiện tại của dự án là nền tảng sạch, hiện đại (.NET 10.0 MVC), sẵn sàng để mở rộng thêm Area mới mà không gây ảnh hưởng hay xung đột với các module hiện có (`TrangChu`, `GayQuy`, `BanToChuc`, `TinTuc`, `KyUc`).
- Cấu trúc đề xuất đáp ứng 100% tất cả các tiêu chí trong `ORIGINAL_REQUEST.md`:
  - **R1**: Điều hướng ngang hàng Trang chủ trên Header/Navbar, trải nghiệm Responsive cho cả Mobile/Desktop, đầy đủ chức năng xem sản phẩm, xem chi tiết, giỏ hàng và đặt mua/ủng hộ.
  - **R2**: Quản trị Admin đầy đủ CRUD (danh sách bảng, thêm, sửa, xóa sản phẩm, upload ảnh), chuẩn phong cách dark-sidebar & Be Vietnam Pro.
  - **R3**: Cấu trúc module nằm trong ASP.NET Core Area riêng biệt, lưu trữ dữ liệu với Entity Framework Core, biên dịch `dotnet build` sạch sẽ 0 lỗi, kèm sẵn Seed Data ít nhất 3 sản phẩm gây quỹ mang ý nghĩa thiện nguyện sâu sắc.

---

## 5. Phương pháp Xác minh Độc lập (Independent Verification Method)

1. **Kiểm tra biên dịch dự án**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Kỳ vọng*: Mã thoát bằng 0, không có lỗi biên dịch (`0 Error(s)`).

2. **Kiểm tra các thành phần cấu trúc mã nguồn**:
   - Xem `WEB_TinhNguyen/Program.cs`: Kiểm tra route areas và session service.
   - Xem `WEB_TinhNguyen/Data/ApplicationDbContext.cs`: Kiểm tra các DbSet.
   - Xem `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`: Xác nhận liên kết dẫn tới cửa hàng gây quỹ trên navbar.
   - Xem `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`: Xác nhận mục quản lý sản phẩm trong sidebar.
