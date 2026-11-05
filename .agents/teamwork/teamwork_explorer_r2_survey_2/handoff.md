# Báo Cáo Khảo Sát & Đánh Giá UI/UX, Layout, Navigation Và Views (Round 2 - Survey 2)

**Người thực hiện**: Explorer (Survey 2 - Frontend UI/UX Audit)  
**Ngày thực hiện**: 2026-11-04  
**Phạm vi**: 
- `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`, `Views/Shared/_AdminLayout.cshtml`, `Views/Admin/Index.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/Index.cshtml`, `ChiTiet.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/Index.cshtml`, `DatHangThanhCong.cshtml`
- `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`, `Create.cshtml`, `Edit.cshtml`
- Các Controller liên quan: `GioHangController.cs`, `CuaHangController.cs`, `DbInitializer.cs`

---

## 1. Observation (Quan Sát Thực Nghiệm & Bằng Chứng Chi Tiết)

### 1.1. Điều Hướng (Navigation) Trên Desktop & Mobile
- **Desktop Navigation trong `Views/Shared/_Layout.cshtml`**:
  - Dòng 374-379: Mục "Cửa hàng Gây quỹ" được đặt liền kề sau "Trang chủ":
    ```html
    <a class="rounded-full px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright" asp-area="" asp-controller="Home" asp-action="Index">Trang chủ</a>
    <a class="rounded-full px-3.5 py-2 text-sm font-medium transition @(ViewContext.RouteData.Values["Area"]?.ToString() == "CuaHang" ? "bg-leaf/15 text-leaf-deep font-semibold dark:bg-white/15 dark:text-leaf-bright" : "text-gray-700 hover:bg-leaf/10 hover:text-leaf-deep dark:text-gray-100 dark:hover:bg-white/10 dark:hover:text-leaf-bright")" asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index">Cửa hàng Gây quỹ</a>
    ```
  - **Quan sát thiếu sót**: Thanh header máy tính hoàn toàn **không có biểu tượng Giỏ hàng hoặc số lượng món trong giỏ (Cart Badge)**. Người dùng đang ở Trang chủ, Tin tức hoặc thậm chí đang xem Cửa hàng không thể nhìn thấy số lượng sản phẩm mình đã chọn hay click nhanh vào giỏ hàng (`/CuaHang/GioHang`) từ đầu trang.

- **Mobile Liquid Bottom Navigation trong `Views/Shared/_Layout.cshtml`**:
  - Dòng 478-524: Thanh menu di động dạng Liquid Indicator nổi với 7 mục: Trang chủ, Cửa hàng (`/cuahang`), Sự kiện, Ký ức, Đóng góp, Tổ chức, Tin tức.
  - **Quan sát thiếu sót**:
    - Mỗi mục chiếm `100 / 7 = 14.28%` chiều rộng màn hình. Trên các dòng điện thoại màn hình nhỏ (360px - 390px), các biểu tượng và chữ rất sát nhau.
    - Hoàn toàn **không có nút Giỏ hàng trên thanh điều hướng di động**. Khi người dùng lướt web trên điện thoại và chọn mua sản phẩm, không có lối tắt cố định để vào giỏ hàng thanh toán.

- **Admin Sidebar & Dashboard Navigation**:
  - `Views/Shared/_AdminLayout.cshtml` dòng 240-307: Sidebar chỉ chứa các mục: Bảng điều khiển, Cấu hình Trang chủ, Cấu hình Chương trình, Cấu hình Đóng Góp, **Sản phẩm Gây Quỹ** (`/CuaHang/AdminSanPham`), Cấu hình Ký ức, Cấu hình Ban Tổ Chức, Cấu hình Tin tức, Đăng ký TNV, Xem trang ngoài.
  - `Views/Admin/Index.cshtml` dòng 105-157: Chỉ có các thẻ lối tắt chức năng cho Trang chủ, Sản phẩm Gây Quỹ, Ký ức, Tin tức, Lời chúc.
  - **Quan sát thiếu sót**: Hoàn toàn **KHÔNG có menu "Quản lý Đơn hàng Gây quỹ"** (`/CuaHang/AdminDonHang`) trong Sidebar hay Dashboard. Cơ sở dữ liệu đã có bảng `DonHangGayQuys` và `ChiTietDonHangGayQuys`, nhưng Admin không có trang để kiểm tra ai đã đặt mua, xem mã chuyển khoản VietQR, duyệt đơn hay cập nhật trạng thái giao hàng.

