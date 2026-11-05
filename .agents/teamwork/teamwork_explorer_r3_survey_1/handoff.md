# BÁO CÁO KHẢO SÁT & BẢN THIẾT KẾ KIẾN TRÚC CƠ SỞ DỮ LIỆU (REQUIREMENT R1)
**Vai trò**: Kiến trúc sư Cơ sở dữ liệu (Database Architect Explorer - EF Core)  
**Đường dẫn làm việc**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1`  
**Mục tiêu**: Khảo sát toàn diện hệ thống phục vụ Yêu cầu R1 (Tái cấu trúc CSDL và dọn dẹp mã nguồn), xác định toàn bộ thuộc tính màu sắc, kiểm tra các bảng/cột cần thuần hóa tiếng Việt, và xây dựng bản kế hoạch thực thi Code First Migration chi tiết cho Lập trình viên C# (.NET 10).

---

## 1. Quan sát thực tế (Observation)

### 1.1 Khảo sát Models, DbContext và Entity Mappings
- **DbContext chính**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Data/ApplicationDbContext.cs`
  - Đang kế thừa `Microsoft.EntityFrameworkCore.DbContext`.
  - Khai báo 22 `DbSet` gồm 19 bảng thuộc phân hệ lõi thiện nguyện và 3 bảng thuộc phân hệ cửa hàng gây quỹ:
    - Lõi: `ChienDichs`, `CauHinhTrangChus`, `CauHinhAnhNois`, `BannerTrangChus`, `ChuongTrinhDemNguocs`, `CauHinhKyUcs`, `AnhKyUcs`, `NhaTaiTroKyUcs`, `CauHinhChuongTrinhs`, `ChangChuongTrinhs`, `ChiTietLoTrinhs`, `CauHinhGayQuys`, `KenhDongGops`, `NhaTaiTroGayQuys`, `CauHinhBanToChucs`, `ThanhVienBanToChucs`, `CauHinhTinTucs`, `BaiVietTinTucs`, `ThuVienHieuUngs`.
    - Phân hệ bán hàng `Areas/CuaHang`: `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`.
    - 4 thực thể phụ được ánh xạ thông qua quan hệ tập hợp (Navigation Collection) trong `CauHinhTrangChu`: `TVKhoanhKhac`, `TinTucTrangChu`, `NhaTaiTroTrangChu`, `FAQTrangChu`.
  - Phương thức `OnModelCreating` (dòng 41-80) cấu hình Fluent API:
    - `SanPhamGayQuy`: kiểu `decimal(18,2)` cho `GiaBan`, `GiaGoc`.
    - `DonHangGayQuy`: kiểu `decimal(18,2)` cho `TongTienHang`, `TienUngHoThem`, `PhiVanChuyen`, `TongThanhToan`.
    - `ChiTietDonHangGayQuy`: kiểu `decimal(18,2)` cho `DonGia`, `ThanhTien`; khóa ngoại `IDDonHang` (Cascade Delete) và `IDSanPham` (Restrict Delete).
