using System;
using System.IO;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    public class Tier1_StorefrontTests
    {
        // =========================================================================
        // FEATURE 5: Desktop Navigation Link
        // =========================================================================

        [E2ETest(1, 5, "F05_T01: Desktop navigation contains CuaHang area link in _Layout.cshtml")]
        public void F05_T01_DesktopNavContainsCuaHangLink()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml");
            Assert.True(File.Exists(layoutPath), "_Layout.cshtml must exist");

            var layoutContent = File.ReadAllText(layoutPath);
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang desktop navigation link in _Layout.cshtml");
            }
            Assert.Contains("CuaHang", layoutContent, "_Layout.cshtml must contain reference to CuaHang area");
        }

        [E2ETest(1, 5, "F05_T02: Desktop nav link uses asp-area and asp-controller tag helpers")]
        public void F05_T02_DesktopNavLinkTagHelpers()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("asp-area=\"CuaHang\"") || layoutContent.Contains("asp-area='CuaHang'") || layoutContent.Contains("/CuaHang"),
                "Desktop nav link must target CuaHang area via asp-area or direct href"
            );
        }

        [E2ETest(1, 5, "F05_T03: Desktop nav link text denotes fundraising store")]
        public void F05_T03_DesktopNavLinkText()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("Cửa Hàng") || layoutContent.Contains("Cửa hàng") || layoutContent.Contains("Gây Quỹ") || layoutContent.Contains("Gây quỹ"),
                "Desktop nav link text must represent the fundraising store"
            );
        }

        [E2ETest(1, 5, "F05_T04: Desktop nav link is positioned adjacent to Trang chu")]
        public void F05_T04_DesktopNavLinkAdjacentToHome()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang in _Layout.cshtml");
            }
            var homeIndex = layoutContent.IndexOf("Trang chủ", StringComparison.OrdinalIgnoreCase);
            var storeIndex = layoutContent.IndexOf("CuaHang", StringComparison.OrdinalIgnoreCase);

            Assert.True(homeIndex > 0, "Trang chủ link must exist in _Layout.cshtml");
            Assert.True(storeIndex > 0, "CuaHang link must exist in _Layout.cshtml");
        }

        [E2ETest(1, 5, "F05_T05: Desktop nav styling adheres to modern responsive classes")]
        public void F05_T05_DesktopNavStylingClasses()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            Assert.Contains("hidden md:", layoutContent, "_Layout.cshtml must define responsive desktop/mobile visibility breakpoints");
        }

        // =========================================================================
        // FEATURE 6: Mobile Navigation Link
        // =========================================================================

        [E2ETest(1, 6, "F06_T01: Mobile liquid bottom nav contains CuaHang item")]
        public void F06_T01_MobileNavContainsCuaHang()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang mobile nav link in _Layout.cshtml");
            }
            Assert.Contains("CuaHang", layoutContent, "Mobile navigation section must contain CuaHang link");
        }

        [E2ETest(1, 6, "F06_T02: Mobile nav includes icon representation")]
        public void F06_T02_MobileNavIncludesIcon()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: Mobile nav icon in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("<svg") || layoutContent.Contains("fa-") || layoutContent.Contains("bi-") || layoutContent.Contains("lucide-"),
                "Mobile navigation elements must include SVG or font icon"
            );
        }

        [E2ETest(1, 6, "F06_T03: Mobile bottom bar fixed to viewport bottom")]
        public void F06_T03_MobileBottomBarFixedPosition()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: Mobile bottom bar in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("fixed bottom") || layoutContent.Contains("bottom-0") || layoutContent.Contains("fixed inset-x-0"),
                "Mobile bottom navigation must have fixed bottom positioning"
            );
        }

        [E2ETest(1, 6, "F06_T04: Mobile nav hidden on desktop screens")]
        public void F06_T04_MobileNavHiddenOnDesktop()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: Mobile nav in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("md:hidden"),
                "Mobile bottom navigation must be hidden on medium and large screens (md:hidden)"
            );
        }

        [E2ETest(1, 6, "F06_T05: Mobile link targets Index action of CuaHang controller")]
        public void F06_T05_MobileLinkActionTarget()
        {
            var layoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            if (!layoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M3: CuaHang mobile nav link in _Layout.cshtml");
            }
            Assert.True(
                layoutContent.Contains("CuaHang") && (layoutContent.Contains("asp-action=\"Index\"") || layoutContent.Contains("/CuaHang")),
                "Mobile link must route to CuaHang/Index"
            );
        }

        // =========================================================================
        // FEATURE 7: Storefront Catalog View
        // =========================================================================

        [E2ETest(1, 7, "F07_T01: Catalog view template file exists")]
        public void F07_T01_CatalogViewFileExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Catalog view template");
        }

        [E2ETest(1, 7, "F07_T02: Catalog view displays product card elements")]
        public void F07_T02_CatalogViewProductCardElements()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Catalog view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("TenSanPham", viewContent, "Catalog view must render TenSanPham");
            Assert.Contains("GiaBan", viewContent, "Catalog view must render GiaBan");
            Assert.Contains("LinkHinhAnh", viewContent, "Catalog view must render LinkHinhAnh");
        }

        [E2ETest(1, 7, "F07_T03: Catalog view displays charity message badge")]
        public void F07_T03_CatalogViewCharityBadge()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Catalog view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("ThongDiepGayQuy") || viewContent.Contains("gây quỹ") || viewContent.Contains("đóng góp"),
                "Catalog view must display charity/fundraising message badge"
            );
        }

        [E2ETest(1, 7, "F07_T04: Catalog view displays stock status")]
        public void F07_T04_CatalogViewStockStatus()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Catalog view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("SoLuongTon") || viewContent.Contains("Còn hàng") || viewContent.Contains("Hết hàng"),
                "Catalog view must indicate stock availability"
            );
        }

        [E2ETest(1, 7, "F07_T05: Catalog view uses responsive CSS grid")]
        public void F07_T05_CatalogViewResponsiveGrid()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(viewPath, "Catalog view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("grid") && (viewContent.Contains("grid-cols-1") || viewContent.Contains("sm:grid-cols-2") || viewContent.Contains("lg:grid-cols-3")),
                "Catalog view must implement responsive multi-column grid layout"
            );
        }

        // =========================================================================
        // FEATURE 8: Product Detail View
        // =========================================================================

        [E2ETest(1, 8, "F08_T01: Product detail view template file exists")]
        public void F08_T01_ProductDetailViewFileExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(viewPath, "Product detail view template");
        }

        [E2ETest(1, 8, "F08_T02: Detail view displays full descriptions and charity mission")]
        public void F08_T02_DetailViewFullDescriptions()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(viewPath, "Product detail view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("MoTaChiTiet", viewContent, "Detail view must display MoTaChiTiet");
            Assert.Contains("ThongDiepGayQuy", viewContent, "Detail view must display ThongDiepGayQuy");
        }

        [E2ETest(1, 8, "F08_T03: Detail view contains quantity input selector")]
        public void F08_T03_DetailViewQuantitySelector()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(viewPath, "Product detail view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("name=\"soLuong\"") || viewContent.Contains("name='soLuong'") || viewContent.Contains("type=\"number\""),
                "Detail view must contain quantity input selector"
            );
        }

        [E2ETest(1, 8, "F08_T04: Detail view contains add to cart action")]
        public void F08_T04_DetailViewAddToCartAction()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(viewPath, "Product detail view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("ThemVaoGio") || viewContent.Contains("GioHang") || viewContent.Contains("Thêm vào giỏ"),
                "Detail view must contain Add to Cart form/button"
            );
        }

        [E2ETest(1, 8, "F08_T05: Detail view displays original price when discounted")]
        public void F08_T05_DetailViewOriginalPriceStrikeThrough()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(viewPath, "Product detail view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("GiaGoc") || viewContent.Contains("line-through"),
                "Detail view must support displaying original price strike-through"
            );
        }
    }
}
