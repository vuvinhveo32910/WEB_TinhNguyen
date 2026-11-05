# Task Dispatch: Worker Remediation - Terminal Cancellation, Store Isolation & Feature 18 Isolation

## Role
Teamwork Worker (`teamwork_worker`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_remediation`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`
- Reviewer 2 Handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2/handoff.md`
- Challenger 1 Handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_1/handoff.md`

## Mandatory Integrity Warning
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Write Ownership (Exclusively Owned Files)
You are permitted to modify ONLY the following files:
1. `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`
2. `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs`
3. `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs`
4. `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs`

## Remediation Tasks

### 1. Fix Terminal State for Cancelled Orders (`DaHuy`) & Restock Safeguard
- In `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs` (`CapNhatTrangThai`):
  - If `string.Equals(order.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)` and the target status is NOT `"DaHuy"`:
    - Do NOT allow reactivation.
    - Set `TempData["ErrorMessage"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy.";`
    - If AJAX, return JSON `{ success = false, message = "Không thể thay đổi trạng thái của đơn hàng đã hủy." }`.
    - Redirect to `ChiTiet(id)` or `Index()`.
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (`UpdateInMemoryOrderStatus`):
  - If `string.Equals(order.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)` and `!string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)`:
    - Return `false` (cannot reactivate cancelled order).

### 2. Defensive Cloning in `AddInMemoryOrder`
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (lines 378–380):
  - Replace `_inMemoryOrders.Add(order);` with `_inMemoryOrders.Add(CloneOrder(order));`.
  - Ensure the cloned order in `_inMemoryOrders` has the assigned `IDDonHang` and `NgayDatHang`, and returns a deep clone to the caller so the internal store instance cannot be mutated outside of `lock (_syncLock)`.

### 3. Guarantee Default Sample Orders on Store Reset
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (`ResetInMemoryStore()`):
  - Inside the lock, after resetting IDs and clearing collections:
    `SeedSampleOrders();`
    `_sampleOrdersSeeded = true;`
    So sample orders are immediately restored every time the store is reset.

### 4. Inventory Restock on Order Deletion
- In `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs` (`XoaDonHang`):
  - Before deleting `order`, if `order.TrangThaiDonHang != "DaHuy"` and `order.ChiTietDonHangs != null`:
    - Restore product inventory: `prod.SoLuongTon += detail.SoLuong; prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);`
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (`DeleteInMemoryOrder`):
  - Before removing `order`, if `order.TrangThaiDonHang != "DaHuy"` and `order.ChiTietDonHangs != null`:
    - Restore in-memory product stock.

### 5. Exclude Cancelled Orders from Revenue & Charity Funds
- In `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs` (`Index`):
  - Calculate `TongDoanhThu` and `TongTienQuy` only from active orders:
    ```csharp
    var activeOrders = allOrders.Where(o => !string.Equals(o.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)).ToList();
    ViewBag.TongDoanhThu = activeOrders.Sum(o => o.TongThanhToan);
    ViewBag.TongTienQuy = activeOrders.Sum(o => o.TienUngHoThem + (o.ChiTietDonHangs?.Sum(c => c.TienDongGop) ?? 0m));
    ```

### 6. Fix Feature 18 Test Isolation in `CH_M4_01` & `F18_T12`
- In `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs`:
  - In `CH_M4_01`:
    * Add a sample line item to `ChiTietDonHangs` for each generated order, OR call `DbInitializer.ResetInMemoryStore()` at the end of `CH_M4_01`.
  - In `CH_M4_12_Adv`, `CH_M4_13_Adv`, and `CH_M4_16_Adv` (the adversarial harness tests added by Challenger 1):
    * Update `CH_M4_12_Adv` to assert that mutating local reference does NOT affect the store (because `AddInMemoryOrder` now clones).
    * Update `CH_M4_13_Adv` to assert that attempting to transition a cancelled order back to active fails (because `DaHuy` is terminal), preventing phantom restock.
    * Update `CH_M4_16_Adv` to assert that `ResetInMemoryStore()` preserves sample orders.
- In `tests/E2ETests/Tier1/Tier1_AdminOrderTests.cs`:
  - In `F18_T12_DbInitializerGetInMemoryOrdersReturnsList`:
    * Ensure clean test isolation by calling `DbInitializer.ResetInMemoryStore();` or asserting `orders.Any(o => o.ChiTietDonHangs != null && o.ChiTietDonHangs.Count > 0)`.

## Verification Commands Required
Before delivering handoff, you MUST run:
1. Solution build:
   `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
   Must pass with 0 errors, 0 warnings.
2. Isolated Feature 18 tests:
   `bash tests/run_e2e_tests.sh --feature 18`
   Must pass 100% with exit code 0.
3. Full E2E test suite:
   `bash tests/run_e2e_tests.sh --all`
   Must pass 100% with exit code 0.

Write your report to `handoff.md` in your working directory.
