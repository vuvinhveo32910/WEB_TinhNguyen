# Forensic Audit Report: Milestone 2 — Admin Product Management

**Work Product**: Milestone 2 Implementation Files:
- `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml`
- `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`
- `WEB_TinhNguyen/Views/Admin/Index.cshtml`

**Profile**: General Project (Demo Mode)  
**Auditor**: `teamwork_auditor_m2_1`  
**Parent**: Project Orchestrator (`108b9d81-1dd6-495f-84eb-99dbb197aebe`)  
**Verdict**: **CLEAN**

---

### Phase Results
- **Hardcoded Output Detection**: PASS — No hardcoded test results, expected test strings (e.g. `[PASS]`, `F12`, `F16`), or fixed constants were found in the codebase.
- **Facade Implementation Detection**: PASS — Controller actions (`Index`, `Create`, `Edit`, `Delete`) implement genuine CRUD logic with EF Core and synchronized in-memory fallback.
- **Pre-populated Artifact Detection**: PASS — No pre-existing test result logs or fake attestation files exist in the repository.
- **File Upload Security & Storage**: PASS — Whitelist validation for image extensions (`.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`, `.svg`), 10MB size limit, GUID-based collision/path traversal protection (`Guid.NewGuid():N`), and physical storage in `wwwroot/uploads/sanpham/`.
- **Relational Integrity & Deletion Handling**: PASS — Delete POST action inspects foreign keys in `ChiTietDonHangs`; performs soft-delete (`TrangThai = false`) when past orders exist and hard-delete when no orders exist.
- **CSRF Protection**: PASS — `[ValidateAntiForgeryToken]` enforced on all mutation actions (`Create`, `Edit`, `Delete`) and paired with `@Html.AntiForgeryToken()` in all views.
- **Build Verification**: PASS — `dotnet build WEB_TinhNguyen.slnx` and `dotnet build tests/E2ETests/E2ETests.csproj` compiled with 0 errors using `XDG_RUNTIME_DIR=/tmp`.
- **E2E Feature Test Execution**: PASS — 25 out of 25 feature tests (F12, F13, F14, F15, F16) PASSED.
- **Scenario & Stress Harness**: PASS — 6 out of 6 Admin real-world scenarios (S07-S12) and 66 out of 66 Tier 5 stress tests PASSED.

---

## 1. Observation

Direct observations verified through forensic inspection and command execution:

1. **Source Code Inspection**:
   - `AdminSanPhamController.cs`:
     - Lines 21: Decorator `[Area("CuaHang")]`.
     - Lines 37-79: `Index()` queries `_context.SanPhamGayQuys.OrderBy(p => p.ThuTu).ThenByDescending(p => p.IDSanPham).ToListAsync()` with `DbInitializer.Seed(_context)` auto-seed and in-memory fallback.
     - Lines 84-98: `Create()` GET initializes `SanPhamCrudViewModel` with default fundraising values (`PhanTramDongGop = 100`, `SoLuongTon = 50`).
     - Lines 100-163: `Create()` POST enforces extension whitelist (`.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`, `.svg`), 10MB size limit, saves file asynchronously with GUID filename, persists entity to `_context.SanPhamGayQuys`, and synchronizes in-memory fallback.
     - Lines 168-205: `Edit(int id)` GET validates ID, retrieves entity from EF or in-memory fallback, converts to ViewModel.
     - Lines 207-286: `Edit(int id, SanPhamCrudViewModel model)` POST verifies ID matching, handles optional new image upload, preserves existing image if no file supplied, updates DB entity and in-memory fallback.
     - Lines 295-384: `Delete(int id)` POST validates ID, checks `ChiTietDonHangs` relation to prevent FK constraint violations, soft-deletes or removes from EF context and in-memory store.
     - Lines 389-413: `SaveFileAsync` resolves `wwwroot/uploads/sanpham`, creates directory if non-existent, writes stream with unique GUID, returns `/uploads/sanpham/{guid}{ext}`.

