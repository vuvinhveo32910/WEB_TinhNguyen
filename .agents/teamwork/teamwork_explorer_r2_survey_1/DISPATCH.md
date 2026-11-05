# Dispatch Task for Explorer R2 Survey 1: Backend & Domain Logic Audit

## Role
Teamwork Explorer (Survey 1)

## Assigned Work
Examine the existing backend implementation in `WEB_TinhNguyen`:
1. Controllers in `Areas/CuaHang/Controllers/` (`CuaHangController.cs`, `GioHangController.cs`, `AdminSanPhamController.cs`).
2. Entities and ViewModels in `Areas/CuaHang/Models/` (`SanPhamGayQuy.cs`, `DonHangGayQuy.cs`, `ChiTietDonHangGayQuy.cs`, ViewModels).
3. Database context in `Data/ApplicationDbContext.cs` and helpers in `Areas/CuaHang/Helpers/` (`DbInitializer.cs`, `SessionExtensions.cs`).
4. Read `.agents/teamwork/ORIGINAL_REQUEST.md`.

## Specific Questions to Answer:
1. Are orders (`DonHangGayQuy`) currently saved to DB/in-memory fallback when a user checks out? What happens to product stock quantity (`SoLuongTon`) and sold count (`DaBan`)?
2. Is there an Admin Order Management controller / view (`AdminDonHangController` or similar) to view orders, check order details, filter by status, or update order status (e.g. Chờ xác nhận, Đã thanh toán, Đang giao hàng, Hoàn thành, Đã hủy)?
3. What fields exist on `DonHangGayQuy` (e.g., TrangThai, TienUngHo, PhuongThucThanhToan, NgayDat)? Can an admin view total funds raised vs total product revenue?
4. What error handling or validation improvements are needed (e.g., checkout validation, negative quantity, stock checks, concurrency/thread-safety)?

## Output Requirement
Write your findings to `.agents/teamwork/teamwork_explorer_r2_survey_1/handoff.md`.
Include: Observation, Logic Chain, Unfinished/Missing Features, Recommended Improvements, Verification.

## 2026-11-04T00:25:48Z
You are teamwork_explorer_r2_survey_1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1
Authoritative user request record: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Your task dispatch instructions are in: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1/DISPATCH.md

You MUST read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before starting work.

Your objective:
Conduct an in-depth audit of the backend, domain logic, data persistence, and models in `Areas/CuaHang/` and `Data/ApplicationDbContext.cs`.
Check:
1. `Areas/CuaHang/Controllers/` (`CuaHangController.cs`, `GioHangController.cs`, `AdminSanPhamController.cs`).
2. `Areas/CuaHang/Models/` (`SanPhamGayQuy.cs`, `DonHangGayQuy.cs`, `ChiTietDonHangGayQuy.cs`, ViewModels).
3. `Areas/CuaHang/Helpers/` (`DbInitializer.cs`, `SessionExtensions.cs`).
4. Current handling of orders, stock updates, donation tracking, and whether Admin currently has an order management controller/view.
5. Identify all missing, unfinished, or improvable backend capabilities to support Admin order/donation tracking and enhanced storefront features.

Write your complete findings and report to:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_1/handoff.md`.
Send a completion message back to the orchestrator with your findings summary.
