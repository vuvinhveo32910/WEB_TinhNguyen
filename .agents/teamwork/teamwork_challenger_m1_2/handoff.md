# BÁO CÁO KIỂM THỬ THÁCH THỨC THỰC NGHIỆM MILESTONE 1 (EMPIRICAL CHALLENGER REPORT)

**Người thực hiện**: `teamwork_challenger_m1_2`  
**Vai trò**: critic, specialist (Empirical Challenger)  
**Thư mục làm việc**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_m1_2`  
**Người nhận**: Project Orchestrator (`108b9d81-1dd6-495f-84eb-99dbb197aebe`)  
**Thời gian**: 2026-11-03  
**Dự án**: HỆ THỐNG CỬA HÀNG SẢN PHẨM GÂY QUỸ TÌNH NGUYỆN (WEB_TinhNguyen, .NET 10.0, EF Core 10.0.12)  
**Phán quyết (Verdict)**: **`APPROVE`**

---

## 1. Observation (Các quan sát thực nghiệm trực tiếp)

1. **Biên dịch và thực thi bộ kiểm thử thách thức Tier 5**:
   - Tệp kiểm thử được triển khai: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/Tier5/Tier5_ChallengerTests.cs` (gồm 47 ca kiểm thử chuyên sâu C01-C47 bổ sung cho các ca CH01-CH19 của Challenger 1, tổng cộng 66 ca kiểm thử Tier 5).
   - Lệnh thực thi thực nghiệm:
     ```bash
     XDG_RUNTIME_DIR=/tmp dotnet run --no-restore --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj -- --tier 5 --verbose
     ```
   - Kết quả đầu ra nguyên văn:
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
       OVERALL: 66 Tests | 66 Passed | 0 Failed | 0 Skipped in 1.74s
     ================================================================================
     ```
     Toàn bộ 66/66 kịch bản kiểm thử biên và stress test đều **ĐẠT (PASS 100%)**.

2. **Kiểm thử DataAnnotations & Quy tắc Xác thực Mô hình (Validator.TryValidateObject)**:
   - **`SanPhamGayQuy`**:
     - Tên rỗng/null: Bị từ chối bởi `[Required]`, báo lỗi `TenSanPham`.
     - Giá bán âm (-1 VNĐ): Bị từ chối bởi `[Range(0, 1000000000)]`.
     - Giá bán 0 VNĐ: Được chấp nhận (phục vụ quà tặng tri ân gây quỹ).
     - Biên giá bán cực đại (1.000.000.000 VNĐ): Chấp nhận tại ngưỡng 1 tỷ, từ chối khi vượt trần (1.000.000.001 VNĐ).
     - Giá niêm yết cũ âm (-5.000 VNĐ): Bị từ chối bởi `[Range]`.
     - Tỷ lệ đóng góp quỹ `PhanTramDongGop`: 0% bị từ chối, 1% đạt, 100% đạt, 101% bị từ chối, -10% bị từ chối.
     - Số lượng tồn kho `SoLuongTon`: -1 bị từ chối, 0 đạt, 1.000.000 đạt, 1.000.001 bị từ chối.
     - Giới hạn độ dài chuỗi: `MoTaNgan` > 500 ký tự bị từ chối; `ThongDiepGayQuy` > 500 ký tự bị từ chối; `LinkHinhAnh` > 1000 ký tự bị từ chối.
     - Thuộc tính `[NotMapped]`: `ConHang` trả về `false` khi `SoLuongTon == 0`, `true` khi `> 0`; `TienDongGopTrenMoiSp` tính chính xác `GiaBan * PhanTramDongGop / 100`.
   - **`SanPhamCrudViewModel`**:
     - Giá âm hoặc tồn kho âm bị từ chối.
     - `ToEntity()`: Tự động `Trim()` chuỗi tên, mô tả, thông điệp; tự điền danh mục mặc định "Quà lưu niệm" khi rỗng; ưu tiên đường dẫn ảnh tải lên (`uploadedImagePath`) trước `LinkHinhAnh`.
     - `UpdateEntity()`: Bảo lưu đường dẫn ảnh cũ khi không có file mới tải lên, thay thế chính xác khi có file mới, tự động cập nhật `NgayCapNhat`.
     - `FromEntity()`: Sao chép đối xứng 100% thuộc tính.
   - **`DonHangGayQuy` & `CheckoutViewModel`**:
     - `HoTenNguoiMua`, `SoDienThoai`, `DiaChiNhanHang` rỗng: Bị từ chối bởi `[Required]`.
     - `SoDienThoai` không phải số ("abc-not-a-number"): Bị từ chối bởi `[Phone]`.
     - `Email` không có ký tự `@`: Bị từ chối bởi `[EmailAddress]`; `Email == null` được chấp nhận vì email là trường tùy chọn.
     - `TienUngHoThem` trong `CheckoutViewModel`: Âm (-50.000 VNĐ) bị từ chối bởi `[Range(0, 1000000000)]`, 0 VNĐ đạt, 100.000.000 VNĐ đạt.
     - Tính toán tổng tiền: `TongThanhToan` = `TongTienHang + PhiVanChuyen + TienUngHoThem`; `TongTienDongGop` = tổng trích quỹ từ sản phẩm + tiền ủng hộ thêm, tính toán hoàn toàn chính xác.
   - **`ChiTietDonHangGayQuy`**:
     - Số lượng `SoLuong` <= 0 (0 hoặc -3): Bị từ chối bởi `[Range(1, 100000)]`.
     - `PhanTramDongGop`: Nằm trong dải `[0, 100]`, vượt dải bị từ chối.
     - `TienDongGop`: Tính chính xác theo `ThanhTien * PhanTramDongGop / 100m`.
   - **`CartItem` & `CartViewModel`**:
     - Giỏ hàng rỗng (`new CartViewModel()`): `TongSoLuong == 0`, `TongTienHang == 0`, `CoSanPham == false`.
     - Phòng vệ Null (`Items = null!`): Thuộc tính tính toán an toàn trả về 0, không gây ra `NullReferenceException`.

3. **Kiểm thử EF Core DbContext Metadata (Không cần kết nối CSDL vật lý)**:
   - Thực thi `context.Model` trên `ApplicationDbContext` với cấu hình SqlServer ảo:
     ```csharp
     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
         .UseSqlServer("Server=dummy;Database=dummy;")
         .Options;
     using var context = new ApplicationDbContext(options);
     var model = context.Model; // Khởi tạo biên dịch Semantic IModel thuần túy trong RAM
     ```
   - Kết quả quan sát:
     - Khởi tạo thành công không lỗi, không cần socket DB lắng nghe.
     - 3 thực thể `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy` được nhận diện đầy đủ trong `IModel`.
     - Tên bảng ánh xạ chính xác: `"SanPhamGayQuy"`, `"DonHangGayQuy"`, `"ChiTietDonHangGayQuy"`.
     - Kiểu dữ liệu 8 cột tiền tệ đều được cấu hình chuẩn `decimal(18,2)`: `GiaBan`, `GiaGoc`, `TongTienHang`, `TienUngHoThem`, `PhiVanChuyen`, `TongThanhToan`, `DonGia`, `ThanhTien`.
     - Khóa ngoại và hành vi xóa: Chi tiết đơn hàng liên kết đơn hàng với `DeleteBehavior.Cascade`; Chi tiết đơn hàng liên kết sản phẩm với `DeleteBehavior.Restrict`.

4. **Kiểm thử SessionExtensions & DbInitializer In-Memory**:
   - `SessionExtensions.Set<T>` với giá trị `null`: Tự động xóa key khỏi session.
   - Đọc dữ liệu JSON bị hỏng: `SessionExtensions.Get<T>` bắt lỗi JSON và trả về `default` an toàn, không làm crash ứng dụng.
   - Thao tác đa luồng trên kho lưu trữ In-Memory: 100 tác vụ chạy song song đồng thời thực hiện đọc, thêm sản phẩm, thêm đơn hàng, cập nhật và xóa không phát sinh xung đột `InvalidOperationException` nhờ khóa `lock (_syncLock)`.
   - Đặt hàng vượt tồn kho: Logic `Math.Max(0, prod.SoLuongTon - detail.SoLuong)` kìm tồn kho tại 0, không bị tràn số âm.
   - Đọc dữ liệu in-memory trả về bản sao độc lập (Clone Product), ngăn chặn rò rỉ biến đổi từ bên ngoài.

5. **Phát hiện hiện tượng dương tính giả trong kiểm thử Tier 1**:
   - Kiểm thử `F02_T02` trong `Tier1_FoundationTests.cs` (kiểm tra thứ tự `UseSession` và `UseAuthorization`) sử dụng hàm `IndexOf("UseAuthorization")`.
   - Trong `Program.cs`, tại dòng 42 có chú thích:
     `// Middleware Session: BẮT BUỘC đặt sau UseRouting() và trước UseAuthorization()`
     dẫn tới `IndexOf("UseAuthorization")` tìm thấy dòng 42 (trước `UseSession` ở dòng 43).
   - Trên thực tế câu lệnh thực thi `app.UseSession();` (dòng 43) nằm trước `app.UseAuthorization();` (dòng 45). Kiểm thử thực nghiệm `CH19` lọc bỏ chú thích đã xác nhận thứ tự thực thi thực tế hoàn toàn đúng chuẩn ASP.NET Core.

