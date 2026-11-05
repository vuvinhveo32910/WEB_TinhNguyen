# Handoff Report: Backend, Domain Logic, Data Persistence & Models Audit

- **Agent**: `teamwork_explorer_r2_survey_1`
- **Working Directory**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1`
- **Date**: 2026-11-04T00:33:00Z
- **Scope**: Backend, domain logic, data persistence, and models in `Areas/CuaHang/` and `Data/ApplicationDbContext.cs`.

---

## 1. Observation

### Obs 1.1: Order Saving and Inventory Tracking in `GioHangController.cs`
- In `WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs`, lines 321–356 (`DatHang` action):
  ```csharp
  int orderId = 0;
  try
  {
      if (_context.Database.CanConnect())
      {
          _context.DonHangGayQuys.Add(donHang);

          // Trừ tồn kho và tăng số lượng đã bán của sản phẩm trong cơ sở dữ liệu
          foreach (var detail in donHang.ChiTietDonHangs)
          {
              if (detail.IDSanPham.HasValue)
              {
                  var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
                  if (prod != null)
                  {
                      prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong);
                      prod.DaBan += detail.SoLuong;
                  }
              }
          }

          await _context.SaveChangesAsync();
          orderId = donHang.IDDonHang;
      }
      else
      {
          var saved = DbInitializer.AddInMemoryOrder(donHang);
          orderId = saved.IDDonHang;
      }
  }
  catch
  {
      var saved = DbInitializer.AddInMemoryOrder(donHang);
      orderId = saved.IDDonHang;
  }
  ```
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`, lines 243–256 (`AddInMemoryOrder`):
  ```csharp
  // Cập nhật số lượng tồn kho và đã bán tương ứng
  if (order.ChiTietDonHangs != null)
  {
      foreach (var detail in order.ChiTietDonHangs)
      {
          var prod = _inMemoryProducts.FirstOrDefault(p => p.IDSanPham == detail.IDSanPham);
          if (prod != null)
          {
              prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong);
              prod.DaBan += detail.SoLuong;
          }
      }
  }
  ```
- **Finding**: Orders (`DonHangGayQuy` and child items `ChiTietDonHangGayQuy`) ARE saved to SQL Server DB if available, and fallback to `DbInitializer.AddInMemoryOrder` if offline. Both paths deduct `SoLuongTon` using `Math.Max(0, prod.SoLuongTon - detail.SoLuong)` and increment `DaBan += detail.SoLuong`.
- **Deficiency**: `DatHang` does NOT perform a pre-check on inventory before creating the order. If requested quantity exceeds remaining stock, it silently clamps `SoLuongTon` to 0 and allows overselling.

### Obs 1.2: Cart Quantity Manipulation Corner Cases
- In `WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs`, lines 180–184 (`CapNhatSoLuong` action):
  ```csharp
  item.SoLuong = item.SoLuongTon > 0 ? Math.Min(soLuong, item.SoLuongTon) : soLuong;
  ```
- **Finding**: If `item.SoLuongTon` is 0 (or negative), the condition `item.SoLuongTon > 0` evaluates to false, causing `item.SoLuong = soLuong`. A user can arbitrarily increase quantity for an out-of-stock item via AJAX / form POST.

### Obs 1.3: Absence of Admin Order Management
- Scanning controllers in `WEB_TinhNguyen/Areas/CuaHang/Controllers/`:
  - `AdminSanPhamController.cs` (Handles CRUD for `SanPhamGayQuy`)
  - `CuaHangController.cs` (Handles Catalog & Detail)
  - `GioHangController.cs` (Handles Cart, Checkout, Order Confirmation)
  - Result: **NO `AdminDonHangController.cs` exists.**
- Scanning views in `WEB_TinhNguyen/Areas/CuaHang/Views/`:
  - `AdminSanPham/` (`Create.cshtml`, `Edit.cshtml`, `Index.cshtml`)
  - `CuaHang/` (`ChiTiet.cshtml`, `Index.cshtml`)
  - `GioHang/` (`DatHangThanhCong.cshtml`, `Index.cshtml`)
  - Result: **NO `Areas/CuaHang/Views/AdminDonHang/` directory or order views exist.**
- In `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`, lines 267–271:
  Only `/CuaHang/AdminSanPham` is linked in the Admin sidebar navigation. There is NO link or route for managing customer orders.
- In `WEB_TinhNguyen/Views/Admin/Index.cshtml`, lines 118–126:
  Only a quick-link tile for `Sản phẩm Gây Quỹ` (`/CuaHang/AdminSanPham`) is present.

