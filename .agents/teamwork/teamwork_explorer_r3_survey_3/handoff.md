# BÁO CÁO KHẢO SÁT & BẢN THIẾT KẾ KIẾN TRÚC UI/UX, TAILWIND CSS & DARK MODE (R3 & R4)

**Tác giả**: Chuyên gia UI/UX & Tailwind CSS (Teamwork Explorer 3)  
**Mã tiến trình**: `teamwork_explorer_r3_survey_3`  
**Ngày lập**: 2026-11-04  
**Phạm vi**: Yêu cầu R3 (Đổi mới giao diện, Tailwind CSS, Bản sắc tình nguyện, Cô lập Cửa hàng) & R4 (Chuyển đổi Sáng/Tối - Light/Dark Mode)

---

## 1. Observation (Các phát hiện thực tế trên Codebase)

### 1.1. Hiện trạng cấu trúc Views và Layout
1. **Layout chính (`WEB_TinhNguyen/Views/Shared/_Layout.cshtml`)**:
   - **Tham chiếu dữ liệu DB**: Dòng 10-29 và 78-86 đang truy vấn `activeConfig?.MauNenStart`, `MauChuDao`, `MauNhan`, `MauChu`, `FontTieuDe`, `FontNoiDung` từ `ApplicationDbContext` để gán vào các biến CSS `:root` (`--bg-web`, `--color-main`, `--color-sub`, `--color-text`).
   - **Thư viện Styling**: 
     - Dòng 44 nạp `~/css/site.css`.
     - Dòng 47 nạp Google Fonts qua URL template động.
     - Dòng 48 nạp FontAwesome 6.4.0.
     - Dòng 50 nạp CDN Tailwind: `<script src="https://cdn.tailwindcss.com"></script>`.
     - Dòng 54-58 cấu hình inline: `tailwind.config = { darkMode: 'class', theme: { extend: { colors: { 'night': '#0c1712', 'night-2': '#14241b' } } } }`.
   - **Cơ chế Dark Mode hiện tại**:
     - Dòng 61-75: Script inline trong `<head>` đọc `localStorage.getItem('theme')` để gán class `dark` vào thẻ `html`.
     - Dòng 93-161: Sử dụng CSS cưỡng bức với `!important` (`html:not(.dark) body { background-color: var(--bg-web) !important; color: var(--color-text); }`, `html.dark body { background-color: #0c1712 !important; color: #f1f5f9 !important; }`, `html.dark main p:not(...) { color: #e2e8f0 !important; }`).
     - Dòng 165-230, 351-355 & 595-650: Nút bật/tắt dark mode hiện tại được xây dựng bằng cơ chế vật lý "Dây kéo bật đèn" (`#pullcord-wrapper`, `#pullcord-line`, `#pullcord-handle`, `#pullcord-hitarea`), kéo thả theo trục Y để kích hoạt hàm `toggleTheme()`.
     - Dòng 665-679: Chạy hàm `syncInlineColorsForTheme(isDark)` quét toàn bộ thẻ `main [style*="color"]` trong DOM để ghi đè `style.color` bằng `#86efac` hoặc `#f1f5f9`.
   - **Header & Navigation**:
     - Desktop Menu (dòng 380-411) chứa liên kết tới `Home` và `Areas/CuaHang` (Cửa hàng gây quỹ, Giỏ hàng có badge `cartCountBadge`).
     - Mobile Bottom Nav (dòng 494-540) dùng phong cách Liquid bar (`.navigation fixed bottom-[15px] md:hidden lg:hidden`) với 7 mục menu.
     - Nút Giỏ hàng nổi (dòng 542-553) `#floatingCartBtn` (`fixed right-5 bottom-24 md:bottom-8 z-40`).

