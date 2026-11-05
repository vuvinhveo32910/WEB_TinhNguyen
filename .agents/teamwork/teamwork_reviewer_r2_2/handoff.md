# Acceptance Criteria Audit & Adversarial Review Report

**Agent**: `teamwork_reviewer_r2_2`  
**Role**: Reviewer & Adversarial Critic  
**Working Directory**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2`  
**Date**: 2026-11-04  
**Verdict**: `REQUEST_CHANGES (Test isolation state leakage in Feature 18 causes 'bash tests/run_e2e_tests.sh --feature 18' to fail with exit code 1)`

---

## 1. Observation

### 1.1. Solution Compilation
Command executed:
```bash
export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp
dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal
```
Output:
```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:17.01
```

### 1.2. Full E2E Test Suite Execution
Command executed:
```bash
bash tests/run_e2e_tests.sh --all
```
Output:
```text
================================================================================
                              TEST EXECUTION SUMMARY
================================================================================
  Tier 1: Total: 100 | Passed: 100 | Failed:   0 | Skipped:   0
  Tier 2: Total:  28 | Passed:  28 | Failed:   0 | Skipped:   0
  Tier 3: Total:  18 | Passed:  18 | Failed:   0 | Skipped:   0
  Tier 4: Total:  12 | Passed:  12 | Failed:   0 | Skipped:   0
  Tier 5: Total: 152 | Passed: 152 | Failed:   0 | Skipped:   0
--------------------------------------------------------------------------------
  OVERALL: 310 Tests | 310 Passed | 0 Failed | 0 Skipped in 4.56s
================================================================================
```

### 1.3. Individual Tier Runs
- `bash tests/run_e2e_tests.sh --tier 1` -> Total: 100 | Passed: 100 | Failed: 0
- `bash tests/run_e2e_tests.sh --tier 2` -> Total: 28  | Passed: 28  | Failed: 0
- `bash tests/run_e2e_tests.sh --tier 3` -> Total: 18  | Passed: 18  | Failed: 0
- `bash tests/run_e2e_tests.sh --tier 4` -> Total: 12  | Passed: 12  | Failed: 0
- `bash tests/run_e2e_tests.sh --tier 5` -> Total: 152 | Passed: 152 | Failed: 0

### 1.4. Isolated Feature 18 Verification Run Failure
Command recommended by Worker M2.3 in `teamwork_worker_m2_3/handoff.md` (Section 5, Item 3: *"Verify Specific Milestone M2.3 Test Tiers: bash tests/run_e2e_tests.sh --feature 18, Expected: 25 tests for Feature 18 pass with 100% success rate"*):
```bash
bash tests/run_e2e_tests.sh --feature 18
```
Verbatim execution result:
```text
Command exited with code 1.

================================================================================
                              TEST EXECUTION SUMMARY
================================================================================
  Tier 1: Total:  15 | Passed:  14 | Failed:   1 | Skipped:   0
  Tier 5: Total:  10 | Passed:  10 | Failed:   0 | Skipped:   0
--------------------------------------------------------------------------------
  OVERALL: 25 Tests | 24 Passed | 1 Failed | 0 Skipped in 0.26s
================================================================================

Failed Test Details:
  [FAIL] Tier 1 | F18 | F18_T12: DbInitializer.GetInMemoryOrders returns list of orders safely
         Reason: Assert.True Failed: Orders must have associated item details
