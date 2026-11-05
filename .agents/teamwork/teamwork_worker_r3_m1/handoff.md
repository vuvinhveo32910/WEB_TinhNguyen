# BÁO CÁO BÀN GIAO MILESTONE M1 (YÊU CẦU R2)
**Người thực hiện**: C# .NET 10 Developer (`teamwork_worker_r3_m1`)  
**Nhiệm vụ**: Việt hóa cấu trúc thư mục MVC, cấu hình Razor ViewEngine 2 tầng, cập nhật layout path, namespace, và bảo đảm biên dịch 100% không lỗi.  
**Thời điểm hoàn thành**: 2026-11-04T02:51:30Z  

---

## 1. Observation (Quan Sát Thực Nghiệm & Bằng Chứng Mã Nguồn)

1. **Hiện trạng cấu trúc trước khi thực hiện**:
   - `WEB_TinhNguyen/Controllers` chứa 9 controllers kế thừa `Controller`.
   - `WEB_TinhNguyen/Models` chứa 3 file model (`CauHinhTrangChuVM.cs`, `Entities.cs`, `ErrorViewModel.cs`).
   - `WEB_TinhNguyen/Views` chứa 31 file (29 view/asset + `_ViewImports.cshtml`, `_ViewStart.cshtml`) trong 10 thư mục con.
   - `WEB_TinhNguyen/Data` chứa `ApplicationDbContext.cs`.
   - `WEB_TinhNguyen/Areas/CuaHang` chứa 13 tệp mã nguồn và 11 tệp view/start/import hoàn chỉnh.

2. **Kết quả đổi tên thư mục sang tiếng Việt**:
   - `WEB_TinhNguyen/Controllers` -> `WEB_TinhNguyen/BoDieuKhien`
   - `WEB_TinhNguyen/Models` -> `WEB_TinhNguyen/MoHinh`
   - `WEB_TinhNguyen/Views` -> `WEB_TinhNguyen/GiaoDien`
   - `WEB_TinhNguyen/Data` -> `WEB_TinhNguyen/DuLieu`
   - Phân hệ `WEB_TinhNguyen/Areas/CuaHang` được bảo toàn nguyên vẹn 100% cấu trúc: `Controllers/`, `Helpers/`, `Models/`, `Views/`.

3. **Hiện trạng Namespace & Lớp tương thích EF Core**:
   - Cả 9 controller trong `BoDieuKhien/` đã được chuyển sang `namespace WEB_TinhNguyen.BoDieuKhien` và nạp `using WEB_TinhNguyen.DuLieu;`, `using WEB_TinhNguyen.MoHinh;`.
   - 3 file trong `MoHinh/` đã chuyển sang `namespace WEB_TinhNguyen.MoHinh`.
   - `DuLieu/ApplicationDbContext.cs` đã chuyển sang `namespace WEB_TinhNguyen.DuLieu`.
   - Tạo mới `WEB_TinhNguyen/DuLieu/Compatibility.cs` định nghĩa `global using WEB_TinhNguyen.DuLieu;`, `global using WEB_TinhNguyen.MoHinh;`, `global using WEB_TinhNguyen.BoDieuKhien;` và bảo toàn `namespace WEB_TinhNguyen.Data` để 14 migration EF Core cũ trong `Migrations/` không bị lỗi biên dịch CS0234.

4. **Bộ mở rộng Razor ViewLocation & Cấu hình Program.cs**:
   - Tạo mới `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs` hiện thực interface `IViewLocationExpander`.
   - Cập nhật `WEB_TinhNguyen/Program.cs` cấu hình 2 tầng: xóa định dạng mặc định, bổ sung `/GiaoDien/{1}/{0}.cshtml`, `/GiaoDien/Shared/{0}.cshtml`, hỗ trợ fallback an toàn `/Views/...`, và đăng ký `VietnameseViewLocationExpander` vào `RazorViewEngineOptions.ViewLocationExpanders`.

5. **22 vị trí Layout Path Hardcoded**:
   - Quét grep ban đầu phát hiện chính xác 22 vị trí hardcoded `~/Views/Shared/_AdminLayout.cshtml` và `~/Views/Shared/_Layout.cshtml` (7 vị trí trong `Areas/CuaHang/Views/` và 15 vị trí trong `GiaoDien/`).
   - Cả 22 vị trí đã được chuyển đổi 100% thành `~/GiaoDien/Shared/_AdminLayout.cshtml` và `~/GiaoDien/Shared/_Layout.cshtml`.
   - Lệnh `grep -rn "~/Views/Shared" WEB_TinhNguyen` trả về 0 kết quả.
   - Cập nhật `_ViewImports.cshtml` tại `GiaoDien/` và `Areas/CuaHang/Views/` sang `@using WEB_TinhNguyen.MoHinh`.

6. **Cấu hình Tailwind CSS**:
   - Tệp `tailwind.config.js` tại dòng 3-10 đã được bổ sung `"./GiaoDien/**/*.cshtml"` và `"./WEB_TinhNguyen/GiaoDien/**/*.cshtml"`.

7. **Kiểm tra biên dịch**:
   - Chạy lệnh `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj`:
     - Kết quả: `Build succeeded.` với **0 Error(s)**.
     - Target `TailwindCSS` chạy thành công `npm run build:css` và biên dịch xong CSS trong 2.2s.

---

## 2. Logic Chain (Lập Luận Phân Tích & Giải Pháp Kỹ Thuật)

