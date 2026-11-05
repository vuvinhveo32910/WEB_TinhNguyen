using System;
using System.IO;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    public class Tier1_CartCheckoutTests
    {
        // =========================================================================
        // FEATURE 9: Cart Management
        // =========================================================================

        [E2ETest(1, 9, "F09_T01: Cart item subtotal calculation matches oracle formula")]
        public void F09_T01_CartItemSubtotalCalculation()
        {
            var unitPrice = 150000m;
            var quantity = 3;
            var expectedSubtotal = ReferenceOracles.LineTotal(unitPrice, quantity);

            Assert.Equal(450000m, expectedSubtotal, "Subtotal must equal unitPrice * quantity");

            var cartItemType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.CartItem", "CartItem");
            var item = Activator.CreateInstance(cartItemType);
            Assert.NotNull(item, "CartItem must be instantiable");

            cartItemType.GetProperty("DonGia")?.SetValue(item, unitPrice);
            cartItemType.GetProperty("SoLuong")?.SetValue(item, quantity);
            var actualThanhTien = (decimal)(cartItemType.GetProperty("ThanhTien")?.GetValue(item) ?? 0m);

            Assert.Equal(expectedSubtotal, actualThanhTien, "CartItem.ThanhTien property must equal 450,000 VNĐ");
        }

        [E2ETest(1, 9, "F09_T02: CartViewModel aggregates total quantity and merchandise total")]
        public void F09_T02_CartViewModelAggregations()
        {
            var cartVmType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.CartViewModel", "CartViewModel");

            Assert.NotNull(cartVmType.GetProperty("Items"), "CartViewModel must have Items list");
            Assert.NotNull(cartVmType.GetProperty("TongSoLuong"), "CartViewModel must have TongSoLuong");
            Assert.NotNull(cartVmType.GetProperty("TongTienHang"), "CartViewModel must have TongTienHang");
            Assert.NotNull(cartVmType.GetProperty("TongThanhToan"), "CartViewModel must have TongThanhToan");
        }

        [E2ETest(1, 9, "F09_T03: Cart view template exists and renders cart items")]
        public void F09_T03_CartViewTemplateExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Cart view Areas/CuaHang/Views/GioHang/Index.cshtml");

            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("Items", viewContent, "Cart view must render Items collection");
            Assert.Contains("TongThanhToan", viewContent, "Cart view must render TongThanhToan");
        }

        [E2ETest(1, 9, "F09_T04: Cart view contains quantity update and delete controls")]
        public void F09_T04_CartViewUpdateAndDeleteControls()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Cart view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("CapNhatGio") || viewContent.Contains("update") || viewContent.Contains("soLuong"),
                "Cart view must have quantity update action"
            );
            Assert.True(
                viewContent.Contains("XoaKhoiGio") || viewContent.Contains("delete") || viewContent.Contains("remove"),
                "Cart view must have remove item action"
            );
        }

        [E2ETest(1, 9, "F09_T05: Cart handles empty cart state gracefully")]
        public void F09_T05_EmptyCartStateHandling()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Cart view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("trống") || viewContent.Contains("chưa có sản phẩm") || viewContent.Contains("!Model.Items.Any()"),
                "Cart view must provide empty state message when cart is empty"
            );
        }

        // =========================================================================
        // FEATURE 10: Checkout & Donation Form
        // =========================================================================

        [E2ETest(1, 10, "F10_T01: CheckoutViewModel contains required customer fields")]
        public void F10_T01_CheckoutViewModelRequiredFields()
        {
            var checkoutType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.CheckoutViewModel", "CheckoutViewModel");

            Assert.NotNull(checkoutType.GetProperty("HoTen"), "CheckoutViewModel must have HoTen");
            Assert.NotNull(checkoutType.GetProperty("SoDienThoai"), "CheckoutViewModel must have SoDienThoai");
            Assert.NotNull(checkoutType.GetProperty("DiaChi"), "CheckoutViewModel must have DiaChi");
            Assert.NotNull(checkoutType.GetProperty("Email"), "CheckoutViewModel must have Email");
            Assert.NotNull(checkoutType.GetProperty("TienUngHoThem"), "CheckoutViewModel must have TienUngHoThem");
            Assert.NotNull(checkoutType.GetProperty("PhuongThucThanhToan"), "CheckoutViewModel must have PhuongThucThanhToan");
        }

        [E2ETest(1, 10, "F10_T02: Checkout phone validation oracle passes valid VN mobile numbers")]
        public void F10_T02_PhoneValidationOracle()
        {
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("0901234567"), "0901234567 must be valid");
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("0389998888"), "0389998888 must be valid");
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("0771234567"), "0771234567 must be valid");
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("12345"), "Short phone must be invalid");
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("02838383838"), "Landline 028 must be invalid for mobile validation");
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("abcdefghij"), "Non-numeric phone must be invalid");
        }

        [E2ETest(1, 10, "F10_T03: Voluntary extra donation calculation matches oracle")]
        public void F10_T03_ExtraDonationCalculation()
        {
            var subtotal = 300000m;
            var extraDonation = 50000m;
            var expectedTotal = ReferenceOracles.OrderTotal(subtotal, extraDonation, 0m);

            Assert.Equal(350000m, expectedTotal, "Total must equal subtotal + extra donation");
        }

        [E2ETest(1, 10, "F10_T04: Payment methods support VietQR and COD options")]
        public void F10_T04_PaymentMethodOptionsSupported()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Checkout form in GioHang/Index.cshtml");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("QR") || viewContent.Contains("ChuyenKhoan") || viewContent.Contains("Chuyển khoản"),
                "Checkout form must support QR / Bank Transfer payment option"
            );
            Assert.True(
                viewContent.Contains("COD") || viewContent.Contains("tiền mặt") || viewContent.Contains("nhận hàng"),
                "Checkout form must support COD payment option"
            );
        }

        [E2ETest(1, 10, "F10_T05: Checkout form contains order placement submit button")]
        public void F10_T05_CheckoutSubmitButton()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Checkout form in GioHang/Index.cshtml");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("type=\"submit\"") || viewContent.Contains("type='submit'"),
                "Checkout section must contain submit button to place order"
            );
        }

        // =========================================================================
        // FEATURE 11: Order Confirmation & VietQR
        // =========================================================================

        [E2ETest(1, 11, "F11_T01: Order confirmation view template exists")]
        public void F11_T01_ConfirmationViewExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(viewPath, "Confirmation view Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml");
        }

        [E2ETest(1, 11, "F11_T02: Order confirmation displays order code and donor information")]
        public void F11_T02_ConfirmationDisplaysOrderCode()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(viewPath, "Confirmation view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("MaDonHang", viewContent, "Confirmation view must display MaDonHang");
            Assert.Contains("HoTenNguoiMua", viewContent, "Confirmation view must display HoTenNguoiMua");
            Assert.Contains("TongThanhToan", viewContent, "Confirmation view must display TongThanhToan");
        }

        [E2ETest(1, 11, "F11_T03: VietQR URL generator creates compliant endpoint")]
        public void F11_T03_VietQrUrlGeneration()
        {
            var orderCode = "GQ20261103-TEST";
            var amount = 250000m;
            var qrUrl = ReferenceOracles.VietQrImageUrl("VCB", "1023456789", amount, orderCode);

            Assert.Contains("img.vietqr.io/image/VCB-1023456789-compact2.png", qrUrl, "VietQR URL must have correct image path");
            Assert.Contains("amount=250000", qrUrl, "VietQR URL must include amount parameter");
            Assert.Contains("addInfo=GQ20261103-TEST", qrUrl, "VietQR URL must include memo parameter with order code");
        }

        [E2ETest(1, 11, "F11_T04: Confirmation view embeds VietQR when payment is bank transfer")]
        public void F11_T04_ConfirmationEmbedsVietQr()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(viewPath, "Confirmation view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("vietqr") || viewContent.Contains("QR") || viewContent.Contains("Quét mã"),
                "Confirmation view must embed VietQR image or instructions for transfer"
            );
        }

        [E2ETest(1, 11, "F11_T05: Confirmation view provides navigation back to store")]
        public void F11_T05_ConfirmationReturnNavigation()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(viewPath, "Confirmation view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("CuaHang") || viewContent.Contains("tiếp tục mua sắm") || viewContent.Contains("Trang chủ"),
                "Confirmation view must offer link to return to store or homepage"
            );
        }
    }
}
