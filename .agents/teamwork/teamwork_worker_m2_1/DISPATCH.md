# Task Dispatch: Worker M2.1 - Backend Order Logic & Inventory Parity

## Role
Teamwork Worker (`teamwork_worker`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_1`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`
- Backend Survey Report: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1/handoff.md`

## Mandatory Integrity Warning
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Write Ownership (Exclusively Owned Files)
You are permitted to modify or create ONLY the following files:
1. `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`
2. `WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs`
3. `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs` (Create new)

Do NOT modify any files outside this list.

## Implementation Tasks

### 1. Enhance `DbInitializer.cs`
- Add in-memory order management methods to achieve full parity with database mode:
  - `List<DonHangGayQuy> GetInMemoryOrders()`: returns all in-memory orders ordered by `NgayDatHang` descending. Ensure thread-safety via `lock (_lock)`.
  - `bool UpdateInMemoryOrderStatus(int id, string trangThaiDonHang, string? trangThaiThanhToan)`:
    - Finds the order by `id`.
    - Updates `TrangThaiDonHang` and optionally `TrangThaiThanhToan`.
    - Updates `NgayCapNhat = DateTime.Now`.
    - **Restock logic**: If `trangThaiDonHang == "DaHuy"`, restore the inventory of each product in `ChiTietDonHangs`:
      `prod.SoLuongTon += detail.SoLuong; prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);`
    - Returns `true` if successful, `false` if order not found.
  - `bool DeleteInMemoryOrder(int id)`: removes order from `_inMemoryOrders` safely inside lock.
  - Seed at least 2 realistic sample orders in `_inMemoryOrders` (e.g. 1 "MoiDat", 1 "DaXacNhan") with complete customer details and order details so admin order management can be tested immediately.

### 2. Enhance `GioHangController.cs`
- In `CapNhatSoLuong`:
  - Fix the bug identified in survey: if `item.SoLuongTon <= 0`, do NOT bypass clamping. If out of stock, clamp to 0 or return error/warning.
- In `DatHang` (POST):
  - Pre-checkout inventory check: Before saving the order, check if any requested item in cart has `item.SoLuong > product.SoLuongTon`.
  - If any product has insufficient stock, return an error message to the user (`TempData["ErrorMessage"] = $"Sản phẩm {item.TenSanPham} chỉ còn {product.SoLuongTon} sản phẩm trong kho.";` or JSON error if AJAX) instead of silently overselling.

### 3. Implement `AdminDonHangController.cs`
- Location: `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminDonHangController.cs`
- Namespace: `WEB_TinhNguyen.Areas.CuaHang.Controllers`
- Attributes: `[Area("CuaHang")]`
- Dependency Injection: `ApplicationDbContext _context`
- Implement Actions:
  - `Index(string? q, string? trangThai, string? thanhToan, string? sort)`:
    - Fetches orders from EF Core if DB is connected (`_context.Database.CanConnect()`), otherwise falls back to `DbInitializer.GetInMemoryOrders()`.
    - Supports search by `q` (donor name, phone number, order code).
    - Supports filter by `trangThai` ("MoiDat", "DaXacNhan", "DangGiao", "HoanThanh", "DaHuy", or all).
    - Supports filter by `thanhToan` ("ChuaThanhToan", "DaThanhToan", or all).
    - Calculates and passes KPI statistics to `ViewBag`:
      * `ViewBag.TongDonHang` (Total count of orders)
      * `ViewBag.ChoXacNhan` (Count of orders with status "MoiDat")
      * `ViewBag.TongDoanhThu` (Sum of `TongThanhToan`)
      * `ViewBag.TongTienQuy` (Sum of `TienUngHoThem` + `TienDongGop` across all details)
    - Returns `View(orders)`.
  - `ChiTiet(int id)`:
    - Retrieves order with its `ChiTietDonHangs` (and includes `SanPhamGayQuy` if available).
    - Returns `View(order)`. If not found, returns `NotFound()`.
  - `CapNhatTrangThai(int id, string trangThaiDonHang, string? trangThaiThanhToan)` [HttpPost]:
    - Validates order ID and status.
    - Transitions order status in DB or In-memory fallback (`DbInitializer.UpdateInMemoryOrderStatus`).
    - If status changed to "DaHuy", restore product stock:
      `prod.SoLuongTon += detail.SoLuong; prod.DaBan = Math.Max(0, prod.DaBan - detail.SoLuong);`
    - Sets `TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";`
    - Redirects to `ChiTiet(id)` or `Index()`. If AJAX, returns JSON `{ success = true, message = ... }`.
  - `XoaDonHang(int id)` [HttpPost]:
    - Deletes order in DB or `DbInitializer.DeleteInMemoryOrder(id)`.
    - Sets `TempData["SuccessMessage"] = "Đã xóa đơn hàng thành công!";`
    - Redirects to `Index()`.

## Verification Commands Required
Before delivering handoff, you MUST run:
1. Solution build:
   `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
   Must pass with 0 errors, 0 warnings.
2. Existing E2E test suite regression:
   `bash tests/run_e2e_tests.sh --all`
   Must pass 280/280 tests with 0 regressions.

Write your report to `handoff.md` in your working directory.
