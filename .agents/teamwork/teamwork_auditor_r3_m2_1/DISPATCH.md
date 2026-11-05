## 2026-11-04T06:50:24Z
Bạn là Forensic Auditor kiểm định pháp y tính toàn vẹn cho Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn).

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m2_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r3_m2_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`

Nhiệm vụ:
- Kiểm tra tính xác thực của mã nguồn: không có dummy/facade implementations, không che giấu lỗi, không fake code.
- Kiểm tra không có hardcode output trong các query hay test assertion.
- Kiểm tra tính toàn vẹn và cô lập của `Areas/CuaHang/`.
- Tự tay thực thi build `dotnet build` và test `dotnet run --project tests/E2ETests/E2ETests.csproj`, đối chiếu log độc lập.
- Viết `handoff.md` kết luận rõ ràng: CLEAN hoặc INTEGRITY VIOLATION (kèm bằng chứng cụ thể), và gửi tin nhắn báo cáo cho Orchestrator.
