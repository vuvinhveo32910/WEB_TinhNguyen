# Test Suite, Verification & Integration Audit Report

**Author**: `teamwork_explorer_r2_survey_3`  
**Date**: 2026-11-04  
**Target Solution**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen`  
**Objective**: Comprehensive audit of `tests/E2ETests/`, build process, existing 280 test cases across Tiers 1–5, verification requirements (`XDG_RUNTIME_DIR=/tmp`), and proposed test coverage expansions for Phase 2 enhancements (Admin Order Management, Status Updates, Search/Filter, Toasts/Modals, Stock Deduction Checks).

---

## 1. Observation

### 1.1 Solution & Project Structure
- **Solution File**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
  ```xml
  <Solution>
    <Project Path="WEB_TinhNguyen/WEB_TinhNguyen.csproj" />
  </Solution>
  ```
  *Observation*: `WEB_TinhNguyen.slnx` only contains `WEB_TinhNguyen.csproj`. `E2ETests.csproj` is intentionally kept as an independent project outside `slnx` to decouple test builds from main web builds.

- **Test Project File**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`
  ```xml
  <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
      <OutputType>Exe</OutputType>
      <TargetFramework>net10.0</TargetFramework>
      <Nullable>enable</Nullable>
      <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
      <FrameworkReference Include="Microsoft.AspNetCore.App" />
      <ProjectReference Include="../../WEB_TinhNguyen/WEB_TinhNguyen.csproj" />
    </ItemGroup>
  </Project>
  ```
  *Observation*: Test suite is a standalone console application (`OutputType=Exe`), targeting `net10.0`, referencing `Microsoft.AspNetCore.App` and the web project. It has **zero dependencies** on external xUnit, NUnit, or MSTest NuGet packages, eliminating network restore risks in this offline environment.

- **Offline NuGet Configuration**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/NuGet.Config`
  ```xml
  <?xml version="1.0" encoding="utf-8"?>
  <configuration>
    <packageSources>
      <clear />
      <add key="local-cache" value="/home/hutech/.nuget/packages" />
    </packageSources>
  </configuration>
  ```
  *Observation*: Explicitly clears online package sources and binds solely to `/home/hutech/.nuget/packages`. When `dotnet build` is invoked without `--configfile tests/NuGet.Config`, it attempts to connect to `https://api.nuget.org/v3/index.json`, times out for ~6 seconds, and emits `warning NU1900`.

- **Build Target in Web Project**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj` lines 21–24:
  ```xml
  <Target Name="TailwindCSS" BeforeTargets="Build">
      <Exec Command="npm run build:css" />
  </Target>
  ```
  *Observation*: Every build executes Tailwind CSS compilation (`tailwindcss -i Styles/app.css -o wwwroot/css/site.css --minify`). This step depends on NodeJS, NPM, and a writable user home / runtime directory.

- **Execution Script**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh` lines 19–24:
  ```bash
  export XDG_RUNTIME_DIR=/tmp
  export DOTNET_CLI_HOME=/tmp
  export HOME=/tmp

  CONFIG_FILE="$SCRIPT_DIR/NuGet.Config"
  PROJECT_FILE="$SCRIPT_DIR/E2ETests/E2ETests.csproj"
  ```
  Line 34: `dotnet build "$PROJECT_FILE" --configfile "$CONFIG_FILE" --verbosity minimal`  
  Line 41: `dotnet run --no-build --project "$PROJECT_FILE" -- --all`

### 1.2 Test Suite Execution Output
Running `bash tests/run_e2e_tests.sh --all` produces:
```text
========================================================================
 Building WEB_TinhNguyen E2E Test Suite...
========================================================================
  Determining projects to restore...
  All projects are up-to-date for restore.
  WEB_TinhNguyen -> /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/bin/Debug/net10.0/WEB_TinhNguyen.dll
  Done in 2413ms.
  E2ETests -> /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
========================================================================
 Executing WEB_TinhNguyen E2E Test Suite...
========================================================================
  [PASS] Tier 5 | F02 | CH01: SessionExtensions throws ArgumentNullException on null session (6.7ms)
  ...
  [PASS] Tier 1 | F08 | F08_T05: Detail view displays original price when discounted (0.3ms)

================================================================================
                              TEST EXECUTION SUMMARY
================================================================================
  Tier 1: Total:  85 | Passed:  85 | Failed:   0 | Skipped:   0
  Tier 2: Total:  25 | Passed:  25 | Failed:   0 | Skipped:   0
  Tier 3: Total:  16 | Passed:  16 | Failed:   0 | Skipped:   0
  Tier 4: Total:  12 | Passed:  12 | Failed:   0 | Skipped:   0
--------------------------------------------------------------------------------
  OVERALL: 280 Tests | 280 Passed | 0 Failed | 0 Skipped in 2.75s
================================================================================
```