### Obs 1.4: In-Memory Store API Asymmetry in `DbInitializer.cs`
- In `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`, lines 231–276:
  - Methods present: `AddInMemoryOrder(DonHangGayQuy order)`, `GetInMemoryOrderById(int id)`, `GetInMemoryOrderByCode(string code)`.
  - Methods missing:
    - No `GetInMemoryOrders()` -> Cannot fetch order list for Admin.
    - No `UpdateInMemoryOrderStatus(int id, string trangThaiDonHang, string? trangThaiThanhToan)` -> Cannot update order status in fallback mode.
    - No `DeleteInMemoryOrder(int id)` -> Cannot remove test orders.
    - No restock logic when an order is cancelled or rejected.

### Obs 1.5: Schema & Donation vs Revenue Tracking Capabilities
- In `WEB_TinhNguyen/Areas/CuaHang/Models/DonHangGayQuy.cs`:
  - `MaDonHang` (string, max 50)
  - `HoTenNguoiMua` (string, max 255)
  - `SoDienThoai` (string, max 20)
  - `Email` (string?, max 255)
  - `DiaChiNhanHang` (string, max 500)
  - `GhiChu` (string?, max 1000)
  - `TongTienHang` (decimal, money from products)
  - `TienUngHoThem` (decimal, voluntary extra donation)
  - `PhiVanChuyen` (decimal, shipping)
  - `TongThanhToan` (decimal: `TongTienHang + TienUngHoThem + PhiVanChuyen`)
  - `PhuongThucThanhToan` (string, max 50: "ChuyenKhoanQR" or "COD")
  - `TrangThaiDonHang` (string, max 50: "MoiDat", "DaXacNhan", "DangGiao", "HoanThanh", "DaHuy")
  - `TrangThaiThanhToan` (string, max 50: "ChuaThanhToan", "DaThanhToan")
  - `NgayDatHang` (DateTime)
  - `NgayCapNhat` (DateTime?)
- In `WEB_TinhNguyen/Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`:
  - `DonGia` (decimal), `SoLuong` (int), `ThanhTien` (decimal)
  - `PhanTramDongGop` (int, 0–100%)
  - `TienDongGop => ThanhTien * PhanTramDongGop / 100m` (NotMapped)
- In `WEB_TinhNguyen/Data/ApplicationDbContext.cs`:
  All 3 models (`SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`) are declared as `DbSet` with decimal precision `(18,2)` and configured foreign keys (Cascade delete on DonHang -> ChiTietDonHang; Restrict on SanPham -> ChiTietDonHang).
- **Finding**: The database schema and entities possess all necessary numerical fields to track charity contributions separately from merchandise sales revenue:
  - Product sales revenue = `TongTienHang`
  - Charity portion from products = $\sum (\text{detail.ThanhTien} \times \text{detail.PhanTramDongGop} / 100)$
  - Additional donation = `TienUngHoThem`
  - **Total Charity Fund Contribution** = (Charity portion from products) + `TienUngHoThem`
  - **Total Gross Revenue** = `TongThanhToan`
- **Deficiency**: There is currently NO Admin view, Controller action, or Dashboard card aggregating or visualizing these figures.

### Obs 1.6: Database Migrations State
- Inspection of `WEB_TinhNguyen/Migrations/`:
  - 26 migration files exist. The newest migration is `20261014042058_AddMascot.cs`.
  - `grep_search` for `SanPhamGayQuy` in `ApplicationDbContextModelSnapshot.cs` returns no matches.
  - **Finding**: EF Core migrations were never generated for `Areas/CuaHang` tables. The codebase currently relies on `DbInitializer` and try/catch fallback to operate.

---

## 2. Logic Chain

1. **Premise 1 (Order Lifecycle)**: A customer order must be received, verified, fulfilled, and accounted for.
   - *From Obs 1.1 & 1.5*: The customer side creates orders in `GioHangController.DatHang`, stores them in DB or `DbInitializer`, and displays `DatHangThanhCong` with VietQR transfer information.
   - *From Obs 1.3*: However, there is no corresponding controller (`AdminDonHangController`) or view for administrators. Therefore, once an order is created, no administrator can see it, verify the bank transfer, approve the order, mark it as shipped, or cancel it.
2. **Premise 2 (Inventory & Concurrency Integrity)**:
   - *From Obs 1.1*: Stock deduction happens blindly via `Math.Max(0, prod.SoLuongTon - detail.SoLuong)` without verifying stock availability at checkout time.
   - *From Obs 1.2*: In `CapNhatSoLuong`, if stock is 0, quantity clamps are bypassed.
   - *From Obs 1.4*: In-memory store does not have status change or cancellation logic, meaning if an order is cancelled, stock is not returned.
3. **Premise 3 (Donation vs Commercial Accounting)**:
   - *From Obs 1.5*: The data model supports both charity contribution calculations and total revenue calculations.
   - *From Obs 1.3*: Because there is no Admin order management or reporting interface, stakeholders have no way to audit the total funds raised for the charity mission versus merchandise costs/revenue.
4. **Premise 4 (System Robustness & Persistence)**:
   - *From Obs 1.6*: While the fallback mechanism prevents runtime crashes during demo/offline operation, missing EF Core migrations means a clean deployment to a fresh SQL Server instance requires either running a migration or `context.Database.EnsureCreated()`.

