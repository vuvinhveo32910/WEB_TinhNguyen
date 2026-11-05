# BRIEFING — 2026-11-04T02:22:30Z

## Mission
Investigate and architect the Vietnamese renaming of MVC structure (Controllers -> BoDieuKhien, Models -> MoHinh, Views -> GiaoDien, Data -> DuLieu), Razor ViewEngine custom view locations, controller routing/discovery, namespace transitions, and Areas/CuaHang/ isolation for .NET Core / .NET 10.

## 🔒 My Identity
- Archetype: explorer
- Roles: C# MVC Architecture Explorer (Lập trình viên C# - .NET 10 MVC Structure & Routing)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2
- Original parent: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Milestone: Survey & Investigation (R2 MVC Structure & Routing)

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Must analyze folder structure renaming (Controllers -> BoDieuKhien, Models -> MoHinh, Views -> GiaoDien, Data -> DuLieu)
- Must determine exact Razor ViewEngine configuration and routing options
- Must inspect namespace impacts, views, layouts, and Areas/CuaHang/ isolation
- Maintain 100% buildability and functionality

## Current Parent
- Conversation ID: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Updated: not yet

## Investigation State
- **Explored paths**: `WEB_TinhNguyen.csproj`, `Program.cs`, `Controllers/` (9 files), `Models/` (3 files), `Views/` (29 files), `Data/` (1 file), `Areas/CuaHang/` (13 files), `Migrations/` (27 files), `tailwind.config.js`, `package.json`.
- **Key findings**:
  1. Target Framework is `net10.0` running with SDK `10.0.114`.
  2. In Linux sandbox, `dotnet build` fails without `XDG_RUNTIME_DIR=/tmp` due to missing `/run/user/1000`. With this variable set, build succeeds in 7.8s.
  3. Renaming directories: `Controllers` -> `BoDieuKhien` (9 files), `Models` -> `MoHinh` (3 files), `Views` -> `GiaoDien` (29 files across 10 folders), `Data` -> `DuLieu` (1 file).
  4. Controller discovery in ASP.NET Core relies on `*Controller` suffix / `ControllerFeatureProvider`, not folder path. URL routing `{controller=Home}` will continue resolving `HomeController` located in `BoDieuKhien/`.
  5. Razor ViewEngine requires custom `ViewLocationFormats` / `IViewLocationExpander` because it queries physical/virtual paths: `/GiaoDien/{1}/{0}.cshtml`, `/GiaoDien/Shared/{0}.cshtml`, plus `/GiaoDien/Shared/{0}.cshtml` fallback for Area views.
  6. 22 view files contain hardcoded string paths `Layout = "~/Views/Shared/_AdminLayout.cshtml";` and `Layout = "~/Views/Shared/_Layout.cshtml";` (including 7 files inside `Areas/CuaHang/Views/`). These must be updated to `~/GiaoDien/Shared/...` to prevent runtime 500 exceptions.
  7. `tailwind.config.js` content array includes `./Views/**/*.cshtml` and `./WEB_TinhNguyen/Views/**/*.cshtml`. It MUST be updated to include `./WEB_TinhNguyen/GiaoDien/**/*.cshtml` or Tailwind build will skip classes in GiaoDien.
  8. `Areas/CuaHang` must maintain its internal folder structure (`Controllers/`, `Models/`, `Views/`, `Helpers/`) for 100% preservation per R3, but its layout references and model/data usings must be updated to ensure seamless operation.
  9. Namespace modernization (`WEB_TinhNguyen.BoDieuKhien`, `MoHinh`, `DuLieu`) can be cleanly supported with global usings and type-aliasing compatibility for EF migrations.
- **Unexplored areas**: None, full codebase surveyed for R2.

## Key Decisions Made
- Use both `RazorViewEngineOptions.ViewLocationFormats` / `AreaViewLocationFormats` configuration AND a dedicated `VietnameseViewLocationExpander` class for maximum robustness.
- Preserve `Areas/CuaHang` folder structure while linking its layouts to `~/GiaoDien/Shared/`.
- Provide complete code snippets for `Program.cs`, `VietnameseViewLocationExpander.cs`, `_ViewImports.cshtml`, `_ViewStart.cshtml`, and `tailwind.config.js`.

## Artifact Index
- DISPATCH.md — Dispatch instructions
- BRIEFING.md — Working memory and context
- progress.md — Heartbeat and status
- handoff.md — Comprehensive technical blueprint