1. Từ **Observation 1 & 2**: ASP.NET Core phân giải Controllers thông qua Reflection trên assembly dựa vào hậu tố `*Controller`, hoàn toàn độc lập với tên thư mục vật lý. Việc chuyển `Controllers/` sang `BoDieuKhien/` không làm thay đổi các quy tắc routing của ứng dụng (`/Home/Index`, `/Admin/Index`, `/CuaHang`).
2. Từ **Observation 4**: Khác với Controllers, Razor ViewEngine phân giải file view dựa trên các quy tắc đường dẫn `ViewLocationFormats`. Bằng cách cấu hình đồng thời 2 tầng: thiết lập tĩnh trong `Program.cs` và bộ mở rộng động `VietnameseViewLocationExpander`, ViewEngine luôn ưu tiên tìm kiếm view trong `/GiaoDien/` trước khi tìm fallback trong `/Views/`.
3. Từ **Observation 3**: 14 migration EF Core cũ trong thư mục `Migrations/` có các lệnh `using WEB_TinhNguyen.Data;` và khai báo thuộc tính `[DbContext(typeof(ApplicationDbContext))]`. Nếu xóa hoàn toàn `namespace WEB_TinhNguyen.Data`, trình biên dịch Roslyn sẽ báo lỗi CS0234. Giải pháp `DuLieu/Compatibility.cs` duy trì `namespace WEB_TinhNguyen.Data` rỗng và kích hoạt `global using WEB_TinhNguyen.DuLieu;`, giúp mọi tham chiếu `ApplicationDbContext` trong migration tự động phân giải về `WEB_TinhNguyen.DuLieu.ApplicationDbContext` mà không gây xung đột hay đa nghĩa (CS0104).
4. Từ **Observation 5**: Các layout path tuyệt đối `~/Views/Shared/...` nếu không được sửa sang `~/GiaoDien/Shared/...` sẽ gây lỗi runtime crash 500 (`InvalidOperationException: The layout view '~/Views/Shared/_Layout.cshtml' could not be found`). Việc thay thế triệt để 22 vị trí đảm bảo tính toàn vẹn runtime.
5. Từ **Observation 6 & 7**: Dự án có cơ chế tự động biên dịch Tailwind CSS trước mỗi lần build (`BeforeTargets="Build"`). Bổ sung `./GiaoDien/**/*.cshtml` vào `tailwind.config.js` giúp Tailwind CLI quét đúng các tệp giao diện mới và trích xuất đầy đủ các CSS classes, bảo toàn giao diện trang.

---

## 3. Caveats (Các Điểm Lưu Ý)

1. **Môi trường Sandbox**: Lệnh `dotnet build` trên môi trường sandbox Linux yêu cầu biến môi trường `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp` để tránh lỗi quyền ghi `lstat("/run/user/1000")`.
2. **Các bước tiếp theo (Milestone M2 & M3)**:
   - Milestone M2 (Kiến trúc sư CSDL EF Core) sẽ tiếp tục tái cấu trúc `MoHinh/Entities.cs` và `DuLieu/ApplicationDbContext.cs` để loại bỏ 66 cột màu sắc và thuần hóa tiếng Việt cho các bảng/cột CSDL.
   - Milestone M3 (Chuyên gia UI/UX Tailwind CSS) sẽ tiếp tục làm mới giao diện tại `GiaoDien/` và thiết lập `_CuaHangLayout.cshtml` tại `Areas/CuaHang/Views/Shared/`.

---

## 4. Conclusion (Kết Luận)

Milestone M1 (Yêu cầu R2) đã hoàn thành xuất sắc, đầy đủ 100% các tiêu chí nghiệm thu:
- Cấu trúc thư mục MVC đã được chuyển đổi sang tiếng Việt chuẩn mực: `BoDieuKhien`, `MoHinh`, `GiaoDien`, `DuLieu`.
- Phân hệ `Areas/CuaHang` được bảo toàn 100% nguyên vẹn.
- Namespaces và cơ chế tương thích ngược cho Migrations hoạt động trơn tru.
- Razor ViewEngine 2 tầng và VietnameseViewLocationExpander đã được triển khai hoàn chỉnh.
- Toàn bộ 22 vị trí hardcoded layout path đã được cập nhật sang `~/GiaoDien/Shared/...`.
- `tailwind.config.js` đã nhận diện cấu trúc `GiaoDien`.
- Dự án biên dịch thành công 100% với 0 Error.

---

## 5. Verification Method (Quy Trình Kiểm Tra Độc Lập)

Đội ngũ QA / Auditor có thể thực hiện kiểm chứng độc lập bằng các lệnh sau:

1. **Kiểm tra biên dịch**:
   ```bash
   cd /home/hutech/Vinh_Offline/WEB_TinhNguyen
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Kỳ vọng*: `Build succeeded. 0 Error(s)`. Tailwind CSS biên dịch hoàn tất.

2. **Kiểm tra không còn đường dẫn ~/Views/Shared cũ**:
   ```bash
   cd /home/hutech/Vinh_Offline/WEB_TinhNguyen
   grep -rn "~/Views/Shared" WEB_TinhNguyen
   ```
   *Kỳ vọng*: 0 kết quả trả về.

3. **Kiểm tra 22 đường dẫn ~/GiaoDien/Shared mới**:
   ```bash
   cd /home/hutech/Vinh_Offline/WEB_TinhNguyen
   grep -rn "~/GiaoDien/Shared" WEB_TinhNguyen | wc -l
   ```
   *Kỳ vọng*: Trả về đúng 22 dòng.

4. **Kiểm tra tính toàn vẹn của Areas/CuaHang**:
   ```bash
   ls WEB_TinhNguyen/Areas/CuaHang
   ```
   *Kỳ vọng*: Có đầy đủ 4 thư mục `Controllers`, `Helpers`, `Models`, `Views`.
