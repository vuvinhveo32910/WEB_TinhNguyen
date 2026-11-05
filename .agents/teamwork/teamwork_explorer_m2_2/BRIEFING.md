# BRIEFING — 2026-11-03T13:30:00Z

## Mission
Design and provide exact Razor views code for Admin Product Management (Index.cshtml, Create.cshtml, Edit.cshtml) matching _AdminLayout.cshtml styling and SanPhamCrudViewModel / SanPhamGayQuy entity structure.

## 🔒 My Identity
- Archetype: Teamwork explorer
- Roles: Read-only investigation, Razor views design & synthesis
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: M2 - Admin Product Management

## 🔒 Key Constraints
- Read-only investigation — do NOT directly modify source code unless directed; explorer prepares analysis, design, and exact proposed view templates in handoff.
- Strict adherence to _AdminLayout styling, Tailwind/CSS classes, FontAwesome icons, responsive design.
- Accurate model bindings matching SanPhamGayQuy and SanPhamCrudViewModel.

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T13:30:00Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` (Dark navy sidebar `#161a2d`, emerald highlights `#4caf50`, Be Vietnam Pro typography, topbar)
  - `WEB_TinhNguyen/Views/AdminTinTuc/Index.cshtml`, `Edit.cshtml`, and `AdminChuongTrinh/Index.cshtml` (form patterns, table structure, badges)
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs` & `SanPhamCrudViewModel.cs` (properties, annotations, conversions)
  - `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewImports.cshtml` & `_ViewStart.cshtml` (imports & automatic layout selection)
  - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (in-memory demo fallback, seed data, image fallback paths)
- **Key findings**:
  - `Index.cshtml` needs `IEnumerable<SanPhamGayQuy>` with full responsive table, badges, stat cards, instant client filter, and delete POST form with confirmation.
  - `Create.cshtml` & `Edit.cshtml` need `SanPhamCrudViewModel` with dual image input (`FileHinhAnh` upload & `LinkHinhAnh` URL) with live JS preview, range slider for fundraising %, and full validation.
  - Baseline project compiles cleanly with 0 errors via `XDG_RUNTIME_DIR=/tmp dotnet build`.
- **Unexplored areas**:
  - Storefront views (Milestone M3).

## Key Decisions Made
- Generated 3 standalone proposed templates in working directory (`proposed_Index.cshtml`, `proposed_Create.cshtml`, `proposed_Edit.cshtml`).
- Verified all tag helpers (`asp-for`, `asp-validation-for`, `asp-action`, `asp-area`, `@Html.AntiForgeryToken()`) match ASP.NET Core conventions.
- Included complete 5-component handoff report.

## Artifact Index
- DISPATCH.md — Task assignment from orchestrator
- proposed_Index.cshtml — Proposed Razor code for AdminSanPham/Index.cshtml
- proposed_Create.cshtml — Proposed Razor code for AdminSanPham/Create.cshtml
- proposed_Edit.cshtml — Proposed Razor code for AdminSanPham/Edit.cshtml
- handoff.md — Final handoff report containing complete Razor view codes and documentation