### 1.3 Exact Test Inventory and Tier Breakdown
Inspecting the code via Python AST/regex analysis over `tests/E2ETests/` reveals:
- **Total Tests Found**: Exactly 280 methods decorated with `[E2ETest(...)]`.
- **Distribution by Tier**:
  - **Tier 1 (Contract & Functional Integrity)**: 85 tests
    - `Tier1/Tier1_FoundationTests.cs`: 20 tests (Features F01–F04)
    - `Tier1/Tier1_StorefrontTests.cs`: 20 tests (Features F05–F08)
    - `Tier1/Tier1_CartCheckoutTests.cs`: 15 tests (Features F09–F11)
    - `Tier1/Tier1_AdminTests.cs`: 25 tests (Features F12–F16)
    - `Tier1/Tier1_BuildVerificationTests.cs`: 5 tests (Feature F17)
  - **Tier 2 (Boundary & Corner Cases)**: 25 tests
    - `Tier2/Tier2_BoundaryCornerTests.cs`: 25 tests (`B01` to `B25`)
  - **Tier 3 (Cross-Feature Combinations)**: 16 tests
    - `Tier3/Tier3_CrossFeatureTests.cs`: 16 tests (`C01` to `C16`)
  - **Tier 4 (Real-World End-to-End Scenarios)**: 12 tests
    - `Tier4/Tier4_RealWorldScenarioTests.cs`: 12 tests (`S01` to `S12`)
  - **Tier 5 (Adversarial Stress & Challenges)**: 142 tests
    - `Tier5/Tier5_ChallengerTests.cs`: 47 tests
    - `Tier5/Tier5_ChallengerM1Harness.cs`: 19 tests
    - `Tier5/Tier5_ChallengerM2Harness.cs`: 42 tests
    - `Tier5/Tier5_ChallengerM3_2Harness.cs`: 34 tests
- **Discovered Reporting Quirk in `TestRunner.cs` Line 219**:
  ```csharp
  for (int t = 1; t <= 4; t++)
  {
      var tierResults = results.Where(r => r.Tier == t).ToList();
      ...
  }
  ```
  The loop specifically terminates at `t <= 4`. Consequently, the breakdown displays Tiers 1–4 (85 + 25 + 16 + 12 = 138), while the overall total correctly reports `280 Tests | 280 Passed` (138 + 142 Tier 5 = 280).

### 1.4 Codebase Feature Gap Analysis
Examining `WEB_TinhNguyen/Areas/CuaHang/` and `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`:
- **Existing Entities & DbSets**:
  - `DonHangGayQuy.cs` and `ChiTietDonHangGayQuy.cs` exist and are mapped in `ApplicationDbContext.cs`.
  - Properties include: `MaDonHang`, `HoTenNguoiMua`, `SoDienThoai`, `Email`, `DiaChiNhanHang`, `GhiChu`, `TongTienHang`, `TienUngHoThem`, `PhiVanChuyen`, `TongThanhToan`, `PhuongThucThanhToan`, `TrangThaiDonHang`, `TrangThaiThanhToan`, `NgayDatHang`, `NgayCapNhat`.
- **Existing In-Memory Storage in `DbInitializer.cs`**:
  - `DbInitializer.AddInMemoryOrder(order)`: registers orders, generates order codes (`GQ2026-yyyyMMdd-XXXX`), floors stock deduction `prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong)` and increments `prod.DaBan`.
  - `DbInitializer.GetInMemoryOrderById(id)` & `DbInitializer.GetInMemoryOrderByCode(code)` exist.
- **Missing Elements for Admin Order Management**:
  - **No Controller**: `AdminDonHangController.cs` does not exist.
  - **No Views**: No `Areas/CuaHang/Views/AdminDonHang/` folder (missing `Index.cshtml`, `ChiTiet.cshtml`).
  - **No Admin Navigation**: `_AdminLayout.cshtml` has a link for `Sản phẩm Gây Quỹ` (`/CuaHang/AdminSanPham`) at line 268, but **no link** for `Đơn hàng Gây Quỹ` / `Quản lý Đơn hàng`.
  - **Missing In-Memory Order Methods in `DbInitializer.cs`**:
    * `DbInitializer.GetInMemoryOrders()` does not exist (needed for Admin Order listing).
    * `DbInitializer.UpdateInMemoryOrderStatus(int id, string orderStatus, string? paymentStatus)` does not exist.
    * `DbInitializer.DeleteInMemoryOrder(int id)` does not exist.
- **Storefront Features Current State**:
  - `CuaHangController.Index(string? q, string? danhMuc, string? sort)` already supports keyword search, category filtering, and sorting (`gia-tang`, `gia-giam`, `dong-gop`, `moi-nhat`).
  - `Index.cshtml` has a search input and category filter tabs, but lacks an empty state clear/reset button test and live toast notifications for AJAX cart additions.

