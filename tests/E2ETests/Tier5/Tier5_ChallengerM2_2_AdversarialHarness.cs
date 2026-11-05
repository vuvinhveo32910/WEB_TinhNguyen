using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    /// Tier 5: Adversarial Challenger 2 Harness for Milestone M2 (Requirement R1)
    /// Explicit verification for:
    /// 1. Zero regression on Sales Subsystem (Areas/CuaHang/): models, controllers, views, layouts.
    /// 2. In-Memory fallback resilience in DbInitializer when physical database is offline.
    /// 3. Concurrency, state transitions, and inventory conservation invariants.
    /// 4. Controller runtime lifecycle under adversarial and boundary conditions.
    /// </summary>
    public class Tier5_ChallengerM2_2_AdversarialHarness
    {
        #region Mock Providers & Helpers

        public class MockSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

            public bool IsAvailable => true;
            public string Id { get; } = Guid.NewGuid().ToString("N");
            public IEnumerable<string> Keys => _store.Keys;

            public void Clear() => _store.Clear();

            public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => _store.Remove(key);

            public void Set(string key, byte[] value) => _store[key] = value;

            public bool TryGetValue(string key, out byte[]? value)
            {
                if (_store.TryGetValue(key, out var bytes))
                {
                    value = bytes;
                    return true;
                }
                value = null;
                return false;
            }
        }

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

        private static ApplicationDbContext CreateDisconnectedContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=127.0.0.1,1;Database=NonExistentDb_OfflineTest;Connect Timeout=1;")
                .Options;
            return new ApplicationDbContext(options);
        }

        private static DefaultHttpContext CreateMockHttpContext(MockSession? session = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session ?? new MockSession();
            return httpContext;
        }

        #endregion

        // =========================================================================
        // SECTION 1: SALES SUBSYSTEM ZERO REGRESSION & COLOR HYGIENE (AREAS/CUAHANG)
        // =========================================================================

        [E2ETest(5, 101, "M2_2_C01: Absolute zero regression - Areas/CuaHang contains NO references to dropped color properties")]
        public void M2_2_C01_ZeroRegression_NoColorPropertiesInSalesSubsystem()
        {
            var cuaHangDir = Path.Combine(E2ETestContext.ProjectRoot, "Areas", "CuaHang");
            Assert.True(Directory.Exists(cuaHangDir), "Areas/CuaHang directory must exist");

            var allFiles = Directory.GetFiles(cuaHangDir, "*.*", SearchOption.AllDirectories);
            Assert.True(allFiles.Length >= 20, $"Expected >= 20 files in Areas/CuaHang, found {allFiles.Length}");

            var forbiddenColorTokens = new[]
            {
                "MauChu", "MauNen", "MauIcon", "MauTieuDe", "MauChuDao", "MauNenPhu",
                "MauVien", "MauNhan", "MauDam", "MauNhat", "type=\"color\""
            };

            foreach (var file in allFiles)
            {
                var content = File.ReadAllText(file);
                foreach (var token in forbiddenColorTokens)
                {
                    Assert.False(content.Contains(token, StringComparison.OrdinalIgnoreCase),
                        $"File '{Path.GetFileName(file)}' must NOT contain dropped color token '{token}'");
                }
            }
        }

        [E2ETest(5, 101, "M2_2_C02: Preserved Sales Models exist with correct namespaces and properties")]
        public void M2_2_C02_SalesModels_IntegrityCheck()
        {
            var p = new SanPhamGayQuy
            {
                IDSanPham = 10,
                TenSanPham = "Sản phẩm kiểm thử",
                GiaBan = 99000m,
                GiaGoc = 120000m,
                SoLuongTon = 50,
                DaBan = 10,
                PhanTramDongGop = 100,
                DanhMuc = "Lưu niệm",
                TrangThai = true
            };

            Assert.Equal(10, p.IDSanPham, "IDSanPham matches");
            Assert.Equal(99000m, p.GiaBan, "GiaBan matches");
            Assert.Equal(120000m, p.GiaGoc, "GiaGoc matches");
            Assert.Equal(100, p.PhanTramDongGop, "PhanTramDongGop matches");

            var o = new DonHangGayQuy
            {
                IDDonHang = 20,
                MaDonHang = "GQ2026-TEST",
                TongTienHang = 198000m,
                TienUngHoThem = 20000m,
                PhiVanChuyen = 0m,
                TongThanhToan = 218000m,
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan"
            };

            Assert.Equal("GQ2026-TEST", o.MaDonHang, "MaDonHang matches");
            Assert.Equal(218000m, o.TongThanhToan, "TongThanhToan matches");
        }

        // =========================================================================
        // SECTION 2: DBINITIALIZER IN-MEMORY FALLBACK STABILITY & THREAD SAFETY
        // =========================================================================

        [E2ETest(5, 102, "M2_2_C03: DbInitializer.Seed and Initialize survive gracefully when Database is offline")]
        public void M2_2_C03_DbInitializer_OfflineResilience()
        {
            var disconnectedContext = CreateDisconnectedContext();

            // DbInitializer.Seed and Initialize must catch any database exception internally without throwing
            DbInitializer.Seed(disconnectedContext);
            DbInitializer.Initialize(disconnectedContext);

            // In-Memory store must still be populated and accessible
            var inMemProducts = DbInitializer.GetInMemoryProducts();
            Assert.NotNull(inMemProducts, "In-memory products list must not be null");
            Assert.True(inMemProducts.Count >= 4, $"Expected >= 4 default products, got {inMemProducts.Count}");

            var inMemOrders = DbInitializer.GetInMemoryOrders();
            Assert.NotNull(inMemOrders, "In-memory orders list must not be null");
            Assert.True(inMemOrders.Count >= 2, $"Expected >= 2 default orders, got {inMemOrders.Count}");
        }

        [E2ETest(5, 102, "M2_2_C04: High concurrency stress: 100 parallel tasks bombarding DbInitializer")]
        public void M2_2_C04_DbInitializer_ConcurrencyBombardment()
        {
            DbInitializer.ResetInMemoryStore();

            Parallel.For(0, 100, i =>
            {
                int op = i % 5;
                if (op == 0)
                {
                    DbInitializer.AddInMemoryProduct(new SanPhamGayQuy
                    {
                        TenSanPham = $"Concurrent Product #{i}",
                        GiaBan = 10000m * (i + 1),
                        SoLuongTon = 10
                    });
                }
                else if (op == 1)
                {
                    DbInitializer.AddInMemoryOrder(new DonHangGayQuy
                    {
                        HoTenNguoiMua = $"Concurrent Buyer #{i}",
                        SoDienThoai = "0987654321",
                        DiaChiNhanHang = "TP.HCM",
                        TongThanhToan = 100000m,
                        ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                        {
                            new ChiTietDonHangGayQuy
                            {
                                IDSanPham = 1,
                                TenSanPham = "Áo Thun",
                                DonGia = 129000m,
                                SoLuong = 1,
                                ThanhTien = 129000m
                            }
                        }
                    });
                }
                else if (op == 2)
                {
                    DbInitializer.UpdateInMemoryOrderStatus(1001, "DaXacNhan", "DaThanhToan");
                }
                else if (op == 3)
                {
                    var p = DbInitializer.GetInMemoryProductById(1);
                    Assert.NotNull(p, "Product #1 must exist");
                }
                else
                {
                    var orders = DbInitializer.GetInMemoryOrders();
                    Assert.NotNull(orders, "Orders must not be null");
                }
            });

            var finalProducts = DbInitializer.GetInMemoryProducts();
            Assert.True(finalProducts.Count >= 4, "Final product store must remain intact");
            DbInitializer.ResetInMemoryStore();
        }

        // =========================================================================
        // SECTION 3: INVENTORY CONSERVATION & CANCELLATION RESTOCK INVARIANTS
        // =========================================================================

        [E2ETest(5, 103, "M2_2_C05: Order placement deducts inventory and cancellation restores inventory exactly once")]
        public void M2_2_C05_InventoryConservation_And_CancellationIdempotence()
        {
            DbInitializer.ResetInMemoryStore();
            var prodBefore = DbInitializer.GetInMemoryProductById(1)!;
            int initialStock = prodBefore.SoLuongTon;
            int initialSold = prodBefore.DaBan;

            // Place an order of 3 items
            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Lê Văn An",
                SoDienThoai = "0912345678",
                DiaChiNhanHang = "Khu Công Nghệ Cao, TP. Thủ Đức",
                TongThanhToan = 387000m,
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy
                    {
                        IDSanPham = 1,
                        TenSanPham = prodBefore.TenSanPham,
                        DonGia = prodBefore.GiaBan,
                        SoLuong = 3,
                        ThanhTien = 387000m
                    }
                }
            };

            var placedOrder = DbInitializer.AddInMemoryOrder(order);
            var prodAfterOrder = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(initialStock - 3, prodAfterOrder.SoLuongTon, "Stock must decrement by 3");
            Assert.Equal(initialSold + 3, prodAfterOrder.DaBan, "DaBan must increment by 3");

            // Cancel order: Restock should happen
            bool cancelResult = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy");
            Assert.True(cancelResult, "Cancellation must succeed");

            var prodAfterCancel = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(initialStock, prodAfterCancel.SoLuongTon, "Stock must be fully restored upon cancellation");
            Assert.Equal(initialSold, prodAfterCancel.DaBan, "DaBan must be restored to initial value upon cancellation");

            // Idempotence test: Repeated cancellation must NOT restock again (prevent inventory duplication bug)
            bool repeatCancelResult = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy");
            Assert.True(repeatCancelResult, "Repeat cancellation should return true");

            var prodAfterRepeatCancel = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(initialStock, prodAfterRepeatCancel.SoLuongTon, "Stock must NOT inflate on repeated cancellation");
            Assert.Equal(initialSold, prodAfterRepeatCancel.DaBan, "DaBan must NOT decrease on repeated cancellation");

            // Terminal state test: Cannot reactivate a cancelled order back to active status
            bool reactivateResult = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DangGiao");
            Assert.False(reactivateResult, "Terminal state: cannot transition cancelled order to DangGiao");

            DbInitializer.ResetInMemoryStore();
        }

        // =========================================================================
        // SECTION 4: CONTROLLER INTEGRATION & ENDPOINT RESILIENCE
        // =========================================================================

        [E2ETest(5, 104, "M2_2_C06: CuaHangController Index handles search, category, sort, and SQL injection without throwing")]
        public async Task M2_2_C06_CuaHangController_Index_AdversarialInputs()
        {
            DbInitializer.ResetInMemoryStore();
            var context = CreateDisconnectedContext();
            var controller = new CuaHangController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() }
            };

            // 1. Default load
            var resDefault = await controller.Index(null, null, null);
            var viewResDefault = Assert.IsType<ViewResult>(resDefault, "Index must return ViewResult");
            var productsDefault = Assert.IsType<List<SanPhamGayQuy>>(viewResDefault.Model, "Model must be List<SanPhamGayQuy>");
            Assert.True(productsDefault.Count >= 4, "Default catalog must have >= 4 items");

            // 2. Search query matching
            var resSearch = await controller.Index("Áo", null, null);
            var viewResSearch = Assert.IsType<ViewResult>(resSearch, "Search must return ViewResult");
            var productsSearch = Assert.IsType<List<SanPhamGayQuy>>(viewResSearch.Model, "Model must be List<SanPhamGayQuy>");
            Assert.True(productsSearch.Any(p => p.TenSanPham.Contains("Áo")), "Search result must contain 'Áo'");

            // 3. Adversarial SQL Injection payload search
            var resSql = await controller.Index("'; DROP TABLE SanPhamGayQuy;--", null, null);
            var viewResSql = Assert.IsType<ViewResult>(resSql, "SQL injection search must return ViewResult safely");
            var productsSql = Assert.IsType<List<SanPhamGayQuy>>(viewResSql.Model, "Model must be empty or safe list");
            Assert.Equal(0, productsSql.Count, "No items match SQL injection string");

            // 4. Sorting
            var resSortAsc = await controller.Index(null, null, "price_asc");
            var productsSortAsc = Assert.IsType<List<SanPhamGayQuy>>(((ViewResult)resSortAsc).Model, "Model must be list");
            for (int i = 0; i < productsSortAsc.Count - 1; i++)
            {
                Assert.True(productsSortAsc[i].GiaBan <= productsSortAsc[i + 1].GiaBan, "Prices must be ascending");
            }
        }

        [E2ETest(5, 104, "M2_2_C07: CuaHangController ChiTiet handles valid, nonexistent, and negative IDs safely")]
        public async Task M2_2_C07_CuaHangController_ChiTiet_IdBoundaries()
        {
            DbInitializer.ResetInMemoryStore();
            var context = CreateDisconnectedContext();
            var controller = new CuaHangController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() }
            };

            // Valid ID
            var resValid = await controller.ChiTiet(1);
            var viewValid = Assert.IsType<ViewResult>(resValid, "ChiTiet(1) must return ViewResult");
            var prod = Assert.IsType<SanPhamGayQuy>(viewValid.Model, "Model must be SanPhamGayQuy");
            Assert.Equal(1, prod.IDSanPham, "ID must be 1");

            // Non-existent ID
            var resNotFound = await controller.ChiTiet(9999);
            Assert.IsType<NotFoundResult>(resNotFound, "ChiTiet(9999) must return NotFoundResult");

            // Negative ID
            var resNegative = await controller.ChiTiet(-10);
            Assert.IsType<NotFoundResult>(resNegative, "ChiTiet(-10) must return NotFoundResult");
        }

        [E2ETest(5, 104, "M2_2_C08: GioHangController Add, Update, Remove, and DatHang full lifecycle")]
        public async Task M2_2_C08_GioHangController_FullLifecycle()
        {
            DbInitializer.ResetInMemoryStore();
            var context = CreateDisconnectedContext();
            var session = new MockSession();
            var httpContext = CreateMockHttpContext(session);
            var controller = new GioHangController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, new MockTempDataProvider())
            };

            // 1. Add valid product #1 to cart (quantity 2)
            var addRes = await controller.ThemVaoGio(1, 2);
            var redirectAdd = Assert.IsType<RedirectToActionResult>(addRes, "ThemVaoGio must redirect");
            Assert.Equal("Index", redirectAdd.ActionName, "Must redirect to Index");

            // Verify cart in session
            var cartRaw = session.TryGetValue("CuaHang_GioHang", out var bytes);
            Assert.True(cartRaw, "Session must contain cart key");
            var cart = JsonSerializer.Deserialize<List<CartItem>>(bytes!)!;
            Assert.Equal(1, cart.Count, "Cart should have 1 item");
            Assert.Equal(2, cart[0].SoLuong, "Quantity should be 2");

            // 2. Update quantity to 4
            var updateRes = controller.CapNhatSoLuong(1, 4);
            Assert.IsType<RedirectToActionResult>(updateRes, "CapNhatSoLuong must redirect");
            session.TryGetValue("CuaHang_GioHang", out bytes);
            cart = JsonSerializer.Deserialize<List<CartItem>>(bytes!)!;
            Assert.Equal(4, cart[0].SoLuong, "Quantity should be updated to 4");

            // 3. Checkout with valid customer data
            var checkoutVm = new CheckoutViewModel
            {
                HoTen = "Trần Thị Hoàng Yến",
                SoDienThoai = "0909123456",
                Email = "hoangyen@volunteer.org",
                DiaChi = "123 Điện Biên Phủ, Bình Thạnh",
                TienUngHoThem = 50000m,
                PhuongThucThanhToan = "ChuyenKhoanQR"
            };

            var orderRes = await controller.DatHang(checkoutVm);
            var redirectOrder = Assert.IsType<RedirectToActionResult>(orderRes, "DatHang must redirect to DatHangThanhCong");
            Assert.Equal("DatHangThanhCong", redirectOrder.ActionName, "Must redirect to DatHangThanhCong");

            // Cart session must now be cleared
            session.TryGetValue("CuaHang_GioHang", out bytes);
            Assert.Null(bytes, "Cart must be emptied after placing order");

            // Order confirmation view
            int orderId = (int)redirectOrder.RouteValues!["id"]!;
            var confirmRes = await controller.DatHangThanhCong(orderId);
            var confirmView = Assert.IsType<ViewResult>(confirmRes, "DatHangThanhCong must return ViewResult");
            var orderModel = Assert.IsType<DonHangGayQuy>(confirmView.Model, "Model must be DonHangGayQuy");
            Assert.Equal(orderId, orderModel.IDDonHang, "Order ID must match");
            Assert.Equal(50000m, orderModel.TienUngHoThem, "TienUngHoThem must match");

            DbInitializer.ResetInMemoryStore();
        }

        [E2ETest(5, 104, "M2_2_C09: AdminSanPhamController and AdminDonHangController run smoothly on In-Memory fallback")]
        public async Task M2_2_C09_AdminControllers_RunOnInMemoryFallback()
        {
            DbInitializer.ResetInMemoryStore();
            var context = CreateDisconnectedContext();
            var httpContext = CreateMockHttpContext();

            // AdminSanPhamController
            var mockEnv = new Tier5_ChallengerM2Harness.MockWebHostEnvironment();
            var sanPhamController = new AdminSanPhamController(context, mockEnv)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, new MockTempDataProvider())
            };

            var spIndexRes = await sanPhamController.Index();
            var spIndexView = Assert.IsType<ViewResult>(spIndexRes, "AdminSanPham Index must return ViewResult");
            var spList = Assert.IsType<List<SanPhamGayQuy>>(spIndexView.Model, "Model must be List<SanPhamGayQuy>");
            Assert.True(spList.Count >= 4, "AdminSanPham must list all in-memory products");

            // AdminDonHangController
            var donHangController = new AdminDonHangController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, new MockTempDataProvider())
            };

            var dhIndexRes = await donHangController.Index(null, null, null, null);
            var dhIndexView = Assert.IsType<ViewResult>(dhIndexRes, "AdminDonHang Index must return ViewResult");
            var dhList = Assert.IsType<List<DonHangGayQuy>>(dhIndexView.Model, "Model must be List<DonHangGayQuy>");
            Assert.True(dhList.Count >= 2, "AdminDonHang must list in-memory orders");

            // Detail of order #1001
            var dhDetailRes = await donHangController.ChiTiet(1001);
            var dhDetailView = Assert.IsType<ViewResult>(dhDetailRes, "ChiTiet must return ViewResult");
            var dhItem = Assert.IsType<DonHangGayQuy>(dhDetailView.Model, "Model must be DonHangGayQuy");
            Assert.Equal(1001, dhItem.IDDonHang, "ID must be 1001");

            // Update status
            var dhUpdateRes = await donHangController.CapNhatTrangThai(1001, "DaXacNhan", "DaThanhToan");
            var dhRedirect = Assert.IsType<RedirectToActionResult>(dhUpdateRes, "CapNhatTrangThai must redirect");
            Assert.Equal("ChiTiet", dhRedirect.ActionName, "Must redirect to ChiTiet");

            var updatedOrder = DbInitializer.GetInMemoryOrderById(1001)!;
            Assert.Equal("DaXacNhan", updatedOrder.TrangThaiDonHang, "Status must be DaXacNhan");
            Assert.Equal("DaThanhToan", updatedOrder.TrangThaiThanhToan, "Payment must be DaThanhToan");

            DbInitializer.ResetInMemoryStore();
        }
    }
}
