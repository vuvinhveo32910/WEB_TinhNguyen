# Challenger 1 Dispatch: Milestone M2 (Thử nghiệm đối kháng & Kiểm chứng thực nghiệm)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_1/`

## Nhiệm vụ Thử nghiệm Đối kháng (Adversarial Verification):
1. **Quét tự động toàn bộ codebase tìm kiếm vết tích màu sắc**:
   - Viết hoặc chạy kịch bản quét toàn diện regex/grep trên mọi tệp `.cs`, `.cshtml` trong `WEB_TinhNguyen/` (ngoại trừ `Areas/CuaHang/`):
     - `MauChuDao`, `MauNhan`, `MauChu`, `MauNenStart`, `MauNenMid`, `MauNenEnd`, `MauIcon`, `MauTieuDe`, v.v.
     - `type="color"`, `#([0-9a-fA-F]{3}){1,2}` trong input fields.
   - Báo cáo số lượng vi phạm phát hiện được (kỳ vọng: 0).
2. **Kiểm tra Migration và Snapshot Schema**:
   - Xác minh `ApplicationDbContextModelSnapshot.cs` không còn bất kỳ Property nào liên quan đến cột màu sắc đã bị xóa.
   - Xác minh các bảng `BieuNguTrangChu`, `ThuVienKhoanhKhac`, `CauHoiThuongGapTrangChu` được khai báo chuẩn xác trong Snapshot.
3. **Thực thi và đo lường**:
   - Chạy build và E2E tests, ghi nhận thời gian thực thi và tỷ lệ pass.

Ghi rõ kết luận: **APPROVE** hoặc **REJECT** kèm dữ liệu thực nghiệm vào `handoff.md`.

## 2026-11-04T06:50:09Z
Bạn là Challenger 1 cho Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn).

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r3_m2_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`

Nhiệm vụ:
- Quét đối kháng toàn bộ codebase tìm kiếm vết tích màu sắc (MauChu, MauNen, type="color", v.v.).
- Kiểm tra Migration và Snapshot Schema độc lập.
- Chạy thực nghiệm build và test suite, đo lường kết quả.
- Viết `handoff.md` kết luận APPROVE hoặc REJECT kèm bằng chứng thực nghiệm, và gửi tin nhắn báo cáo cho Orchestrator.