2. **Giao diện các trang thuộc `Views/Home/`**:
   - `Views/Home/Index.cshtml`: Tích hợp Swiper 11, Card đếm ngược sự kiện (`validEvents`), FAQ accordion. Xuất hiện rất nhiều inline styles phụ thuộc trường model màu sắc cũ: `style="color: @ev.MauChu;"`, `style="background-color: @(Model?.VeDuAn_MauNenTag ?? "#4caf50");"`.
   - `Views/Home/ChuongTrinh.cshtml`: Sử dụng `.dynamic-font`, `.dynamic-bg` dựa trên `conf.MauNenHienTai`, `conf.MauChuDao`, `conf.MauChu`.
   - `Views/Home/GayQuy.cshtml`: Sử dụng gradient inline từ `Model?.Hero_MauNenStart`, `Hero_MauNenMid`, `Hero_MauNenEnd`.
   - `Views/Home/BanToChuc.cshtml`: Sơ đồ cây 3 cấp bậc với các đường nối `.org-line-v` và mã màu `#dc2646`, `#2574b8`, v.v.
   - `Views/Home/TinTuc.cshtml`: Lưới bài viết dạng card và modal đọc tin `#newsReaderModal`.
   - `Views/Home/KyUc.cshtml`: Thư viện ảnh hồi ức theo năm với gradient overlay.

3. **Hiện trạng phân hệ bán hàng `Areas/CuaHang/`**:
   - **Tập tin cấu hình View (`Areas/CuaHang/Views/_ViewStart.cshtml`)**:
     ```csharp
     if (controller.StartsWith("Admin", StringComparison.OrdinalIgnoreCase) ||
         viewPath.Contains("/Admin", StringComparison.OrdinalIgnoreCase))
     {
         Layout = "~/Views/Shared/_AdminLayout.cshtml";
     }
     else
     {
         Layout = "~/Views/Shared/_Layout.cshtml";
     }
     ```
   - **Danh sách Views của `Areas/CuaHang/`**:
     - Storefront: `Views/CuaHang/Index.cshtml` (Catalog, bộ lọc pills giá/danh mục, tìm kiếm live), `Views/CuaHang/ChiTiet.cshtml` (Xem sản phẩm, câu chuyện gây quỹ, thêm giỏ hàng).
     - Giỏ hàng & Checkout: `Views/GioHang/Index.cshtml` (Bảng sản phẩm, điều chỉnh số lượng AJAX, form đặt mua, VietQR modal), `Views/GioHang/DatHangThanhCong.cshtml` (Xác nhận, mã đơn, tiến trình 4 bước).
     - Quản trị: `Views/AdminSanPham/Index.cshtml`, `Create.cshtml`, `Edit.cshtml`; `Views/AdminDonHang/Index.cshtml`, `ChiTiet.cshtml`.
   - **Phong cách DOM & CSS của Cửa Hàng**:
     - Các view này được thiết kế hoàn chỉnh bằng các utility classes Tailwind hiện đại: `bg-slate-50`, `text-slate-900`, `bg-emerald-600`, `from-[#143220]`, `dark:bg-night`, `dark:text-white`.
     - Cấu trúc DOM, ID và logic JavaScript (`liveSearchInput`, `debounceLiveFilter`, `filterByPriceRange`, `capNhatSoLuongAjax`, `showClearCartConfirmModal`, VietQR banking) đang gắn liền chặt chẽ với các thẻ DOM hiện hành.

### 1.2. Hiện trạng Build Pipeline & Static Assets
1. **File `WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj` (dòng 21-24)**:
   ```xml
   <Target Name="TailwindCSS" BeforeTargets="Build">
       <Exec Command="npm run build:css" />
   </Target>
   ```
2. **File `package.json` tại thư mục gốc**:
   ```json
   "scripts": {
     "build:css": "tailwindcss -i Styles/app.css -o wwwroot/css/site.css --minify",
     "watch:css": "tailwindcss -i Styles/app.css -o wwwroot/css/site.css --watch"
   }
   ```
3. **Phát hiện xung đột đường dẫn tĩnh (Path Discrepancy)**:
   - Khi `dotnet build` chạy trong môi trường, `npm run build:css` xuất ra file `wwwroot/css/site.css` ở **thư mục gốc** (`/home/hutech/Vinh_Offline/WEB_TinhNguyen/wwwroot/css/site.css` dung lượng ~91KB).
   - Tuy nhiên, dự án ASP.NET Core MVC nằm trong thư mục con `WEB_TinhNguyen/`, và `app.MapStaticAssets()` (`Program.cs:47`) phục vụ static files từ thư mục con `WEB_TinhNguyen/wwwroot/css/site.css` (chỉ có dung lượng ~2KB với các class font cơ bản).
   - Do đó, trang web đang hiển thị giao diện Tailwind thông qua CDN script tag `<script src="https://cdn.tailwindcss.com"></script>` trong `_Layout.cshtml`, chứ chưa thực sự nhận file compiled từ `npm run build:css`.
