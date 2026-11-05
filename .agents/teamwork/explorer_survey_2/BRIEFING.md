# BRIEFING — 2026-11-05T06:54:30Z

## Mission
Analyze C# Models, DbContext configurations, entity mapping, and attributes ([Column], [NotMapped], Fluent API) related to the 8 invalid columns (DuongDanAnhBia, DuongDanAnhLinhVat, HienThiHieuUng3D, HienThiLinhVat, KieuChuNoiDung, KieuChuTieuDe, VeDuAn_DuongDanAnh, ViTriLinhVat), and provide architectural/technical synchronization strategies.

## 🔒 My Identity
- Archetype: Explorer
- Roles: Models and DbContext Analyst
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2
- Original parent: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Milestone: Survey & Analysis

## 🔒 Key Constraints
- Read-only investigation — do NOT implement / do NOT modify source code
- Write only to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/
- Produce structured 5-component handoff.md and maintain progress.md

## Current Parent
- Conversation ID: 13941881-5ddb-4ef5-85c6-23bd414f7662
- Updated: 2026-11-05T06:54:30Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/MoHinh/Entities.cs`
  - `WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`
  - `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`
  - `WEB_TinhNguyen/DuLieu/Compatibility.cs`
  - `WEB_TinhNguyen/BoDieuKhien/` (HomeController, AdminTrangChuController, AdminKyUcController, AdminChuongTrinhController, AdminGayQuyController, AdminBanToChucController, AdminTinTucController)
  - `WEB_TinhNguyen/GiaoDien/` (Home/Index, Shared/_Layout, etc.)
  - `WEB_TinhNguyen/Migrations/` (20261104000000_XoaCotMauVaVietHoaTenBangCot, ApplicationDbContextModelSnapshot)
  - `SQLQuery1.sql`
  - `tests/E2ETests/`
- **Key findings**:
  - All 8 properties are declared in `CauHinhTrangChu` as unannotated public auto-properties.
  - Several properties are also declared across `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `CauHinhBanToChuc`, and `CauHinhTinTuc`.
  - Zero Fluent API configuration exists in `ApplicationDbContext.OnModelCreating` for these entities; EF Core uses convention and reflects properties to SQL columns matching their C# names.
  - The previous migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` renamed columns in C# but was not applied to the actual database schema.
  - Zero Razor Views consume the 8 renamed names; all views use the legacy aliases (`LinkAnhBia`, `FontTieuDe`, etc.).
  - Recommended strategy: Apply `[NotMapped]` to all 8 properties across all affected entities, supplemented by Fluent API `.Ignore()` in `ApplicationDbContext.OnModelCreating`, and remove them from `ApplicationDbContextModelSnapshot.cs`.
- **Unexplored areas**: None within the survey scope for Models and DbContext.

## Key Decisions Made
- Confirmed that `[NotMapped]` + Fluent API `.Ignore()` is vastly superior to complete property deletion because it prevents `SqlException: Invalid column name` with zero cascading compilation breakage across controllers and ViewModels.

## Artifact Index
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/DISPATCH.md — Incoming parent dispatches
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/BRIEFING.md — Working memory & state
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/progress.md — Liveness & status tracking
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/explorer_survey_2/handoff.md — Final 5-component report
