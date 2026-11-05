# Dispatch Task for Worker M1 (Area & Data Foundation Implementation)

## Objective
Implement Milestone 1 (Area & Data Foundation) for the volunteer fundraising store system according to the architectural blueprints provided by Explorers 1, 2, and 3.

## Exclusive Write Ownership
You exclusively own and may create/modify only the following files:
1. `WEB_TinhNguyen/Program.cs`
2. `WEB_TinhNguyen/Data/ApplicationDbContext.cs`
3. `tailwind.config.js`
4. `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`
5. `WEB_TinhNguyen/Areas/CuaHang/Models/DonHangGayQuy.cs`
6. `WEB_TinhNguyen/Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`
7. `WEB_TinhNguyen/Areas/CuaHang/Models/CartItem.cs`
8. `WEB_TinhNguyen/Areas/CuaHang/Models/CartViewModel.cs`
9. `WEB_TinhNguyen/Areas/CuaHang/Models/CheckoutViewModel.cs`
10. `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`
11. `WEB_TinhNguyen/Areas/CuaHang/Helpers/SessionExtensions.cs`
12. `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`
13. `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewImports.cshtml`
14. `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`

## Input Blueprints & Reports
- ORIGINAL_REQUEST.md: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master PROJECT.md: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Explorer 1 Report (Program.cs, ApplicationDbContext.cs, tailwind.config.js): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/handoff.md`
- Explorer 2 Report (Entities & ViewModels): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2/handoff.md`
- Explorer 3 Report (SessionExtensions.cs, DbInitializer.cs, Views setup): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3/handoff.md`

## Verification Requirements
You MUST run the build command:
`XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
and document that it compiles with 0 errors (`Build succeeded. 0 Error(s)`).

## Mandatory Integrity Warning
> DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Completion Output
Write `handoff.md` in `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1/handoff.md` following the Handoff Protocol with verified build results and layout compliance.
Send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.


## 2026-11-03T12:51:38Z
You are teamwork_worker_m1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1/DISPATCH.md
4. Read the 3 Explorer handoff reports for Milestone 1:
   - Explorer 1: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/handoff.md
   - Explorer 2: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2/handoff.md
   - Explorer 3: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_3/handoff.md
5. Update your progress.md regularly with "Last visited: [timestamp]" as your heartbeat.
6. Implement all required files per your exclusive write ownership:
   - Program.cs, ApplicationDbContext.cs, tailwind.config.js
   - Areas/CuaHang/Models/SanPhamGayQuy.cs, DonHangGayQuy.cs, ChiTietDonHangGayQuy.cs, CartItem.cs, CartViewModel.cs, CheckoutViewModel.cs, SanPhamCrudViewModel.cs
   - Areas/CuaHang/Helpers/SessionExtensions.cs, DbInitializer.cs
   - Areas/CuaHang/Views/_ViewImports.cshtml, _ViewStart.cshtml
7. Verify compilation: run `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` and confirm 0 errors.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

8. Write handoff.md following the Handoff Protocol and send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).