---

### 1.2. Danh Mục Cửa Hàng & Bộ Lọc (`Areas/CuaHang/Views/CuaHang/Index.cshtml`)
- Dòng 79-141: Form tìm kiếm, sắp xếp và các pill danh mục:
  ```html
  <form method="get" asp-area="CuaHang" asp-controller="CuaHang" asp-action="Index" id="filterForm">
  ```
- **Quan sát thực tế**:
  - Tìm kiếm (`q`), sắp xếp (`sort`) và danh mục (`danhMuc`) hoạt động qua phương thức HTTP GET truyền thống. Mỗi thao tác đều tải lại toàn bộ trang (full-page reload), gây giật màn hình và cuộn ngược lên đầu trang.
  - **Không có tìm kiếm thời gian thực (Live Search / Debounced Search)** khi người dùng đang gõ phím.
  - **Không có bộ lọc khoảng giá** (ví dụ: Dưới 50.000₫, 50.000₫ - 150.000₫, Trên 150.000₫).
  - Nút thêm vào giỏ trên từng thẻ sản phẩm (dòng 243-250) dùng thẻ `<form method="post">` truyền thống:
    ```html
    <form asp-area="CuaHang" asp-controller="GioHang" asp-action="ThemVaoGio" method="post" class="inline-block">
    ```
    Khi bấm nút, trang bị reload hoàn toàn và chỉ hiển thị thông báo trong thẻ `TempData["SuccessMessage"]` tĩnh ở đầu trang (dòng 57-68).

---

### 1.3. Trang Chi Tiết Sản Phẩm (`Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`)
- Giao diện 2 cột responsive đẹp mắt, có ảnh sản phẩm kèm badge "100% Lợi Nhuận Gây Quỹ", 3 cam kết thiện nguyện (Giao toàn quốc, Gây quỹ 100%, Minh bạch sao kê), khối thông điệp thiện nguyện, bộ chọn số lượng (+/-) và danh sách 4 sản phẩm liên quan.
- **Quan sát thiếu sót**:
  - Dòng 155-207: Form thêm vào giỏ vẫn là form submit POST đồng bộ. Bấm thêm vào giỏ sẽ reload trang, làm mất vị trí xem của người dùng.
  - Thiếu nút "Mua ngay" (chuyển thẳng đến giỏ hàng để điền thông tin quyên góp).

---

### 1.4. Trang Giỏ Hàng & Đặt Mua (`Areas/CuaHang/Views/GioHang/Index.cshtml`)
- Bố cục 2 cột (Cột trái 7 phần: Danh sách sản phẩm; Cột phải 5 phần: Form đặt hàng & thanh toán sticky).
- **Quan sát tương tác**:
  - Dòng 112-139: Các nút tăng/giảm số lượng và ô nhập số lượng:
    ```html
    <form asp-area="CuaHang" asp-controller="GioHang" asp-action="CapNhatSoLuong" method="post" class="inline">
    ...
    <input type="number" name="soLuong" value="@item.SoLuong" onchange="this.form.submit()" />
    ```
    Mỗi lần bấm nút `+` hoặc `-` đều gửi 1 form POST và tải lại toàn bộ trang! Người dùng không thể bấm nhanh liên tiếp 2 lần.
  - Dòng 73: Nút xóa toàn bộ giỏ hàng dùng:
    ```html
    onsubmit="return confirm('Bạn có chắc chắn muốn làm trống toàn bộ giỏ hàng?');"
    ```
    Đây là hộp thoại alert/confirm mặc định của trình duyệt, thô sơ và không đồng bộ thiết kế Tailwind.
  - Dòng 149-155: Nút xóa từng dòng sản phẩm không có xác nhận, nếu người dùng bấm nhầm trên màn hình cảm ứng sẽ mất ngay sản phẩm.
  - Dòng 320-323: Nút "XÁC NHẬN ĐẶT HÀNG & ỦNG HỘ QUỸ" không có hiệu ứng loading spinner hoặc tự vô hiệu hóa (disabled) khi submit, dễ xảy ra tình trạng người dùng click đúp gửi 2 đơn hàng trùng lặp.
  - **Khám phá quan trọng trong Controller**: `GioHangController.cs` dòng 60-66, 103, 150, 188, 219:
    ```csharp
    private bool IsAjaxRequest() { ... }
    // Controller ĐÃ HỖ TRỢ TRẢ VỀ JSON cho ThemVaoGio, CapNhatSoLuong, XoaKhoiGio!
    ```
    Backend đã sẵn sàng phục vụ AJAX hoàn hảo, nhưng Frontend lại chưa tận dụng mà vẫn dùng form submit reload trang!

