# BÁO CÁO KHẢO SÁT KIẾN TRÚC AREA, ADMIN VÀ ĐIỀU HƯỚNG (SURVEY REPORT)

**Tác vụ**: Khảo sát ASP.NET Core Areas, cấu trúc Admin controllers/views/layouts/styles, định tuyến trong Program.cs và yêu cầu xác thực/phân quyền nhằm phục vụ xây dựng phân hệ Sản phẩm Gây quỹ (Cửa hàng & Quản trị Admin).  
**Người thực hiện**: `teamwork_explorer_survey_2`  
**Ngày thực hiện**: 2026-11-03  
**Dự án**: WEB_TinhNguyen (.NET 10.0, ASP.NET Core MVC)

---

## 1. Observation (Các quan sát thực tế)

### 1.1. Hiện trạng ASP.NET Core Areas trong dự án
- **Thư mục Areas**: Hiện tại **hoàn toàn chưa có thư mục `Areas`** nào trong dự án (`find_by_name` cho kết quả 0 matches).
- **Vị trí hiện tại của các Controller Admin**:
  Toàn bộ các Controller phục vụ quản trị Admin đang được đặt trực tiếp trong thư mục gốc `WEB_TinhNguyen/Controllers/`:
  - `AdminController.cs` (Dòng 1-13): Controller chính cho Dashboard `/Admin`.
  - `AdminTrangChuController.cs`: Cấu hình Banner, đếm ngược, màu sắc giao diện.
  - `AdminChuongTrinhController.cs`: Cấu hình các chặng hành trình và lộ trình chi tiết.
  - `AdminGayQuyController.cs`: Cấu hình kênh quyên góp trực tiếp, QR tài khoản và nhà tài trợ.
  - `AdminKyUcController.cs`: Cấu hình thư viện hình ảnh và các mùa dự án.
  - `AdminBanToChucController.cs`: Cấu hình sơ đồ tổ chức và danh sách thành viên BTC.
  - `AdminTinTucController.cs`: Quản lý các bài viết tin tức.
  - `AdminHieuUngController.cs`: Quản lý tải lên và kích hoạt file JavaScript hiệu ứng động.
- **Vị trí hiện tại của Views Admin**:
  Nằm trong thư mục `WEB_TinhNguyen/Views/`:
  - `Views/Admin/Index.cshtml`
  - `Views/AdminTrangChu/`
  - `Views/AdminChuongTrinh/`
  - `Views/AdminGayQuy/`
  - `Views/AdminKyUc/`
  - `Views/AdminBanToChuc/`
  - `Views/AdminTinTuc/`
  - `Views/AdminHieuUng/`

### 1.2. Cấu hình Định tuyến (Routing) trong `Program.cs`
- Quan sát tại `WEB_TinhNguyen/Program.cs` (Dòng 24-37):
  ```csharp
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
- **Không có route cho Area**: Hiện tại chưa có cấu hình `app.MapControllerRoute(name: "areas", ...)` hay `app.MapAreaControllerRoute(...)`.
- **Target framework**: `net10.0` (xem `WEB_TinhNguyen.csproj`), sử dụng tính năng mới của ASP.NET Core (.NET 9/10) là `.MapStaticAssets()` và `.WithStaticAssets()`.

### 1.3. Cấu trúc Giao diện Admin (`_AdminLayout.cshtml`) và Views Admin
- **File Layout dùng chung**: `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` (342 dòng).
- **Công nghệ Frontend được nạp**:
  - Tailwind CSS via CDN: `<script src="https://cdn.tailwindcss.com"></script>` (Dòng 9).
  - FontAwesome 6: `<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">` (Dòng 11).
  - Google Font: `Be Vietnam Pro` (Dòng 13).
- **Cấu trúc Sidebar ("Boys - Tech" expandable sidebar)**:
  - Sidebar cố định bên trái, thu gọn mặc định `width: 85px` (chỉ hiện icon), khi hover mở rộng ra `width: 280px` (`.sidebar-container:hover`).
  - Nền Sidebar: Màu xanh đen navy `#161a2d`, bo góc bên phải `border-bottom-right-radius: 25px; border-top-right-radius: 25px`.
  - Icon khi active/hover chuyển sang màu xanh lá `#4caf50`, thẻ `<a>` có nền `rgba(255, 255, 255, 0.06)`.
  - Danh mục trong Sidebar hiện tại:
    - *Bảng điều khiển*: `@Url.Action("Index", "Admin")`
    - *Nội dung*: Cấu hình Trang chủ (`AdminTrangChu`), Chương trình (`AdminChuongTrinh`), Đóng Góp (`AdminGayQuy`), Ký ức (`AdminKyUc`), Ban Tổ Chức (`AdminBanToChuc`), Tin tức (`AdminTinTuc`).
    - *Tương tác*: Đăng ký TNV (`#`).
    - *Hệ thống*: Xem trang ngoài (`@Url.Action("Index", "Home")`).
