# Handoff Report: Explorer M3.1 (Storefront Controller & Views)

## 1. Observation

### 1.1 Existing Files and Area Structure
1. **Area Directory Location**:
   - The ASP.NET Core project root is located at `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/`.
   - The Area directory exists at `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/`.
   - Existing controllers in Area: only `AdminSanPhamController.cs` exists in `Areas/CuaHang/Controllers/`. `CuaHangController.cs` is missing.
   - Existing views in Area: `Areas/CuaHang/Views/` contains `_ViewImports.cshtml`, `_ViewStart.cshtml`, and `AdminSanPham/` folder. The folder `Areas/CuaHang/Views/CuaHang/` does not exist yet (`Index.cshtml` and `ChiTiet.cshtml` are missing).

2. **Routing Configuration (`Program.cs:50-53`)**:
   ```csharp
   app.MapControllerRoute(
       name: "areas",
       pattern: "{area:exists}/{controller=CuaHang}/{action=Index}/{id?}")
       .WithStaticAssets();
   ```
   - Defaults: controller defaults to `CuaHang`, action defaults to `Index`.
   - Requests to `/CuaHang` route directly to `CuaHangController.Index`.
   - Requests to `/CuaHang/ChiTiet/{id}` route to `CuaHangController.ChiTiet`.

3. **Layout Resolution (`Areas/CuaHang/Views/_ViewStart.cshtml:7-15`)**:
   ```csharp
   if (controller.StartsWith("Admin", StringComparison.OrdinalIgnoreCase) ||
       viewPath.Contains("/Admin", StringComparison.OrdinalIgnoreCase))
   {
       Layout = "~/Views/Shared/_AdminLayout.cshtml";
   }
   else
   {
       Layout = "~/Views/Shared/_Layout.cshtml";
   }
   ```
   - For `CuaHangController` views (`Index.cshtml`, `ChiTiet.cshtml`), `controller` is `"CuaHang"` and `viewPath` does not contain `"/Admin"`, so it automatically resolves `~/Views/Shared/_Layout.cshtml`.

4. **Data Entities & Dual-Mode Access (`DbInitializer.cs` & `SanPhamGayQuy.cs`)**:
   - `DbInitializer.cs` defines 4 default charity products:
     - ID 1: Áo Thun Tình Nguyện "Trăng Sáng Langbiang 2026" (129,000₫, 100% quỹ, 100 tồn kho, link `/assets/journey.jpg`)
     - ID 2: Túi Tote Canvas "Yêu Thương Gửi Về Núi Rừng" (79,000₫, 100% quỹ, 85 tồn kho, link `/assets/cooking.jpg`)
     - ID 3: Bình Giữ Nhiệt Vỏ Tre Khắc Tên 500ml (159,000₫, 100% quỹ, 50 tồn kho, link `/assets/children.jpg`)
     - ID 4: Móc Khóa Linh Vật Thỏ Trăng Langbiang Đan Len (35,000₫, 100% quỹ, 120 tồn kho, link `/assets/lanterns.jpg`)
   - `DbInitializer.GetInMemoryProducts()` returns `List<SanPhamGayQuy>` safely cloned.
   - `DbInitializer.GetInMemoryProductById(int id)` returns a single product or `null`.
   - `SanPhamGayQuy.cs` provides helper properties:
     - `ConHang => SoLuongTon > 0`
     - `TienDongGopTrenMoiSp => GiaBan * PhanTramDongGop / 100m`

### 1.2 Test Assertions Direct Quotes

1. **Feature 7 Test Requirements (`Tier1_StorefrontTests.cs:152-205`)**:
   - **F07_T01**: `Assert.RequireFile(Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml"), "Catalog view template");`
   - **F07_T02**:
     - `Assert.Contains("TenSanPham", viewContent, "Catalog view must render TenSanPham");`
     - `Assert.Contains("GiaBan", viewContent, "Catalog view must render GiaBan");`
     - `Assert.Contains("LinkHinhAnh", viewContent, "Catalog view must render LinkHinhAnh");`
   - **F07_T03**:
     - `viewContent.Contains("ThongDiepGayQuy") || viewContent.Contains("gây quỹ") || viewContent.Contains("đóng góp")`
   - **F07_T04**:
     - `viewContent.Contains("SoLuongTon") || viewContent.Contains("Còn hàng") || viewContent.Contains("Hết hàng")`
   - **F07_T05**:
     - `viewContent.Contains("grid") && (viewContent.Contains("grid-cols-1") || viewContent.Contains("sm:grid-cols-2") || viewContent.Contains("lg:grid-cols-3"))`

