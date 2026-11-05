# BÁO CÁO NGHIÊN CỨU & ĐẶC TẢ TÍCH HỢP ĐIỀU HƯỚNG ADMIN VÀ ĐỘ TƯƠNG THÍCH KIỂM THỬ E2E (M2-3)

## 1. Observation (Quan sát thực tế)

Qua việc kiểm tra trực tiếp mã nguồn, bố cục giao diện và bộ kiểm thử tự động, các quan sát kỹ thuật sau đã được xác minh chính xác:

1. **Vị trí và cấu trúc Sidebar trong `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`**:
   - Tệp layout quản trị tồn tại tại: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml` (342 dòng).
   - Sidebar được thiết kế kiểu *hover-to-expand* ("Boys - Tech" style), nền tối `#161a2d`, bo góc `rounded-25px`, biểu tượng FontAwesome, các hiệu ứng hover/active sử dụng điểm nhấn màu xanh ngọc `#4caf50` (lines 67, 75, 205, 211).
   - Danh sách menu `<ul class="sidebar-links">` (lines 240-302) phân cấp theo các tiêu đề nhóm `<h4>`:
     - Nhóm **Nội dung** (lines 247-282): gồm `AdminTrangChu`, `AdminChuongTrinh`, `AdminGayQuy` (lines 262-266), `AdminKyUc` (lines 267-271), `AdminBanToChuc`, `AdminTinTuc`.
     - Nhóm **Tương tác** (lines 283-292): Đăng ký TNV.
     - Nhóm **Hệ thống** (lines 294-301): Nút "Xem trang ngoài" (`/Home/Index`).
   - Hiện tại tệp `_AdminLayout.cshtml` **chưa có** mục liên kết tới Controller `AdminSanPham` hoặc Area `CuaHang`.

2. **Cấu trúc Lối tắt Dashboard trong `WEB_TinhNguyen/Views/Admin/Index.cshtml`**:
   - Tệp tồn tại tại: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Views/Admin/Index.cshtml` (148 dòng).
   - Khối "Lựa chọn Chức năng Quản trị" (lines 104-147) có tiêu đề và lưới `grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4` chứa 4 thẻ quick link: Giao diện Trang chủ, Quản lý Ký ức, Quản lý Tin tức, Duyệt Lời chúc.
   - Chưa có thẻ truy cập nhanh dành riêng cho phân hệ Quản lý Sản phẩm Gây Quỹ.

3. **Hiện trạng bộ kiểm thử E2E trong `tests/E2ETests/Tier1/Tier1_AdminTests.cs`**:
   - Bộ kiểm thử độc lập gồm 25 test cases bao phủ Features 12, 13, 14, 15, 16.
   - Khi chạy lệnh `./tests/run_e2e_tests.sh --tier 1`:
     - `F12_T01` (AdminLayoutExists): **PASS**
     - `F12_T02` (AdminSidebarContainsProductLink): **[SKIP]** `Pending implementation in M2: AdminSanPham menu link in _AdminLayout.cshtml`
     - `F12_T03` (AdminSidebarLinkTitle): **[SKIP]** `Pending implementation in M2: Admin sidebar menu item title in _AdminLayout.cshtml`
     - `F12_T04` (AdminSidebarLinkIcon): **[SKIP]** `Pending implementation in M2: AdminSanPham menu icon in _AdminLayout.cshtml`
     - `F12_T05` (AdminSidebarEmeraldStyling): **PASS** (tìm thấy `#4caf50`)
     - `F13_T01`-`F13_T05` (Admin Product Table View): **[SKIP]** do thiếu view `Areas/CuaHang/Views/AdminSanPham/Index.cshtml`.
     - `F14_T01`, `F14_T02`, `F14_T04`, `F14_T05`: **[SKIP]** do thiếu `Create.cshtml`.
     - `F14_T03` (ViewModel validation): **PASS** (`TenSanPham` có `[Required]`).
     - `F15_T01`-`F15_T04`: **[SKIP]** do thiếu `Edit.cshtml`.
     - `F15_T05` (Preserves existing image): **PASS** (`UpdateEntity` giữ nguyên ảnh cũ khi giá trị mới null).
     - `F16_T01`-`F16_T02`: **[SKIP]** do thiếu `AdminSanPhamController`.
     - `F16_T03`: **[SKIP]** do thiếu view `Index.cshtml`.
     - `F16_T04`-`F16_T05`: **PASS** (`DeleteInMemoryProduct` trong `DbInitializer.cs` hoạt động chuẩn).
   - Trong `Tier4_RealWorldScenarioTests.cs`:
     - `S07_AdminFlow_NavigationToAdminProducts`: **[SKIP]** do thiếu `AdminSanPham` trong `_AdminLayout.cshtml`.