4. **File `tailwind.config.js` tại thư mục gốc**:
   - `content` mảng đang khai báo:
     `./Views/**/*.cshtml`, `./Areas/**/*.cshtml`, `./WEB_TinhNguyen/Views/**/*.cshtml`, `./WEB_TinhNguyen/Areas/**/*.cshtml`, `./WEB_TinhNguyen/wwwroot/js/**/*.js`, `./wwwroot/js/**/*.js`.
   - Chưa hỗ trợ cấu hình `darkMode: 'class'`.
   - Khi R2 đổi tên thư mục `Views` -> `GiaoDien`, nếu không cập nhật `tailwind.config.js` thì Tailwind CLI sẽ không quét được các view mới.

---

## 2. Logic Chain (Lập luận & Phân tích giải pháp)

### 2.1. Phân tích Yêu cầu R3 (Đổi mới giao diện & Bản sắc tình nguyện)
1. **Bản chất yêu cầu**:
   - *Yêu cầu gốc*: "Làm mới toàn bộ giao diện sang phong cách hiện đại, truyền cảm hứng, bổ sung animations mượt mà và mang đậm bản sắc tình nguyện sử dụng Tailwind CSS. Mặc định toàn trang có nền trắng với màu chữ mới hài hòa hợp chủ đề tình nguyện."
   - *Mối liên hệ với R1*: R1 xóa toàn bộ thuộc tính `MauChu`, `MauNenStart`, `MauChuDao` khỏi Models/DbContext. Vì vậy, giao diện mới **tuyệt đối không được phụ thuộc vào database** để lấy màu sắc; toàn bộ bảng màu và phong cách phải được định nghĩa bằng CSS Variables và Tailwind Theme Token chuẩn mực.
2. **Xác lập "Bản sắc tình nguyện" (Volunteer Identity Palette)**:
   - **Nền trắng mặc định (`bg-white` / `#ffffff`)**: Mang lại cảm giác trong sáng, tinh khôi, minh bạch và thân thiện. Các khối phân cách sử dụng màu nền chuyển tiếp siêu nhẹ (`#f8faf8` / `bg-emerald-50/30`).
   - **Màu chữ mới bản sắc tình nguyện (`--text-base: #143823`)**: Thay vì màu đen tuyền `#000000` khô cứng, sử dụng màu xanh ngọc bích sẫm (Deep Volunteer Forest Slate / Rừng xanh Tây Nguyên). Màu này vừa mang đậm tinh thần áo xanh thanh niên tình nguyện, vừa đạt độ tương phản vượt chuẩn WCAG AAA (> 11.5:1 trên nền trắng), giúp văn bản sắc nét, trang nhã.
   - **Màu nhấn thương hiệu (Brand Accents)**:
     - *Xanh mầm non hy vọng (Emerald / Leaf)*: `#16a34a` (chính), `#22c55e` (sáng), `#15803d` (đậm).
     - *Ánh nắng nhiệt huyết tuổi trẻ (Warm Sun / Amber)*: `#f59e0b` / `#fbbf24` (năng lượng, ấm áp vùng cao).
     - *Trái tim sẻ chia (Rose Coral)*: `#f43f5e` / `#e11d48` (yêu thương, nhân ái).
     - *Nguồn nước trong lành (Sky Azure)*: `#0284c7` (tượng trưng cho công trình nước sạch học đường).
3. **Animations mượt mà (Smooth Keyframe Animations)**:
   - Bổ sung các hiệu ứng CSS thuần không gây giật lag:
     - `animate-float`: Chuyển động bồng bềnh êm ái cho mascot, trăng Langbiang.
     - `animate-pulse-slow`: Hiệu ứng nhịp đập trái tim cho các nút "Ủng hộ", "Đóng góp", icon trái tim gây quỹ.
     - `animate-shimmer`: Ánh sáng lướt nhẹ trên các CTA button quan trọng.
     - `animate-fade-in-up`: Xuất hiện mượt mà khi người dùng cuộn trang.

