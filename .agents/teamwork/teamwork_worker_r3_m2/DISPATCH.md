# DISPATCH — Worker M2 (Database Architect & EF Core Developer)

**Role**: Database Architect & EF Core Developer (Kiến trúc sư Cơ sở dữ liệu & EF Core)
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
**Project Document**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
**Explorer 1 Survey Blueprint**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md

## Scope & Objective: Milestone M2 (Tái cấu trúc CSDL và dọn dẹp mã nguồn - Requirement R1)

Chi tiết thực thi theo bản thiết kế của Explorer 1:

1. **Xóa 66 thuộc tính màu trong Models & ViewModels**:
   - `WEB_TinhNguyen/MoHinh/Entities.cs`: Xóa 66 thuộc tính màu trên 11 class (`CauHinhTrangChu`: 9, `TinTucTrangChu`: 1, `ChuongTrinhDemNguoc`: 3, `CauHinhKyUc`: 4, `CauHinhChuongTrinh`: 3, `CauHinhGayQuy`: 16, `KenhDongGop`: 5, `CauHinhBanToChuc`: 9, `ThanhVienBanToChuc`: 2, `CauHinhTinTuc`: 10, `BaiVietTinTuc`: 4).
   - `WEB_TinhNguyen/MoHinh/CauHinhTrangChuVM.cs`: Xóa toàn bộ 66 thuộc tính màu tương ứng trên các class ViewModels.

2. **Thuần hóa tiếng Việt tên Bảng và Cột trong Models**:
   - `BannerTrangChu` -> `BieuNguTrangChu` (`IDBanner` -> `IDBieuNgu`, `TieuDeAlt` -> `TieuDeThayThe`, `LinkAnh` -> `DuongDanAnh`)
   - `TVKhoanhKhac` -> `ThuVienKhoanhKhac` (`LinkAnh` -> `DuongDanAnh`)
   - `FAQTrangChu` -> `CauHoiThuongGapTrangChu` (`IDFAQ` -> `IDCauHoi`)
   - Cập nhật các cột trong `CauHinhAnhNoi`, `CauHinhTrangChu`, `CauHinhBanToChuc` (`KeuGoi_...`), `CauHinhGayQuy` (`NoiBat_...`), `KenhDongGop`, `BaiVietTinTuc`, `NhaTaiTro...` (`DuongDan...`).

3. **Cập nhật `WEB_TinhNguyen/DuLieu/ApplicationDbContext.cs`**:
   - Cập nhật các `DbSet` (`BieuNguTrangChus`, `ThuVienKhoanhKhacs`, `CauHoiThuongGapTrangChus`).
   - Cập nhật `OnModelCreating` nếu có.
   - Thêm phương thức tự động gọi `context.Database.Migrate()` trong `Program.cs` khi `context.Database.CanConnect() == true`.

4. **Tạo bản Code First Migration và cập nhật Snapshot**:
   - Tạo file migration `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` với phương thức `Up()` (Drop 66 cột màu, RenameTable, RenameColumn) và `Down()`.
   - Tạo file Designer `20261104000000_XoaCotMauVaVietHoaTenBangCot.Designer.cs`.
   - Cập nhật `ApplicationDbContextModelSnapshot.cs` loại bỏ các trường màu và cập nhật tên bảng/cột mới.

5. **Cập nhật Controllers & các câu truy vấn LINQ/SQL trong `WEB_TinhNguyen/BoDieuKhien/`**:
   - `HomeController.cs`: Cập nhật LINQ Include (`BieuNguTrangChus`, `ThuVienKhoanhKhacs`, `CauHoiThuongGaps`, ...).
   - `AdminTrangChuController.cs`: Xóa gán màu trong GET/POST Edit, đổi sang `BieuNguTrangChu`.
   - `AdminChuongTrinhController.cs`, `AdminGayQuyController.cs`, `AdminBanToChucController.cs`, `AdminTinTucController.cs`, `AdminKyUcController.cs`: Xóa toàn bộ gán thuộc tính màu.

6. **Dọn dẹp form Edit trong `WEB_TinhNguyen/GiaoDien/Admin*`**:
   - Xóa các thẻ `<input type="color" ...>` và ô nhập mã HEX.

7. **Bảo toàn phân hệ Cửa Hàng**:
   - Không thay đổi bất kỳ trường nào của `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy` và `DbInitializer.cs`.

8. **Kiểm tra biên dịch & Test**:
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj` (0 errors).
   - `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build tests/E2ETests/E2ETests.csproj --configfile tests/NuGet.Config` (0 errors).
   - `bash tests/run_e2e_tests.sh --all`

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.
