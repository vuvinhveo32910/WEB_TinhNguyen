## 2026-11-04T08:08:19Z
BẠN LÀ FINAL CHALLENGER 2 (ADVERSARIAL STRESS & EDGE-CASE VERIFIER).
Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m5_2/
Path to ORIGINAL_REQUEST.md: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Path to PROJECT.md: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md

NHIỆM VỤ:
Tiến hành kiểm thử đối kháng (Adversarial stress-testing) trên toàn bộ hệ thống sau tái cấu trúc:
1. Thử nghiệm rò rỉ Dark mode vào Cửa Hàng qua mọi biến thể URL (`/cuahang`, `/CUAHANG`, `/CuaHang/Index`, query params, hash).
2. Thử nghiệm chuyển tiếp qua lại giữa Trang chủ (Dark mode) và Cửa hàng (Light mode cưỡng chế) xem trạng thái localStorage có được khôi phục chuẩn xác.
3. Thử nghiệm trình duyệt chặn localStorage (Private Mode) xem có crash trang không.
4. Quét tĩnh toàn bộ mã nguồn xác nhận: 0 thuộc tính màu trong MoHinh, 0 input màu trong GiaoDien/Admin*, 0 file diff trong Areas/CuaHang/Views/.
5. Viết `handoff.md` chi tiết tại thư mục làm việc của bạn và báo cáo kết luận: **APPROVE** hoặc **REQUEST_CHANGES**.