---

### 1.5. Trang Xác Nhận Đơn Hàng & VietQR (`Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`)
- Dòng 7: Tự động render mã VietQR động qua chuẩn Napas247 (`https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}&accountName=NGUYEN%20THAI%20VINH`).
- Hiển thị đầy đủ thông tin tài khoản ngân hàng Quân Đội (MBBank), STK 0924044283, chủ TK NGUYEN THAI VINH, mã đơn hàng.
- **Quan sát thiếu sót**:
  - **Không có nút "Sao chép" (Copy to clipboard)** cho STK và Nội dung chuyển khoản. Người dùng mobile phải bôi đen chọn số rất dễ nhầm lẫn.
  - **Không có thanh tiến trình trạng thái trực quan (Visual Order Tracking)**: Hiện chỉ là trang tĩnh, chưa có timeline các bước: (1) Đã đặt hàng -> (2) Chờ xác nhận chuyển khoản -> (3) Đang đóng gói quà -> (4) Giao hàng thành công.
  - Không có nút "In phiếu đóng góp / Lưu biên lai" phục vụ minh chứng ủng hộ.

---

### 1.6. Quản Trị Admin Sản Phẩm (`AdminSanPham/Index.cshtml`, `Create.cshtml`, `Edit.cshtml`)
- `AdminSanPham/Index.cshtml`:
  - Dòng 333: Thao tác xóa dùng `onsubmit="return confirm('Bạn có chắc chắn muốn xóa sản phẩm ...');"` (Browser confirm dialog cũ kỹ).
  - Thiếu nút chuyển nhanh trạng thái (Hiển thị / Ẩn) ngay trên dòng bảng (Toggle Switch AJAX).
- `Create.cshtml` & `Edit.cshtml`:
  - Form thiết kế rất chi tiết, có preview ảnh trực tiếp và thanh kéo tỷ lệ trích quỹ (Range slider).
  - Thiếu thông báo Toast đẹp mắt khi cập nhật thành công (hiện đang dùng TempData banner truyền thống).

---

### 1.7. Hệ Thống Thông Báo Toàn Cục (Toast Notifications / SweetAlert2)
- Cả `_Layout.cshtml` và `_AdminLayout.cshtml` đều **chưa tích hợp thư viện Toast hoặc SweetAlert2**.
- Mọi phản hồi đều là thẻ HTML box tĩnh hiển thị ở đầu trang (nếu người dùng đang cuộn ở cuối trang sẽ không nhìn thấy thông báo thành công).

---

## 2. Logic Chain (Chuỗi Lập Luận Từ Quan Sát Đến Giải Pháp)

1. **Từ việc thiếu Cart Badge trên Header và thiếu nút Giỏ hàng trên Mobile Navigation**:
   - *Lý do*: Người dùng khi vào website gây quỹ có thể xem nhiều trang trước khi quyết định mua. Nếu không thấy số lượng giỏ hàng hiển thị thường trực, họ dễ quên hoặc không biết cách mở giỏ hàng.
   - *Kết luận*: Cần bổ sung biểu tượng Giỏ hàng có Badge đếm số lượng động trên Header Desktop và tạo 1 nút Floating Cart Button (nút tròn nổi bật có huy hiệu số) ở góc phải dưới màn hình (trên di động nằm trên thanh bottom nav), giúp truy cập giỏ hàng 1 chạm từ bất kỳ vị trí nào.

