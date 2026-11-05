# Forensic Auditor Dispatch: Milestone M3 (Kiểm định Pháp y Tính toàn vẹn - Integrity Forensics)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m3_1/`

## Nhiệm vụ Kiểm định Pháp y:
1. **Kiểm tra Tính xác thực của Giao diện (No Facade / No Mock UI)**:
   - Xác minh các view `GiaoDien/Home/` được viết bằng mã Razor và Tailwind CSS thật, kết nối view models thật, không phải giao diện dummy giả lập.
   - Kiểm tra `package.json` và `tailwind.config.js` có thực sự biên dịch ra CSS thật (`WEB_TinhNguyen/wwwroot/css/site.css`).
2. **Kiểm tra Tuyệt đối Không Sửa Đổi Trái Phép `Areas/CuaHang/`**:
   - Kiểm tra `git diff` xem có bất kỳ thay đổi nào bên trong các view của `Areas/CuaHang/Views/` không.
   - Kiểm tra `_CuaHangLayout.cshtml` có thực sự cô lập storefront của Cửa hàng hay không.
3. **Kiểm tra Không có Hardcoding hay Xâm phạm Test Suites**:
   - Kiểm tra `tests/E2ETests/` xem có bị sửa đổi để làm giả kết quả test không.
4. **Tự tay thực thi Build và Test độc lập**:
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`

Ghi rõ kết luận: **CLEAN** hoặc **INTEGRITY VIOLATION** kèm bằng chứng chi tiết vào `handoff.md`.
Lưu ý: Kết luận INTEGRITY VIOLATION sẽ phủ quyết ngay lập tức toàn bộ milestone.

## 2026-11-04T07:28:09Z
Bạn là Forensic Auditor kiểm định pháp y tính toàn vẹn cho Milestone M3 (Requirement R3: Đổi mới Giao diện Tailwind CSS & Bảo toàn CuaHang).

Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m3_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m3_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m3/handoff.md`

Nhiệm vụ:
- Kiểm tra tính xác thực của giao diện: mã Razor và CSS thật, không dùng facade/dummy giao diện.
- Kiểm tra `Areas/CuaHang/` tuyệt đối không bị sửa đổi trái phép (git diff = 0 trên các view hiện hữu).
- Kiểm tra không có hardcoding kết quả trong test suite.
- Tự tay thực thi build và test độc lập.
- Viết `handoff.md` kết luận rõ ràng CLEAN hoặc INTEGRITY VIOLATION kèm bằng chứng chi tiết, và gửi tin nhắn báo cáo cho Orchestrator.
