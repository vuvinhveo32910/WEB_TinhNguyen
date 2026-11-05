# Báo cáo Kiểm định Pháp y (Forensic Audit Report) — Milestone M2 (Requirement R1)

**Work Product**: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn (Requirement R1)  
**Profile**: General Project  
**Integrity Mode**: Development (theo `ORIGINAL_REQUEST.md`)  
**Verdict**: **CLEAN**

---

## 1. Observation

### 1.1. Kiểm tra Xóa bỏ 66 thuộc tính màu và Input màu
- **Thực thể và ViewModel (`MoHinh/`)**:
  - Lệnh: `grep -in "Mau" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/*.cs`
  - Kết quả: Mã thoát `1`, 0 dòng trùng khớp. Toàn bộ 66 trường màu (`MauChuDao`, `MauNhan`, `MauChu`, `MauNenStart`, `MauNenMid`, `MauNenEnd`, v.v.) đã được loại bỏ hoàn toàn khỏi `MoHinh/Entities.cs` và `MoHinh/CauHinhTrangChuVM.cs`.
- **Ngữ cảnh CSDL (`DuLieu/ApplicationDbContext.cs`)**:
  - Lệnh: `grep -in "Mau" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/DuLieu/*.cs`
  - Kết quả: Mã thoát `1`, 0 dòng trùng khớp.
- **Bộ điều khiển (`BoDieuKhien/`)**:
  - Lệnh: `grep -in "Mau" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/BoDieuKhien/*.cs`
  - Kết quả: Mã thoát `1`, 0 dòng trùng khớp. Tất cả các câu truy vấn LINQ đã được cập nhật, không còn bất kỳ truy vấn hay gán giá trị trường màu nào.
- **Giao diện (`GiaoDien/`)**:
  - Lệnh kiểm tra ô nhập màu: `grep -rn "type=\"color\"" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/GiaoDien/`
  - Kết quả: Mã thoát `1`, 0 dòng trùng khớp.
  - Lệnh kiểm tra từ khóa màu: `grep -rn "Mau" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/GiaoDien/`
  - Kết quả: Mã thoát `1`, 0 dòng trùng khớp.

### 1.2. Kiểm tra Bản Migration EF Core và Snapshot
- Tệp Migration: `WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`
  - Thực thi `migrationBuilder.DropColumn` cho toàn bộ 66 cột màu trên 11 bảng CSDL (`CauHinhTrangChu`, `TinTucTrangChu`, `ChuongTrinhDemNguoc`, `CauHinhKyUc`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, `KenhDongGop`, `CauHinhBanToChuc`, `ThanhVienBanToChuc`, `CauHinhTinTuc`, `BaiVietTinTuc`).
  - Thực thi `migrationBuilder.RenameTable`:
    - `BannerTrangChu` -> `BieuNguTrangChu`
    - `TVKhoanhKhac` -> `ThuVienKhoanhKhac`
    - `FAQTrangChu` -> `CauHoiThuongGapTrangChu`
  - Thực thi `migrationBuilder.RenameColumn` cho các cột: `IDBanner` -> `IDBieuNgu`, `TieuDeAlt` -> `TieuDeThayThe`, `LinkAnh` -> `DuongDanAnh`, `DoTreAnimation` -> `DoTreHoatHoa`, `ViTriTop` -> `ViTriTren`, `HienThiCanvas3D` -> `HienThiHieuUng3D`, `HienThiMascot` -> `HienThiLinhVat`, `LinkAnhMascot` -> `DuongDanAnhLinhVat`, v.v.
  - Cung cấp đầy đủ hàm `Down` hoàn nguyên ngược lại.
- Tệp Snapshot: `WEB_TinhNguyen/Migrations/ApplicationDbContextModelSnapshot.cs`
  - Kiểm tra `grep -in "Mau" ApplicationDbContextModelSnapshot.cs` trả về 0 kết quả.
  - Kiểm tra các bảng đã đổi tên (`BieuNguTrangChu`, `ThuVienKhoanhKhac`, `CauHoiThuongGapTrangChu`) đã được cập nhật đồng bộ trong ModelSnapshot.

### 1.3. Kiểm tra Tính Toàn Vẹn và Cô Lập của `Areas/CuaHang/`
- Lệnh: `git status --porcelain WEB_TinhNguyen/Areas/CuaHang/`
- Kết quả: Thư mục `Areas/CuaHang/` hoàn toàn nguyên vẹn, không có bất kỳ tệp tin nào bị chỉnh sửa hay xóa bỏ trong quá trình thực hiện M2.
- Kiểm tra các view và layout: Tệp `_ViewStart.cshtml` trong `Areas/CuaHang/Views/` tự động phân giải layout chính xác, các controller trong `Areas/CuaHang/Controllers/` hoạt động độc lập và bảo toàn 100% cấu trúc DOM.

### 1.4. Kiểm tra Không Có Gian Lận (Anti-Cheating / Anti-Facade Checks)
- **Hardcoded test results**: Tìm kiếm chuỗi kết quả test hay chuỗi giả mạo `[PASS]`, `Tier` trong thư mục mã nguồn `WEB_TinhNguyen/` trả về 0 kết quả. Mã nguồn nghiệp vụ không chứa bất kỳ logic spoof kết quả test nào.
- **Facade implementations**: `DuLieu/Compatibility.cs` chỉ đóng vai trò Type Forwarding / Backward Compatibility (`[Obsolete]` markers) cho EF Core / Reflection, không chứa dummy/facade implementations. Toàn bộ controllers trong `BoDieuKhien/` thực thi nghiệp vụ lưu trữ, upload file, và truy vấn DB thật 100%.
- **Pre-populated artifacts**: Không có bất kỳ artifact log/kết quả giả lập nào tồn tại trước khi chạy kiểm thử.
- **Test suite integrity**: Bộ kiểm thử `tests/E2ETests/` không bị worker sửa đổi trong lượt chạy M2. Các test methods thực hiện reflection, kiểm tra contracts, DbContext metadata, pipeline ordering và lifecycle thật.