- **Mô hình thực thể (Entities)**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/Entities.cs` (613 dòng, chứa 20 class thực thể).
- **Mô hình hiển thị (ViewModels)**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/CauHinhTrangChuVM.cs` (453 dòng, chứa các ViewModels đồng bộ với Entities).
- **Mô hình phân hệ CuaHang**: Nằm tách biệt tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/`.

### 1.2 Danh mục chi tiết 66 thuộc tính & cột lưu trữ màu sắc cần xóa bỏ
Qua đối chiếu giữa `Entities.cs`, `CauHinhTrangChuVM.cs` và `ApplicationDbContextModelSnapshot.cs`, phát hiện chính xác **66 thuộc tính/cột màu sắc** phân bố tại **11 thực thể/bảng**:

| STT | Bảng / Thực thể | Tên thuộc tính / cột | Kiểu C# | Dòng trong `Entities.cs` | Mục đích hiện tại |
|---|---|---|---|---|---|
| 1 | `CauHinhTrangChu` | `MauChuDao` | `string?` | 38 | Màu chủ đạo (Leaf/Green) |
| 2 | `CauHinhTrangChu` | `MauNhan` | `string?` | 39 | Màu nhấn phụ |
| 3 | `CauHinhTrangChu` | `MauChu` | `string?` | 40 | Màu chữ chung (Forest) |
| 4 | `CauHinhTrangChu` | `MauNenStart` | `string?` | 41 | Màu nền gradient bắt đầu |
| 5 | `CauHinhTrangChu` | `MauNenMid` | `string?` | 42 | Màu nền gradient giữa |
| 6 | `CauHinhTrangChu` | `MauNenEnd` | `string?` | 43 | Màu nền gradient kết thúc |
| 7 | `CauHinhTrangChu` | `VeDuAn_MauTieuDe` | `string?` | 55 | Màu tiêu đề mục "Về dự án" |
| 8 | `CauHinhTrangChu` | `VeDuAn_MauChu` | `string?` | 56 | Màu chữ mô tả "Về dự án" |
| 9 | `CauHinhTrangChu` | `VeDuAn_MauNenTag` | `string?` | 57 | Màu nền badge tag "Về dự án" |
| 10 | `TinTucTrangChu` | `MauTieuDe` | `string?` | 139 | Màu tiêu đề tin tức trang chủ |
| 11 | `ChuongTrinhDemNguoc` | `MauNenStart` | `string?` | 188 | Màu nền countdown start |
| 12 | `ChuongTrinhDemNguoc` | `MauNenEnd` | `string?` | 189 | Màu nền countdown end |
| 13 | `ChuongTrinhDemNguoc` | `MauChu` | `string?` | 190 | Màu chữ đếm ngược |
| 14 | `CauHinhKyUc` | `MauTieuDeTongKet` | `string?` | 214 | Màu tiêu đề tổng kết |
| 15 | `CauHinhKyUc` | `MauChuDao` | `string?` | 216 | Màu chủ đạo trang ký ức |
| 16 | `CauHinhKyUc` | `MauNenHienTai` | `string?` | 217 | Màu nền trang ký ức |
| 17 | `CauHinhKyUc` | `MauChu` | `string?` | 218 | Màu chữ trang ký ức |
| 18 | `CauHinhChuongTrinh` | `MauChuDao` | `string?` | 275 | Màu chủ đạo chương trình |
| 19 | `CauHinhChuongTrinh` | `MauNenHienTai` | `string?` | 276 | Màu nền chương trình |
| 20 | `CauHinhChuongTrinh` | `MauChu` | `string?` | 277 | Màu chữ chương trình |
| 21 | `CauHinhGayQuy` | `MauChuDao` | `string?` | 332 | Màu chủ đạo gây quỹ |
| 22 | `CauHinhGayQuy` | `Hero_MauChuNhan` | `string?` | 336 | Màu chữ nhãn hero gây quỹ |
| 23 | `CauHinhGayQuy` | `Hero_MauNenNhan` | `string?` | 337 | Màu nền nhãn hero gây quỹ |
| 24 | `CauHinhGayQuy` | `Hero_MauTieuDe` | `string?` | 340 | Màu tiêu đề hero gây quỹ |
| 25 | `CauHinhGayQuy` | `Hero_MauMoTa` | `string?` | 344 | Màu mô tả hero gây quỹ |
| 26 | `CauHinhGayQuy` | `Hero_MauNutStart` | `string?` | 348 | Màu nút bắt đầu hero |
| 27 | `CauHinhGayQuy` | `Hero_MauNutEnd` | `string?` | 349 | Màu nút kết thúc hero |
| 28 | `CauHinhGayQuy` | `Hero_MauNenStart` | `string?` | 352 | Màu nền hero start |
| 29 | `CauHinhGayQuy` | `Hero_MauNenMid` | `string?` | 353 | Màu nền hero mid |
| 30 | `CauHinhGayQuy` | `Hero_MauNenEnd` | `string?` | 354 | Màu nền hero end |
| 31 | `CauHinhGayQuy` | `DongGop_MauTieuDe` | `string?` | 359 | Màu tiêu đề kênh đóng góp |
| 32 | `CauHinhGayQuy` | `DongGop_MauMoTa` | `string?` | 361 | Màu mô tả đóng góp |
| 33 | `CauHinhGayQuy` | `BaoCao_MauNen` | `string?` | 365 | Màu nền báo cáo thu chi |
| 34 | `CauHinhGayQuy` | `BaoCao_MauTieuDe` | `string?` | 368 | Màu tiêu đề báo cáo |
| 35 | `CauHinhGayQuy` | `BaoCao_MauMoTa` | `string?` | 370 | Màu mô tả báo cáo |
| 36 | `CauHinhGayQuy` | `TaiTro_MauTieuDe` | `string?` | 378 | Màu tiêu đề đối tác tài trợ |
| 37 | `KenhDongGop` | `MauIcon` | `string?` | 398 | Màu biểu tượng kênh ủng hộ |
| 38 | `KenhDongGop` | `MauNenIcon` | `string?` | 399 | Màu nền biểu tượng |
| 39 | `KenhDongGop` | `MauTieuDe` | `string?` | 401 | Màu tiêu đề thẻ kênh |
| 40 | `KenhDongGop` | `MauMoTa` | `string?` | 402 | Màu mô tả thẻ kênh |
| 41 | `KenhDongGop` | `MauNenCard` | `string?` | 403 | Màu nền thẻ kênh ủng hộ |
| 42 | `CauHinhBanToChuc` | `MauNenStart` | `string?` | 453 | Màu nền bắt đầu BTC |
| 43 | `CauHinhBanToChuc` | `MauNenMid` | `string?` | 454 | Màu nền giữa BTC |
| 44 | `CauHinhBanToChuc` | `MauNenEnd` | `string?` | 455 | Màu nền kết thúc BTC |
| 45 | `CauHinhBanToChuc` | `SangLap_MauTieuDe` | `string?` | 461 | Màu tiêu đề ban sáng lập |
| 46 | `CauHinhBanToChuc` | `SoDo_MauTieuDe` | `string?` | 468 | Màu tiêu đề sơ đồ tổ chức |
| 47 | `CauHinhBanToChuc` | `SoDo_MauDuongKe` | `string?` | 470 | Màu đường nối cây tổ chức |
| 48 | `CauHinhBanToChuc` | `BTC_MauTieuDe` | `string?` | 476 | Màu tiêu đề danh sách BTC |
| 49 | `CauHinhBanToChuc` | `CTA_MauNutStart` | `string?` | 483 | Màu nút kêu gọi start |
| 50 | `CauHinhBanToChuc` | `CTA_MauNutEnd` | `string?` | 484 | Màu nút kêu gọi end |
| 51 | `ThanhVienBanToChuc` | `MauChuDaoThe` | `string?` | 518 | Màu chủ đạo thẻ nhân sự |
| 52 | `ThanhVienBanToChuc` | `MauNenThe` | `string?` | 519 | Màu nền thẻ nhân sự |
| 53 | `CauHinhTinTuc` | `MauChuDao` | `string?` | 539 | Màu chủ đạo tin tức |
| 54 | `CauHinhTinTuc` | `Hero_MauChuNhan` | `string?` | 543 | Màu chữ nhãn hero tin tức |
| 55 | `CauHinhTinTuc` | `Hero_MauNenNhan` | `string?` | 544 | Màu nền nhãn hero tin tức |
| 56 | `CauHinhTinTuc` | `Hero_MauTieuDe` | `string?` | 547 | Màu tiêu đề hero tin tức |
| 57 | `CauHinhTinTuc` | `Hero_MauMoTa` | `string?` | 551 | Màu mô tả hero tin tức |
| 58 | `CauHinhTinTuc` | `Hero_MauNenStart` | `string?` | 553 | Màu nền hero tin tức start |
| 59 | `CauHinhTinTuc` | `Hero_MauNenMid` | `string?` | 554 | Màu nền hero tin tức mid |
| 60 | `CauHinhTinTuc` | `Hero_MauNenEnd` | `string?` | 555 | Màu nền hero tin tức end |
| 61 | `CauHinhTinTuc` | `DanhSach_MauNenStart`| `string?`| 558 | Màu nền danh sách tin start |
| 62 | `CauHinhTinTuc` | `DanhSach_MauNenEnd` | `string?` | 559 | Màu nền danh sách tin end |
| 63 | `BaiVietTinTuc` | `MauNenTag` | `string?` | 576 | Màu nền badge bài viết |
| 64 | `BaiVietTinTuc` | `MauChuTag` | `string?` | 577 | Màu chữ badge bài viết |
| 65 | `BaiVietTinTuc` | `MauTieuDeThe` | `string?` | 586 | Màu tiêu đề trên card |
| 66 | `BaiVietTinTuc` | `MauTieuDePopup` | `string?` | 587 | Màu tiêu đề trong modal popup |

> **Ghi chú quan trọng về Phân hệ bán hàng**: Toàn bộ các bảng trong `Areas/CuaHang` (`SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`) **hoàn toàn không có bất kỳ cột màu nào**, thỏa mãn 100% nguyên tắc cô lập và bảo toàn phân hệ bán hàng theo yêu cầu R3.

### 1.3 Hiện trạng EF Core Migrations, Connection String & DbInitializer
1. **Connection String** (`appsettings.json` và `appsettings.Development.json`):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=TinhNguyen;User Id=sa;Password=Vuvinh2910@;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```
2. **Lịch sử Migrations hiện có**:
   - Gồm 13 bản migration liên tiếp trong thư mục `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/`:
     `20261006032230_AddBannerTable`, `20261006035646_AddChuongTrinhDemNguoc`, `20261006070510_AddVeDuAnColors`, `20261006074015_AddTinTucDetail`, `20261012070914_AddKyUcModule`, `20261012075303_AddTieuDeTongKet`, `20261012083652_AddChuongTrinhModule`, `20261012090043_FixLoTrinh`, `20261013005058_AddModuleGayQuy`, `20261013012535_AddModuleBanToChuc`, `20261013015906_AddModuleTinTuc`, `20261014034002_AddHieuUng`, `20261014042058_AddMascot`.
   - File Snapshot: `ApplicationDbContextModelSnapshot.cs` (1449 dòng) lưu trữ trạng thái hiện thời của model.