### 2.2. Phân tích Yêu cầu R4 (Chuyển đổi Sáng/Tối - Light/Dark Mode)
1. **Thiết kế nút chuyển đổi Sáng/Tối**:
   - Cơ chế dây kéo vật lý (`#pullcord-wrapper`) hiện tại tuy sáng tạo nhưng chiếm diện tích, khó tương tác trên màn hình nhỏ và không phù hợp với tiêu chuẩn Accessibility (A11y).
   - *Giải pháp tối ưu*: Thiết kế nút chuyển đổi chuẩn dạng Icon Button hình tròn / viên nang (Pill Switch) tinh gọn, đặt cố định trên thanh Menu Desktop (bên cạnh nút Giỏ hàng và nút Đóng góp) và trên Header Mobile.
   - Khi ở chế độ Sáng: Nút hiển thị icon Trăng khuyết (`fa-moon`) màu xanh thẫm, kèm hiệu ứng hover xoay nhẹ.
   - Khi ở chế độ Tối: Nút chuyển sang icon Mặt trời (`fa-sun`) màu vàng ấm (`#fbbf24`), phát sáng nhẹ.
2. **Kiến trúc CSS Variables cho 2 chế độ**:
   - Chế độ Sáng (Light - Mặc định):
     - Nền: `var(--bg-base) = #ffffff`
     - Chữ: `var(--text-base) = #143823` (Màu xanh tình nguyện)
     - Card / Surface: `var(--bg-surface) = #ffffff` (border: `rgba(22, 163, 74, 0.15)`)
   - Chế độ Tối (Dark):
     - Nền: `var(--bg-base) = #0a140e` (Đêm rừng cao nguyên sâu thẳm, không dùng xám xịt thông thường)
     - Chữ: `var(--text-base) = #f8fafc` (Trắng ánh ngọc mờ dịu mắt)
     - Card / Surface: `var(--bg-surface) = #13241b` (border: `rgba(74, 222, 128, 0.15)`)
     - Điểm nhấn: `var(--brand-primary) = #4ade80` (Màu lá non phát sáng trong đêm)
3. **Loại bỏ Flash of Unstyled Content (FOUC)**:
   - Script khởi tạo theme đặt ngay đầu thẻ `<head>` trước khi bất kỳ phần tử DOM nào được render, ngăn chặn hiện tượng chớp nháy màn hình khi chuyển trang.

### 2.3. Phân tích Cô lập 100% Phân hệ Bán hàng (`Areas/CuaHang/`)
1. **Nguy cơ xâm lấn hiện tại**:
   - Hiện tại `Areas/CuaHang/Views/_ViewStart.cshtml` đang chỉ định `Layout = "~/Views/Shared/_Layout.cshtml"`.
   - Nếu `_Layout.cshtml` thay đổi CSS toàn cục, hoặc khi người dùng bật Dark Mode (gán class `dark` lên thẻ `html`), các quy tắc CSS ghi đè như `html.dark body`, `html.dark main ...` sẽ tự động đè lên giao diện `Areas/CuaHang/`, làm biến đổi màu sắc, font chữ và các thành phần giao diện của cửa hàng.
2. **Giải pháp kiến trúc cô lập đa tầng (Multi-Layer Isolation Architecture)**:
   - **Tầng 1 - Layout Guard tự động trong `_Layout.cshtml`**:
     ```csharp
     @{
         bool isCuaHang = string.Equals(ViewContext.RouteData.Values["area"]?.ToString(), "CuaHang", StringComparison.OrdinalIgnoreCase);
     }
     ```
     - Nếu `isCuaHang == true`:
       1. Thẻ `<main>` được gán class riêng biệt: `<main class="cuahang-isolated-scope">`.
       2. Toàn bộ CSS biến số của theme mới và các selector `html.dark` chỉ áp dụng cho `main:not(.cuahang-isolated-scope)`.
       3. Nút bấm chuyển đổi Dark Mode trên Header/Mobile **tự động ẩn đi** khi người dùng đang duyệt trong Cửa hàng gây quỹ.
       4. Script trong `<head>` kiểm tra URL: Nếu đường dẫn chứa `/CuaHang`, hệ thống lập tức cưỡng chế loại bỏ class `dark` và đặt lại `light`, đảm bảo giao diện luôn hiển thị ở trạng thái chuẩn mực ban đầu.
   - **Tầng 2 - Phương án Layout độc lập (`_CuaHangLayout.cshtml`) (Lựa chọn tối thượng)**:
     - Tạo một file Layout riêng biệt: `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`.
     - Layout này giữ nguyên 100% mã nguồn HTML/CSS gốc của `_Layout.cshtml` trước khi đổi mới, loại bỏ hoàn toàn nút Dark Mode và biến màu mới.
     - Cập nhật `Areas/CuaHang/Views/_ViewStart.cshtml`:
       ```csharp
       if (controller.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
           Layout = "~/Views/Shared/_AdminLayout.cshtml";
       else
           Layout = "~/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml";
       ```
     - **Hiệu quả**: 100% file `.cshtml` bên trong `Areas/CuaHang/Views/` (CuaHang/Index, CuaHang/ChiTiet, GioHang/Index, GioHang/DatHangThanhCong) giữ nguyên từng ký tự, không bị bất kỳ đoạn CSS/JS nào của giao diện mới can thiệp.
   - **Tầng 3 - Bảo toàn Tailwind Content Purging**:
     - Trong `tailwind.config.js`, giữ nguyên các đường dẫn quét `./Areas/**/*.cshtml` và `./WEB_TinhNguyen/Areas/**/*.cshtml` để bộ dựng Tailwind không bao giờ xóa nhầm các class tiện ích mà Cửa hàng đang sử dụng (`bg-slate-50`, `emerald-600`, `dark:bg-night-2`, v.v.).

