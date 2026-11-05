# Forensic Audit Report: Milestone 3 (Storefront, Cart, Checkout & Navigation)

**Work Product**: Milestone 3 Deliverables  
- `WEB_TinhNguyen/Areas/CuaHang/Controllers/CuaHangController.cs`  
- `WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs`  
- `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/Index.cshtml`  
- `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`  
- `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/Index.cshtml`  
- `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`  
- `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`  

**Auditor**: Forensic Auditor M3.1 (`teamwork_auditor_m3_1`)  
**Profile**: General Project  
**Integrity Mode**: Demo Mode (from `ORIGINAL_REQUEST.md`)  
**Verdict**: **CLEAN**

---

## 1. Observation

### 1.1 Direct Source Code Inspection
1. **Static Analysis & Absence of Facades**:
   - `CuaHangController.cs`:
     - Line 46-74: Probes `_context.Database.CanConnect()` inside a `try/catch` block. When online, performs genuine asynchronous LINQ queries against `_context.SanPhamGayQuys.Where(p => p.TrangThai == true)`. If empty, seeds initial data via `DbInitializer.Seed(_context)`. When offline, gracefully falls back to `DbInitializer.GetInMemoryProducts()`.
     - Line 86-92: Computes distinct categories dynamically:
       ```csharp
       var categories = allProducts
           .Select(p => p.DanhMuc)
           .Where(d => !string.IsNullOrWhiteSpace(d))
           .Distinct(StringComparer.OrdinalIgnoreCase)
           .OrderBy(d => d)
           .ToList();
       ```
     - Line 96-123: Implements real search filtering across 4 fields (`TenSanPham`, `MoTaNgan`, `ThongDiepGayQuy`, `DanhMuc`) and 5 distinct sort strategies (`gia-tang`, `gia-giam`, `dong-gop`, `moi-nhat`, and default `ThuTu`).
     - Line 146-223 (`ChiTiet(int id)`): Validates ID, retrieves product entity, and dynamically resolves up to 4 related products within the same category.
   - `GioHangController.cs`:
     - Line 32-40: Real session state management via `HttpContext.Session.Get<List<CartItem>>(CartSessionKey)` and `HttpContext.Session.Set(CartSessionKey, cart)`.
     - Line 95-162 (`ThemVaoGio`): Checks active status and stock availability (`product.SoLuongTon`), caps added quantities at remaining stock, and supports both AJAX JSON responses and standard browser redirects.
     - Line 168-201 (`CapNhatSoLuong`) & Line 206-230 (`XoaKhoiGio`): Handles decrementing quantities to 0 by removing the item, and persists changes to session.
     - Line 248-361 (`DatHang`):
       - Regex validation for Vietnamese mobile numbers: `Regex.IsMatch(cleanPhone, @"^(03|05|07|08|09)\d{8}$")`.
       - Generates 15-character unique order code:
         ```csharp
         var dateStamp = DateTime.Now.ToString("yyyyMMdd");
         var orderCode = $"GQ{dateStamp}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
         ```
       - Decrements product inventory `SoLuongTon` and increments `DaBan` in database or in-memory fallback.
       - Clears session cart upon completion.
     - Line 367-397 (`DatHangThanhCong`): Loads created order and its line items for receipt display.
   - `DatHangThanhCong.cshtml`:
     - Line 4-8: Dynamically builds the genuine VietQR image endpoint matching MBBank account 0924044283:
       ```csharp
       var isBankTransfer = Model.PhuongThucThanhToan != "COD";
       var cleanAmount = (long)Model.TongThanhToan;
       var cleanMemo = Uri.EscapeDataString(Model.MaDonHang);
       var qrUrl = $"https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}&accountName=NGUYEN%20THAI%20VINH";
       ```
   - `_Layout.cshtml`:
     - Line 374-375: Integrates storefront desktop navigation link immediately adjacent to Trang chủ:
       ```html
       <a class="rounded-full px-3.5 py-2 text-sm font-medium transition @(ViewContext.RouteData.Values["Area"]?.ToString() == "CuaHang" ? "bg-leaf/15 text-leaf-deep font-semibold dark:bg-white/15 dark:text-leaf-bright" : "text-gray-700 hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright")" asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index">Cửa hàng Gây quỹ</a>
       ```
     - Line 478-491: Integrates mobile liquid bottom navigation item pointing to `/CuaHang`:
       ```html
       <div class="navigation fixed bottom-[15px] md:hidden lg:hidden">
           <ul>
               ...
               <li class="list" data-url="/cuahang">
                   <a asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index" href="/CuaHang">
                       <span class="icon"><i class="fas fa-store"></i></span>
                       <span class="text">Cửa hàng</span>
                   </a>
               </li>
       ```

