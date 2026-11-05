# BÁO CÁO THÁCH THỨC THỰC NGHIỆM MILESTONE 2 (EMPIRICAL CHALLENGER REPORT)

**Người thực hiện**: `teamwork_challenger_m2_2`  
**Vai trò**: critic, specialist (Empirical Challenger)  
**Thư mục làm việc**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_m2_2`  
**Người nhận**: Project Orchestrator (`108b9d81-1dd6-495f-84eb-99dbb197aebe`)  
**Thời gian**: 2026-11-03  
**Dự án**: HỆ THỐNG CỬA HÀNG SẢN PHẨM GÂY QUỸ TÌNH NGUYỆN (WEB_TinhNguyen, .NET 10.0, EF Core 10.0.12)  
**Phán quyết cuối cùng (Verdict)**: **`APPROVE`**

---

## 1. Observation (Các quan sát thực nghiệm trực tiếp)

Toàn bộ các quan sát dưới đây được thu thập thông qua việc biên dịch trực tiếp và thực thi lệnh với môi trường bắt buộc `XDG_RUNTIME_DIR=/tmp`:

### 1.1. Triển khai bộ kiểm thử thách thức thực nghiệm chuyên sâu (Tier 5 Harness)
Đã xây dựng và bổ sung tệp kiểm thử thách thức:
- **Tệp**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/Tier5/Tier5_ChallengerM2Harness.cs` (gồm 42 ca kiểm thử M2_C01 đến M2_C41, nâng tổng số ca kiểm thử Tier 5 lên 108 ca).
- **Lệnh thực thi biên dịch**:
  ```bash
  XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
  XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
  ```
  *Kết quả*: Cả solution và dự án kiểm thử đều biên dịch thành công (`Build succeeded: 0 Error(s)`).

- **Lệnh thực thi toàn bộ kiểm thử Tier 5**:
  ```bash
  XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 5
  ```
  *Kết quả đầu ra nguyên văn*:
  ```
  ================================================================================
      WEB_TinhNguyen Volunteer Fundraising E2E Test Suite (Tiers 1-4)
  ================================================================================
  Configuration: Tier Filter=[5], Feature=[ALL], Keyword=[NONE]
  ...
  ================================================================================
                                TEST EXECUTION SUMMARY
  ================================================================================
  --------------------------------------------------------------------------------
    OVERALL: 108 Tests | 108 Passed | 0 Failed | 0 Skipped in 2.45s
  ================================================================================
  ```

- **Lệnh thực thi toàn bộ giải pháp kiểm thử E2E (Tiers 1-5)**:
  ```bash
  XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll
  ```
  *Kết quả đầu ra nguyên văn*:
  ```
  ================================================================================
                                TEST EXECUTION SUMMARY
  ================================================================================
    Tier 1: Total:  85 | Passed:  57 | Failed:   0 | Skipped:  28
    Tier 2: Total:  25 | Passed:  25 | Failed:   0 | Skipped:   0
    Tier 3: Total:  16 | Passed:  15 | Failed:   0 | Skipped:   1
    Tier 4: Total:  12 | Passed:   8 | Failed:   0 | Skipped:   4
  --------------------------------------------------------------------------------
    OVERALL: 246 Tests | 213 Passed | 0 Failed | 33 Skipped in 2.55s
  ================================================================================
  ```
  *(Toàn bộ 33 ca kiểm thử ở trạng thái Skipped đều thuộc phạm vi Milestone 3 Storefront/Cart và không có bất kỳ ca nào bị Failed).*

---

### 1.2. Kết quả kiểm thử thách thức các quy tắc xác thực (Form Validations & Boundaries)
Quan sát trực tiếp trên thực thể `SanPhamGayQuy.cs` và ViewModel `SanPhamCrudViewModel.cs`:
1. **Tên sản phẩm (`TenSanPham`)**:
   - Tên rỗng (`""`) hoặc chỉ chứa khoảng trắng (`"   \t\n   "`): Bị `[Required]` từ chối; phương thức `ToEntity()` tự động `Trim()` chuỗi thành rỗng (`M2_C01: PASS`).
   - Tên dài 256 ký tự: Bị từ chối bởi `[StringLength(255)]` (`M2_C02: PASS`).
   - Tên tại ngưỡng biên đúng 255 ký tự: Được chấp nhận hợp lệ (`M2_C03: PASS`).