---

## 2. Logic Chain (Chuỗi lập luận và Phân tích)

1. Từ quan sát 1.1 và 1.2, các mô hình thực thể và ViewModel đã được trang bị đầy đủ các thuộc tính xác thực DataAnnotations chuẩn (`[Required]`, `[Range]`, `[StringLength]`, `[Phone]`, `[EmailAddress]`). Các trường hợp kiểm thử biên (Boundary Values: 0, 1, 100, 101, 1.000.000.000) và dữ liệu độc hại (số âm, chuỗi quá dài, định dạng số điện thoại/email sai) đều bị hệ thống bắt giữ và từ chối hợp lệ.
2. Từ quan sát 1.3, cấu hình Entity Framework Core trong `ApplicationDbContext.OnModelCreating` đáp ứng đầy đủ yêu cầu: loại bỏ triệt để cảnh báo mất độ chính xác của cột tiền tệ qua `decimal(18,2)`, bảo toàn tính toàn vẹn dữ liệu quan hệ cha-con (`Cascade`) và chống xóa sản phẩm khi đã có đơn hàng (`Restrict`).
3. Từ quan sát 1.4, tiện ích `SessionExtensions` và `DbInitializer` có khả năng phòng thủ cao trước các kịch bản bất thường (dữ liệu rác, lỗi JSON, xung đột đa luồng, tràn số tồn kho). Cơ chế Dual-Mode cho phép hệ thống vận hành bền bỉ 100% cả trong môi trường có SQL Server lẫn offline demo.
4. Từ quan sát 1.5, lỗi kiểm tra thứ tự middleware trong Tier 1 chỉ là lỗi logic của phương pháp quét chuỗi trong bộ kiểm thử (chuỗi xuất hiện trong chú thích mã nguồn), mã thực thi trong `Program.cs` đáp ứng đúng chuẩn kiến trúc ASP.NET Core.

