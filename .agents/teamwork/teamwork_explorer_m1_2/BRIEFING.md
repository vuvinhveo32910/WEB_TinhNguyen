# BRIEFING — 2026-11-03T12:50:00Z

## Mission
Investigate and produce exact implementation designs and code specifications for SanPhamGayQuy.cs, DonHangGayQuy.cs, ChiTietDonHangGayQuy.cs, and ViewModels (CartItem, CartViewModel, CheckoutViewModel, SanPhamCrudViewModel) for Milestone 1.

## 🔒 My Identity
- Archetype: teamwork_explorer
- Roles: explorer, investigator, synthesizer
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: M1 (Area & Data Foundation)

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Provide exact implementation designs for SanPhamGayQuy.cs, DonHangGayQuy.cs, ChiTietDonHangGayQuy.cs, and ViewModels (CartItem, CartViewModel, CheckoutViewModel, SanPhamCrudViewModel)
- Output only in .agents/teamwork/teamwork_explorer_m1_2/
- Follow Handoff Protocol (handoff.md)

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: not yet

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/Models/Entities.cs`
  - `WEB_TinhNguyen/Models/CauHinhTrangChuVM.cs`
  - `WEB_TinhNguyen/Data/ApplicationDbContext.cs`
  - `WEB_TinhNguyen/Controllers/AdminTinTucController.cs`
  - `.agents/teamwork/ORIGINAL_REQUEST.md`
  - `.agents/teamwork/orchestrator/PROJECT.md`
  - `.agents/teamwork/teamwork_explorer_survey_1/handoff.md`
  - `.agents/teamwork/teamwork_explorer_survey_2/handoff.md`
  - `.agents/teamwork/teamwork_explorer_survey_3/handoff.md`
  - `.agents/teamwork/teamwork_explorer_m1_1/DISPATCH.md`
  - `.agents/teamwork/teamwork_explorer_m1_2/DISPATCH.md`
  - `.agents/teamwork/teamwork_explorer_m1_3/DISPATCH.md`
- **Key findings**:
  - ASP.NET Core project root is located at `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/`
  - Entities follow EF Core DataAnnotations (`[Table]`, `[Key]`, `[ForeignKey]`, `[Column(TypeName = "decimal(18,2)")]`)
  - Target area models folder: `WEB_TinhNguyen/Areas/CuaHang/Models/`
  - Image upload pattern in codebase uses combination of string URL/path and `IFormFile?`
  - Designed all 7 required files with 100% complete, compile-ready code
- **Unexplored areas**:
  - None. Scope fully covered and specified.

## Key Decisions Made
- All entity and ViewModel classes placed under namespace `WEB_TinhNguyen.Areas.CuaHang.Models`.
- Initialized all non-nullable strings (`= string.Empty;`) and collections (`= new List<...>();`) to avoid CS8618 warnings with `<Nullable>enable</Nullable>`.
- Added `[Column(TypeName = "decimal(18,2)")]` on all entity currency properties to ensure SQL Server decimal precision and suppress EF Core warnings.
- Added bidirectional mapping methods in `SanPhamCrudViewModel` (`FromEntity`, `ToEntity`, `UpdateEntity`) to streamline controller implementation.
- Added snapshot fields in `ChiTietDonHangGayQuy` to freeze item metadata at time of order placement.

## Artifact Index
- `.agents/teamwork/teamwork_explorer_m1_2/BRIEFING.md` — persistent working memory
- `.agents/teamwork/teamwork_explorer_m1_2/progress.md` — heartbeat and progress tracking
- `.agents/teamwork/teamwork_explorer_m1_2/handoff.md` — 5-component handoff report