2. **Razor Views Inspection**:
   - `Index.cshtml`: Displays 4 KPI summary cards, instant client-side search `filterAdminTable()`, responsive 8-column table with product thumbnails, donation badges (`❤️ @item.PhanTramDongGop% Quỹ`), status pills, and deletion form with confirmation prompt `onsubmit="return confirm(...)";`.
   - `Create.cshtml`: Declares `enctype="multipart/form-data"`, `@Html.AntiForgeryToken()`, charity donation range slider and number input synchronized in real time, client-side image preview for uploaded files and URL links.
   - `Edit.cshtml`: Declares `enctype="multipart/form-data"`, `@Html.AntiForgeryToken()`, `<input type="hidden" asp-for="IDSanPham" />`, current image preview with fallback, and image replacement upload.

3. **Navigation Integration**:
   - `_AdminLayout.cshtml`: Contains `<a href="/CuaHang/AdminSanPham" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")">` with `<i class="fas fa-boxes"></i>` and text `Sản phẩm Gây Quỹ`.
   - `Views/Admin/Index.cshtml`: Contains dashboard quick-link tile for "Sản phẩm Gây Quỹ" linking to `/CuaHang/AdminSanPham`.

4. **Independent Build Output**:
   ```
   Command: XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   WEB_TinhNguyen -> /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/bin/Debug/net10.0/WEB_TinhNguyen.dll
   Build succeeded.
       2 Warning(s) [NU1900 offline vulnerability feed advisory]
       0 Error(s)
   ```

5. **Empirical Test Suite Execution**:
   - Features 12 to 16:
     - `F12`: 5 Passed | 0 Failed
     - `F13`: 5 Passed | 0 Failed
     - `F14`: 5 Passed | 0 Failed
     - `F15`: 5 Passed | 0 Failed
     - `F16`: 5 Passed | 0 Failed
   - Tier 4 Admin Scenarios (S07 to S12): 6 Passed | 0 Failed
   - Tier 5 Adversarial Stress Tests: 66 Passed | 0 Failed

---

## 2. Logic Chain

1. **Absence of Hardcoding / Test Collusion**:
   - Grep searches for test identifiers (`F12`, `F16`, `Tier`, `[PASS]`) yielded 0 matches within `Areas/CuaHang`.
   - All tests interrogate real properties, reflection signatures, DOM structures, and file upload behavior.
   - The implementations do not branch on test runner environment variables or inject prefabricated test assertions.

2. **Authentic Business Logic**:
   - File uploads genuinely save to disk (`FileStream`) using GUIDs to eliminate path traversal.
   - Deletions genuinely remove items from `_context.SanPhamGayQuys` and `DbInitializer` in-memory collections.
   - Entity mapping (`ToEntity`, `UpdateEntity`, `FromEntity`) trims strings, applies default values, and handles image persistence correctly.

3. **Resilience & Security**:
   - Both online SQL Server execution and offline sandbox execution are supported deterministically via `_context.Database.CanConnect()` detection.
   - All POST endpoints are protected by anti-forgery tokens.
   - All user inputs are validated with DataAnnotations (`[Required]`, `[Range]`, `[StringLength]`).

---

## 3. Caveats

- **No Caveats**: The audited work product implements 100% of Milestone 2 requirements without facades, test hardcoding, or external code plagiarism.

---

## 4. Conclusion

- **Verdict**: **CLEAN**
- Milestone 2 work products fulfill all acceptance criteria and integrity constraints under Demo Mode.
- Ready to proceed to Milestone 3 (Storefront, Cart & Customer Navigation).

---

## 5. Verification Method

To independently reproduce this forensic audit:

1. **Verify Build**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
   ```

2. **Run Milestone 2 Feature Tests**:
   ```bash
   for f in 12 13 14 15 16; do
       XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature $f
   done
   ```

3. **Run Admin Scenarios and Stress Harness**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 4
   XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 5
   ```