4. **Kế thừa Layout tự động trong `Areas/CuaHang/Views/_ViewStart.cshtml`**:
   - `_ViewStart.cshtml` đã được cấu hình tự động: Nếu controller bắt đầu bằng `"Admin"` hoặc đường dẫn view chứa `"/Admin"`, hệ thống tự động gán `Layout = "~/Views/Shared/_AdminLayout.cshtml"`. Điều này đã được kiểm chứng thông qua bài test `F17_T05` và `C14_AutomaticLayoutSwitching` (**PASS**).

---

## 2. Logic Chain (Chuỗi lập luận kỹ thuật)

1. **Chuỗi lập luận cho Tích hợp Điều hướng Sidebar (`_AdminLayout.cshtml`)**:
   - *Bước 1 (Vị trí phân loại)*: Theo quy hoạch nghiệp vụ của website Trăng Sáng Langbiang, hoạt động bán sản phẩm gây quỹ thuộc nhóm nội dung chiến dịch. Vì nhóm "Nội dung" đã có `AdminGayQuy` (Cấu hình Đóng Góp), việc đặt mục "Sản phẩm Gây Quỹ" ngay sau `AdminGayQuy` (dòng 266) tạo sự gắn kết ngữ cảnh gây quỹ tự nhiên nhất cho Quản trị viên.
   - *Bước 2 (Địa chỉ liên kết URL & Routing)*: Route Area được định nghĩa trong `Program.cs` là `"{area:exists}/{controller=CuaHang}/{action=Index}/{id?}"`. Do đó, URL `/CuaHang/AdminSanPham` sẽ kích hoạt `AdminSanPhamController.Index()` trong Area `CuaHang`.
   - *Bước 3 (Thỏa mãn kiểm thử F12_T02 & S07)*: Test yêu cầu layout phải chứa xâu `"AdminSanPham"` và `"CuaHang"`. Việc dùng `href="/CuaHang/AdminSanPham"` trực tiếp thỏa mãn cả hai điều kiện của `F12_T02` và `S07`.
   - *Bước 4 (Thỏa mãn kiểm thử F12_T03)*: Test yêu cầu chuỗi tiêu đề phải chứa `"Sản phẩm Gây Quỹ"`. Khai báo thẻ `<span class="link-text">Sản phẩm Gây Quỹ</span>` khớp chính xác 100% từng ký tự Unicode tiếng Việt.
   - *Bước 5 (Thỏa mãn kiểm thử F12_T04)*: Test yêu cầu biểu tượng FontAwesome chứa một trong các lớp: `fa-box`, `fa-store`, `fa-gift`, `fa-shopping-cart`. Thẻ `<i class="fas fa-boxes"></i>` chứa chuỗi con `"fa-box"`, vừa khớp yêu cầu kiểm thử vừa biểu thị kho hàng/sản phẩm trực quan.
   - *Bước 6 (Cơ chế Active class)*: Khi truy cập bất kỳ action nào của `AdminSanPhamController`, giá trị `ViewContext.RouteData.Values["Controller"]?.ToString()` là `"AdminSanPham"`. Biểu thức `@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")` sẽ kích hoạt class `active`, làm sáng icon sang màu xanh ngọc `#4caf50` và nổi bật nền mục chọn đồng bộ với các mục khác trong layout.

