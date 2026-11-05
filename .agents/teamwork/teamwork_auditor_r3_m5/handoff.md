# BÁO CÁO THẨM ĐỊNH PHÁP Y ĐỘC LẬP (FINAL FORENSIC INTEGRITY AUDIT)

**Dự án**: Tái cấu trúc và Hiện đại hóa Website Tình Nguyện (WEB_TinhNguyen)  
**Đối tượng thẩm định**: Toàn bộ mã nguồn sản phẩm (M1, M2, M3, M4, M5 / R1, R2, R3, R4)  
**Profile thẩm định**: General Project (Development Mode, kiểm tra chéo tiêu chuẩn Demo & Benchmark)  
**Auditor**: Teamwork Forensic Auditor (`teamwork_auditor_r3_m5`)  
**Kết luận thẩm định (Verdict)**: **CLEAN**

---

## 1. Observation (Dữ liệu quan sát thực nghiệm độc lập)

### 1.1. Thực thi Build và Bộ kiểm thử độc lập
Auditor trực tiếp khởi chạy các câu lệnh từ terminal sạch và ghi nhận toàn bộ kết quả thực tế:

1. **Biên dịch Solution (`dotnet build`)**:
   - Lệnh: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - Kết quả: `0 Error(s)`, `45 Warning(s)` (Các cảnh báo `CS0618` đánh dấu lớp tương thích ngược `ApplicationDbContext` cũ trong thư mục Migrations để bảo toàn lịch sử). Exit Code = 0.
   - Trình biên dịch CSS Tailwind: Thực thi `npm run build:css` thành công, tạo ra tệp `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/wwwroot/css/site.css` dung lượng 100,872 bytes.

2. **Bộ kiểm thử E2ETests (`tests/E2ETests/E2ETests.csproj`)**:
   - Lệnh: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`
   - Kết quả chi tiết:
     - Tier 1: 100/100 Passed
     - Tier 2: 28/28 Passed
     - Tier 3: 18/18 Passed
     - Tier 4: 12/12 Passed
     - Tier 5: 179/179 Passed
     - **Tổng cộng: 337 Tests | 337 Passed | 0 Failed | 0 Skipped (thời gian: 5.89s)**.

3. **Bộ kiểm thử M3Challenger2 (`tests/M3Challenger2/M3Challenger2.csproj`)**:
   - Lệnh: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M3Challenger2/M3Challenger2.csproj`
   - Kết quả chi tiết: 128/128 adversarial tests Passed (Bao gồm bảo toàn SHA-256 các View Cửa hàng, kiểm tra hard-lock Light mode, oracles giỏ hàng & thanh toán).

