# Nhiệm vụ: Triển khai Milestone M4 — Tính năng Chuyển đổi Sáng/Tối Light/Dark Mode (Requirement R4)

## Thông tin nhiệm vụ
- **Vai trò**: Lập trình viên Front-End / JavaScript & UI/UX Specialist (.NET 10)
- **Thư mục làm việc của Agent**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/`
- **Mã định danh nhiệm vụ**: `worker_r3_m4`
- **Yêu cầu gốc**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC đọc trước khi làm)
- **Kế hoạch dự án**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- **Bản thiết kế chi tiết Light/Dark Mode**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3/handoff.md` (Đặc biệt mục 2.2 và 4.2)

## CẢNH BÁO TÍNH TOÀN VẸN (MANDATORY INTEGRITY WARNING)
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Phạm vi công việc (Scope)
Thực thi trọn vẹn Yêu cầu R4:
1. **Nút chuyển đổi Sáng/Tối (Light/Dark Mode Toggle Button)**:
   - Trong `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml`:
     - Xây dựng nút icon Mặt trăng (`fa-moon`) / Mặt trời (`fa-sun`) dạng button tròn tinh gọn, đặt trên Header Desktop (cạnh giỏ hàng/đóng góp) và Header Mobile.
     - Nút chỉ hiển thị khi ngoài phân hệ Cửa Hàng: `@if (ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang")`.
     - Trạng thái Sáng: icon Mặt trăng màu xanh thẫm, hover xoay nhẹ.
     - Trạng thái Tối: icon Mặt trời màu vàng ấm `#fbbf24`, phát sáng nhẹ.
2. **Cơ chế Chống chớp nháy (Anti-FOUC Script)**:
   - Trong thẻ `<head>` của `_Layout.cshtml`, đặt script tự thực thi:
     - Đọc `localStorage.getItem('site_theme')`.
     - Nếu `/cuahang` trong pathname: cưỡng chế gỡ class `dark` và thêm class `light`.
     - Nếu lưu `dark`: thêm class `dark` vào thẻ `html`.
     - Mặc định: nền trắng với class `light`.
3. **JavaScript chuyển đổi Theme (`toggleThemeMode()`)**:
   - Gắn hàm `toggleThemeMode()` chuyển đổi giữa class `dark` và `light` trên `document.documentElement`, lưu `localStorage.setItem('site_theme', ...)`.
   - Ngăn chặn hoàn toàn nếu URL thuộc `/cuahang`.
4. **Phối màu Chế độ Tối (Dark Mode Styling)**:
   - CSS Variables khi `html.dark`:
     - Nền tối đêm rừng Tây Nguyên: `--bg-page: #0a140e`, `--bg-surface: #13241b`, `--bg-surface-soft: #0d1d14`.
     - Màu chữ sáng rõ: `--text-main: #f8fafc`, `--text-muted: #cbd5e1`, `--text-subtle: #94a3b8`.
     - Card border: `rgba(74, 222, 128, 0.2)`, shadow nổi bật.
   - Khi chuyển về Sáng:
     - Nền trắng: `--bg-page: #ffffff`, `--bg-surface: #ffffff`.
     - Chữ xanh tình nguyện: `--text-main: #143823`.
5. **Khóa an toàn phân hệ Cửa hàng (Safety Lock)**:
   - Đảm bảo khi duyệt vào bất kỳ trang nào của `/CuaHang*`, giao diện lập tức khóa chế độ Sáng (`class="light"`), nút toggle tự động ẩn, và không bị ảnh hưởng bởi theme tối.
6. **BẢO TOÀN NGUYÊN VẸN `Areas/CuaHang/`**:
   - KHÔNG ĐƯỢC CHỈNH SỬA các file view nội bộ trong `Areas/CuaHang/Views/`.

## Kiểm tra và Xác minh bắt buộc
Trước khi viết `handoff.md`, Worker phải chạy:
1. `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` -> 0 Error.
2. `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj` -> 100% Pass (tối thiểu 325/325 tests).
3. Kiểm tra mã nguồn `_Layout.cshtml` có đầy đủ nút toggle, hàm `toggleThemeMode()`, script Anti-FOUC, và biến màu dark mode.

## Yêu cầu Báo cáo Handoff
Viết file `handoff.md` tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`.

## 2026-11-04T07:39:55Z
[Message] timestamp=2026-11-04T07:39:55Z sender=656d28b8-bcd4-4f61-b4e3-dd425076f84e priority=MESSAGE_PRIORITY_HIGH content=Bạn là Worker chịu trách nhiệm thực thi Milestone M4: Tính năng Chuyển đổi Sáng/Tối Light/Dark Mode (Requirement R4) cho dự án WEB_TinhNguyen.
Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/`
Bắt đầu bằng việc đọc kỹ các tài liệu sau:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/DISPATCH.md` (Hướng dẫn chi tiết từng bước)
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3/handoff.md` (Mục 2.2 và 4.2: Thiết kế Light/Dark Mode)
4. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

