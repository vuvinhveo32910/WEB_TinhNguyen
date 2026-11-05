using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    public class Tier5_ChallengerM1Harness
    {
        // =========================================================================
        // SECTION 1: SESSION EXTENSIONS ADVERSARIAL STRESS TESTS
        // =========================================================================

        [E2ETest(5, 2, "CH01: SessionExtensions throws ArgumentNullException on null session")]
        public void CH01_SessionExtensions_NullSession_Throws()
        {
            Microsoft.AspNetCore.Http.ISession nullSession = null!;

            Assert.Throws<ArgumentNullException>(() =>
            {
                nullSession.Set("test", "val");
            }, "Set with null session must throw ArgumentNullException");

            Assert.Throws<ArgumentNullException>(() =>
            {
                nullSession.Get<string>("test");
            }, "Get with null session must throw ArgumentNullException");

            Assert.Throws<ArgumentNullException>(() =>
            {
                nullSession.GetOrCreate("test", () => "new_val");
            }, "GetOrCreate with null session must throw ArgumentNullException");
        }

        [E2ETest(5, 2, "CH02: SessionExtensions null or empty key handling")]
        public void CH02_SessionExtensions_NullOrEmptyKey()
        {
            var session = E2ETestContext.CreateMockSession();

            Assert.Throws<ArgumentNullException>(() =>
            {
                session.Set("", "value");
            }, "Set with empty key must throw ArgumentNullException");

            Assert.Throws<ArgumentNullException>(() =>
            {
                session.Set(null!, "value");
            }, "Set with null key must throw ArgumentNullException");

            var nullResult = session.Get<string>("");
            Assert.Null(nullResult, "Get with empty key must return default/null");

            var nullResult2 = session.Get<string>(null!);
            Assert.Null(nullResult2, "Get with null key must return default/null");

            // RemoveKey should be safe no-op
            session.RemoveKey("");
            session.RemoveKey(null!);
        }

        [E2ETest(5, 2, "CH03: SessionExtensions Set with null value removes the key")]
        public void CH03_SessionExtensions_NullValue_RemovesKey()
        {
            var session = E2ETestContext.CreateMockSession();
            var testItem = new CartItem { IDSanPham = 1, TenSanPham = "Test", DonGia = 50000m };

            session.Set("CartKey", testItem);
            var retrieved = session.Get<CartItem>("CartKey");
            Assert.NotNull(retrieved, "Item must be set");

            session.Set<CartItem?>("CartKey", null);
            var retrievedAfterNull = session.Get<CartItem>("CartKey");
            Assert.Null(retrievedAfterNull, "Setting null value must remove key from session");
        }

        [E2ETest(5, 2, "CH04: SessionExtensions Get with corrupted JSON returns default without crashing")]
        public void CH04_SessionExtensions_CorruptedJson_ReturnsDefault()
        {
            var session = E2ETestContext.CreateMockSession();
            // Corrupt raw JSON bytes
            var corruptBytes = Encoding.UTF8.GetBytes("{ broken json \"\" : 1234 invalid");
            session.Set("CorruptedCart", corruptBytes);

            var result = session.Get<CartItem>("CorruptedCart");
            Assert.Null(result, "Corrupted JSON in session must return default and not throw unhandled exception");
        }

        public class CircularNode
        {
            public string Name { get; set; } = string.Empty;
            public CircularNode? SelfRef { get; set; }
        }

        [E2ETest(5, 2, "CH05: SessionExtensions circular reference object serialization survives")]
        public void CH05_SessionExtensions_CircularReference()
        {
            var session = E2ETestContext.CreateMockSession();
            var node = new CircularNode { Name = "Root" };
            node.SelfRef = node; // Cycle

            session.Set("CycleNode", node);
            var retrieved = session.Get<CircularNode>("CycleNode");
            Assert.NotNull(retrieved, "Serialization with ReferenceHandler.IgnoreCycles must survive cycles");
            Assert.Equal("Root", retrieved!.Name, "Deserialized root node name must match");
        }

        [E2ETest(5, 2, "CH06: SessionExtensions Vietnamese Unicode and special characters roundtrip")]
        public void CH06_SessionExtensions_VietnameseUnicode_RoundTrip()
        {
            var session = E2ETestContext.CreateMockSession();
            var complexItem = new CartItem
            {
                IDSanPham = 99,
                TenSanPham = "Áo Thun \"Trăng Sáng Langbiang 2026\" - 100% Cotton (Đặc Biệt & Hạn Chế)",
                ThongDiepGayQuy = "Đóng góp 50.000₫/áo vào quỹ quà tặng Trung thu cho 200 em nhỏ đồng bào K'Ho tại Langbiang! <3 ✨",
                DonGia = 129000.50m,
                SoLuong = 3,
                PhanTramDongGop = 100,
                LinkHinhAnh = "/assets/images/ao_thun_đặc_biệt.jpg?v=1&type=cotton"
            };

            var list = new List<CartItem> { complexItem };
            session.Set("ComplexCartList", list);

            var retrieved = session.Get<List<CartItem>>("ComplexCartList");
            Assert.NotNull(retrieved, "Retrieved cart list must not be null");
            Assert.Equal(1, retrieved!.Count, "Cart list should have 1 item");

            var item = retrieved[0];
            Assert.Equal(complexItem.TenSanPham, item.TenSanPham, "Unicode product name must match exactly");
            Assert.Equal(complexItem.ThongDiepGayQuy, item.ThongDiepGayQuy, "Unicode charity message must match exactly");
            Assert.Equal(complexItem.DonGia, item.DonGia, "Decimal DonGia must match");
            Assert.Equal(complexItem.ThanhTien, item.ThanhTien, "Calculated ThanhTien must match");
            Assert.Equal(complexItem.TienDongGop, item.TienDongGop, "Calculated TienDongGop must match");
        }

        [E2ETest(5, 2, "CH07: SessionExtensions GetOrCreate invokes factory only once")]
        public void CH07_SessionExtensions_GetOrCreate_FactoryInvocation()
        {
            var session = E2ETestContext.CreateMockSession();
            int factoryCalls = 0;

            var item1 = session.GetOrCreate("KeyCounter", () =>
            {
                factoryCalls++;
                return new CartItem { IDSanPham = 10, TenSanPham = "First" };
            });

            Assert.Equal(1, factoryCalls, "Factory should be called on first lookup");
            Assert.NotNull(item1, "Returned item must not be null");

            var item2 = session.GetOrCreate("KeyCounter", () =>
            {
                factoryCalls++;
                return new CartItem { IDSanPham = 20, TenSanPham = "Second" };
            });

            Assert.Equal(1, factoryCalls, "Factory should NOT be called on second lookup for existing key");
            Assert.Equal(10, item2.IDSanPham, "Returned item should be the previously stored item");
        }

        // =========================================================================
        // SECTION 2: DBINITIALIZER IN-MEMORY STORE & CRUD STRESS TESTS
        // =========================================================================

        [E2ETest(5, 4, "CH08: DbInitializer ResetInMemoryStore restores original state")]
        public void CH08_DbInitializer_ResetRestoresOriginalState()
        {
            DbInitializer.ResetInMemoryStore();
            var initialProducts = DbInitializer.GetInMemoryProducts();
            Assert.Equal(4, initialProducts.Count, "Initial in-memory products count must be 4");

            // Add a product and delete a product
            var added = DbInitializer.AddInMemoryProduct(new SanPhamGayQuy { TenSanPham = "Temp", GiaBan = 1000m });
            DbInitializer.DeleteInMemoryProduct(1);

            var alteredProducts = DbInitializer.GetInMemoryProducts();
            Assert.Equal(4, alteredProducts.Count, "Altered count (4 - 1 + 1 = 4)");
            Assert.Null(DbInitializer.GetInMemoryProductById(1), "Product 1 should be deleted");

            // Now reset
            DbInitializer.ResetInMemoryStore();
            var resetProducts = DbInitializer.GetInMemoryProducts();
            Assert.Equal(4, resetProducts.Count, "Products count after reset must be 4");
            Assert.NotNull(DbInitializer.GetInMemoryProductById(1), "Product 1 must be restored after reset");
            Assert.Null(DbInitializer.GetInMemoryProductById(added.IDSanPham), "Added temp product must be removed after reset");
        }

        [E2ETest(5, 4, "CH09: DbInitializer GetInMemoryProducts returns isolated clones")]
        public void CH09_DbInitializer_DeepCopy_ProtectionAgainstExternalMutation()
        {
            DbInitializer.ResetInMemoryStore();
            var p1 = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(p1, "Product 1 must exist");

            // Mutate retrieved product
            var originalName = p1!.TenSanPham;
            p1.TenSanPham = "MUTATED_BY_EXTERNAL_CALLER";

            var p1Fresh = DbInitializer.GetInMemoryProductById(1);
            Assert.Equal(originalName, p1Fresh!.TenSanPham, "External mutation must NOT affect the internal store product");
        }

        [E2ETest(5, 4, "CH10: DbInitializer AddInMemoryProduct auto-increments IDs and sets NgayTao")]
        public void CH10_DbInitializer_AddProduct_AutoIncrement()
        {
            DbInitializer.ResetInMemoryStore();

            var newProd1 = new SanPhamGayQuy
            {
                TenSanPham = "Nón Kết Tình Nguyện",
                GiaBan = 65000m,
                SoLuongTon = 50
            };
            var added1 = DbInitializer.AddInMemoryProduct(newProd1);
            Assert.Equal(5, added1.IDSanPham, "First added product ID must be 5");
            Assert.True(added1.NgayTao > DateTime.MinValue, "NgayTao must be set");

            var newProd2 = new SanPhamGayQuy
            {
                TenSanPham = "Huy Hiệu Tình Nguyện",
                GiaBan = 25000m,
                SoLuongTon = 100
            };
            var added2 = DbInitializer.AddInMemoryProduct(newProd2);
            Assert.Equal(6, added2.IDSanPham, "Second added product ID must be 6");

            var stored1 = DbInitializer.GetInMemoryProductById(5);
            Assert.NotNull(stored1, "Added product 5 must be retrievable");
            Assert.Equal("Nón Kết Tình Nguyện", stored1!.TenSanPham, "Product name must match");
        }

        [E2ETest(5, 4, "CH11: DbInitializer UpdateInMemoryProduct preserves existing image if empty")]
        public void CH11_DbInitializer_UpdateProduct_ImagePreservation()
        {
            DbInitializer.ResetInMemoryStore();
            var original = DbInitializer.GetInMemoryProductById(1);
            Assert.NotNull(original, "Product 1 must exist");
            var originalImage = original!.LinkHinhAnh;
            Assert.False(string.IsNullOrEmpty(originalImage), "Product 1 must have an initial image");

            // Update with empty image
            var updateModel = new SanPhamGayQuy
            {
                IDSanPham = 1,
                TenSanPham = "Áo Thun Cập Nhật Tên",
                GiaBan = 135000m,
                GiaGoc = 160000m,
                SoLuongTon = 80,
                DaBan = 40,
                DanhMuc = "Trang phục",
                LinkHinhAnh = "" // Empty image
            };

            var success = DbInitializer.UpdateInMemoryProduct(updateModel);
            Assert.True(success, "Update must return true for existing product");

            var updated = DbInitializer.GetInMemoryProductById(1);
            Assert.Equal("Áo Thun Cập Nhật Tên", updated!.TenSanPham, "Name should be updated");
            Assert.Equal(135000m, updated.GiaBan, "GiaBan should be updated");
            Assert.Equal(originalImage, updated.LinkHinhAnh, "Existing image must be preserved when empty image is passed");

            // Update non-existing product
            var nonExisting = new SanPhamGayQuy { IDSanPham = 99999, TenSanPham = "Fake" };
            var nonExistingResult = DbInitializer.UpdateInMemoryProduct(nonExisting);
            Assert.False(nonExistingResult, "Update must return false for non-existing product");
        }

        [E2ETest(5, 4, "CH12: DbInitializer DeleteInMemoryProduct lifecycle")]
        public void CH12_DbInitializer_DeleteProduct()
        {
            DbInitializer.ResetInMemoryStore();
            var success = DbInitializer.DeleteInMemoryProduct(2);
            Assert.True(success, "Delete existing product must return true");

            var lookup = DbInitializer.GetInMemoryProductById(2);
            Assert.Null(lookup, "Deleted product must no longer be found");

            var fail = DbInitializer.DeleteInMemoryProduct(99999);
            Assert.False(fail, "Delete non-existent product must return false");
        }

        [E2ETest(5, 4, "CH13: DbInitializer AddInMemoryOrder stock deduction and code format")]
        public void CH13_DbInitializer_OrderPlacement_StockDeduction()
        {
            DbInitializer.ResetInMemoryStore();
            var p1Before = DbInitializer.GetInMemoryProductById(1)!;
            var initialStock1 = p1Before.SoLuongTon;
            var initialSold1 = p1Before.DaBan;

            var p2Before = DbInitializer.GetInMemoryProductById(2)!;
            var initialStock2 = p2Before.SoLuongTon;
            var initialSold2 = p2Before.DaBan;

            var order = new DonHangGayQuy
            {
                HoTenNguoiMua = "Nguyễn Văn Test",
                SoDienThoai = "0901234567",
                DiaChiNhanHang = "123 Đường Tình Nguyện",
                TongTienHang = 208000m,
                TienUngHoThem = 20000m,
                TongThanhToan = 228000m,
                PhuongThucThanhToan = "ChuyenKhoanQR",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy { IDSanPham = 1, TenSanPham = p1Before.TenSanPham, DonGia = 129000m, SoLuong = 3, ThanhTien = 387000m },
                    new ChiTietDonHangGayQuy { IDSanPham = 2, TenSanPham = p2Before.TenSanPham, DonGia = 79000m, SoLuong = 2, ThanhTien = 158000m }
                }
            };

            var placedOrder = DbInitializer.AddInMemoryOrder(order);
            Assert.NotNull(placedOrder, "Order must be created");
            Assert.Equal(1, placedOrder.IDDonHang, "First order ID must be 1");
            Assert.True(placedOrder.MaDonHang.StartsWith("GQ2026-"), "Order code must start with GQ2026-");

            // Verify stock deduction on Product 1
            var p1After = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(initialStock1 - 3, p1After.SoLuongTon, "Product 1 stock must decrease by 3");
            Assert.Equal(initialSold1 + 3, p1After.DaBan, "Product 1 DaBan must increase by 3");

            // Verify stock deduction on Product 2
            var p2After = DbInitializer.GetInMemoryProductById(2)!;
            Assert.Equal(initialStock2 - 2, p2After.SoLuongTon, "Product 2 stock must decrease by 2");
            Assert.Equal(initialSold2 + 2, p2After.DaBan, "Product 2 DaBan must increase by 2");
        }

        [E2ETest(5, 4, "CH14: DbInitializer OrderStock floors at zero and does not become negative")]
        public void CH14_DbInitializer_StockFloorsAtZero()
        {
            DbInitializer.ResetInMemoryStore();
            var p = DbInitializer.GetInMemoryProductById(3)!;
            var currentStock = p.SoLuongTon;

            var excessiveOrder = new DonHangGayQuy
            {
                HoTenNguoiMua = "Người mua số lượng lớn",
                SoDienThoai = "0912345678",
                DiaChiNhanHang = "Hà Nội",
                ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                {
                    new ChiTietDonHangGayQuy { IDSanPham = 3, TenSanPham = p.TenSanPham, SoLuong = currentStock + 500 }
                }
            };

            DbInitializer.AddInMemoryOrder(excessiveOrder);
            var pAfter = DbInitializer.GetInMemoryProductById(3)!;
            Assert.Equal(0, pAfter.SoLuongTon, "Stock must floor at 0 and not become negative");
        }

        [E2ETest(5, 4, "CH15: DbInitializer GetInMemoryOrderByCode is case-insensitive")]
        public void CH15_DbInitializer_OrderLookup_CaseInsensitive()
        {
            DbInitializer.ResetInMemoryStore();
            var order = new DonHangGayQuy
            {
                MaDonHang = "GQ2026-CUSTOM-CODE-99",
                HoTenNguoiMua = "Lê Thị B",
                SoDienThoai = "0987654321",
                DiaChiNhanHang = "TP.HCM"
            };

            DbInitializer.AddInMemoryOrder(order);

            var exactLookup = DbInitializer.GetInMemoryOrderByCode("GQ2026-CUSTOM-CODE-99");
            Assert.NotNull(exactLookup, "Exact lookup must find order");

            var lowerLookup = DbInitializer.GetInMemoryOrderByCode("gq2026-custom-code-99");
            Assert.NotNull(lowerLookup, "Lower case lookup must find order");

            var nonExistent = DbInitializer.GetInMemoryOrderByCode("UNKNOWN_CODE");
            Assert.Null(nonExistent, "Non existent code must return null");
        }

        [E2ETest(5, 4, "CH16: DbInitializer concurrency stress test (100 parallel tasks)")]
        public void CH16_DbInitializer_ConcurrencyStressTest()
        {
            DbInitializer.ResetInMemoryStore();

            // Run 100 concurrent tasks performing mixed CRUD and order operations
            Parallel.For(0, 100, i =>
            {
                // Read operations
                var list = DbInitializer.GetInMemoryProducts();
                var p = DbInitializer.GetInMemoryProductById((i % 4) + 1);

                // Add product
                var added = DbInitializer.AddInMemoryProduct(new SanPhamGayQuy
                {
                    TenSanPham = $"Concurrent Product {i}",
                    GiaBan = 10000m + i,
                    SoLuongTon = 50
                });

                // Update product
                DbInitializer.UpdateInMemoryProduct(new SanPhamGayQuy
                {
                    IDSanPham = added.IDSanPham,
                    TenSanPham = $"Updated Concurrent Product {i}",
                    GiaBan = 20000m + i,
                    SoLuongTon = 40
                });

                // Add order
                var ord = DbInitializer.AddInMemoryOrder(new DonHangGayQuy
                {
                    HoTenNguoiMua = $"Concurrent Buyer {i}",
                    SoDienThoai = "0900000000",
                    DiaChiNhanHang = "Test Address",
                    ChiTietDonHangs = new List<ChiTietDonHangGayQuy>
                    {
                        new ChiTietDonHangGayQuy { IDSanPham = (i % 4) + 1, SoLuong = 1 }
                    }
                });

                // Lookup order
                var foundOrder = DbInitializer.GetInMemoryOrderById(ord.IDDonHang);
                Assert.NotNull(foundOrder, "Concurrent order lookup must succeed");

                // Delete the added product
                DbInitializer.DeleteInMemoryProduct(added.IDSanPham);
            });

            // Post-condition check: store is still consistent
            var finalList = DbInitializer.GetInMemoryProducts();
            Assert.True(finalList.Count >= 4, "Store must remain consistent with original products intact");
        }

        // =========================================================================
        // SECTION 3: VIEWMODELS & MODEL VALIDATION STRESS TESTS
        // =========================================================================

        [E2ETest(5, 3, "CH17: SanPhamCrudViewModel mapping roundtrip and trimming")]
        public void CH17_SanPhamCrudViewModel_MappingAndTrimming()
        {
            var entity = new SanPhamGayQuy
            {
                IDSanPham = 42,
                TenSanPham = "  Áo Phông Tình Nguyện  ",
                GiaBan = 120000m,
                GiaGoc = 150000m,
                LinkHinhAnh = "/assets/default.jpg",
                MoTaNgan = "  Mô tả ngắn  ",
                MoTaChiTiet = "  Mô tả chi tiết  ",
                ThongDiepGayQuy = "  Thông điệp nhân ái  ",
                PhanTramDongGop = 100,
                SoLuongTon = 50,
                DanhMuc = "  Trang phục  ",
                TrangThai = true,
                ThuTu = 3,
                IDChienDich = 7
            };

            // FromEntity
            var vm = SanPhamCrudViewModel.FromEntity(entity);
            Assert.Equal(42, vm.IDSanPham, "IDSanPham should map");
            Assert.Equal(entity.TenSanPham, vm.TenSanPham, "TenSanPham should map");
            Assert.Equal(entity.IDChienDich, vm.IDChienDich, "IDChienDich should map");

            // ToEntity with string trimming and image upload override
            var createdEntity = vm.ToEntity("/uploads/new_image.jpg");
            Assert.Equal("Áo Phông Tình Nguyện", createdEntity.TenSanPham, "TenSanPham must be trimmed");
            Assert.Equal("Mô tả ngắn", createdEntity.MoTaNgan, "MoTaNgan must be trimmed");
            Assert.Equal("/uploads/new_image.jpg", createdEntity.LinkHinhAnh, "Uploaded image must override existing link");
            Assert.Equal("Trang phục", createdEntity.DanhMuc, "DanhMuc must be trimmed");

            // UpdateEntity
            var targetEntity = new SanPhamGayQuy();
            vm.TenSanPham = "Tên Mới Sau Khi Sửa";
            vm.UpdateEntity(targetEntity, null);
            Assert.Equal("Tên Mới Sau Khi Sửa", targetEntity.TenSanPham, "Target entity TenSanPham must be updated");
            Assert.True(targetEntity.NgayCapNhat.HasValue, "NgayCapNhat must be set upon update");
        }

        [E2ETest(5, 3, "CH18: CartViewModel and CheckoutViewModel monetary calculation boundaries")]
        public void CH18_CartAndCheckout_MonetaryCalculations()
        {
            var cartVm = new CartViewModel
            {
                Items = new List<CartItem>
                {
                    new CartItem { IDSanPham = 1, DonGia = 100000m, SoLuong = 2, PhanTramDongGop = 50 }, // Line: 200,000, DongGop: 100,000
                    new CartItem { IDSanPham = 2, DonGia = 50000m, SoLuong = 3, PhanTramDongGop = 100 }  // Line: 150,000, DongGop: 150,000
                },
                PhiVanChuyen = 30000m,
                TienUngHoThem = 20000m
            };

            Assert.Equal(5, cartVm.TongSoLuong, "TongSoLuong must be 5");
            Assert.Equal(350000m, cartVm.TongTienHang, "TongTienHang must be 350,000");
            Assert.Equal(250000m, cartVm.TongTienDongGop, "TongTienDongGop must be 250,000");
            Assert.Equal(400000m, cartVm.TongThanhToan, "TongThanhToan must be 400,000 (350k + 30k + 20k)");
            Assert.True(cartVm.CoSanPham, "CoSanPham must be true");

            // Empty cart null safety
            var emptyCart = new CartViewModel { Items = null! };
            Assert.Equal(0, emptyCart.TongSoLuong, "Null items TongSoLuong should be 0");
            Assert.Equal(0m, emptyCart.TongTienHang, "Null items TongTienHang should be 0");
            Assert.False(emptyCart.CoSanPham, "Null items CoSanPham should be false");
        }

        // =========================================================================
        // SECTION 4: PROGRAM.CS CODE CONTRACT & PIPELINE EMPIRICAL INSPECTION
        // =========================================================================

        [E2ETest(5, 1, "CH19: Program.cs empirical pipeline ordering and code statement check")]
        public void CH19_ProgramCs_EmpiricalPipelineOrdering()
        {
            var programCsPath = Path.Combine(E2ETestContext.ProjectRoot, "Program.cs");
            Assert.True(File.Exists(programCsPath), "Program.cs must exist");
            var lines = File.ReadAllLines(programCsPath);

            // Find actual code statements (ignoring comment lines)
            int actualRoutingLine = -1;
            int actualSessionLine = -1;
            int actualAuthLine = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("//")) continue;

                if (line.Contains("app.UseRouting()")) actualRoutingLine = i;
                if (line.Contains("app.UseSession()")) actualSessionLine = i;
                if (line.Contains("app.UseAuthorization()")) actualAuthLine = i;
            }

            Assert.True(actualRoutingLine > 0, "app.UseRouting() must be present");
            Assert.True(actualSessionLine > 0, "app.UseSession() must be present");
            Assert.True(actualAuthLine > 0, "app.UseAuthorization() must be present");

            Assert.True(actualSessionLine > actualRoutingLine, "In code execution: app.UseSession() must come after app.UseRouting()");
            Assert.True(actualSessionLine < actualAuthLine, "In code execution: app.UseSession() must come before app.UseAuthorization()");
        }
    }
}