- **Cấu trúc Topbar**:
  - Hiển thị tiêu đề trang `@ViewData["Title"]` (Dòng 313).
  - Block thông tin người dùng: Icon chữ "A", "Admin Hệ thống" / "Quản trị viên" (Dòng 318-326).
  - Nút Đăng xuất dạng icon `<i class="fas fa-sign-out-alt"></i>` (Dòng 328-330).
- **Vùng nội dung con**: `<main class="flex-1 overflow-y-auto p-4 md:p-8 pt-4">@RenderBody()</main>` (Dòng 335-337).
- **Khai báo Layout trong từng View Admin**:
  Mỗi View Admin hiện đều có khai báo tường minh:
  ```csharp
  @{
      ViewData["Title"] = "...";
      Layout = "~/Views/Shared/_AdminLayout.cshtml";
  }
  ```

### 1.4. Quy ước Thao tác CRUD trong Phân hệ Admin
Quan sát qua các Controller mẫu (`AdminHieuUngController.cs`, `AdminTinTucController.cs`, `AdminChuongTrinhController.cs`):
- **Kế thừa**: `public class Admin...Controller : Controller`.
- **Dependency Injection**: Inject `ApplicationDbContext _context` và `IWebHostEnvironment _env`.
- **Upload file / Hình ảnh**:
  - Lưu vào thư mục `_env.WebRootPath` (`wwwroot/gallery`, `wwwroot/uploads`, `wwwroot/effects`).
  - Định danh file duy nhất: `Guid.NewGuid().ToString() + "_" + file.FileName`.
- **Thông báo trạng thái**: Sử dụng `TempData["Success"] = "..."` hoặc `TempData["Error"] = "..."`.
  - Phía View hiển thị banner màu xanh ngọc:
    `<div class="p-4 text-emerald-800 border border-emerald-200 rounded-xl bg-emerald-50">...</div>`.
- **Bảo mật Form**: Áp dụng thuộc tính `[HttpPost]` và `[ValidateAntiForgeryToken]`.
- **Cơ chế Xóa (Delete)**:
  - POST request qua form nhỏ với `@item.ID...`.
  - Có hộp thoại xác nhận JavaScript: `onsubmit="return confirm('Xác nhận xóa?...');"`.
  - Đồng thời xóa file vật lý tương ứng trong `wwwroot` nếu có.
- **Khởi tạo dữ liệu mẫu tự động (Auto-seed)**:
  Các controller (`AdminTinTucController`, `AdminGayQuyController`, `AdminBanToChucController`) đều kiểm tra nếu bảng dữ liệu rỗng trong DB thì tự động tạo seed data mẫu để hệ thống không bị lỗi rỗng giao diện.

### 1.5. Hiện trạng Xác thực & Phân quyền (Authentication & Authorization)
- `grep_search` kiểm tra `[Authorize]`, `AddAuthentication`, `UseAuthentication`, `AddIdentity`: **0 kết quả trong toàn bộ source code**.
- `Entities.cs` & `ApplicationDbContext.cs`: `ApplicationDbContext` kế thừa `DbContext` thuần, không dùng ASP.NET Core Identity.
- Ghi chú trong `SQLQuery1.sql` (Dòng 68): *"(Dung INT vi khong co User Login)"*.
- Khẳng định: **Dự án chạy ở chế độ Demo công khai (Open Demo Mode)**, chưa áp dụng cơ chế đăng nhập / phân quyền bằng mật khẩu hay Claims. Bất kỳ ai vào đường dẫn `/Admin` đều có thể thao tác quản trị.