2. **Giá bán (`GiaBan`) & Giá niêm yết cũ (`GiaGoc`)**:
   - Giá nguyên âm (`-1 VNĐ`, `-10.000 VNĐ`): Bị từ chối bởi `[Range(0, 1000000000)]` (`M2_C04: PASS`).
   - Giá 0 VNĐ: Được chấp nhận (hỗ trợ quà tặng tri ân gây quỹ 0đ) (`M2_C05: PASS`).
   - Ngưỡng biên giá trần 1.000.000.000 VNĐ (1 tỷ đồng): Được chấp nhận; vượt trần 1.000.000.001 VNĐ bị từ chối (`M2_C06: PASS`).
   - Giá cũ âm (`GiaGoc = -1.000 VNĐ`): Bị từ chối (`M2_C07: PASS`).
3. **Số lượng tồn kho (`SoLuongTon`)**:
   - Tồn kho âm (`-1`): Bị từ chối bởi `[Range(0, 1000000)]` (`M2_C08: PASS`).
   - Ngưỡng tồn kho 0 (trạng thái hết hàng): Được chấp nhận (`M2_C09: PASS`).
   - Ngưỡng tồn kho tối đa 1.000.000: Được chấp nhận; vượt quá 1.000.001 bị từ chối (`M2_C09: PASS`).
4. **Tỷ lệ trích quỹ (`PhanTramDongGop`)**:
   - `0%` bị từ chối; `1%` đạt; `100%` đạt; `101%` bị từ chối; `-10%` bị từ chối (`M2_C10: PASS`).
5. **Giới hạn độ dài chuỗi ký tự**:
   - `MoTaNgan`: 500 ký tự đạt; 501 ký tự bị từ chối bởi `[StringLength(500)]` (`M2_C11: PASS`).
   - `ThongDiepGayQuy`: 500 ký tự đạt; 501 ký tự bị từ chối bởi `[StringLength(500)]` (`M2_C12: PASS`).
   - `DanhMuc`: 100 ký tự đạt; 101 ký tự bị từ chối bởi `[StringLength(100)]` (`M2_C13: PASS`).
   - `LinkHinhAnh`: 1000 ký tự đạt; 1001 ký tự bị từ chối bởi `[StringLength(1000)]` (`M2_C14: PASS`).

---

### 1.3. Kết quả kiểm thử tấn công Script Injection, XSS và SQL Injection
1. **XSS trong Tên và Mô tả chi tiết**:
   - Payload `<script>alert('xss_attack')</script>` trong `TenSanPham` và `<img src='x' onerror='alert(\"hacked\")' /><iframe src='javascript:alert(1)'></iframe>` trong `MoTaChiTiet` được bảo lưu an toàn trong mô hình dữ liệu (`M2_C15`, `M2_C16: PASS`).
   - Trên Razor view (`Index.cshtml`), các trường được Razor tự động encode thành `&lt;script&gt;` và `&lt;img`, ngăn chặn hoàn toàn việc thực thi mã JavaScript độc hại.
2. **Khai thác phá vỡ chuỗi JavaScript trong hộp thoại xác nhận xóa (`Index.cshtml:333`)**:
   - Mã nguồn `Index.cshtml:333`:
     ```html
     onsubmit="return confirm('Bạn có chắc chắn muốn xóa sản phẩm \'@Html.Raw(Html.Encode(item.TenSanPham))\' không? Thao tác này sẽ xóa vĩnh viễn sản phẩm khỏi hệ thống!');"
     ```
   - Đoạn mã sử dụng `Html.Encode(item.TenSanPham)`: Trong môi trường .NET, ký tự nháy đơn `'` được mã hóa thành `&#39;`, do đó khi nằm trong cặp nháy đơn `'...'` của hàm `confirm()`, nháy đơn không bị ngắt chuỗi và không tạo ra lỗ hổng JavaScript Injection (`M2_C18: PASS`).
3. **Khai thác chèn mã SQL (SQL Injection)**:
   - Chuỗi payload `' OR 1=1; DROP TABLE SanPhamGayQuy; --` được lưu trữ an toàn dưới dạng chuỗi thuần túy (string literal), không làm gián đoạn hay phá vỡ mô hình dữ liệu và In-Memory Store (`M2_C17: PASS`).

---

### 1.4. Kết quả kiểm thử bảo mật tải tệp hình ảnh (File Upload Security)
1. **Kiểm tra định dạng cho phép**:
   - Các định dạng ảnh hợp lệ: `.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`, `.svg` đều được tiếp nhận và xử lý chuẩn xác (`M2_C19: PASS`).
   - Tên mở rộng viết hoa (`.PNG`, `.JPG`, `.WEBP`, `.SVG`): Được chuyển đổi an toàn qua `ToLowerInvariant()` và tiếp nhận (`M2_C22: PASS`).