---

## 3. Caveats (Các giả định & Lưu ý biên giới kỹ thuật)

1. **Sự đồng bộ với Explorer 1 (R1 - Xóa trường màu trong DB/Models)**:
   - Khi Explorer 1 thực hiện xóa các cột màu (`MauChu`, `MauNenStart`, `MauChuDao`...) khỏi các Model C# (`CauHinhTrangChu`, `CauHinhChuongTrinh`, `CauHinhGayQuy`, v.v.), bất kỳ đoạn mã Razor nào trong `Views/Home/*` hoặc `Views/Admin*` còn tham chiếu tới `@Model.MauChu` sẽ gây lỗi biên dịch (Compilation Error CS1061).
   - Đội ngũ triển khai UI (Workers) phải đồng thời xóa sạch các cú pháp `@Model.Mau...` trong file `.cshtml` và thay bằng các class Tailwind ngữ nghĩa (`text-[var(--text-base)]` hoặc `text-emerald-800`).
2. **Sự đồng bộ với Explorer 2 (R2 - Đổi tên thư mục `Views` -> `GiaoDien`)**:
   - Khi R2 đổi tên `Views` thành `GiaoDien`, đường dẫn layout `~/Views/Shared/_Layout.cshtml` sẽ được định tuyến thông qua cấu hình Razor ViewEngine mới (`~/GiaoDien/Shared/_Layout.cshtml`).
   - Cần đảm bảo file `tailwind.config.js` được cấu hình bao quát cả `./GiaoDien/**/*.cshtml` và `./WEB_TinhNguyen/GiaoDien/**/*.cshtml`.
3. **Môi trường Build Tailwind (NPM vs CDN)**:
   - Để ứng dụng chạy hoàn hảo cả khi phát triển offline lẫn khi build production, giải pháp đề xuất cung cấp đồng thời:
     - Cấu hình chuẩn cho file `tailwind.config.js` + sửa file `package.json` xuất đúng file CSS vào `WEB_TinhNguyen/wwwroot/css/site.css`.
     - Cấu hình runtime CDN fallback tương thích 100% trong `_Layout.cshtml` để trang hiển thị tức thì mà không cần cài đặt môi trường Node phức tạp nếu chạy demo.

---

## 4. Conclusion (Bản thiết kế kỹ thuật chi tiết & Hướng dẫn thi công)

### 4.1. Bản thiết kế Bảng màu & Typography (Volunteer Theme Tokens)