### 1.6. Thanh Điều Hướng (Navigation) phía Người dùng (`_Layout.cshtml`)
Quan sát tại `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`:
- **Desktop Header Nav** (Dòng 372-395):
  ```html
  <nav class="hidden items-center gap-1 lg:flex pr-24">
      <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="Index">Trang chủ</a>
      <a ... asp-controller="Home" asp-action="ChuongTrinh">Chương trình</a>
      <a ... asp-controller="Home" asp-action="GayQuy">Đóng góp</a>
      <a ... asp-controller="Home" asp-action="BanToChuc">Ban tổ chức</a>
      <a ... asp-controller="Home" asp-action="TinTuc">Tin tức</a>
      <div class="relative group">Ký ức ...</div>
  </nav>
  ```
- **Mobile Liquid Bottom Navigation** (Dòng 475-516):
  Thanh điều hướng nổi dưới chân trang trên mobile:
  ```html
  <div class="navigation lg:hidden">
      <ul>
          <li class="list" data-url="/home/index"><a href="/"><span class="icon"><i class="fas fa-home"></i></span><span class="text">Trang chủ</span></a></li>
          <li class="list" data-url="/home/chuongtrinh"><a href="/Home/ChuongTrinh"><span class="icon"><i class="fas fa-calendar-alt"></i></span><span class="text">Sự kiện</span></a></li>
          ...
      </ul>
  </div>
  ```
- **Footer Links** (Dòng 453-458): Danh mục liên kết khám phá.

### 1.7. Build và Tailwind CSS
- Lệnh biên dịch thành công: `XDG_RUNTIME_DIR=/tmp dotnet build`.
- File `.csproj` có Target tự động chạy `npm run build:css` trước khi build:
  `<Exec Command="npm run build:css" />`.
- `tailwind.config.js` có `content: ["./Views/**/*.cshtml", "./wwwroot/js/**/*.js"]`. Cần bổ sung path cho Area (e.g. `"./Areas/**/*.cshtml"`).

---

## 2. Logic Chain (Chuỗi lập luận và Phân tích)

1. **Từ Quan sát 1.1 & 1.2**: Dự án hiện không có Area nào và Program.cs chỉ có default route. Yêu cầu R3 và Acceptance Criterion 37 nêu rõ: *"Toàn bộ Controllers, Views, Models/ViewModels liên quan đến tính năng mới được đặt trong thư mục riêng biệt theo kiến trúc Area của ASP.NET Core để dễ quản lý."*
   -> **Suy ra**: Cần tạo một thư mục Area mới (ví dụ: `Areas/CuaHang` hoặc `Areas/Shop`). Để ASP.NET Core nhận diện Controller trong Area này, bắt buộc phải đăng ký route cho Area trong `Program.cs` trước route mặc định:
   ```csharp
   app.MapControllerRoute(
       name: "areas",
       pattern: "{area:exists}/{controller=SanPham}/{action=Index}/{id?}")
       .WithStaticAssets();
   ```

2. **Từ Quan sát 1.1 & Yêu cầu R2**: Phân hệ quản trị Admin hiện tại có các controller đặt ở root (`AdminController`, `AdminTinTucController`...). Tuy nhiên, với tính năng mới "Sản phẩm gây quỹ", yêu cầu bài toán đặt ra là:
   - R2: Xây dựng trang quản trị sản phẩm trong Admin cho phép xem danh sách, thêm mới, sửa, xóa sản phẩm, đồng bộ phong cách Admin.
   - R3: Toàn bộ Controllers, Views, Models/ViewModels liên quan đến tính năng mới được tổ chức trong thư mục riêng biệt theo kiến trúc Area.
   -> **Suy ra**: Giải pháp tối ưu và sạch sẽ nhất là đặt cả Controller Storefront (`SanPhamController`, `GioHangController`) lẫn Controller Quản trị Admin (`AdminSanPhamController`) vào trong cùng Area `CuaHang` (hoặc `Shop`), ví dụ:
   - Storefront Controller: `Areas/CuaHang/Controllers/SanPhamController.cs` và `GioHangController.cs` (gắn thuộc tính `[Area("CuaHang")]`).
   - Admin Controller: `Areas/CuaHang/Controllers/AdminSanPhamController.cs` (gắn thuộc tính `[Area("CuaHang")]`).
   - Giao diện Admin: `Areas/CuaHang/Views/AdminSanPham/` kế thừa trực tiếp layout có sẵn `~/Views/Shared/_AdminLayout.cshtml`.
   - Giao diện Cửa hàng: `Areas/CuaHang/Views/SanPham/` và `Views/GioHang/` kế thừa `_Layout.cshtml` để đồng nhất với toàn bộ giao diện khách ghé thăm website.

