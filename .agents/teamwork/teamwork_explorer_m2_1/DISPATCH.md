# Dispatch Task for M2 Explorer 1 (Admin Controller Design)

## Objective
Design the complete implementation specification and code for:
`WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs`

## Key Requirements
1. Controller attribute: `[Area("CuaHang")]`.
2. Dependency Injection: `ApplicationDbContext _context`, `IWebHostEnvironment _env`.
3. Actions to specify with full logic:
   - `Index`: Returns list of products (`List<SanPhamGayQuy>`). If database connection fails (offline sandbox mode), smoothly fall back to `DbInitializer.GetInMemoryProducts()`.
   - `Create` (GET): Returns Create view with `SanPhamCrudViewModel`.
   - `Create` (POST): Validates model, handles file upload (`IFormFile? FileHinhAnh`) to `wwwroot/uploads/sanpham/` with unique filename (Guid), saves via `model.ToEntity(imagePath)`. If DB fails, uses `DbInitializer.AddInMemoryProduct()`. Sets `TempData["Success"]`, redirects to `Index`.
   - `Edit` (GET, int id): Retrieves product by id (from DB or `DbInitializer.GetInMemoryProductById(id)`), maps via `SanPhamCrudViewModel.FromEntity()`, returns Edit view.
   - `Edit` (POST, int id, SanPhamCrudViewModel model): Validates, uploads new image if provided, updates entity via `model.UpdateEntity(entity, imagePath)`, saves. Sets `TempData["Success"]`, redirects to `Index`.
   - `Delete` (POST, int id): Finds product, handles Foreign Key constraint (if ordered, can mark `TrangThai = false` or catch `DbUpdateException`), deletes from DB or `DbInitializer.DeleteInMemoryProduct(id)`. Sets `TempData["Success"]`, redirects to `Index`.
4. Anti-forgery tokens on all POST actions: `[HttpPost]`, `[ValidateAntiForgeryToken]`.

## Output
Write comprehensive handoff report to:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/handoff.md`.


## 2026-11-03T13:21:18Z
You are teamwork_explorer_m2_1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/DISPATCH.md
4. Provide the exact implementation specification and complete code for `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs` (Index, Create GET/POST, Edit GET/POST, Delete POST, anti-forgery, upload handling, and offline fallback via DbInitializer).
5. Write your handoff report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/handoff.md.
6. Send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.
