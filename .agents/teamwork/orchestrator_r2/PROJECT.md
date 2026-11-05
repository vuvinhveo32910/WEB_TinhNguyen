# Project: HOÀN THIỆN TOÀN DIỆN HỆ THỐNG MUA BÁN SẢN PHẨM GÂY QUỸ TÌNH NGUYỆN (ROUND 2)

## Architecture
- **Framework & Runtime**: ASP.NET Core MVC trên .NET 10.0 (`net10.0`), SDK 10.0.114.
- **ORM & Data Access**: Entity Framework Core 10.0.12, mở rộng `ApplicationDbContext` với Dual-mode persistence: kết nối SQL Server khi khả dụng và tự động fallback sang thread-safe in-memory store `DbInitializer.cs`.
- **Area Structure**: Toàn bộ tính năng thuộc ASP.NET Core Area `Areas/CuaHang`.
- **State & Communication**:
  - Session cart management với `AddDistributedMemoryCache()` và `AddSession()`.
  - AJAX JSON responses (`IsAjaxRequest()`) kết hợp SweetAlert2 / Toast notifications và Modern Interactive Tailwind Modals.
- **Admin System**:
  - Giao diện Admin đồng bộ layout `_AdminLayout.cshtml` (Dark Navy `#161a2d`, Emerald highlights `#4caf50`).
  - Quản lý sản phẩm (`AdminSanPhamController`) và phân hệ mới Quản lý đơn hàng (`AdminDonHangController`).

## Code Layout
```
WEB_TinhNguyen/
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml                    # Bổ sung Cart Badge Header & Floating Cart Button
│   │   └── _AdminLayout.cshtml               # Bổ sung menu "Đơn hàng Gây Quỹ"
│   └── Admin/
│       └── Index.cshtml                      # Bổ sung Quick tile & KPI đơn hàng gây quỹ
└── Areas/
    └── CuaHang/
        ├── Controllers/
        │   ├── CuaHangController.cs          # Catalog, Live search, Price filter
        │   ├── GioHangController.cs          # AJAX cart, pre-checkout stock validation
        │   ├── AdminSanPhamController.cs     # CRUD sản phẩm, modal delete
        │   └── AdminDonHangController.cs     # MỚI: Quản lý đơn hàng, đổi trạng thái, thống kê quỹ
        ├── Helpers/
        │   └── DbInitializer.cs              # Mở rộng in-memory orders & restock khi hủy đơn
        └── Views/
            ├── CuaHang/
            │   ├── Index.cshtml              # Live search, bộ lọc giá, AJAX add-to-cart, toast
            │   └── ChiTiet.cshtml            # AJAX add-to-cart, toast, nút mua ngay
            ├── GioHang/
            │   ├── Index.cshtml              # AJAX quantity (+/-), modal confirm delete, anti-double-click
            │   └── DatHangThanhCong.cshtml   # 1-click copy STK & mã đơn, visual 4-step tracker
            ├── AdminSanPham/
            │   └── Index.cshtml              # Modal confirm delete
            └── AdminDonHang/
                ├── Index.cshtml              # MỚI: Danh sách đơn hàng, lọc trạng thái, KPI quỹ
                └── ChiTiet.cshtml            # MỚI: Chi tiết đơn hàng, duyệt trạng thái, VietQR

tests/E2ETests/
├── Framework/
│   └── TestRunner.cs                         # Hiển thị t <= 5 trong summary
├── Tier1/
│   └── Tier1_AdminOrderTests.cs              # MỚI: 15 unit/contract tests cho AdminDonHang
├── Tier2/
│   └── Tier2_BoundaryCornerTests.cs         # Mở rộng search unicode, empty reset, out-of-stock
├── Tier3/
│   └── Tier3_CrossFeatureTests.cs            # Mở rộng inventory deduction & DaBan increment
└── Tier5/
    └── Tier5_ChallengerM4OrderHarness.cs     # MỚI: Adversarial concurrency & restock tests
```

