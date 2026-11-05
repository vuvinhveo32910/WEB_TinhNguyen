# Reviewer 1 Dispatch: Milestone M4 (Requirement R4: Chuyển đổi Sáng/Tối Light/Dark Mode)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m4_1/`

## Trọng tâm Review:
1. **Kiểm tra Nút chuyển đổi Sáng/Tối**:
   - Trên Header Desktop (`id="themeToggleBtn"`) và Header Mobile (`id="themeToggleBtnMobile"`).
   - Nút chứa icon Mặt trăng (`fa-moon`) khi sáng và icon Mặt trời (`fa-sun`) khi tối.
   - Nút được bọc bởi `@if (ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang")`.
2. **Kiểm tra Anti-FOUC Script**:
   - Script trong `<head>` nạp theme từ `localStorage.getItem('site_theme')`, cưỡng chế light mode khi ở `/cuahang*`.
3. **Kiểm tra hàm `toggleThemeMode()`**:
   - Điều khiển hoán đổi class `dark`/`light`, lưu `site_theme` vào `localStorage`, chặn thực thi trên `/cuahang*`.
4. **Kiểm tra Phối màu Dark Mode**:
   - Nền tối Tây Nguyên `--bg-page: #0a140e`, `--bg-surface: #13241b`, `--text-main: #f8fafc`.
   - Nền sáng: `--bg-page: #ffffff`, `--text-main: #143823`.
5. **Thực thi build và test độc lập**:
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`

Ghi rõ kết luận: **APPROVE** hoặc **REQUEST_CHANGES** kèm bằng chứng chi tiết vào `handoff.md`.

## 2026-11-04T07:57:19Z
[Message from 656d28b8-bcd4-4f61-b4e3-dd425076f84e]
Bạn là Reviewer 1 cho Milestone M4 (Requirement R4: Chuyển đổi Sáng/Tối Light/Dark Mode).

Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m4_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m4_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`

Nhiệm vụ:
- Kiểm tra nút toggle trên Desktop và Mobile Header, icon Mặt trăng/Mặt trời.
- Kiểm tra script Anti-FOUC trong head và hàm toggleThemeMode().
- Kiểm tra biến CSS dark mode (#0a140e, #13241b, #f8fafc) và light mode (#ffffff, #143823).
- Chạy lệnh build và test độc lập.
- Viết `handoff.md` kết luận APPROVE hoặc REQUEST_CHANGES kèm bằng chứng chi tiết, và gửi tin nhắn báo cáo cho Orchestrator.
