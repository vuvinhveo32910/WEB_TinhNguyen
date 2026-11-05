# BRIEFING — 2026-11-03T12:37:00Z

## Mission
Investigate ASP.NET Core Areas, Admin controllers/views/layouts/styles, routing configuration, and authentication/authorization requirements for fundraising product shop and management.

## 🔒 My Identity
- Archetype: explorer
- Roles: survey, investigation, synthesis
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_survey_2
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: Survey & Architectural Analysis

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Strictly read-only on project source code
- Files for content delivery, messages for coordination

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T12:30:34Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/Program.cs` & `Startup` (only Program.cs exists, .NET 10 minimal API, only default route)
  - `WEB_TinhNguyen/Controllers/Admin*Controller.cs` (7 Admin controllers, none in an Area, no [Authorize])
  - `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` & `_Layout.cshtml` (Layout structures, sidebar navigation, top navigation)
  - `WEB_TinhNguyen/Views/Admin*` (Admin UI views, styling, scripts, modals)
  - `WEB_TinhNguyen/Models/Entities.cs` & `Data/ApplicationDbContext.cs` (Data models, DbSets, SQL schema)
  - `tailwind.config.js`, `package.json`, `WEB_TinhNguyen.csproj` (Build targets, CSS build, CDN Tailwind)
- **Key findings**:
  1. No existing ASP.NET Core Areas exist. All existing controllers reside in root `Controllers/`.
  2. Program.cs has only `name: "default"` route; Area routing is absent and must be mapped via `MapControllerRoute` with `{area:exists}` before default route.
  3. No Authentication/Authorization configured in Program.cs (`[Authorize]` is absent across entire project; project runs in open demo mode).
  4. Admin views share `Views/Shared/_AdminLayout.cshtml` which uses Tailwind CDN, FontAwesome 6, custom sidebar (hover-expandable 85px -> 280px).
  5. Recommended Area structure: `Areas/CuaHang/` (or `Areas/Shop/`) containing User Storefront (SanPham, GioHang), Admin Management (AdminSanPham), Models, and Views, integrating seamlessly with `_Layout.cshtml` and `_AdminLayout.cshtml`.
- **Unexplored areas**: None within the survey scope.

## Key Decisions Made
- Fully documented 5 core dispatch questions with file citations, line numbers, code snippets, and clear architectural blueprint for the implementer.

## Artifact Index
- DISPATCH.md — Task dispatch instructions
- progress.md — Heartbeat and activity log
- handoff.md — Comprehensive survey report
