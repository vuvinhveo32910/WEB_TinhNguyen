# BRIEFING — 2026-11-04T02:50:00Z

## Mission
Thực hiện Milestone M1 (Yêu cầu R2): Việt hóa cấu trúc thư mục MVC (BoDieuKhien, MoHinh, GiaoDien, DuLieu), cấu hình Razor ViewEngine 2 tầng, cập nhật layout path, namespace, tailwind.config.js và bảo đảm biên dịch 100% thành công không lỗi.

## 🔒 My Identity
- Archetype: implementer / qa / specialist
- Roles: C# .NET 10 Developer (Lập trình viên C# - .NET 10)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1
- Original parent: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Milestone: M1 (Requirement R2)

## 🔒 Key Constraints
- DO NOT CHEAT: All implementations genuine, real state, real behavior.
- Renaming:
  - Controllers -> BoDieuKhien
  - Models -> MoHinh
  - Views -> GiaoDien
  - Data -> DuLieu
- Areas/CuaHang/ PRESERVED 100% UNTOUCHED structurally.
- Compatibility layer for existing EF Core migrations (Data.ApplicationDbContext).
- 2-layer Razor ViewEngine configuration (VietnameseViewLocationExpander + Program.cs ViewLocationFormats).
- Update all 22 hardcoded layout paths ~/Views/Shared/... -> ~/GiaoDien/Shared/...
- Update tailwind.config.js content paths.
- dotnet build with XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp must succeed with 0 errors.

## Current Parent
- Conversation ID: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Updated: 2026-11-04T02:50:00Z

## Task Summary
- **What to build**: Rename MVC directories, update namespaces, create VietnameseViewLocationExpander, configure Razor ViewEngine in Program.cs, update ViewImports and Layout paths, update tailwind.config.js, verify build.
- **Success criteria**: 0 compilation errors, all 22 layout paths updated, ViewEngine finds views in GiaoDien, Areas/CuaHang preserved.
- **Interface contracts**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md § Interface Contracts
- **Code layout**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md § Code Layout

## Key Decisions Made
- Used Linux `mv` to relocate directories (`Controllers`->`BoDieuKhien`, `Models`->`MoHinh`, `Views`->`GiaoDien`, `Data`->`DuLieu`) due to read-only git lock in sandbox environment.
- Configured 2-layer Razor ViewEngine via `VietnameseViewLocationExpander` and static `ViewLocationFormats` / `AreaViewLocationFormats` in `Program.cs`.
- Created `Compatibility.cs` providing `global using` and keeping `WEB_TinhNguyen.Data` namespace in scope for backward compatibility with 14 existing EF Core migration files.
- Replaced all 22 instances of `~/Views/Shared/` with `~/GiaoDien/Shared/` across Views and Areas/CuaHang views.
- Updated `tailwind.config.js` with `./GiaoDien/**/*.cshtml` and `./WEB_TinhNguyen/GiaoDien/**/*.cshtml`.

## Artifact Index
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/DISPATCH.md — Task assignment
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/BRIEFING.md — Persistent context
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/progress.md — Progress tracker and heartbeat
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/handoff.md — Final handoff report

## Change Tracker
- **Files modified**:
  - `WEB_TinhNguyen/Program.cs`: configured 2-layer Razor ViewEngine and VietnameseViewLocationExpander
  - `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs`: new IViewLocationExpander
  - `WEB_TinhNguyen/DuLieu/Compatibility.cs`: global usings and backward compatibility namespace
  - `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`: namespace updated to WEB_TinhNguyen.DuLieu
  - `WEB_TinhNguyen/BoDieuKhien/*.cs` (9 files): namespace updated to WEB_TinhNguyen.BoDieuKhien
  - `WEB_TinhNguyen/MoHinh/*.cs` (3 files): namespace updated to WEB_TinhNguyen.MoHinh
  - `WEB_TinhNguyen/GiaoDien/` views: 15 views updated layout path and models
  - `WEB_TinhNguyen/Areas/CuaHang/` views: 7 views updated layout path, _ViewImports updated
  - `WEB_TinhNguyen/Areas/CuaHang/Controllers/*.cs` (4 files): using updated to DuLieu
  - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`: using updated to DuLieu
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`: using updated to MoHinh
  - `tailwind.config.js`: added GiaoDien paths
- **Build status**: PASS (0 Error, Tailwind rebuilt in 2.2s)
- **Pending issues**: None

## Quality Status
- **Build/test result**: PASS (dotnet build succeeded with 0 Error, 2 NuGet vulnerability warnings)
- **Lint status**: PASS
- **Tests added/modified**: Verified compilation and Tailwind execution

## Loaded Skills
- None specified by orchestrator.