3. **Từ Quan sát 1.3 & 1.4**: Các trang Admin hiện tại sử dụng bảng danh sách chuẩn Tailwind, icon FontAwesome, thẻ card màu trắng có viền xám nhẹ `border-gray-200`, bo góc `rounded-2xl`, nút hành động xanh lá (`#1b5e20`, `#2e7d32`) và nút xóa đỏ với confirm dialog.
   -> **Suy ra**: Trang quản lý Admin sản phẩm gây quỹ (`AdminSanPham`) cần tuân thủ 100% phong cách này:
   - Danh sách sản phẩm: Table responsive với cột Ảnh, Tên sản phẩm, Giá, Số lượng còn, Mô tả tóm tắt, Trạng thái và Cột Thao tác (Sửa, Xóa).
   - Form Thêm mới / Chỉnh sửa: Đầy đủ các trường tên, giá, số lượng, mô tả, mục đích gây quỹ, upload hình ảnh lên `wwwroot/uploads/products/`.
   - Nút Thêm mới dẫn đến `Create.cshtml` hoặc modal popup.
   - Sau khi Thêm/Sửa/Xóa thành công, chuyển hướng về `Index` kèm thông báo `TempData["Success"]`.

4. **Từ Quan sát 1.5**: Hệ thống không áp dụng xác thực `[Authorize]` hay Identity.
   -> **Suy ra**: Không áp đặt thuộc tính `[Authorize]` lên `AdminSanPhamController` để tránh xung đột với kiến trúc demo mở của toàn bộ các Controller Admin hiện tại trong dự án.

5. **Từ Quan sát 1.6 & Yêu cầu R1**: Acceptance Criterion yêu cầu: *"Mục dẫn tới trang mua bán sản phẩm gây quỹ hiển thị trên thanh điều hướng/slide bar trên cùng, ngang hàng với Trang chủ trong `_Layout.cshtml`."*
   -> **Suy ra**: Cần bổ sung mục điều hướng "Cửa hàng gây quỹ" (hoặc "Sản phẩm gây quỹ") vào:
   - Header Desktop Nav trong `_Layout.cshtml`: ngang hàng với Trang chủ, Chương trình, Đóng góp.
   - Thanh Mobile Liquid Bottom Nav: Thêm tab hoặc liên kết đến `/CuaHang/SanPham`.
   - Sidebar Admin trong `_AdminLayout.cshtml`: Thêm mục "Sản phẩm gây quỹ" dẫn đến `@Url.Action("Index", "AdminSanPham", new { area = "CuaHang" })` để quản trị viên truy cập tiện lợi từ mọi trang admin.

6. **Từ Quan sát 1.4 & Dữ liệu giỏ hàng (Cart)**: Cần giao diện giỏ hàng và biểu mẫu đặt mua. Trong ASP.NET Core, Session mặc định chưa được kích hoạt trong `Program.cs`.
   -> **Suy ra**: Có 2 cách tiếp cận linh hoạt:
   - Cách 1 (Khuyến nghị cao): Quản lý giỏ hàng bằng LocalStorage/SessionStorage phía client kết hợp giao diện động. Khi người dùng bấm "Đặt hàng", gửi dữ liệu giỏ hàng kèm thông tin khách hàng (Họ tên, SĐT, Email, Địa chỉ, Lời nhắn) lên Action POST của Controller. Cách này hoạt động độc lập, không phụ thuộc bộ nhớ server, không mất giỏ khi refresh.
   - Cách 2: Bổ sung `builder.Services.AddDistributedMemoryCache(); builder.Services.AddSession();` và `app.UseSession();` trong `Program.cs`.

7. **Từ Quan sát 1.4 & Acceptance Criterion 39**: Cần có ít nhất 3 sản phẩm gây quỹ mẫu ban đầu.
   -> **Suy ra**: Áp dụng cơ chế auto-seed trực tiếp trong Controller hoặc DbInitializer: khi bảng sản phẩm rỗng, tự động chèn 3 sản phẩm mẫu mang đậm dấu ấn gây quỹ của dự án Trăng Sáng Langbiang (ví dụ: *Áo thun tình nguyện Trăng Sáng Langbiang*, *Túi Canvas Gieo Mầm Yêu Thương*, *Set Móc Khóa & Huy Hiệu Kỷ Niệm*).

