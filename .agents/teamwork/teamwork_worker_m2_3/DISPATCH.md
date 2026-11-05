# Task Dispatch: Worker M2.3 - Test Expansion & Verification Harness

## Role
Teamwork Worker (`teamwork_worker`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_3`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`
- Testing Survey Report: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_3/handoff.md`

## Mandatory Integrity Warning
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Write Ownership (Exclusively Owned Files)
You are permitted to modify or create ONLY the following files:
1. `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs` (Create new)
2. `tests/E2ETests/Framework/TestRunner.cs`
3. `tests/E2ETests/Tier2/Tier2_BoundaryCornerTests.cs`
4. `tests/E2ETests/Tier3/Tier3_CrossFeatureTests.cs`
5. `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs` (Create new)

Do NOT modify any web application source files (`WEB_TinhNguyen/`).

## Implementation Tasks

### 1. Fix `tests/E2ETests/Framework/TestRunner.cs`
- At line 219, change `for (int t = 1; t <= 4; t++)` to `for (int t = 1; t <= 5; t++)` so Tier 5 is included in the execution summary breakdown table.

### 2. Implement `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs`
- Create new test class `Tier1_AdminOrderTests` with 15 tests decorated with `[E2ETest(Tier = 1, Feature = "F18", ...)]`:
  - `F18_T01`: `AdminDonHangController` class exists in `WEB_TinhNguyen.Areas.CuaHang.Controllers` and has `[Area("CuaHang")]`.
  - `F18_T02`: `_AdminLayout.cshtml` contains link to `/CuaHang/AdminDonHang` or `asp-controller="AdminDonHang"`.
  - `F18_T03`: `_AdminLayout.cshtml` order link displays "Đơn hàng Gây Quỹ" or "Đơn hàng" with icon `fa-receipt` or `fa-clipboard-list`.
  - `F18_T04`: `Admin/Index.cshtml` contains quick link / action card for "Đơn hàng Gây Quỹ".
  - `F18_T05`: `AdminDonHang/Index.cshtml` exists and renders table headers for orders (Mã đơn, Người nhận, SĐT, Tổng tiền, Trạng thái, Thao tác).
  - `F18_T06`: `AdminDonHang/Index.cshtml` contains 4 KPI cards for Total Orders, Pending Orders, Revenue, and Charity funds.
  - `F18_T07`: `AdminDonHang/ChiTiet.cshtml` exists and displays donor contact, delivery address, order items breakdown, and charity details.
  - `F18_T08`: `AdminDonHangController.Index` action method exists and returns View with orders model.
  - `F18_T09`: `AdminDonHangController.ChiTiet` action method exists and retrieves order by ID.
  - `F18_T10`: `AdminDonHangController.CapNhatTrangThai` action method exists with `[HttpPost]`.
  - `F18_T11`: `AdminDonHangController.XoaDonHang` action method exists with `[HttpPost]`.
  - `F18_T12`: `DbInitializer.GetInMemoryOrders()` returns list of current orders safely.
  - `F18_T13`: `DbInitializer.UpdateInMemoryOrderStatus(id, status, paymentStatus)` updates status and timestamp.
  - `F18_T14`: `DbInitializer.DeleteInMemoryOrder(id)` removes order safely.
  - `F18_T15`: `AdminDonHang/ChiTiet.cshtml` displays VietQR transfer instructions and bank account `0924044283`.

### 3. Expand `Tier2_BoundaryCornerTests.cs` & `Tier3_CrossFeatureTests.cs`
- In `Tier2_BoundaryCornerTests.cs`:
  - Add `B26_Storefront_Search_Unicode`: Search by Vietnamese keyword ("áo thun") matches products.
  - Add `B27_Storefront_Out_Of_Stock_Badge`: Product with stock 0 renders "Hết hàng" and disables button.
  - Add `B28_Admin_Order_Filter_Invalid_Status`: Invalid status filter handles gracefully without throwing exceptions.
- In `Tier3_CrossFeatureTests.cs`:
  - Add `C17_Checkout_Reduces_Stock_And_Increases_DaBan`: Verifies order placement decrements `SoLuongTon` and increments `DaBan`.
  - Add `C18_Admin_Cancel_Order_Restores_Stock`: Transitioning order to `DaHuy` restores product inventory and decrements `DaBan`.

### 4. Implement `Tier5_ChallengerM4OrderHarness.cs`
- Create `Tier5_ChallengerM4OrderHarness` with 10 adversarial stress and concurrency tests:
  - `CH_M4_01` to `CH_M4_10`:
    * Concurrency stress test: 20 parallel order updates in `DbInitializer` remain thread-safe.
    * Concurrency stress test: parallel status updates and restock checks do not produce race conditions or negative stock.
    * Rapid transition stress: multiple rapid status changes (`MoiDat` -> `DaXacNhan` -> `DangGiao` -> `HoanThanh` -> `DaHuy`) calculate final inventory accurately without duplicate restock.
    * Idempotence test: calling `UpdateInMemoryOrderStatus` with `DaHuy` twice does not restock twice.
    * Cloned order protection: modifying an order object returned by `GetInMemoryOrderById` does not mutate internal storage.
    * Floating cart button markup and SweetAlert2 presence validation in `_Layout.cshtml`.

## Verification Commands Required
Before delivering handoff, you MUST run:
1. Solution build:
   `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
   Must pass with 0 errors, 0 warnings.
2. Full test suite:
   `bash tests/run_e2e_tests.sh --all`
   Must pass ALL tests (310+ tests) with 0 failures, 0 skipped.

Write your report to `handoff.md` in your working directory.


## 2026-11-04T01:03:27Z
You are teamwork_worker_m2_3.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_3
Authoritative user request record: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Your task dispatch instructions are in: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_3/DISPATCH.md

You MUST read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before starting work.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Your objective:
Implement Milestone M2.3: Test Expansion & Verification Harness.
1. Update `tests/E2ETests/Framework/TestRunner.cs` so summary iterates `t <= 5`.
2. Create `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs` with 15 tests for Feature 18.
3. Expand `Tier2_BoundaryCornerTests.cs` (B26-B28) and `Tier3_CrossFeatureTests.cs` (C17-C18).
4. Create `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs` with 10 adversarial stress & concurrency tests.
5. Verify build succeeds (0 errors, 0 warnings) and all 310+ tests pass with 100% success rate.
6. Write your report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_3/handoff.md`.

Send a completion message back to the orchestrator when finished.