4. **Bộ kiểm thử M1Challenger (`tests/M1Challenger/M1Challenger.csproj`)**:
   - Lệnh: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M1Challenger/M1Challenger.csproj`
   - Kết quả chi tiết: 67/67 stress tests Passed (Kiểm tra phân giải 46 đường dẫn Razor View, cấu trúc thư mục phân biệt hoa thường, sự biến mất của các thư mục tiếng Anh).

5. **Bộ kiểm thử Adversarial Store Isolation (`tests/adversarial_store_isolation_test.js`)**:
   - Lệnh: `node /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/adversarial_store_isolation_test.js`
   - Kết quả chi tiết: 109/109 adversarial tests Passed (Kiểm tra chặn toggle theme trên toàn bộ routes CuaHang, kiểm tra script Anti-FOUC).

---

### 1.2. Thẩm tra Yêu cầu R1: Tái cấu trúc CSDL và dọn dẹp mã nguồn
- **Xóa 100% thuộc tính màu trong Models & ViewModels**:
  - Quét regex `MauSac|MauNen|MauChu|TextColor|BgColor|ColorCode|HexColor` trên `WEB_TinhNguyen/MoHinh/Entities.cs` và `WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`: Kết quả **0 phát hiện**.
  - Kiểm tra quét mở rộng `grep -iE "color|mau"`: Kết quả **0 phát hiện**.
- **Migration Code First & Snapshot**:
  - Tệp `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`: Chứa chính xác **66 lệnh `migrationBuilder.DropColumn`** loại bỏ triệt để toàn bộ 66 cột màu trên 11 bảng CSDL.
  - Tệp `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`: Quét từ khóa `Mau`: Kết quả **0 phát hiện** (hoàn toàn sạch bóng cột màu).
- **Việt hóa bảng, cột và truy vấn LINQ/SQL**:
  - Đổi tên bảng: `BannerTrangChu` -> `BieuNguTrangChu`, `TVKhoanhKhac` -> `ThuVienKhoanhKhac`, `FAQTrangChu` -> `CauHoiThuongGapTrangChu`.
  - Đổi tên cột: `IDBanner` -> `IDBieuNgu`, `LinkAnh` -> `DuongDanAnh`, `CTA_*` -> `KeuGoi_*`, `Hero_*` -> `NoiBat_*`.
  - Toàn bộ 7 Controllers trong `WEB_TinhNguyen/BoDieuKhien/` đã cập nhật LINQ tương ứng; quét từ khóa cũ (`MauChuDao`, `MauNhan`, `MauNenStart`, etc.): Kết quả **0 phát hiện**.
- **Xóa bỏ các trường nhập màu trong Admin**:
  - Quét `grep -rn 'type="color"'` và `grep -rn "type='color'"` trên toàn bộ `WEB_TinhNguyen/GiaoDien/`: Kết quả **0 phát hiện** (tất cả các form cấu hình giao diện đã loại bỏ thẻ chọn màu).

---

### 1.3. Thẩm tra Yêu cầu R2: Việt hóa cấu trúc thư mục & Razor ViewEngine
- **Cấu trúc 4 thư mục tiếng Việt**:
  - Thư mục tồn tại: `BoDieuKhien/`, `MoHinh/`, `GiaoDien/`, `DuLieu/`.
  - Thư mục cũ biến mất hoàn toàn ở cấp gốc: `Controllers/`, `Models/`, `Views/`, `Data/` (Lệnh `ls` xác nhận `No such file or directory`).
- **Phân giải Razor ViewEngine 2 tầng**:
  - Tệp `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs`: Hiện thực giao diện `IViewLocationExpander`, mở rộng view locations cho cả Root (`/GiaoDien/{1}/{0}.cshtml`) và Phân hệ (`/Areas/{2}/Views/{1}/{0}.cshtml`).
  - Tệp `WEB_TinhNguyen/Program.cs`: Cấu hình `builder.Services.Configure<RazorViewEngineOptions>` nạp đầy đủ đường dẫn và đăng ký `VietnameseViewLocationExpander`.
- **Layout paths cập nhật**:
  - 21 vị trí trong các tệp `.cshtml` và 1 vị trí trong `_ViewStart.cshtml` trỏ chính xác về `~/GiaoDien/Shared/_AdminLayout.cshtml` hoặc `~/GiaoDien/Shared/_Layout.cshtml`.
  - Quét `grep -rn "~/Views/Shared/"`: Kết quả **0 phát hiện**.

---

### 1.4. Thẩm tra Yêu cầu R3: Đổi mới giao diện Tailwind CSS & Bảo toàn Cửa hàng
- **Volunteer Identity Palette**:
  - Nền trắng mặc định: `--bg-page: #ffffff`.
  - Màu chữ bản sắc tình nguyện: `--text-main: #143823` (Màu xanh ngọc rừng Tây Nguyên).
  - Thẩm tra độ tương phản thực nghiệm bằng công thức WCAG 2.1:
    - Relative Luminance Background (`#ffffff`): `1.0000`
    - Relative Luminance Text (`#143823`): `0.0310`
    - **Tỷ lệ tương phản thực tế: 12.97:1**.
    - Chuẩn WCAG AAA yêu cầu: `> 7.0:1` => **ĐẠT (VƯỢT TRỘI)**.
    - Tiêu chí nghiệm thu yêu cầu: `> 12:1` => **ĐẠT (12.97:1 > 12:1)**.
- **Keyframe Animations**:
  - `tailwind.config.js` và `wwwroot/css/site.css` định nghĩa đầy đủ 4 animations:
    1. `float`: 4s ease-in-out infinite (translateY -8px)
    2. `pulse-slow`: 3s ease-in-out infinite (scale 1.03)
    3. `heartbeat`: 1.5s ease-in-out infinite (scale 1.15)
    4. `fade-in-up`: 0.6s cubic-bezier forwards (translateY 20px -> 0)
