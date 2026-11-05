# Challenger 2 Dispatch: Milestone M4 (Kiểm chứng Đối kháng Cô lập Cửa hàng & Anti-FOUC)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m4_2/`

## Nhiệm vụ Thử nghiệm Đối kháng:
1. **Kiểm tra Đối kháng Cô lập Cửa hàng**:
   - Thử nghiệm mô phỏng người dùng đang ở chế độ Tối (`site_theme = dark`, thẻ html có class `dark`), sau đó điều hướng vào `/CuaHang/Index` hoặc `/CuaHang/GioHang`:
     - Kiểm tra xem script Anti-FOUC và Layout Guard có tự động tước bỏ class `dark` và ép class `light` không.
     - Kiểm tra xem nút toggle có bị ẩn hoàn toàn trong layout Cửa hàng không.
2. **Kiểm tra Stress Test và Tính Nguyên vẹn CuaHang**:
   - Chạy bộ test `M3Challenger2` (128 tests).
   - Kiểm tra `git diff` toàn bộ thư mục `Areas/CuaHang/Views/`: 0 thay đổi.
3. **Thực thi build và test độc lập**:
   - Ghi nhận kết quả test suite.

Ghi rõ kết luận: **APPROVE** hoặc **REJECT** kèm dữ liệu thực nghiệm vào `handoff.md`.

## 2026-11-04T07:57:47Z
Bạn là Challenger 2 cho Milestone M4 (Requirement R4: Kiểm chứng Đối kháng Cô lập Cửa hàng & Anti-FOUC).

Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m4_2/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m4_2/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m4/handoff.md`

Nhiệm vụ:
- Kiểm tra đối kháng kịch bản người dùng đang bật Dark mode rồi duyệt vào `/CuaHang/Index` hay `/CuaHang/GioHang`: xác nhận class `dark` bị tước bỏ, class `light` được nạp, nút toggle bị ẩn.
- Chạy stress test bộ `M3Challenger2` (128 tests).
- Kiểm tra `git diff Areas/CuaHang/Views/`: 0 thay đổi.
- Viết `handoff.md` kết luận APPROVE hoặc REJECT kèm dữ liệu thực nghiệm, và gửi tin nhắn báo cáo cho Orchestrator.
