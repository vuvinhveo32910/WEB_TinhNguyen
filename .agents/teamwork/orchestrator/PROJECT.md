# Project: TÁI CẤU TRÚC VÀ HIỆN ĐẠI HÓA WEBSITE TÌNH NGUYỆN (PRODUCTION REFACTORING)

## Architecture
- **Framework & Runtime**: ASP.NET Core MVC trên .NET 10.0 (`net10.0`), SDK 10.0.114.
- **ORM & Data Access**: Entity Framework Core 10.0.12 (`Microsoft.EntityFrameworkCore.SqlServer`), mở rộng `ApplicationDbContext` trong `DuLieu/`.
- **Thư mục MVC Việt hóa**:
  - `Controllers/` -> `BoDieuKhien/` (Namespace: `WEB_TinhNguyen.BoDieuKhien`)
  - `Models/` -> `MoHinh/` (Namespace: `WEB_TinhNguyen.MoHinh`)
  - `Views/` -> `GiaoDien/` (Razor View Location Formats: `/GiaoDien/{1}/{0}.cshtml`, `/GiaoDien/Shared/{0}.cshtml`)
  - `Data/` -> `DuLieu/` (Namespace: `WEB_TinhNguyen.DuLieu`)
- **Phân giải View Engine**: Cấu hình 2 tầng gồm `RazorViewEngineOptions.ViewLocationFormats` / `AreaViewLocationFormats` và `VietnameseViewLocationExpander : IViewLocationExpander`.
- **Phân hệ Bán hàng (`Areas/CuaHang`)**:
  - Bảo toàn 100% cấu trúc nội bộ: `Areas/CuaHang/Controllers/`, `Areas/CuaHang/Models/`, `Areas/CuaHang/Views/`, `Areas/CuaHang/Helpers/`.
  - Cô lập tuyệt đối bằng Layout riêng biệt: `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`.
  - Giữ nguyên 100% cấu trúc DOM và class Tailwind của toàn bộ 9 views hiện có.
- **Giao diện & UI/UX**:
  - Tailwind CSS hiện đại, animations (`float`, `pulse-slow`, `heartbeat`, `fade-in-up`).
  - Mặc định toàn trang nền trắng (`--bg-page: #ffffff`) với màu chữ mới mang đậm bản sắc tình nguyện (`--text-main: #143823` - Xanh ngọc sẫm Tây Nguyên, chuẩn WCAG AAA > 11.5:1).
  - Tách rời 100% khỏi các cột màu lưu trong database cũ.
- **Chế độ Sáng/Tối (Light/Dark Mode)**:
  - Nút chuyển đổi (Icon Button Trăng/Mặt trời) trên Header Layout chung (`GiaoDien/Shared/_Layout.cshtml`).
  - Điều khiển qua CSS variables và class `dark` trên thẻ `html`, lưu `localStorage`.
  - Khóa an toàn: Tự động tắt Dark mode và ẩn nút toggle khi duyệt vào `/CuaHang*`.

## Code Layout
```
WEB_TinhNguyen/
├── Program.cs                                    # Cấu hình Routing, RazorViewEngine, VietnameseViewLocationExpander, Migration
├── tailwind.config.js                            # Content paths: ./GiaoDien/**/*.cshtml, ./Areas/**/*.cshtml, darkMode: 'class'
├── package.json                                  # Build:css xuất vào WEB_TinhNguyen/wwwroot/css/site.css
├── BoDieuKhien/                                  # Thay thế Controllers/ (9 controllers)
│   ├── HomeController.cs
│   ├── AdminController.cs
│   ├── AdminTrangChuController.cs
│   ├── AdminChuongTrinhController.cs
│   ├── AdminGayQuyController.cs
│   ├── AdminBanToChucController.cs
│   ├── AdminTinTucController.cs
│   ├── AdminKyUcController.cs
│   └── AdminHieuUngController.cs
├── MoHinh/                                       # Thay thế Models/ (3 files)
│   ├── Entities.cs                               # Đã xóa 66 trường màu, chuẩn hóa tên bảng/cột tiếng Việt
│   ├── CauHinhTrangChuVM.cs                      # Đã xóa 66 trường màu tương ứng
│   └── ErrorViewModel.cs
├── DuLieu/                                       # Thay thế Data/ (3 files)
│   ├── ApplicationDbContext.cs                   # DbSets & Fluent API chuẩn hóa tiếng Việt
│   ├── VietnameseViewLocationExpander.cs         # Razor ViewLocation expander
│   └── Compatibility.cs                          # Lớp tương thích ngược cho Migrations cũ
├── Migrations/
│   ├── 20261104000000_XoaCotMauVaVietHoaTenBangCot.cs # Migration mới: Drop 66 cột màu, Rename bảng/cột
│   └── ApplicationDbContextModelSnapshot.cs      # Snapshot cập nhật
├── GiaoDien/                                     # Thay thế Views/ (29 files across 10 dirs)
│   ├── _ViewImports.cshtml                       # Nạp @using WEB_TinhNguyen.MoHinh
│   ├── _ViewStart.cshtml                         # Trỏ vào ~/GiaoDien/Shared/_Layout.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml                        # Layout mới: Tailwind, Dark mode toggle, CSS vars
│   │   ├── _AdminLayout.cshtml                   # Admin layout
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/ (Index, ChuongTrinh, GayQuy, BanToChuc, TinTuc, KyUc,...)
│   └── Admin/ (Index, Edit forms sạch thẻ <input type="color">)
└── Areas/
    └── CuaHang/                                  # BẢO TOÀN NGUYÊN VẸN 100% CẤU TRÚC
        ├── Controllers/ (4 controllers)
        ├── Models/ (7 models)
        ├── Helpers/ (DbInitializer.cs, SessionExtensions.cs)
        └── Views/ (9 views)
            ├── _ViewStart.cshtml                 # Trỏ Storefront -> _CuaHangLayout.cshtml
            ├── Shared/
            │   └── _CuaHangLayout.cshtml         # Layout riêng cô lập tuyệt đối khỏi giao diện mới
            ├── CuaHang/ (Index, ChiTiet)
            ├── GioHang/ (Index, DatHangThanhCong)
            └── AdminSanPham/ (Index, Create, Edit)
```

