# Challenger 2 Dispatch: Milestone M2 (Kiểm chứng biên và kiểm thử hồi quy phân hệ bán hàng)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_2/`

## Nhiệm vụ Thử nghiệm Đối kháng:
1. **Kiểm tra hồi quy tuyệt đối cho Phân hệ Bán hàng (`Areas/CuaHang/`)**:
   - Dùng `git status` và `git diff` kiểm tra xem có bất kỳ thay đổi nào bên trong `Areas/CuaHang/` không (kỳ vọng: 0 thay đổi).
   - Kiểm tra các endpoint bán hàng: Xem sản phẩm, Giỏ hàng, Đặt hàng, Admin Sản phẩm xem có bị ảnh hưởng bởi việc đổi tên bảng/cột hay xóa cột màu không.
2. **Kiểm tra khả năng biên dịch và vận hành khi không có DB vật lý (In-Memory fallback)**:
   - Xác minh `DbInitializer` vẫn seed dữ liệu mẫu thành công khi `context.Database.CanConnect()` trả về false hoặc offline.
3. **Thực thi bộ test Tier 5 & stress tests**:
   - Chạy test suite và kiểm tra toàn bộ test cases liên quan đến CuaHang, Admin, và User endpoints.

Ghi rõ kết luận: **APPROVE** hoặc **REJECT** kèm dữ liệu thực nghiệm vào `handoff.md`.

## 2026-11-04T06:50:15Z
Bạn là Challenger 2 cho Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn).

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_2/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_2/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`

Nhiệm vụ:
- Kiểm tra đối kháng hồi quy tuyệt đối cho Phân hệ Bán hàng (`Areas/CuaHang/`): kiểm tra git diff/status, kiểm tra tính toàn vẹn của mô hình sản phẩm, giỏ hàng, đặt hàng.
- Kiểm tra tính ổn định của In-Memory fallback trong `DbInitializer`.
- Chạy stress tests và test suite Tier 5.
- Viết `handoff.md` kết luận APPROVE hoặc REJECT kèm bằng chứng thực nghiệm, và gửi tin nhắn báo cáo cho Orchestrator.
