# Empirical Challenge & Stress Verification Report (Round 2)

**Agent**: `teamwork_challenger_r2_1`  
**Role**: Empirical Challenger (`critic`, `specialist`)  
**Working Directory**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_1`  
**Date**: 2026-11-04T01:27:30Z  
**Verdict**: `Verdict: REQUEST_CHANGES (Critical restock loop hole allows phantom inventory generation via repeated order cancellation/reactivation, in-memory reference mutation vulnerability in AddInMemoryOrder, and sample order erasure bug on store reset)`

---

## 1. Observation

### Obs 1.1: Baseline Test Suite Execution
- **Command**: `bash tests/run_e2e_tests.sh --all`
- **Output**:
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
    OVERALL: 310 Tests | 310 Passed | 0 Failed | 0 Skipped in 5.29s
  ================================================================================
  ```

### Obs 1.2: Code Inspection of Order Cancellation & Restock State Transitions
In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`, lines 448–473:
```csharp
448: bool wasNotCancelled = !string.Equals(order.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase);
449: bool isCancelled = string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase);
450: 
451: order.TrangThaiDonHang = trangThaiDonHang;
452: if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
453: {
454:     order.TrangThaiThanhToan = trangThaiThanhToan;
455: }
456: order.NgayCapNhat = DateTime.Now;
457: 
458: // Restock logic: If trangThaiDonHang == "DaHuy", restore the inventory of each product in ChiTietDonHangs
459: if (wasNotCancelled && isCancelled && order.ChiTietDonHangs != null)
460: {
461:     foreach (var detail in order.ChiTietDonHangs)
462:     {
463:         if (detail.IDSanPham.HasValue)
464:         {
465:             var prod = _inMemoryProducts.FirstOrDefault(p => p.IDSanPham == detail.IDSanPham.Value);
466:             if (prod != null)
467:             {
468:                 prod.SoLuongTon += detail.SoLuong;
469:                 prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);
470:             }
471:         }
472:     }
473: }
```
And in `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs`, lines 195–221:
```csharp
195: var oldStatus = order.TrangThaiDonHang;
196: bool wasNotCancelled = !string.Equals(oldStatus, "DaHuy", StringComparison.OrdinalIgnoreCase);
197: bool isCancelled = string.Equals(trangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase);
198: 
199: order.TrangThaiDonHang = trangThaiDonHang;
...
207: if (wasNotCancelled && isCancelled && order.ChiTietDonHangs != null)
208: {
209:     foreach (var detail in order.ChiTietDonHangs)
210:     {
211:         if (detail.IDSanPham.HasValue)
212:         {
213:             var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
214:             if (prod != null)
215:             {
216:                 prod.SoLuongTon += detail.SoLuong;
217:                 prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);
218:             }
219:         }
220:     }
221: }
```

### Obs 1.3: Code Inspection of In-Memory Store Insertion Mutability
In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`, lines 326 vs 379:
- `AddInMemoryProduct`:
  ```csharp
  326: _inMemoryProducts.Add(CloneProduct(item));
  ```
- `AddInMemoryOrder`:
  ```csharp
  379: _inMemoryOrders.Add(order);
  ```
`AddInMemoryProduct` defensively clones `item` prior to insertion into the internal list. However, `AddInMemoryOrder` inserts the caller's instance reference `order` directly without cloning.