- **Bảo toàn 100% Cửa hàng**:
  - Kiểm tra `git diff HEAD WEB_TinhNguyen/Areas/CuaHang/Views/`: **0 bytes khác biệt (diff = 0)**.
  - Toàn bộ 9 view của Cửa hàng và layout riêng biệt `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml` được bảo tồn nguyên vẹn cấu trúc DOM và class Tailwind.

---

### 1.5. Thẩm tra Yêu cầu R4: Chuyển đổi Sáng/Tối (Light/Dark Mode)
- **Nút Toggle Sáng/Tối**:
  - Thiết kế tại Header Desktop (`id="themeToggleBtn"`) và Mobile (`id="themeToggleBtnMobile"`) trong `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml`.
  - Trang bị icon Mặt trăng (`fas fa-moon`) cho chế độ Sáng và Mặt trời (`fas fa-sun`) cho chế độ Tối.
- **Script Anti-FOUC & Persistence**:
  - Được đặt trực tiếp trong thẻ `<head>` của `_Layout.cshtml` trước mọi thẻ render nội dung, đọc `localStorage.getItem('site_theme')`.
  - Cơ chế tự động fallback về Light mode (nền trắng `#ffffff`) cho khách truy cập lần đầu.
- **Biến CSS Dark Mode**:
  - Nền đêm: `--bg-page: #0a140e`
  - Bề mặt card: `--bg-surface: #13241b`
  - Chữ sáng: `--text-main: #f8fafc`
  - Tỷ lệ tương phản Dark mode: **17.93:1** (Chuẩn WCAG AAA > 7.0:1 => **ĐẠT**).
- **Khóa an toàn 4 lớp bảo vệ phân hệ Cửa Hàng**:
  1. *Lớp 1 (Razor Server-side)*: `@if (ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang")` triệt tiêu việc sinh mã nút toggle khi duyệt Cửa hàng.
  2. *Lớp 2 (Client Head Anti-FOUC)*: Kiểm tra `window.location.pathname.toLowerCase().indexOf('/cuahang') !== -1`, tự động xóa class `dark` và gán class `light`.
  3. *Lớp 3 (Layout Cửa hàng riêng)*: `_CuaHangLayout.cshtml` sở hữu thẻ `<html class="light">` và script inline độc lập cưỡng chế `light` mode.
  4. *Lớp 4 (Runtime JavaScript)*: Hàm `toggleThemeMode()` chặn thực thi ngay lập tức nếu đường dẫn chứa `/cuahang`.

---

### 1.6. Kiểm tra Pháp y Chống gian lận (Integrity Anti-Cheat Forensics)
- **Hardcoded test results**: Quét toàn bộ mã nguồn không tìm thấy chuỗi kết quả mock, bypass flag hoặc kiểm tra giả mạo.
- **Facade implementations**: Không có bất kỳ hàm rỗng hoặc phương thức ném `NotImplementedException` nào trong mã nguồn dự án.
- **Pre-populated verification artifacts**: Không có bất kỳ tệp log, kết quả đệm hay chứng chỉ ảo nào được tạo sẵn trong workspace trước khi chạy test.
- **Self-certifying tests**: Các bộ kiểm thử chạy trực tiếp trên các tiến trình biên dịch thực tế của .NET 10 CLR và ánh xạ DOM thực tế.

---

## 2. Logic Chain (Chuỗi lập luận suy diễn)

