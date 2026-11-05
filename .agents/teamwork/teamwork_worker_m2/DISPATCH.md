# Dispatch Task for Worker M2 (Admin Product Management Implementation)

## Objective
Implement Milestone 2 (Admin Product Management) by applying the complete blueprints and code provided by M2 Explorers:
1. `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs`:
   Copy / apply from `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/proposed_AdminSanPhamController.cs`.
2. Admin Views in `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/`:
   - `Index.cshtml`: Copy / apply from `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Index.cshtml`.
   - `Create.cshtml`: Copy / apply from `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Create.cshtml`.
   - `Edit.cshtml`: Copy / apply from `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Edit.cshtml`.
3. Navigation in `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` and `WEB_TinhNguyen/Views/Admin/Index.cshtml`:
   Apply from `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_3/proposed_admin_navigation.patch` (or manually add the sidebar item for "Sản phẩm Gây Quỹ" linking to `/CuaHang/AdminSanPham`).

## Exclusive Write Ownership
- `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml`
- `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`
- `WEB_TinhNguyen/Views/Admin/Index.cshtml`

## Verification Requirements
You MUST run:
`XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
and:
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 12`
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 13`
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 14`
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 15`
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 16`
Confirm that all 25 Admin feature tests (Features 12 to 16) PASS!

## Mandatory Integrity Warning
> DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Write `handoff.md` in `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2/handoff.md` and send message to orchestrator.
