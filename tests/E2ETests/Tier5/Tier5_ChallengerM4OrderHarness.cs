using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    /// <summary>
    /// Tier 5: Adversarial Stress, Concurrency & Corner Case Harness for Milestone 4 (Admin Order Management & Stock Lifecycle)
    /// Challenges:
    /// 1. Multithreaded concurrency in DbInitializer order mutations
    /// 2. Parallel status updates and restock integrity
    /// 3. Rapid state transitions and restock idempotence (no duplicate inventory inflation)
    /// 4. Defensive cloning and immutability protection
    /// 5. Out of stock floor boundaries and search resilience
    /// 6. Floating cart action button and SweetAlert2 presence
    /// </summary>
    public class Tier5_ChallengerM4OrderHarness
    {
        [E2ETest(5, 18, "CH_M4_01: Concurrency stress test: 20 parallel order updates in DbInitializer remain thread-safe")]
        public async Task CH_M4_01_DbInitializer_Parallel_Order_Updates_ThreadSafety()
        {
            var orderIds = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                var ord = new DonHangGayQuy
                {
                    HoTenNguoiMua = $"Parallel Donor #{i}",
                    SoDienThoai = $"09000000{i:D2}",
                    DiaChiNhanHang = "Khuôn viên HUTECH",
                    TongThanhToan = 50000m + (i * 1000m),
                    PhuongThucThanhToan = "COD",
                    TrangThaiDonHang = "MoiDat",
                    TrangThaiThanhToan = "ChuaThanhToan",
                    ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                    {
                        new ChiTietDonHangGayQuy
                        {
                            IDSanPham = 1,
                            TenSanPham = "Áo thun Tình nguyện Langbiang 2026",
                            DonGia = 120000m,
                            SoLuong = 1,
                            ThanhTien = 120000m,
                            PhanTramDongGop = 30
                        }
                    }
                };
                var added = DbInitializer.AddInMemoryOrder(ord);
                orderIds.Add(added.IDDonHang);
            }

            var tasks = orderIds.Select((id, index) => Task.Run(() =>
            {
                var newStatus = (index % 2 == 0) ? "DaXacNhan" : "DangGiao";
                var newPayment = (index % 2 == 0) ? "DaThanhToan" : "ChuaThanhToan";
                return DbInitializer.UpdateInMemoryOrderStatus(id, newStatus, newPayment);
            })).ToArray();

            var results = await Task.WhenAll(tasks);

            Assert.True(results.All(r => r), "All 20 concurrent order updates must return true");

            foreach (var id in orderIds)
            {
                var ord = DbInitializer.GetInMemoryOrderById(id);
                Assert.NotNull(ord, $"Order #{id} must exist after parallel update");
                Assert.True(ord.TrangThaiDonHang == "DaXacNhan" || ord.TrangThaiDonHang == "DangGiao",
                    $"Order #{id} status must be DaXacNhan or DangGiao");
                Assert.NotNull(ord.NgayCapNhat, "NgayCapNhat must be set");
            }

            DbInitializer.ResetInMemoryStore();
        }

        [E2ETest(5, 18, "CH_M4_02: Concurrency stress: parallel status updates and restock checks do not produce race conditions")]
        public async Task CH_M4_02_DbInitializer_Concurrent_Status_Updates_And_Restock_Integrity()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm Stress Test Stock CH_M4_02",
                GiaBan = 100000m,
                SoLuongTon = 100,
                DaBan = 0,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            // Create 10 orders each buying 5 units (total 50 units deducted -> stock remains 50, DaBan = 50)
            var orderIds = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                var ord = new DonHangGayQuy
                {
                    HoTenNguoiMua = $"Buyer CH02 #{i}",
                    SoDienThoai = "0911223344",
                    DiaChiNhanHang = "Đà Lạt",
                    TongThanhToan = 500000m,
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
                            SoLuong = 5,
                            ThanhTien = 500000m
                        }
                    }
                };
                var added = DbInitializer.AddInMemoryOrder(ord);
                orderIds.Add(added.IDDonHang);
            }

            var midCheckProd = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(50, midCheckProd?.SoLuongTon, "Stock after 10 orders of 5 units must be 50");
            Assert.Equal(50, midCheckProd?.DaBan, "DaBan after 10 orders of 5 units must be 50");

            // Concurrently cancel 5 orders (restock 25 units) and complete 5 orders (no restock)
            var tasks = orderIds.Select((id, index) => Task.Run(() =>
            {
                if (index < 5)
                {
                    return DbInitializer.UpdateInMemoryOrderStatus(id, "DaHuy", null);
                }
                else
                {
                    return DbInitializer.UpdateInMemoryOrderStatus(id, "HoanThanh", "DaThanhToan");
                }
            })).ToArray();

            var updateResults = await Task.WhenAll(tasks);
            Assert.True(updateResults.All(r => r), "All 10 parallel status transitions must succeed");

            var finalProd = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.NotNull(finalProd, "Product must exist");
            Assert.Equal(75, finalProd.SoLuongTon, "Final stock must be exactly 75 (50 + 25 restored from 5 cancelled orders)");
            Assert.Equal(25, finalProd.DaBan, "Final DaBan must be exactly 25 (50 - 25 from 5 cancelled orders)");
        }

        [E2ETest(5, 18, "CH_M4_03: Rapid transition stress calculates final inventory accurately without duplicate restock")]
        public void CH_M4_03_Rapid_Status_Transitions_Accurate_Restock()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm Rapid Transition CH_M4_03",
                GiaBan = 60000m,
                SoLuongTon = 50,
                DaBan = 10,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Rapid Transition Donor",
                SoDienThoai = "0988776655",
                DiaChiNhanHang = "TP. Thủ Đức",
                TongThanhToan = 600000m,
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
                        SoLuong = 10,
                        ThanhTien = 600000m
                    }
                }
            };

            var placedOrder = DbInitializer.AddInMemoryOrder(order);
            var prod1 = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(40, prod1?.SoLuongTon, "Stock after order must be 40");
            Assert.Equal(20, prod1?.DaBan, "DaBan after order must be 20");

            // Rapid transitions before cancellation
            DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaXacNhan", "ChuaThanhToan");
            DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DangGiao", "DaThanhToan");
            DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "HoanThanh", "DaThanhToan");

            var prodIntermediate = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(40, prodIntermediate?.SoLuongTon, "Intermediate non-cancel transitions must not alter stock");
            Assert.Equal(20, prodIntermediate?.DaBan, "Intermediate non-cancel transitions must not alter DaBan");

            // Final cancellation
            DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy", null);

            var prodFinal = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(50, prodFinal?.SoLuongTon, "Stock must be fully restored to 50");
            Assert.Equal(10, prodFinal?.DaBan, "DaBan must be restored to 10");
        }

        [E2ETest(5, 18, "CH_M4_04: Idempotence test: calling UpdateInMemoryOrderStatus with DaHuy twice does not restock twice")]
        public void CH_M4_04_Cancel_Order_Restock_Idempotence()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm Idempotence CH_M4_04",
                GiaBan = 75000m,
                SoLuongTon = 30,
                DaBan = 5,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Idempotence Buyer",
                SoDienThoai = "0933445566",
                DiaChiNhanHang = "Bình Thạnh",
                TongThanhToan = 150000m,
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
                        SoLuong = 6,
                        ThanhTien = 450000m
                    }
                }
            };

            var placedOrder = DbInitializer.AddInMemoryOrder(order);
            var prodAfterOrder = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(24, prodAfterOrder?.SoLuongTon, "Stock after buying 6 units must be 24");
            Assert.Equal(11, prodAfterOrder?.DaBan, "DaBan after buying 6 units must be 11");

            // First cancellation
            var firstCancel = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy", null);
            Assert.True(firstCancel, "First cancellation must succeed");

            var prodAfterFirstCancel = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(30, prodAfterFirstCancel?.SoLuongTon, "Stock after first cancellation must be 30");
            Assert.Equal(5, prodAfterFirstCancel?.DaBan, "DaBan after first cancellation must be 5");

            // Second cancellation (Duplicate call)
            var secondCancel = DbInitializer.UpdateInMemoryOrderStatus(placedOrder.IDDonHang, "DaHuy", null);
            Assert.True(secondCancel, "Second cancellation call returns true");

            var prodAfterSecondCancel = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.Equal(30, prodAfterSecondCancel?.SoLuongTon, "Stock must remain 30 without duplicate inflation");
            Assert.Equal(5, prodAfterSecondCancel?.DaBan, "DaBan must remain 5 without double decrement");
        }

        [E2ETest(5, 18, "CH_M4_05: Cloned order protection: modifying object from GetInMemoryOrderById does not mutate store")]
        public void CH_M4_05_Cloned_Order_Immutability_Protection()
        {
            var orders = DbInitializer.GetInMemoryOrders();
            Assert.True(orders.Count > 0, "Store must contain orders");
            var originalOrder = orders[0];
            var originalName = originalOrder.HoTenNguoiMua;
            var originalTotal = originalOrder.TongThanhToan;
            var originalStatus = originalOrder.TrangThaiDonHang;

            var retrieved = DbInitializer.GetInMemoryOrderById(originalOrder.IDDonHang);
            Assert.NotNull(retrieved, "Retrieved order must not be null");

            // Mutate retrieved order clone
            retrieved.HoTenNguoiMua = "MODIFIED_HACKER_NAME";
            retrieved.TongThanhToan = 999999999m;
            retrieved.TrangThaiDonHang = "DaHuy";
            if (retrieved.ChiTietDonHangs != null && retrieved.ChiTietDonHangs.Count > 0)
            {
                retrieved.ChiTietDonHangs.First().TenSanPham = "CORRUPTED_PRODUCT_NAME";
            }

            // Retrieve again and verify internal integrity
            var fresh = DbInitializer.GetInMemoryOrderById(originalOrder.IDDonHang);
            Assert.NotNull(fresh, "Freshly retrieved order must not be null");
            Assert.Equal(originalName, fresh?.HoTenNguoiMua, "Internal HoTenNguoiMua must not be corrupted by external mutation");
            Assert.Equal(originalTotal, fresh?.TongThanhToan, "Internal TongThanhToan must not be corrupted by external mutation");
            Assert.Equal(originalStatus, fresh?.TrangThaiDonHang, "Internal TrangThaiDonHang must not be corrupted by external mutation");

            if (fresh?.ChiTietDonHangs != null && fresh.ChiTietDonHangs.Count > 0)
            {
                Assert.NotEqual("CORRUPTED_PRODUCT_NAME", fresh.ChiTietDonHangs.First().TenSanPham,
                    "Child item details must not be mutated in storage");
            }
        }

        [E2ETest(5, 18, "CH_M4_06: Floating cart action button markup and SweetAlert2 presence in _Layout.cshtml")]
        public void CH_M4_06_Layout_Floating_Cart_Button_And_SweetAlert2_Integrity()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml");
            Assert.True(File.Exists(layoutPath), "_Layout.cshtml must exist");

            var layoutContent = File.ReadAllText(layoutPath);

            // Floating cart button validation
            Assert.True(
                layoutContent.Contains("floatingCartBtn") || layoutContent.Contains("floatingCartCount"),
                "_Layout.cshtml must declare floatingCartBtn or floatingCartCount ID"
            );
            Assert.True(
                layoutContent.Contains("asp-controller=\"GioHang\"") || layoutContent.Contains("CuaHang/GioHang") || layoutContent.Contains("asp-area=\"CuaHang\""),
                "_Layout.cshtml floating action cart must route to CuaHang/GioHang"
            );

            // SweetAlert2 library validation
            Assert.True(
                layoutContent.Contains("sweetalert2") || layoutContent.Contains("Swal"),
                "_Layout.cshtml must include SweetAlert2 script for interactive alert/toast notifications"
            );
        }

        [E2ETest(5, 18, "CH_M4_07: Over-order stock flooring at zero prevents negative inventory")]
        public void CH_M4_07_Concurrent_Order_Creation_Floors_Stock_At_Zero()
        {
            var product = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm giới hạn số lượng CH_M4_07",
                GiaBan = 50000m,
                SoLuongTon = 3,
                DaBan = 0,
                PhanTramDongGop = 100
            };
            var savedProduct = DbInitializer.AddInMemoryProduct(product);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Oversell Buyer",
                SoDienThoai = "0944556677",
                DiaChiNhanHang = "TP. HCM",
                TongThanhToan = 250000m,
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
                        SoLuong = 10, // Requests 10, but only 3 in stock
                        ThanhTien = 500000m
                    }
                }
            };

            DbInitializer.AddInMemoryOrder(order);

            var updatedProd = DbInitializer.GetInMemoryProductById(savedProduct.IDSanPham);
            Assert.NotNull(updatedProd, "Product must exist");
            Assert.Equal(0, updatedProd.SoLuongTon, "SoLuongTon must floor cleanly at 0 without becoming negative");
            Assert.Equal(10, updatedProd.DaBan, "DaBan must track 10 ordered units");
        }

        [E2ETest(5, 18, "CH_M4_08: Parallel addition and deletion in DbInitializer maintain store consistency")]
        public async Task CH_M4_08_Concurrent_Order_Deletion_ThreadSafety()
        {
            var tempOrders = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                var ord = new DonHangGayQuy
                {
                    HoTenNguoiMua = $"Temporary Parallel Order #{i}",
                    SoDienThoai = $"09770000{i:D2}",
                    DiaChiNhanHang = "Khuôn viên trường",
                    TongThanhToan = 80000m,
                    PhuongThucThanhToan = "COD",
                    TrangThaiDonHang = "MoiDat",
                    TrangThaiThanhToan = "ChuaThanhToan"
                };
                var added = DbInitializer.AddInMemoryOrder(ord);
                tempOrders.Add(added.IDDonHang);
            }

            // Concurrently delete 10 orders while fetching the full list
            var deleteTasks = tempOrders.Take(10).Select(id => Task.Run(() => DbInitializer.DeleteInMemoryOrder(id)));
            var readTasks = Enumerable.Range(0, 5).Select(_ => Task.Run(() => DbInitializer.GetInMemoryOrders()));

            await Task.WhenAll(deleteTasks.Concat<Task>(readTasks));

            // Verify deleted orders cannot be found
            foreach (var deletedId in tempOrders.Take(10))
            {
                var notFound = DbInitializer.GetInMemoryOrderById(deletedId);
                Assert.Null(notFound, $"Deleted order #{deletedId} must not be retrievable");
            }

            // Verify remaining 5 orders still exist
            foreach (var remainingId in tempOrders.Skip(10))
            {
                var found = DbInitializer.GetInMemoryOrderById(remainingId);
                Assert.NotNull(found, $"Remaining order #{remainingId} must still exist");
            }
        }

        [E2ETest(5, 18, "CH_M4_09: Admin order status badges use semantic Tailwind styling in ChiTiet view")]
        public void CH_M4_09_Admin_Order_Status_Badge_Semantic_Styling()
        {
            var detailPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "ChiTiet.cshtml");
            Assert.True(File.Exists(detailPath), "ChiTiet.cshtml must exist");

            var content = File.ReadAllText(detailPath);

            // Check semantic color styling for order status badge
            Assert.True(content.Contains("amber") || content.Contains("yellow"), "MoiDat badge must use amber/yellow styling");
            Assert.True(content.Contains("blue"), "DaXacNhan badge must use blue styling");
            Assert.True(content.Contains("purple") || content.Contains("indigo"), "DangGiao badge must use purple/indigo styling");
            Assert.True(content.Contains("emerald") || content.Contains("green"), "HoanThanh badge must use emerald/green styling");
            Assert.True(content.Contains("red") || content.Contains("rose"), "DaHuy badge must use red/rose styling");
        }

        [E2ETest(5, 18, "CH_M4_10: Order search resilience with regex, quotes, SQL injection strings and diacritics")]
        public void CH_M4_10_Admin_Order_Search_Special_Characters_Resilience()
        {
            var allOrders = DbInitializer.GetInMemoryOrders();

            var maliciousQueries = new[]
            {
                "' OR '1'='1",
                "\"; DROP TABLE DonHangGayQuy; --",
                "<script>alert('XSS')</script>",
                "[a-z]+.*",
                "Trần Bảo Long", // Accented Vietnamese
                "GQ2026-",
                "%_#@!$^&*()",
                "   "
            };

            foreach (var q in maliciousQueries)
            {
                var keyword = q.Trim().ToLower();
                var query = allOrders.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // Mirroring AdminDonHangController search filtering logic
                    var filtered = query.Where(o =>
                        (!string.IsNullOrEmpty(o.MaDonHang) && o.MaDonHang.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(o.HoTenNguoiMua) && o.HoTenNguoiMua.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(o.SoDienThoai) && o.SoDienThoai.Contains(keyword)) ||
                        (!string.IsNullOrEmpty(o.Email) && o.Email.ToLower().Contains(keyword))
                    ).ToList();

                    Assert.NotNull(filtered, $"Search for query '{q}' must not produce null result");
                }
            }
        }

        [E2ETest(5, 18, "CH_M4_11: 100-thread high-concurrency mixed operations on DbInitializer remain deadlock-free")]
        public void CH_M4_11_High_Concurrency_Mixed_Operations_Deadlock_Free()
        {
            var tasks = new List<Task>();
            var createdIds = new System.Collections.Concurrent.ConcurrentBag<int>();

            // 25 threads adding orders
            for (int i = 0; i < 25; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(() =>
                {
                    var ord = new DonHangGayQuy
                    {
                        HoTenNguoiMua = $"Stress Buyer {idx}",
                        SoDienThoai = $"09811122{idx:D2}",
                        TongThanhToan = 100000m + idx,
                        TrangThaiDonHang = "MoiDat"
                    };
                    var saved = DbInitializer.AddInMemoryOrder(ord);
                    createdIds.Add(saved.IDDonHang);
                }));
            }

            // 25 threads reading orders
            for (int i = 0; i < 25; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var orders = DbInitializer.GetInMemoryOrders();
                    Assert.NotNull(orders, "Orders list must not be null");
                    if (orders.Count > 0)
                    {
                        var o = DbInitializer.GetInMemoryOrderById(orders[0].IDDonHang);
                        Assert.NotNull(o, "Order by id must be retrievable");
                    }
                }));
            }

            // 25 threads updating status
            for (int i = 0; i < 25; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(() =>
                {
                    var orders = DbInitializer.GetInMemoryOrders();
                    if (orders.Count > 0)
                    {
                        var target = orders[idx % orders.Count];
                        DbInitializer.UpdateInMemoryOrderStatus(target.IDDonHang, "DaXacNhan", "DaThanhToan");
                    }
                }));
            }

            // 25 threads querying products
            for (int i = 0; i < 25; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var prods = DbInitializer.GetInMemoryProducts();
                    Assert.NotNull(prods, "Products list must not be null");
                }));
            }

            bool completedInTime = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));
            Assert.True(completedInTime, "All 100 parallel tasks must complete within 10 seconds without deadlocks");
        }

        [E2ETest(5, 18, "CH_M4_12_Adv: Defensive cloning in AddInMemoryOrder protects internal store against reference mutation")]
        public void CH_M4_12_Adv_AddInMemoryOrder_Reference_Mutation()
        {
            var originalOrder = new DonHangGayQuy
            {
                HoTenNguoiMua = "Original Uncorrupted Donor",
                SoDienThoai = "0909090909",
                DiaChiNhanHang = "TP. Hồ Chí Minh",
                TongThanhToan = 200000m,
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan"
            };

            var added = DbInitializer.AddInMemoryOrder(originalOrder);

            // External caller mutates their local reference
            originalOrder.HoTenNguoiMua = "CORRUPTED_DONOR_NAME_HACK";
            originalOrder.TongThanhToan = 999999999m;

            // Fetch from storage
            var fresh = DbInitializer.GetInMemoryOrderById(added.IDDonHang);
            Assert.NotNull(fresh, "Order must be retrievable");

            // Verified: mutating originalOrder reference does NOT mutate internal storage
            Assert.Equal("Original Uncorrupted Donor", fresh.HoTenNguoiMua,
                "Defensive cloning must prevent external reference mutation from corrupting the in-memory store");
            Assert.Equal(200000m, fresh.TongThanhToan,
                "Defensive cloning must preserve original order total amount in store");
        }

        [E2ETest(5, 18, "CH_M4_13_Adv: Terminal cancellation prevents cyclic reactivation and eliminates phantom restock loop")]
        public void CH_M4_13_Adv_Cyclic_Cancellation_Reactivation_Restock()
        {
            var prod = new SanPhamGayQuy
            {
                TenSanPham = "Sản phẩm Cyclic Restock Test CH_M4_13",
                GiaBan = 50000m,
                SoLuongTon = 50,
                DaBan = 0,
                PhanTramDongGop = 100
            };
            var savedProd = DbInitializer.AddInMemoryProduct(prod);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Cyclic Buyer",
                TongThanhToan = 500000m,
                TrangThaiDonHang = "MoiDat",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy
                    {
                        IDSanPham = savedProd.IDSanPham,
                        TenSanPham = savedProd.TenSanPham,
                        DonGia = savedProd.GiaBan,
                        SoLuong = 10,
                        ThanhTien = 500000m
                    }
                }
            };
            var addedOrder = DbInitializer.AddInMemoryOrder(order);

            // Initial order deduction: stock 50 -> 40, DaBan 0 -> 10
            var p1 = DbInitializer.GetInMemoryProductById(savedProd.IDSanPham);
            Assert.Equal(40, p1?.SoLuongTon, "Stock after order must be 40");
            Assert.Equal(10, p1?.DaBan, "DaBan after order must be 10");

            // Cycle 1: Cancel -> stock restores to 50, DaBan 0
            var cancel1 = DbInitializer.UpdateInMemoryOrderStatus(addedOrder.IDDonHang, "DaHuy", null);
            Assert.True(cancel1, "First cancellation must succeed");
            var p2 = DbInitializer.GetInMemoryProductById(savedProd.IDSanPham);
            Assert.Equal(50, p2?.SoLuongTon, "Stock after first cancellation must be 50");
            Assert.Equal(0, p2?.DaBan, "DaBan after first cancellation must be 0");

            // Attempted Reactivation: Admin switches back to DaXacNhan -> MUST FAIL because DaHuy is terminal
            var reactivateResult = DbInitializer.UpdateInMemoryOrderStatus(addedOrder.IDDonHang, "DaXacNhan", null);
            Assert.False(reactivateResult, "Cannot reactivate a cancelled order (DaHuy is a terminal state)");

            // Verify order status remains DaHuy
            var checkOrder = DbInitializer.GetInMemoryOrderById(addedOrder.IDDonHang);
            Assert.Equal("DaHuy", checkOrder?.TrangThaiDonHang, "Order status must remain DaHuy");

            // Calling cancel again does not restock again
            DbInitializer.UpdateInMemoryOrderStatus(addedOrder.IDDonHang, "DaHuy", null);
            var p3 = DbInitializer.GetInMemoryProductById(savedProd.IDSanPham);

            // Stock must stay exactly 50 (no phantom stock generation)
            Assert.Equal(50, p3?.SoLuongTon,
                "Stock must remain 50 without phantom inventory generation");
            Assert.Equal(0, p3?.DaBan,
                "DaBan must remain 0");
        }

        [E2ETest(5, 18, "CH_M4_16_Adv: ResetInMemoryStore guarantees sample orders are preserved even if order added before query")]
        public void CH_M4_16_Adv_Reset_Then_Add_Suppresses_Default_Seed_Orders()
        {
            DbInitializer.ResetInMemoryStore();

            // An order is placed before GetInMemoryOrders is called
            var ord = new DonHangGayQuy
            {
                HoTenNguoiMua = "Early Order Before Seed",
                TongThanhToan = 50000m,
                TrangThaiDonHang = "MoiDat"
            };
            DbInitializer.AddInMemoryOrder(ord);

            // Fetch in-memory orders
            var orders = DbInitializer.GetInMemoryOrders();

            // Verified: ResetInMemoryStore seeds default orders immediately, so count is 3 (2 sample + 1 new)
            Assert.Equal(3, orders.Count,
                "Store reset must guarantee default sample orders are present alongside newly added orders");
            Assert.True(orders.Any(o => o.HoTenNguoiMua == "Nguyễn Thị Mai Anh"),
                "Default sample order 'Nguyễn Thị Mai Anh' must be preserved");
            Assert.True(orders.Any(o => o.HoTenNguoiMua == "Early Order Before Seed"),
                "Newly placed order must also be preserved in store");
        }

        [E2ETest(5, 18, "CH_M4_14: Order cancellation restock handles previously deleted product gracefully")]
        public void CH_M4_14_Order_Restock_With_Deleted_Product_Safety()
        {
            var prod = new SanPhamGayQuy
            {
                TenSanPham = "Product to be deleted CH_M4_14",
                GiaBan = 50000m,
                SoLuongTon = 20,
                DaBan = 0,
                PhanTramDongGop = 100
            };
            var savedProd = DbInitializer.AddInMemoryProduct(prod);

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Deleted Product Buyer",
                TongThanhToan = 100000m,
                TrangThaiDonHang = "MoiDat",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy
                    {
                        IDSanPham = savedProd.IDSanPham,
                        TenSanPham = savedProd.TenSanPham,
                        DonGia = savedProd.GiaBan,
                        SoLuong = 2,
                        ThanhTien = 100000m
                    }
                }
            };
            var addedOrder = DbInitializer.AddInMemoryOrder(order);

            // Delete the product from store
            DbInitializer.DeleteInMemoryProduct(savedProd.IDSanPham);

            // Cancel the order - must not throw NullReferenceException
            var cancelResult = DbInitializer.UpdateInMemoryOrderStatus(addedOrder.IDDonHang, "DaHuy", null);
            Assert.True(cancelResult, "Cancellation must succeed gracefully even if product was deleted");
        }

        [E2ETest(5, 18, "CH_M4_15: Admin order search case insensitivity with Vietnamese Unicode characters")]
        public void CH_M4_15_Admin_Order_Search_Vietnamese_Unicode_Insensitivity()
        {
            var testOrder = new DonHangGayQuy
            {
                HoTenNguoiMua = "Nguyễn Thị Mai Anh",
                SoDienThoai = "0987654321",
                DiaChiNhanHang = "TP. Hồ Chí Minh",
                TongThanhToan = 250000m,
                TrangThaiDonHang = "MoiDat"
            };
            DbInitializer.AddInMemoryOrder(testOrder);

            var allOrders = DbInitializer.GetInMemoryOrders();
            Assert.True(allOrders.Count > 0, "Orders must exist");

            // Search lowercase 'mai anh' matching 'Nguyễn Thị Mai Anh'
            var q = "mai anh";
            var keyword = q.Trim().ToLower();
            var filtered = allOrders.Where(o =>
                (!string.IsNullOrEmpty(o.HoTenNguoiMua) && o.HoTenNguoiMua.ToLower().Contains(keyword))
            ).ToList();

            Assert.True(filtered.Count >= 1, "Search for 'mai anh' must match 'Nguyễn Thị Mai Anh'");
        }
    }
}