---

## 3. Caveats (Các lưu ý & Giả định)

1. `CartItem` là POCO dùng cho serialization lưu trong Session nên không chứa DataAnnotations; việc xác thực số lượng hợp lệ (>= 1 và <= Tồn kho) cần được kiểm soát chặt chẽ ở lớp Controller khi thêm/cập nhật giỏ hàng trong Milestone 3.
2. Tồn kho trong kho In-Memory được kìm ở mức 0 khi người dùng đặt quá số lượng có sẵn; trong giai đoạn triển khai Controller (M3), cần hiển thị thông báo lỗi cho người dùng khi số lượng mua vượt quá số lượng tồn thay vì chỉ tự động trừ về 0.

---

## 4. Conclusion (Kết luận & Phán quyết)

- Toàn bộ nền tảng mã nguồn Milestone 1 (Area & Data Foundation) đã vượt qua toàn bộ 66 ca kiểm thử thực nghiệm chuyên sâu (Tier 5 Adversarial & Boundary Harness) với tỷ lệ **đạt 100% (66/66 PASS)**.
- Các quy tắc xác thực, xử lý biên, kiểu dữ liệu tiền tệ, quan hệ thực thể và cơ chế phòng thủ đa luồng đạt độ tin cậy và chất lượng cao.
- **Phán quyết chính thức**: **`APPROVE`**. Nền tảng Milestone 1 đủ điều kiện nghiệm thu để các phân hệ Milestone 2 (Admin Product Management) và Milestone 3 (Storefront & Cart) tiếp tục triển khai.

---

## 5. Verification Method (Phương pháp tái hiện và kiểm chứng)

Bất kỳ kiểm định viên nào cũng có thể kiểm chứng độc lập kết quả báo cáo bằng lệnh:

```bash
XDG_RUNTIME_DIR=/tmp dotnet run --no-restore --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj -- --tier 5 --verbose
```
- **Tiêu chí đạt**: `66 Tests | 66 Passed | 0 Failed | 0 Skipped`.
- **Tệp kiểm thử cần thanh tra**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/Tier5/Tier5_ChallengerTests.cs`.