2. **Từ việc `GioHangController` đã hỗ trợ sẵn AJAX JSON (`IsAjaxRequest()`) nhưng Views vẫn submit form POST**:
   - *Lý do*: Các view trước đó được viết theo mẫu MVC cơ bản, chưa kết nối JavaScript Fetch API.
   - *Kết luận*: Nâng cấp JavaScript trên `CuaHang/Index.cshtml`, `ChiTiet.cshtml` và `GioHang/Index.cshtml` để gọi AJAX. Khi người dùng bấm "Thêm vào giỏ", "Tăng/Giảm số lượng", hoặc "Xóa món", hệ thống thực hiện ngầm, cập nhật số liệu ngay lập tức trên DOM và bắn Toast thông báo ngọt ngào mà không hề chớp/nháy trang.

3. **Từ việc dùng `confirm(...)` mặc định của trình duyệt**:
   - *Lý do*: Hàm `window.confirm` chặn luồng trình duyệt, giao diện phụ thuộc hệ điều hành (trên Linux/Windows/Android trông rất thô), không thể tùy biến màu sắc hay icon tình nguyện.
   - *Kết luận*: Cần thay thế bằng Modern Interactive Modal (Tailwind Modal hoặc SweetAlert2) cho các thao tác: Xóa toàn bộ giỏ hàng, xóa từng món trong giỏ, và xóa sản phẩm trong Admin.

4. **Từ việc thiếu tính năng Quản lý Đơn hàng cho Admin**:
   - *Lý do*: Đợt triển khai trước tập trung hoàn thiện CRUD Sản phẩm gây quỹ và luồng thanh toán VietQR cho người dùng, chưa dựng Controller và View quản trị đơn hàng cho Admin.
   - *Kết luận*: Cần xây dựng phân hệ `AdminDonHangController` trong Area `CuaHang`, tích hợp liên kết "Đơn hàng Gây Quỹ" vào `_AdminLayout.cshtml` và thêm thẻ thống kê đơn hàng trên `Views/Admin/Index.cshtml`.

5. **Từ trải nghiệm thanh toán VietQR trên di động**:
   - *Lý do*: Việc chuyển khoản ngân hàng đòi hỏi độ chính xác tuyệt đối ở STK và Mã đơn hàng (nội dung chuyển khoản).
   - *Kết luận*: Bổ sung 2 nút "Sao chép số tài khoản" và "Sao chép mã đơn hàng" có hiệu ứng tooltip "Đã sao chép" giúp người dùng chuyển app ngân hàng dán mã trong 2 giây mà không sợ sai lệch. Bổ sung Step Tracker 4 giai đoạn giúp người đóng góp an tâm.

---

## 3. Caveats (Phạm Vi & Giới Hạn)

1. **Quyền hạn Explorer**: Bản khảo sát này là điều tra đọc mã và đánh giá hiện trạng (Read-only investigation). Mọi giải pháp mã nguồn được mô tả chi tiết dưới dạng thiết kế kỹ thuật và code snippet để các Agent triển khai (Implementer) thực hiện an toàn.
2. **Khả năng tương thích cơ sở dữ liệu kép**: Hệ thống đang hỗ trợ song song SQL Server và In-Memory Store (`DbInitializer`). Mọi bổ sung cho chức năng Quản lý Đơn hàng Admin cần đảm bảo hoạt động tốt trên cả 2 chế độ này (không được phụ thuộc cứng vào chỉ 1 nguồn dữ liệu).
3. **Hiệu năng Tailwind CDN**: Dự án hiện dùng cả Tailwind CDN và bản build qua `Styles/app.css`. Các class tiện ích Tailwind mới được đề xuất nằm trong bộ chuẩn của Tailwind v3, đảm bảo tương thích 100%.

---

## 4. Conclusion & Kế Hoạch Nâng Cấp UI/UX Chi Tiết (Actionable Plan)

Bảng tổng hợp 7 nhóm giải pháp nâng cấp trải nghiệm người dùng:

| STT | Khu vực | Hiện trạng | Giải pháp nâng cấp chi tiết |
|:---:|:---|:---|:---|
| **1** | **Header & Navigation** | Chưa có Cart Badge trên Header; Mobile Nav 7 mục thiếu Giỏ hàng | - Thêm giỏ hàng mini trên Desktop Header cạnh link Cửa hàng, hiển thị số lượng sản phẩm realtime.<br>- Thêm Floating Cart Button ở góc phải dưới (hiển thị trên mobile & desktop) kèm hiệu ứng rung/bounce khi có món mới. |
| **2** | **Bộ lọc & Tìm kiếm Storefront** | Form GET truyền thống, reload toàn trang, thiếu lọc giá | - Thêm Live Search thời gian thực (lọc ngay trên client/AJAX).<br>- Thêm các nút lọc khoảng giá nhanh (Tất cả, <50k, 50k-150k, >150k) và nút lọc "100% Gây quỹ". |
| **3** | **Tương tác Thêm vào giỏ & Thông báo** | Reload trang, cuộn lên đầu, thông báo TempData tĩnh | - Tích hợp Toast Notification chuẩn đẹp (SweetAlert2 hoặc Tailwind Toast) hiển thị góc trên phải.<br>- Kết nối AJAX với `GioHangController.ThemVaoGio`, thêm sản phẩm không reload trang, cập nhật tức thì Cart Badge. |
| **4** | **Thao tác Giỏ hàng (Cart UI)** | Tăng/giảm số lượng reload trang; alert confirm mặc định | - AJAX hóa các nút `+`, `-` và xóa sản phẩm trong `GioHang/Index.cshtml`, tự động cập nhật Tổng tiền hàng, Tiền ủng hộ thêm, Tổng thanh toán và Quỹ đóng góp.<br>- Thay thế `confirm()` bằng Interactive Tailwind Modal xác nhận xóa.<br>- Thêm trạng thái Spinner cho nút "Xác nhận đặt hàng" để chống click đúp. |
| **5** | **Trang Xác nhận VietQR** | Chưa có nút Copy, chưa có Visual Tracking | - Thêm nút "Sao chép STK" và "Sao chép Mã đơn" 1-click với Toast phản hồi.<br>- Bổ sung Visual Step Tracker 4 bước: (1. Đặt hàng thành công ➔ 2. Xác nhận thanh toán ➔ 3. Đóng gói & Chuẩn bị ➔ 4. Giao hàng tình nguyện).<br>- Thêm nút In hóa đơn / Tải chứng nhận đóng góp gây quỹ. |
| **6** | **Quản trị Đơn hàng Admin** | Chưa có trang quản lý đơn hàng, chưa có menu trong Admin Layout | - Xây dựng `AdminDonHangController` và view danh sách đơn hàng có bộ lọc trạng thái (Mới đặt, Đã xác nhận, Đang giao, Hoàn thành) và trạng thái thanh toán (Chưa TT, Đã TT).<br>- Thêm mục "Đơn hàng Gây Quỹ" vào Sidebar `_AdminLayout.cshtml`.<br>- Thêm thẻ thống kê đơn hàng trên `Views/Admin/Index.cshtml`. |
| **7** | **Quản trị Sản phẩm Admin** | Dùng `confirm()` xóa sản phẩm, thiếu Toggle trạng thái | - Thay `confirm()` xóa bằng Confirmation Modal trực quan.<br>- Bổ sung nút Quick Toggle (Bật/Tắt hiển thị sản phẩm) qua AJAX không cần vào trang Edit. |

---

## 5. Đề Xuất Mã Nguồn & Thiết Kế Kỹ Thuật Chi Tiết (Technical Specifications)

### 5.1. Bổ sung Toast Notification & Cart Badge trên Layout
**File**: `Views/Shared/_Layout.cshtml`
- Nhúng SweetAlert2 qua CDN:
  ```html
  <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
  ```
- Thêm Cart Badge trên Desktop Header (cạnh nút Cửa hàng):
  ```html
  <a asp-area="CuaHang" asp-controller="GioHang" asp-action="Index" 
     class="relative inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-bold bg-emerald-50 text-emerald-700 hover:bg-emerald-100 transition shadow-xs">
      <i class="fas fa-shopping-basket text-emerald-600"></i>
      <span class="hidden sm:inline">Giỏ quà</span>
      <span id="cartCountBadge" class="inline-flex items-center justify-center min-w-[20px] h-5 px-1.5 text-[11px] font-black text-white bg-rose-500 rounded-full">
          @(Context.Session.Get<List<CartItem>>("GioHangGayQuy")?.Sum(i => i.SoLuong) ?? 0)
      </span>
  </a>
  ```
