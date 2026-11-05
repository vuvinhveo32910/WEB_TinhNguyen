# BÁO CÁO KIỂM THỬ ĐỐI KHÁNG MILESTONE M3 (CHALLENGER 1 HANDOFF REPORT)

**Người thực hiện**: Challenger 1 (Adversarial Empirical Reviewer) — `teamwork_challenger_r3_m3_1`  
**Ngày thực hiện**: 2026-11-04  
**Phạm vi**: Milestone M3 — Requirement R3 (Đổi mới Giao diện Tailwind CSS & Bảo toàn CuaHang)  
**Kết luận**: **APPROVE**  

---

## 1. Observation (Các quan sát và dữ liệu thực tế)

### 1.1. Bảng màu mặc định (Nền trắng `#ffffff`, Chữ tình nguyện `#143823`)
- **Tệp `Styles/app.css:6-16, 60-63`**:
  ```css
  :root {
    --bg-page: #ffffff;
    --bg-surface: #ffffff;
    --bg-surface-soft: #f8faf8;
    --text-main: #143823;
    --text-muted: #3d6049;
    --text-subtle: #6b8f77;
  ...
  body:not(.cuahang-body) {
    background-color: var(--bg-page);
    color: var(--text-main);
  }
  ```
- **Tệp `WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml:145-155, 214-224`**:
  - Khai báo biến `:root`: `--bg-page: #ffffff;`, `--text-main: #143823;`.
  - Quy tắc CSS `body:not(.cuahang-body)` và `html:not(.dark) body` thiết lập `background-color: var(--bg-page) !important;` và `color: var(--text-main) !important;`.
- **Tệp `tailwind.config.js:28-47`**:
  - `volunteer-text`: `#143823`
  - Thang màu `volunteer`: 50 (`#f0fdf4`) đến 950 (`#052e16`).
  - Màu bổ trợ: `volunteer-night`: `#0a140e`, `volunteer-card`: `#13241b`, `volunteer-amber`: `#f59e0b`, `volunteer-rose`: `#f43f5e`.
