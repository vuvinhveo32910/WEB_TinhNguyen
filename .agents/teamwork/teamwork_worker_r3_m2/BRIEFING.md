# BRIEFING — 2026-11-04T03:37:00Z

## Mission
Triển khai Milestone M2 (Yêu cầu R1): Tái cấu trúc CSDL và dọn dẹp mã nguồn. Xóa 66 thuộc tính màu, thuần hóa tiếng Việt tên bảng và cột trong Models & DbContext, tạo EF Core Migration và Snapshot, tự động Migrate trong Program.cs, cập nhật câu truy vấn LINQ/Controllers, dọn dẹp form Edit trong Admin Views, bảo toàn 100% Cửa hàng và kiểm tra biên dịch 0 lỗi.

## 🔒 My Identity
- Archetype: worker
- Roles: implementer, qa, specialist
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2
- Original parent: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Milestone: M2 (Tái cấu trúc CSDL và dọn dẹp mã nguồn - Requirement R1)

## 🔒 Key Constraints
- Xóa toàn bộ 66 thuộc tính màu trong Models và ViewModels.
- Chuẩn hóa tên bảng và cột sang tiếng Việt chuẩn mực (BieuNguTrangChu, ThuVienKhoanhKhac, CauHoiThuongGapTrangChu, DuongDanAnh, IDCauHoi, IDBieuNgu, KeuGoi_..., NoiBat_...).
- Tạo EF Core Code First Migration 20261104000000_XoaCotMauVaVietHoaTenBangCot.cs + Designer và cập nhật ModelSnapshot.
- Cập nhật Program.cs gọi context.Database.Migrate() khi context.Database.CanConnect() == true.
- Cập nhật LINQ queries và xóa gán màu trong tất cả controllers thuộc BoDieuKhien/.
- Dọn dẹp form Edit trong GiaoDien/Admin* (xóa thẻ <input type="color"> và hex input).
- Bảo toàn tuyệt đối 100% cấu trúc và code của Areas/CuaHang/.
- Đảm bảo dotnet build cho cả 2 project (WEB_TinhNguyen & E2ETests) thành công 0 lỗi.

## Current Parent
- Conversation ID: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Updated: 2026-11-04T03:37:00Z

## Task Summary
- **What to build**: Xóa 66 cột màu, đổi tên bảng/cột tiếng Việt, Migration EF Core mới, cập nhật Controllers/LINQ, làm sạch View Admin.
- **Success criteria**: dotnet build 0 error, test passes, không còn thuộc tính màu, CuaHang 100% nguyên vẹn.
- **Interface contracts**: DuLieu.ApplicationDbContext ↔ BoDieuKhien, Areas/CuaHang bảo toàn.
- **Code layout**: MoHinh/, DuLieu/, BoDieuKhien/, GiaoDien/, Migrations/.

## Key Decisions Made
- Tuân thủ thiết kế chi tiết từ Explorer 1 Blueprint (teamwork_explorer_r3_survey_1/handoff.md).
- Migration Up() thực hiện DropColumn cho 66 cột màu, RenameTable và RenameColumn cho các bảng/cột mới.
- Down() phục hồi ngược lại các RenameTable.

## Artifact Index
- DISPATCH.md — Chỉ thị nhiệm vụ chi tiết từ Orchestrator
- BRIEFING.md — Trạng thái làm việc hiện tại
- progress.md — Nhật ký tiến độ và heartbeat liveness
- handoff.md — Báo cáo nghiệm thu hoàn tất Milestone M2

## Change Tracker
- **Files modified**: Chưa thay đổi
- **Build status**: Initial build PASS (0 errors)
- **Pending issues**: Chưa bắt đầu thực thi

## Quality Status
- **Build/test result**: Initial build clean
- **Lint status**: 0 warnings/errors (ngoài NuGet cache warning)
- **Tests added/modified**: Sẽ kiểm thử E2E sau khi hoàn thành

## Loaded Skills
- Không yêu cầu external skill đặc thù.