3. **Cơ chế khởi tạo & Fallback dự phòng (`DbInitializer.cs`)**:
   - File: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Helpers/DbInitializer.cs`.
   - Trong `Program.cs` (dòng 61-71):
     ```csharp
     try
     {
         using var scope = app.Services.CreateScope();
         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
         DbInitializer.Initialize(context);
     }
     catch (Exception ex)
     {
         app.Logger.LogWarning(ex, "Database initialization warning: SQL Server có thể đang offline, hệ thống sẽ sử dụng dữ liệu mẫu in-memory dự phòng.");
     }
     ```
   - Trong `DbInitializer.Seed()`: Kiểm tra `context.Database.CanConnect()`. Nếu không kết nối được SQL Server (ví dụ cổng 1433 đóng hoặc offline), mã nguồn tự động chuyển sang sử dụng `_inMemoryProducts` và `_inMemoryOrders`. Không làm ứng dụng bị crash khi mất kết nối SQL Server.

### 1.4 Danh mục các Bảng & Cột cần thuần hóa sang tiếng Việt chuẩn mực
Đối chiếu toàn bộ các bảng và cột của hệ thống với tiêu chí R1 ("Đổi tên toàn bộ bảng, cột trong SQL và cập nhật lại toàn bộ các câu truy vấn LINQ/SQL sang tiếng Việt thực tế trong dự án"):

1. **Bảng và Cột có yếu tố tiếng Anh / viết tắt cần chuẩn hóa**:
   - Bảng `BannerTrangChu`: Đổi sang tiếng Việt chuẩn thực tế `BieuNguTrangChu`.
     - Cột `IDBanner` -> `IDBieuNgu`
     - Cột `TieuDeAlt` -> `TieuDeThayThe`
   - Bảng `TVKhoanhKhac`: Đổi tên viết tắt `TV` sang `ThuVienKhoanhKhac`.
     - Cột `LinkAnh` -> `DuongDanAnh`
   - Bảng `FAQTrangChu`: Đổi tên tiếng Anh `FAQ` sang `CauHoiThuongGapTrangChu`.
     - Cột `IDFAQ` -> `IDCauHoi`
   - Bảng `CauHinhAnhNoi`:
     - Cột `DoTreAnimation` -> `DoTreHoatHoa` (hoặc `DoTreChuyenDong`)
     - Cột `ViTriTop` -> `ViTriTren`
     - Cột `TieuDeAlt` -> `TieuDeThayThe`
     - Cột `LinkAnh` -> `DuongDanAnh`
   - Bảng `CauHinhTrangChu`:
     - Cột `HienThiCanvas3D` -> `HienThiHieuUng3D`
     - Cột `HienThiMascot` -> `HienThiLinhVat`
     - Cột `LinkAnhMascot` -> `DuongDanAnhLinhVat`
     - Cột `ViTriMascot` -> `ViTriLinhVat`
     - Cột `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
     - Cột `LinkAnhBia` -> `DuongDanAnhBia`
     - Cột `VeDuAn_LinkAnh` -> `VeDuAn_DuongDanAnh`
   - Bảng `CauHinhBanToChuc`:
     - Tiền tố `CTA_...` (Call To Action) -> `KeuGoi_...` (`KeuGoi_MoTa`, `KeuGoi_ChuNut`, `KeuGoi_DuongDanNut`)
     - Cột `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
   - Bảng `CauHinhGayQuy`:
     - Tiền tố `Hero_...` -> `NoiBat_...` (`NoiBat_Nhan`, `NoiBat_TieuDe`, `NoiBat_CoChuTieuDe`, `NoiBat_MoTa`, `NoiBat_ChuNut`, `NoiBat_DuongDanNut`, `NoiBat_DuongDanAnh`)
     - Cột `BaoCao_TextNut`, `BaoCao_LinkNut` -> `BaoCao_ChuNut`, `BaoCao_DuongDanNut`
     - Cột `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
   - Bảng `KenhDongGop`:
     - Cột `LoaiIcon` -> `LoaiBieuTuong`
     - Cột `TextNut` -> `ChuNut`
     - Cột `LinkNgoai` -> `DuongDanNgoai`
     - Cột `LinkAnhQR` -> `DuongDanAnhQR`
   - Bảng `NhaTaiTroGayQuy`, `NhaTaiTroKyUc`, `NhaTaiTroTrangChu`:
     - Cột `LinkLogo` -> `DuongDanLogo`
     - Cột `LinkWebsite` -> `DuongDanTrangWeb`
   - Bảng `ThuVienHieuUng`:
     - Cột `LinkFileJS` -> `DuongDanTepJS`
   - Bảng `BaiVietTinTuc`:
     - Cột `LinkAnhDaiDien` -> `DuongDanAnhDaiDien`
     - Cột `LinkAnhTrongBai` -> `DuongDanAnhTrongBai`
     - Cột `LinkBaiVietGoc` -> `DuongDanBaiVietGoc`