- **Đo lường độ tương phản WCAG (Contrast Ratio Calculation)**:
  - Công thức Relative Luminance: $L(\#ffffff) = 1.0$, $L(\#143823) = 0.03083$.
  - Tỷ lệ tương phản: $(1.0 + 0.05) / (0.03083 + 0.05) = \mathbf{12.97:1}$.
  - Đạt chuẩn WCAG AA (yêu cầu $\ge 4.5:1$) và WCAG AAA (yêu cầu $\ge 7.0:1$) cho văn bản thông thường (vượt 85% so với ngưỡng AAA).

### 1.2. Bốn Keyframe Animations
- **Tệp `tailwind.config.js:60-86` và `_Layout.cshtml:90-116`**:
  1. `float`: `@keyframes float { 0%, 100%: translateY(0px); 50%: translateY(-8px); }` -> token `'float': 'float 4s ease-in-out infinite'`.
  2. `pulseSlow`: `@keyframes pulseSlow { 0%, 100%: opacity: 1, scale(1); 50%: opacity: 0.85, scale(1.03); }` -> token `'pulse-slow': 'pulseSlow 3s ease-in-out infinite'`.
  3. `heartBeat`: `@keyframes heartBeat { 0%, 100%: scale(1); 14%: scale(1.15); 28%: scale(1); 42%: scale(1.1); 70%: scale(1); }` -> token `'heartbeat': 'heartBeat 1.5s ease-in-out infinite'`.
  4. `fadeInUp`: `@keyframes fadeInUp { 0%: opacity 0, translateY(20px); 100%: opacity 1, translateY(0); }` -> token `'fade-in-up': 'fadeInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) forwards'`.
- **Sử dụng thực tế trong views (`WEB_TinhNguyen/GiaoDien/Home/`)**:
  - `animate-float`: sử dụng trong `ComingSoon.cshtml`, `TinTuc.cshtml`.
  - `animate-pulse-slow`: sử dụng trong `GayQuy.cshtml`, `ComingSoon.cshtml`.
  - `animate-heartbeat`: sử dụng trong `GayQuy.cshtml`.
  - `animate-fade-in-up`: sử dụng trong `GayQuy.cshtml`, `KyUc.cshtml`, `TinTuc.cshtml`, `ChuongTrinh.cshtml`.

### 1.3. Tệp CSS đã biên dịch `WEB_TinhNguyen/wwwroot/css/site.css`
- Dung lượng tệp: **100,392 bytes** (~100.4 KB), được nén minified.
- Quét đối kháng nội dung `site.css`:
  - `@keyframes float`: **FOUND**
  - `@keyframes pulseSlow`: **FOUND**
  - `@keyframes heartBeat`: **FOUND**
  - `@keyframes fadeInUp`: **FOUND**
  - `.animate-float`: **FOUND** (`.animate-float{animation:float 4s ease-in-out infinite}`)
  - `.animate-pulse-slow`: **FOUND** (`.animate-pulse-slow{animation:pulseSlow 3s ease-in-out infinite}`)
  - `.animate-heartbeat`: **FOUND** (`.animate-heartbeat{animation:heartBeat 1.5s ease-in-out infinite}`)
  - `.animate-fade-in-up`: **FOUND** (`.animate-fade-in-up{animation:fadeInUp .6s cubic-bezier(.16,1,.3,1) forwards}`)
  - `--bg-page:#fff;`: **FOUND** (Tailwind minifier tối ưu hóa 6 ký tự `#ffffff` thành 3 ký tự `#fff`).
  - `--text-main:#143823;`: **FOUND**.
  - `body:not(.cuahang-body)`: **FOUND**.
- Quy trình biên dịch:
  - `package.json:7`: `"build:css": "tailwindcss -i Styles/app.css -o WEB_TinhNguyen/wwwroot/css/site.css --minify"`
  - `WEB_TinhNguyen/WEB_TinhNguyen.csproj:22-24`: Target `TailwindCSS BeforeTargets="Build"` tự động kích hoạt `npm run build:css`.
  - Chạy thực nghiệm `npm run build:css`: Thành công trong 3.47s.

### 1.4. Bảo toàn và Cô lập Phân hệ Bán hàng (`Areas/CuaHang/`)
- Tệp `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`:
  - Điều hướng Storefront sang `~/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`.
  - Điều hướng Admin Cửa hàng sang `~/GiaoDien/Shared/_AdminLayout.cshtml`.
- Tệp `WEB_TinhNguyen/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`:
  - Thẻ `<html>` và `<body>` khóa cứng chế độ sáng: `class="light bg-slate-50 text-slate-900 cuahang-body antialiased"`.
  - Style cô lập: `body.cuahang-body { background-color: #f8fafc !important; color: #0f172a !important; }`.
  - Đoạn mã Javascript Anti-Dark Mode thực thi ngay tại `<head>` và sự kiện `DOMContentLoaded`:
    `document.documentElement.classList.remove('dark'); document.documentElement.classList.add('light');`.
  - Toàn bộ các ID và cấu trúc giỏ hàng được bảo lưu 100%: `#cartCountBadge`, `#floatingCartBtn`, `#floatingCartCount`.
- Trạng thái 9 views hiện hữu trong `Areas/CuaHang/Views/`:
  - `AdminDonHang/ChiTiet.cshtml`, `AdminDonHang/Index.cshtml`
  - `AdminSanPham/Create.cshtml`, `AdminSanPham/Edit.cshtml`, `AdminSanPham/Index.cshtml`
  - `CuaHang/ChiTiet.cshtml`, `CuaHang/Index.cshtml`
  - `GioHang/DatHangThanhCong.cshtml`, `GioHang/Index.cshtml`
  - **Kết quả kiểm tra**: 0 tệp bị sửa đổi nội dung, timestamp giữ nguyên bản.

### 1.5. Sửa lỗi `LinkBaiVietGoc` trong `Entities.cs`
- Tệp `WEB_TinhNguyen/MoHinh/Entities.cs:641`:
  - Mã hiện tại: `public string? LinkBaiVietGoc { get => DuongDanBaiVietGoc; set => DuongDanBaiVietGoc = value; }`
  - Đã khắc phục triệt để lỗi logic gán nhầm vào `DuongDanAnhTrongBai`.

### 1.6. Kết quả chạy Build và Test Suite thực tế
1. **Biên dịch `dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`**:
   - Kết quả: **Build succeeded (0 Error, 2 Warnings NU1900 do sandbox offline)**.
   - Thời gian thực thi: 13.70 giây.
   - Target `npm run build:css` hoàn thành trong 4.985 giây.
2. **Chạy E2ETests (`tests/E2ETests/E2ETests.csproj`)**:
   - Tier 1: 100/100 Passed
   - Tier 2: 28/28 Passed
   - Tier 3: 18/18 Passed
   - Tier 4: 12/12 Passed
   - Tier 5: 167/167 Passed
   - **Tổng: 325/325 Tests Passed (100% Pass rate)**, 0 Failed, 0 Skipped trong 5.69 giây.
3. **Chạy M1Challenger (`tests/M1Challenger/M1Challenger.csproj`)**:
   - **Tổng: 67/67 Tests Passed (100% Pass rate)**, 0 Failed.
4. **Chạy M3 Adversarial Stress Harness (Challenger 1)**:
   - **Tổng: 49/49 Tests Passed (100% Pass rate)**, 0 Failed.

---

## 2. Logic Chain (Chuỗi suy luận logic)

1. Từ yêu cầu R3: Website phải có giao diện hiện đại mang bản sắc tình nguyện bằng Tailwind CSS, mặc định toàn trang nền trắng (`#ffffff`), chữ mới hài hòa hợp chủ đề tình nguyện (`#143823`), bổ sung animations mượt mà, và bảo toàn 100% giao diện/cấu trúc phân hệ bán hàng `Areas/CuaHang/`.
2. Quan sát 1.1 và 1.2 chứng minh bảng màu volunteer và 4 animation keyframes (`float`, `pulseSlow`, `heartBeat`, `fadeInUp`) đã được định nghĩa đầy đủ trong `tailwind.config.js`, `Styles/app.css` và `_Layout.cshtml`.
3. Kiểm nghiệm độ tương phản màu sắc cho thấy cặp màu `#ffffff` và `#143823` đạt tỷ lệ 12.97:1, vượt xa yêu cầu WCAG AAA (7.0:1), đảm bảo khả năng tiếp cận và tính thẩm mỹ cao.
4. Quan sát 1.3 chứng minh lệnh `npm run build:css` biên dịch chính xác tệp `WEB_TinhNguyen/wwwroot/css/site.css` (100,392 bytes) chứa đầy đủ các keyframe, các animation utilities và các biến màu CSS.
5. Quan sát 1.4 chứng minh giải pháp sử dụng Layout riêng `_CuaHangLayout.cshtml` phối hợp với selector `body:not(.cuahang-body)` và đoạn mã cưỡng chế Light Mode giúp cô lập hoàn toàn phân hệ `Areas/CuaHang/`, giữ nguyên vẹn 100% cấu trúc DOM và 9 tệp views của cửa hàng.
6. Quan sát 1.5 và 1.6 chứng minh mã nguồn không có lỗi logic ẩn (đã sửa lỗi typo tại `Entities.cs`), giải pháp biên dịch sạch (0 errors) và vượt qua 100% các bộ kiểm thử E2ETests (325 tests), M1Challenger (67 tests) và bộ stress test đối kháng M3 (49 tests).

---

## 3. Caveats (Các lưu ý và giới hạn kiểm định)

1. **Minification Color Format**: Trình biên dịch CSS của Tailwind tự động rút gọn mã màu HEX `#ffffff` thành `#fff` trong file nén `site.css`. Điều này tuân thủ chuẩn CSS3 spec và tương đương giá trị màu sắc 100%.
2. **Nút Toggle Dark Mode**: Nút icon chuyển đổi Sáng/Tối trực quan thuộc phạm vi Milestone M4. Tại Milestone M3, hạ tầng CSS variables hai chế độ (`:root` và `html.dark`) cùng bộ chống nháy Anti-FOUC đã được dựng sẵn sàng và hoạt động ổn định.
3. **Cảnh báo NU1900**: 2 cảnh báo NU1900 xuất hiện do máy trạm chạy môi trường sandbox không kết nối ra Internet để tải vulnerability feed từ nuget.org; đây là hành vi bình thường của môi trường kiểm thử offline và không ảnh hưởng đến chất lượng giải pháp.

---

## 4. Conclusion (Kết luận)

**VERDICT: APPROVE**

Milestone M3 (Requirement R3) đáp ứng đầy đủ và vượt các tiêu chuẩn kỹ thuật đề ra:
- Bảng màu nền trắng `#ffffff` và chữ `#143823` đạt chuẩn WCAG AAA (12.97:1).
- 4 Keyframe animations (`float`, `pulseSlow`, `heartBeat`, `fadeInUp`) hoạt động mượt mà và được tích hợp trong các views.
- Tệp `WEB_TinhNguyen/wwwroot/css/site.css` được biên dịch tự động và chuẩn xác.
- Phân hệ Cửa hàng `Areas/CuaHang/` được cô lập tuyệt đối và bảo toàn 100%.
- Toàn bộ các bài kiểm thử tự động đạt tỷ lệ đậu 100% (325/325 E2E Tests, 67/67 M1Challenger Tests, 49/49 M3 Adversarial Tests).

---

## 5. Verification Method (Hướng dẫn kiểm chứng độc lập)

Để tái hiện độc lập toàn bộ các kết quả thực nghiệm trên, thực thi các lệnh sau:

1. **Biên dịch giải pháp**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx
   ```
   *Kỳ vọng*: 0 Error. Lệnh `npm run build:css` tự động chạy thành công.

2. **Chạy bộ kiểm thử E2ETests**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj
   ```
   *Kỳ vọng*: 325/325 Passed (100%), 0 Failed, 0 Skipped.

3. **Chạy bộ kiểm thử M1Challenger**:
   ```bash
   XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/M1Challenger/M1Challenger.csproj
   ```
   *Kỳ vọng*: 67/67 Passed (100%), 0 Failed.

4. **Kiểm tra file compiled site.css**:
   ```bash
   python3 -c "
   with open('WEB_TinhNguyen/wwwroot/css/site.css') as f:
       css = f.read()
   for item in ['float', 'pulseSlow', 'heartBeat', 'fadeInUp', 'animate-float', 'animate-pulse-slow', 'animate-heartbeat', 'animate-fade-in-up', '143823', 'cuahang-body']:
       assert item in css, f'Missing {item}'
   print('CSS Verification PASS: All keyframes and tokens verified!')
   "
   ```