2. **Chuỗi lập luận cho Lối tắt Dashboard (`Views/Admin/Index.cshtml`)**:
   - *Bước 1 (Đồng bộ thị giác)*: Bảng điều khiển quản trị là màn hình đầu tiên admin nhìn thấy khi đăng nhập. Việc bổ sung card "Sản phẩm Gây Quỹ" tại đây giúp quản trị viên truy cập ngay lập tức chỉ với 1 click mà không cần mở rộng sidebar.
   - *Bước 2 (Bố cục Responsive)*: Chuyển lưới từ `grid-cols-1 sm:grid-cols-2 md:grid-cols-4` sang `grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4` đảm bảo 5 chức năng quản trị hiển thị cân đối trên cả desktop lẫn tablet/mobile.
   - *Bước 3 (Nhận diện thương hiệu)*: Card sử dụng màu `emerald` (`bg-emerald-50 text-emerald-600 hover:border-emerald-500 group-hover:bg-emerald-600 group-hover:text-white`) làm nổi bật tính chất thiện nguyện, đi kèm icon `fas fa-boxes text-lg` và mô tả ngắn "Quản lý kho, thêm/sửa sản phẩm...".

3. **Chuỗi lập luận cho Tương thích Kiểm thử (Features 12 - 16)**:
   - *Feature 12 (Sidebar Nav)*: 5/5 assertions đã được bảo đảm bằng giải pháp tại Mục 1.
   - *Feature 13 (Table View)*:
     - `F13_T02` đòi hỏi `LinkHinhAnh` hoặc `<img`. View `Index.cshtml` dùng `<img src="@item.LinkHinhAnh" ... />`.
     - `F13_T03` đòi hỏi `TenSanPham`, `GiaBan`, `SoLuongTon`. Cả 3 trường dữ liệu này đều có mặt trong bảng.
     - `F13_T04` đòi hỏi `"Create"` hoặc `"Thêm mới"`. Button thêm mới dùng `asp-action="Create"` và nhãn `"Thêm mới sản phẩm"`.
     - `F13_T05` đòi hỏi (`"Edit"` hoặc `"Sửa"`) và (`"Delete"` hoặc `"Xóa"`). Hàng dữ liệu có nút Sửa (`asp-action="Edit"`, `"Sửa"`) và nút Xóa (`asp-action="Delete"`, `"Xóa"`).
   - *Feature 14 (Create Form)*:
     - `F14_T02` đòi hỏi `enctype="multipart/form-data"`. Form thêm mới bắt buộc phải có thuộc tính này.
     - `F14_T04` đòi hỏi input `PhanTramDongGop`. Trường này đã sẵn sàng với giá trị mặc định là 100%.
     - `F14_T05` đòi hỏi `type="submit"`. Nút Lưu có `type="submit"`.
   - *Feature 15 (Edit Form)*:
     - `F15_T02` đòi hỏi `asp-for="IDSanPham"` và `type="hidden"`. Form chỉnh sửa bắt buộc chứa `<input type="hidden" asp-for="IDSanPham" />`.
     - `F15_T03` đòi hỏi `LinkHinhAnh` hoặc `<img` để xem trước ảnh hiện tại.
     - `F15_T04` đòi hỏi `enctype="multipart/form-data"` để hỗ trợ thay đổi ảnh đại diện.
   - *Feature 16 (Delete Action)*:
     - **Phát hiện quan trọng**: Test `F16_T01` và `F16_T02` sử dụng reflection `ctrlType.GetMethod("Delete")`. Nếu controller khai báo 2 overload phương thức cùng tên `Delete` (ví dụ GET `Delete(int id)` và POST `Delete(int id)`), hàm `GetMethod("Delete")` của .NET Core Reflection sẽ ném ngoại lệ `AmbiguousMatchException`. Do đó, `AdminSanPhamController` **bắt buộc chỉ được khai báo 1 phương thức công khai duy nhất mang tên `Delete(int id)`** nhận tham số `int id` và gắn thuộc tính `[HttpPost]`, `[ValidateAntiForgeryToken]`.
     - `F16_T03` đòi hỏi chuỗi `"confirm"` hoặc `"Xác nhận"` hoặc `"bạn có chắc"`. Form xóa trên view `Index.cshtml` sử dụng `onsubmit="return confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?');"`.
     - Đồng bộ In-Memory fallback: Sau khi xóa trong DB (hoặc khi DB offline), bắt buộc gọi `DbInitializer.DeleteInMemoryProduct(id)` để đồng bộ bộ nhớ demo, thỏa mãn `F16_T04` và `F16_T05`.

---

## 3. Caveats (Lưu ý kỹ thuật)

