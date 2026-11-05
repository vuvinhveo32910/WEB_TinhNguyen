# BRIEFING — 2026-11-03T13:28:00Z

## Mission
Design and provide complete specification and code for `WEB_TinhNguyen/Areas/CuaHang/Controllers/AdminSanPhamController.cs` with full CRUD, upload handling, anti-forgery, and offline fallback.

## 🔒 My Identity
- Archetype: explorer
- Roles: Teamwork explorer (Read-only investigation: analyze problems, synthesize findings, produce structured reports)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1
- Original parent: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Milestone: M2 (Admin Product Management)

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Scope limited to AdminSanPhamController design and handoff
- Must be compatible with E2ETests (Tier 1-5) and existing M1 foundation

## Current Parent
- Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe
- Updated: 2026-11-03T13:28:00Z

## Investigation State
- **Explored paths**:
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`
  - `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`
  - `WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`
  - `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`
  - `tests/E2ETests/Tier1/Tier1_AdminTests.cs`
  - `tests/E2ETests/Tier3/Tier3_CrossFeatureTests.cs`
  - `tests/E2ETests/Tier4/Tier4_RealWorldScenarioTests.cs`
  - `tests/E2ETests/Tier5/Tier5_ChallengerTests.cs`
  - `WEB_TinhNguyen/Controllers/AdminTinTucController.cs`
  - `WEB_TinhNguyen/Controllers/AdminGayQuyController.cs`
- **Key findings**:
  - `Tier1_AdminTests` uses reflection `ctrlType.GetMethod("Delete")` without arguments, which requires `Delete` to have no method overloads.
  - Delete must take an `id` parameter.
  - File upload path should be `wwwroot/uploads/sanpham/` with fallback to `Directory.GetCurrentDirectory()/wwwroot/uploads/sanpham/` if WebRootPath is null.
  - DbInitializer has `GetInMemoryProducts()`, `GetInMemoryProductById(id)`, `AddInMemoryProduct(p)`, `UpdateInMemoryProduct(p)`, `DeleteInMemoryProduct(id)`.
  - Database fallback pattern: `try { ... } catch (Exception ex) { ... }` or `context.Database.CanConnect()` gracefully using `DbInitializer`.
- **Unexplored areas**: None.

## Key Decisions Made
- Provided complete production-grade C# implementation of `AdminSanPhamController.cs` in proposal artifact and handoff report.
- Designed `Delete(int id)` as single POST action decorated with `[HttpPost]`, `[ValidateAntiForgeryToken]` to satisfy reflection check in `F16_T01` and `F16_T02`.
- Robust offline fallback checking both DB connection and exceptions (e.g. timeout, missing tables, DbUpdateException).
- Completed `handoff.md` following 5-component report protocol.

## Artifact Index
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/handoff.md` — Complete handoff report with exact controller code, analysis, and verification instructions.
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_1/proposed_AdminSanPhamController.cs` — The standalone C# controller code ready for worker implementation.
