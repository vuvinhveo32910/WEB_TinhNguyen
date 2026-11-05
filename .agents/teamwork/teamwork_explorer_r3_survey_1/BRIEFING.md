# BRIEFING — 2026-11-04T02:32:00Z

## Mission
Investigate codebase for Requirement R1 (Tái cấu trúc CSDL và dọn dẹp mã nguồn) including models, color fields, migrations, table/column renaming, queries, and DbInitializer.

## 🔒 My Identity
- Archetype: explorer
- Roles: Database Architect Explorer (Kiến trúc sư Cơ sở dữ liệu - EF Core)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1
- Original parent: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Milestone: survey

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Do not modify source code (only write to working directory)
- Provide exact evidence chain (file paths, lines, verbatim code)
- Produce comprehensive handoff.md adhering to 5-component report format

## Current Parent
- Conversation ID: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Updated: 2026-11-04T02:32:00Z

## Investigation State
- **Explored paths**: `Entities.cs`, `CauHinhTrangChuVM.cs`, `ApplicationDbContext.cs`, `DbInitializer.cs`, `Program.cs`, `Migrations/`, `Controllers/`, `Views/`, `Areas/CuaHang/`
- **Key findings**:
  1. Exactly 66 color properties identified across 11 entities (`MauChu`, `MauNen`, `MauChuDao`, etc.).
  2. Identified English/abbreviated tables/columns needing Vietnamese normalization (`BannerTrangChu` -> `BieuNguTrangChu`, `TVKhoanhKhac` -> `ThuVienKhoanhKhac`, `FAQTrangChu` -> `CauHoiThuongGapTrangChu`, `IDFAQ` -> `IDCauHoi`, `CTA_...` -> `KeuGoi_...`, `Hero_...` -> `NoiBat_...`).
  3. `Areas/CuaHang` contains 0 color fields and is 100% compliant with R3 isolation requirements.
  4. Database offline fallback in `DbInitializer.cs` operates cleanly with in-memory store.
  5. Built migration & query update strategy with exact migration schema and SQL scripts.
- **Unexplored areas**: None for R1.

## Key Decisions Made
- Fully catalogued all 66 color fields and their mapping to ViewModels and Controllers.
- Prepared exact blueprint for EF Core migration, LINQ query updates, and DbInitializer preservation.

## Artifact Index
- `DISPATCH.md` — Dispatch instructions
- `BRIEFING.md` — Persistent working memory
- `progress.md` — Liveness heartbeat
- `handoff.md` — Authoritative technical survey & blueprint report