```

### 1.5. Root Cause Code Inspection
1. In `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs` (lines 28–42):
   ```csharp
   for (int i = 0; i < 20; i++)
   {
       var ord = new DonHangGayQuy
       {
           HoTenNguoiMua = $"Parallel Donor #{i}",
           SoDienThoai = $"09000000{i:D2}",
           DiaChiNhanHang = "Khuôn viên HUTECH",
           TongThanhToan = 50000m + (i * 1000m),
           PhuongThucThanhToan = "COD",
           TrangThaiDonHang = "MoiDat",
           TrangThaiThanhToan = "ChuaThanhToan"
           // NOTE: ChiTietDonHangs is NOT initialized or populated!
       };
       var added = DbInitializer.AddInMemoryOrder(ord);
       orderIds.Add(added.IDDonHang);
   }
   ```
   `CH_M4_01` injects 20 orders without `ChiTietDonHangs` into the static `DbInitializer._inMemoryOrders` list.

2. In `tests/E2ETests/Framework/TestRunner.cs`:
   The test runner iterates through discovered tests by Tier descending (`Tier 5` runs before `Tier 1`).
   When filtered by `--feature 18`, Tier 5 Feature 18 tests execute first, populating `_inMemoryOrders` with orders that have `ChiTietDonHangs == null`.

3. In `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs` (lines 180–187):
   ```csharp
   [E2ETest(1, 18, "F18_T12: DbInitializer.GetInMemoryOrders returns list of orders safely")]
   public void F18_T12_DbInitializerGetInMemoryOrdersReturnsList()
   {
       var orders = DbInitializer.GetInMemoryOrders();
       Assert.NotNull(orders, "DbInitializer.GetInMemoryOrders() must not return null");
       Assert.True(orders.Count > 0, "DbInitializer must provide sample seed orders");
       Assert.NotNull(orders[0].MaDonHang, "Orders must have non-null MaDonHang");
       Assert.True(orders[0].ChiTietDonHangs != null && orders[0].ChiTietDonHangs.Count > 0, "Orders must have associated item details");
   }
   ```
   Because `GetInMemoryOrders()` sorts by `NgayDatHang DESC`, `orders[0]` is one of the newly created dummy orders from `CH_M4_01`, which has no line items, causing `Assert.True(orders[0].ChiTietDonHangs != null && orders[0].ChiTietDonHangs.Count > 0)` to fail.

4. Why `--all` masked the failure:
   In `--all`, Tier 5 Feature 18 tests ran, but subsequent Tier 5 tests in other files (e.g. `Tier5_ChallengerM3_2Harness.cs` line 830, or `Tier5_ChallengerM1Harness.cs`) called `DbInitializer.ResetInMemoryStore()`, which wiped out the 20 polluted orders and re-seeded the 2 default sample orders before Tier 1 executed. Thus, `--all` passing was an accidental artifact of test execution ordering.

### 1.6. Requirement & Acceptance Criteria Inspection
- **R1 (Storefront & Navigation)**:
  - `_Layout.cshtml` contains "Cửa hàng Gây quỹ" immediately adjacent to "Trang chủ" (lines 381–382).
  - Floating Cart button (`#floatingCartBtn`) and Cart Badge (`#cartCountBadge`) implemented with live item count.
  - SweetAlert2 CDN and Toast alerts integrated.
  - `Areas/CuaHang/Views/CuaHang/Index.cshtml` has live search (`#liveSearchInput`), quick price pills, responsive card grid, and charity mission badges.
  - `Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml` has AJAX add to cart and instant checkout ("Mua ngay").
  - `Areas/CuaHang/Views/GioHang/Index.cshtml` has AJAX quantity adjustments (`stepQuantityAjax`), modern Tailwind modals (`#clearCartModal`, `#deleteSingleItemModal`), and anti-double-click spinner on order placement.
  - `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml` provides 1-click clipboard copy for MBBank `0924044283` and Order Code, visual 4-step order progress tracker, and printable receipt button (`window.print()`).
  - **Status**: PASSED.

- **R2 (Admin Management)**:
  - `_AdminLayout.cshtml` links both `AdminSanPham` (`fa-boxes`) and `AdminDonHang` (`fa-receipt`).
  - `Views/Admin/Index.cshtml` has quick dashboard action tiles for both.
  - `Areas/CuaHang/Controllers/AdminSanPhamController.cs` provides full CRUD for products with image upload and safe deletion.
  - `Areas/CuaHang/Controllers/AdminDonHangController.cs` provides listing, filtering by status/payment, searching by code/name/phone/email, sorting, 4 KPI stats cards (`TongDonHang`, `ChoXacNhan`, `TongDoanhThu`, `TongTienQuy`), detail view with VietQR, status update with automatic stock restocking when transitioned to `DaHuy`, and safe deletion.
  - **Status**: PASSED.

- **R3 (Architecture & Seed Data)**:
  - All fundraising code is strictly encapsulated in `Areas/CuaHang/` (`Controllers/`, `Views/`, `Models/`, `Helpers/`).
  - `DbInitializer.cs` provides 4 authentic Langbiang 2026 charity products and 2 sample orders.
  - Solution compiles with 0 errors and 0 warnings.
  - **Status**: PASSED.

