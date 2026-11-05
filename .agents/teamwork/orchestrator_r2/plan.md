# Execution Plan - Round 2 Enhancement

## Objectives
Hoàn thiện toàn bộ các tính năng còn thiếu, chưa hoàn chỉnh hoặc có thể nâng cấp trong hệ thống Mua bán sản phẩm gây quỹ tình nguyện (Area `CuaHang`), bao gồm:
1. Quản lý Đơn hàng & Quyên góp trong Admin (`AdminDonHangController` hoặc mở rộng Admin để xem danh sách đơn hàng, xem chi tiết đơn, cập nhật trạng thái đơn hàng: Đang xử lý, Đã thanh toán / Đã giao, Hủy đơn).
2. Tinh chỉnh nâng cấp UI/UX: Toast thông báo (thêm giỏ hàng, đặt hàng), Modal tương tác trực quan, thanh tìm kiếm sản phẩm theo tên/danh mục/giá, bộ lọc trực tiếp mượt mà.
3. Độ bền vững (Robustness): Xử lý ngoại lệ, validation chặt chẽ dữ liệu đầu vào, stock inventory check khi đặt hàng, hiển thị thông báo lỗi rõ ràng.
4. Tối ưu seed data và responsive trên mọi thiết bị.
5. Kiểm thử toàn diện: Mở rộng bộ test E2E để kiểm tra cả tính năng mới và tính năng hiện hữu, đảm bảo 100% pass và build không lỗi.

## Phase Breakdown
- **Phase 0: Survey**: Dispatch 3 Explorers:
  - Explorer 1: Khảo sát hiện trạng code `Areas/CuaHang` (Controllers, Views, Models, DbContext, Helpers, Session).
  - Explorer 2: Khảo sát `_Layout.cshtml`, `_AdminLayout.cshtml`, `Views/Admin/Index.cshtml`, giao diện người dùng và Admin, UI/UX, responsive, toast/modal.
  - Explorer 3: Khảo sát bộ test hiện có (`tests/E2ETests/`, các harness test), cấu hình build, database fallback, các ca kiểm thử.
- **Phase 1: Project Plan & Decomposition**: Lập `PROJECT.md`, phân tách các milestone chi tiết.
- **Phase 2: Execution**: Dispatch Worker & Reviewer & Challenger & Auditor per milestone.
- **Phase 3: Final Verification & Delivery**: Kiểm tra 100% test pass, lập `handoff.md`, thông báo Sentinel.