---

## 2. Logic Chain

1. **Self-Contained Offline Test Framework**:
   - The test suite uses reflection against `WEB_TinhNguyen.dll` loaded inside an executable console host (`tests/E2ETests/`).
   - Because it avoids third-party runner packages, it runs cleanly and instantaneously (2.75s for 280 tests) in offline mode without network calls.
   - It respects the contract-driven test pattern where assertions test entity attributes, controller actions, view content strings, model state validation, and business formulas.

2. **Sandbox Environment Compatibility (`XDG_RUNTIME_DIR=/tmp`)**:
   - The build process triggers `npm run build:css` via an MSBuild target in `WEB_TinhNguyen.csproj`.
   - Node, Tailwind, and .NET CLI try to access `$HOME` and user runtime directories.
   - In container sandboxes with isolated non-root users, defaulting to system `/run/user/<uid>` can cause permission denials.
   - Pre-setting `XDG_RUNTIME_DIR=/tmp`, `DOTNET_CLI_HOME=/tmp`, and `HOME=/tmp` ensures that .NET, NPM, and Tailwind create files in `/tmp` without error.

3. **Requirement for Admin Order Management in Round 2**:
   - The user requested completing remaining unfinished areas (`ORIGINAL_REQUEST.md`).
   - Donors place orders via `GioHangController.ThanhToan` into `DonHangGayQuys`, but administrators currently have no UI or API to view or transition order statuses.
   - To make the fundraising store feature complete, `AdminDonHangController` and its accompanying views must be created under Area `CuaHang`.
   - To ensure regression safety and high test coverage, tests must be added to verify `AdminDonHangController`, order listing, status transitions, and in-memory fallback.

4. **Test Suite Expansion Strategy**:
   - Rather than disrupting existing 280 tests (which all pass 100%), new test cases can be added cleanly by:
     a. Creating a new test class or extending Tier 1 with a new Feature: `F18: Admin Order Management`.
     b. Adding boundary/stress tests in Tier 5 (`Tier5_ChallengerM4OrderHarness.cs` or adding to `Tier5_ChallengerTests.cs`).
     c. Updating `TestRunner.cs` line 219 (`t <= 5`) so Tier 5 is properly displayed in the console report.

---

## 3. Caveats

- **No Caveats on Execution Reliability**: All 280 tests execute and pass 100% cleanly in the offline sandbox environment.
- **SQL Server vs In-Memory Fallback**: When testing locally without a live SQL Server instance, both the web application and test runner gracefully fall back to `DbInitializer` in-memory collections. New admin features and tests must maintain this dual-mode capability (SQL Server EF Core + In-Memory Fallback).
- **TestRunner Tier 5 Summary Display**: In `TestRunner.cs`, Tier 5 tests are executed and included in the grand total, but the per-tier summary loop only iterates `1..4`. Updating this loop to `1..5` will improve reporting visibility.

---

## 4. Conclusion & Actionable Test Plan

The test architecture is robust, fast, and completely decoupled from external network access. To support the new Phase 2 enhancements, the test suite should be expanded with **30+ new test cases** covering the following modules:

### 4.1 Recommended New Test Specifications

#### Module A: Admin Order Management (Feature 18) — Proposed 15 Tests in Tier 1
1. **F18_T01**: `AdminDonHangController` class exists in `WEB_TinhNguyen.Areas.CuaHang.Controllers` with `[Area("CuaHang")]`.
2. **F18_T02**: `_AdminLayout.cshtml` contains navigation item linking to `/CuaHang/AdminDonHang` or `asp-controller="AdminDonHang"`.
3. **F18_T03**: Admin order navigation link displays title "Đơn Hàng Gây Quỹ" or "Quản lý đơn hàng" with dedicated icon.
4. **F18_T04**: `AdminDonHang/Index.cshtml` view template exists and renders order table headers (Mã đơn, Người nhận, SĐT, Tổng tiền, Trạng thái, Thao tác).
5. **F18_T05**: `AdminDonHang/Index.cshtml` contains KPI metrics (Tổng đơn, Chờ xác nhận, Doanh thu gây quỹ).
6. **F18_T06**: `AdminDonHang/ChiTiet.cshtml` view template exists and displays donor contact, delivery address, order items breakdown, and notes.
7. **F18_T07**: `AdminDonHangController.Index` supports status filtering (`MoiDat`, `DaXacNhan`, `DangGiao`, `HoanThanh`, `DaHuy`, `TatCa`).
8. **F18_T08**: `AdminDonHangController.Index` supports search query by donor name, phone number, or order code.
9. **F18_T09**: `AdminDonHangController.CapNhatTrangThai` action method exists, accepts order ID and status string.
10. **F18_T10**: Status transition to valid states (`MoiDat`, `DaXacNhan`, `DangGiao`, `HoanThanh`, `DaHuy`) succeeds and updates `NgayCapNhat`.
11. **F18_T11**: Status update sets `TempData["SuccessMessage"]` and redirects to Index or ChiTiet.
12. **F18_T12**: Status update with invalid/empty order ID returns NotFound or safe redirect.
13. **F18_T13**: `DbInitializer.GetInMemoryOrders()` returns list of current orders safely.
14. **F18_T14**: `DbInitializer.UpdateInMemoryOrderStatus(id, status, paymentStatus)` correctly updates status and timestamp.
15. **F18_T15**: `AdminDonHang/ChiTiet.cshtml` displays VietQR payment instructions for bank transfer orders.

