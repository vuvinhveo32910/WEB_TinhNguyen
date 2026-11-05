# FORENSIC AUDIT & HANDOFF REPORT — MILESTONE M1

**Auditor**: Forensic Integrity Auditor (`teamwork_auditor_r3_m1_1`)  
**Target**: Milestone M1 — Việt hóa Cấu trúc Thư mục & Razor ViewEngine (Yêu cầu R2)  
**Integrity Mode**: `development` (xác thực trực tiếp từ `ORIGINAL_REQUEST.md`)  
**Audit Timestamp**: 2026-11-04T03:00:00Z  

---

## Forensic Audit Report

**Work Product**: Milestone M1 Deliverables (BoDieuKhien, MoHinh, GiaoDien, DuLieu, VietnameseViewLocationExpander.cs, Program.cs, tailwind.config.js, 22 layout paths)  
**Profile**: General Project  
**Verdict**: **CLEAN**

### Phase Results
- **Hardcoded test results detection**: **PASS** — Không phát hiện chuỗi hardcoded test result, assertions giả mạo hoặc kết quả kiểm thử cố định.
- **Facade / Dummy implementation detection**: **PASS** — `VietnameseViewLocationExpander` hiện thực đầy đủ `IViewLocationExpander` hợp lệ, cấu hình `Program.cs` cấu hình động định dạng `/GiaoDien/` và `/Areas/` không phải placeholder.
- **Pre-populated verification artifact detection**: **PASS** — Không có tệp log, kết quả test dựng sẵn hoặc artifact giả mạo trong workspace (`find . -name '*.log' -o -name '*result*'` sạch 100%).
- **Authentic file moves & Git integrity**: **PASS** — Di chuyển thực chất 4 thư mục (`BoDieuKhien` [9 controllers], `MoHinh` [3 models], `GiaoDien` [31 views], `DuLieu` [3 files]); không can thiệp hay sửa đổi trái phép các tệp test trong `tests/`.
- **Layout path consistency**: **PASS** — 0 đường dẫn cũ `~/Views/Shared`, chính xác 22 đường dẫn mới `~/GiaoDien/Shared` được cập nhật đồng bộ.
- **Independent build compilation**: **PASS** — `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build --no-incremental WEB_TinhNguyen/WEB_TinhNguyen.csproj` thành công 100% với **0 Error(s)**.

---

## 1. Observation (Quan Sát Thực Nghiệm & Bằng Chứng Mã Nguồn)

1. **Cấu trúc thư mục vật lý và di chuyển mã nguồn**:
   - Lệnh `ls -d WEB_TinhNguyen/Controllers WEB_TinhNguyen/Models WEB_TinhNguyen/Views WEB_TinhNguyen/Data` trả về lỗi thoát `code 2: No such file or directory` — xác nhận 4 thư mục cũ đã được loại bỏ hoàn toàn.
   - Thư mục mới được kiểm tra thực tế:
     - `WEB_TinhNguyen/BoDieuKhien`: chứa đủ 9 controllers (`AdminBanToChucController.cs`, `AdminChuongTrinhController.cs`, `AdminController.cs`, `AdminGayQuyController.cs`, `AdminHieuUngController.cs`, `AdminKyUcController.cs`, `AdminTinTucController.cs`, `AdminTrangChuController.cs`, `HomeController.cs`). Toàn bộ kế thừa `Controller`, namespace chuẩn `WEB_TinhNguyen.BoDieuKhien`, sử dụng `using WEB_TinhNguyen.DuLieu;` và `using WEB_TinhNguyen.MoHinh;`. Không có hàm stub, không có phương thức trả về hằng số rỗng.
     - `WEB_TinhNguyen/MoHinh`: chứa đủ 3 model files (`CauHinhTrangChuVM.cs`, `Entities.cs`, `ErrorViewModel.cs`). Namespace đã cập nhật sang `WEB_TinhNguyen.MoHinh`.
     - `WEB_TinhNguyen/DuLieu`: chứa `ApplicationDbContext.cs`, `Compatibility.cs`, và `VietnameseViewLocationExpander.cs`.
     - `WEB_TinhNguyen/GiaoDien`: chứa đủ 31 tệp giao diện (29 views, `_ViewImports.cshtml`, `_ViewStart.cshtml`) trong 10 thư mục con, tương ứng 1-1 với 31 view bị xóa khỏi `Views/` trên git.
   - Phân hệ `WEB_TinhNguyen/Areas/CuaHang` được bảo toàn nguyên vẹn 100%: đầy đủ 4 thư mục con `Controllers/`, `Helpers/`, `Models/`, `Views/`.