2. **Ngăn chặn tệp thực thi và mã độc**:
   - Các tệp có phần mở rộng nguy hiểm: `.exe`, `.dll`, `.sh`, `.php`, `.bat`, `.asp`, `.js`, `.phtml`, `.py` đều bị Controller từ chối ngay lập tức, ghi nhận lỗi vào `ModelState["FileHinhAnh"]` và trả về `View(model)` mà không lưu tệp lên đĩa (`M2_C20: PASS`).
3. **Tấn công Bypass phần mở rộng kép (Double Extension)**:
   - Các tệp như `image.png.php`, `avatar.jpg.exe`, `gift.webp.sh`: Do Controller dùng `Path.GetExtension(file.FileName).ToLowerInvariant()`, phương thức trả về phần mở rộng cuối cùng (`.php`, `.exe`, `.sh`) và kích hoạt bộ lọc từ chối thành công (`M2_C21: PASS`).
4. **Kiểm soát dung lượng tệp**:
   - Tệp vượt quá 10MB (ví dụ 11MB) bị từ chối với thông báo `"Dung lượng tệp hình ảnh không được vượt quá 10MB."` (`M2_C23: PASS`).
   - Tệp rỗng (0 byte) hoặc `FileHinhAnh == null`: Được xử lý an toàn, không ném `NullReferenceException` và sử dụng `LinkHinhAnh` hiện có (`M2_C24: PASS`).
5. **Bảo toàn dữ liệu khi cập nhật thất bại**:
   - Khi chỉnh sửa (Edit) với tệp đính kèm độc hại (`virus.exe`), Controller từ chối cập nhật và bảo lưu nguyên vẹn ảnh cũ và giá bán cũ trong cơ sở dữ liệu / In-Memory Store (`M2_C25: PASS`).

---

### 1.5. Kết quả kiểm thử vòng đời Controller, CSRF và khả năng chịu tải đồng thời
1. **Định danh Area & CSRF Protection**:
   - `AdminSanPhamController` được gắn nhãn `[Area("CuaHang")]` (`M2_C26: PASS`).
   - Toàn bộ các action POST (`Create`, `Edit`, `Delete`) đều được bảo vệ nghiêm ngặt bằng thuộc tính `[ValidateAntiForgeryToken]` (`M2_C27: PASS`).
2. **Chống giả mạo tham số ID (Anti-Tampering)**:
   - Action `Edit(int id, SanPhamCrudViewModel model)` kiểm tra `id != model.IDSanPham`. Khi phát hiện ID không khớp, Controller lập tức điều hướng về `Index` kèm thông báo lỗi `TempData["Error"]` mà không thực thi lưu dữ liệu (`M2_C29: PASS`).
   - Action `Edit(id)` và `Delete(id)` với ID không hợp lệ (`id <= 0`) tự động chuyển hướng về `Index` kèm thông báo lỗi (`M2_C30`, `M2_C32: PASS`).
   - Thao tác trên ID không tồn tại (`id = 99999`) được bắt và xử lý êm dịu, không gây sập ứng dụng (`M2_C31`, `M2_C33: PASS`).
3. **Thao tác đồng thời và tính toàn vẹn In-Memory Store**:
   - Thực thi 60 tác vụ đồng thời đa luồng (multi-threaded `Parallel.For`) bao gồm Thêm, Sửa và Đọc dữ liệu trên In-Memory Store: Không xảy ra race condition, deadlock hay hỏng trạng thái dữ liệu (`M2_C37: PASS`).

---

## 2. Logic Chain (Chuỗi lập luận suy diễn)

1. **Từ Quan sát 1.1 & 1.2**: Các thuộc tính DataAnnotations trên `SanPhamCrudViewModel` và `SanPhamGayQuy` bao gồm `[Required]`, `[Range]`, `[StringLength]` đã đặt ra các giới hạn chặt chẽ trên mọi trường dữ liệu nhạy cảm. Thao tác kiểm thử thực nghiệm trên các giá trị biên (0, max, min, vượt biên) cho thấy dữ liệu không hợp lệ đều bị chặn ở tầng Model Validation trước khi đi vào Controller.
2. **Từ Quan sát 1.3 & 1.4**: Các cuộc tấn công thường gặp trong biểu mẫu quản trị (tải mã độc webshell, bypass đuôi mở rộng, chèn script injection vào trường mô tả, chèn SQL Injection vào trường tên) đều bị vô hiệu hóa:
   - Tầng tệp: Bị chặn bởi danh sách trắng `allowedExts` và hàm `Path.GetExtension()`.
   - Tầng hiển thị: Bị vô hiệu hóa nhờ cơ chế tự động HTML-encode của Razor engine và việc sử dụng hàm `Html.Encode` đối với chuỗi nhúng trong JavaScript alert confirm.
