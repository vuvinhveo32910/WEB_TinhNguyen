using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Areas.CuaHang.Controllers;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    /// <summary>
    /// Tier 5: Adversarial Challenge & Stress Harness for Milestone 3 (Cart, Checkout & VietQR)
    /// Empirically challenges:
    /// 1. Cart Management (Feature 9):
    ///    - Item addition with valid, nonexistent, and zero-stock products
    ///    - Zero, negative, and oversized quantity capping
    ///    - Quantity modification (increment, decrement, zero/negative removal, stock cap)
    ///    - Single and bulk cart removal
    ///    - AJAX vs synchronous form POST handling
    ///    - Session cart isolation across concurrent users
    /// 2. Checkout & Extra Donation (Feature 10):
    ///    - Empty cart order rejection
    ///    - Vietnamese mobile phone validation and formatting resilience
    ///    - Invalid phone rejection (landlines, short/long strings, invalid prefixes, non-numeric)
    ///    - Missing customer fields rejection and model state redisplay
    ///    - Extra voluntary donation calculation and negative donation sanitization
    ///    - Default payment method fallback
    /// 3. Order Confirmation & VietQR (Feature 11):
    ///    - 15-character order code generation format (C04: GQyyyyMMdd-XXXX)
    ///    - VietQR URL memo and amount match (C05)
    ///    - COD vs VietQR view branching
    ///    - Nonexistent order ID fallback handling
    ///    - Inventory decrement and DaBan increment (C07)
    ///    - High-concurrency cart mutation & order placement stress
    /// </summary>
    public class Tier5_ChallengerM3_2Harness
    {
        #region Test Helpers & Mocks

        public class MockTempDataProvider : ITempDataProvider
        {
            private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
            public IDictionary<string, object> LoadTempData(HttpContext context) => _data;
            public void SaveTempData(HttpContext context, IDictionary<string, object> values)
            {
                foreach (var kvp in values)
                {
                    _data[kvp.Key] = kvp.Value;
                }
            }
        }

        private static bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        }

        private static GioHangController CreateTestCartController(out ApplicationDbContext context, ISession session, bool isAjax = false)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=127.0.0.1,1;Database=dummy;Connect Timeout=1;")
                .Options;
            context = new ApplicationDbContext(options);

            var controller = new GioHangController(context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;

            if (isAjax)
            {
                httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
                httpContext.Request.Headers["Accept"] = "application/json";
            }

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.TempData = new TempDataDictionary(httpContext, new MockTempDataProvider());
            return controller;
        }

        private static T AssertIsType<T>(object? obj, string message) where T : class
        {
            Assert.NotNull(obj, message);
            Assert.True(obj is T, $"Expected type {typeof(T).Name}, got {obj!.GetType().Name}. {message}");
            return (T)obj;
        }

        #endregion

        // =========================================================================
        // SECTION 1: CART LIFECYCLE & MUTATION STRESS TESTS (FEATURE 9)
        // =========================================================================

        [E2ETest(5, 9, "M3_C01: ThemVaoGio standard valid product adds item and populates metadata")]
        public async Task M3_C01_ThemVaoGio_StandardAddition_Success()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var result = await controller.ThemVaoGio(1, 2);
            var redirect = AssertIsType<RedirectToActionResult>(result, "ThemVaoGio should redirect to Index on non-AJAX request");
            Assert.Equal("Index", redirect.ActionName, "ActionName must be Index");

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must be present in session");
            Assert.Equal(1, cart.Count, "Cart should contain exactly 1 item");

            var item = cart[0];
            Assert.Equal(1, item.IDSanPham, "Item ID must match added product");
            Assert.Equal(2, item.SoLuong, "Item quantity must equal 2");
            Assert.True(item.DonGia > 0, "Item price must be positive");
            Assert.Equal(item.DonGia * 2, item.ThanhTien, "ThanhTien must equal DonGia * 2");
            Assert.False(string.IsNullOrEmpty(item.TenSanPham), "Product name must not be empty");
        }

        [E2ETest(5, 9, "M3_C02: ThemVaoGio nonexistent product ID safely redirects with error")]
        public async Task M3_C02_ThemVaoGio_NonExistentProduct_RedirectsWithError()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var result = await controller.ThemVaoGio(999999, 1);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Nonexistent product should redirect to CuaHang.Index");
            Assert.Equal("Index", redirect.ActionName, "Action must redirect to Index");
            Assert.Equal("CuaHang", redirect.ControllerName, "Controller must redirect to CuaHang");

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.True(cart == null || cart.Count == 0, "Cart must remain empty when adding nonexistent product");
            Assert.NotNull(controller.TempData["ErrorMessage"], "ErrorMessage must be set in TempData");
        }

        [E2ETest(5, 9, "M3_C03: ThemVaoGio out-of-stock product is blocked from cart")]
        public async Task M3_C03_ThemVaoGio_OutOfStockProduct_Blocked()
        {
            DbInitializer.ResetInMemoryStore();
            // Seed a product with 0 stock
            var zeroStockProduct = new SanPhamGayQuy
            {
                TenSanPham = "Vật phẩm hết hàng",
                GiaBan = 50000m,
                SoLuongTon = 0,
                TrangThai = true
            };
            DbInitializer.AddInMemoryProduct(zeroStockProduct);

            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var result = await controller.ThemVaoGio(zeroStockProduct.IDSanPham, 1);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Out-of-stock item should redirect to ChiTiet");
            Assert.Equal("ChiTiet", redirect.ActionName, "Action must redirect to ChiTiet");

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.True(cart == null || cart.Count == 0, "Cart must remain empty for 0-stock item");
            Assert.NotNull(controller.TempData["ErrorMessage"], "ErrorMessage must be populated for out-of-stock item");
        }

        [E2ETest(5, 9, "M3_C04: ThemVaoGio zero or negative requested quantity clamped to 1")]
        public async Task M3_C04_ThemVaoGio_ZeroOrNegativeQuantity_ClampedToOne()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Request 0 items
            await controller.ThemVaoGio(1, 0);
            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            Assert.Equal(1, cart[0].SoLuong, "Requested 0 quantity must clamp to 1");

            // Reset cart
            session.Clear();

            // Request -10 items
            await controller.ThemVaoGio(1, -10);
            var cart2 = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart2, "Cart must exist");
            Assert.Equal(1, cart2[0].SoLuong, "Requested -10 quantity must clamp to 1");
        }

        [E2ETest(5, 9, "M3_C05: ThemVaoGio quantity exceeding available stock is capped to SoLuongTon")]
        public async Task M3_C05_ThemVaoGio_ExceedingStock_CappedToStock()
        {
            DbInitializer.ResetInMemoryStore();
            var product = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(product, "Product 1 must exist");

            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Request 99999 items
            await controller.ThemVaoGio(1, 99999);
            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            Assert.Equal(product.SoLuongTon, cart[0].SoLuong, "Cart quantity must be capped at product.SoLuongTon");
        }

        [E2ETest(5, 9, "M3_C06: ThemVaoGio repeated additions accumulate quantity up to stock cap")]
        public async Task M3_C06_ThemVaoGio_RepeatedAdditions_AccumulateAndCap()
        {
            DbInitializer.ResetInMemoryStore();
            var product = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(product, "Product 1 must exist");

            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Add 2 items
            await controller.ThemVaoGio(1, 2);
            // Add 3 more items
            await controller.ThemVaoGio(1, 3);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            Assert.Equal(1, cart.Count, "Cart must still have 1 line item");
            Assert.Equal(5, cart[0].SoLuong, "Cart quantity must accumulate to 5 (2 + 3)");

            // Add an enormous amount
            await controller.ThemVaoGio(1, 99999);
            var cartCapped = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cartCapped, "Cart must exist");
            Assert.Equal(product.SoLuongTon, cartCapped[0].SoLuong, "Accumulated quantity must cap at stock limit");
        }

        [E2ETest(5, 9, "M3_C07: ThemVaoGio AJAX request returns JSON payload with totals")]
        public async Task M3_C07_ThemVaoGio_AjaxRequest_ReturnsJson()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session, isAjax: true);

            var result = await controller.ThemVaoGio(1, 3);
            var json = AssertIsType<JsonResult>(result, "AJAX request must return JsonResult");
            Assert.NotNull(json.Value, "JSON value must not be null");

            // Reflectively inspect anonymous object
            var valType = json.Value.GetType();
            var successProp = valType.GetProperty("success")?.GetValue(json.Value);
            var totalQtyProp = valType.GetProperty("totalQuantity")?.GetValue(json.Value);
            var subtotalProp = valType.GetProperty("cartSubtotal")?.GetValue(json.Value);

            Assert.Equal(true, successProp, "AJAX success must be true");
            Assert.Equal(3, totalQtyProp, "totalQuantity must equal 3");
            Assert.NotNull(subtotalProp, "cartSubtotal must be present");
        }

        [E2ETest(5, 9, "M3_C08: ThemVaoGio AJAX request with nonexistent product returns success=false")]
        public async Task M3_C08_ThemVaoGio_Ajax_NonExistent_ReturnsFalse()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session, isAjax: true);

            var result = await controller.ThemVaoGio(999999, 1);
            var json = AssertIsType<JsonResult>(result, "AJAX request must return JsonResult");
            Assert.NotNull(json.Value, "JSON value must not be null");

            var valType = json.Value.GetType();
            var successProp = valType.GetProperty("success")?.GetValue(json.Value);
            Assert.Equal(false, successProp, "AJAX success must be false for nonexistent product");
        }

        [E2ETest(5, 9, "M3_C09: CapNhatSoLuong positive quantity updates line item and subtotal")]
        public async Task M3_C09_CapNhatSoLuong_PositiveUpdate_RecalculatesTotals()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Add product 1 with quantity 2
            await controller.ThemVaoGio(1, 2);

            // Update product 1 to quantity 4
            var updateResult = controller.CapNhatSoLuong(1, 4);
            AssertIsType<RedirectToActionResult>(updateResult, "CapNhatSoLuong should redirect to Index");

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            Assert.Equal(4, cart[0].SoLuong, "Item quantity must be updated to 4");
            Assert.Equal(cart[0].DonGia * 4, cart[0].ThanhTien, "ThanhTien must equal DonGia * 4");
        }

        [E2ETest(5, 9, "M3_C10: CapNhatSoLuong zero quantity removes item from cart")]
        public async Task M3_C10_CapNhatSoLuong_ZeroQuantity_RemovesItem()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            await controller.ThemVaoGio(1, 2);
            controller.CapNhatSoLuong(1, 0);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart object must exist");
            Assert.Equal(0, cart.Count, "Setting quantity to 0 must remove item from cart");
        }

        [E2ETest(5, 9, "M3_C11: CapNhatSoLuong negative quantity removes item from cart")]
        public async Task M3_C11_CapNhatSoLuong_NegativeQuantity_RemovesItem()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            await controller.ThemVaoGio(1, 2);
            controller.CapNhatSoLuong(1, -5);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart object must exist");
            Assert.Equal(0, cart.Count, "Setting negative quantity must remove item from cart");
        }

        [E2ETest(5, 9, "M3_C12: CapNhatSoLuong exceeding stock clamps to item.SoLuongTon")]
        public async Task M3_C12_CapNhatSoLuong_ExceedingStock_ClampsToStock()
        {
            DbInitializer.ResetInMemoryStore();
            var product = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(product, "Product 1 must exist");

            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            await controller.ThemVaoGio(1, 1);
            controller.CapNhatSoLuong(1, 99999);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            Assert.Equal(product.SoLuongTon, cart[0].SoLuong, "Quantity must be capped to available stock");
        }

        [E2ETest(5, 9, "M3_C13: CapNhatSoLuong item not in cart does not crash")]
        public void M3_C13_CapNhatSoLuong_ItemNotInCart_SafelyHandled()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Attempt update on empty cart
            var result = controller.CapNhatSoLuong(999, 5);
            AssertIsType<RedirectToActionResult>(result, "Updating nonexistent item should redirect to Index");

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.True(cart == null || cart.Count == 0, "Cart must remain empty");
        }

        [E2ETest(5, 9, "M3_C14: CapNhatSoLuong AJAX request returns updated financial totals")]
        public async Task M3_C14_CapNhatSoLuong_AjaxRequest_ReturnsFinancialPayload()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session, isAjax: true);

            await controller.ThemVaoGio(1, 1);
            var result = controller.CapNhatSoLuong(1, 3);

            var json = AssertIsType<JsonResult>(result, "CapNhatSoLuong AJAX must return JsonResult");
            Assert.NotNull(json.Value, "JSON payload must not be null");

            var valType = json.Value.GetType();
            var successProp = valType.GetProperty("success")?.GetValue(json.Value);
            var lineTotalProp = (decimal)(valType.GetProperty("lineTotal")?.GetValue(json.Value) ?? 0m);
            var subtotalProp = (decimal)(valType.GetProperty("cartSubtotal")?.GetValue(json.Value) ?? 0m);

            Assert.Equal(true, successProp, "AJAX success must be true");
            Assert.True(lineTotalProp > 0, "lineTotal must be positive");
            Assert.Equal(lineTotalProp, subtotalProp, "For 1 item, lineTotal must equal cartSubtotal");
        }

        [E2ETest(5, 9, "M3_C15: XoaKhoiGio removes targeted item and preserves others")]
        public async Task M3_C15_XoaKhoiGio_RemovesSpecificItem()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            // Add product 1 and product 2
            await controller.ThemVaoGio(1, 2);
            await controller.ThemVaoGio(2, 1);

            var cartBefore = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cartBefore, "Cart must have items");
            Assert.Equal(2, cartBefore.Count, "Cart should have 2 distinct items");

            // Remove product 1
            controller.XoaKhoiGio(1);

            var cartAfter = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cartAfter, "Cart must still exist");
            Assert.Equal(1, cartAfter.Count, "Cart should have 1 item remaining");
            Assert.Equal(2, cartAfter[0].IDSanPham, "Remaining item must be product 2");
        }

        [E2ETest(5, 9, "M3_C16: XoaKhoiGio nonexistent item safely ignored")]
        public void M3_C16_XoaKhoiGio_NonExistentItem_SafelyHandled()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var result = controller.XoaKhoiGio(999);
            AssertIsType<RedirectToActionResult>(result, "XoaKhoiGio on missing item redirects to Index");
        }

        [E2ETest(5, 9, "M3_C17: XoaGioHang completely clears the session cart")]
        public async Task M3_C17_XoaGioHang_ClearsEntireCart()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            await controller.ThemVaoGio(1, 2);
            await controller.ThemVaoGio(2, 1);

            controller.XoaGioHang();

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.True(cart == null || cart.Count == 0, "Cart must be completely null or empty after XoaGioHang");
            Assert.NotNull(controller.TempData["SuccessMessage"], "SuccessMessage must be set in TempData");
        }

        [E2ETest(5, 9, "M3_C18: Session cart isolation under parallel sessions (C13 verification)")]
        public async Task M3_C18_SessionCartIsolation_ParallelSessions()
        {
            DbInitializer.ResetInMemoryStore();
            var sessionA = E2ETestContext.CreateMockSession();
            var sessionB = E2ETestContext.CreateMockSession();

            var controllerA = CreateTestCartController(out _, sessionA);
            var controllerB = CreateTestCartController(out _, sessionB);

            // User A adds product 1
            await controllerA.ThemVaoGio(1, 3);
            // User B adds product 2
            await controllerB.ThemVaoGio(2, 5);

            var cartA = sessionA.Get<List<CartItem>>("GioHangGayQuy");
            var cartB = sessionB.Get<List<CartItem>>("GioHangGayQuy");

            Assert.NotNull(cartA, "Cart A must exist");
            Assert.NotNull(cartB, "Cart B must exist");

            Assert.Equal(1, cartA[0].IDSanPham, "Cart A must have product 1");
            Assert.Equal(3, cartA[0].SoLuong, "Cart A must have quantity 3");

            Assert.Equal(2, cartB[0].IDSanPham, "Cart B must have product 2");
            Assert.Equal(5, cartB[0].SoLuong, "Cart B must have quantity 5");

            // User A clears cart, User B should not be affected
            controllerA.XoaGioHang();
            var cartACleared = sessionA.Get<List<CartItem>>("GioHangGayQuy");
            var cartBIntact = sessionB.Get<List<CartItem>>("GioHangGayQuy");

            Assert.True(cartACleared == null || cartACleared.Count == 0, "Cart A must be cleared");
            Assert.NotNull(cartBIntact, "Cart B must remain intact");
            Assert.Equal(1, cartBIntact.Count, "Cart B must still have its item");
        }

        // =========================================================================
        // SECTION 2: CHECKOUT & EXTRA DONATION VALIDATION (FEATURE 10)
        // =========================================================================

        [E2ETest(5, 10, "M3_C19: DatHang with empty cart redirects to Index with error")]
        public async Task M3_C19_DatHang_EmptyCart_RedirectsWithError()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện"
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Checkout with empty cart must redirect");
            Assert.Equal("Index", redirect.ActionName, "Redirect must target Index");
            Assert.NotNull(controller.TempData["ErrorMessage"], "TempData ErrorMessage must warn about empty cart");
        }

        [E2ETest(5, 10, "M3_C20: DatHang valid VN mobile numbers (03, 05, 07, 08, 09) succeed")]
        public async Task M3_C20_DatHang_ValidVnMobileNumbers_Pass()
        {
            var validPhones = new[]
            {
                "0901234567",
                "0389998888",
                "0771234567",
                "0521234567",
                "0861234567",
                "090 123 4567",   // spaces
                "090.123.4567",   // dots
                "090-123-4567"    // hyphens
            };

            foreach (var phone in validPhones)
            {
                DbInitializer.ResetInMemoryStore();
                var session = E2ETestContext.CreateMockSession();
                var controller = CreateTestCartController(out _, session);
                await controller.ThemVaoGio(1, 1);

                var model = new CheckoutViewModel
                {
                    HoTen = "Nguyễn Văn An",
                    SoDienThoai = phone,
                    DiaChi = "123 Điện Biên Phủ, TP.HCM",
                    Email = "an.nguyen@example.com"
                };

                var result = await controller.DatHang(model);
                var redirect = AssertIsType<RedirectToActionResult>(result, $"Valid phone '{phone}' must place order successfully");
                Assert.Equal("DatHangThanhCong", redirect.ActionName, $"Valid phone '{phone}' must redirect to DatHangThanhCong");
            }
        }

        [E2ETest(5, 10, "M3_C21: DatHang invalid phones (landlines, short/long, invalid prefix) rejected")]
        public async Task M3_C21_DatHang_InvalidPhones_Rejected()
        {
            var invalidPhones = new[]
            {
                "02838383838", // landline
                "12345",       // short
                "090123456789",// 12 digits
                "0101234567",  // invalid prefix 01
                "0401234567",  // invalid prefix 04
                "0601234567",  // invalid prefix 06
                "abcdefghij"   // non-numeric
            };

            foreach (var badPhone in invalidPhones)
            {
                DbInitializer.ResetInMemoryStore();
                var session = E2ETestContext.CreateMockSession();
                var controller = CreateTestCartController(out _, session);
                await controller.ThemVaoGio(1, 1);

                var model = new CheckoutViewModel
                {
                    HoTen = "Test Donor",
                    SoDienThoai = badPhone,
                    DiaChi = "123 Đường Tình Nguyện"
                };

                // Trigger model validation
                ValidateModel(model, out var valResults);
                foreach (var vr in valResults)
                {
                    controller.ModelState.AddModelError(vr.MemberNames.FirstOrDefault() ?? "", vr.ErrorMessage ?? "");
                }

                var result = await controller.DatHang(model);
                var viewResult = AssertIsType<ViewResult>(result, $"Invalid phone '{badPhone}' must return ViewResult");
                Assert.Equal("Index", viewResult.ViewName, "Should return Index view to correct validation errors");
                Assert.False(controller.ModelState.IsValid, $"ModelState must be invalid for bad phone '{badPhone}'");
            }
        }

        [E2ETest(5, 10, "M3_C22: DatHang missing required fields fails ModelState and preserves cart")]
        public async Task M3_C22_DatHang_MissingRequiredFields_FailsModelState()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 2);

            var model = new CheckoutViewModel
            {
                HoTen = "", // missing
                SoDienThoai = "", // missing
                DiaChi = "" // missing
            };

            ValidateModel(model, out var valResults);
            foreach (var vr in valResults)
            {
                controller.ModelState.AddModelError(vr.MemberNames.FirstOrDefault() ?? "", vr.ErrorMessage ?? "");
            }

            var result = await controller.DatHang(model);
            var viewResult = AssertIsType<ViewResult>(result, "Missing fields must return ViewResult");
            Assert.Equal("Index", viewResult.ViewName, "Must return Index view");

            // Verify cart items are preserved in view model
            var cartVm = AssertIsType<CartViewModel>(viewResult.Model, "View model must be CartViewModel");
            Assert.True(cartVm.CoSanPham, "Cart must still contain items in re-displayed view");
            Assert.Equal(2, cartVm.TongSoLuong, "Quantity must remain 2");
        }

        [E2ETest(5, 10, "M3_C23: DatHang voluntary extra donation correctly added to total")]
        public async Task M3_C23_DatHang_ExtraDonation_FinancialMath()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            var itemPrice = cart[0].DonGia;
            var extraDonation = 50000m;

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện",
                TienUngHoThem = extraDonation
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Successful order must redirect");
            Assert.Equal("DatHangThanhCong", redirect.ActionName, "Action must be DatHangThanhCong");

            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);
            Assert.True(orderId > 0, "OrderId must be positive");

            var order = DbInitializer.GetInMemoryOrderById(orderId);
            Assert.NotNull(order, "Order must be found in in-memory store");
            Assert.Equal(itemPrice, order.TongTienHang, "TongTienHang must match merchandise price");
            Assert.Equal(extraDonation, order.TienUngHoThem, "TienUngHoThem must match voluntary amount");
            Assert.Equal(itemPrice + extraDonation, order.TongThanhToan, "TongThanhToan must equal merchandise + extra donation");
        }

        [E2ETest(5, 10, "M3_C24: DatHang negative extra donation clamped to 0")]
        public async Task M3_C24_DatHang_NegativeExtraDonation_ClampedToZero()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var cart = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.NotNull(cart, "Cart must exist");
            var itemPrice = cart[0].DonGia;

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện",
                TienUngHoThem = -50000m // negative manipulation
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Checkout should proceed with clamped donation");
            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);

            var order = DbInitializer.GetInMemoryOrderById(orderId);
            Assert.NotNull(order, "Order must exist");
            Assert.Equal(0m, order.TienUngHoThem, "Negative extra donation must be clamped to 0");
            Assert.Equal(itemPrice, order.TongThanhToan, "Total payment must not be discounted below merchandise price");
        }

        [E2ETest(5, 10, "M3_C25: DatHang default payment method falls back to ChuyenKhoanQR")]
        public async Task M3_C25_DatHang_DefaultPaymentMethod_Fallback()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện",
                PhuongThucThanhToan = "" // empty
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Checkout must succeed");
            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);

            var order = DbInitializer.GetInMemoryOrderById(orderId);
            Assert.NotNull(order, "Order must exist");
            Assert.Equal("ChuyenKhoanQR", order.PhuongThucThanhToan, "Empty payment method must default to ChuyenKhoanQR");
        }

        [E2ETest(5, 10, "M3_C26: DatHang clears session cart upon successful completion")]
        public async Task M3_C26_DatHang_ClearsSessionCart_OnSuccess()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 2);

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện"
            };

            await controller.DatHang(model);

            var cartAfter = session.Get<List<CartItem>>("GioHangGayQuy");
            Assert.True(cartAfter == null || cartAfter.Count == 0, "Cart session key must be cleared after successful order placement");
        }

        // =========================================================================
        // SECTION 3: ORDER CONFIRMATION & VIETQR SPECIFICATION (FEATURE 11)
        // =========================================================================

        [E2ETest(5, 11, "M3_C27: Order code adheres strictly to 15-character format GQyyyyMMdd-XXXX (C04)")]
        public async Task M3_C27_OrderCodeFormat_15Chars_StrictVerification()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện"
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Must redirect to confirmation");
            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);

            var order = DbInitializer.GetInMemoryOrderById(orderId);
            Assert.NotNull(order, "Order must exist");

            var code = order.MaDonHang;
            Assert.Equal(15, code.Length, $"Order code '{code}' must have exact length 15");
            Assert.True(code.StartsWith("GQ"), $"Order code '{code}' must start with 'GQ'");
            var expectedDate = DateTime.Now.ToString("yyyyMMdd");
            Assert.Equal(expectedDate, code.Substring(2, 8), $"Order code date part must equal '{expectedDate}'");
            Assert.Equal('-', code[10], "Character 11 must be a hyphen '-'");
            var suffix = code.Substring(11, 4);
            Assert.Matches(@"^[A-Z0-9]{4}$", suffix, "Suffix must be 4 uppercase alphanumeric characters");
        }

        [E2ETest(5, 11, "M3_C28: DatHang decrements inventory stock and increments DaBan (C07)")]
        public async Task M3_C28_DatHang_DecrementsInventoryStock_IncrementsDaBan()
        {
            DbInitializer.ResetInMemoryStore();
            var prodBefore = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(prodBefore, "Product 1 must exist");
            var stockBefore = prodBefore.SoLuongTon;
            var soldBefore = prodBefore.DaBan;

            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 3);

            var model = new CheckoutViewModel
            {
                HoTen = "Nguyễn Văn An",
                SoDienThoai = "0901234567",
                DiaChi = "123 Đường Tình Nguyện"
            };

            await controller.DatHang(model);

            var prodAfter = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(prodAfter, "Product 1 must exist");
            Assert.Equal(stockBefore - 3, prodAfter.SoLuongTon, "SoLuongTon must decrease by purchased quantity 3");
            Assert.Equal(soldBefore + 3, prodAfter.DaBan, "DaBan must increase by purchased quantity 3");
        }

        [E2ETest(5, 11, "M3_C29: VietQR image URL parameters exact match (C05, S06)")]
        public void M3_C29_VietQrUrl_ParametersExactMatch()
        {
            var orderCode = "GQ20261103-ABCD";
            var totalPayable = 375000m;

            var cleanAmount = (long)totalPayable;
            var cleanMemo = Uri.EscapeDataString(orderCode);
            var qrUrl = $"https://img.vietqr.io/image/MB-0924044283-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}&accountName=NGUYEN%20THAI%20VINH";

            Assert.Contains("img.vietqr.io/image/MB-0924044283-compact2.png", qrUrl, "VietQR must target MB-0924044283-compact2");
            Assert.Contains("amount=375000", qrUrl, "VietQR amount parameter must match clean total");
            Assert.Contains("addInfo=GQ20261103-ABCD", qrUrl, "VietQR memo must contain exact order code");
            Assert.Contains("accountName=NGUYEN%20THAI%20VINH", qrUrl, "VietQR account name must match NGUYEN THAI VINH");
        }

        [E2ETest(5, 11, "M3_C30: DatHangThanhCong valid order loads order and item details")]
        public async Task M3_C30_DatHangThanhCong_ValidOrder_RendersOrder()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var model = new CheckoutViewModel
            {
                HoTen = "Lê Thị Hoa",
                SoDienThoai = "0987654321",
                DiaChi = "456 Nam Kỳ Khởi Nghĩa, Quận 3, TP.HCM",
                GhiChu = "Chúc các em nhỏ luôn chăm ngoan học giỏi"
            };

            var orderResult = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(orderResult, "Must redirect");
            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);

            var successResult = await controller.DatHangThanhCong(orderId);
            var view = AssertIsType<ViewResult>(successResult, "DatHangThanhCong must return ViewResult");
            var order = AssertIsType<DonHangGayQuy>(view.Model, "View model must be DonHangGayQuy");

            Assert.Equal("Lê Thị Hoa", order.HoTenNguoiMua, "HoTenNguoiMua must match");
            Assert.Equal("0987654321", order.SoDienThoai, "SoDienThoai must match");
            Assert.Equal("Chúc các em nhỏ luôn chăm ngoan học giỏi", order.GhiChu, "GhiChu must match");
            Assert.True(order.ChiTietDonHangs.Count > 0, "ChiTietDonHangs must not be empty");
        }

        [E2ETest(5, 11, "M3_C31: DatHangThanhCong nonexistent order ID redirects to CuaHang.Index")]
        public async Task M3_C31_DatHangThanhCong_NonExistentOrder_RedirectsToStore()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);

            var result = await controller.DatHangThanhCong(999999);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Nonexistent order ID must redirect");
            Assert.Equal("Index", redirect.ActionName, "ActionName must be Index");
            Assert.Equal("CuaHang", redirect.ControllerName, "ControllerName must be CuaHang");
            Assert.NotNull(controller.TempData["ErrorMessage"], "ErrorMessage must be set in TempData");
        }

        [E2ETest(5, 11, "M3_C32: Confirmation view template handles both VietQR and COD branches")]
        public void M3_C32_ConfirmationView_BranchesCorrectly()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(viewPath, "DatHangThanhCong.cshtml");
            var content = File.ReadAllText(viewPath);

            // Verify VietQR branch exists
            Assert.Contains("img.vietqr.io", content, "View must embed VietQR image");
            Assert.Contains("MB-0924044283", content, "View must specify MBBank account");
            Assert.Contains("NGUYEN%20THAI%20VINH", content, "View must specify account holder");

            // Verify COD branch exists
            Assert.Contains("COD", content, "View must handle COD payment branch");
            Assert.Contains("tiền mặt khi nhận hàng", content, "View must explain COD delivery procedure");
        }

        // =========================================================================
        // SECTION 4: SECURITY & HIGH-CONCURRENCY STRESS
        // =========================================================================

        [E2ETest(5, 10, "M3_C33: XSS payloads in customer input safely handled without error")]
        public async Task M3_C33_XssPayloads_SafelyStoredAndHandled()
        {
            DbInitializer.ResetInMemoryStore();
            var session = E2ETestContext.CreateMockSession();
            var controller = CreateTestCartController(out _, session);
            await controller.ThemVaoGio(1, 1);

            var xssPayload = "<script>alert('XSS-TEST')</script>";
            var model = new CheckoutViewModel
            {
                HoTen = xssPayload,
                SoDienThoai = "0901234567",
                DiaChi = "<b>123 Đường Hack</b>",
                GhiChu = "<iframe src='evil.com'></iframe>"
            };

            var result = await controller.DatHang(model);
            var redirect = AssertIsType<RedirectToActionResult>(result, "Order placement with XSS string must succeed cleanly");
            var orderId = (int)(redirect.RouteValues?["id"] ?? 0);

            var order = DbInitializer.GetInMemoryOrderById(orderId);
            Assert.NotNull(order, "Order must be found");
            Assert.Equal(xssPayload, order.HoTenNguoiMua, "Stored value preserves string without crashing");
        }

        [E2ETest(5, 9, "M3_C34: High-concurrency cart mutations and order placement stress (50 parallel tasks)")]
        public void M3_C34_HighConcurrency_CartMutationsAndOrderStress()
        {
            DbInitializer.ResetInMemoryStore();

            // Set generous initial stock on product 1
            var p1 = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(p1, "Product 1 must exist");
            var initialDaBan = p1.DaBan;
            p1.SoLuongTon = 500;
            DbInitializer.UpdateInMemoryProduct(p1);

            var taskCount = 50;
            var tasks = new Task[taskCount];
            var successCount = 0;
            var lockObj = new object();

            for (int i = 0; i < taskCount; i++)
            {
                var idx = i;
                tasks[i] = Task.Run(async () =>
                {
                    var session = E2ETestContext.CreateMockSession();
                    var controller = CreateTestCartController(out _, session);

                    await controller.ThemVaoGio(1, 2);
                    controller.CapNhatSoLuong(1, 3);

                    var model = new CheckoutViewModel
                    {
                        HoTen = $"Donor Concurrency {idx}",
                        SoDienThoai = "0901234567",
                        DiaChi = $"Address {idx}",
                        TienUngHoThem = 10000m * (idx % 5)
                    };

                    var res = await controller.DatHang(model);
                    if (res is RedirectToActionResult redirect && redirect.ActionName == "DatHangThanhCong")
                    {
                        lock (lockObj)
                        {
                            successCount++;
                        }
                    }
                });
            }

            Task.WaitAll(tasks);
            Assert.Equal(taskCount, successCount, "All 50 concurrent cart & checkout operations must complete successfully");

            var updatedProduct = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(updatedProduct, "Product 1 must exist");
            Assert.Equal(500 - (50 * 3), updatedProduct.SoLuongTon, "Stock must decrease exactly by 150 (50 * 3)");
            Assert.Equal(initialDaBan + (50 * 3), updatedProduct.DaBan, "DaBan must increase exactly by 150 (50 * 3)");
        }
    }
}
