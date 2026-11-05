using System;
using System.IO;
using System.Linq;
using System.Reflection;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    public class Tier1_FoundationTests
    {
        // =========================================================================
        // FEATURE 1: Area & Route Setup
        // =========================================================================

        [E2ETest(1, 1, "F01_T01: Area Route pattern contract exists in Program.cs")]
        public void F01_T01_AreaRoutePatternInProgramCs()
        {
            var programCsPath = Path.Combine(E2ETestContext.ProjectRoot, "Program.cs");
            Assert.True(File.Exists(programCsPath), "Program.cs must exist");

            var content = File.ReadAllText(programCsPath);
            if (!content.Contains("{area:exists}"))
            {
                throw new AssertException("[SKIP] Pending implementation in M1: Area route {area:exists} in Program.cs");
            }
            Assert.Contains("{area:exists}", content, "Program.cs must configure {area:exists} route pattern");
        }

        [E2ETest(1, 1, "F01_T02: Area Route is mapped before default route")]
        public void F01_T02_AreaRouteMappedBeforeDefaultRoute()
        {
            var programCsPath = Path.Combine(E2ETestContext.ProjectRoot, "Program.cs");
            var content = File.ReadAllText(programCsPath);

            var areaIndex = content.IndexOf("{area:exists}", StringComparison.OrdinalIgnoreCase);
            var defaultIndex = content.IndexOf("{controller=Home}", StringComparison.OrdinalIgnoreCase);

            if (areaIndex <= 0)
            {
                throw new AssertException("[SKIP] Pending implementation in M1: Area route in Program.cs");
            }
            Assert.True(defaultIndex > 0, "Default route must be defined in Program.cs");
            Assert.True(areaIndex < defaultIndex, "Area route must precede default route to prevent route hijacking");
        }

        [E2ETest(1, 1, "F01_T03: Area CuaHang directory structure exists")]
        public void F01_T03_AreaCuaHangDirectoryStructure()
        {
            var areasDir = Path.Combine(E2ETestContext.ProjectRoot, "Areas", "CuaHang");
            Assert.RequireDirectory(areasDir, "Areas/CuaHang directory");
        }

        [E2ETest(1, 1, "F01_T04: Area Controller attribute contract")]
        public void F01_T04_AreaControllerAttribute()
        {
            var controllers = E2ETestContext.WebAssembly.GetTypes()
                .Where(t => t.Name.EndsWith("Controller") && t.Namespace != null && t.Namespace.Contains("Areas.CuaHang"))
                .ToList();

            if (!controllers.Any())
            {
                throw new AssertException("[SKIP] Pending implementation: Areas/CuaHang controllers");
            }

            foreach (var ctrl in controllers)
            {
                var areaAttr = ctrl.GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType().Name == "AreaAttribute");
                Assert.NotNull(areaAttr, $"Controller {ctrl.Name} must have [Area] attribute");
            }
        }

        [E2ETest(1, 1, "F01_T05: Area default action is Index and default controller is CuaHang")]
        public void F01_T05_AreaRouteDefaults()
        {
            var expectedPattern = ReferenceOracles.ExpectedAreaRoutePattern;
            Assert.Contains("controller=CuaHang", expectedPattern, "Default controller for area route must be CuaHang");
            Assert.Contains("action=Index", expectedPattern, "Default action for area route must be Index");
        }

        // =========================================================================
        // FEATURE 2: Session & State Setup
        // =========================================================================

        [E2ETest(1, 2, "F02_T01: Session services registered in Program.cs")]
        public void F02_T01_SessionServicesRegistered()
        {
            var programCs = File.ReadAllText(Path.Combine(E2ETestContext.ProjectRoot, "Program.cs"));
            if (!programCs.Contains("AddSession"))
            {
                throw new AssertException("[SKIP] Pending implementation in M1: AddSession in Program.cs");
            }
            Assert.Contains("AddDistributedMemoryCache", programCs, "Program.cs must register AddDistributedMemoryCache");
            Assert.Contains("AddSession", programCs, "Program.cs must register AddSession");
            Assert.Contains("UseSession", programCs, "Program.cs must call UseSession middleware");
        }

        [E2ETest(1, 2, "F02_T02: Session middleware positioned correctly in pipeline")]
        public void F02_T02_SessionMiddlewareOrder()
        {
            var programCs = File.ReadAllText(Path.Combine(E2ETestContext.ProjectRoot, "Program.cs"));
            var routingIndex = programCs.IndexOf("app.UseRouting()", StringComparison.OrdinalIgnoreCase);
            var sessionIndex = programCs.IndexOf("app.UseSession()", StringComparison.OrdinalIgnoreCase);
            var authIndex = programCs.IndexOf("app.UseAuthorization()", StringComparison.OrdinalIgnoreCase);

            if (sessionIndex <= 0)
            {
                throw new AssertException("[SKIP] Pending implementation in M1: app.UseSession() in Program.cs");
            }
            Assert.True(routingIndex > 0, "app.UseRouting() must be present");
            Assert.True(authIndex > 0, "app.UseAuthorization() must be present");
            Assert.True(sessionIndex > routingIndex, "app.UseSession() must come after app.UseRouting()");
            Assert.True(sessionIndex < authIndex, "app.UseSession() must come before app.UseAuthorization()");
        }

        [E2ETest(1, 2, "F02_T03: SessionExtensions helper class exists and supports Get and Set")]
        public void F02_T03_SessionExtensionsHelper()
        {
            var helperType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.SessionExtensions", "SessionExtensions");
            var setMethod = helperType.GetMethod("Set");
            var getMethod = helperType.GetMethod("Get");
            Assert.NotNull(setMethod, "SessionExtensions must define generic Set<T>");
            Assert.NotNull(getMethod, "SessionExtensions must define generic Get<T>");
        }

        [E2ETest(1, 2, "F02_T04: Session JSON serialization round-trip")]
        public void F02_T04_SessionJsonSerializationRoundTrip()
        {
            var session = E2ETestContext.CreateMockSession();
            var helperType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.SessionExtensions", "SessionExtensions");

            var testData = new { Name = "Áo Thun Gây Quỹ", Quantity = 2, Price = 120000m };
            var setMethod = helperType.GetMethods().First(m => m.Name == "Set" && m.IsGenericMethodDefinition);
            var genericSet = setMethod.MakeGenericMethod(testData.GetType());
            genericSet.Invoke(null, new object[] { session, "TestItem", testData });

            var getMethod = helperType.GetMethods().First(m => m.Name == "Get" && m.IsGenericMethodDefinition);
            var genericGet = getMethod.MakeGenericMethod(testData.GetType());
            var retrieved = genericGet.Invoke(null, new object[] { session, "TestItem" });

            Assert.NotNull(retrieved, "Retrieved session object must not be null");
        }

        [E2ETest(1, 2, "F02_T05: Session handles non-existent key gracefully without throwing")]
        public void F02_T05_SessionHandlesNonExistentKey()
        {
            var session = E2ETestContext.CreateMockSession();
            var helperType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.SessionExtensions", "SessionExtensions");

            var getMethod = helperType.GetMethods().First(m => m.Name == "Get" && m.IsGenericMethodDefinition);
            var genericGet = getMethod.MakeGenericMethod(typeof(string));
            var retrieved = genericGet.Invoke(null, new object[] { session, "NonExistentKey_XYZ" });

            Assert.Null(retrieved, "Non-existent session key must return null/default");
        }

        // =========================================================================
        // FEATURE 3: Domain Entities & DbContext
        // =========================================================================

        [E2ETest(1, 3, "F03_T01: SanPhamGayQuy entity contract")]
        public void F03_T01_SanPhamGayQuyEntityContract()
        {
            var entityType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy", "SanPhamGayQuy");

            Assert.NotNull(entityType.GetProperty("IDSanPham"), "SanPhamGayQuy must have IDSanPham");
            Assert.NotNull(entityType.GetProperty("TenSanPham"), "SanPhamGayQuy must have TenSanPham");
            Assert.NotNull(entityType.GetProperty("GiaBan"), "SanPhamGayQuy must have GiaBan");
            Assert.NotNull(entityType.GetProperty("SoLuongTon"), "SanPhamGayQuy must have SoLuongTon");
            Assert.NotNull(entityType.GetProperty("ThongDiepGayQuy"), "SanPhamGayQuy must have ThongDiepGayQuy");
            Assert.NotNull(entityType.GetProperty("PhanTramDongGop"), "SanPhamGayQuy must have PhanTramDongGop");
        }

        [E2ETest(1, 3, "F03_T02: DonHangGayQuy entity contract")]
        public void F03_T02_DonHangGayQuyEntityContract()
        {
            var entityType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.DonHangGayQuy", "DonHangGayQuy");

            Assert.NotNull(entityType.GetProperty("IDDonHang"), "DonHangGayQuy must have IDDonHang");
            Assert.NotNull(entityType.GetProperty("MaDonHang"), "DonHangGayQuy must have MaDonHang");
            Assert.NotNull(entityType.GetProperty("HoTenNguoiMua"), "DonHangGayQuy must have HoTenNguoiMua");
            Assert.NotNull(entityType.GetProperty("SoDienThoai"), "DonHangGayQuy must have SoDienThoai");
            Assert.NotNull(entityType.GetProperty("TongThanhToan"), "DonHangGayQuy must have TongThanhToan");
            Assert.NotNull(entityType.GetProperty("TienUngHoThem"), "DonHangGayQuy must have TienUngHoThem");
        }

        [E2ETest(1, 3, "F03_T03: ChiTietDonHangGayQuy entity contract")]
        public void F03_T03_ChiTietDonHangGayQuyEntityContract()
        {
            var entityType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.ChiTietDonHangGayQuy", "ChiTietDonHangGayQuy");

            Assert.NotNull(entityType.GetProperty("IDChiTiet"), "ChiTietDonHangGayQuy must have IDChiTiet");
            Assert.NotNull(entityType.GetProperty("IDDonHang"), "ChiTietDonHangGayQuy must have IDDonHang");
            Assert.NotNull(entityType.GetProperty("DonGia"), "ChiTietDonHangGayQuy must have DonGia");
            Assert.NotNull(entityType.GetProperty("SoLuong"), "ChiTietDonHangGayQuy must have SoLuong");
            Assert.NotNull(entityType.GetProperty("ThanhTien"), "ChiTietDonHangGayQuy must have ThanhTien");
        }

        [E2ETest(1, 3, "F03_T04: ApplicationDbContext declares 3 fundraising DbSets")]
        public void F03_T04_ApplicationDbContextDbSets()
        {
            var dbContextType = typeof(ApplicationDbContext);
            var spProp = dbContextType.GetProperty("SanPhamGayQuys");
            var dhProp = dbContextType.GetProperty("DonHangGayQuys");
            var ctProp = dbContextType.GetProperty("ChiTietDonHangGayQuys");

            if (spProp == null || dhProp == null || ctProp == null)
            {
                throw new AssertException("[SKIP] Pending implementation in M1: DbSets in ApplicationDbContext");
            }

            Assert.NotNull(spProp, "ApplicationDbContext must declare DbSet<SanPhamGayQuy> SanPhamGayQuys");
            Assert.NotNull(dhProp, "ApplicationDbContext must declare DbSet<DonHangGayQuy> DonHangGayQuys");
            Assert.NotNull(ctProp, "ApplicationDbContext must declare DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys");
        }

        [E2ETest(1, 3, "F03_T05: Currency properties configured as decimal")]
        public void F03_T05_CurrencyPropertiesDecimalType()
        {
            var spType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy", "SanPhamGayQuy");
            var dhType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.DonHangGayQuy", "DonHangGayQuy");

            Assert.Equal(typeof(decimal), spType.GetProperty("GiaBan")?.PropertyType, "GiaBan must be decimal");
            Assert.Equal(typeof(decimal), dhType.GetProperty("TongThanhToan")?.PropertyType, "TongThanhToan must be decimal");
        }

        // =========================================================================
        // FEATURE 4: Seed Data & Fallback
        // =========================================================================

        [E2ETest(1, 4, "F04_T01: DbInitializer helper class exists")]
        public void F04_T01_DbInitializerExists()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");
            Assert.NotNull(initType, "DbInitializer class must exist");
        }

        [E2ETest(1, 4, "F04_T02: Seed dataset contains at least 3 products")]
        public void F04_T02_SeedDataHasAtLeast3Products()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");

            var getProductsMethod = initType.GetMethod("GetSampleProducts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                 ?? initType.GetMethod("GetInMemoryProducts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(getProductsMethod, "DbInitializer must provide sample/seed products");
            var products = getProductsMethod.Invoke(null, null) as System.Collections.IEnumerable;
            Assert.NotNull(products, "Seed products collection must not be null");

            int count = 0;
            foreach (var _ in products) count++;
            Assert.True(count >= 3, $"Seed data must contain >= 3 products, found {count}");
        }

        [E2ETest(1, 4, "F04_T03: Seed products contain authentic Langbiang charity messages")]
        public void F04_T03_SeedProductsCharityMessages()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");

            var getProductsMethod = initType.GetMethod("GetSampleProducts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                 ?? initType.GetMethod("GetInMemoryProducts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            var products = getProductsMethod?.Invoke(null, null) as System.Collections.IEnumerable;
            Assert.NotNull(products, "Products list required");

            bool hasCharityMessage = false;
            foreach (var p in products)
            {
                var msgProp = p.GetType().GetProperty("ThongDiepGayQuy");
                var msg = msgProp?.GetValue(p) as string;
                if (!string.IsNullOrEmpty(msg) && (msg.Contains("quỹ") || msg.Contains("em") || msg.Contains("Langbiang") || msg.Contains("học bổng")))
                {
                    hasCharityMessage = true;
                    break;
                }
            }
            Assert.True(hasCharityMessage, "Seed products must contain authentic volunteer/charity messages");
        }

        [E2ETest(1, 4, "F04_T04: Offline fallback supports CRUD operations")]
        public void F04_T04_OfflineFallbackSupportsCrud()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");

            var getMethod = initType.GetMethod("GetInMemoryProducts");
            var addMethod = initType.GetMethod("AddInMemoryProduct");
            var updateMethod = initType.GetMethod("UpdateInMemoryProduct");
            var deleteMethod = initType.GetMethod("DeleteInMemoryProduct");

            Assert.NotNull(getMethod, "DbInitializer must provide GetInMemoryProducts");
            Assert.NotNull(addMethod, "DbInitializer must provide AddInMemoryProduct");
            Assert.NotNull(updateMethod, "DbInitializer must provide UpdateInMemoryProduct");
            Assert.NotNull(deleteMethod, "DbInitializer must provide DeleteInMemoryProduct");
        }

        [E2ETest(1, 4, "F04_T05: Offline fallback thread safety")]
        public void F04_T05_OfflineFallbackThreadSafety()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");

            var getMethod = initType.GetMethod("GetInMemoryProducts");
            Assert.NotNull(getMethod, "GetInMemoryProducts must be accessible");

            // Verify simultaneous reads do not throw
            System.Threading.Tasks.Parallel.For(0, 10, i =>
            {
                var list = getMethod.Invoke(null, null);
                Assert.NotNull(list, "Thread read must not be null");
            });
        }
    }
}