---

## 3. Caveats (Các điểm cần lưu ý & Giới hạn khảo sát)

1. **Cơ sở dữ liệu SQL Server Local**: Máy trạm hiện tại chưa bật dịch vụ SQL Server trên port 1433 (`Connection refused`). Do đó, mã nguồn Model và Entity Framework Core cần được viết chuẩn cú pháp EF Core 10 (`DbSet`, thuộc tính DataAnnotations, quan hệ ForeignKey) và có cơ chế auto-seed / xử lý ngoại lệ an toàn nếu DB chưa sẵn sàng.
2. **Biên dịch trong môi trường Sandbox**: Khi chạy lệnh `dotnet build`, hệ thống sandbox Linux báo lỗi với `/run/user/1000`. Cần luôn đính kèm biến môi trường: `XDG_RUNTIME_DIR=/tmp dotnet build`.
3. **Tailwind CSS JIT vs CDN**: Cả `_Layout.cshtml` và `_AdminLayout.cshtml` đều nhúng `<script src="https://cdn.tailwindcss.com"></script>`. Điều này giúp mọi class Tailwind mới viết trong Area sẽ hiển thị chính xác ngay lập tức trên trình duyệt mà không phụ thuộc tuyệt đối vào build tĩnh của `site.css`. Tuy nhiên, để đảm bảo tiêu chuẩn build sạch, nên thêm `./Areas/**/*.cshtml` vào `tailwind.config.js`.

---

## 4. Conclusion (Đề xuất Thiết kế & Kiến trúc Chi tiết)

### 4.1. Kiến trúc Thư mục Đề xuất cho Area `CuaHang`
```
WEB_TinhNguyen/
├── Areas/
│   └── CuaHang/
│       ├── Controllers/
│       │   ├── SanPhamController.cs        # [Area("CuaHang")] Storefront: Danh sách sản phẩm, Chi tiết sản phẩm
│       │   ├── GioHangController.cs        # [Area("CuaHang")] Storefront: Giỏ hàng, Form đặt mua, Xác nhận thành công
│       │   └── AdminSanPhamController.cs   # [Area("CuaHang")] Admin CRUD: Index, Create, Edit, Delete
│       ├── Models/
│       │   ├── SanPhamGayQuy.cs            # Entity: ID, Ten, Gia, SoLuong, MoTa, LinkAnh, MucDichGayQuy...
│       │   ├── DonHangGayQuy.cs            # Entity: ID, MaDon, HoTen, SDT, Email, DiaChi, TongTien, LoiNhan...
│       │   ├── ChiTietDonHang.cs           # Entity: ID, IDDonHang, IDSanPham, SoLuong, DonGia, ThanhTien
│       │   └── CuaHangViewModels.cs        # ViewModels cho Giỏ hàng, Đặt hàng và Form CRUD Admin
│       └── Views/
│           ├── _ViewImports.cshtml         # Nạp TagHelpers, Models của Area
│           ├── _ViewStart.cshtml           # Layout mặc định = "_Layout"
│           ├── SanPham/
│           │   ├── Index.cshtml            # Danh sách sản phẩm gây quỹ (Responsive, filter, card sản phẩm)
│           │   └── ChiTiet.cshtml          # Chi tiết sản phẩm, nút "Thêm vào giỏ hàng"
│           ├── GioHang/
│           │   ├── Index.cshtml            # Xem giỏ hàng, tăng/giảm số lượng, xóa item
│           │   ├── DatHang.cshtml          # Form điền thông tin đặt mua & ủng hộ gây quỹ
│           │   └── ThanhCong.cshtml        # Thông báo đặt hàng thành công + Lời tri ân
│           └── AdminSanPham/
│               ├── Index.cshtml            # Bảng danh sách quản trị sản phẩm (Layout = _AdminLayout)
│               ├── Create.cshtml           # Form thêm sản phẩm mới kèm upload ảnh
│               └── Edit.cshtml             # Form chỉnh sửa sản phẩm kèm cập nhật ảnh
```

### 4.2. Cấu hình Cần Bổ Sung trong `Program.cs`
Bổ sung cấu hình Area route trước default route (tại vị trí dòng 30):
```csharp
// Đăng ký route cho ASP.NET Core Areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=SanPham}/{action=Index}/{id?}")
    .WithStaticAssets();

// Route mặc định trang chủ
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```

