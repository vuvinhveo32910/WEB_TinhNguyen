# DISPATCH for Explorer 1 (Database Architect - EF Core)

**Role**: Database Architect Explorer (Kiến trúc sư Cơ sở dữ liệu - EF Core)
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md

## Mission & Scope
Investigate codebase for Requirement R1 (Tái cấu trúc CSDL và dọn dẹp mã nguồn):
1. Locate all entity models, DbContext classes, and configurations across the project.
2. Identify all properties/fields storing text color and background color (e.g. `MauChu`, `MauNen`, etc.).
3. Check all existing Migrations, DB initialization (`DbInitializer.cs`), and how EF Core is configured in `Program.cs`.
4. Enumerate all tables and columns, and determine exact Vietnamese naming scheme.
5. Identify all LINQ/SQL queries across controllers, views, services that interact with these tables and columns.
6. Provide concrete recommendations for Code First Migration and LINQ/SQL updates.
7. Write findings and technical blueprint to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md`.

## 2026-11-04T02:21:44Z
You are Database Architect Explorer (Kiến trúc sư Cơ sở dữ liệu - EF Core).
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1
Read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before doing anything else.
Also read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/DISPATCH.md.

Scope & Objective:
Conduct an authoritative codebase investigation for Requirement R1 (Tái cấu trúc CSDL và dọn dẹp mã nguồn):
1. Examine all Models, DbContext (`ApplicationDbContext.cs`), and entity mappings.
2. Find all properties and columns storing text colors, background colors (e.g. `MauChu`, `MauNen`, etc.).
3. Check existing EF Core migrations, SQL Server configuration / connection strings, and fallback/mock mechanisms (`DbInitializer.cs`).
4. Enumerate all tables and columns in the database and models that must be renamed to realistic Vietnamese names.
5. Search all LINQ and SQL queries across controllers, views, and helpers that query these tables/columns.
6. Provide an exact, concrete step-by-step strategy for:
   - Removing color properties from Models and DbContext.
   - Creating and executing EF Core Code First Migration to drop color columns.
   - Renaming all tables and columns to Vietnamese in DbContext/Models and SQL.
   - Updating all LINQ/SQL queries to match.
   - Updating DbInitializer and any seed/test data.

Update progress.md in your working directory with a "Last visited: [timestamp]" header after meaningful steps.
When finished, write your comprehensive report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md and send a completion message back.
