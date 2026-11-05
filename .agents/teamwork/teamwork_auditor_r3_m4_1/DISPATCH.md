# Forensic Auditor Dispatch: Milestone M4 (Kiểm định Pháp y Tính toàn vẹn - Integrity Forensics)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m4_1/`

## Nhiệm vụ Kiểm định Pháp y:
1. **Kiểm tra Tính xác thực của Tính năng (Genuine Implementation vs Dummy/Facade)**:
   - Nút chuyển đổi Sáng/Tối, hàm `toggleThemeMode()`, và script Anti-FOUC là mã nguồn JavaScript & CSS thực thi thật, không phải mockup hay giả lập.
   - Kiểm tra xem có bất kỳ hành vi hardcode kết quả kiểm thử nào trong mã nguồn không.
2. **Kiểm tra Cô lập Tuyệt đối Cửa hàng**:
   - Xác minh `git diff WEB_TinhNguyen/Areas/CuaHang/Views/` = 0 (bảo toàn 100% cấu trúc DOM và giao diện hiện tại của phân hệ bán hàng).
   - Xác minh `_CuaHangLayout.cshtml` khóa cứng light mode, không bị ô nhiễm bởi dark mode.
3. **Tự tay thực thi Build và Test độc lập**:
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`

Ghi rõ kết luận: **CLEAN** hoặc **INTEGRITY VIOLATION** kèm bằng chứng chi tiết vào `handoff.md`.
Lưu ý: Kết luận INTEGRITY VIOLATION sẽ phủ quyết ngay lập tức toàn bộ milestone.
## 2026-11-04T07:57:58Z
[Message from 656d28b8-bcd4-4f61-b4e3-dd425076f84e]
Bạn là Forensic Auditor kiểm định pháp y tính toàn vẹn cho Milestone M4 (Requirement R4: Chuyển đổi Sáng/Tối Light/Dark Mode).

Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m4_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m4_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`

Nhiệm vụ:
- Kiểm tra tính xác thực: nút toggle, hàm toggleThemeMode, Anti-FOUC script là mã thật, không phải mockup/dummy.
- Kiểm tra bảo toàn tuyệt đối `Areas/CuaHang/` (git diff = 0).
- Kiểm tra không có hardcoding kết quả trong test suites.
- Tự tay thực thi build và test độc lập.
- Viết `handoff.md` kết luận rõ ràng CLEAN hoặc INTEGRITY VIOLATION kèm bằng chứng chi tiết, và gửi tin nhắn báo cáo cho Orchestrator.