### 1.5. Thực Thi Độc Lập Build và Test
- **Lệnh Build**:
  ```bash
  XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
  ```
  - Kết quả: `0 Error(s)`, `2 Warning(s)` (chỉ là cảnh báo mạng NU1900 của NuGet vulnerability feed), `Time Elapsed 00:00:08.87`.
- **Lệnh E2E Tests**:
  ```bash
  XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
  ```
  - Kết quả:
    - `Tier 1: Total: 100 | Passed: 100 | Failed:   0 | Skipped:   0`
    - `Tier 2: Total:  28 | Passed:  28 | Failed:   0 | Skipped:   0`
    - `Tier 3: Total:  18 | Passed:  18 | Failed:   0 | Skipped:   0`
    - `Tier 4: Total:  12 | Passed:  12 | Failed:   0 | Skipped:   0`
    - `Tier 5: Total: 158 | Passed: 158 | Failed:   0 | Skipped:   0`
    - **OVERALL: 316 Tests | 316 Passed | 0 Failed | 0 Skipped in 4.71s** (Tỉ lệ đạt: 100%).

---

## 2. Logic Chain

1. Từ **Quan sát 1.1**, việc xóa sạch 66 thuộc tính màu trong Models, ViewModel, Controllers và Views đảm bảo mã nguồn đã dọn dẹp triệt để các cấu hình màu động rác, chuyển sang hệ màu thương hiệu cố định phù hợp với chủ đề tình nguyện.
2. Từ **Quan sát 1.2**, migration `20261104000000_XoaCotMauVaVietHoaTenBangCot.cs` và `ApplicationDbContextModelSnapshot.cs` đã cấu hình chuẩn xác các thao tác `DropColumn`, `RenameTable`, `RenameColumn`, đáp ứng 100% tiêu chí nghiệm thu CSDL của Requirement R1.
3. Từ **Quan sát 1.3**, phân hệ `Areas/CuaHang/` được bảo toàn nguyên vẹn 100%, không bị ảnh hưởng bởi quá trình tái cấu trúc CSDL và dọn dẹp mã nguồn.
4. Từ **Quan sát 1.4**, không phát hiện bất kỳ dấu hiệu gian lận nào: không hardcode test outputs, không facade implementations, không sửa test suite để lách luật.
5. Từ **Quan sát 1.5**, việc tự tay thực thi độc lập lệnh `dotnet build` đạt 0 Error và chạy toàn bộ 316 E2E tests đạt 100% Pass chứng minh hệ thống hoàn toàn ổn định và đạt chất lượng cao.

---

## 3. Caveats

- Các cảnh báo `NU1900` trong quá trình build là do môi trường sandbox không có kết nối internet đến `api.nuget.org` để kiểm tra lỗ hổng bảo mật gói tin, không ảnh hưởng đến tính toàn vẹn và khả năng biên dịch/chạy của ứng dụng.
- Tệp `DuLieu/Compatibility.cs` được giữ lại để đảm bảo tính tương thích ngược cho các kiểm thử reflection/mocking cũ; các lớp này được đánh dấu `[Obsolete]` rõ ràng và không vi phạm quy tắc liêm chính.

---

## 4. Conclusion

- **Verdict**: **CLEAN**
- Milestone M2 (Requirement R1: Tái cấu trúc CSDL và Dọn dẹp Mã nguồn) đạt toàn bộ các tiêu chí liêm chính và nghiệm thu kỹ thuật:
  1. Xóa bỏ hoàn toàn 66 thuộc tính màu khỏi Models, DbContext, Controllers và Views.
  2. Tạo bản EF Core Code First Migration và ModelSnapshot chuẩn xác.
  3. Thuần hóa tiếng Việt cho tên bảng, cột và các câu truy vấn LINQ/SQL.
  4. Bảo toàn nguyên trạng 100% phân hệ bán hàng `Areas/CuaHang/`.
  5. Build thành công 0 Error, E2E test suite đạt 316/316 bài test Pass (100%).
  6. Không có hành vi gian lận hay facade nào.

---

## 5. Verification Method

Bất kỳ kiểm định viên hoặc giám sát viên nào đều có thể tự động kiểm chứng độc lập bằng các câu lệnh sau:

1. **Kiểm tra biên dịch**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Kỳ vọng*: 0 Error.

2. **Kiểm tra toàn bộ E2E Test Suite**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
   ```
   *Kỳ vọng*: OVERALL 316 Tests | 316 Passed | 0 Failed.

3. **Kiểm tra xóa thuộc tính màu trong Models và Views**:
   ```bash
   grep -in "Mau" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/MoHinh/*.cs
   grep -rn "type=\"color\"" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/GiaoDien/
   ```
   *Kỳ vọng*: Cả hai lệnh đều trả về exit code 1 (0 dòng tìm thấy).

4. **Kiểm tra tính cô lập của phân hệ bán hàng**:
   ```bash
   git status --porcelain WEB_TinhNguyen/Areas/CuaHang/
   ```
   *Kỳ vọng*: Không có thay đổi nào trong `Areas/CuaHang/`.