2. **Phân hệ bán hàng (`Areas/CuaHang`)**:
   - `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`: Giữ nguyên vẹn 100% để tuân thủ triệt để R3 ("Bảo toàn 100% cấu trúc MVC/DOM và giữ nguyên toàn bộ giao diện hiện tại của phân hệ bán hàng").

---

## 2. Chuỗi lập luận (Logic Chain)

```
[Khảo sát hiện trạng code & CSDL]
 ├── Phát hiện 66 thuộc tính/cột màu sắc (MauChu, MauNen, MauChuDao, ...) tại 11 bảng
 ├── 6 Controllers Admin đang đọc/ghi 66 thuộc tính này qua form Edit.cshtml (<input type="color">)
 ├── 7 Views ngoài Client đang dùng inline-styles style="color: @Model.Mau..." để gán giao diện
 └── Database đang chứa một số bảng/cột tiếng Anh/viết tắt (Banner, FAQ, TV, Hero, CTA, Icon, Link)
         │
         ▼
[Đối chiếu với các yêu cầu tổng thể (R1 + R3 + R4)]
 ├── R1: Xóa sạch toàn bộ thuộc tính màu trong Models & DbContext, tạo EF Core Migration drop cột màu,
 │       đổi tên bảng/cột sang tiếng Việt thực tế, cập nhật lại toàn bộ LINQ/SQL.
 ├── R3: Làm mới giao diện sang phong cách hiện đại Tailwind CSS, mặc định nền trắng chữ mới,
 │       bảo toàn 100% phân hệ bán hàng (Areas/CuaHang/) và cô lập hoàn toàn.
 └── R4: Dark Mode chuyển đổi bằng JS và CSS variables (không dựa vào DB column).
         │
         ▼
[Kết luận kiến trúc]
 1. Việc lưu mã màu HEX trong Database là tàn tích kiến trúc cũ, gây cứng nhắc, không tương thích
    với hệ thống biến CSS tĩnh / Tailwind Dark Mode. Loại bỏ hoàn toàn là chuẩn xác.
 2. EF Core Code First Migration mới phải được cấu hình tường minh để DROP toàn bộ 66 cột màu
    và RENAME các bảng/cột sang tiếng Việt mà không làm gián đoạn hay mất dữ liệu cốt lõi.
 3. Các câu truy vấn LINQ Include trong HomeController, AdminTrangChuController,... phải được đồng bộ
    với tên Entity & Navigation Property mới.
 4. Phân hệ CuaHang phải được giữ nguyên vẹn cấu trúc bảng và mô hình thực thể.
```