2. **Feature 8 Test Requirements (`Tier1_StorefrontTests.cs:207-262`)**:
   - **F08_T01**: `Assert.RequireFile(Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml"), "Product detail view template");`
   - **F08_T02**:
     - `Assert.Contains("MoTaChiTiet", viewContent, "Detail view must display MoTaChiTiet");`
     - `Assert.Contains("ThongDiepGayQuy", viewContent, "Detail view must display ThongDiepGayQuy");`
   - **F08_T03**:
     - `viewContent.Contains("name=\"soLuong\"") || viewContent.Contains("name='soLuong'") || viewContent.Contains("type=\"number\"")`
   - **F08_T04**:
     - `viewContent.Contains("ThemVaoGio") || viewContent.Contains("GioHang") || viewContent.Contains("Thêm vào giỏ")`
   - **F08_T05**:
     - `viewContent.Contains("GiaGoc") || viewContent.Contains("line-through")`

3. **Tier 4 Real-World Scenario Assertions (`Tier4_RealWorldScenarioTests.cs:13-33`)**:
   - **S01**: `Assert.RequireFile(catalogViewPath, "CuaHang/Index catalog"); Assert.Contains("TenSanPham", content); Assert.Contains("GiaBan", content);`
   - **S02**: `Assert.RequireFile(detailViewPath, "CuaHang/ChiTiet view"); Assert.Contains("ThongDiepGayQuy", content);`

4. **Current Test Execution Results**:
   - Running `./tests/run_e2e_tests.sh --feature 7`:
     `[SKIP] Tier 1 | F07 | F07_T01... to F07_T05 -> [SKIP] Pending implementation: Catalog view template (Index.cshtml)`
   - Running `./tests/run_e2e_tests.sh --feature 8`:
     `[SKIP] Tier 1 | F08 | F08_T01... to F08_T05 -> [SKIP] Pending implementation: Product detail view template (ChiTiet.cshtml)`
   - Running `./tests/run_e2e_tests.sh --tier 4`:
     `S01` and `S02` are skipped because `Index.cshtml` and `ChiTiet.cshtml` do not yet exist.

---

## 2. Logic Chain

1. **Routing and Controller Binding**:
   - `Program.cs:52` defines area route pattern `{area:exists}/{controller=CuaHang}/{action=Index}/{id?}`.
   - Therefore, `CuaHangController` decorated with `[Area("CuaHang")]` maps seamlessly to `/CuaHang` and `/CuaHang/Index`.
   - `ChiTiet(int id)` maps to `/CuaHang/ChiTiet/{id}`.

2. **Dual-Mode Data Access**:
   - In production or local dev with SQL Server online: `_context.Database.CanConnect()` returns true, products are queried from `_context.SanPhamGayQuys`.
   - In offline demo or sandbox: `_context.Database.CanConnect()` is false or throws an exception. Catching this and querying `DbInitializer.GetInMemoryProducts()` guarantees 100% zero-crash operation.
   - Active filtering (`p.TrangThai == true`) must be applied in both branches.

3. **Catalog Filtering & Sorting Logic**:
   - Keyword search (`q`): Case-insensitive match across `TenSanPham`, `MoTaNgan`, `ThongDiepGayQuy`, and `DanhMuc`.
   - Category filter (`danhMuc`): When specified and not "Tất cả", filters `p.DanhMuc == danhMuc`.
   - Sorting (`sort`):
     - `gia-tang` / `price-asc`: `.OrderBy(p => p.GiaBan)`
     - `gia-giam` / `price-desc`: `.OrderByDescending(p => p.GiaBan)`
     - `dong-gop` / `donation`: `.OrderByDescending(p => p.PhanTramDongGop).ThenByDescending(p => p.GiaBan)`
     - `moi-nhat` / `newest`: `.OrderByDescending(p => p.NgayTao)`
     - default: `.OrderBy(p => p.ThuTu).ThenByDescending(p => p.IDSanPham)`
   - Categories extraction: Dynamic distinct list of active categories sent to `ViewBag.Categories` for filtering tabs.

4. **Product Detail & Related Items Logic**:
   - `ChiTiet(int id)` validates `id > 0`, retrieves active product by ID.
   - Non-existent/inactive product handling: Gracefully sets `TempData["ErrorMessage"]` and redirects to `Index` (or returns `NotFound()`).
   - Related products: Selects up to 4 other active products (`p.IDSanPham != id`), prioritizing same `DanhMuc`, passes to `ViewBag.RelatedProducts`.

5. **View Contract & Assertion Satisfaction**:
   - `Index.cshtml`:
     - Renders model `IEnumerable<SanPhamGayQuy>`.
     - Hero banner conveying "Trăng Sáng Langbiang 2026" fundraising mission.
     - Responsive grid: `grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6`.
     - Product cards show `LinkHinhAnh`, `TenSanPham`, `GiaBan`, `GiaGoc` (with `line-through` when discounted), charity badge (`ThongDiepGayQuy`, `gây quỹ`, `đóng góp`), stock status (`SoLuongTon`, `Còn hàng`, `Hết hàng`).
     - Buttons: "Xem chi tiết" linking to `ChiTiet` and "Thêm vào giỏ" posting to `/CuaHang/GioHang/ThemVaoGio`.
   - `ChiTiet.cshtml`:
     - Renders model `SanPhamGayQuy`.
     - 2-column layout (gallery/image preview left, details & purchase panel right).
     - Displays `TenSanPham`, `GiaBan`, `GiaGoc` with `line-through`.
     - Displays `ThongDiepGayQuy` prominently in a humanitarian mission impact card.
     - Quantity selector with `-` and `+` buttons, `type="number"`, `name="soLuong"`, `min="1"`, `max="@Model.SoLuongTon"`.
     - Form POST action `ThemVaoGio` on `GioHangController` in area `CuaHang`.
     - Displays `MoTaChiTiet` in detailed product description block.
     - Displays related products grid at bottom.

