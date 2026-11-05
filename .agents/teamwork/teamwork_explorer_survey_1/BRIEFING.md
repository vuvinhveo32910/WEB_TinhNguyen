# BRIEFING — 2026-11-03T12:41:00Z

## Mission
Survey project structure, ASP.NET Core version, DbContext & database setup, models, and layout/navigation components for volunteer fundraising store.

## 🔒 My Identity
- Archetype: explorer
- Roles: survey, read-only analysis
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_survey_1
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: Survey & Investigation Complete

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Strictly write only to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_survey_1/
- No copying core logic from open-source projects

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T12:41:00Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen.slnx` & `WEB_TinhNguyen/WEB_TinhNguyen.csproj` (Target .NET 10.0, EF Core 10.0.12)
  - `WEB_TinhNguyen/Program.cs` (Route mapping, DbContext registration, static assets)
  - `WEB_TinhNguyen/Data/ApplicationDbContext.cs` & `WEB_TinhNguyen/Models/Entities.cs`
  - `WEB_TinhNguyen/Migrations/` (27 migration files, SQL Server snapshot)
  - `WEB_TinhNguyen/Views/Shared/_Layout.cshtml` & `_AdminLayout.cshtml`
  - `WEB_TinhNguyen/Controllers/` (Home and Admin controllers)
  - `tailwind.config.js` & `package.json`
- **Key findings**:
  - Compilation requires `XDG_RUNTIME_DIR=/tmp dotnet build` due to Linux sandboxed `/run/user/1000` access restriction. Build succeeds with 0 errors.
  - Project uses .NET 10 (`net10.0`) with Tailwind CSS CDN and Tailwind CLI pre-build target.
  - Layout has desktop navbar (lines 373-394 in `_Layout.cshtml`) and mobile liquid bottom navbar (`.navigation lg:hidden` lines 476-516).
  - Admin layout has expandable sidebar with active state routing.
  - No Area exists yet. Implementing Area requires `app.MapControllerRoute` for areas in `Program.cs`.
  - Session is not yet registered; shopping cart needs `builder.Services.AddDistributedMemoryCache(); builder.Services.AddSession();` and `app.UseSession()`.
  - No product/order entities exist yet.
- **Unexplored areas**: None for survey scope.

## Key Decisions Made
- Fully documented compilation environment caveat (`XDG_RUNTIME_DIR=/tmp`) to avoid build failures for downstream agents.
- Formulated concrete Area architecture and model design for fundraising store and admin CRUD.

## Artifact Index
- handoff.md — Comprehensive survey report
- progress.md — Heartbeat and task progress
- BRIEFING.md — Working memory index