## Feature Inventory
| # | Feature | Description | Milestone | Source |
|---|---------|-------------|-----------|--------|
| 1 | Thư mục BoDieuKhien | Đổi tên `Controllers` -> `BoDieuKhien` và cập nhật namespace `WEB_TinhNguyen.BoDieuKhien` | M1 | R2 |
| 2 | Thư mục MoHinh | Đổi tên `Models` -> `MoHinh` và cập nhật namespace `WEB_TinhNguyen.MoHinh` | M1 | R2 |
| 3 | Thư mục GiaoDien | Đổi tên `Views` -> `GiaoDien` và di chuyển toàn bộ 29 view, layouts | M1 | R2 |
| 4 | Thư mục DuLieu | Đổi tên `Data` -> `DuLieu`, cập nhật namespace `WEB_TinhNguyen.DuLieu` | M1 | R2 |
| 5 | Razor ViewLocationExpander | Xây dựng `VietnameseViewLocationExpander` và đăng ký với `RazorViewEngineOptions` | M1 | R2 |
| 6 | Cập nhật 22 Layout Path | Thay thế toàn bộ `~/Views/Shared/...` thành `~/GiaoDien/Shared/...` trong các view | M1 | R2 |
| 7 | Cập nhật tailwind.config.js | Bổ sung `./GiaoDien/**/*.cshtml` vào mảng `content` và `darkMode: 'class'` | M1 | R2, R3 |
| 8 | Xóa 66 thuộc tính màu trong Models | Loại bỏ 66 trường màu HEX khỏi `Entities.cs` và `CauHinhTrangChuVM.cs` | M2 | R1 |
| 9 | Thuần hóa tiếng Việt Bảng & Cột | Đổi tên `BannerTrangChu` -> `BieuNguTrangChu`, `TVKhoanhKhac` -> `ThuVienKhoanhKhac`, `FAQTrangChu` -> `CauHoiThuongGapTrangChu`, `IDBanner` -> `IDBieuNgu`, `CTA_...` -> `KeuGoi_...`, `Hero_...` -> `NoiBat_...`, `Link...` -> `DuongDan...` | M2 | R1 |
| 10 | Code First Migration R1 | Tạo migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` (Drop 66 cột màu, Rename bảng/cột) | M2 | R1 |
| 11 | Cập nhật LINQ/SQL Queries | Cập nhật `.Include(...)` và truy vấn LINQ trong tất cả 7 Controllers lõi | M2 | R1 |
| 12 | Dọn dẹp Form Admin | Xóa toàn bộ `<input type="color">` và ô nhập mã HEX trong các view Edit Admin | M2 | R1 |
| 13 | Volunteer Identity Palette | Thiết lập biến CSS tĩnh: nền trắng `--bg-page: #ffffff`, chữ xanh rừng `--text-main: #143823`, accents | M3 | R3 |
| 14 | Smooth Keyframe Animations | Bổ sung animation `float`, `pulse-slow`, `heartbeat`, `fade-in-up` mượt mà | M3 | R3 |
| 15 | Giao diện mới GiaoDien/Home | Làm mới các trang Home, Chương trình, Gây quỹ, Tin tức, Ký ức theo phong cách tình nguyện | M3 | R3 |
| 16 | Layout Riêng _CuaHangLayout | Tạo `_CuaHangLayout.cshtml` tại `Areas/CuaHang/Views/Shared/` cô lập 100% Cửa hàng | M3 | R3 |
| 17 | Bảo toàn 100% DOM CuaHang | Giữ nguyên 100% cấu trúc HTML/DOM và class Tailwind của 9 view trong `Areas/CuaHang/` | M3 | R3 |
| 18 | Nút Toggle Sáng/Tối | Xây dựng nút icon Mặt trăng/Mặt trời trên Header chung `_Layout.cshtml` | M4 | R4 |
| 19 | Script Anti-FOUC & Persistence | Script trong `<head>` đọc `localStorage` và đặt class `dark` không gây nhấp nháy | M4 | R4 |
| 20 | Bảng màu Dark Mode | Nền đêm Tây Nguyên `#0a140e`, chữ sáng `#f8fafc`, card bề mặt `#13241b` | M4 | R4 |
| 21 | Khóa an toàn Dark Mode CuaHang | Tự động loại bỏ Dark mode và khóa Light mode khi duyệt `/CuaHang*`, ẩn nút toggle | M4 | R4 |
| 22 | Biên dịch không lỗi (`dotnet build`) | Lệnh `XDG_RUNTIME_DIR=/tmp dotnet build` thành công 100%, 0 Error | M5 | AC |
| 23 | E2E & Forensic Audit Verification | Toàn bộ kiểm thử và kiểm tra pháp y đạt chuẩn sạch (CLEAN) | M5 | AC |

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| M1 | Việt hóa Cấu trúc Thư mục & Razor ViewEngine (R2) | Đổi tên `Controllers`->`BoDieuKhien`, `Models`->`MoHinh`, `Views`->`GiaoDien`, `Data`->`DuLieu`; tạo `VietnameseViewLocationExpander`; sửa 22 layout path; cập nhật `tailwind.config.js`; kiểm tra build sạch | none | DONE |
| M2 | Tái cấu trúc CSDL & Dọn dẹp Mã nguồn (R1) | Xóa 66 thuộc tính màu; Việt hóa bảng/cột; tạo bản EF Core Migration; cập nhật LINQ queries; xóa input màu khỏi view admin | M1 | DONE |
| M3 | Đổi mới Giao diện Tailwind CSS & Cô lập Phân hệ Bán hàng (R3) | Sửa output `package.json`; thiết lập CSS variables nền trắng chữ xanh tình nguyện & animations; làm mới các trang client; tạo `_CuaHangLayout.cshtml` bảo toàn 100% CuaHang | M1, M2 | DONE |
| M4 | Tính năng Chuyển đổi Sáng/Tối Light/Dark Mode (R4) | Thêm nút Toggle Trăng/Mặt trời trên Header; script Anti-FOUC `<head>`; biến màu dark mode; chốt an toàn cưỡng chế Light mode cho `/CuaHang*` | M3 | DONE |
| M5 | E2E Testing, Adversarial Verification & Forensic Audit Gate | Biên dịch sạch `dotnet build`; kiểm thử runtime toàn bộ endpoint; Reviewers APPROVE; Challengers APPROVE; Forensic Auditor CLEAN | M1, M2, M3, M4 | DONE |



