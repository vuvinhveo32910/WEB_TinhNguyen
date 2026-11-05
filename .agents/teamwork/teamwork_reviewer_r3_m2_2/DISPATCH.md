# Reviewer 2 Dispatch: Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_2/`

## Trọng tâm Review:
1. **Kiểm tra độc lập về Tính toàn vẹn của CSDL**:
   - Đối chiếu giữa bản thiết kế ban đầu (`teamwork_explorer_r3_survey_1/handoff.md`) và mã nguồn hiện tại xem có bỏ sót bất kỳ trường màu nào trong số 66 trường đã khảo sát không.
   - Kiểm tra `ApplicationDbContext.cs` và `Compatibility.cs` xem các alias và DbSet mới có đảm bảo type safety không.
2. **Kiểm tra Migration & Cơ chế Fallback**:
   - Migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` có logic `Down()` hoàn nguyên hợp lệ không.
   - `Program.cs` có kiểm tra `context.Database.CanConnect()` an toàn trước khi gọi `Migrate()` không.
3. **Kiểm tra Giao diện & Trải nghiệm**:
   - Các view người dùng có còn inline-styles `style="color: @Model..."` nào gây lỗi hiển thị không.
   - Các view Admin Edit có hoạt động bình thường sau khi loại bỏ color inputs không.
4. **Bảo toàn CuaHang**:
   - Xác nhận `Areas/CuaHang/` tuyệt đối không bị tác động.
5. **Xác minh thực thi**:
   - Chạy lệnh build: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - Chạy test suite: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`

Ghi rõ kết luận: **APPROVE** hoặc **REQUEST_CHANGES** kèm bằng chứng chi tiết vào `handoff.md`.

## 2026-11-04T06:50:01Z
Bạn là Reviewer 2 cho Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn).

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_2/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_2/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`

Nhiệm vụ:
- Kiểm tra tính toàn vẹn của CSDL và đối chiếu không bỏ sót bất kỳ thuộc tính màu nào.
- Kiểm tra Migration, Snapshot và tính an toàn của `context.Database.CanConnect()` trong `Program.cs`.
- Kiểm tra các view người dùng và admin edit.
- Xác nhận `Areas/CuaHang/` tuyệt đối không bị tác động.
- Chạy lệnh build và test độc lập.
- Viết `handoff.md` kết luận rõ APPROVE hoặc REQUEST_CHANGES kèm bằng chứng chi tiết, và gửi tin nhắn báo cáo cho Orchestrator.