## Feature Inventory
| # | Feature | Description | Milestone | Source |
|---|---------|-------------|-----------|--------|
| 1 | In-Memory Order Storage Parity | Bổ sung GetInMemoryOrders, UpdateInMemoryOrderStatus, DeleteInMemoryOrder, restock logic | M2.1 | Survey 1 |
| 2 | Pre-checkout Inventory Check | Kiểm tra tồn kho trước khi tạo đơn, ngăn chặn oversell, fix bug CapNhatSoLuong khi stock <= 0 | M2.1 | Survey 1 |
| 3 | AdminDonHangController Backend | Tạo controller AdminDonHang với Index, ChiTiet, CapNhatTrangThai, XoaDonHang, thống kê quỹ thiện nguyện | M2.1 | Survey 1, R2 |
| 4 | Order Status Lifecycle & Restock | Hỗ trợ các trạng thái: MoiDat, DaXacNhan, DangGiao, HoanThanh, DaHuy; tự động hoàn trả tồn kho khi hủy đơn | M2.1 | Survey 1 |
| 5 | Admin Order Views | Tạo Views/AdminDonHang/Index.cshtml và ChiTiet.cshtml với giao diện Admin Navy/Emerald | M2.2 | Survey 2, R2 |
| 6 | Admin Layout & Dashboard Navigation | Bổ sung link Đơn hàng Gây Quỹ vào _AdminLayout.cshtml và thẻ thống kê trên Views/Admin/Index.cshtml | M2.2 | Survey 2, R2 |
| 7 | Storefront Cart Badge & Floating Cart | Thêm Cart Counter Badge trên Header Desktop và Floating Action Cart Button cố định trên Mobile & Desktop | M2.2 | Survey 2, R1 |
| 8 | AJAX Cart & Toast Notifications | Tích hợp SweetAlert2/Toast thông báo khi thêm giỏ, cập nhật số lượng không reload trang | M2.2 | Survey 2, R1 |
| 9 | Modern Tailwind Confirmation Modals | Thay thế confirm() trình duyệt bằng Tailwind Modal cho xóa giỏ hàng và xóa sản phẩm/đơn hàng | M2.2 | Survey 2, R1 |
| 10 | Storefront Live Search & Price Filter | Thêm tìm kiếm thời gian thực, lọc theo khoảng giá (<50k, 50k-150k, >150k) và lọc 100% gây quỹ | M2.2 | Survey 2, R1 |
| 11 | VietQR 1-Click Copy & Visual Step Tracker | Nút copy STK và Mã đơn 1-click kèm tooltip, thanh tiến trình trực quan 4 bước | M2.2 | Survey 2, R1 |
| 12 | E2E Test Suite Expansion | Mở rộng 30+ ca kiểm thử cho AdminDonHang, search, filters, modals, toasts, stock logic | M2.3 | Survey 3 |
| 13 | TestRunner Tier 5 Summary Fix | Cập nhật loop summary lên t <= 5 để hiển thị đầy đủ kết quả | M2.3 | Survey 3 |
| 14 | Solution Build & Integrity Verification | Biên dịch 0 lỗi, 100% tests pass (310+ tests), xác minh độc lập Reviewer, Challenger, Auditor | M2.3 | AC38, Audit |

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| M2.1 | Backend Order Logic & Inventory Parity | AdminDonHangController, DbInitializer methods, pre-checkout stock validation, restock logic | none | DONE |
| M2.2 | UI/UX Polish, Admin Views, Toasts & Modals | AdminDonHang Views, Sidebar navigation, Header cart badge, Floating cart, AJAX cart, Toasts, Modals, Filters, VietQR copy & tracking | M2.1 | DONE |
| M2.3 | Test Expansion, Adversarial Verification & Gate | Thêm 36 tests (tổng 316 tests), Tier 1-5 validation, Reviewers, Challengers, Forensic Auditor verification | M2.1, M2.2 | DONE |

## Interface Contracts
### AdminDonHangController
- `Index(string? q, string? trangThai, string? thanhToan, string? sort)` -> Returns View(List<DonHangGayQuy>) kèm ViewBag thống kê: `TongDonHang`, `ChoXacNhan`, `TongDoanhThu`, `TongTienQuy`.
- `ChiTiet(int id)` -> Returns View(DonHangGayQuy) với đầy đủ ChiTietDonHangs, thông tin người mua, VietQR info.
- `CapNhatTrangThai(int id, string trangThaiDonHang, string? trangThaiThanhToan)` -> Updates DB / In-memory, hoàn trả kho nếu DaHuy, sets TempData, redirects.
- `XoaDonHang(int id)` -> Safe removal, redirects.

### DbInitializer Extensions
- `List<DonHangGayQuy> GetInMemoryOrders()`
- `bool UpdateInMemoryOrderStatus(int id, string trangThaiDonHang, string? trangThaiThanhToan)`
- `bool DeleteInMemoryOrder(int id)`
- `bool RestockCancelledOrder(DonHangGayQuy order)`

### AJAX Cart Protocol
- Endpoint: POST `/CuaHang/GioHang/ThemVaoGio` (id, soLuong)
- Request Header: `X-Requested-With: XMLHttpRequest`
- JSON Response: `{ success: bool, message: string, totalQuantity: int, subTotal: decimal, totalAmount: decimal }`
