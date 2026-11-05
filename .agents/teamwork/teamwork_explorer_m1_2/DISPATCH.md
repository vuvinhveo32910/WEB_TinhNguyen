# Dispatch Task for M1 Explorer 2 (Entities & ViewModels)

## Objective
Analyze and specify exact implementation details for:
1. `Areas/CuaHang/Models/SanPhamGayQuy.cs`: Complete entity with properties, validation, column types, and navigation properties.
2. `Areas/CuaHang/Models/DonHangGayQuy.cs`: Complete entity for orders, recipient info, payment methods, extra donation, and order items.
3. `Areas/CuaHang/Models/ChiTietDonHangGayQuy.cs`: Complete entity for order line items.
4. `Areas/CuaHang/Models/CartItem.cs`, `CartViewModel.cs`, `CheckoutViewModel.cs`, `SanPhamCrudViewModel.cs`: Complete ViewModels for storefront cart, checkout, and admin CRUD.

## Input Files
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

## Output
Write detailed design to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2/handoff.md`.


## 2026-11-03T12:43:38Z
You are teamwork_explorer_m1_2.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2/DISPATCH.md
4. Update your progress.md regularly with "Last visited: [timestamp]" as your heartbeat.
5. Provide exact implementation designs for SanPhamGayQuy.cs, DonHangGayQuy.cs, ChiTietDonHangGayQuy.cs, and ViewModels (CartItem, CartViewModel, CheckoutViewModel, SanPhamCrudViewModel).
6. Write your report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_2/handoff.md following the Handoff Protocol.
7. When finished, send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).