---

## 3. Caveats

1. **Session Cart Dependency**:
   - The quick-add button in `Index.cshtml` and the purchase button in `ChiTiet.cshtml` POST to `/CuaHang/GioHang/ThemVaoGio` with `id` and `soLuong`. This action is implemented by Explorer/Worker M3.2 (`GioHangController.cs`). The contract between M3.1 and M3.2 is verified: M3.2 expects `int id` and `int soLuong = 1`.
2. **Navigation Integration Dependency**:
   - Desktop and Mobile Navigation links in `Views/Shared/_Layout.cshtml` are assigned to Explorer/Worker M3.3.
3. **Database vs In-Memory IDs**:
   - When SQL Server is online and identity is generated, IDs match database primary keys. When in-memory fallback is active, IDs match `1, 2, 3, 4`. Both modes support integer `id` uniformly.

---

## 4. Conclusion (Concrete Implementation Recommendations)

The Worker should create three files using the exact code prepared in this agent folder:

### File 1: `WEB_TinhNguyen/Areas/CuaHang/Controllers/CuaHangController.cs`
- Source template: `.agents/teamwork/teamwork_explorer_m3_1/proposed_CuaHangController.cs`
- Key specs:
  - `[Area("CuaHang")]`
  - Constructor injects `ApplicationDbContext`
  - Action `Index(string? q, string? danhMuc, string? sort)` with dual-mode fallback, category filtering, text search, multi-mode sorting.
  - Action `ChiTiet(int id)` with dual-mode fallback, 404/redirect error handling, related products query.

### File 2: `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/Index.cshtml`
- Source template: `.agents/teamwork/teamwork_explorer_m3_1/proposed_Index.cshtml`
- Target path: `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/Index.cshtml` (create directory `Views/CuaHang/` if not present)
- Key specs:
  - Hero banner with fundraising mission and purpose.
  - Responsive search bar and dynamic category filter pills.
  - Responsive multi-column grid (`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6`).
  - Cards rendering `LinkHinhAnh`, `TenSanPham`, `GiaBan`, `GiaGoc` + `line-through`, `ThongDiepGayQuy`, `gây quỹ`, `đóng góp`, `SoLuongTon`, `Còn hàng`, `Hết hàng`.
  - Links to `ChiTiet` and form POST to `/CuaHang/GioHang/ThemVaoGio`.

### File 3: `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`
- Source template: `.agents/teamwork/teamwork_explorer_m3_1/proposed_ChiTiet.cshtml`
- Target path: `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`
- Key specs:
  - Responsive 2-column layout (gallery preview left, details right).
  - Breadcrumbs navigation.
  - Price display with original price strike-through (`GiaGoc` + `line-through`).
  - Humanitarian impact story (`ThongDiepGayQuy`).
  - Quantity selector (`name="soLuong"`, `type="number"`, `min="1"`, `max="@Model.SoLuongTon"`).
  - Add to cart form (`asp-area="CuaHang" asp-controller="GioHang" asp-action="ThemVaoGio"`).
  - Detailed product story (`MoTaChiTiet`).
  - Related products grid (`ViewBag.RelatedProducts`).

---

## 5. Verification Method

To independently verify the implementation after Worker executes:

1. **Build Verification**:
   ```bash
   export XDG_RUNTIME_DIR=/tmp
   dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Expected*: Build succeeded with 0 errors.

2. **E2E Feature 7 Test (Catalog View)**:
   ```bash
   /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --feature 7
   ```
   *Expected*: All 5 tests PASS (`F07_T01` to `F07_T05`).

3. **E2E Feature 8 Test (Product Detail View)**:
   ```bash
   /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --feature 8
   ```
   *Expected*: All 5 tests PASS (`F08_T01` to `F08_T05`).

4. **E2E Tier 4 Scenarios (S01, S02)**:
   ```bash
   /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --tier 4
   ```
   *Expected*: `S01` and `S02` turn from `[SKIP]` to `[PASS]`.

5. **Invalidation Conditions**:
   - If `TenSanPham`, `GiaBan`, `LinkHinhAnh`, `ThongDiepGayQuy`, or `SoLuongTon` are omitted from `Index.cshtml`, F07 tests will fail.
   - If `MoTaChiTiet`, `ThongDiepGayQuy`, `name="soLuong"`, `ThemVaoGio`, or `GiaGoc` are omitted from `ChiTiet.cshtml`, F08 tests will fail.
   - If `CuaHangController` does not have `[Area("CuaHang")]`, route resolution will fail.