#### Module B: Storefront Search, Category Filter & Stock Checks — Proposed 8 Tests in Tier 2 & Tier 3
16. **Storefront_Search_Unicode**: Storefront search by Vietnamese accented characters ("áo thun", "móc khóa") matches accurately.
17. **Storefront_Search_EmptyState**: Non-matching search keyword renders friendly empty state view with reset button.
18. **Storefront_Category_Filter**: Selecting specific category ("Trang phục", "Phụ kiện") returns only products in that category.
19. **Storefront_Sort_Donation**: Sorting by `dong-gop` prioritizes products with highest `PhanTramDongGop`.
20. **Storefront_Stock_Zero_Indicator**: Products with `SoLuongTon == 0` display "Hết hàng" (Sold Out) badge in catalog and disable Add to Cart button.
21. **Cart_Quantity_Stock_Cap**: Adding quantity exceeding `SoLuongTon` caps at available stock or reports warning.
22. **Checkout_Stock_Deduction**: Completing checkout reduces product `SoLuongTon` by exact ordered amount.
23. **Checkout_DaBan_Increment**: Completing checkout increases product `DaBan` by exact ordered amount.

#### Module C: UI/UX Toast Notifications & Modals — Proposed 7 Tests in Tier 1 & Tier 5
24. **UI_Toast_Container_Storefront**: Storefront layouts/views include Toast notification markup or dismissible alerts for `TempData["SuccessMessage"]` and `TempData["ErrorMessage"]`.
25. **UI_Toast_Container_Admin**: Admin views include alert/toast elements for CRUD confirmation feedback.
26. **Cart_Ajax_Response_Format**: Adding to cart via AJAX request returns valid JSON with `success`, `message`, `cartCount` for toast consumption.
27. **Admin_Delete_Modal_Prompt**: Product and order deletion or status changes include confirmation dialog / modal attributes.
28. **Admin_Order_Status_Badge_Styling**: Status badges use distinct semantic Tailwind classes (Yellow for `MoiDat`, Blue for `DaXacNhan`, Indigo for `DangGiao`, Emerald for `HoanThanh`, Red for `DaHuy`).
29. **DbInitializer_Order_Concurrency**: 50 parallel order placements safely update in-memory order list and decrement inventory without race conditions.
30. **TestRunner_Tier5_Summary_Fix**: Test runner breakdown loops through Tier 5 and outputs 5-tier summary.

---

## 5. Verification Method

To independently verify all findings and test suite execution:

1. **Verify Full E2E Test Suite**:
   ```bash
   bash tests/run_e2e_tests.sh --all
   ```
   *Expected Result*: 280 tests execute, 280 pass, 0 fail, 0 skipped, time ~2.7s.

2. **Verify Filtered Tier 1 Tests**:
   ```bash
   bash tests/run_e2e_tests.sh --tier 1
   ```
   *Expected Result*: 85 tests execute, 85 pass, 0 fail, time ~0.3s.

3. **Verify Filtered Tier 5 Tests**:
   ```bash
   bash tests/run_e2e_tests.sh --tier 5
   ```
   *Expected Result*: 142 tests execute, 142 pass, 0 fail, time ~0.8s.

4. **Verify Offline Clean Build of Web Project with Environment Variables**:
   ```bash
   export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp
   dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal
   ```
   *Expected Result*: Tailwind CSS builds successfully, 0 errors, 0 warnings, build succeeds.

5. **Files to Inspect for Structural Verification**:
   - `tests/E2ETests/Framework/TestRunner.cs` (lines 50–240)
   - `tests/E2ETests/Framework/Assertions.cs` (lines 11–155)
   - `tests/E2ETests/Tier1/` (AdminTests, BuildVerificationTests, CartCheckoutTests, FoundationTests, StorefrontTests)
   - `tests/E2ETests/Tier5/` (ChallengerTests, ChallengerM1Harness, ChallengerM2Harness, ChallengerM3_2Harness)
   - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs` (lines 150–303)
   - `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` (lines 240–285)