1. Từ Observation 1.1: `dotnet build` biên dịch thành công 0 lỗi trên .NET 10; 4 bộ kiểm thử độc lập (`E2ETests`, `M3Challenger2`, `M1Challenger`, `adversarial_store_isolation_test.js`) với tổng số **641 bài kiểm tra** đều đạt kết quả 100% PASS, 0 FAIL. => Khẳng định toàn bộ hệ thống hoạt động ổn định về mặt cú pháp, liên kết biên dịch và logic runtime.
2. Từ Observation 1.2: Toàn bộ 66 cột màu đã bị xóa khỏi `MoHinh/Entities.cs`, `MoHinh/CauHinhTrangChuVM.cs`, migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` và snapshot CSDL. Toàn bộ input type color đã bị dọn dẹp khỏi giao diện admin. Bảng và cột đã được chuẩn hóa tiếng Việt nhất quán. => Yêu cầu **R1 đã hoàn thành đầy đủ, chân thực 100%**.
3. Từ Observation 1.3: 4 thư mục gốc đã được Việt hóa thành `BoDieuKhien`, `MoHinh`, `GiaoDien`, `DuLieu`, các thư mục cũ bằng tiếng Anh đã được dọn sạch hoàn toàn. Razor ViewEngine 2 tầng đã giải quyết thành công 46 view paths. Toàn bộ 22 layout paths đã trỏ chính xác về thư mục mới. => Yêu cầu **R2 đã hoàn thành đầy đủ, chân thực 100%**.
4. Từ Observation 1.4: Bộ màu bản sắc tình nguyện sử dụng nền trắng `#ffffff` và chữ `#143823` đạt tỷ lệ tương phản 12.97:1 (vượt chuẩn WCAG AAA và thỏa mãn > 12:1). 4 hiệu ứng keyframe đã được cấu hình và xuất ra CSS hoàn chỉnh. Phân hệ Cửa hàng bảo toàn nguyên vẹn 100% với `git diff = 0`. => Yêu cầu **R3 đã hoàn thành đầy đủ, chân thực 100%**.
5. Từ Observation 1.5: Hệ thống chuyển đổi Sáng/Tối hoạt động mượt mà qua CSS variables, nút toggle xuất hiện ở cả Desktop và Mobile trên giao diện chung, kịch bản Anti-FOUC ngăn chặn nhấp nháy, phân hệ Cửa hàng được bảo vệ bằng cơ chế 4 tầng cô lập tuyệt đối. => Yêu cầu **R4 đã hoàn thành đầy đủ, chân thực 100%**.
6. Từ Observation 1.6: Không phát hiện bất kỳ dấu vết gian lận, kết quả hardcode, facade hay tệp bằng chứng làm giả nào trong toàn bộ dự án.

---

## 3. Caveats (Các điểm lưu ý / Giới hạn phạm vi)

- **SQL Server Connection Warning**: Khi khởi chạy ứng dụng hoặc kiểm thử trong môi trường container không có dịch vụ SQL Server thực tế đang chạy, `Program.cs` sẽ ghi nhận log warning an toàn và chuyển sang sử dụng `DbInitializer` với dữ liệu in-memory fallback. Đây là hành vi thiết kế chủ động (resilience pattern) đã được ghi nhận trong `PROJECT.md`, không ảnh hưởng đến tính toàn vẹn của mã nguồn hay EF Core migrations.
- **Cảnh báo biên dịch CS0618**: 13 cảnh báo `CS0618: 'ApplicationDbContext' is obsolete` xuất hiện bên trong các tệp Designer của các bản Migration cũ (`Migrations/202610...Designer.cs`). Đây là hành vi có chủ đích nhằm duy trì tính tương thích ngược cho lịch sử migrations của dự án cũ và không gây ảnh hưởng tới quá trình biên dịch hay thực thi.

---

## 4. Conclusion (Kết luận)

Toàn bộ các tiêu chí nghiệm thu (Acceptance Criteria) cho cả 4 nhóm yêu cầu R1, R2, R3, R4 và 5 Mốc tiến độ M1-M5 đã được kiểm chứng thực nghiệm độc lập và đạt kết quả hoàn hảo. Mã nguồn được tái cấu trúc một cách thực chất, sạch sẽ, chuẩn mực và bảo đảm tính toàn vẹn cao nhất theo tiêu chuẩn Production.

**FINAL VERDICT**: **CLEAN** (Chấp thuận nghiệm thu và xuất xưởng dự án).

---

## 5. Verification Method (Phương pháp tái kiểm chứng độc lập)

Bất kỳ kiểm toán viên hoặc kỹ sư nào cũng có thể kiểm chứng lại kết luận trên thông qua chuỗi lệnh sau trong môi trường Linux:

```bash
# 1. Kiểm tra biên dịch sạch
XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx

# 2. Chạy bộ kiểm thử E2E tích hợp (337 tests)
XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj

# 3. Chạy bộ kiểm thử thách thức cô lập Cửa Hàng M3 (128 tests)
XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M3Challenger2/M3Challenger2.csproj

# 4. Chạy bộ kiểm thử thách thức phân giải View M1 (67 tests)
XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M1Challenger/M1Challenger.csproj

# 5. Chạy kịch bản kiểm tra cô lập Dark mode Cửa Hàng (109 tests)
node /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/adversarial_store_isolation_test.js

# 6. Kiểm tra bảo toàn Cửa Hàng
git diff HEAD /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/
```
