# Dispatch Task for M1 Explorer 3 (SessionHelpers, DbInitializer & Area Views Setup)

## Objective
Analyze and specify exact implementation details for:
1. `Areas/CuaHang/Helpers/SessionExtensions.cs`: Generic `Get<T>` and `Set<T>` for ASP.NET Core `ISession` using `System.Text.Json`.
2. `Areas/CuaHang/Helpers/DbInitializer.cs`: Robust seed method for >= 3 (or 4) realistic charity products for "Trăng Sáng Langbiang 2026", with error handling / fallback if SQL Server is not reachable in demo mode.
3. `Areas/CuaHang/Views/_ViewImports.cshtml`: Tag helpers, namespaces for models and viewmodels.
4. `Areas/CuaHang/Views/_ViewStart.cshtml`: Automatic layout selection (Admin views use `_AdminLayout`, storefront views use `_Layout`).

## Input Files
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

## Output
Write detailed design to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3/handoff.md`.


## 2026-11-03T12:43:45Z
You are teamwork_explorer_m1_3.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3/DISPATCH.md
4. Update your progress.md regularly with "Last visited: [timestamp]" as your heartbeat.
5. Provide exact implementation designs for SessionExtensions.cs, DbInitializer.cs (Seed Data logic with offline fallback), and Areas/CuaHang/Views/_ViewImports.cshtml & _ViewStart.cshtml.
6. Write your report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3/handoff.md following the Handoff Protocol.
7. When finished, send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).
