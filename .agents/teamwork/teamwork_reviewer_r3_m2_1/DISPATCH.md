# Reviewer 1 Dispatch: Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn)

## Đường dẫn tài liệu quan trọng
- Yêu cầu gốc (BẮT BUỘC ĐỌC): `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Kế hoạch dự án: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- Báo cáo của Worker: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`
- Thư mục làm việc: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_1/`

## Trọng tâm Review:
1. **Kiểm tra xóa 66 thuộc tính màu**:
   - `WEB_TinhNguyen/MoHinh/Entities.cs` và `CauHinhTrangChuVM.cs`: xác nhận sạch 100% trường màu (`MauChu`, `MauNen`, `MauChuDao`, `MauNhan`,...).
2. **Kiểm tra thuần hóa tiếng Việt**:
   - Tên bảng: `BieuNguTrangChu`, `ThuVienKhoanhKhac`, `CauHoiThuongGapTrangChu`.
   - Các cột: `IDBieuNgu`, `TieuDeThayThe`, `DuongDanAnh`, `DoTreHoatHoa`, `ViTriTren`, `KeuGoi_*`, `NoiBat_*`.
3. **Kiểm tra EF Core Migration & DbContext**:
   - File `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` có đầy đủ `DropColumn`, `RenameTable`, `RenameColumn`.
   - `ApplicationDbContextModelSnapshot.cs` đã được cập nhật đồng bộ.
4. **Kiểm tra Controllers & Views**:
   - Các câu truy vấn LINQ trong `BoDieuKhien/` không còn gán hay đọc thuộc tính màu.
   - Các form Edit trong `GiaoDien/Admin*` đã xóa sạch `<input type="color">`.
5. **Bảo toàn 100% phân hệ bán hàng**:
   - `WEB_TinhNguyen/Areas/CuaHang/` không có bất kỳ thay đổi nào làm ảnh hưởng đến hoạt động và giao diện.
6. **Xác minh thực thi**:
   - Chạy lệnh build: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
   - Chạy E2E tests: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`

Ghi rõ kết luận: **APPROVE** hoặc **REQUEST_CHANGES** kèm bằng chứng chi tiết vào `handoff.md`.


## 2026-11-04T06:49:51Z
Bạn là Reviewer 1 cho Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn).

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_1/`
Đọc kỹ:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r3_m2_1/DISPATCH.md`
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md`

Nhiệm vụ:
- Kiểm tra việc xóa 66 thuộc tính màu trong `MoHinh/Entities.cs` và `CauHinhTrangChuVM.cs`.
- Kiểm tra thuần hóa tiếng Việt tên bảng/cột.
- Kiểm tra Migration mới và DbContext.
- Kiểm tra LINQ queries và loại bỏ input type="color" khỏi Admin views.
- Kiểm tra bảo toàn 100% `Areas/CuaHang/`.
- Chạy `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
- Chạy `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj`
- Viết `handoff.md` kết luận rõ APPROVE hoặc REQUEST_CHANGES kèm bằng chứng chi tiết, và gửi tin nhắn báo cáo cho Orchestrator.
