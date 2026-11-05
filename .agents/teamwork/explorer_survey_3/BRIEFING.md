# BRIEFING — 2026-11-05T06:55:00Z

## Mission
Investigate controllers, services, repositories, viewmodels, views, raw SQL/LINQ queries, build status, and startup endpoints referencing the 8 invalid columns.

## 🔒 My Identity
- Archetype: explorer
- Roles: Queries Controllers and Build Investigator
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_3
- Original parent: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Milestone: milestone_survey

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Write only to working directory (/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_3/)
- Do not modify source code or tests outside working directory

## Current Parent
- Conversation ID: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Updated: not yet

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/BoDieuKhien/` (All Controllers)
  - `WEB_TinhNguyen/MoHinh/` (Entities.cs, CauHinhTrangChuVM.cs)
  - `WEB_TinhNguyen/GiaoDien/` and `WEB_TinhNguyen/Areas/CuaHang/Views/` (All Razor .cshtml views)
  - `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`
  - `WEB_TinhNguyen/Program.cs`
  - `WEB_TinhNguyen/Migrations/`
  - `tests/` (Test suite and test runners)
- **Key findings**:
  1. No raw SQL queries (FromSqlRaw, ExecuteSqlRaw, Dapper, ADO.NET) exist in project code.
  2. The 8 columns (DuongDanAnhBia, DuongDanAnhLinhVat, HienThiHieuUng3D, HienThiLinhVat, KieuChuNoiDung, KieuChuTieuDe, VeDuAn_DuongDanAnh, ViTriLinhVat) originated from migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`.
  3. The actual database schema retains original column names: LinkAnhBia, LinkAnhMascot, HienThiCanvas3D, HienThiMascot, FontNoiDung, FontTieuDe, VeDuAn_LinkAnh, ViTriMascot.
  4. Razor views (.cshtml) ALREADY use the original column/alias names (LinkAnhBia, FontTieuDe, etc.) and contain ZERO references to the 8 invalid names.
  5. Current `dotnet build` succeeds (0 errors, 4 warnings) when provided with `XDG_RUNTIME_DIR=/tmp`. Full E2E test suite (337 tests) currently passes in memory.
  6. Mapped properties in `Entities.cs` cause EF Core to generate SELECT queries containing the 8 invalid columns on endpoints `/`, `/Home/ChuongTrinh`, `/Home/GayQuy`, `/Home/BanToChuc`, `/Home/TinTuc`, `/Home/KyUc`, and in Admin controllers, resulting in `SqlException: Invalid column name`.
- **Unexplored areas**: None. Complete inventory and call graph established.

## Key Decisions Made
- Confirmed full inventory of references and mapped out zero-risk remapping/NotMapped strategy.
- Documented complete 5-component handoff report.

## Artifact Index
- DISPATCH.md — Received dispatch instructions
- progress.md — Liveness and task tracking
- BRIEFING.md — Situational awareness
- handoff.md — Final 5-component report
