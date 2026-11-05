# Task Dispatch: Worker M2.2 - UI/UX Polish, Admin Views, Toasts & Modals

## Role
Teamwork Worker (`teamwork_worker`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_2`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`
- Frontend Survey Report: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r2_survey_2/handoff.md`

## Mandatory Integrity Warning
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

## Write Ownership (Exclusively Owned Files)
You are permitted to modify or create ONLY the following files:
1. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminDonHang/Index.cshtml` (Create new)
2. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml` (Create new)
3. `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`
4. `WEB_TinhNguyen/Views/Admin/Index.cshtml`
5. `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`
6. `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/Index.cshtml`
7. `WEB_TinhNguyen/Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml`
8. `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/Index.cshtml`
9. `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`
10. `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`

Do NOT modify any backend controller or model files outside this list.

## Implementation Tasks

### 1. Admin Order Management Views
- Create `WEB_TinhNguyen/Areas/CuaHang/Views/AdminDonHang/Index.cshtml`:
  - Consistent layout with `_AdminLayout.cshtml` (Dark navy `#161a2d`, emerald highlights).
  - 4 KPI Stat Cards:
    * Tổng đơn hàng (`ViewBag.TongDonHang`)
    * Đơn chờ xác nhận (`ViewBag.ChoXacNhan`)
    * Doanh thu hàng (`ViewBag.TongDoanhThu`)
    * Quỹ tình nguyện đóng góp (`ViewBag.TongTienQuy`)
  - Filter and Search Toolbar:
    * Search input by donor name, phone, order code.
    * Dropdown/tabs for status: Tất cả, Mới đặt (`MoiDat`), Đã xác nhận (`DaXacNhan`), Đang giao (`DangGiao`), Hoàn thành (`HoanThanh`), Đã hủy (`DaHuy`).
    * Dropdown for payment status: Chưa thanh toán (`ChuaThanhToan`), Đã thanh toán (`DaThanhToan`).
  - Table of Orders:
    * Columns: Mã đơn, Thời gian, Người đặt & SĐT, Phương thức, Tổng tiền (kèm badge tiền ủng hộ thêm), Trạng thái đơn (badge màu sắc ngữ nghĩa rõ ràng: Vàng cho MoiDat, Xanh dương cho DaXacNhan, Tím cho DangGiao, Xanh lá cho HoanThanh, Đỏ cho DaHuy), Trạng thái TT, Thao tác (Xem chi tiết, Nút đổi nhanh trạng thái, Xóa).
  - Empty state view if no orders found.
- Create `WEB_TinhNguyen/Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml`:
  - Detailed donor information (Họ tên, SĐT, Email, Địa chỉ giao hàng, Ghi chú).
  - Order details table (Ảnh, Tên sản phẩm, Đơn giá, Số lượng, Thành tiền, % Gây quỹ, Tiền trích quỹ).
  - Summary section: Tiền hàng, Tiền ủng hộ thêm, Tổng thanh toán, Tổng tiền trích vào Quỹ tình nguyện.
  - VietQR transfer info box (nếu thanh toán ChuyenKhoanQR): QR image, STK MBBank 0924044283, Nội dung chuyển khoản, nút xác nhận đã nhận tiền.
  - Status transition form: Dropdown chọn trạng thái (`MoiDat`, `DaXacNhan`, `DangGiao`, `HoanThanh`, `DaHuy`) và trạng thái thanh toán (`ChuaThanhToan`, `DaThanhToan`) kèm nút "Cập nhật trạng thái".
  - Nút quay lại danh sách.

### 2. Admin Layout & Dashboard Navigation Links
- In `WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`:
  - Add link "Đơn hàng Gây Quỹ" under the sidebar (adjacent to "Sản phẩm Gây Quỹ"):
    `href="/CuaHang/AdminDonHang"` with icon `<i class="fas fa-receipt"></i>` or `<i class="fas fa-clipboard-list"></i>` and active state checking `ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminDonHang"`.
- In `WEB_TinhNguyen/Views/Admin/Index.cshtml`:
  - Add quick action card / KPI summary for "Đơn hàng Gây Quỹ" with direct link to `/CuaHang/AdminDonHang`.

### 3. Storefront Navigation & Floating Cart Button
- In `WEB_TinhNguyen/Views/Shared/_Layout.cshtml`:
  - Include SweetAlert2 CDN: `<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>`.
  - On Desktop Topbar (adjacent to "Cửa hàng Gây quỹ"): Add a Cart Badge link pointing to `/CuaHang/GioHang` with icon `fas fa-shopping-basket` and badge counter `#cartCountBadge` showing total items in session cart `GioHangGayQuy`.
  - Add a responsive Floating Action Cart Button (`#floatingCartBtn`) fixed at the bottom right (`fixed right-5 bottom-24 md:bottom-8 z-40`), linking to `/CuaHang/GioHang`, with icon, counter badge `#floatingCartCount`, bounce animation, accessible on both desktop and mobile.

### 4. Storefront Catalog Polish (`Areas/CuaHang/Views/CuaHang/Index.cshtml` & `ChiTiet.cshtml`)
- In `Index.cshtml`:
  - Integrate Live Search / filter for products (debounced search input that filters product cards instantly or submits smoothly).
  - Add quick price filter pills: Tất cả, Dưới 50k, 50k - 150k, Trên 150k, và 100% Gây quỹ.
  - Update "Thêm vào giỏ" button to use AJAX (`fetch('/CuaHang/GioHang/ThemVaoGio')` with `X-Requested-With: XMLHttpRequest`), prevent page reload, update both `#cartCountBadge` and `#floatingCartCount`, and trigger a SweetAlert2 toast notification ("Đã thêm ... vào giỏ hàng"). Fall back smoothly if JS disabled.
  - For products with `SoLuongTon <= 0`, display badge "Hết hàng" and disable add to cart.
- In `ChiTiet.cshtml`:
  - AJAX add-to-cart with toast notification.
  - Add "Mua ngay" button that adds to cart via AJAX and immediately redirects to `/CuaHang/GioHang`.
  - Display "Hết hàng" if stock is 0.

### 5. Cart Management & Modal Interactions (`Areas/CuaHang/Views/GioHang/Index.cshtml`)
- In `Index.cshtml`:
  - Update quantity buttons (`+` and `-`) to use AJAX (`fetch('/CuaHang/GioHang/CapNhatSoLuong')`), update DOM totals (tiền hàng, tổng thanh toán, tiền quỹ) without full page reload.
  - Replace native browser `confirm()` with a modern Tailwind Confirmation Modal for clearing the cart and deleting individual items.
  - Add submit-state spinner to the "Xác nhận đặt hàng" button (`onclick: this.disabled = true; this.form.submit();`) to prevent duplicate order submissions.

### 6. Order Success VietQR Polish (`Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`)
- In `DatHangThanhCong.cshtml`:
  - Add 1-click "Sao chép" button for Account Number (0924044283) and Order Code (Nội dung CK) with SweetAlert2 toast feedback ("Đã sao chép vào bộ nhớ tạm").
  - Add visual 4-step progress tracker:
    (1) Đặt hàng thành công ➔ (2) Chờ xác nhận chuyển khoản ➔ (3) Đang đóng gói & chuẩn bị ➔ (4) Giao hàng tình nguyện.
  - Add print/download button (`window.print()`) for donation receipt.

### 7. Admin SanPham Modal Delete (`Areas/CuaHang/Views/AdminSanPham/Index.cshtml`)
- Replace browser native `confirm()` with modern interactive Tailwind confirmation modal before deleting a product.

## Verification Commands Required
Before delivering handoff, you MUST run:
1. Solution build:
   `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
   Must pass with 0 errors, 0 warnings.
2. Existing E2E test suite regression:
   `bash tests/run_e2e_tests.sh --all`
   Must pass 280/280 tests with 0 regressions.

Write your report to `handoff.md` in your working directory.
