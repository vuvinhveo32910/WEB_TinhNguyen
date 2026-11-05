# BRIEFING — 2026-11-04T00:32:00Z

## Mission
Conduct an in-depth audit of the backend, domain logic, data persistence, and models in `Areas/CuaHang/` and `Data/ApplicationDbContext.cs` to identify missing, unfinished, or improvable backend capabilities.

## 🔒 My Identity
- Archetype: explorer
- Roles: survey, backend audit, synthesis
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1
- Original parent: 539c754b-e018-4f42-a411-9b35533a184a
- Milestone: Survey & Gap Analysis Round 2

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Scope: Backend, domain logic, data persistence, and models in `Areas/CuaHang/` and `Data/ApplicationDbContext.cs`
- Deliver findings in `handoff.md` and notify parent via `send_message`

## Current Parent
- Conversation ID: 539c754b-e018-4f42-a411-9b35533a184a
- Updated: 2026-11-04T00:32:00Z

## Investigation State
- **Explored paths**:
  - `Data/ApplicationDbContext.cs`
  - `Areas/CuaHang/Models/` (`SanPhamGayQuy.cs`, `DonHangGayQuy.cs`, `ChiTietDonHangGayQuy.cs`, `CartItem.cs`, `CartViewModel.cs`, `CheckoutViewModel.cs`, `SanPhamCrudViewModel.cs`)
  - `Areas/CuaHang/Helpers/` (`DbInitializer.cs`, `SessionExtensions.cs`)
  - `Areas/CuaHang/Controllers/` (`CuaHangController.cs`, `GioHangController.cs`, `AdminSanPhamController.cs`)
  - `Views/Shared/_AdminLayout.cshtml`, `Views/Admin/Index.cshtml`, `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`
  - `Migrations/`
- **Key findings**:
  1. `DonHangGayQuy` is saved to DB (when reachable) or `DbInitializer._inMemoryOrders` (when offline); stock deduction logic clamps `SoLuongTon` to 0 without pre-check.
  2. NO Admin Order Management controller (`AdminDonHangController`) or views exist; admin cannot view, filter, or manage orders.
  3. `DonHangGayQuy` and `ChiTietDonHangGayQuy` contain necessary price, donation percentage, and extra donation fields, but no aggregation or admin dashboard displays donation vs sales revenue.
  4. Stock validation pre-check missing at checkout; negative or zero stock handling in cart updates can be bypassed; in-memory fallback lacks `GetInMemoryOrders()` and status update methods; no EF Core migration exists for CuaHang tables.
- **Unexplored areas**: None within backend/domain scope.

## Key Decisions Made
- Structured findings into clear evidence-based observations with exact file and line references.
- Identified concrete recommendations for Admin Order Management, inventory validation, and donation reporting.

## Artifact Index
- DISPATCH.md — Task dispatch and instructions
- BRIEFING.md — Situational awareness and working memory
- progress.md — Liveness heartbeat and step tracking
- handoff.md — 5-component handoff report