---

## 3. Caveats

1. **Frontend / Razor View Scope**: This audit focused strictly on backend, domain logic, data models, and persistence. Specific styling/CSS layout audits of the storefront are covered in companion explorer reports.
2. **Authentication / Authorization**: Admin authentication in this project is currently lightweight (basic session / route-based in standard controllers, no ASP.NET Identity authentication cookie enforced on `AdminSanPhamController`). Any new `AdminDonHangController` should align with the current architecture pattern.
3. **Payment Gateway Integration**: VietQR is currently handled via dynamic image generation (`img.vietqr.io`) with Napas247 memo, which is lightweight and standard for volunteer fundraising without needing complex bank webhooks.

---

## 4. Conclusion & Actionable Recommendations

### Answers to Assigned Questions:
1. **Are orders saved to DB / in-memory fallback? What happens to stock and sold count?**
   - **Yes**, orders are saved to `DonHangGayQuys` in DB, or fallback to `DbInitializer.AddInMemoryOrder`.
   - In both cases, `prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong)` and `prod.DaBan += detail.SoLuong`.
   - **Gap**: There is no pre-check for stock exhaustion at checkout time.
2. **Is there an Admin Order Management controller / view?**
   - **No**. Neither `AdminDonHangController` nor any order management views currently exist. Administrators cannot view, filter, update, or cancel orders.
3. **What fields exist on `DonHangGayQuy`? Can admin view total funds raised vs total product revenue?**
   - Fields include: `IDDonHang`, `MaDonHang`, `HoTenNguoiMua`, `SoDienThoai`, `Email`, `DiaChiNhanHang`, `GhiChu`, `TongTienHang`, `TienUngHoThem`, `PhiVanChuyen`, `TongThanhToan`, `PhuongThucThanhToan`, `TrangThaiDonHang`, `TrangThaiThanhToan`, `NgayDatHang`, `NgayCapNhat`, and `ChiTietDonHangs`.
   - The fields allow exact calculation of funds raised vs merchandise revenue, but **currently admin cannot view this** because no admin order management UI or summary dashboard exists.
4. **What error handling or validation improvements are needed?**
   - Pre-checkout stock check in `GioHangController.DatHang` to reject orders if `SoLuong > SoLuongTon`.
   - Fix bug in `GioHangController.CapNhatSoLuong` when `SoLuongTon <= 0`.
   - Restock logic (`SoLuongTon += detail.SoLuong`, `DaBan -= detail.SoLuong`) when an order is cancelled (`DaHuy`).
   - Concurrency protection for database stock decrement.
5. **Missing or unfinished backend capabilities identified**:
   - **`AdminDonHangController.cs`**:
     - `Index(string? q, string? trangThai, string? thanhToan, string? sort)`: List orders, search by code/customer/phone, filter by status, calculate aggregate revenue & charity funds.
     - `ChiTiet(int id)`: View comprehensive order details and line items.
     - `CapNhatTrangThai(int id, string trangThaiDonHang, string? trangThaiThanhToan)`: Transition order states and handle restocking if cancelled.
     - `XoaDonHang(int id)`: Delete order (or mark deleted) with safety checks.
   - **`DbInitializer.cs` enhancements**:
     - Add `GetInMemoryOrders()`, `UpdateInMemoryOrderStatus(int id, string status, string? paymentStatus)`, and `DeleteInMemoryOrder(int id)` to achieve full parity with DB mode.
   - **Admin UI Integration**:
     - Add "Quản lý Đơn hàng" link to `_AdminLayout.cshtml` and KPI summary to `Views/Admin/Index.cshtml`.
   - **Storefront Customer Order Lookup**:
     - Add `TraCuu(string? maDonHang, string? soDienThoai)` in `GioHangController` or `CuaHangController` so donors can track their order status.

---

## 5. Verification Method

1. **Compilation Check**:
   ```bash
   TMPDIR=/tmp DOTNET_CLI_HOME=/tmp XDG_DATA_HOME=/tmp dotnet build --no-restore WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Expected*: Build succeeds with 0 errors and 0 warnings.
2. **E2E Test Suite Run**:
   ```bash
   TMPDIR=/tmp DOTNET_CLI_HOME=/tmp XDG_DATA_HOME=/tmp dotnet run --project tests/E2ETests/E2ETests.csproj --no-restore
   ```
   *Expected*: All 280 existing tests pass.
3. **Controller & File Inspection**:
   - Verify non-existence of `Areas/CuaHang/Controllers/AdminDonHangController.cs` via `find_by_name`.
   - Inspect `Areas/CuaHang/Controllers/GioHangController.cs` lines 180–184 and 321–356.
   - Inspect `Areas/CuaHang/Helpers/DbInitializer.cs` lines 231–276.