#### A. Khai báo CSS Variables trong File Styles (`Styles/app.css` hoặc `<style>` của Layout)
```css
:root {
    /* Nền trắng mặc định */
    --bg-page: #ffffff;
    --bg-surface: #ffffff;
    --bg-surface-soft: #f8faf8;
    
    /* Màu chữ mới mang đậm bản sắc tình nguyện (Deep Volunteer Pine) */
    --text-main: #143823;
    --text-muted: #3d6049;
    --text-subtle: #6b8f77;

    /* Màu chủ đạo thương hiệu (Áo xanh tình nguyện) */
    --volunteer-primary: #15803d;      /* Xanh lá đậm */
    --volunteer-primary-hover: #166534;
    --volunteer-accent: #22c55e;       /* Xanh lá tươi hy vọng */
    --volunteer-light: #dcfce7;        /* Nền tag xanh non */
    
    /* Màu nhấn bổ trợ (Nhiệt huyết & Tình yêu thương) */
    --volunteer-sun: #f59e0b;          /* Vàng cam ánh trăng/lửa trại */
    --volunteer-heart: #f43f5e;        /* Đỏ hồng trái tim thiện nguyện */
    --volunteer-water: #0284c7;        /* Xanh dương công trình thanh niên */

    /* Đường viền & Đổ bóng */
    --border-light: rgba(20, 56, 35, 0.08);
    --border-card: rgba(22, 163, 74, 0.15);
    --shadow-card: 0 10px 25px -5px rgba(20, 56, 35, 0.05), 0 8px 10px -6px rgba(20, 56, 35, 0.05);
    --shadow-elevated: 0 20px 30px -10px rgba(21, 128, 61, 0.15);
}

/* Chế độ Tối (Dark Mode) */
html.dark {
    --bg-page: #0a140e;                /* Đêm rừng Tây Nguyên thâm trầm */
    --bg-surface: #13241b;             /* Thẻ card nổi trong đêm */
    --bg-surface-soft: #0d1d14;

    --text-main: #f8fafc;              /* Trắng mây sáng rõ */
    --text-muted: #cbd5e1;             /* Xám khói */
    --text-subtle: #94a3b8;

    --volunteer-primary: #4ade80;      /* Xanh huỳnh quang nổi bật */
    --volunteer-primary-hover: #86efac;
    --volunteer-accent: #86efac;
    --volunteer-light: rgba(74, 222, 128, 0.12);

    --volunteer-sun: #fbbf24;
    --volunteer-heart: #fb7185;
    --volunteer-water: #38bdf8;

    --border-light: rgba(255, 255, 255, 0.08);
    --border-card: rgba(74, 222, 128, 0.2);
    --shadow-card: 0 10px 25px -5px rgba(0, 0, 0, 0.5);
    --shadow-elevated: 0 20px 30px -10px rgba(0, 0, 0, 0.7);
}

/* Áp dụng toàn cục ngoài phân hệ bán hàng */
body:not(.cuahang-body) {
    background-color: var(--bg-page);
    color: var(--text-main);
    font-family: 'Be Vietnam Pro', system-ui, sans-serif;
    transition: background-color 0.3s ease, color 0.3s ease;
}
```