---

## 3. Các lưu ý & Giả định (Caveats)

1. **Môi trường Database SQL Server**:
   - Máy chủ SQL Server hiện không mở cổng 1433 trực tiếp trong môi trường sandbox hiện tại. Ứng dụng đang tự động chạy qua cơ chế in-memory fallback của `DbInitializer`.
   - Do đó, bản Code First Migration và file script SQL phải được chuẩn bị đầy đủ cả về mặt mã nguồn C# EF Core (để ứng dụng tự động chạy khi kết nối database qua `context.Database.Migrate()`) lẫn tệp SQL script thủ công sẵn sàng thực thi khi bàn giao cho DBA.
2. **Quy tắc bảo toàn R3 đối với Areas/CuaHang**:
   - Yêu cầu R3 bắt buộc bảo toàn 100% cấu trúc MVC/DOM của `Areas/CuaHang/`. Vì vậy, không thực hiện bất kỳ thay đổi nào làm phá vỡ các trường thuộc tính của `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy` hoặc đường dẫn routing của Area này.
3. **Môi trường thực thi lệnh Build trên Linux Sandbox**:
   - Lệnh `dotnet build` trên hệ thống Ubuntu sandbox yêu cầu biến môi trường `XDG_RUNTIME_DIR=/tmp` và `DOTNET_CLI_HOME=/tmp` để tránh lỗi NuGet Mutex và Template Engine read-only.

---

## 4. Bản chiến lược thực thi chi tiết (Step-by-Step Blueprint)

Dưới đây là hướng dẫn kỹ thuật chi tiết từng bước dành cho Lập trình viên C# (.NET 10) để triển khai Yêu cầu R1:

### Bước 1: Xóa bỏ 66 thuộc tính màu trong Models và ViewModels

#### 1.1 Sửa file `WEB_TinhNguyen/Models/Entities.cs`:
- Trong class `CauHinhTrangChu`: Xóa 9 trường: `MauChuDao`, `MauNhan`, `MauChu`, `MauNenStart`, `MauNenMid`, `MauNenEnd`, `VeDuAn_MauTieuDe`, `VeDuAn_MauChu`, `VeDuAn_MauNenTag`.
- Trong class `TinTucTrangChu`: Xóa trường `MauTieuDe`.
- Trong class `ChuongTrinhDemNguoc`: Xóa 3 trường: `MauNenStart`, `MauNenEnd`, `MauChu`.
- Trong class `CauHinhKyUc`: Xóa 4 trường: `MauTieuDeTongKet`, `MauChuDao`, `MauNenHienTai`, `MauChu`.
- Trong class `CauHinhChuongTrinh`: Xóa 3 trường: `MauChuDao`, `MauNenHienTai`, `MauChu`.
- Trong class `CauHinhGayQuy`: Xóa 16 trường: `MauChuDao`, `Hero_MauChuNhan`, `Hero_MauNenNhan`, `Hero_MauTieuDe`, `Hero_MauMoTa`, `Hero_MauNutStart`, `Hero_MauNutEnd`, `Hero_MauNenStart`, `Hero_MauNenMid`, `Hero_MauNenEnd`, `DongGop_MauTieuDe`, `DongGop_MauMoTa`, `BaoCao_MauNen`, `BaoCao_MauTieuDe`, `BaoCao_MauMoTa`, `TaiTro_MauTieuDe`.
- Trong class `KenhDongGop`: Xóa 5 trường: `MauIcon`, `MauNenIcon`, `MauTieuDe`, `MauMoTa`, `MauNenCard`.
- Trong class `CauHinhBanToChuc`: Xóa 9 trường: `MauNenStart`, `MauNenMid`, `MauNenEnd`, `SangLap_MauTieuDe`, `SoDo_MauTieuDe`, `SoDo_MauDuongKe`, `BTC_MauTieuDe`, `CTA_MauNutStart`, `CTA_MauNutEnd`.
- Trong class `ThanhVienBanToChuc`: Xóa 2 trường: `MauChuDaoThe`, `MauNenThe`.
- Trong class `CauHinhTinTuc`: Xóa 10 trường: `MauChuDao`, `Hero_MauChuNhan`, `Hero_MauNenNhan`, `Hero_MauTieuDe`, `Hero_MauMoTa`, `Hero_MauNenStart`, `Hero_MauNenMid`, `Hero_MauNenEnd`, `DanhSach_MauNenStart`, `DanhSach_MauNenEnd`.
- Trong class `BaiVietTinTuc`: Xóa 4 trường: `MauNenTag`, `MauChuTag`, `MauTieuDeThe`, `MauTieuDePopup`.

