# BÁO CÁO NGHIÊN CỨU & THIẾT KẾ MODULE GIỎ HÀNG & THANH TOÁN (M3.2)
**Agent**: Explorer M3.2 (`teamwork_explorer_m3_2`)  
**Target Milestone**: Milestone 3 - Storefront, Cart & Checkout  
**Working Directory**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m3_2`  
**Parent Orchestrator ID**: `108b9d81-1dd6-495f-84eb-99dbb197aebe`

---

## 1. Observation

### 1.1 Hiện trạng mã nguồn và thư mục
- **Area CuaHang**: Đã có nền tảng hoàn thiện từ M1 và M2 tại thư mục `WEB_TinhNguyen/Areas/CuaHang/`:
  - `Models/`: `CartItem.cs`, `CartViewModel.cs`, `CheckoutViewModel.cs`, `DonHangGayQuy.cs`, `ChiTietDonHangGayQuy.cs`, `SanPhamGayQuy.cs`, `SanPhamCrudViewModel.cs`.
  - `Helpers/`: `SessionExtensions.cs` (cung cấp `Get<T>`, `Set<T>`, `GetOrCreate<T>`, `RemoveKey`), `DbInitializer.cs` (cung cấp 4 sản phẩm mẫu, In-Memory Store đầy đủ CRUD sản phẩm và đơn hàng).
  - `Controllers/`: `AdminSanPhamController.cs` (mẫu kiến trúc dual-mode Database / In-Memory fallback).
  - `Views/`: `_ViewImports.cshtml` (đã nạp TagHelpers, Models, Helpers), `_ViewStart.cshtml` (tự động phân luồng: nếu controller không bắt đầu bằng "Admin" thì dùng `_Layout.cshtml`).
- **Thư mục mục tiêu chưa tồn tại**:
  - `WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs` (chưa có)
  - `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/Index.cshtml` (chưa có)
  - `WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml` (chưa có)

### 1.2 Kết quả thực thi bộ kiểm thử E2E hiện tại
Khi chạy bộ kiểm thử độc lập qua `./run_e2e_tests.sh`:
- **Tier 1 - Feature 9 (Cart Management)**:
  - `F09_T01`: [PASS] (20.8ms) - `CartItem.ThanhTien` = `DonGia * SoLuong`.
  - `F09_T02`: [PASS] (0.4ms) - `CartViewModel` có đủ `Items`, `TongSoLuong`, `TongTienHang`, `TongThanhToan`.
  - `F09_T03`: [SKIP] - Yêu cầu tệp `Areas/CuaHang/Views/GioHang/Index.cshtml` tồn tại và chứa `"Items"`, `"TongThanhToan"`.
  - `F09_T04`: [SKIP] - Yêu cầu `GioHang/Index.cshtml` chứa hành động cập nhật số lượng (`CapNhatGio` hoặc `update` hoặc `soLuong`) và nút xóa (`XoaKhoiGio` hoặc `delete` hoặc `remove`).
  - `F09_T05`: [SKIP] - Yêu cầu `GioHang/Index.cshtml` xử lý giỏ hàng trống (`trống` hoặc `chưa có sản phẩm` hoặc `!Model.Items.Any()`).
- **Tier 1 - Feature 10 (Checkout & Donation Form)**:
  - `F10_T01`: [PASS] (20.9ms) - `CheckoutViewModel` chứa đủ `HoTen`, `SoDienThoai`, `DiaChi`, `Email`, `TienUngHoThem`, `PhuongThucThanhToan`.
  - `F10_T02`: [PASS] (22.1ms) - Oracle kiểm tra số điện thoại di động Việt Nam 10 chữ số (đầu số 03, 05, 07, 08, 09).
  - `F10_T03`: [PASS] (1.0ms) - Phép tính tiền ủng hộ thêm: `OrderTotal(subtotal, extraDonation, 0m)`.
  - `F10_T04`: [SKIP] - Yêu cầu `GioHang/Index.cshtml` chứa tùy chọn thanh toán VietQR (`QR` hoặc `ChuyenKhoan` hoặc `Chuyển khoản`) và COD (`COD` hoặc `tiền mặt` hoặc `nhận hàng`).
  - `F10_T05`: [SKIP] - Yêu cầu biểu mẫu đặt hàng có nút submit (`type="submit"` hoặc `type='submit'`).
- **Tier 1 - Feature 11 (Order Confirmation & VietQR)**:
  - `F11_T01`: [SKIP] - Yêu cầu tệp `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml` tồn tại.
  - `F11_T02`: [SKIP] - Yêu cầu `DatHangThanhCong.cshtml` hiển thị `"MaDonHang"`, `"HoTenNguoiMua"`, `"TongThanhToan"`.
  - `F11_T03`: [PASS] (11.2ms) - URL generator VietQR tuân thủ chuẩn `img.vietqr.io`.
  - `F11_T04`: [SKIP] - Yêu cầu `DatHangThanhCong.cshtml` nhúng VietQR (`vietqr` hoặc `QR` hoặc `Quét mã`).
  - `F11_T05`: [SKIP] - Yêu cầu `DatHangThanhCong.cshtml` có liên kết quay lại cửa hàng (`CuaHang` hoặc `tiếp tục mua sắm` hoặc `Trang chủ`).
- **Tier 2 (Boundary & Corner Cases)**: 25/25 [PASS] (0.05s).
- **Tier 3 (Cross-Feature Combinations)**: 15/16 [PASS] (1 skipped là navigation menu).
  - `C04`: Mã đơn hàng bắt đầu bằng `"GQ"`, chứa chuỗi ngày `"yyyyMMdd"`, độ dài chính xác 15 ký tự (ví dụ: `GQ20261103-XXXX`).
  - `C05`: Mã đơn hàng được nhúng nguyên vẹn vào tham số `addInfo` của URL VietQR.
  - `C07`: Tồn kho giảm theo số lượng đặt mua và số lượng đã bán tăng tương ứng.
  - `C13`: Hai session khác nhau có giỏ hàng cô lập độc lập.
- **Tier 4 (Real-World Scenarios)**:
  - `S03`: [SKIP] - Kiểm tra thêm 2 áo và xem giỏ hàng tại `GioHang/Index.cshtml`.
  - `S04`: [PASS] - Điền thông tin người ủng hộ hợp lệ.
  - `S05`: [PASS] - Ủng hộ thêm 100.000 VNĐ và chọn VietQR `ChuyenKhoanQR`.
  - `S06`: [SKIP] - Trang xác nhận `DatHangThanhCong.cshtml` với mã VietQR.
- **Tier 5 (Adversarial Challenger)**: 108/108 [PASS] (2.48s).

---

## 2. Logic Chain

1. **State Management trong Giỏ hàng**:
   - Sử dụng Session key chuẩn: `"GioHangGayQuy"`.
   - Lưu trữ kiểu `List<CartItem>` bằng `SessionExtensions.Get<List<CartItem>>(key)` và `SessionExtensions.Set(key, cart)`.
   - Mỗi item bao gồm: `IDSanPham`, `TenSanPham`, `LinkHinhAnh`, `DonGia`, `SoLuong`, `ThongDiepGayQuy`, `PhanTramDongGop`, `SoLuongTon`.
   - Tính toán dòng tiền: `ThanhTien = DonGia * SoLuong`, `TienDongGop = ThanhTien * PhanTramDongGop / 100`.

2. **Luồng thao tác Giỏ hàng (Cart Lifecycle)**:
   - `ThemVaoGio(int id, int soLuong = 1)`: Hỗ trợ cả `[HttpPost]` (từ form chi tiết sản phẩm) và `[HttpGet]` (từ link danh mục cửa hàng).
     - Kiểm tra sản phẩm tồn tại và đang hoạt động (`TrangThai == true`).
     - Kiểm tra tồn kho: nếu `SoLuongTon <= 0` thông báo hết hàng.
     - Nếu sản phẩm đã có trong giỏ, tăng số lượng nhưng không vượt quá `SoLuongTon`.
     - Hỗ trợ phản hồi linh hoạt: nếu là AJAX request (kiểm tra `X-Requested-With` hoặc `Accept: application/json`), trả về JSON `{ success, message, totalQuantity, cartSubtotal }`; nếu request thông thường, lưu `TempData["SuccessMessage"]` và chuyển hướng tới `Index`.
   - `CapNhatSoLuong(int id, int soLuong)`:
     - Nếu `soLuong <= 0`: xóa sản phẩm khỏi giỏ.
     - Nếu `soLuong > 0`: gán lại số lượng (giới hạn tối đa bằng `SoLuongTon`).
     - Hỗ trợ trả về JSON cho tương tác mượt mà hoặc Redirect về `Index`.
   - `XoaKhoiGio(int id)`: Xóa item tương ứng khỏi giỏ trong Session.
   - `XoaGioHang()`: Xóa sạch toàn bộ giỏ hàng (`Session.RemoveKey("GioHangGayQuy")`).

3. **Luồng Đặt hàng & Ủng hộ gây quỹ (Checkout Lifecycle)**:
   - `DatHang(CheckoutViewModel model)` [HttpPost, ValidateAntiForgeryToken]:
     - Đọc giỏ hàng từ Session. Nếu rỗng, báo lỗi và chuyển hướng về giỏ hàng.
     - Xác thực các trường người nhận: `HoTen` (bắt buộc, <= 255 ký tự), `SoDienThoai` (chuẩn di động VN 10 số theo Regex `^(03|05|07|08|09)\d{8}$`), `Email` (định dạng email hợp lệ nếu có), `DiaChi` (bắt buộc, <= 500 ký tự).
     - Nếu `!ModelState.IsValid`: nạp lại `cart` vào `model.Items`, đóng gói `CartViewModel` và `ViewBag.CheckoutModel = model`, hiển thị lại trang `Index` với thông báo lỗi validation.
     - Sinh mã đơn hàng chuẩn 15 ký tự: `var orderCode = $"GQ{DateTime.Now:yyyyMMdd}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();`.
     - Tính tổng tiền:
       `TongTienHang = cart.Sum(i => i.ThanhTien);`
       `TienUngHoThem = model.TienUngHoThem >= 0 ? model.TienUngHoThem : 0m;`
       `PhiVanChuyen = 0m;`
       `TongThanhToan = TongTienHang + TienUngHoThem + PhiVanChuyen;`
     - Tạo thực thể `DonHangGayQuy` và các dòng `ChiTietDonHangGayQuy`.
     - Lưu trữ Dual-Mode: Thử lưu qua `_context.DonHangGayQuys` và trừ tồn kho `SanPhamGayQuy`. Nếu database không kết nối được hoặc ném lỗi ngoại lệ, chuyển sang gọi `DbInitializer.AddInMemoryOrder(donHang)`.
     - Xóa Session giỏ hàng (`Session.RemoveKey("GioHangGayQuy")`).
     - Chuyển hướng sang `DatHangThanhCong(int id)`.