### Obs 1.4: Code Inspection of Store Reset and Lazy Seeding Coupling
In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`, lines 265–276 and line 403:
```csharp
265: public static void ResetInMemoryStore()
266: {
267:     lock (_syncLock)
268:     {
269:         _inMemoryProducts.Clear();
270:         _inMemoryProducts.AddRange(GetDefaultProducts());
271:         _inMemoryOrders.Clear();
272:         _nextProductId = 5;
273:         _nextOrderId = 1;
274:         _sampleOrdersSeeded = false;
275:     }
276: }
...
403: if (!_sampleOrdersSeeded && _inMemoryOrders.Count == 0)
404: {
405:     SeedSampleOrders();
406: }
```
When `ResetInMemoryStore()` executes, `_inMemoryOrders` is cleared, and `_sampleOrdersSeeded` is set to `false`. But `SeedSampleOrders()` is not called. If any order is added via `AddInMemoryOrder` before `GetInMemoryOrders` is called, `_inMemoryOrders.Count` becomes 1. The lazy-seed condition `!_sampleOrdersSeeded && _inMemoryOrders.Count == 0` evaluates to `true && false = false`, permanently suppressing sample order seeding.

### Obs 1.5: Code Inspection of Order Deletion
In `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs`, lines 296–300 and `DbInitializer.cs` line 485:
```csharp
297: _context.DonHangGayQuys.Remove(order);
298: await _context.SaveChangesAsync();
```
Deleting an order removes the order entity completely from the database / in-memory store. If the order was in `MoiDat`, `DaXacNhan`, or `DangGiao`, its reserved stock deduction is never refunded, leaving inventory permanently missing with zero surviving audit records.

### Obs 1.6: Code Inspection of Financial KPI Cards in Admin
In `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs`, lines 77–78:
```csharp
77: ViewBag.TongDoanhThu = allOrders.Sum(o => o.TongThanhToan);
78: ViewBag.TongTienQuy = allOrders.Sum(o => o.TienUngHoThem + (o.ChiTietDonHangs?.Sum(c => c.TienDongGop) ?? 0m));
```
`allOrders.Sum(...)` sums across all orders, including cancelled orders (`TrangThaiDonHang == "DaHuy"`).

### Obs 1.7: Empirical Test Harness Results
Six adversarial tests (`CH_M4_11` through `CH_M4_16_Adv`) were added to `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs`.
- `CH_M4_11`: 100-thread concurrent mixed operations (25 adds, 25 reads, 25 updates, 25 product queries) completed in 15.6ms without deadlocks.
- `CH_M4_12_Adv`: Verified that mutating the caller's local reference after `AddInMemoryOrder` directly mutates internal storage:
  `Assert.Equal("CORRUPTED_DONOR_NAME_HACK", fresh.HoTenNguoiMua) -> PASSED (Adversarial vulnerability confirmed).`
- `CH_M4_13_Adv`: Verified that cancelling, reactivating, and cancelling again inflated a 50-stock product to 60 units:
  `Assert.Equal(60, p3?.SoLuongTon) -> PASSED (Phantom inventory generation confirmed).`
- `CH_M4_14`: Safe product deletion during restock handles gracefully.
- `CH_M4_15`: Search case-insensitivity on Vietnamese strings passes.
- `CH_M4_16_Adv`: Verified that calling `ResetInMemoryStore()` followed by `AddInMemoryOrder()` resulted in `orders.Count == 1` with default sample orders completely lost:
  `Assert.Equal(1, orders.Count) -> PASSED (Seed erasure vulnerability confirmed).`
- **Total Suite Execution**: 316 Tests | 316 Passed | 0 Failed in 2.99s.

---

## 2. Logic Chain

1. **Phantom Inventory Generation via Cancellation Loophole**:
   - *From Obs 1.2*: Restock runs whenever `wasNotCancelled && isCancelled`.
   - *From Obs 1.7*: When an order (10 units) for a 50-unit product is placed, stock becomes 40. Cancelling it sets status to `"DaHuy"` and restores stock to 50.
   - When an admin selects `"DaXacNhan"` in `ChiTiet.cshtml` or via API, `wasNotCancelled` is false (`"DaHuy"`), so stock is not deducted (remains 50).
   - When the admin subsequently selects `"DaHuy"` again, `wasNotCancelled` is true (previous status was `"DaXacNhan"`), and `isCancelled` is true. Stock is incremented by 10 again, reaching 60 units.
   - *Conclusion*: A malicious or accidental status toggle creates phantom inventory, enabling overselling of nonexistent physical stock.

2. **In-Memory Store Isolation Bypass**:
   - *From Obs 1.3 & 1.7*: `AddInMemoryOrder` does not clone `order`.
   - Any external code maintaining a reference to the `order` object can modify its customer name, payment amount, or order status outside of `_syncLock`.
   - *Conclusion*: Breaks concurrency invariants and thread safety.

3. **Sample Order Erasure on Store Reset**:
   - *From Obs 1.4 & 1.7*: `ResetInMemoryStore()` clears `_inMemoryOrders` and sets `_sampleOrdersSeeded = false`, but does not re-seed orders.
   - Adding an order immediately increments count to 1, causing the lazy check `!_sampleOrdersSeeded && _inMemoryOrders.Count == 0` to fail permanently.
   - *Conclusion*: Default sample orders ("Nguyễn Thị Mai Anh", "Trần Bảo Long") disappear permanently from the admin dashboard after any new checkout in in-memory mode.

4. **Untracked Inventory Leakage on Order Deletion**:
   - *From Obs 1.5*: Calling `XoaDonHang` deletes the order without checking if status was `"DaHuy"`.
   - Reserved stock previously deducted upon checkout is never restored to `SanPhamGayQuys`.
   - *Conclusion*: Inventory tracking permanently leaks items whenever an uncancelled order is deleted.

5. **Financial KPI Distortions**:
   - *From Obs 1.6*: Cancelled orders are included in `TongDoanhThu` and `TongTienQuy`.
   - *Conclusion*: Revenue and charity funds displayed to administrators are artificially inflated by unfulfilled/cancelled transactions.

---

## 3. Caveats

- Testing was performed using ASP.NET Core 10.0 test harness and EF Core in-memory / fallback store. Live SQL Server cluster network partitions and distributed deadlock hazards were not tested due to offline standalone integrity mode.
- In `GioHangController.DatHang`, pre-checkout stock check is performed before `_context.SaveChangesAsync()` without pessimistic database row locking; under high parallel checkout concurrency for the final unit of stock, overselling could occur in SQL Server if transaction isolation level is not `Serializable`.

---

## 4. Conclusion

**Verdict: REQUEST_CHANGES**

The solution demonstrates robust foundational architecture, clean layout separation, and passes 316 comprehensive tests. However, critical integrity defects exist in the order state machine and in-memory store that must be remediated:

### Required Remediation Actions:
1. **Fix Restock State Machine**:
   - In `DbInitializer.UpdateInMemoryOrderStatus` and `AdminDonHangController.CapNhatTrangThai`:
     * Option A (Recommended): Treat `DaHuy` as a terminal state. If `oldStatus == "DaHuy"`, reject transition to active states (`MoiDat`, `DaXacNhan`, `DangGiao`, `HoanThanh`) and return an error message: `"Không thể chuyển đổi trạng thái đơn hàng đã hủy."`.
     * Option B: If reactivation from `DaHuy` is allowed, re-deduct inventory from `SanPhamGayQuy` (and verify sufficient stock exists).
2. **Defensive Cloning in `AddInMemoryOrder`**:
   - In `DbInitializer.cs:379`, replace `_inMemoryOrders.Add(order);` with `_inMemoryOrders.Add(CloneOrder(order));` to mirror `AddInMemoryProduct`.
3. **Fix Store Reset Default Orders**:
   - In `DbInitializer.cs:265-276` (`ResetInMemoryStore()`), add `SeedSampleOrders();` directly inside the lock so sample orders are guaranteed to be restored on reset.
4. **Restock on Deletion or Require Cancellation First**:
   - In `AdminDonHangController.XoaDonHang` and `DbInitializer.DeleteInMemoryOrder`:
     * If deleting an order whose status is not `"DaHuy"`, automatically restore its items' inventory before removal, OR require the admin to cancel the order prior to deletion.
5. **Filter Cancelled Orders in KPI Cards**:
   - In `AdminDonHangController.Index`:
     ```csharp
     var activeOrders = allOrders.Where(o => !string.Equals(o.TrangThaiDonHang, "DaHuy", StringComparison.OrdinalIgnoreCase)).ToList();
     ViewBag.TongDoanhThu = activeOrders.Sum(o => o.TongThanhToan);
     ViewBag.TongTienQuy = activeOrders.Sum(o => o.TienUngHoThem + (o.ChiTietDonHangs?.Sum(c => c.TienDongGop) ?? 0m));
     ```

---

## 5. Verification Method

To independently verify these findings:
1. Run the test suite:
   ```bash
   bash tests/run_e2e_tests.sh --all
   ```
2. Verify empirical finding tests in `tests/E2ETests/Tier5/Tier5_ChallengerM4OrderHarness.cs`:
   - `CH_M4_11`: 100-thread concurrency test (proves deadlock-free execution under mixed load).
   - `CH_M4_12_Adv`: Reference mutation proof (proves uncloned object reference vulnerability).
   - `CH_M4_13_Adv`: Cyclic cancellation proof (proves stock increases from 50 to 60 on repeated cancellation).
   - `CH_M4_16_Adv`: Lazy seed erasure proof (proves default orders are lost on reset followed by order add).
3. Review code in:
   - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` lines 265–276, 379, 448–473
   - `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs` lines 77–78, 195–221, 296–300
