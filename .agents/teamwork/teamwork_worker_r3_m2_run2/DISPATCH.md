# Nhiệm vụ: Triển khai Milestone M2 — Tái cấu trúc CSDL và Dọn dẹp Mã nguồn (Requirement R1)

## Thông tin nhiệm vụ
- **Vai trò**: Lập trình viên C# (.NET 10) & Kiến trúc sư Cơ sở dữ liệu (EF Core)
- **Thư mục làm việc của Agent**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/`
- **Mã định danh nhiệm vụ**: `worker_r3_m2_run2`
- **Yêu cầu gốc**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC đọc trước khi làm)
- **Kế hoạch dự án**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`
- **Bản thiết kế chi tiết (Blueprint của Database Architect)**: Đọc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md`

## CẢNH BÁO TÍNH TOÀN VẸN (MANDATORY INTEGRITY WARNING)
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Phạm vi công việc (Scope)
Thực thi trọn vẹn Yêu cầu R1 theo bản thiết kế chi tiết:
1. **Xóa 66 thuộc tính màu sắc trong Models & ViewModels**:
   - `WEB_TinhNguyen/MoHinh/Entities.cs`: Xóa 66 thuộc tính màu (MauChu, MauNen, MauChuDao, MauNhan, ...) tại 11 class thực thể theo bảng kê mục 1.2 trong blueprint.
   - `WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`: Xóa 66 thuộc tính màu tương ứng trên các class ViewModel.
2. **Chuẩn hóa tiếng Việt cho Tên Bảng & Cột**:
   - `BannerTrangChu` -> `BieuNguTrangChu` (`[Table("BieuNguTrangChu")]`), cột `IDBanner` -> `IDBieuNgu`, `TieuDeAlt` -> `TieuDeThayThe`, `LinkAnh` -> `DuongDanAnh`
   - `TVKhoanhKhac` -> `ThuVienKhoanhKhac` (`[Table("ThuVienKhoanhKhac")]`), cột `LinkAnh` -> `DuongDanAnh`
   - `FAQTrangChu` -> `CauHoiThuongGapTrangChu` (`[Table("CauHoiThuongGapTrangChu")]`), cột `IDFAQ` -> `IDCauHoi`
   - Chuẩn hóa các cột trong `CauHinhAnhNoi`, `CauHinhTrangChu`, `CauHinhBanToChuc`, `CauHinhGayQuy`, `KenhDongGop`, `CauHinhTinTuc`, `BaiVietTinTuc`, `NhaTaiTroGayQuy`, `NhaTaiTroKyUc`, `NhaTaiTroTrangChu`, `ThuVienHieuUng` theo mục 1.4 và Bước 2 trong blueprint.
3. **Cập nhật `ApplicationDbContext.cs`**:
   - Đổi `DbSet<BannerTrangChu>` -> `DbSet<BieuNguTrangChu> BieuNguTrangChus { get; set; }`
   - Thêm `DbSet<ThuVienKhoanhKhac> ThuVienKhoanhKhacs { get; set; }`
   - Thêm `DbSet<CauHoiThuongGapTrangChu> CauHoiThuongGapTrangChus { get; set; }`
   - Xóa mapping cột màu khỏi Fluent API (nếu có).
4. **Tạo Code First Migration mới & Cập nhật Snapshot**:
   - Tạo file migration `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` với `DropColumn` cho 66 cột màu và `RenameTable`/`RenameColumn` cho bảng/cột tiếng Việt (theo code mẫu mục Bước 3 trong blueprint).
   - Tạo file Designer `20261104000000_XoaCotMauVaVietHoaTenBangCot.Designer.cs`.
   - Cập nhật `ApplicationDbContextModelSnapshot.cs` loại bỏ 66 trường màu và cập nhật tên bảng/cột mới.
5. **Cập nhật `Program.cs`**:
   - Trong khối `DbInitializer.Initialize(context)`, bổ sung lệnh `if (context.Database.CanConnect()) { context.Database.Migrate(); }` (bọc trong try/catch an toàn).
6. **Cập nhật LINQ queries và Controllers**:
   - Cập nhật các câu truy vấn LINQ (`.Include(...)`, `Where(...)`) và loại bỏ code đọc/ghi thuộc tính màu trong:
     - `WEB_TinhNguyen/BoDieuKhien/HomeController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminTrangChuController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminChuongTrinhController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminGayQuyController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminBanToChucController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminTinTucController.cs`
     - `WEB_TinhNguyen/BoDieuKhien/AdminKyUcController.cs`
7. **Dọn dẹp Views**:
   - Xóa toàn bộ `<input type="color">` và các input nhập mã màu HEX khỏi các form trong `WEB_TinhNguyen/GiaoDien/Admin*/Edit.cshtml`.
   - Xóa biến màu inline style trong `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml` (dòng 19-22: `bgWeb`, `colorMain`, `colorSub`, `colorText`).
8. **Đồng bộ hóa kiểm thử E2ETests (nếu cần)**:
   - Nếu test suite trong `tests/E2ETests/` có class/trường kiểm tra thuộc tính màu hoặc tên cũ `BannerTrangChu`, cập nhật tương thích để toàn bộ test suite biên dịch và vượt qua.
9. **QUY TẮC BẢO TOÀN TUYỆT ĐỐI**:
   - KHÔNG ĐƯỢC CHẠM VÀO `WEB_TinhNguyen/Areas/CuaHang/` (bảo toàn 100% cấu trúc, controllers, models, views).

## Kiểm tra và Xác minh bắt buộc
Trước khi viết `handoff.md`, Worker phải chạy:
1. `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` -> Phải thành công 100%, 0 Error.
2. Kiểm tra `grep -rn "MauChu" WEB_TinhNguyen/MoHinh/Entities.cs` -> 0 kết quả.
3. Kiểm tra `grep -rn "MauNen" WEB_TinhNguyen/MoHinh/Entities.cs` -> 0 kết quả.
4. Chạy toàn bộ E2E test suite:
   `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet test /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj --logger "console;verbosity=normal"` -> Phải vượt qua 100%.

## Yêu cầu Báo cáo Handoff
Viết file `handoff.md` tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md` bao gồm:
- Tóm tắt danh sách 66 thuộc tính màu đã xóa.
- Danh sách các bảng/cột đã đổi sang tiếng Việt.
- Tên và vị trí file Migration mới.
- Kết quả kiểm tra lệnh build và test (kèm log).
- Xác nhận phân hệ `Areas/CuaHang` nguyên vẹn 100%.