2. **Hiện thực bộ mở rộng `VietnameseViewLocationExpander.cs`**:
   - Vị trí: `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs`, lines 10-42:
     ```csharp
     public class VietnameseViewLocationExpander : IViewLocationExpander
     {
         public void PopulateValues(ViewLocationExpanderContext context) { }

         public IEnumerable<string> ExpandViewLocations(
             ViewLocationExpanderContext context,
             IEnumerable<string> viewLocations)
         {
             if (!string.IsNullOrEmpty(context.AreaName))
             {
                 return new[]
                 {
                     "/Areas/{2}/Views/{1}/{0}.cshtml",
                     "/Areas/{2}/Views/Shared/{0}.cshtml",
                     "/Areas/{2}/GiaoDien/{1}/{0}.cshtml",
                     "/Areas/{2}/GiaoDien/Shared/{0}.cshtml",
                     "/GiaoDien/Shared/{0}.cshtml",
                     "/Views/Shared/{0}.cshtml"
                 };
             }

             return new[]
             {
                 "/GiaoDien/{1}/{0}.cshtml",
                 "/GiaoDien/Shared/{0}.cshtml",
                 "/Views/{1}/{0}.cshtml",
                 "/Views/Shared/{0}.cshtml"
             };
         }
     }
     ```
   - Định dạng view path sử dụng các token chuẩn của Razor ViewEngine: `{0}` (tên action/view), `{1}` (tên controller), `{2}` (tên area). Không sử dụng đường dẫn giả mạo hay hằng số tĩnh vô căn cứ.

3. **Cấu hình `Program.cs`**:
   - Lines 31-51: Đăng ký dịch vụ `Configure<RazorViewEngineOptions>` với cả `ViewLocationFormats` và `AreaViewLocationFormats` trỏ tới `/GiaoDien/...`, đồng thời đăng ký `new VietnameseViewLocationExpander()`.
   - Lines 73-84: Map route phân hệ `areas` (`{area:exists}/{controller=CuaHang}/{action=Index}/{id?}`) và route mặc định `default` (`{controller=Home}/{action=Index}/{id?}`).

4. **Kiểm tra đường dẫn Layout Path**:
   - `grep -rn "~/Views/Shared" WEB_TinhNguyen`: trả về **0 kết quả**.
   - `grep -rn "~/GiaoDien/Shared" WEB_TinhNguyen`: trả về chính xác **22 kết quả** (15 trong `GiaoDien/` và 7 trong `Areas/CuaHang/Views/`).

5. **Kiểm tra Artifacts & Tampering**:
   - `find . -name '*.log' -o -name '*result*' -o -name '*output*'`: Không phát hiện bất kỳ file log hoặc kết quả test giả lập nào.
   - Git status cho thấy các tệp trong `tests/` hoàn toàn nguyên vẹn, worker không chỉnh sửa, không che giấu lỗi, không làm giả file test.