## Interface Contracts
### Razor ViewEngine ↔ GiaoDien
- Root View Location Formats: `"/GiaoDien/{1}/{0}.cshtml"`, `"/GiaoDien/Shared/{0}.cshtml"`.
- Area View Location Formats: `"/Areas/{2}/Views/{1}/{0}.cshtml"`, `"/Areas/{2}/Views/Shared/{0}.cshtml"`.
- Layout References: `~/GiaoDien/Shared/_Layout.cshtml`, `~/GiaoDien/Shared/_AdminLayout.cshtml`.

### DuLieu.ApplicationDbContext ↔ BoDieuKhien
- Entities: `WEB_TinhNguyen.MoHinh.*`
- DbContext: `WEB_TinhNguyen.DuLieu.ApplicationDbContext`
- Renamed tables: `BieuNguTrangChus`, `ThuVienKhoanhKhacs`, `CauHoiThuongGapTrangChus`.
- Preserved store tables: `SanPhamGayQuys`, `DonHangGayQuys`, `ChiTietDonHangGayQuys`.

### UI Theme ↔ Areas/CuaHang
- Scoped Layout: Storefront uses `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml` (light-mode locked, 0 dependency on main dark mode).
- CSS Variables: Main site uses `--bg-page: #ffffff`, `--text-main: #143823`. Dark mode uses `--bg-page: #0a140e`, `--text-main: #f8fafc`.
- Route Isolation: Route containing `/CuaHang` automatically strips `class="dark"` and enforces `class="light"`.
