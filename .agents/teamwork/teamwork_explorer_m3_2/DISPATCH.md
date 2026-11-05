## 2026-11-03T13:59:38Z
You are Explorer M3.2 (Cart & Checkout Controller & Views) for Milestone 3.
Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m3_2
Your parent is Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY: Read the original user request at:
/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Also read the project architecture at:
/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md

Investigate and design the Shopping Cart & Checkout module:
1. `Areas/CuaHang/Controllers/GioHangController.cs`:
   - `[Area("CuaHang")]`
   - Session storage via `HttpContext.Session.Get<List<CartItem>>("GioHangGayQuy")` and `Set(...)`.
   - Action `Index()`: displays `CartViewModel` (list of items, subtotal, total items, donation summary) and pre-populates `CheckoutViewModel`.
   - Action `ThemVaoGio(int id, int soLuong = 1)` [HttpPost / HttpGet]: adds item or increases quantity, validates stock. Returns redirect to Cart or JSON for AJAX.
   - Action `CapNhatSoLuong(int id, int soLuong)` [HttpPost]: updates item quantity; if <= 0 removes item.
   - Action `XoaKhoiGio(int id)` [HttpPost]: removes item from cart session.
   - Action `XoaGioHang()` [HttpPost]: empties cart session.
   - Action `DatHang(CheckoutViewModel model)` [HttpPost, ValidateAntiForgeryToken]: validates donor details (`HoTen`, `SoDienThoai`, `Email`, `DiaChi`), calculates total (`TongTienHang + TienUngHoThem`), creates `DonHangGayQuy` with code (e.g. `GQ-20261103-XXXX`), creates `ChiTietDonHangGayQuy` entries, saves to DB (or in-memory store fallback), clears cart session, redirects to `DatHangThanhCong(int id)`.
   - Action `DatHangThanhCong(int id)`: retrieves order, displays confirmation and VietQR.
2. `Areas/CuaHang/Views/GioHang/Index.cshtml`:
   - Cart item list with thumbnail, name, unit price, quantity controls (+ / - / input), line total, remove button.
   - Empty cart state with illustration and "Khám phá sản phẩm gây quỹ" button linking to `/CuaHang`.
   - Checkout form: Donor name, phone, email, delivery address, notes, extra donation input (`TienUngHoThem`), payment method options (COD vs VietQR bank transfer).
3. `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`:
   - Order success card with `MaDonHang`, donor info, summary of items and donation amount.
   - Dynamic VietQR generator using MBBank `0924044283` (`https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={amount}&addInfo={addInfo}&accountName=NGUYEN%20THAI%20VINH`).
4. Inspect `tests/E2ETests/` (Features 9, 10, 11 and Scenarios S03-S06) to ensure the design satisfies all test assertions.

Write your handoff report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m3_2/handoff.md` with:
1. Observation
2. Logic Chain
3. Caveats
4. Conclusion (Concrete implementation recommendations)
5. Verification Method

Remember: You are read-only. Do NOT edit C# source files or Razor views.
Send a message back when complete.