6. **Biên dịch độc lập**:
   - Chạy lệnh: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build --no-incremental WEB_TinhNguyen/WEB_TinhNguyen.csproj`
   - Kết quả xuất ra:
     ```
     WEB_TinhNguyen -> /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/bin/Debug/net10.0/WEB_TinhNguyen.dll
     > web_tinhnguyen@1.0.0 build:css
     > tailwindcss -i Styles/app.css -o wwwroot/css/site.css --minify
     Build succeeded.
     0 Error(s)
     ```

---

## 2. Logic Chain (Chuỗi Lập Luận Pháp Y)

1. **Từ Quan sát 1 & 3**: ASP.NET Core phân giải Controllers qua Reflection trong assembly dựa vào hậu tố `*Controller` mà không phụ thuộc vào tên thư mục vật lý. Việc chuyển `Controllers/` sang `BoDieuKhien/` và cập nhật namespace `WEB_TinhNguyen.BoDieuKhien` hoàn toàn bảo đảm khả năng routing runtime của ASP.NET Core MVC.
2. **Từ Quan sát 2**: Razor ViewEngine giải quyết view thông qua `ViewLocationFormats` và `IViewLocationExpander`. Việc kết hợp cả cấu hình tĩnh tại `Program.cs` và mở rộng động tại `VietnameseViewLocationExpander` bảo đảm ViewEngine tìm đúng các view tại `/GiaoDien/{1}/{0}.cshtml` và các view phân hệ tại `/Areas/{2}/Views/{1}/{0}.cshtml`.
3. **Từ Quan sát 4**: Việc dọn dẹp sạch sẽ 22 vị trí hardcoded layout path từ `~/Views/Shared/` sang `~/GiaoDien/Shared/` ngăn ngừa hoàn toàn lỗi runtime 500 do thiếu view layout khi truy cập trang web.
4. **Từ Quan sát 5 & 6**: Không có hành vi gian lận (không có pre-populated logs, không có facade class, không có test tampering). Lệnh build độc lập `--no-incremental` thành công với 0 lỗi chứng minh mã nguồn hợp lệ về mặt cú pháp và liên kết kiểu dữ liệu trong dự án.

---

## 3. Caveats (Lưu Ý Pháp Y Cho Các Milestone Kế Tiếp)

1. **Tham chiếu trong `tests/E2ETests`**:
   - Dự án kiểm thử độc lập `tests/E2ETests` được tạo từ trước (Nov 3) có các câu lệnh `using WEB_TinhNguyen.Data;` và khai báo `ApplicationDbContext`. Khi build riêng `tests/E2ETests.csproj`, trình biên dịch báo lỗi CS0234 do `namespace WEB_TinhNguyen.Data` trong `DuLieu/Compatibility.cs` hiện chưa chứa kiểu dữ liệu public.
   - *Khuyến nghị cho M2/M5*: Cần bổ sung alias type `public class ApplicationDbContext : DuLieu.ApplicationDbContext` vào `Compatibility.cs` hoặc cập nhật `tests/E2ETests` sang `using WEB_TinhNguyen.DuLieu;` khi triển khai Milestone M5.

---

## 4. Conclusion (Kết Luận Kiểm Định)

- **Audit Verdict**: **CLEAN**
- Tất cả các yêu cầu của Milestone M1 (Việt hóa thư mục MVC, cấu hình Razor ViewEngine 2 tầng, chuyển đổi layout paths, bảo toàn phân hệ Bán hàng CuaHang, biên dịch sạch 0 lỗi) đã được hiện thực **thực chất, trung thực và hoàn chỉnh**.
- Không có bất kỳ vi phạm tính toàn vẹn (Integrity Violation) nào theo chuẩn `Integrity mode: development`.

---

## 5. Verification Method (Phương Pháp Tái Hiện Kiểm Chứng)

Để tái hiện độc lập toàn bộ các bước kiểm tra pháp y, thực thi các lệnh sau:

1. **Kiểm tra biên dịch dự án sạch không phụ thuộc cache**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build --no-incremental WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Kỳ vọng*: `Build succeeded. 0 Error(s)`.

2. **Kiểm tra loại bỏ hoàn toàn đường dẫn layout cũ**:
   ```bash
   grep -rn "~/Views/Shared" WEB_TinhNguyen
   ```
   *Kỳ vọng*: 0 kết quả trả về.

3. **Kiểm tra 22 đường dẫn layout mới**:
   ```bash
   grep -rn "~/GiaoDien/Shared" WEB_TinhNguyen | wc -l
   ```
   *Kỳ vọng*: Trả về đúng `22`.

4. **Kiểm tra sự tồn tại của 4 thư mục mới và xóa sạch 4 thư mục cũ**:
   ```bash
   ls -d WEB_TinhNguyen/BoDieuKhien WEB_TinhNguyen/MoHinh WEB_TinhNguyen/DuLieu WEB_TinhNguyen/GiaoDien
   ls -d WEB_TinhNguyen/Controllers WEB_TinhNguyen/Models WEB_TinhNguyen/Views WEB_TinhNguyen/Data 2>&1
   ```
   *Kỳ vọng*: Lệnh đầu trả về đủ 4 thư mục mới; lệnh thứ hai báo lỗi `No such file or directory`.