4. **Trang Xác nhận & VietQR (Order Success & Payment)**:
   - `DatHangThanhCong(int id)`:
     - Truy vấn đơn hàng theo `id` (database hoặc `DbInitializer.GetInMemoryOrderById(id)`).
     - Truyền `DonHangGayQuy` vào view.
     - Trong View:
       - Hiển thị mã đơn hàng `MaDonHang` nổi bật.
       - Hiển thị thông tin người mua `HoTenNguoiMua`, số điện thoại, địa chỉ nhận hàng, thời gian đặt.
       - Hiển thị tổng thanh toán `TongThanhToan`, tiền ủng hộ thêm `TienUngHoThem`.
       - Hiển thị bảng chi tiết các sản phẩm và số tiền đóng góp vào quỹ.
       - Nếu phương thức là `ChuyenKhoanQR` (hoặc `!= "COD"`):
         - Tạo mã VietQR động theo chuẩn MBBank của ban tổ chức:
           `https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}&accountName=NGUYEN%20THAI%20VINH`
         - Hộp thông tin chuyển khoản ngân hàng: MBBank - STK `0924044283` - Tên `NGUYEN THAI VINH` - Nội dung `MaDonHang`.
       - Nếu phương thức là `COD`: thông báo giao hàng và thu tiền mặt tận nơi.
       - Nút điều hướng quay lại Cửa hàng (`/CuaHang`) và Trang chủ (`/`).

---

## 3. Caveats

1. **Khả năng ngoại lệ khi Database offline trong môi trường sandbox**:
   - Máy chủ SQL Server có thể không phản hồi trong sandbox; toàn bộ logic truy vấn và ghi đơn hàng phải được bọc trong khối `try/catch` có fallback an toàn sang `DbInitializer.GetInMemoryProductById`, `DbInitializer.AddInMemoryOrder`, `DbInitializer.GetInMemoryOrderById`.
2. **Độ dài và định dạng mã đơn hàng**:
   - Bài test `Tier3_CrossFeatureTests.cs` (C04) yêu cầu chính xác độ dài 15 ký tự (`Assert.Equal(15, orderCode.Length)`), bắt đầu bằng `"GQ"`, có chuỗi ngày `"yyyyMMdd"`. Cần sử dụng công thức: `$"GQ{DateTime.Now:yyyyMMdd}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()`.
3. **Từ khóa bắt buộc trong Razor Views**:
   - `Index.cshtml` PHẢI chứa: `"Items"`, `"TongThanhToan"`, (`"CapNhatGio"` hoặc `"update"` hoặc `"soLuong"`), (`"XoaKhoiGio"` hoặc `"delete"` hoặc `"remove"`), (`"trống"` hoặc `"chưa có sản phẩm"` hoặc `"!Model.Items.Any()"`), (`"QR"` hoặc `"ChuyenKhoan"` hoặc `"Chuyển khoản"`), (`"COD"` hoặc `"tiền mặt"` hoặc `"nhận hàng"`), (`type="submit"`).
   - `DatHangThanhCong.cshtml` PHẢI chứa: `"MaDonHang"`, `"HoTenNguoiMua"`, `"TongThanhToan"`, (`"vietqr"` hoặc `"QR"` hoặc `"Quét mã"`), (`"CuaHang"` hoặc `"tiếp tục mua sắm"` hoặc `"Trang chủ"`).

---

## 4. Conclusion (Concrete Implementation Blueprints)

### 4.1 `Areas/CuaHang/Controllers/GioHangController.cs`
Đường dẫn tệp: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Controllers/GioHangController.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Data;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

