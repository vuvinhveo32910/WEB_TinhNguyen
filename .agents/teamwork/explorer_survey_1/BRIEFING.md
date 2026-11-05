# BRIEFING — 2026-11-05T06:58:30Z

## Mission
Investigate database schema, appsettings, DbContext, and SQL scripts to determine tables and columns involved with the 8 invalid column errors.

## 🔒 My Identity
- Archetype: explorer
- Roles: Database and Schema Researcher
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_1
- Original parent: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Milestone: Database Schema & Entity Discrepancy Survey

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Write only to working directory (/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_1/)
- Never write or modify source code

## Current Parent
- Conversation ID: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Updated: 2026-11-05T06:55:52Z

## Investigation State
- **Explored paths**:
  - `SQLQuery1.sql` (Baseline DB schema)
  - `WEB_TinhNguyen/Migrations/` (All 14 EF Core migrations including `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`)
  - `WEB_TinhNguyen/MoHinh/Entities.cs` (C# Entity definitions)
  - `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs` (DbContext model & mapping)
  - `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml`, `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`, `BoDieuKhien/HomeController.cs` (LINQ query entry points)
  - `tests/` test suites (`E2ETests`, `M1Challenger`, `M3Challenger2`)
- **Key findings**:
  - All 8 invalid column names (`DuongDanAnhBia`, `DuongDanAnhLinhVat`, `HienThiHieuUng3D`, `HienThiLinhVat`, `KieuChuNoiDung`, `KieuChuTieuDe`, `VeDuAn_DuongDanAnh`, `ViTriLinhVat`) belong to table `CauHinhTrangChu`.
  - The actual columns existing in the database are: `LinkAnhBia`, `LinkAnhMascot`, `HienThiCanvas3D`, `HienThiMascot`, `FontNoiDung`, `FontTieuDe`, `VeDuAn_LinkAnh`, `ViTriMascot`.
  - Migration `20261104000000_XoaCotMauVaVietHoaTenBangCot` was written to rename them, but was never applied to the SQL database.
  - In `Entities.cs`, the actual DB columns were marked `[NotMapped]` and new column names were mapped without `[Column]`, causing EF Core to emit `SqlException: Invalid column name`.
- **Unexplored areas**: None within the survey scope.

## Key Decisions Made
- Completed root cause analysis and mapped the 8 invalid columns to existing DB table and column names.
- Documented mapping matrix and fix recommendations in handoff.md.

## Artifact Index
- DISPATCH.md — Incoming dispatch messages
- BRIEFING.md — Persistent context & state
- progress.md — Liveness heartbeat & task progress
- handoff.md — Final 5-component handoff report