#### B. Cấu hình Tailwind Config mở rộng (`tailwind.config.js`)
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./Views/**/*.cshtml",
    "./GiaoDien/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./WEB_TinhNguyen/Views/**/*.cshtml",
    "./WEB_TinhNguyen/GiaoDien/**/*.cshtml",
    "./WEB_TinhNguyen/Areas/**/*.cshtml",
    "./WEB_TinhNguyen/wwwroot/js/**/*.js",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        'volunteer': {
          '50': '#f0fdf4',
          '100': '#dcfce7',
          '200': '#bbf7d0',
          '300': '#86efac',
          '400': '#4ade80',
          '500': '#22c55e',
          '600': '#16a34a',
          '700': '#15803d',
          '800': '#166534',
          '900': '#14532d',
          '950': '#052e16',
        },
        'volunteer-night': '#0a140e',
        'volunteer-card': '#13241b',
        'volunteer-text': '#143823',
        'volunteer-amber': '#f59e0b',
        'volunteer-rose': '#f43f5e',
      },
      animation: {
        'float': 'float 4s ease-in-out infinite',
        'pulse-slow': 'pulseSlow 3s ease-in-out infinite',
        'heartbeat': 'heartBeat 1.5s ease-in-out infinite',
        'fade-in-up': 'fadeInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) forwards',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-8px)' },
        },
        pulseSlow: {
          '0%, 100%': { opacity: '1', transform: 'scale(1)' },
          '50%': { opacity: '0.85', transform: 'scale(1.03)' },
        },
        heartBeat: {
          '0%, 100%': { transform: 'scale(1)' },
          '14%': { transform: 'scale(1.15)' },
          '28%': { transform: 'scale(1)' },
          '42%': { transform: 'scale(1.1)' },
          '70%': { transform: 'scale(1)' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        }
      }
    },
  },
  plugins: [
    require('@tailwindcss/container-queries'),
  ],
}
```

---

### 4.2. Bản thiết kế Nút chuyển đổi Sáng/Tối & Script điều khiển (R4)

#### A. DOM Nút chuyển đổi trên Header chung (`Views/Shared/_Layout.cshtml`)
Đặt cạnh icon giỏ hàng trên thanh Navigation Desktop và trên Header Mobile:
```html
<!-- NÚT CHUYỂN ĐỔI SÁNG / TỐI (LIGHT/DARK MODE TOGGLE) -->
@if (ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang")
{
    <button id="themeToggleBtn" type="button" onclick="toggleThemeMode()"
            class="relative inline-flex items-center justify-center w-10 h-10 rounded-full border border-emerald-500/20 bg-emerald-50 text-emerald-800 hover:bg-emerald-100 dark:bg-white/10 dark:text-amber-400 dark:hover:bg-white/15 dark:border-white/10 transition-all duration-300 shadow-xs focus:outline-none focus:ring-2 focus:ring-emerald-500"
            title="Chuyển đổi giao diện Sáng / Tối"
            aria-label="Chuyển đổi giao diện Sáng / Tối">
        <!-- Icon Mặt Trăng (Hiển thị khi đang ở chế độ SÁNG -> mời bật TỐI) -->
        <i class="fas fa-moon text-sm text-emerald-700 transition-transform duration-300 rotate-0 dark:-rotate-90 dark:hidden"></i>
        <!-- Icon Mặt Trời (Hiển thị khi đang ở chế độ TỐI -> mời bật SÁNG) -->
        <i class="fas fa-sun text-sm text-amber-400 transition-transform duration-300 hidden dark:inline-block rotate-0"></i>
    </button>
}
```

#### B. JavaScript điều khiển Theme chống nháy (Anti-FOUC) trong thẻ `<head>`
```html
<script>
    (function () {
        try {
            // CÔ LẬP CỬA HÀNG: Nếu đang ở /CuaHang, BẮT BUỘC bỏ Dark mode
            var pathname = window.location.pathname.toLowerCase();
            if (pathname.indexOf('/cuahang') !== -1) {
                document.documentElement.classList.remove('dark');
                document.documentElement.classList.add('light');
                return;
            }

            var savedTheme = localStorage.getItem('site_theme');
            if (savedTheme === 'dark') {
                document.documentElement.classList.add('dark');
                document.documentElement.classList.remove('light');
            } else {
                // MẶC ĐỊNH LÀ LIGHT MODE VỚI NỀN TRẮNG
                document.documentElement.classList.remove('dark');
                document.documentElement.classList.add('light');
                localStorage.setItem('site_theme', 'light');
            }
        } catch (e) {
            console.error('Theme initialization error:', e);
        }
    })();
</script>
```

#### C. JavaScript chuyển đổi trạng thái (Gắn cuối trang `_Layout.cshtml`)
```javascript
function toggleThemeMode() {
    // Ngăn chặn tuyệt đối nếu vô tình gọi trên trang Cửa Hàng
    if (window.location.pathname.toLowerCase().indexOf('/cuahang') !== -1) {
        return;
    }

    var isDark = document.documentElement.classList.contains('dark');
    if (isDark) {
        document.documentElement.classList.remove('dark');
        document.documentElement.classList.add('light');
        localStorage.setItem('site_theme', 'light');
    } else {
        document.documentElement.classList.add('dark');
        document.documentElement.classList.remove('light');
        localStorage.setItem('site_theme', 'dark');
    }

    // Phát sự kiện để các biểu đồ hoặc Swiper slider re-render nếu cần
    window.dispatchEvent(new CustomEvent('themeChanged', { detail: { isDark: !isDark } }));
}
```

---

### 4.3. Bản thiết kế Kiến trúc Cô lập Tuyệt đối cho `Areas/CuaHang/`

Để thực hiện đúng 100% cam kết:
> *"Bảo toàn 100% cấu trúc MVC/DOM và giữ nguyên toàn bộ giao diện hiện tại của phân hệ bán hàng (Areas/CuaHang/), cô lập hoàn toàn vùng này khỏi giao diện mới."*

Kiến trúc thực hiện theo mô hình **Độc lập hóa Layout (Dedicated Layout Architecture)**:

#### Bước 1: Khởi tạo Layout Riêng `_CuaHangLayout.cshtml`
- Tạo mới file: `Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml`.
- File này chứa nguyên bản cấu trúc Layout của Cửa Hàng (Header cửa hàng, Giỏ hàng, Chân trang, CSS Tailwind hiện hữu), nhưng:
  - Khóa cứng thẻ `<html>` và `<body>` ở chế độ Sáng (`class="light bg-slate-50"`).
  - Không nhúng đoạn mã Dây kéo bật đèn hay nút Dark Mode.
  - Không chịu bất kỳ quy tắc biến màu nào từ giao diện mới.

#### Bước 2: Cập nhật Điều hướng trong `Areas/CuaHang/Views/_ViewStart.cshtml`
```csharp
@{
    var controller = ViewContext.RouteData.Values["controller"]?.ToString() ?? "";
    var viewPath = ViewContext.View?.Path ?? "";

    // Phân luồng Layout riêng biệt:
    // 1. Phân hệ Quản trị Cửa Hàng -> Dùng _AdminLayout
    if (controller.StartsWith("Admin", StringComparison.OrdinalIgnoreCase) ||
        viewPath.Contains("/Admin", StringComparison.OrdinalIgnoreCase))
    {
        Layout = "~/Views/Shared/_AdminLayout.cshtml";
    }
    // 2. Phân hệ Bán hàng (Storefront) -> Dùng _CuaHangLayout RIÊNG BIỆT 100%
    else
    {
        Layout = "~/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml";
    }
}
```

#### Bước 3: Bảo toàn 100% File Views trong `Areas/CuaHang/`
Các file sau đây **giữ nguyên vẹn 100% không chỉnh sửa bất kỳ thẻ HTML nào**:
1. `Areas/CuaHang/Views/CuaHang/Index.cshtml`
2. `Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`
3. `Areas/CuaHang/Views/GioHang/Index.cshtml`
4. `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`
5. `Areas/CuaHang/Views/AdminSanPham/Index.cshtml`
6. `Areas/CuaHang/Views/AdminSanPham/Create.cshtml`
7. `Areas/CuaHang/Views/AdminSanPham/Edit.cshtml`
8. `Areas/CuaHang/Views/AdminDonHang/Index.cshtml`
9. `Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml`

#### Bước 4: Sửa lỗi Build Output của `package.json`
Chỉnh sửa file `package.json` tại thư mục gốc để Tailwind CLI biên dịch trực tiếp vào thư mục tĩnh của project MVC:
```json
"scripts": {
  "build:css": "tailwindcss -i Styles/app.css -o WEB_TinhNguyen/wwwroot/css/site.css --minify",
  "watch:css": "tailwindcss -i Styles/app.css -o WEB_TinhNguyen/wwwroot/css/site.css --watch"
}
```

---

## 5. Verification Method (Quy trình kiểm chứng độc lập)

| Hạng mục kiểm tra | Lệnh hoặc thao tác thực hiện | Tiêu chí đạt chuẩn |
|---|---|---|
| **1. Biên dịch Project** | `XDG_RUNTIME_DIR=/tmp dotnet build --no-restore` | Build thành công 100% (0 Error). Lệnh `npm run build:css` tự động chạy và xuất file `site.css` mới. |
| **2. Nền trắng & Màu chữ Tình nguyện** | Mở trình duyệt vào `https://localhost:5001/` (hoặc view `Home/Index`) | Mặc định toàn trang có nền trắng tuyền (`#ffffff`), màu chữ xanh thẫm bản sắc tình nguyện (`#143823`). Không có hiện tượng nhấp nháy màu cũ. |
| **3. Chuyển đổi Dark Mode** | Nhấn vào nút Mặt trăng trên Header | Giao diện chuyển mượt mà sang nền tối đêm rừng (`#0a140e`), chữ sáng (`#f8fafc`). Tải lại trang (F5) trạng thái Dark mode vẫn được duy trì qua `localStorage`. |
| **4. Cô lập Cửa Hàng Gây Quỹ** | Khi đang ở chế độ Tối, truy cập URL `/CuaHang` hoặc `/CuaHang/GioHang` | Trang Cửa hàng hiển thị 100% giao diện nguyên bản (nền `slate-50`, chữ `slate-900`, banner xanh thẫm, giỏ hàng, thẻ VietQR). Không có bất kỳ thành phần nào bị đen hóa hoặc vỡ layout. Nút chuyển đổi Dark Mode không xuất hiện trên Cửa hàng. |
| **5. Cấu trúc DOM Cửa Hàng** | So sánh `git diff Areas/CuaHang/Views/` | Kết quả diff bằng rỗng (0 file bị thay đổi cấu trúc thẻ HTML/DOM). |
