# Dispatch Task for M2 Explorer 2 (Admin Views Design)

## Objective
Design the complete Razor views specification and code for Admin Product Management:
1. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`
2. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml`
3. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml`

## Key Requirements
1. Conform to `_AdminLayout.cshtml` styling (dark navy sidebar, white card containers with soft shadow, rounded corners `rounded-2xl`, emerald button accents, FontAwesome icons, Be Vietnam Pro font).
2. `Index.cshtml`: Responsive table with columns:
   - Ảnh sản phẩm (thumbnail)
   - Tên sản phẩm & Danh mục
   - Giá bán & Giá gốc
   - Số lượng tồn kho & Đã bán
   - Tỷ lệ gây quỹ (%) & Thông điệp
   - Trạng thái hiển thị (Active/Inactive badge)
   - Thao tác: Nút "Thêm sản phẩm mới" nổi bật, Nút Sửa dẫn đến `Edit`, Nút Xóa với POST form và JavaScript `confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?')`.
   - Hiển thị thông báo `TempData["Success"]` và `TempData["Error"]`.
3. `Create.cshtml` & `Edit.cshtml`:
   - `multipart/form-data` form.
   - All fields from `SanPhamCrudViewModel` with validation tags (`asp-validation-for`).
   - Image upload input (`FileHinhAnh`) and direct link input (`LinkHinhAnh`).
   - Thumbnail preview of current image in `Edit.cshtml`.
   - Action buttons: "Lưu sản phẩm" (emerald) and "Hủy bỏ / Quay lại" (gray).

## Output
Write comprehensive handoff report to:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/handoff.md`.


## 2026-11-03T13:21:27Z
You are teamwork_explorer_m2_2.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/DISPATCH.md
4. Provide the exact Razor code for:
   - WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml
   - WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml
   - WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml
   matching `_AdminLayout.cshtml` styling (dark navy card, emerald accents, table responsive, validation).
5. Write your handoff report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/handoff.md.
6. Send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.