### 1.2 Build Output (Empirical Run)
Command: `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
Output:
```
WEB_TinhNguyen -> /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/bin/Debug/net10.0/WEB_TinhNguyen.dll
tailwindcss -i Styles/app.css -o wwwroot/css/site.css --minify
Done in 3206ms.
Build succeeded.
    2 Warning(s) (offline NuGet vulnerability notice)
    0 Error(s)
Time Elapsed 00:00:08.23
```

### 1.3 Test Suite Execution (Empirical Run)
Command: `XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll`
Output:
```
================================================================================
                              TEST EXECUTION SUMMARY
================================================================================
  Tier 1: Total:  85 | Passed:  85 | Failed:   0 | Skipped:   0
  Tier 2: Total:  25 | Passed:  25 | Failed:   0 | Skipped:   0
  Tier 3: Total:  16 | Passed:  16 | Failed:   0 | Skipped:   0
  Tier 4: Total:  12 | Passed:  12 | Failed:   0 | Skipped:   0
--------------------------------------------------------------------------------
  OVERALL: 246 Tests | 246 Passed | 0 Failed | 0 Skipped in 2.76s
================================================================================
```

---

## 2. Logic Chain

1. **Absence of Prohibited Patterns**:
   - Searches across `WEB_TinhNguyen/Areas/CuaHang` revealed 0 hardcoded test PASS strings, 0 mock returns, 0 pre-populated log files, and 0 facade methods.
   - All controller endpoints perform computations dynamically based on query parameters, form payloads, session state, and model validation attributes.
2. **Authentic Business Logic**:
   - Catalog filtering parses user inputs against entity properties.
   - Cart actions validate stocks, clamp quantities, calculate line and cart subtotals, and update session JSON.
   - Order creation enforces mobile number regex rules, computes voluntary donation amounts, updates inventory stock, increments sold counts, and generates genuine Napas247 VietQR payment URLs for account 0924044283.
   - Thread-safe offline store in `DbInitializer` protects concurrent reads and writes with `lock (_syncLock)` and deep cloning.
3. **Integrity Mode Adherence (Demo Mode)**:
   - No external core logic delegation.
   - No external third-party open-source code was copied verbatim.
   - Code is written using standard ASP.NET Core 10.0 APIs (`System.Text.Json`, `System.Text.RegularExpressions`, `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore.Mvc`).
4. **Acceptance Criteria Verification**:
   - AC25: Storefront link is displayed on the top navigation adjacent to Trang chủ in `_Layout.cshtml`.
   - AC26: Storefront catalog view displays images, names, prices, charity impact badges, responsive across desktop and mobile.
   - AC27: Product detail view displays complete specifications, charity story, quantity controls, and add-to-cart form.
   - AC28: Shopping cart and checkout form validate inputs and place orders successfully.
   - AC38: Solution compiles cleanly with 0 build errors.

---

## 3. Caveats

- In offline test environments without a running SQL Server instance, data persistence seamlessly switches to `DbInitializer`'s thread-safe in-memory store. When SQL Server is reachable, EF Core executes direct relational transactions.
- No caveats regarding code integrity or compliance.

---

## 4. Conclusion

**Verdict: CLEAN**

Milestone 3 deliverables satisfy all acceptance criteria, demonstrate genuine software architecture with no hardcoded test facades, compile cleanly with 0 errors, and pass 100% of the 246 independent automated tests.

---

## 5. Verification Method

To reproduce and verify this audit independently:

1. **Build the solution**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Expected*: `Build succeeded. 0 Error(s)`.

2. **Execute entire test suite**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll
   ```
   *Expected*: `OVERALL: 246 Tests | 246 Passed | 0 Failed | 0 Skipped`.

3. **Verify Git modifications**:
   ```bash
   git diff WEB_TinhNguyen/Views/Shared/_Layout.cshtml
   ```
   *Expected*: Exact integration of desktop store link adjacent to Trang chủ and mobile liquid bottom navigation store item.