1. **Ràng buộc khóa ngoại (Foreign Key) khi xóa sản phẩm**:
   - Nếu sản phẩm đã phát sinh chi tiết trong bảng `ChiTietDonHangGayQuys`, việc xóa cứng vật lý sẽ gây ngoại lệ `DbUpdateException`. Trong `AdminSanPhamController`, Worker cần xử lý bắt lỗi này và chuyển sang "xóa mềm" (chuyển cờ `TrangThai = false`), đồng thời thông báo rõ cho người dùng.
2. **Quyền riêng tư thư mục và quyền ghi tệp hình ảnh**:
   - Thư mục tải ảnh `wwwroot/uploads/sanpham/` cần được kiểm tra và tự động khởi tạo bằng `Directory.CreateDirectory()` nếu chưa tồn tại.
3. **Độ an toàn CSRF**:
   - Toàn bộ các form thao tác dữ liệu (`Create`, `Edit`, `Delete`) phải luôn kèm `@Html.AntiForgeryToken()` trên Razor view và `[ValidateAntiForgeryToken]` trên Controller actions.

---

## 4. Conclusion (Kết luận & Đề xuất hành động chi tiết)

### Đề xuất 1: Mã nguồn tích hợp vào `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`
Chèn đoạn mã sau vào giữa dòng 266 và 267 (ngay sau mục `AdminGayQuy` và trước `AdminKyUc`):

```html
                <li>
                    <a href="/CuaHang/AdminSanPham" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")">
                        <i class="fas fa-boxes"></i> <span class="link-text">Sản phẩm Gây Quỹ</span>
                    </a>
                </li>
```

### Đề xuất 2: Mã nguồn tích hợp vào `WEB_TinhNguyen/Views/Admin/Index.cshtml`
Thay thế khối lưới Quick Links tại dòng 105 bằng:

```html
    <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">

        <a href="@Url.Action("Index", "AdminTrangChu")" class="bg-white border border-gray-200 rounded-xl p-5 flex items-center gap-4 hover:border-green-500 hover:shadow-lg transition-all group">
            <div class="w-12 h-12 rounded-full bg-green-50 text-green-600 flex items-center justify-center group-hover:bg-green-600 group-hover:text-white transition-colors">
                <i class="fas fa-home text-lg"></i>
            </div>
            <div>
                <p class="font-bold text-gray-800 text-sm mb-0.5 group-hover:text-green-700 transition-colors">Giao diện Trang chủ</p>
                <p class="text-xs text-gray-500">Banner, Form đếm ngược...</p>
            </div>
        </a>

        <!-- Lối tắt Quản lý Sản phẩm Gây Quỹ -->
        <a href="/CuaHang/AdminSanPham" class="bg-white border border-gray-200 rounded-xl p-5 flex items-center gap-4 hover:border-emerald-500 hover:shadow-lg transition-all group">
            <div class="w-12 h-12 rounded-full bg-emerald-50 text-emerald-600 flex items-center justify-center group-hover:bg-emerald-600 group-hover:text-white transition-colors">
                <i class="fas fa-boxes text-lg"></i>
            </div>
            <div>
                <p class="font-bold text-gray-800 text-sm mb-0.5 group-hover:text-emerald-700 transition-colors">Sản phẩm Gây Quỹ</p>
                <p class="text-xs text-gray-500">Quản lý kho, thêm/sửa sản phẩm...</p>
            </div>
        </a>

        <a href="@Url.Action("Index", "AdminKyUc")" class="bg-white border border-gray-200 rounded-xl p-5 flex items-center gap-4 hover:border-blue-500 hover:shadow-lg transition-all group">
            <div class="w-12 h-12 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center group-hover:bg-blue-600 group-hover:text-white transition-colors">
                <i class="fas fa-history text-lg"></i>
            </div>
            <div>
                <p class="font-bold text-gray-800 text-sm mb-0.5 group-hover:text-blue-700 transition-colors">Quản lý Ký ức</p>
                <p class="text-xs text-gray-500">Thêm năm dự án, Gallery...</p>
            </div>
        </a>

        <a href="#" class="bg-white border border-gray-200 rounded-xl p-5 flex items-center gap-4 hover:border-orange-500 hover:shadow-lg transition-all group">
            <div class="w-12 h-12 rounded-full bg-orange-50 text-orange-600 flex items-center justify-center group-hover:bg-orange-600 group-hover:text-white transition-colors">
                <i class="fas fa-newspaper text-lg"></i>
            </div>
            <div>
                <p class="font-bold text-gray-800 text-sm mb-0.5 group-hover:text-orange-700 transition-colors">Quản lý Tin tức</p>
                <p class="text-xs text-gray-500">Đăng bài viết mới</p>
            </div>
        </a>

        <a href="#" class="bg-white border border-gray-200 rounded-xl p-5 flex items-center gap-4 hover:border-pink-500 hover:shadow-lg transition-all group">
            <div class="w-12 h-12 rounded-full bg-pink-50 text-pink-600 flex items-center justify-center group-hover:bg-pink-600 group-hover:text-white transition-colors">
                <i class="fas fa-heart text-lg"></i>
            </div>
            <div>
                <p class="font-bold text-gray-800 text-sm mb-0.5 group-hover:text-pink-700 transition-colors">Duyệt Lời chúc</p>
                <p class="text-xs text-gray-500">Ẩn/hiện lời chúc TNV</p>
            </div>
        </a>

    </div>
```

