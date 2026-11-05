# DISPATCH for Explorer 2 (C# .NET 10 Developer - MVC Architecture & Routing)

**Role**: C# MVC Architecture Explorer (Lập trình viên C# - .NET 10 MVC Structure & Routing)
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md

## Mission & Scope
Investigate codebase for Requirement R2 (Việt hóa cấu trúc thư mục và cấu hình MVC):
1. Examine the current folder structure: `Controllers/`, `Models/`, `Views/`, `Data/` (and check `Areas/CuaHang/`).
2. Plan renaming to standard Vietnamese names: `Controllers` -> `BoDieuKhien`, `Models` -> `MoHinh`, `Views` -> `GiaoDien`, `Data` -> `DuLieu`.
3. Check project target framework (.NET 10 / current target in `.csproj`), compilation settings, assembly attributes.
4. Detail how to configure Razor ViewEngine (e.g. `IViewLocationExpander` or `RazorViewEngineOptions.ViewLocationFormats` / `AreaViewLocationFormats`) so that views inside `GiaoDien/` (and areas if applicable) resolve correctly without breaking MVC resolution.
5. Detail how to configure controller routing / discovery in `Program.cs` so controllers in `BoDieuKhien/` resolve correctly.
6. Check all namespace impacts (`WEB_TinhNguyen.Controllers`, `WEB_TinhNguyen.Models`, `WEB_TinhNguyen.Data`), `_ViewImports.cshtml`, `_ViewStart.cshtml`, layout references, and view models.
7. Detail how `Areas/CuaHang/` interacts with this renaming (preserving 100% functionality and isolation).
8. Write findings and technical blueprint to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2/handoff.md`.


## 2026-11-04T02:21:52Z
You are C# MVC Architecture Explorer (Lập trình viên C# - .NET 10 MVC Structure & Routing).
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2
Read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before doing anything else.
Also read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2/DISPATCH.md.

Scope & Objective:
Conduct an authoritative codebase investigation for Requirement R2 (Việt hóa cấu trúc thư mục và cấu hình MVC):
1. Examine the current folder structure: `Controllers/`, `Models/`, `Views/`, `Data/` (and check `Areas/CuaHang/`).
2. Plan renaming to standard Vietnamese names:
   - `Controllers` -> `BoDieuKhien`
   - `Models` -> `MoHinh`
   - `Views` -> `GiaoDien`
   - `Data` -> `DuLieu`
3. Check the target framework (.NET 10 / current target in `.csproj`), compilation settings, and references.
4. Detail how to configure Razor ViewEngine (e.g., custom `IViewLocationExpander` or `RazorViewEngineOptions.ViewLocationFormats` / `AreaViewLocationFormats`) so that views inside `GiaoDien/` resolve correctly without breaking MVC resolution.
5. Detail how to configure controller routing / discovery in `Program.cs` so controllers in `BoDieuKhien/` resolve correctly.
6. Check all namespace impacts (`WEB_TinhNguyen.Controllers`, `WEB_TinhNguyen.Models`, `WEB_TinhNguyen.Data`), `_ViewImports.cshtml`, `_ViewStart.cshtml`, layout references, and view models.
7. Detail how `Areas/CuaHang/` interacts with this renaming (ensuring 100% functionality and isolation are maintained).
8. Verify how `dotnet build` interacts with these changes and note any potential compilation traps.

Update progress.md in your working directory with a "Last visited: [timestamp]" header after meaningful steps.
When finished, write your comprehensive report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2/handoff.md and send a completion message back.