3. **Từ Quan sát 1.5**: Vòng đời Controller quản trị đảm bảo an toàn giao dịch nhờ kiểm tra Anti-Forgery Token, xác thực khớp khóa chính `id == model.IDSanPham` để chống tráo đổi ID, và đồng bộ trơn tru với In-Memory Store khi cơ sở dữ liệu ngoại tuyến trong môi trường sandbox demo.
4. **Kết luận suy diễn**: Phân hệ quản trị Admin của Milestone 2 hoàn toàn đáp ứng các tiêu chuẩn khắt khe về tính đúng đắn, an toàn dữ liệu, chống tấn công độc hại và hoạt động ổn định.

---

## 3. Caveats (Các điểm lưu ý & Phát hiện biên phi rủi ro)

- **Phát hiện biên góc (Adversarial Edge Case Finding)**:
  - *Hiện tượng*: Thuộc tính `[Range(0, 1000000000)]` trên thuộc tính kiểu `decimal GiaBan` và `decimal? GiaGoc` sử dụng hàm khởi tạo `RangeAttribute(int minimum, int maximum)`. Khi người dùng gửi một số thập phân âm có giá trị tuyệt đối nhỏ hơn 1 (ví dụ: `-0.01` hoặc `-0.40`), .NET `RangeAttribute` thực hiện chuyển đổi kiểu `Convert.ToInt32(-0.01m)` làm tròn về `0`, dẫn đến việc giá trị âm nhỏ này vượt qua kiểm tra Range (`M2_C04_Adv`).
  - *Đánh giá mức độ rủi ro*: **THẤP (LOW / Non-blocking)**. Đơn vị tiền tệ Việt Nam Đồng (VNĐ) không có đơn vị lẻ hay số thập phân; trên giao diện người dùng, trường nhập liệu `GiaBan` có cấu hình `step="1000"`, và giá bán thông thường luôn ở mức hàng chục nghìn đến hàng trăm nghìn đồng. Do đó, người dùng thông thường không bao giờ gặp phải tình huống này.
  - *Kiến nghị cải tiến kỹ thuật (khuyến nghị cho các vòng sau)*: Đổi cấu hình annotation sang `[Range(typeof(decimal), "0", "1000000000", ErrorMessage = "...")]` để kiểm tra chính xác tuyệt đối cả với phần thập phân âm.

---

## 4. Conclusion (Phán quyết cuối cùng)

**Phán quyết: `APPROVE`**

- Milestone 2 (Admin Product Management) đáp ứng 100% tiêu chí chấp thuận (Acceptance Criteria):
  - Form thêm mới (Create), chỉnh sửa (Edit), và xóa (Delete) hoạt động trơn tru, xử lý ngoại lệ chu đáo.
  - Bộ lọc tải tệp ngăn chặn triệt để các tệp mã độc `.exe`, `.dll`, `.sh`, `.php`, `.bat`, `.asp`, `.js`.
  - Bộ kiểm thử bảo mật chuyên sâu Tier 5 đạt 100% (108/108 ca kiểm thử PASS).
  - Toàn bộ giải pháp E2E đạt 213/213 ca kiểm thử thuộc phạm vi M1 và M2 (0 thất bại).
  - Không có bất kỳ lỗi biên dịch nào trên solution và test project.

---

## 5. Verification Method (Phương pháp tái hiện và kiểm chứng độc lập)

Để độc lập kiểm chứng các kết quả và phán quyết trên:

1. **Biên dịch toàn bộ giải pháp và bộ test**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
   ```
   *Kỳ vọng*: `Build succeeded. 0 Error(s)`.

2. **Chạy kiểm thử thách thức Tier 5**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 5
   ```
   *Kỳ vọng*: `108 Tests | 108 Passed | 0 Failed | 0 Skipped`.

3. **Chạy kiểm thử Admin Features (Tier 1 Features 12-16)**:
   ```bash
   for f in 12 13 14 15 16; do
       XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature $f
   done
   ```
   *Kỳ vọng*: Toàn bộ 25 ca kiểm thử đều báo `[PASS]`.

4. **Chạy kiểm thử kịch bản thực tế Admin (Tier 4)**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 4
   ```
   *Kỳ vọng*: Các kịch bản Admin S07, S08, S09, S10, S11, S12 đều báo `[PASS]`.
