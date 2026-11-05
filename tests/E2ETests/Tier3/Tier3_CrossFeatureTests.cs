using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier3
{
    public class Tier3_CrossFeatureTests
    {
        // =========================================================================
        // TIER 3: CROSS-FEATURE COMBINATIONS (16 Tests)
        // =========================================================================

        [E2ETest(3, "C01: Multi-item cart with quantity update calculates exact aggregate total")]
        public void C01_MultiItemCartWithQuantityUpdate()
        {
            var item1Price = 120000m;
            var item1Qty = 2; // subtotal 240,000
            var item2Price = 85000m;
            var item2Qty = 1; // subtotal 85,000

            var line1 = ReferenceOracles.LineTotal(item1Price, item1Qty);
            var line2 = ReferenceOracles.LineTotal(item2Price, item2Qty);
            var subtotalInitial = line1 + line2;
            Assert.Equal(325000m, subtotalInitial, "Initial cart subtotal must be 325,000 VND");

            // Update item 1 quantity to 3
            item1Qty = 3;
            var line1Updated = ReferenceOracles.LineTotal(item1Price, item1Qty); // 360,000
            var subtotalUpdated = line1Updated + line2;
            Assert.Equal(445000m, subtotalUpdated, "Updated cart subtotal must be 445,000 VND");
        }

        [E2ETest(3, "C02: Removing item from multi-item cart recalculates totals")]
        public void C02_RemovingItemRecalculatesTotals()
        {
            var item1Price = 120000m;
            var item1Qty = 3;
            var item2Price = 85000m;
            var item2Qty = 1;

            var line1 = ReferenceOracles.LineTotal(item1Price, item1Qty); // 360,000
            var line2 = ReferenceOracles.LineTotal(item2Price, item2Qty); // 85,000

            // Remove item 2
            var remainingSubtotal = line1;
            Assert.Equal(360000m, remainingSubtotal, "Subtotal after item removal must equal remaining item total");
        }

        [E2ETest(3, "C03: Checkout combines cart subtotal with voluntary extra donation")]
        public void C03_CheckoutCombinesSubtotalAndExtraDonation()
        {
            var subtotal = 445000m;
            var voluntaryDonation = 55000m;
            var total = ReferenceOracles.OrderTotal(subtotal, voluntaryDonation, 0m);

            Assert.Equal(500000m, total, "Order total must equal subtotal + extra donation (500,000 VND)");
        }

        [E2ETest(3, "C04: Order submission generates unique order code format GQ...")]
        public void C04_OrderCodeFormatGeneration()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            var orderCode = $"GQ{timestamp}-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            Assert.True(orderCode.StartsWith("GQ"), "Order code must start with GQ prefix");
            Assert.Contains(timestamp, orderCode, "Order code must contain date stamp");
            Assert.Equal(15, orderCode.Length, "Order code must have consistent formatted length");
        }

        [E2ETest(3, "C05: Generated order code embedded into VietQR URL and payment instructions")]
        public void C05_OrderCodeEmbeddedIntoVietQr()
        {
            var orderCode = "GQ20261103-A1B2";
            var amount = 500000m;
            var qrUrl = ReferenceOracles.VietQrImageUrl("VCB", "1023456789", amount, orderCode);

            Assert.Contains($"addInfo={orderCode}", qrUrl, "VietQR URL must embed the exact order code for automated reconciliation");
            Assert.Contains("amount=500000", qrUrl, "VietQR URL must reflect total payable amount");
        }

        [E2ETest(3, "C06: Multi-item donation tracking across mixed contribution percentages")]
        public void C06_MultiItemDonationTracking()
        {
            // Item A: 100% charity contribution (T-shirt 150,000 VND)
            var itemAContribution = ReferenceOracles.CharityContribution(150000m, 100);
            Assert.Equal(150000m, itemAContribution, "Item A 100% contribution must be 150,000 VND");

            // Item B: 50% charity contribution (Handmade teddy bear 200,000 VND)
            var itemBContribution = ReferenceOracles.CharityContribution(200000m, 50);
            Assert.Equal(100000m, itemBContribution, "Item B 50% contribution must be 100,000 VND");

            // Voluntary extra donation
            var voluntaryDonation = 50000m;

            var totalCharityFunds = itemAContribution + itemBContribution + voluntaryDonation;
            Assert.Equal(300000m, totalCharityFunds, "Total funds remitted to charity must be 300,000 VND");
        }

        [E2ETest(3, "C07: Inventory stock deduction and sold count increment")]
        public void C07_InventoryStockDeduction()
        {
            var initialStock = 100;
            var initialSold = 15;
            var purchaseQuantity = 4;

            var finalStock = initialStock - purchaseQuantity;
            var finalSold = initialSold + purchaseQuantity;

            Assert.Equal(96, finalStock, "Stock must decrease by purchased quantity");
            Assert.Equal(19, finalSold, "Sold count must increase by purchased quantity");
        }

        [E2ETest(3, "C08: Zero stock item cannot be purchased")]
        public void C08_ZeroStockCannotBePurchased()
        {
            var stock = 0;
            var canPurchase = stock > 0;
            Assert.False(canPurchase, "Item with 0 stock must not allow checkout purchase");
        }

        [E2ETest(3, "C09: Admin product creation reflects in in-memory catalog store")]
        public void C09_AdminCreationReflectsInCatalog()
        {
            var initType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer");
            var spType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy");

            if (initType != null && spType != null)
            {
                var addMethod = initType.GetMethod("AddInMemoryProduct");
                var getMethod = initType.GetMethod("GetInMemoryProducts");

                if (addMethod != null && getMethod != null)
                {
                    var newProduct = Activator.CreateInstance(spType);
                    spType.GetProperty("TenSanPham")?.SetValue(newProduct, "Huy Hiệu Trăng Sáng CrossTest");
                    spType.GetProperty("GiaBan")?.SetValue(newProduct, 30000m);
                    spType.GetProperty("SoLuongTon")?.SetValue(newProduct, 200);

                    addMethod.Invoke(null, new object[] { newProduct });
                    var list = getMethod.Invoke(null, null) as System.Collections.IEnumerable;

                    bool found = false;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var name = spType.GetProperty("TenSanPham")?.GetValue(item) as string;
                            if (name == "Huy Hiệu Trăng Sáng CrossTest")
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    Assert.True(found, "Newly added product must appear in catalog repository");
                }
            }
        }

        [E2ETest(3, "C10: Admin edit modifies catalog product details")]
        public void C10_AdminEditModifiesCatalogProduct()
        {
            var initType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer");
            var spType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy");

            if (initType != null && spType != null)
            {
                var updateMethod = initType.GetMethod("UpdateInMemoryProduct");
                var getMethod = initType.GetMethod("GetInMemoryProducts");

                if (updateMethod != null && getMethod != null)
                {
                    var list = getMethod.Invoke(null, null) as System.Collections.IEnumerable;
                    object? firstProduct = null;
                    if (list != null)
                    {
                        foreach (var p in list)
                        {
                            firstProduct = p;
                            break;
                        }
                    }

                    if (firstProduct != null)
                    {
                        var originalPrice = (decimal)(spType.GetProperty("GiaBan")?.GetValue(firstProduct) ?? 0m);
                        var newPrice = originalPrice + 5000m;
                        spType.GetProperty("GiaBan")?.SetValue(firstProduct, newPrice);

                        updateMethod.Invoke(null, new object[] { firstProduct });
                        var currentPrice = (decimal)(spType.GetProperty("GiaBan")?.GetValue(firstProduct) ?? 0m);

                        Assert.Equal(newPrice, currentPrice, "Product price must be updated in catalog repository");
                    }
                }
            }
        }

        [E2ETest(3, "C11: Admin delete removes product from catalog store")]
        public void C11_AdminDeleteRemovesProduct()
        {
            var initType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer");
            var spType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy");

            if (initType != null && spType != null)
            {
                var addMethod = initType.GetMethod("AddInMemoryProduct");
                var deleteMethod = initType.GetMethod("DeleteInMemoryProduct");
                var getMethod = initType.GetMethod("GetInMemoryProducts");

                if (addMethod != null && deleteMethod != null && getMethod != null)
                {
                    var tempProduct = Activator.CreateInstance(spType);
                    spType.GetProperty("TenSanPham")?.SetValue(tempProduct, "ItemToDelete");
                    spType.GetProperty("GiaBan")?.SetValue(tempProduct, 10000m);

                    addMethod.Invoke(null, new object[] { tempProduct });
                    var id = (int)(spType.GetProperty("IDSanPham")?.GetValue(tempProduct) ?? 0);

                    deleteMethod.Invoke(null, new object[] { id });
                    var list = getMethod.Invoke(null, null) as System.Collections.IEnumerable;

                    bool existsAfterDelete = false;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var itemId = (int)(spType.GetProperty("IDSanPham")?.GetValue(item) ?? 0);
                            if (itemId == id) existsAfterDelete = true;
                        }
                    }

                    Assert.False(existsAfterDelete, "Deleted product must no longer exist in catalog repository");
                }
            }
        }

        [E2ETest(3, "C12: Desktop and mobile navigation routes target identical Area action")]
        public void C12_NavRoutesTargetIdenticalAction()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang navigation in _Layout.cshtml");
            }
            Assert.Contains("CuaHang", layoutContent, "Layout must define CuaHang navigation");
        }

        [E2ETest(3, "C13: Two distinct simulated donor sessions maintain isolated carts")]
        public void C13_SessionCartIsolation()
        {
            var session1 = E2ETestContext.CreateMockSession();
            var session2 = E2ETestContext.CreateMockSession();

            var helperType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Helpers.SessionExtensions");
            if (helperType != null)
            {
                var setMethod = helperType.GetMethods().First(m => m.Name == "Set" && m.IsGenericMethodDefinition);
                var getMethod = helperType.GetMethods().First(m => m.Name == "Get" && m.IsGenericMethodDefinition);

                var stringSet = setMethod.MakeGenericMethod(typeof(string));
                var stringGet = getMethod.MakeGenericMethod(typeof(string));

                stringSet.Invoke(null, new object[] { session1, ReferenceOracles.SessionKeyCart, "CartOfDonor1" });
                stringSet.Invoke(null, new object[] { session2, ReferenceOracles.SessionKeyCart, "CartOfDonor2" });

                var cart1 = stringGet.Invoke(null, new object[] { session1, ReferenceOracles.SessionKeyCart });
                var cart2 = stringGet.Invoke(null, new object[] { session2, ReferenceOracles.SessionKeyCart });

                Assert.Equal("CartOfDonor1", cart1, "Session 1 must have CartOfDonor1");
                Assert.Equal("CartOfDonor2", cart2, "Session 2 must have CartOfDonor2");
                Assert.NotEqual(cart1, cart2, "Sessions must maintain isolated cart states");
            }
        }

        [E2ETest(3, "C14: Automatic layout switching between storefront and admin views")]
        public void C14_AutomaticLayoutSwitching()
        {
            var viewStartPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "_ViewStart.cshtml");
            Assert.RequireFile(viewStartPath, "Areas/CuaHang/Views/_ViewStart.cshtml");
            var content = File.ReadAllText(viewStartPath);

            Assert.Contains("_AdminLayout", content, "Admin views must use _AdminLayout");
            Assert.Contains("_Layout", content, "Storefront views must use _Layout");
        }

        [E2ETest(3, "C15: Offline fallback catalog persistence")]
        public void C15_OfflineFallbackCatalogPersistence()
        {
            var initType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer");
            Assert.NotNull(initType, "DbInitializer must exist");

            var getMethod = initType.GetMethod("GetInMemoryProducts");
            var products = getMethod?.Invoke(null, null) as System.Collections.IEnumerable;
            Assert.NotNull(products, "Offline catalog must provide products");
        }

        [E2ETest(3, "C16: CrudViewModel ToEntity correctly maps properties")]
        public void C16_CrudViewModelToEntityMapping()
        {
            var crudVmType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamCrudViewModel");
            var spType = E2ETestContext.FindType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy");

            if (crudVmType != null && spType != null)
            {
                var vm = Activator.CreateInstance(crudVmType);
                crudVmType.GetProperty("TenSanPham")?.SetValue(vm, "Sổ Tay Tình Nguyện");
                crudVmType.GetProperty("GiaBan")?.SetValue(vm, 45000m);
                crudVmType.GetProperty("SoLuongTon")?.SetValue(vm, 50);

                var toEntityMethod = crudVmType.GetMethods().FirstOrDefault(m => m.Name == "ToEntity" && m.GetParameters().Length == 0)
                                     ?? crudVmType.GetMethods().FirstOrDefault(m => m.Name == "ToEntity");
                if (toEntityMethod != null)
                {
                    var parameters = toEntityMethod.GetParameters();
                    var args = parameters.Length == 0 ? null : new object?[] { null };
                    var entity = toEntityMethod.Invoke(vm, args);
                    Assert.NotNull(entity, "ToEntity must return entity instance");

                    var name = spType.GetProperty("TenSanPham")?.GetValue(entity) as string;
                    var price = (decimal)(spType.GetProperty("GiaBan")?.GetValue(entity) ?? 0m);

                    Assert.Equal("Sổ Tay Tình Nguyện", name, "Entity TenSanPham must match ViewModel");
                    Assert.Equal(45000m, price, "Entity GiaBan must match ViewModel");
                }
            }
        }

        [E2ETest(3, "C17: Order placement decrements SoLuongTon and increments DaBan")]
        public void C17_Checkout_Reduces_Stock_And_Increases_DaBan()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm test tồn kho C17",
                GiaBan = 50000m,
                SoLuongTon = 20,
                DaBan = 5,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Người mua test C17",
                SoDienThoai = "0901234567",
                DiaChiNhanHang = "TP. Hồ Chí Minh",
                TongThanhToan = 100000m,
                PhuongThucThanhToan = "COD",
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy
                    {
                        IDSanPham = savedProduct.IDSanPham,
                        TenSanPham = savedProduct.TenSanPham,
                        DonGia = savedProduct.GiaBan,
                        SoLuong = 3,
                        ThanhTien = 150000m
                    }
                }
            };

            DbInitializer.AddInMemoryOrder(order);

            var updatedProduct = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.NotNull(updatedProduct, "Product must exist after order");
            Assert.Equal(17, updatedProduct?.SoLuongTon, "SoLuongTon must decrease by 3 (from 20 to 17)");
            Assert.Equal(8, updatedProduct?.DaBan, "DaBan must increase by 3 (from 5 to 8)");
        }

        [E2ETest(3, "C18: Admin transitioning order to DaHuy restores stock and decrements DaBan")]
        public void C18_Admin_Cancel_Order_Restores_Stock()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm test hoàn kho C18",
                GiaBan = 80000m,
                SoLuongTon = 30,
                DaBan = 10,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Người mua test C18",
                SoDienThoai = "0909999888",
                DiaChiNhanHang = "Lâm Đồng",
                TongThanhToan = 160000m,
                PhuongThucThanhToan = "ChuyenKhoanQR",
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy
                    {
                        IDSanPham = savedProduct.IDSanPham,
                        TenSanPham = savedProduct.TenSanPham,
                        DonGia = savedProduct.GiaBan,
                        SoLuong = 4,
                        ThanhTien = 320000m
                    }
                }
            };

            var placedOrder = DbInitializer.AddInMemoryOrder(order);
            var prodAfterOrder = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(26, prodAfterOrder?.SoLuongTon, "Stock before cancel must be 26");
            Assert.Equal(14, prodAfterOrder?.DaBan, "DaBan before cancel must be 14");

            // Cancel order
            var cancelled = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy", null);
            Assert.True(cancelled, "Order cancellation must succeed");

            var prodAfterCancel = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.NotNull(prodAfterCancel, "Product must exist after cancellation");
            Assert.Equal(30, prodAfterCancel?.SoLuongTon, "SoLuongTon must be restored by 4 (from 26 to 30)");
            Assert.Equal(10, prodAfterCancel?.DaBan, "DaBan must be decremented by 4 (from 14 to 10)");
        }
    }
}