#### 1.2 Sửa file `WEB_TinhNguyen/Models/CauHinhTrangChuVM.cs`:
- Xóa toàn bộ 66 thuộc tính tương ứng trên các class ViewModels (`CauHinhTrangChuVM`, `TinTucTrangChuVM`, `ChuongTrinhDemNguocVM`, `CauHinhKyUcVM`, `CauHinhChuongTrinhVM`, `CauHinhGayQuyVM`, `KenhDongGopVM`, `CauHinhBanToChucVM`, `ThanhVienBTC_VM`, `CauHinhTinTucVM`, `BaiVietTinTucVM`).

### Bước 2: Chuẩn hóa tên bảng & cột sang tiếng Việt trong Models & DbContext

1. **Đổi tên class & bảng trong `Entities.cs`**:
   - `BannerTrangChu` -> `BieuNguTrangChu` với `[Table("BieuNguTrangChu")]`
     - Cột `IDBanner` -> `IDBieuNgu`
     - Cột `TieuDeAlt` -> `TieuDeThayThe`
     - Cột `LinkAnh` -> `DuongDanAnh`
   - `TVKhoanhKhac` -> `ThuVienKhoanhKhac` với `[Table("ThuVienKhoanhKhac")]`
     - Cột `LinkAnh` -> `DuongDanAnh`
   - `FAQTrangChu` -> `CauHoiThuongGapTrangChu` với `[Table("CauHoiThuongGapTrangChu")]`
     - Cột `IDFAQ` -> `IDCauHoi`
   - Cập nhật các cột trong `CauHinhAnhNoi`:
     - `DoTreAnimation` -> `DoTreHoatHoa`
     - `ViTriTop` -> `ViTriTren`
     - `TieuDeAlt` -> `TieuDeThayThe`
     - `LinkAnh` -> `DuongDanAnh`
   - Cập nhật các cột trong `CauHinhTrangChu`:
     - `HienThiCanvas3D` -> `HienThiHieuUng3D`
     - `HienThiMascot` -> `HienThiLinhVat`
     - `LinkAnhMascot` -> `DuongDanAnhLinhVat`
     - `ViTriMascot` -> `ViTriLinhVat`
     - `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
     - `LinkAnhBia` -> `DuongDanAnhBia`
     - `VeDuAn_LinkAnh` -> `VeDuAn_DuongDanAnh`
   - Cập nhật các cột trong `CauHinhBanToChuc`:
     - `CTA_MoTa`, `CTA_TextNut`, `CTA_LinkNut` -> `KeuGoi_MoTa`, `KeuGoi_ChuNut`, `KeuGoi_DuongDanNut`
     - `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
   - Cập nhật các cột trong `CauHinhGayQuy`:
     - `Hero_Nhan`, `Hero_TieuDe`, `Hero_CoChuTieuDe`, `Hero_MoTa`, `Hero_TextNut`, `Hero_LinkNut`, `Hero_LinkAnh` -> `NoiBat_Nhan`, `NoiBat_TieuDe`, `NoiBat_CoChuTieuDe`, `NoiBat_MoTa`, `NoiBat_ChuNut`, `NoiBat_DuongDanNut`, `NoiBat_DuongDanAnh`
     - `BaoCao_TextNut`, `BaoCao_LinkNut` -> `BaoCao_ChuNut`, `BaoCao_DuongDanNut`
     - `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
   - Cập nhật các cột trong `KenhDongGop`:
     - `LoaiIcon` -> `LoaiBieuTuong`
     - `TextNut` -> `ChuNut`
     - `LinkNgoai` -> `DuongDanNgoai`
     - `LinkAnhQR` -> `DuongDanAnhQR`
   - Cập nhật các cột trong `CauHinhTinTuc`:
     - `Hero_Nhan`, `Hero_TieuDe`, `Hero_CoChuTieuDe`, `Hero_MoTa` -> `NoiBat_Nhan`, `NoiBat_TieuDe`, `NoiBat_CoChuTieuDe`, `NoiBat_MoTa`
     - `FontTieuDe`, `FontNoiDung` -> `KieuChuTieuDe`, `KieuChuNoiDung`
   - Cập nhật các cột trong `BaiVietTinTuc`:
     - `LinkAnhDaiDien`, `LinkAnhTrongBai`, `LinkBaiVietGoc` -> `DuongDanAnhDaiDien`, `DuongDanAnhTrongBai`, `DuongDanBaiVietGoc`
   - Cập nhật các cột trong `NhaTaiTroGayQuy`, `NhaTaiTroKyUc`, `NhaTaiTroTrangChu`:
     - `LinkLogo`, `LinkWebsite` -> `DuongDanLogo`, `DuongDanTrangWeb`
   - Cập nhật trong `ThuVienHieuUng`:
     - `LinkFileJS` -> `DuongDanTepJS`

2. **Cập nhật `ApplicationDbContext.cs`**:
   - Đổi `public DbSet<BannerTrangChu> BannerTrangChus { get; set; }` thành `public DbSet<BieuNguTrangChu> BieuNguTrangChus { get; set; }`.
   - Bổ sung `DbSet<ThuVienKhoanhKhac>` và `DbSet<CauHoiThuongGapTrangChu>`.

### Bước 3: Tạo bản Code First Migration và cập nhật ModelSnapshot

Tạo file migration mới tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/20261104000000_XoaCotMauVaVietHoaTenBangCot.cs`:

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_TinhNguyen.Migrations
{
    public partial class XoaCotMauVaVietHoaTenBangCot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. XÓA 9 CỘT MÀU TRONG BẢNG CauHinhTrangChu
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNhan", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenStart", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenMid", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauTieuDe", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauChu", table: "CauHinhTrangChu");
            migrationBuilder.DropColumn(name: "VeDuAn_MauNenTag", table: "CauHinhTrangChu");

            // 2. XÓA CỘT MÀU TRONG BẢNG TinTucTrangChu
            migrationBuilder.DropColumn(name: "MauTieuDe", table: "TinTucTrangChu");

            // 3. XÓA 3 CỘT MÀU TRONG BẢNG ChuongTrinhDemNguoc
            migrationBuilder.DropColumn(name: "MauNenStart", table: "ChuongTrinhDemNguoc");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "ChuongTrinhDemNguoc");
            migrationBuilder.DropColumn(name: "MauChu", table: "ChuongTrinhDemNguoc");

            // 4. XÓA 4 CỘT MÀU TRONG BẢNG CauHinhKyUc
            migrationBuilder.DropColumn(name: "MauTieuDeTongKet", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauNenHienTai", table: "CauHinhKyUc");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhKyUc");

            // 5. XÓA 3 CỘT MÀU TRONG BẢNG CauHinhChuongTrinh
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhChuongTrinh");
            migrationBuilder.DropColumn(name: "MauNenHienTai", table: "CauHinhChuongTrinh");
            migrationBuilder.DropColumn(name: "MauChu", table: "CauHinhChuongTrinh");

            // 6. XÓA 16 CỘT MÀU TRONG BẢNG CauHinhGayQuy
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauChuNhan", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenNhan", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNutStart", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNutEnd", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenStart", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenMid", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "Hero_MauNenEnd", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "DongGop_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "DongGop_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauNen", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauTieuDe", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "BaoCao_MauMoTa", table: "CauHinhGayQuy");
            migrationBuilder.DropColumn(name: "TaiTro_MauTieuDe", table: "CauHinhGayQuy");

            // 7. XÓA 5 CỘT MÀU TRONG BẢNG KenhDongGop
            migrationBuilder.DropColumn(name: "MauIcon", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauNenIcon", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauTieuDe", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauMoTa", table: "KenhDongGop");
            migrationBuilder.DropColumn(name: "MauNenCard", table: "KenhDongGop");

            // 8. XÓA 9 CỘT MÀU TRONG BẢNG CauHinhBanToChuc
            migrationBuilder.DropColumn(name: "MauNenStart", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenMid", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenEnd", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SangLap_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SoDo_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "SoDo_MauDuongKe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "BTC_MauTieuDe", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "CTA_MauNutStart", table: "CauHinhBanToChuc");
            migrationBuilder.DropColumn(name: "CTA_MauNutEnd", table: "CauHinhBanToChuc");

            // 9. XÓA 2 CỘT MÀU TRONG BẢNG ThanhVienBanToChuc
            migrationBuilder.DropColumn(name: "MauChuDaoThe", table: "ThanhVienBanToChuc");
            migrationBuilder.DropColumn(name: "MauNenThe", table: "ThanhVienBanToChuc");

            // 10. XÓA 10 CỘT MÀU TRONG BẢNG CauHinhTinTuc
            migrationBuilder.DropColumn(name: "MauChuDao", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauChuNhan", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenNhan", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauTieuDe", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauMoTa", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenStart", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenMid", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "Hero_MauNenEnd", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "DanhSach_MauNenStart", table: "CauHinhTinTuc");
            migrationBuilder.DropColumn(name: "DanhSach_MauNenEnd", table: "CauHinhTinTuc");

            // 11. XÓA 4 CỘT MÀU TRONG BẢNG BaiVietTinTuc
            migrationBuilder.DropColumn(name: "MauNenTag", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauChuTag", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauTieuDeThe", table: "BaiVietTinTuc");
            migrationBuilder.DropColumn(name: "MauTieuDePopup", table: "BaiVietTinTuc");

            // 12. ĐỔI TÊN BẢNG TIẾNG ANH / VIẾT TẮT SANG TIẾNG VIỆT
            migrationBuilder.RenameTable(name: "BannerTrangChu", newName: "BieuNguTrangChu");
            migrationBuilder.RenameTable(name: "TVKhoanhKhac", newName: "ThuVienKhoanhKhac");
            migrationBuilder.RenameTable(name: "FAQTrangChu", newName: "CauHoiThuongGapTrangChu");

            // 13. ĐỔI TÊN CỘT
            migrationBuilder.RenameColumn(name: "IDBanner", table: "BieuNguTrangChu", newName: "IDBieuNgu");
            migrationBuilder.RenameColumn(name: "TieuDeAlt", table: "BieuNguTrangChu", newName: "TieuDeThayThe");
            migrationBuilder.RenameColumn(name: "IDFAQ", table: "CauHoiThuongGapTrangChu", newName: "IDCauHoi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Hoàn nguyên đổi tên bảng
            migrationBuilder.RenameTable(name: "BieuNguTrangChu", newName: "BannerTrangChu");
            migrationBuilder.RenameTable(name: "ThuVienKhoanhKhac", newName: "TVKhoanhKhac");
            migrationBuilder.RenameTable(name: "CauHoiThuongGapTrangChu", newName: "FAQTrangChu");
        }
    }
}
```

Và cập nhật tương ứng file Designer `20261104000000_XoaCotMauVaVietHoaTenBangCot.Designer.cs` cùng `ApplicationDbContextModelSnapshot.cs`.

### Bước 4: Tự động chạy Migration trong `Program.cs`
Cập nhật khối khởi tạo trong `Program.cs`:
```csharp
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (context.Database.CanConnect())
    {
        context.Database.Migrate();
    }
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Database initialization warning: SQL Server có thể đang offline, hệ thống sẽ sử dụng dữ liệu mẫu in-memory dự phòng.");
}
```

### Bước 5: Cập nhật Controllers & các câu truy vấn LINQ/SQL
1. **`HomeController.cs`**:
   - Cập nhật câu truy vấn `Index()`:
     ```csharp
     var activeConfig = await _context.CauHinhTrangChus
         .Include(c => c.ChienDich)
         .Include(c => c.BieuNguTrangChus)
         .Include(c => c.ChuongTrinhDemNguocs)
         .Include(c => c.CauHinhAnhNois)
         .Include(c => c.ThuVienKhoanhKhacs)
         .Include(c => c.TinTucs)
         .Include(c => c.NhaTaiTros)
         .Include(c => c.CauHoiThuongGaps)
         .Where(c => c.ChienDich.TrangThaiHoatDong == true)
         .AsSplitQuery()
         .FirstOrDefaultAsync();
     ```
2. **`AdminTrangChuController.cs`**:
   - Dòng 175-190: Xóa toàn bộ gán thuộc tính màu trong GET `Edit()`.
   - Dòng 227-254: Xóa toàn bộ gán thuộc tính màu trong POST `Edit()`.
   - Thay `_context.Set<BannerTrangChu>()` thành `_context.Set<BieuNguTrangChu>()`.
3. **`AdminChuongTrinhController.cs`**:
   - Dòng 75-77 & 114-116: Xóa gán `MauChuDao`, `MauNenHienTai`, `MauChu`.
4. **`AdminGayQuyController.cs`**:
   - Dòng 65-83, 108-146, 198-243, 265-269: Xóa gán 16 trường màu và 5 trường màu của `KenhDongGops`.
5. **`AdminBanToChucController.cs`**:
   - Dòng 64-68, 104-131, 152-153, 169-196, 234-235: Xóa gán 9 trường màu và 2 trường màu của thành viên.
6. **`AdminTinTucController.cs`**:
   - Dòng 192-222, 245-291: Xóa gán 10 trường màu và 4 trường màu của bài viết.
7. **`AdminKyUcController.cs`**:
   - Dòng 72-80 & 117-120: Xóa gán 4 trường màu.

### Bước 6: Dọn dẹp Views
- **Admin Views**: Xóa toàn bộ các thẻ `<input type="color" ...>` và ô nhập mã màu HEX khỏi các form trong:
  - `Views/AdminTrangChu/Edit.cshtml`
  - `Views/AdminChuongTrinh/Edit.cshtml`
  - `Views/AdminGayQuy/Edit.cshtml`
  - `Views/AdminBanToChuc/Edit.cshtml`
  - `Views/AdminTinTuc/Edit.cshtml`
  - `Views/AdminKyUc/Edit.cshtml`
- **Client Views & `_Layout.cshtml`**:
  - Xóa dòng 19-22 trong `_Layout.cshtml` (`bgWeb`, `colorMain`, `colorSub`, `colorText`).
  - Chuyển giao việc định nghĩa biến màu cho CSS stylesheet `~/css/site.css` phục vụ Chế độ Sáng/Tối (Light/Dark Mode) theo yêu cầu R3 và R4.

---

## 5. Phương pháp Xác minh Độc lập (Verification Method)

Người kiểm thử, Reviewer hoặc Orchestrator có thể thực hiện kiểm tra độc lập theo các bước sau:

1. **Biên dịch dự án**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Tiêu chí đạt*: Biên dịch thành công 100%, `0 Error(s)`.

2. **Kiểm tra không còn thuộc tính màu sắc trong Models**:
   ```bash
   grep -rn "MauChu" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/Entities.cs
   grep -rn "MauNen" /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Models/Entities.cs
   ```
   *Tiêu chí đạt*: Không trả về bất kỳ kết quả nào.

3. **Kiểm tra bản Code First Migration mới**:
   - Kiểm tra thư mục `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Migrations/`: Có file migration mới với đầy đủ các lệnh `DropColumn` cho 66 cột màu và `RenameTable`/`RenameColumn` cho các bảng/cột tiếng Việt.
   - Kiểm tra `ApplicationDbContextModelSnapshot.cs`: Không còn chứa thuộc tính màu sắc nào.

4. **Kiểm tra tính toàn vẹn của phân hệ bán hàng**:
   ```bash
   git status /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/
   ```
   *Tiêu chí đạt*: Cấu trúc DOM và Controller/View của phân hệ bán hàng được bảo toàn nguyên vẹn 100%.

5. **Kiểm tra độ sẵn sàng của Fallback / Mock Data**:
   - Kiểm tra `DbInitializer.cs`: Vẫn đảm bảo phương thức `Seed(context)` hoạt động an toàn kể cả khi database offline.
