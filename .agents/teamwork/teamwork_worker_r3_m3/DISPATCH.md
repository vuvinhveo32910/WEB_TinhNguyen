# Nhiệm vụ: Triển khai Milestone M3 — Đổi mới Giao diện Tailwind CSS & Bảo toàn Phân hệ Bán hàng (Requirement R3)

## Thông tin nhiệm vụ
- **Vai trò**: Chuyên gia UI/UX & Lập trình viên Tailwind CSS (.NET 10)
- **Thư mục làm việc của Agent**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/`
- **Mã định danh nhiệm vụ**: `worker_r3_m3`
- **Yêu cầu gốc**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC đọc trước khi làm)
- **Kế hoạch dự án**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- **Bản thiết kế chi tiết UI/UX & Tailwind**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3/handoff.md`

## CẢNH BÁO TÍNH TOÀN VẸN (MANDATORY INTEGRITY WARNING)
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Phạm vi công việc (Scope)
Thực thi trọn vẹn Yêu cầu R3 theo bản thiết kế chi tiết của Chuyên gia UI/UX:
1. **Thiết lập Bảng màu & Biến CSS Bản sắc Tình nguyện (Volunteer Identity Palette)**:
   - Trong `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml` (hoặc CSS tĩnh):
     - Mặc định nền trắng tuyền: `--bg-page: #ffffff`, `--bg-surface: #ffffff`, `--bg-surface-soft: #f8faf8`.
     - Màu chữ mới mang bản sắc tình nguyện: `--text-main: #143823` (xanh ngọc sẫm Tây Nguyên, chuẩn WCAG AAA > 11.5:1), `--text-muted: #3d6049`, `--text-subtle: #6b8f77`.
     - Màu thương hiệu áo xanh: `--volunteer-primary: #15803d`, `--volunteer-accent: #22c55e`, `--volunteer-light: #dcfce7`.
     - Màu điểm nhấn ấm áp: `--volunteer-sun: #f59e0b` (vàng nhiệt huyết), `--volunteer-heart: #f43f5e` (trái tim thiện nguyện).
2. **Cấu hình Smooth Keyframe Animations**:
   - Khai báo animations trong CSS / Tailwind config:
     - `animate-float` (chuyển động bồng bềnh mascot, trăng sáng).
     - `animate-pulse-slow` (nhịp tim kêu gọi đóng góp, ủng hộ).
     - `animate-heartbeat` (trái tim gây quỹ).
     - `animate-fade-in-up` (xuất hiện mượt mà các khối nội dung).
3. **Làm mới Giao diện các trang ngoài Cửa hàng (`WEB_TinhNguyen/GiaoDien/Home/`)**:
   - Hiện đại hóa, truyền cảm hứng, responsive mượt mà trên cả desktop và mobile cho các trang:
     - `Index.cshtml`: Hero banner hiện đại, đếm ngược sự kiện mượt mà, thư viện khoảnh khắc đẹp mắt, FAQ accordion tinh tế.
     - `ChuongTrinh.cshtml`: Dòng thời gian các chặng hành trình tình nguyện với card trực quan, badge màu tình nguyện.
     - `GayQuy.cshtml`: Giao diện gây quỹ hiện đại, minh bạch, tiến độ kêu gọi ấn tượng, danh sách kênh đóng góp trực quan.
     - `BanToChuc.cshtml`: Sơ đồ tổ chức phân cấp trang nhã, thẻ thành viên hiện đại.
     - `TinTuc.cshtml`: Lưới tin tức thanh lịch, modal đọc tin tiện ích.
     - `KyUc.cshtml`: Hồi ức tình nguyện sống động.