### 4.3. Bổ sung Entities vào `ApplicationDbContext.cs`
Thêm các `DbSet` vào `WEB_TinhNguyen/Data/ApplicationDbContext.cs`:
```csharp
public DbSet<SanPhamGayQuy> SanPhamGayQuys { get; set; }
public DbSet<DonHangGayQuy> DonHangGayQuys { get; set; }
public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
```

### 4.4. Bổ sung Điều Hướng (Navigation Links)
1. **Trong `WEB_TinhNguyen/Views/Shared/_Layout.cshtml` (Dòng ~375)**:
   Thêm liên kết ngang hàng với "Trang chủ":
   ```html
   <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright"
      asp-area="CuaHang" asp-controller="SanPham" asp-action="Index">
       Cửa hàng gây quỹ
   </a>
   ```
2. **Trong `WEB_TinhNguyen/Views/Shared/_Layout.cshtml` (Thanh Bottom Nav Mobile, Dòng ~483)**:
   ```html
   <li class="list" data-url="/cuahang">
       <a href="/CuaHang/SanPham">
           <span class="icon"><i class="fas fa-shopping-bag"></i></span>
           <span class="text">Cửa hàng</span>
       </a>
   </li>
   ```
3. **Trong `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` (Sidebar, Dòng ~265)**:
   Thêm mục menu quản trị sản phẩm:
   ```html
   <li>
       <a href="/CuaHang/AdminSanPham" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")">
           <i class="fas fa-boxes"></i> <span class="link-text">Sản phẩm gây quỹ</span>
       </a>
   </li>
   ```
4. **Trong `WEB_TinhNguyen/Views/Admin/Index.cshtml` (Quick Links Dashboard, Dòng ~106)**:
   Bổ sung thêm thẻ shortcut dẫn nhanh tới Quản lý sản phẩm gây quỹ.

### 4.5. Dữ liệu Mẫu Ban Đầu (Seed Data - ít nhất 3 sản phẩm)
1. **Áo thun Tình nguyện "Trăng Sáng Langbiang 2026"**: Giá 150.000 VNĐ, Số lượng: 100, Mục đích: Gây quỹ 350 suất quà Trung thu cho trẻ em đồng bào Langbiang.
2. **Túi Canvas "Gieo Mầm Yêu Thương"**: Giá 85.000 VNĐ, Số lượng: 150, Mục đích: Hỗ trợ chi phí tổ chức gian hàng trò chơi dân gian và đêm hội Trăng rằm.
3. **Set Huy Hiệu & Móc Khóa Gỗ Kỷ Niệm**: Giá 45.000 VNĐ, Số lượng: 200, Mục đích: Đóng góp quỹ "Nấu ăn cho em - 250 suất phở nóng".

---

## 5. Verification Method (Phương pháp Kiểm chứng Độc lập)

Để kiểm chứng tính chính xác của báo cáo này và kết quả sau khi triển khai:
1. **Kiểm tra biên dịch dự án**:
   ```bash
   cd /home/hutech/Vinh_Offline/WEB_TinhNguyen
   XDG_RUNTIME_DIR=/tmp dotnet build
   ```
   *Kỳ vọng*: `Build succeeded. 0 Error(s)`.
2. **Kiểm tra Area Routing & Controllers**:
   Kiểm tra sự tồn tại của thuộc tính `[Area("CuaHang")]` trên các controller trong thư mục `Areas/CuaHang/Controllers/`.
3. **Kiểm tra File Layout & Navigation**:
   Xem file `_Layout.cshtml` và `_AdminLayout.cshtml` để đảm bảo đã có link dẫn đến `CuaHang/SanPham` và `CuaHang/AdminSanPham`.
4. **Kiểm tra Seed Data & CRUD**:
   Kiểm tra logic trong `SanPhamController` hoặc `AdminSanPhamController` tự động khởi tạo tối thiểu 3 sản phẩm mẫu khi danh sách rỗng.
5. **Điều kiện vô hiệu hóa (Invalidation conditions)**:
   Nếu dự án sau đó được cấu hình lại với ASP.NET Core Identity yêu cầu đăng nhập bắt buộc thì cần thêm `[Authorize]` vào `AdminSanPhamController`. Hiện tại do dự án không có Identity, điều này không áp dụng.