---

## 2. Logic Chain

1. **Acceptance Criteria Verification**:
   - The application functionality (R1, R2, R3, Follow-up enhancements) meets all requirements of `ORIGINAL_REQUEST.md`.
   - The Razor views, controllers, models, and in-memory dual-mode fallbacks are genuinely implemented with zero dummy/facade implementations or external shortcuts.

2. **Test Suite Integrity & Isolation Analysis**:
   - Tests must run deterministically and independently.
   - Running tests in isolated subsets (such as by feature: `bash tests/run_e2e_tests.sh --feature 18`) should succeed if the feature is cleanly implemented and properly tested.
   - Worker M2.3 explicitly documented in their handoff report that running `bash tests/run_e2e_tests.sh --feature 18` was expected to produce 25 passed tests with 100% success rate.
   - Independent verification revealed that executing this command fails with exit code 1 due to `F18_T12` failing.
   - The failure is caused by state pollution: `Tier5_ChallengerM4OrderHarness.CH_M4_01` adds 20 orders without `ChiTietDonHangs` to the static `DbInitializer._inMemoryOrders` list without cleanup, which poisons `F18_T12` when Tier 1 runs directly afterwards.
   - This represents an unverified claim / state leakage in the test expansion milestone.

---

## 3. Caveats

- In production environments with a live SQL Server instance, EF Core operates against the physical database. In offline / test environments, `DbInitializer` handles in-memory persistence.
- The failure does NOT occur when running all 310 tests (`--all`) or Tier 1 alone (`--tier 1`), but only when running Feature 18 in isolation (`--feature 18`) or if Tier 5 Feature 18 runs directly before `F18_T12` without an intervening `DbInitializer.ResetInMemoryStore()`.

---

## 4. Conclusion

**Verdict: REQUEST_CHANGES**

### Critical Finding (INTEGRITY / TEST ISOLATION): Feature 18 Test Isolation State Contamination
- **What**: Running `bash tests/run_e2e_tests.sh --feature 18` fails with exit code 1 (24 Passed, 1 Failed):
  `[FAIL] Tier 1 | F18 | F18_T12: DbInitializer.GetInMemoryOrders returns list of orders safely`
  `Reason: Assert.True Failed: Orders must have associated item details`
  This contradicts the verification claim in Worker M2.3's handoff report (Section 5, item 3).
- **Where**:
  - `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs` (lines 28–42, `CH_M4_01`)
  - `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs` (lines 180–187, `F18_T12`)
- **Why**: Static in-memory store `DbInitializer._inMemoryOrders` is polluted by `CH_M4_01` with orders missing `ChiTietDonHangs`. Because `TestRunner` executes Tier 5 before Tier 1, `F18_T12` queries the top order (`orders[0]`), finds null `ChiTietDonHangs`, and fails.
- **Required Fix**:
  1. In `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs`:
     - In `CH_M4_01`, populate valid `ChiTietDonHangs` on the 20 generated orders, or call `DbInitializer.ResetInMemoryStore()` at the end of the test.
  2. In `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs`:
     - In `F18_T12`, call `DbInitializer.ResetInMemoryStore();` at the beginning of the test or assert that `orders.Any(o => o.ChiTietDonHangs != null && o.ChiTietDonHangs.Count > 0)` rather than assuming `orders[0]` was not modified by prior concurrent test runs.

---

## 5. Verification Method

To reproduce and independently verify the failure and the solution:

1. **Reproduce the Feature 18 Failure**:
   ```bash
   bash tests/run_e2e_tests.sh --feature 18
   ```
   *Actual Result*: Exit code 1, `F18_T12` fails (`Assert.True Failed: Orders must have associated item details`).

2. **Verify Full Suite Execution**:
   ```bash
   bash tests/run_e2e_tests.sh --all
   ```
   *Actual Result*: Exit code 0, 310 Tests | 310 Passed | 0 Failed.

3. **Verify Web Application Clean Build**:
   ```bash
   export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp
   dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal
   ```
   *Actual Result*: Build succeeded with 0 Warning(s) and 0 Error(s).