4. **BẢO TOÀN 100% CẤU TRÚC MVC/DOM VÀ GIAO DIỆN PHÂN HỆ BÁN HÀNG (`Areas/CuaHang/`)**:
   - Tạo Layout riêng `WEB_TinhNguyen/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`:
     - Giữ nguyên 100% HTML/DOM và class Tailwind gốc của Storefront Cửa hàng (`bg-slate-50`, `text-slate-900`, `from-[#143220]`, giỏ hàng, popup VietQR).
     - Khóa cứng chế độ Sáng (`class="light bg-slate-50"`), không nhúng nút chuyển đổi Dark mode.
     - Tách biệt hoàn toàn, không phụ thuộc vào biến CSS mới của trang chủ.
   - Cập nhật `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`:
     - Nếu controller là Admin -> trỏ `~/GiaoDien/Shared/_AdminLayout.cshtml`.
     - Ngược lại (Storefront) -> trỏ `~/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`.
   - TUYỆT ĐỐI KHÔNG SỬA ĐỔI BẤT KỲ FILE NÀO trong:
     - `Areas/CuaHang/Views/CuaHang/` (Index, ChiTiet)
     - `Areas/CuaHang/Views/GioHang/` (Index, DatHangThanhCong)
     - `Areas/CuaHang/Views/AdminSanPham/` (Index, Create, Edit)
     - `Areas/CuaHang/Views/AdminDonHang/` (Index, ChiTiet)
     - `Areas/CuaHang/Controllers/`
     - `Areas/CuaHang/Models/`
5. **Cập nhật Build Output Pipeline**:
   - Sửa `package.json` tại thư mục gốc để Tailwind CLI biên dịch trực tiếp vào `WEB_TinhNguyen/wwwroot/css/site.css`:
     `"build:css": "tailwindcss -i Styles/app.css -o WEB_TinhNguyen/wwwroot/css/site.css --minify"`
   - Cập nhật `tailwind.config.js`:
     Bổ sung `./GiaoDien/**/*.cshtml`, `./WEB_TinhNguyen/GiaoDien/**/*.cshtml`, `darkMode: 'class'`, và các màu volunteer tokens + keyframe animations.
6. **Sửa lỗi tiểu tiết (Refactor Polish)**:
   - Trong `WEB_TinhNguyen/MoHinh/Entities.cs:641`, sửa setter của `LinkBaiVietGoc`:
     `set => DuongDanBaiVietGoc = value;` (thay vì gán vào `DuongDanAnhTrongBai`).

## Kiểm tra và Xác minh bắt buộc
Trước khi viết `handoff.md`, Worker phải chạy:
1. `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` -> 0 Error.
2. `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj` -> 100% Pass (tối thiểu 325/325 tests).
3. `git status WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/ WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/ WEB_TinhNguyen/Areas/CuaHang/Views/Admin*` -> Không có thay đổi nào trong các view này.

## Yêu cầu Báo cáo Handoff
Viết file `handoff.md` tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/handoff.md` chi tiết các nội dung đã hoàn thành.

## 2026-11-04T07:01:56Z
Bạn là Worker chịu trách nhiệm thực thi Milestone M3: Đổi mới Giao diện Tailwind CSS & Bảo toàn Phân hệ Bán hàng (Requirement R3) cho dự án WEB_TinhNguyen.

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/`
Bắt đầu bằng việc đọc kỹ các tài liệu sau:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/DISPATCH.md` (Hướng dẫn chi tiết từng bước)
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3/handoff.md` (Bản thiết kế chi tiết kiến trúc UI/UX và Tailwind)
4. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

CẢNH BÁO TÍNH TOÀN VẸN (MANDATORY INTEGRITY WARNING):
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected và your work WILL be rejected.

Thực hiện các bước theo DISPATCH.md:
- Thiết lập hệ thống biến CSS tĩnh và Tailwind tokens với nền trắng mặc định (`#ffffff`) và chữ xanh bản sắc tình nguyện (`#143823`).
- Bổ sung keyframe animations mượt mà (`float`, `pulse-slow`, `heartbeat`, `fade-in-up`).
- Hiện đại hóa, truyền cảm hứng các view trong `GiaoDien/Home/` (Index, ChuongTrinh, GayQuy, BanToChuc, TinTuc, KyUc).
- BẢO TOÀN 100% CẤU TRÚC DOM VÀ GIAO DIỆN PHÂN HỆ BÁN HÀNG `Areas/CuaHang/`: tạo layout riêng `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml` khóa cứng Light mode, điều hướng qua `_ViewStart.cshtml`, giữ nguyên 100% các file views bên trong `Areas/CuaHang/Views/`.
- Sửa đường dẫn build output trong `package.json` và cấu hình `tailwind.config.js`.
- Sửa setter `LinkBaiVietGoc` trong `MoHinh/Entities.cs:641`.
- Kiểm tra biên dịch `dotnet build` và chạy test suite E2ETests pass 100%.
- Viết `handoff.md` tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/handoff.md` và gửi tin nhắn báo cáo cho Orchestrator.