Tệp patch sẵn sàng đã được tạo tại:
`proposed_admin_navigation.patch`

### Đề xuất 3: Bảng kiểm tra đối chiếu kỹ thuật cho Worker triển khai (M2)

| Tiêu chí | File đích | Yêu cầu kiểm thử E2E | Trạng thái dự kiến sau M2 |
|---|---|---|---|
| Menu link Admin | `_AdminLayout.cshtml` | Chứa `"AdminSanPham"`, `"CuaHang"`, `"Sản phẩm Gây Quỹ"`, `"fa-box"` | **PASS 100%** (F12_T01 đến F12_T05, S07) |
| Bảng danh sách | `Areas/CuaHang/Views/AdminSanPham/Index.cshtml` | `<img`, `LinkHinhAnh`, `TenSanPham`, `GiaBan`, `SoLuongTon`, `Create`, `Edit`, `Delete`, `confirm` | **PASS 100%** (F13_T01 đến F13_T05, S08, S11, F16_T03) |
| Form thêm mới | `Areas/CuaHang/Views/AdminSanPham/Create.cshtml` | `enctype="multipart/form-data"`, `PhanTramDongGop`, `type="submit"` | **PASS 100%** (F14_T01 đến F14_T05, S09) |
| Form chỉnh sửa | `Areas/CuaHang/Views/AdminSanPham/Edit.cshtml` | `asp-for="IDSanPham"`, `type="hidden"`, `enctype="multipart/form-data"`, `<img` | **PASS 100%** (F15_T01 đến F15_T05, S10) |
| Thao tác Xóa | `Areas/CuaHang/Controllers/AdminSanPhamController.cs` | Duy nhất 1 phương thức public `Delete(int id)`, `[HttpPost]`, `[ValidateAntiForgeryToken]` | **PASS 100%** (F16_T01, F16_T02) |

---

## 5. Verification Method (Phương pháp kiểm chứng độc lập)

Để kiểm chứng tính chính xác của các phân tích và đặc tả kỹ thuật trên:

1. **Kiểm tra tệp và mã nguồn bằng công cụ xem tệp**:
   ```bash
   grep -n "AdminSanPham" WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml
   grep -n "Sản phẩm Gây Quỹ" WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml
   ```
2. **Kiểm tra biên dịch dự án**:
   ```bash
   export XDG_RUNTIME_DIR=/tmp
   dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
3. **Chạy bộ kiểm thử E2E độc lập Tier 1**:
   ```bash
   ./tests/run_e2e_tests.sh --tier 1
   ```
   *Điều kiện hợp lệ*: Khi Worker M2 áp dụng các thay đổi theo đặc tả này cùng với Controller và Views của M2-1, M2-2, toàn bộ 25 bài test của Features 12, 13, 14, 15, 16 sẽ chuyển từ trạng thái `[SKIP]` sang `[PASS] 100%`.
4. **Chạy kịch bản người dùng thực tế Tier 4**:
   ```bash
   ./tests/run_e2e_tests.sh --tier 4
   ```
   *Điều kiện hợp lệ*: Các kịch bản S07, S08, S09, S10, S11, S12 đều chuyển sang `[PASS]`.
