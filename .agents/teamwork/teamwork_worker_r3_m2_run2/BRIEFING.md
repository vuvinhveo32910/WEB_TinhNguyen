# BRIEFING — 2026-11-04T06:50:00Z

## Mission
Thực hiện Milestone M2: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn (Requirement R1) cho dự án WEB_TinhNguyen.

## 🔒 My Identity
- Archetype: Worker (implementer, qa, specialist)
- Roles: C# (.NET 10) Developer & Database Architect (EF Core)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/
- Original parent: 656d28b8-bcd4-4f61-b4e3-dd425076f84e
- Milestone: M2 (Requirement R1)

## 🔒 Key Constraints
- Xóa toàn bộ 66 thuộc tính màu trong MoHinh/Entities.cs và MoHinh/CauHinhTrangChuVM.cs.
- Thuần hóa tiếng Việt cho bảng và cột trong Models, DbContext, Migrations.
- Tạo Code First Migration mới 20261104000000_XoaCotMauVaVietHoaTenBangCot.cs và Designer, cập nhật ApplicationDbContextModelSnapshot.cs.
- Cập nhật LINQ queries trong các controller thuộc BoDieuKhien/.
- Dọn dẹp các form Edit trong GiaoDien/Admin* (xóa input type="color").
- Bảo toàn 100% phân hệ bán hàng Areas/CuaHang/ (KHÔNG ĐƯỢC CHẠM VÀO).
- dotnet build thành công 0 lỗi.
- E2E test suite pass 100%.
- Viết handoff.md đầy đủ và gửi message báo cáo cho Orchestrator.
- DO NOT CHEAT: không hardcode test results, không tạo dummy/facade implementations.

## Current Parent
- Conversation ID: 656d28b8-bcd4-4f61-b4e3-dd425076f84e
- Updated: 2026-11-04T06:50:00Z

## Task Summary
- **What to build**: Xóa 66 cột màu, thuần hóa tiếng Việt tên bảng/cột CSDL, tạo EF Core migration & snapshot, sửa LINQ queries trong BoDieuKhien, xóa input color trong GiaoDien/Admin*, cập nhật user-facing views theo theme tĩnh, bảo toàn 100% Areas/CuaHang.
- **Success criteria**: dotnet build 0 error, 316/316 E2E tests pass 100%, 0 trường màu còn tồn tại trong MoHinh/Entities.cs và GiaoDien/.
- **Interface contracts**: DuLieu.ApplicationDbContext ↔ BoDieuKhien, Compatibility aliases, Areas/CuaHang cô lập.
- **Code layout**: Pure Vietnamese naming (BoDieuKhien, DuLieu, GiaoDien, MoHinh).

## Change Tracker
- **Files modified**:
  - `MoHinh/Entities.cs`: Xóa toàn bộ 66 thuộc tính màu, Việt hóa tên bảng/cột CSDL.
  - `MoHinh/CauHinhTrangChuVM.cs`: Xóa toàn bộ thuộc tính màu, mapping Việt hóa.
  - `DuLieu/ApplicationDbContext.cs`: Cập nhật DbSets Việt hóa và cấu hình ModelBuilder.
  - `DuLieu/Compatibility.cs`: Alias hỗ trợ tương thích ngược.
  - `Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` & `.Designer.cs`: Tạo migration mới DropColumn & Rename.
  - `Migrations/ApplicationDbContextModelSnapshot.cs`: Snapshot đồng bộ với CSDL thuần Việt.
  - `BoDieuKhien/*Controller.cs`: Cập nhật 7 controller theo LINQ queries mới.
  - `GiaoDien/Admin*/Edit.cshtml`: Xóa toàn bộ `input[type="color"]` và mã màu HEX trong cả 6 trang edit admin.
  - `GiaoDien/Home/*.cshtml` & `_Layout.cshtml`: Thay thế các biến màu động bằng màu theme chuẩn tĩnh.
- **Build status**: PASS (0 Error, 41 Warnings).
- **Pending issues**: NONE.

## Quality Status
- **Build/test result**: PASS — 316/316 tests passed (100%).
- **Lint status**: PASS — 0 compilation errors.
- **Tests added/modified**: E2E test suite chạy toàn vẹn không suy giảm (Tier 1..5).

## Key Decisions Made
- Dùng `defaultPalette` và static theme CSS chuẩn (#2e7d32, #1b5e20, #4caf50, #dcfce7, #eef8ea) thay thế hoàn toàn hệ thống màu động phức tạp gây lỗi render.
- Áp dụng `RenameTable` và `RenameColumn` cùng `DropColumn` trong EF Core migration để bảo đảm tính liên tục và tương thích dữ liệu.
- Giữ nguyên vẹn 100% `Areas/CuaHang/`.

## Artifact Index
- `handoff.md` — Báo cáo nghiệm thu hoàn thành Milestone M2
- `progress.md` — Nhật ký tiến độ chi tiết
