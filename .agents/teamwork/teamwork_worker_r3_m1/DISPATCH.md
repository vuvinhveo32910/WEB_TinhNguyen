# DISPATCH — Worker M1 (C# .NET 10 Developer)

**Role**: C# .NET 10 Developer (Lập trình viên C# - .NET 10)
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
**Project Blueprint**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
**Explorer 2 Architectural Handoff**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2/handoff.md

## Scope & Assigned Milestone: M1 (Việt hóa cấu trúc thư mục và cấu hình MVC - Requirement R2)

### Nhiệm vụ cụ thể:
1. **Đổi tên các thư mục MVC sang tiếng Việt**:
   - `Controllers/` -> `BoDieuKhien/` (9 controllers)
   - `Models/` -> `MoHinh/` (3 files)
   - `Views/` -> `GiaoDien/` (29 files across 10 subdirectories)
   - `Data/` -> `DuLieu/` (1 file `ApplicationDbContext.cs`)
   - **BẢO TOÀN 100%** `Areas/CuaHang/` (giữ nguyên cấu trúc nội bộ: Controllers, Models, Views, Helpers).
2. **Cập nhật Namespace đồng bộ**:
   - `WEB_TinhNguyen.BoDieuKhien`
   - `WEB_TinhNguyen.MoHinh`
   - `WEB_TinhNguyen.DuLieu`
   - Tạo `WEB_TinhNguyen/DuLieu/Compatibility.cs` hoặc `GlobalUsings.cs` để bảo toàn tương thích ngược cho 14 migration EF Core hiện có (tránh lỗi `WEB_TinhNguyen.Data.ApplicationDbContext`).
3. **Cấu hình Razor ViewEngine 2 tầng**:
   - Tạo `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs` kế thừa `IViewLocationExpander` hỗ trợ `/GiaoDien/{1}/{0}.cshtml` và `/GiaoDien/Shared/{0}.cshtml`.
   - Cấu hình trong `Program.cs`: `options.ViewLocationFormats` và `options.AreaViewLocationFormats` trỏ tới `GiaoDien/`.
   - Đăng ký `VietnameseViewLocationExpander` vào `RazorViewEngineOptions`.
4. **Cập nhật 22 vị trí hardcoded layout path**:
   - Thay thế toàn bộ `~/Views/Shared/_AdminLayout.cshtml` -> `~/GiaoDien/Shared/_AdminLayout.cshtml`.
   - Thay thế toàn bộ `~/Views/Shared/_Layout.cshtml` -> `~/GiaoDien/Shared/_Layout.cshtml`.
   - Cập nhật `_ViewImports.cshtml` trong `GiaoDien/` và `Areas/CuaHang/Views/` trỏ tới `WEB_TinhNguyen.MoHinh`.
5. **Cập nhật `tailwind.config.js`**:
   - Bổ sung `./GiaoDien/**/*.cshtml` và `./WEB_TinhNguyen/GiaoDien/**/*.cshtml` vào mảng `content`.
6. **Xác minh biên dịch**:
   - Chạy lệnh build: `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj`.
   - Đảm bảo 0 Warning/Error làm hỏng tiến trình build.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## 2026-11-04T02:34:20Z
You are C# .NET 10 Developer (Lập trình viên C# - .NET 10).
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1
Read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before doing anything else.
Also read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md and your dispatch file /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/DISPATCH.md.
Also read the detailed survey findings in /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_2/handoff.md.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Scope & Assigned Task (Milestone M1 - Requirement R2):
1. Rename MVC directories to Vietnamese:
   - `WEB_TinhNguyen/Controllers` -> `WEB_TinhNguyen/BoDieuKhien`
   - `WEB_TinhNguyen/Models` -> `WEB_TinhNguyen/MoHinh`
   - `WEB_TinhNguyen/Views` -> `WEB_TinhNguyen/GiaoDien`
   - `WEB_TinhNguyen/Data` -> `WEB_TinhNguyen/DuLieu`
   - Preserving `Areas/CuaHang/` 100% untouched.
2. Update namespaces:
   - `WEB_TinhNguyen.BoDieuKhien`
   - `WEB_TinhNguyen.MoHinh`
   - `WEB_TinhNguyen.DuLieu`
   - Provide backward compatibility for existing migrations (`WEB_TinhNguyen.Data.ApplicationDbContext`).
3. Configure Razor ViewEngine 2 layers:
   - Create `WEB_TinhNguyen/DuLieu/VietnameseViewLocationExpander.cs` implementing `IViewLocationExpander`.
   - In `Program.cs`, configure `options.ViewLocationFormats` and `options.AreaViewLocationFormats` for `/GiaoDien/`.
   - Register `VietnameseViewLocationExpander` with Razor ViewEngine.
4. Update all 22 hardcoded layout paths `~/Views/Shared/...` to `~/GiaoDien/Shared/...`.
   - Update `_ViewImports.cshtml` in `GiaoDien/` and `Areas/CuaHang/Views/`.
5. Update `tailwind.config.js` to include `./GiaoDien/**/*.cshtml` and `./WEB_TinhNguyen/GiaoDien/**/*.cshtml`.
6. Run build verification:
   `XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj`
   Ensure 0 errors.

Update progress.md in your working directory with "Last visited: [timestamp]" after every meaningful step.
When finished, write your handoff report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m1/handoff.md and send a completion message back.