- Thêm Floating Cart Button (Góc dưới phải màn hình):
  ```html
  <div id="floatingCartBtn" class="fixed right-5 bottom-24 md:bottom-8 z-40">
      <a asp-area="CuaHang" asp-controller="GioHang" asp-action="Index" 
         class="flex items-center gap-2.5 px-4 py-3 rounded-full bg-gradient-to-r from-emerald-600 to-green-700 text-white shadow-xl hover:shadow-2xl hover:scale-105 transition-all">
          <i class="fas fa-shopping-basket text-lg animate-bounce"></i>
          <span class="font-bold text-xs hidden sm:inline">Giỏ quà gây quỹ</span>
          <span id="floatingCartCount" class="w-6 h-6 rounded-full bg-rose-500 text-white font-black text-xs flex items-center justify-center">
              @(Context.Session.Get<List<CartItem>>("GioHangGayQuy")?.Sum(i => i.SoLuong) ?? 0)
          </span>
      </a>
  </div>
  ```

### 5.2. AJAX Add-to-Cart với Toast Notification
**File**: `Areas/CuaHang/Views/CuaHang/Index.cshtml` & `ChiTiet.cshtml`
```javascript
async function addToCartAjax(productId, qty = 1, e) {
    if (e) e.preventDefault();
    try {
        const formData = new FormData();
        formData.append('id', productId);
        formData.append('soLuong', qty);

        const response = await fetch('/CuaHang/GioHang/ThemVaoGio', {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: formData
        });
        const data = await response.json();

        if (data.success) {
            // Cập nhật số lượng trên các badge
            document.querySelectorAll('#cartCountBadge, #floatingCartCount').forEach(el => {
                el.textContent = data.totalQuantity;
            });

            // Bắn Toast ngọt ngào
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: data.message,
                showConfirmButton: false,
                timer: 2500,
                timerProgressBar: true
            });
        }
    } catch (err) {
        window.location.href = '/CuaHang/GioHang/ThemVaoGio?id=' + productId + '&soLuong=' + qty;
    }
}
```

### 5.3. Thêm Menu "Đơn Hàng Gây Quỹ" Trong `_AdminLayout.cshtml`
**Vị trí**: Liền kề dưới "Sản phẩm Gây Quỹ" (dòng 272):
```html
<li>
    <a href="/CuaHang/AdminDonHang" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminDonHang" ? "active" : "")">
        <i class="fas fa-receipt"></i> <span class="link-text">Đơn hàng Gây Quỹ</span>
    </a>
</li>
```

---

## 6. Verification Method (Phương Pháp Kiểm Tra & Xác Nhận Độc Lập)

1. **Lệnh build kiểm tra mã nguồn**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj --no-restore
   ```
   *Điều kiện thành công*: Biên dịch 0 lỗi, 0 cảnh báo. Đã kiểm chứng thành công trong lượt chạy lúc 07:28:15.

2. **Lệnh chạy toàn bộ 280 test case hồi quy**:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet run --project /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/E2ETests.csproj --no-restore
   ```
   *Kết quả hiện tại*: 280/280 tests Passed (Tier 1: 85, Tier 2: 25, Tier 3: 16, Tier 4: 12, Tier 5: 142).

3. **Kiểm tra trực quan các view**:
   - `Views/Shared/_Layout.cshtml`: Kiểm tra sự hiện diện của Cart Badge và Floating Cart.
   - `Views/Shared/_AdminLayout.cshtml`: Kiểm tra link menu `AdminDonHang`.
   - `Areas/CuaHang/Views/CuaHang/Index.cshtml`: Kiểm tra bộ lọc giá, tìm kiếm real-time và submit AJAX.
   - `Areas/CuaHang/Views/GioHang/Index.cshtml`: Kiểm tra tăng/giảm số lượng mượt mà không reload trang.
   - `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`: Kiểm tra nút Copy STK/Mã đơn và Step Tracker.