## 2026-11-04T06:04:49Z
Bạn là Worker chịu trách nhiệm thực thi Milestone M2: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn (Requirement R1) cho dự án WEB_TinhNguyen.

Thư mục làm việc của bạn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/`
Bắt đầu bằng việc đọc kỹ các tài liệu sau:
1. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md` (BẮT BUỘC)
2. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/DISPATCH.md` (Hướng dẫn chi tiết từng bước)
3. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md` (Bản thiết kế chi tiết kiến trúc CSDL)
4. `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

CẢNH BÁO TÍNH TOÀN VẸN (MANDATORY INTEGRITY WARNING):
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Thực hiện các bước theo DISPATCH.md:
- Xóa toàn bộ 66 thuộc tính màu trong `MoHinh/Entities.cs` và `MoHinh/CauHinhTrangChuVM.cs`.
- Thuần hóa tiếng Việt cho bảng và cột trong Models, DbContext, Migrations.
- Tạo Code First Migration mới `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` và Designer, cập nhật `ApplicationDbContextModelSnapshot.cs`.
- Cập nhật LINQ queries trong các controller thuộc `BoDieuKhien/`.
- Dọn dẹp các form Edit trong `GiaoDien/Admin*` (xóa input type="color").
- Bảo toàn 100% phân hệ bán hàng `Areas/CuaHang/` (KHÔNG ĐƯỢC CHẠM VÀO).
- Chạy `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` đảm bảo 0 lỗi.
- Chạy toàn bộ E2E test suite và kiểm tra 100% pass.
- Viết `handoff.md` đầy đủ tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2_run2/handoff.md` và gửi tin nhắn báo cáo cho Orchestrator.