namespace WEB_TinhNguyen.Areas.CuaHang.Controllers
{
    /// <summary>
    /// Phân hệ Giỏ hàng & Thanh toán Quyên góp Gây quỹ Tình nguyện
    /// Quản lý giỏ hàng lưu Session, xử lý thêm/sửa/xóa sản phẩm, biểu mẫu đặt hàng,
    /// tự động tạo đơn hàng và hiển thị mã VietQR chuyển khoản.
    /// </summary>
    [Area("CuaHang")]
    public class GioHangController : Controller
    {
        private const string CartSessionKey = "GioHangGayQuy";
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Helper Methods

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart);
        }

        private async Task<SanPhamGayQuy?> FindProductAsync(int id)
        {
            try
            {
                if (_context.Database.CanConnect())
                {
                    return await _context.SanPhamGayQuys.FirstOrDefaultAsync(p => p.IDSanPham == id && p.TrangThai);
                }
            }
            catch
            {
                // Fallback khi database offline
            }

            var inMemory = DbInitializer.GetInMemoryProductById(id);
            return (inMemory != null && inMemory.TrangThai) ? inMemory : null;
        }

        private bool IsAjaxRequest()
        {
            var requestedWith = Request.Headers["X-Requested-With"].ToString();
            var accept = Request.Headers["Accept"].ToString();
            return string.Equals(requestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                   || accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        // =========================================================================
        // 1. XEM GIỎ HÀNG & BIỂU MẪU ĐẶT HÀNG (INDEX)
        // =========================================================================
        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCart();
            var cartVm = new CartViewModel
            {
                Items = cart
            };

            var checkoutVm = new CheckoutViewModel
            {
                Items = cart,
                PhuongThucThanhToan = "ChuyenKhoanQR"
            };

            ViewBag.CheckoutModel = checkoutVm;
            return View(cartVm);
        }

        // =========================================================================
        // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG (THEM VAO GIO)
        // =========================================================================
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> ThemVaoGio(int id, int soLuong = 1)
        {
            if (soLuong <= 0) soLuong = 1;

            var product = await FindProductAsync(id);
            if (product == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Sản phẩm gây quỹ không tồn tại hoặc đã ngừng kinh doanh." });
                }
                TempData["ErrorMessage"] = "Sản phẩm gây quỹ không tồn tại hoặc đã ngừng cung cấp.";
                return RedirectToAction("Index", "CuaHang");
            }

            if (product.SoLuongTon <= 0)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Sản phẩm này hiện đã hết quà tặng/hiện vật trong kho gây quỹ." });
                }
                TempData["ErrorMessage"] = "Sản phẩm này hiện đã hết hàng.";
                return RedirectToAction("ChiTiet", "CuaHang", new { id });
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (existing != null)
            {
                existing.SoLuong += soLuong;
                if (existing.SoLuong > product.SoLuongTon)
                {
                    existing.SoLuong = product.SoLuongTon;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    IDSanPham = product.IDSanPham,
                    TenSanPham = product.TenSanPham,
                    LinkHinhAnh = product.LinkHinhAnh,
                    DonGia = product.GiaBan,
                    SoLuong = Math.Min(soLuong, product.SoLuongTon),
                    ThongDiepGayQuy = product.ThongDiepGayQuy,
                    PhanTramDongGop = product.PhanTramDongGop,
                    SoLuongTon = product.SoLuongTon
                });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"Đã thêm \"{product.TenSanPham}\" vào giỏ quà gây quỹ thành công!";

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = $"Đã thêm {product.TenSanPham} vào giỏ hàng!",
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 3. CẬP NHẬT SỐ LƯỢNG SẢN PHẨM (CAP NHAT SO LUONG)
        // =========================================================================
        [HttpPost]
        public IActionResult CapNhatSoLuong(int id, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (item != null)
            {
                if (soLuong <= 0)
                {
                    cart.Remove(item);
                    TempData["SuccessMessage"] = $"Đã xóa sản phẩm \"{item.TenSanPham}\" khỏi giỏ hàng.";
                }
                else
                {
                    item.SoLuong = item.SoLuongTon > 0 ? Math.Min(soLuong, item.SoLuongTon) : soLuong;
                    TempData["SuccessMessage"] = $"Đã cập nhật số lượng cho \"{item.TenSanPham}\".";
                }
                SaveCart(cart);
            }

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    lineTotal = item?.ThanhTien ?? 0,
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien),
                    totalDonation = cart.Sum(i => i.TienDongGop)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 4. XÓA MỘT SẢN PHẨM KHỎI GIỎ (XOA KHOI GIO)
        // =========================================================================
        [AcceptVerbs("GET", "POST")]
        public IActionResult XoaKhoiGio(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.IDSanPham == id);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
                TempData["SuccessMessage"] = $"Đã xóa sản phẩm \"{item.TenSanPham}\" khỏi giỏ hàng.";
            }

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    totalQuantity = cart.Sum(i => i.SoLuong),
                    cartSubtotal = cart.Sum(i => i.ThanhTien)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 5. XÓA TOÀN BỘ GIỎ HÀNG (XOA GIO HANG)
        // =========================================================================
        [HttpPost]
        public IActionResult XoaGioHang()
        {
            HttpContext.Session.RemoveKey(CartSessionKey);
            TempData["SuccessMessage"] = "Đã làm trống giỏ hàng gây quỹ.";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 6. XỬ LÝ ĐẶT HÀNG & ỦNG HỘ GÂY QUỸ (DAT HANG)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng chọn sản phẩm gây quỹ trước khi thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            // Xác thực bổ sung số điện thoại di động Việt Nam chuẩn
            if (!string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                var cleanPhone = model.SoDienThoai.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");
                if (!Regex.IsMatch(cleanPhone, @"^(03|05|07|08|09)\d{8}$"))
                {
                    ModelState.AddModelError(nameof(model.SoDienThoai), "Số điện thoại không đúng định dạng di động 10 số của Việt Nam (đầu 03, 05, 07, 08, 09).");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Items = cart;
                var cartVm = new CartViewModel
                {
                    Items = cart,
                    TienUngHoThem = model.TienUngHoThem
                };
                ViewBag.CheckoutModel = model;
                return View("Index", cartVm);
            }

            // Sinh mã đơn hàng chuẩn 15 ký tự dạng GQ2026MMDD-XXXX
            var dateStamp = DateTime.Now.ToString("yyyyMMdd");
            var orderCode = $"GQ{dateStamp}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            decimal tongTienHang = cart.Sum(i => i.ThanhTien);
            decimal tienUngHoThem = model.TienUngHoThem > 0 ? model.TienUngHoThem : 0m;
            decimal phiVanChuyen = 0m;
            decimal tongThanhToan = tongTienHang + tienUngHoThem + phiVanChuyen;

            var donHang = new DonHangGayQuy
            {
                MaDonHang = orderCode,
                HoTenNguoiMua = model.HoTen.Trim(),
                SoDienThoai = model.SoDienThoai.Trim().Replace(" ", "").Replace(".", "").Replace("-", ""),
                Email = model.Email?.Trim(),
                DiaChiNhanHang = model.DiaChi.Trim(),
                GhiChu = model.GhiChu?.Trim(),
                TongTienHang = tongTienHang,
                TienUngHoThem = tienUngHoThem,
                PhiVanChuyen = phiVanChuyen,
                TongThanhToan = tongThanhToan,
                PhuongThucThanhToan = string.IsNullOrWhiteSpace(model.PhuongThucThanhToan) ? "ChuyenKhoanQR" : model.PhuongThucThanhToan,
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan",
                NgayDatHang = DateTime.Now
            };

            foreach (var item in cart)
            {
                donHang.ChiTietDonHangs.Add(new ChiTietDonHangGayQuy
                {
                    IDSanPham = item.IDSanPham > 0 ? item.IDSanPham : null,
                    TenSanPham = item.TenSanPham,
                    LinkHinhAnh = item.LinkHinhAnh,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien,
                    ThongDiepGayQuy = item.ThongDiepGayQuy,
                    PhanTramDongGop = item.PhanTramDongGop
                });
            }

            int orderId = 0;
            try
            {
                if (_context.Database.CanConnect())
                {
                    _context.DonHangGayQuys.Add(donHang);

                    // Trừ tồn kho và tăng số lượng đã bán của sản phẩm trong cơ sở dữ liệu
                    foreach (var detail in donHang.ChiTietDonHangs)
                    {
                        if (detail.IDSanPham.HasValue)
                        {
                            var prod = await _context.SanPhamGayQuys.FindAsync(detail.IDSanPham.Value);
                            if (prod != null)
                            {
                                prod.SoLuongTon = Math.Max(0, prod.SoLuongTon - detail.SoLuong);
                                prod.DaBan += detail.SoLuong;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    orderId = donHang.IDDonHang;
                }
                else
                {
                    var saved = DbInitializer.AddInMemoryOrder(donHang);
                    orderId = saved.IDDonHang;
                }
            }
            catch
            {
                var saved = DbInitializer.AddInMemoryOrder(donHang);
                orderId = saved.IDDonHang;
            }

            // Xóa sạch giỏ hàng trong session
            HttpContext.Session.RemoveKey(CartSessionKey);

            return RedirectToAction(nameof(DatHangThanhCong), new { id = orderId });
        }

        // =========================================================================
        // 7. XÁC NHẬN ĐƠN HÀNG THÀNH CÔNG & VIETQR (DAT HANG THANH CONG)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> DatHangThanhCong(int id)
        {
            DonHangGayQuy? order = null;

            try
            {
                if (_context.Database.CanConnect())
                {
                    order = await _context.DonHangGayQuys
                        .Include(d => d.ChiTietDonHangs)
                        .FirstOrDefaultAsync(d => d.IDDonHang == id);
                }
            }
            catch
            {
                // Fallback khi database offline
            }

            if (order == null)
            {
                order = DbInitializer.GetInMemoryOrderById(id);
            }

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Index", "CuaHang");
            }

            return View(order);
        }
    }
}
```

---

### 4.2 `Areas/CuaHang/Views/GioHang/Index.cshtml`
Đường dẫn tệp: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/Index.cshtml`

```razor
@model WEB_TinhNguyen.Areas.CuaHang.Models.CartViewModel
@{
    ViewData["Title"] = "Giỏ hàng & Đặt hàng Gây quỹ - Trăng Sáng Langbiang 2026";
    var checkout = ViewBag.CheckoutModel as WEB_TinhNguyen.Areas.CuaHang.Models.CheckoutViewModel ?? new WEB_TinhNguyen.Areas.CuaHang.Models.CheckoutViewModel();
}

<div class="min-h-screen bg-slate-50 dark:bg-night py-8 px-4 sm:px-6 lg:px-8 transition-colors duration-200">
    <div class="max-w-7xl mx-auto">
        <!-- Breadcrumb Navigation -->
        <nav class="flex items-center text-sm font-medium text-slate-500 dark:text-slate-400 mb-6 space-x-2">
            <a href="/" class="hover:text-emerald-600 dark:hover:text-emerald-400 transition-colors">
                <i class="fas fa-home mr-1"></i>Trang chủ
            </a>
            <span>/</span>
            <a href="/CuaHang" class="hover:text-emerald-600 dark:hover:text-emerald-400 transition-colors">Cửa hàng Gây quỹ</a>
            <span>/</span>
            <span class="text-emerald-600 dark:text-emerald-400 font-semibold">Giỏ hàng &amp; Đặt mua</span>
        </nav>

        <!-- Tiêu đề trang -->
        <div class="mb-8">
            <h1 class="text-3xl font-extrabold text-slate-900 dark:text-white flex items-center">
                <i class="fas fa-shopping-basket text-emerald-600 dark:text-emerald-400 mr-3"></i>
                Giỏ hàng Sản phẩm Gây quỹ
            </h1>
            <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">
                Mỗi món quà bạn chọn là sự góp sức trao gửi nụ cười và tương lai ấm áp cho 200 em nhỏ tại chân núi Langbiang.
            </p>
        </div>

        @if (TempData["SuccessMessage"] != null)
        {
            <div class="mb-6 p-4 rounded-xl bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-500/30 text-emerald-800 dark:text-emerald-200 flex items-center shadow-sm">
                <i class="fas fa-check-circle text-emerald-600 text-xl mr-3"></i>
                <span>@TempData["SuccessMessage"]</span>
            </div>
        }

        @if (TempData["ErrorMessage"] != null)
        {
            <div class="mb-6 p-4 rounded-xl bg-rose-50 dark:bg-rose-950/40 border border-rose-500/30 text-rose-800 dark:text-rose-200 flex items-center shadow-sm">
                <i class="fas fa-exclamation-circle text-rose-600 text-xl mr-3"></i>
                <span>@TempData["ErrorMessage"]</span>
            </div>
        }

        <!-- KIỂM TRA TRẠNG THÁI GIỎ HÀNG TRỐNG -->
        @if (Model.Items == null || !Model.Items.Any())
        {
            <div class="bg-white dark:bg-night-2 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-12 text-center my-8">
                <div class="w-24 h-24 mx-auto mb-6 rounded-full bg-emerald-50 dark:bg-emerald-950/50 flex items-center justify-center text-emerald-600 dark:text-emerald-400 text-4xl shadow-inner">
                    <i class="fas fa-heart"></i>
                </div>
                <h3 class="text-2xl font-bold text-slate-800 dark:text-slate-100 mb-2">Giỏ hàng gây quỹ của bạn đang trống</h3>
                <p class="text-slate-600 dark:text-slate-400 max-w-md mx-auto mb-8">
                    Hiện bạn chưa có sản phẩm nào trong giỏ hàng. Hãy cùng khám phá những vật phẩm lưu niệm đong đầy yêu thương để ủng hộ chiến dịch bạn nhé!
                </p>
                <a href="/CuaHang" class="inline-flex items-center justify-center px-8 py-3.5 rounded-xl font-bold text-white bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700 shadow-lg shadow-emerald-600/30 transition-all duration-200 transform hover:-translate-y-0.5">
                    <i class="fas fa-store mr-2"></i>Khám phá sản phẩm gây quỹ
                </a>
            </div>
        }
        else
        {
            <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
                <!-- CỘT TRÁI (7 COLS): DANH SÁCH MẶT HÀNG TRONG GIỎ -->
                <div class="lg:col-span-7 space-y-6">
                    <div class="bg-white dark:bg-night-2 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 overflow-hidden">
                        <div class="p-6 border-b border-slate-100 dark:border-slate-800/80 flex items-center justify-between">
                            <h2 class="text-lg font-bold text-slate-800 dark:text-slate-100 flex items-center">
                                <span>Danh sách sản phẩm (@Model.TongSoLuong món)</span>
                            </h2>
                            <form asp-area="CuaHang" asp-controller="GioHang" asp-action="XoaGioHang" method="post" onsubmit="return confirm('Bạn có chắc chắn muốn làm trống toàn bộ giỏ hàng?');">
                                @Html.AntiForgeryToken()
                                <button type="submit" class="text-xs font-semibold text-rose-500 hover:text-rose-700 dark:text-rose-400 dark:hover:text-rose-300 transition-colors">
                                    <i class="fas fa-trash-alt mr-1"></i>Xóa tất cả
                                </button>
                            </form>
                        </div>

                        <!-- Danh sách Cart Items -->
                        <div class="divide-y divide-slate-100 dark:divide-slate-800/60">
                            @foreach (var item in Model.Items)
                            {
                                <div class="p-6 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                                    <!-- Hình ảnh & Thông tin sản phẩm -->
                                    <div class="flex items-center space-x-4 flex-1 min-w-0">
                                        <img src="@(string.IsNullOrEmpty(item.LinkHinhAnh) ? "/assets/journey.jpg" : item.LinkHinhAnh)"
                                             alt="@item.TenSanPham"
                                             class="w-20 h-20 rounded-xl object-cover border border-slate-200 dark:border-slate-700 flex-shrink-0" />
                                        <div class="min-w-0 flex-1">
                                            <a href="/CuaHang/ChiTiet/@item.IDSanPham" class="text-base font-bold text-slate-900 dark:text-white hover:text-emerald-600 dark:hover:text-emerald-400 transition-colors truncate block">
                                                @item.TenSanPham
                                            </a>
                                            <p class="text-sm font-semibold text-emerald-600 dark:text-emerald-400 mt-0.5">
                                                @item.DonGia.ToString("N0") VNĐ
                                            </p>
                                            @if (!string.IsNullOrEmpty(item.ThongDiepGayQuy))
                                            {
                                                <div class="mt-1.5 inline-flex items-center text-xs font-medium px-2 py-0.5 rounded-md bg-amber-50 dark:bg-amber-950/40 text-amber-700 dark:text-amber-300 border border-amber-200 dark:border-amber-800/50">
                                                    <i class="fas fa-heart text-rose-500 mr-1 text-[10px]"></i>
                                                    @item.PhanTramDongGop% Quỹ (@item.TienDongGop.ToString("N0")₫)
                                                </div>
                                            }
                                        </div>
                                    </div>

                                    <!-- Bộ điều khiển số lượng (Quantity controls) -->
                                    <div class="flex items-center space-x-4 self-end sm:self-center">
                                        <div class="flex items-center border border-slate-300 dark:border-slate-700 rounded-lg overflow-hidden bg-white dark:bg-night shadow-sm">
                                            <!-- Nút giảm số lượng -->
                                            <form asp-area="CuaHang" asp-controller="GioHang" asp-action="CapNhatSoLuong" method="post" class="inline">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="id" value="@item.IDSanPham" />
                                                <input type="hidden" name="soLuong" value="@(item.SoLuong - 1)" />
                                                <button type="submit" class="w-8 h-8 flex items-center justify-center text-slate-500 hover:text-emerald-600 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors" title="Giảm số lượng">
                                                    <i class="fas fa-minus text-xs"></i>
                                                </button>
                                            </form>

                                            <!-- Ô nhập số lượng -->
                                            <form asp-area="CuaHang" asp-controller="GioHang" asp-action="CapNhatSoLuong" method="post" class="inline">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="id" value="@item.IDSanPham" />
                                                <input type="number" name="soLuong" value="@item.SoLuong" min="1" max="@item.SoLuongTon"
                                                       class="w-12 h-8 text-center text-sm font-bold border-0 bg-transparent text-slate-900 dark:text-white focus:ring-0 p-0"
                                                       onchange="this.form.submit()" />
                                            </form>

                                            <!-- Nút tăng số lượng -->
                                            <form asp-area="CuaHang" asp-controller="GioHang" asp-action="CapNhatSoLuong" method="post" class="inline">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="id" value="@item.IDSanPham" />
                                                <input type="hidden" name="soLuong" value="@(item.SoLuong + 1)" />
                                                <button type="submit" class="w-8 h-8 flex items-center justify-center text-slate-500 hover:text-emerald-600 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors" title="Tăng số lượng">
                                                    <i class="fas fa-plus text-xs"></i>
                                                </button>
                                            </form>
                                        </div>

                                        <!-- Thành tiền dòng -->
                                        <div class="text-right min-w-[90px]">
                                            <span class="text-base font-extrabold text-slate-900 dark:text-white">
                                                @item.ThanhTien.ToString("N0")₫
                                            </span>
                                        </div>

                                        <!-- Nút xóa khỏi giỏ -->
                                        <form asp-area="CuaHang" asp-controller="GioHang" asp-action="XoaKhoiGio" method="post" class="inline">
                                            @Html.AntiForgeryToken()
                                            <input type="hidden" name="id" value="@item.IDSanPham" />
                                            <button type="submit" class="text-slate-400 hover:text-rose-500 dark:hover:text-rose-400 p-1.5 transition-colors" title="Xóa khỏi giỏ">
                                                <i class="fas fa-trash-alt"></i>
                                            </button>
                                        </form>
                                    </div>
                                </div>
                            }
                        </div>

                        <!-- Footer liên kết tiếp tục mua sắm -->
                        <div class="p-6 bg-slate-50/70 dark:bg-night/50 border-t border-slate-100 dark:border-slate-800 flex items-center justify-between">
                            <a href="/CuaHang" class="inline-flex items-center text-sm font-semibold text-emerald-600 dark:text-emerald-400 hover:underline">
                                <i class="fas fa-arrow-left mr-2"></i>Chọn thêm quà gây quỹ khác
                            </a>
                            <span class="text-xs text-slate-500 dark:text-slate-400">
                                Toàn bộ sản phẩm được đóng gói trang trọng gửi đến bạn
                            </span>
                        </div>
                    </div>
                </div>

                <!-- CỘT PHẢI (5 COLS): BIỂU MẪU ĐẶT HÀNG & THANH TOÁN (CHECKOUT FORM) -->
                <div class="lg:col-span-5">
                    <div class="bg-white dark:bg-night-2 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-6 sticky top-6">
                        <h2 class="text-xl font-bold text-slate-900 dark:text-white mb-6 flex items-center pb-4 border-b border-slate-100 dark:border-slate-800">
                            <i class="fas fa-clipboard-check text-emerald-600 dark:text-emerald-400 mr-2.5"></i>
                            Thông tin Đặt hàng &amp; Ủng hộ
                        </h2>

                        <form asp-area="CuaHang" asp-controller="GioHang" asp-action="DatHang" method="post" id="checkoutForm" class="space-y-4">
                            @Html.AntiForgeryToken()

                            <!-- Họ và tên -->
                            <div>
                                <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-1.5">
                                    Họ và tên người nhận <span class="text-rose-500">*</span>
                                </label>
                                <input type="text" name="HoTen" value="@checkout.HoTen" required
                                       placeholder="Nguyễn Văn An"
                                       class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 dark:border-slate-700 bg-white dark:bg-night text-slate-900 dark:text-white text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-colors" />
                            </div>

                            <!-- Số điện thoại & Email -->
                            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div>
                                    <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-1.5">
                                        Số điện thoại <span class="text-rose-500">*</span>
                                    </label>
                                    <input type="tel" name="SoDienThoai" value="@checkout.SoDienThoai" required
                                           placeholder="0901234567"
                                           class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 dark:border-slate-700 bg-white dark:bg-night text-slate-900 dark:text-white text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-colors" />
                                </div>
                                <div>
                                    <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-1.5">
                                        Email (nhận mã đơn)
                                    </label>
                                    <input type="email" name="Email" value="@checkout.Email"
                                           placeholder="email@example.com"
                                           class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 dark:border-slate-700 bg-white dark:bg-night text-slate-900 dark:text-white text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-colors" />
                                </div>
                            </div>

                            <!-- Địa chỉ nhận hàng -->
                            <div>
                                <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-1.5">
                                    Địa chỉ nhận hàng chi tiết <span class="text-rose-500">*</span>
                                </label>
                                <textarea name="DiaChi" rows="2" required
                                          placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành phố"
                                          class="w-full px-3.5 py-2 rounded-xl border border-slate-300 dark:border-slate-700 bg-white dark:bg-night text-slate-900 dark:text-white text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-colors">@checkout.DiaChi</textarea>
                            </div>

                            <!-- Lời nhắn gửi / Lời chúc -->
                            <div>
                                <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-1.5">
                                    Lời chúc gửi tới các em nhỏ &amp; BTC
                                </label>
                                <input type="text" name="GhiChu" value="@checkout.GhiChu"
                                       placeholder="Chúc các em Trung thu thật ấm áp và luôn vui tươi..."
                                       class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 dark:border-slate-700 bg-white dark:bg-night text-slate-900 dark:text-white text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition-colors" />
                            </div>

                            <!-- ỦNG HỘ THÊM TÙY TÂM CHO QUỸ -->
                            <div class="pt-3 pb-2 border-t border-slate-100 dark:border-slate-800">
                                <label class="block text-xs font-bold uppercase tracking-wider text-amber-600 dark:text-amber-400 mb-1.5 flex items-center">
                                    <i class="fas fa-hand-holding-heart mr-1.5"></i>Ủng hộ thêm tùy tâm cho quỹ (VNĐ)
                                </label>
                                <div class="relative">
                                    <input type="number" id="inputTienUngHoThem" name="TienUngHoThem"
                                           value="@checkout.TienUngHoThem" min="0" step="10000"
                                           placeholder="0"
                                           class="w-full pl-3.5 pr-14 py-2.5 rounded-xl border border-amber-300 dark:border-amber-700/60 bg-amber-50/30 dark:bg-amber-950/20 text-slate-900 dark:text-white font-bold text-sm focus:ring-2 focus:ring-amber-500 focus:border-amber-500 transition-colors" />
                                    <span class="absolute right-3.5 top-2.5 text-xs font-bold text-amber-600 dark:text-amber-400">VNĐ</span>
                                </div>
                                <div class="flex flex-wrap gap-1.5 mt-2">
                                    <button type="button" onclick="setExtraDonation(20000)" class="text-xs px-2.5 py-1 rounded-lg bg-slate-100 dark:bg-slate-800 hover:bg-amber-100 dark:hover:bg-amber-950 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 transition-colors">+20k</button>
                                    <button type="button" onclick="setExtraDonation(50000)" class="text-xs px-2.5 py-1 rounded-lg bg-slate-100 dark:bg-slate-800 hover:bg-amber-100 dark:hover:bg-amber-950 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 transition-colors">+50k</button>
                                    <button type="button" onclick="setExtraDonation(100000)" class="text-xs px-2.5 py-1 rounded-lg bg-slate-100 dark:bg-slate-800 hover:bg-amber-100 dark:hover:bg-amber-950 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 transition-colors">+100k</button>
                                    <button type="button" onclick="setExtraDonation(200000)" class="text-xs px-2.5 py-1 rounded-lg bg-slate-100 dark:bg-slate-800 hover:bg-amber-100 dark:hover:bg-amber-950 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 transition-colors">+200k</button>
                                </div>
                            </div>

                            <!-- PHƯƠNG THỨC THANH TOÁN (VIETQR VS COD) -->
                            <div class="pt-3 pb-2 border-t border-slate-100 dark:border-slate-800">
                                <label class="block text-xs font-bold uppercase tracking-wider text-slate-700 dark:text-slate-300 mb-2">
                                    Phương thức thanh toán <span class="text-rose-500">*</span>
                                </label>
                                <div class="space-y-2.5">
                                    <!-- Chuyển khoản VietQR -->
                                    <label class="flex items-center p-3 rounded-xl border border-emerald-500/40 bg-emerald-50/50 dark:bg-emerald-950/20 cursor-pointer hover:bg-emerald-50 dark:hover:bg-emerald-950/40 transition-colors">
                                        <input type="radio" name="PhuongThucThanhToan" value="ChuyenKhoanQR" checked
                                               class="h-4 w-4 text-emerald-600 focus:ring-emerald-500 border-slate-300" />
                                        <div class="ml-3">
                                            <span class="block text-sm font-bold text-slate-900 dark:text-white flex items-center">
                                                <i class="fas fa-qrcode text-emerald-600 mr-1.5"></i>
                                                Chuyển khoản VietQR (Khuyên dùng)
                                            </span>
                                            <span class="block text-xs text-slate-500 dark:text-slate-400">
                                                Quét mã QR tiện lợi qua app ngân hàng/MoMo, xác nhận đơn lập tức
                                            </span>
                                        </div>
                                    </label>

                                    <!-- Tiền mặt khi nhận hàng (COD) -->
                                    <label class="flex items-center p-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-night cursor-pointer hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                        <input type="radio" name="PhuongThucThanhToan" value="COD"
                                               class="h-4 w-4 text-emerald-600 focus:ring-emerald-500 border-slate-300" />
                                        <div class="ml-3">
                                            <span class="block text-sm font-bold text-slate-900 dark:text-white flex items-center">
                                                <i class="fas fa-truck text-slate-600 dark:text-slate-400 mr-1.5"></i>
                                                Thanh toán khi nhận hàng (COD / Tiền mặt)
                                            </span>
                                            <span class="block text-xs text-slate-500 dark:text-slate-400">
                                                Thanh toán tiền mặt cho shipper khi nhận được bưu phẩm
                                            </span>
                                        </div>
                                    </label>
                                </div>
                            </div>

                            <!-- BẢNG TỔNG KẾT TÀI CHÍNH -->
                            <div class="pt-4 border-t border-slate-200 dark:border-slate-800 space-y-2 text-sm">
                                <div class="flex justify-between text-slate-600 dark:text-slate-400">
                                    <span>Tiền hàng:</span>
                                    <span class="font-semibold text-slate-900 dark:text-white">@Model.TongTienHang.ToString("N0") VNĐ</span>
                                </div>
                                <div class="flex justify-between text-slate-600 dark:text-slate-400">
                                    <span>Phí giao hàng:</span>
                                    <span class="font-semibold text-emerald-600 dark:text-emerald-400">Miễn phí (Tài trợ)</span>
                                </div>
                                <div class="flex justify-between text-slate-600 dark:text-slate-400">
                                    <span>Ủng hộ thêm:</span>
                                    <span id="displayExtraDonation" class="font-semibold text-amber-600 dark:text-amber-400">0 VNĐ</span>
                                </div>
                                <div class="pt-2 border-t border-dashed border-slate-200 dark:border-slate-700 flex justify-between items-baseline">
                                    <span class="text-base font-bold text-slate-900 dark:text-white">Tổng thanh toán:</span>
                                    <span id="displayTongThanhToan" class="text-2xl font-extrabold text-emerald-600 dark:text-emerald-400">
                                        @Model.TongThanhToan.ToString("N0") VNĐ
                                    </span>
                                </div>
                                <div class="p-2.5 rounded-lg bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-500/20 text-xs text-emerald-800 dark:text-emerald-300 flex items-center">
                                    <i class="fas fa-heart text-rose-500 mr-2 flex-shrink-0"></i>
                                    <span>Dự kiến đóng góp trực tiếp vào quỹ: <strong id="displayTotalCharity">@Model.TongTienDongGop.ToString("N0") VNĐ</strong></span>
                                </div>
                            </div>

                            <!-- NÚT XÁC NHẬN ĐẶT HÀNG -->
                            <button type="submit"
                                    class="w-full mt-4 py-4 px-6 rounded-xl font-extrabold text-white bg-gradient-to-r from-emerald-600 via-teal-600 to-emerald-700 hover:from-emerald-700 hover:to-teal-800 shadow-xl shadow-emerald-600/30 transition-all duration-200 transform hover:-translate-y-0.5 text-center flex items-center justify-center space-x-2">
                                <i class="fas fa-check-circle text-lg"></i>
                                <span class="tracking-wide">XÁC NHẬN ĐẶT HÀNG &amp; ỦNG HỘ QUỸ</span>
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        }
    </div>
</div>

<script>
    const baseSubtotal = @Model.TongTienHang;
    const baseCharity = @Model.TongTienDongGop;

    function setExtraDonation(amount) {
        const input = document.getElementById('inputTienUngHoThem');
        input.value = amount;
        updateTotals();
    }

    function updateTotals() {
        const input = document.getElementById('inputTienUngHoThem');
        const extra = Math.max(0, parseFloat(input.value) || 0);
        const grandTotal = baseSubtotal + extra;
        const totalCharity = baseCharity + extra;

        document.getElementById('displayExtraDonation').textContent = extra.toLocaleString('vi-VN') + ' VNĐ';
        document.getElementById('displayTongThanhToan').textContent = grandTotal.toLocaleString('vi-VN') + ' VNĐ';
        document.getElementById('displayTotalCharity').textContent = totalCharity.toLocaleString('vi-VN') + ' VNĐ';
    }

    document.getElementById('inputTienUngHoThem')?.addEventListener('input', updateTotals);
</script>
```

---

### 4.3 `Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`
Đường dẫn tệp: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml`

```razor
@model WEB_TinhNguyen.Areas.CuaHang.Models.DonHangGayQuy
@{
    ViewData["Title"] = "Đặt hàng & Ủng hộ gây quỹ thành công - " + Model.MaDonHang;
    var isBankTransfer = Model.PhuongThucThanhToan != "COD";
    var cleanAmount = (long)Model.TongThanhToan;
    var cleanMemo = Uri.EscapeDataString(Model.MaDonHang);
    var qrUrl = $"https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}&accountName=NGUYEN%20THAI%20VINH";
}

<div class="min-h-screen bg-slate-50 dark:bg-night py-12 px-4 sm:px-6 lg:px-8 transition-colors duration-200">
    <div class="max-w-3xl mx-auto">
        <!-- Card xác nhận đặt hàng thành công -->
        <div class="bg-white dark:bg-night-2 rounded-3xl shadow-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
            <!-- Header chúc mừng & tri ân -->
            <div class="p-8 text-center bg-gradient-to-b from-emerald-500/10 via-emerald-500/5 to-transparent border-b border-slate-100 dark:border-slate-800">
                <div class="w-20 h-20 mx-auto mb-4 rounded-full bg-emerald-500 text-white flex items-center justify-center text-4xl shadow-lg shadow-emerald-500/30">
                    <i class="fas fa-check"></i>
                </div>
                <h1 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight">
                    Đặt hàng &amp; Ủng hộ Thành Công!
                </h1>
                <p class="mt-2 text-base text-slate-600 dark:text-slate-300 max-w-lg mx-auto">
                    Cảm ơn bạn <strong class="text-emerald-600 dark:text-emerald-400">@Model.HoTenNguoiMua</strong> đã đồng hành cùng chiến dịch thiện nguyện <em>"Trăng Sáng Langbiang 2026"</em>.
                </p>
                <div class="mt-4 inline-flex items-center px-4 py-1.5 rounded-full bg-amber-50 dark:bg-amber-950/50 border border-amber-300 dark:border-amber-700/60 text-amber-800 dark:text-amber-200 text-sm font-semibold">
                    <i class="fas fa-tag mr-2 text-amber-600"></i>Mã đơn hàng: <span class="font-mono ml-1 text-base font-extrabold">@Model.MaDonHang</span>
                </div>
            </div>

            <!-- PHẦN THANH TOÁN VIETQR / COD -->
            <div class="p-6 sm:p-8 border-b border-slate-100 dark:border-slate-800">
                @if (isBankTransfer)
                {
                    <div class="bg-gradient-to-br from-emerald-500/5 to-teal-500/10 rounded-2xl p-6 border border-emerald-500/20">
                        <div class="text-center mb-6">
                            <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-extrabold bg-emerald-600 text-white uppercase tracking-wider mb-2">
                                <i class="fas fa-qrcode mr-1.5"></i>Thanh toán qua VietQR
                            </span>
                            <h2 class="text-xl font-bold text-slate-900 dark:text-white">
                                Quét mã VietQR chuyển khoản ủng hộ
                            </h2>
                            <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                                Mở ứng dụng ngân hàng hoặc ví điện tử bất kỳ (Vietcombank, MBBank, Techcombank, MoMo...) và Quét mã để thanh toán tự động
                            </p>
                        </div>

                        <!-- Ảnh mã VietQR động -->
                        <div class="flex justify-center mb-6">
                            <div class="p-3 bg-white rounded-2xl shadow-xl border border-emerald-500/30 max-w-xs">
                                <img src="@qrUrl" alt="VietQR Quét mã thanh toán đơn hàng @Model.MaDonHang"
                                     class="w-full h-auto rounded-xl" />
                                <div class="text-center mt-2">
                                    <span class="text-[11px] font-bold text-slate-500 uppercase tracking-widest">
                                        VIETQR • MBBANK CHUẨN NAPAS247
                                    </span>
                                </div>
                            </div>
                        </div>

                        <!-- Bảng chi tiết thông tin chuyển khoản thủ công -->
                        <div class="bg-white dark:bg-night rounded-xl p-4 border border-slate-200 dark:border-slate-800 space-y-2 text-sm max-w-md mx-auto">
                            <div class="flex justify-between items-center py-1 border-b border-slate-100 dark:border-slate-800">
                                <span class="text-slate-500 dark:text-slate-400">Ngân hàng:</span>
                                <span class="font-bold text-slate-900 dark:text-white">Quân Đội (MBBank)</span>
                            </div>
                            <div class="flex justify-between items-center py-1 border-b border-slate-100 dark:border-slate-800">
                                <span class="text-slate-500 dark:text-slate-400">Số tài khoản:</span>
                                <span class="font-mono font-extrabold text-emerald-600 dark:text-emerald-400 text-base">0924044283</span>
                            </div>
                            <div class="flex justify-between items-center py-1 border-b border-slate-100 dark:border-slate-800">
                                <span class="text-slate-500 dark:text-slate-400">Chủ tài khoản:</span>
                                <span class="font-bold text-slate-900 dark:text-white uppercase">NGUYEN THAI VINH</span>
                            </div>
                            <div class="flex justify-between items-center py-1 border-b border-slate-100 dark:border-slate-800">
                                <span class="text-slate-500 dark:text-slate-400">Số tiền:</span>
                                <span class="font-extrabold text-emerald-600 dark:text-emerald-400 text-base">@Model.TongThanhToan.ToString("N0") VNĐ</span>
                            </div>
                            <div class="flex justify-between items-center py-1">
                                <span class="text-slate-500 dark:text-slate-400">Nội dung CK:</span>
                                <span class="font-mono font-bold text-amber-600 dark:text-amber-400">@Model.MaDonHang</span>
                            </div>
                        </div>
                    </div>
                }
                else
                {
                    <div class="bg-slate-50 dark:bg-night rounded-2xl p-6 border border-slate-200 dark:border-slate-800 text-center">
                        <div class="w-12 h-12 mx-auto mb-3 rounded-full bg-slate-200 dark:bg-slate-700 text-slate-600 dark:text-slate-300 flex items-center justify-center text-xl">
                            <i class="fas fa-truck"></i>
                        </div>
                        <h2 class="text-lg font-bold text-slate-900 dark:text-white">
                            Thanh toán tiền mặt khi nhận hàng (COD)
                        </h2>
                        <p class="text-sm text-slate-600 dark:text-slate-400 mt-2 max-w-md mx-auto">
                            Đơn hàng của bạn đã được ghi nhận. Tình nguyện viên ban tổ chức sẽ liên hệ qua số điện thoại <strong>@Model.SoDienThoai</strong> để xác nhận và gửi chuyển phát nhanh quà đến bạn.
                        </p>
                    </div>
                }
            </div>

            <!-- TÓM TẮT THÔNG TIN NGƯỜI NHẬN & ĐƠN HÀNG -->
            <div class="p-6 sm:p-8 space-y-6">
                <div>
                    <h3 class="text-base font-bold text-slate-900 dark:text-white mb-3 flex items-center">
                        <i class="fas fa-map-marker-alt text-emerald-600 mr-2"></i>Thông tin người nhận
                    </h3>
                    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm bg-slate-50 dark:bg-night p-4 rounded-xl border border-slate-200 dark:border-slate-800">
                        <div>
                            <span class="text-slate-500 dark:text-slate-400">Họ và tên:</span>
                            <p class="font-bold text-slate-900 dark:text-white">@Model.HoTenNguoiMua</p>
                        </div>
                        <div>
                            <span class="text-slate-500 dark:text-slate-400">Số điện thoại:</span>
                            <p class="font-bold text-slate-900 dark:text-white">@Model.SoDienThoai</p>
                        </div>
                        <div class="sm:col-span-2">
                            <span class="text-slate-500 dark:text-slate-400">Địa chỉ giao hàng:</span>
                            <p class="font-bold text-slate-900 dark:text-white">@Model.DiaChiNhanHang</p>
                        </div>
                        @if (!string.IsNullOrEmpty(Model.GhiChu))
                        {
                            <div class="sm:col-span-2">
                                <span class="text-slate-500 dark:text-slate-400">Lời nhắn:</span>
                                <p class="italic text-slate-700 dark:text-slate-300">"@Model.GhiChu"</p>
                            </div>
                        }
                    </div>
                </div>

                <!-- DANH SÁCH SẢN PHẨM ĐÃ ĐẶT -->
                <div>
                    <h3 class="text-base font-bold text-slate-900 dark:text-white mb-3 flex items-center">
                        <i class="fas fa-box text-emerald-600 mr-2"></i>Sản phẩm gây quỹ đã đặt
                    </h3>
                    <div class="border border-slate-200 dark:border-slate-800 rounded-xl overflow-hidden divide-y divide-slate-100 dark:divide-slate-800">
                        @if (Model.ChiTietDonHangs != null)
                        {
                            foreach (var detail in Model.ChiTietDonHangs)
                            {
                                <div class="p-3.5 flex items-center justify-between text-sm">
                                    <div class="flex items-center space-x-3">
                                        <img src="@(string.IsNullOrEmpty(detail.LinkHinhAnh) ? "/assets/journey.jpg" : detail.LinkHinhAnh)"
                                             alt="@detail.TenSanPham"
                                             class="w-12 h-12 rounded-lg object-cover border border-slate-200 dark:border-slate-700" />
                                        <div>
                                            <p class="font-bold text-slate-900 dark:text-white">@detail.TenSanPham</p>
                                            <p class="text-xs text-slate-500">Số lượng: @detail.SoLuong × @detail.DonGia.ToString("N0")₫</p>
                                        </div>
                                    </div>
                                    <div class="text-right">
                                        <p class="font-extrabold text-slate-900 dark:text-white">@detail.ThanhTien.ToString("N0")₫</p>
                                        <p class="text-[11px] text-emerald-600 dark:text-emerald-400">Ủng hộ quỹ: @detail.TienDongGop.ToString("N0")₫</p>
                                    </div>
                                </div>
                            }
                        }
                    </div>

                    <!-- Tổng kết thanh toán -->
                    <div class="mt-4 p-4 rounded-xl bg-slate-50 dark:bg-night border border-slate-200 dark:border-slate-800 text-sm space-y-1.5">
                        <div class="flex justify-between text-slate-600 dark:text-slate-400">
                            <span>Tiền hàng:</span>
                            <span class="font-semibold text-slate-900 dark:text-white">@Model.TongTienHang.ToString("N0") VNĐ</span>
                        </div>
                        @if (Model.TienUngHoThem > 0)
                        {
                            <div class="flex justify-between text-amber-600 dark:text-amber-400">
                                <span>Ủng hộ thêm tùy tâm:</span>
                                <span class="font-bold">+@Model.TienUngHoThem.ToString("N0") VNĐ</span>
                            </div>
                        }
                        <div class="flex justify-between text-slate-600 dark:text-slate-400">
                            <span>Phí giao hàng:</span>
                            <span class="font-semibold text-emerald-600">Miễn phí</span>
                        </div>
                        <div class="pt-2 border-t border-slate-200 dark:border-slate-700 flex justify-between text-base font-extrabold">
                            <span class="text-slate-900 dark:text-white">Tổng thanh toán:</span>
                            <span class="text-emerald-600 dark:text-emerald-400 text-xl">@Model.TongThanhToan.ToString("N0") VNĐ</span>
                        </div>
                    </div>
                </div>

                <!-- ĐIỀU HƯỚNG QUAY LẠI CỬA HÀNG VÀ TRANG CHỦ -->
                <div class="pt-6 border-t border-slate-100 dark:border-slate-800 flex flex-col sm:flex-row items-center justify-center gap-4">
                    <a href="/CuaHang"
                       class="w-full sm:w-auto px-6 py-3 rounded-xl font-bold text-white bg-emerald-600 hover:bg-emerald-700 text-center shadow-lg shadow-emerald-600/30 transition-all">
                        <i class="fas fa-shopping-bag mr-2"></i>Tiếp tục mua sắm quà gây quỹ
                    </a>
                    <a href="/"
                       class="w-full sm:w-auto px-6 py-3 rounded-xl font-bold text-slate-700 dark:text-slate-300 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-center transition-all">
                        <i class="fas fa-home mr-2"></i>Về Trang chủ
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>
```

---

## 5. Verification Method

Sau khi Worker triển khai các tệp trên, tiến hành kiểm tra độc lập bằng các lệnh sau:

1. **Biên dịch hệ thống**:
   ```bash
   export XDG_RUNTIME_DIR=/tmp
   dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Điều kiện hợp lệ*: Biên dịch thành công 0 lỗi.

2. **Kiểm tra Feature 9 (Cart Management)**:
   ```bash
   bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --feature 9
   ```
   *Điều kiện hợp lệ*: 5/5 bài test PASS (F09_T01 đến F09_T05).

3. **Kiểm tra Feature 10 (Checkout & Donation Form)**:
   ```bash
   bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --feature 10
   ```
   *Điều kiện hợp lệ*: 5/5 bài test PASS (F10_T01 đến F10_T05).

4. **Kiểm tra Feature 11 (Order Confirmation & VietQR)**:
   ```bash
   bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --feature 11
   ```
   *Điều kiện hợp lệ*: 5/5 bài test PASS (F11_T01 đến F11_T05).

5. **Kiểm tra các kịch bản thực tế Scenario S03, S04, S05, S06 trong Tier 4**:
   ```bash
   bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --tier 4
   ```
   *Điều kiện hợp lệ*: S03, S04, S05, S06 đều PASS.

6. **Kiểm tra toàn bộ Tier 1, 2, 3, 5**:
   ```bash
   bash /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/run_e2e_tests.sh --all
   ```
