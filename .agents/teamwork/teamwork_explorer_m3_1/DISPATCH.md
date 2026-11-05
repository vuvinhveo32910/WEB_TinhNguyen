## 2026-11-03T13:59:37Z
You are Explorer M3.1 (Storefront Controller & Views) for Milestone 3.
Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m3_1
Your parent is Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY: Read the original user request at:
/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Also read the project architecture at:
/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md

Investigate and design the Storefront Catalog and Product Detail implementation:
1. `Areas/CuaHang/Controllers/CuaHangController.cs`:
   - `[Area("CuaHang")]`
   - Action `Index(string? q, string? danhMuc, string? sort)`: filter active products (`TrangThai == true`), search by name/description, category filter, sorting (price asc/desc, newest, donation rate). Safe dual-mode querying: try `_context.SanPhamGayQuys`, fallback to `DbInitializer.GetInMemoryProducts()`.
   - Action `ChiTiet(int id)`: get active product by id, handle non-existent id gracefully with redirect or 404, related products list.
2. `Areas/CuaHang/Views/CuaHang/Index.cshtml`:
   - Hero banner with fundraising mission and purpose.
   - Category tabs and search input.
   - Responsive product grid (cards showing thumbnail, name, price, original price if discounted, charity badge `❤️ @item.PhanTramDongGop% Quỹ`, stock badge, "Xem chi tiết" button, quick "Thêm vào giỏ" button).
3. `Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`:
   - Responsive 2-column layout (gallery/image preview, details & purchase panel).
   - Price, charity contribution badge, detailed fundraising message (`ThongDiepGayQuy`), inventory count.
   - Quantity selector with +/- controls and min/max limits.
   - "Thêm vào giỏ hàng" form POST to `/CuaHang/GioHang/ThemVaoGio` with `id` and `soLuong`.
4. Inspect `tests/E2ETests/` (specifically Feature 7 and Feature 8 in Tier 1, and Scenarios S01-S06 in Tier 4) to ensure the design satisfies every test assertion.

Write your handoff report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m3_1/handoff.md` with:
1. Observation
2. Logic Chain
3. Caveats
4. Conclusion (Concrete implementation recommendations)
5. Verification Method

Remember: You are read-only. Do NOT edit C# source files or Razor views.
Send a message back when complete.
