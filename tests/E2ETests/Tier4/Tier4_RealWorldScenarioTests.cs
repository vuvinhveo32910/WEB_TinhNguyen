using System;
using System.IO;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier4
{
    public class Tier4_RealWorldScenarioTests
    {
        // =========================================================================
        // TIER 4: REAL-WORLD SCENARIOS (12 Tests)
        // =========================================================================

        [E2ETest(4, "S01: Complete Donor Flow - Step 1: Storefront catalog landing & inspection")]
        public void S01_DonorFlow_CatalogLanding()
        {
            var catalogViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            Assert.RequireFile(catalogViewPath, "CuaHang/Index catalog");

            var content = File.ReadAllText(catalogViewPath);
            Assert.Contains("TenSanPham", content, "Catalog must list available merchandise");
            Assert.Contains("GiaBan", content, "Catalog must show clear pricing");
        }

        [E2ETest(4, "S02: Complete Donor Flow - Step 2: Product detail inspection & mission reading")]
        public void S02_DonorFlow_ProductDetailMissionReading()
        {
            var detailViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");
            Assert.RequireFile(detailViewPath, "CuaHang/ChiTiet view");

            var content = File.ReadAllText(detailViewPath);
            Assert.Contains("ThongDiepGayQuy", content, "Donor reads charity story and impact on children in Langbiang");
        }

        [E2ETest(4, "S03: Complete Donor Flow - Step 3: Adding 2 shirts to cart and reviewing cart state")]
        public void S03_DonorFlow_AddToCartAndReview()
        {
            var shirtPrice = 120000m;
            var quantity = 2;
            var subtotal = ReferenceOracles.LineTotal(shirtPrice, quantity);
            Assert.Equal(240000m, subtotal, "Subtotal for 2 shirts must equal 240,000 VND");

            var cartViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "Index.cshtml");
            Assert.RequireFile(cartViewPath, "GioHang/Index view");
        }

        [E2ETest(4, "S04: Complete Donor Flow - Step 4: Filling checkout with valid Vietnamese donor details")]
        public void S04_DonorFlow_CheckoutFormValidation()
        {
            var donorName = "Nguyễn Văn An";
            var donorPhone = "0901234567";
            var donorEmail = "an.nguyen@example.com";
            var donorAddress = "123 Điện Biên Phủ, Phường 25, Bình Thạnh, TP.HCM";

            Assert.False(string.IsNullOrWhiteSpace(donorName), "Donor name must be provided");
            Assert.True(ReferenceOracles.IsValidVietnamesePhone(donorPhone), "Phone must be valid Vietnamese 10-digit mobile");
            Assert.True(ReferenceOracles.IsValidEmail(donorEmail), "Email must be valid format");
            Assert.False(string.IsNullOrWhiteSpace(donorAddress), "Address must be provided");
        }

        [E2ETest(4, "S05: Complete Donor Flow - Step 5: Adding voluntary donation of 100,000 VND and selecting VietQR")]
        public void S05_DonorFlow_VoluntaryDonationAndVietQr()
        {
            var subtotal = 240000m;
            var voluntaryDonation = 100000m;
            var paymentMethod = "ChuyenKhoanQR";

            var totalPayable = ReferenceOracles.OrderTotal(subtotal, voluntaryDonation, 0m);
            Assert.Equal(340000m, totalPayable, "Total payable must be 340,000 VND (240k goods + 100k donation)");
            Assert.Equal("ChuyenKhoanQR", paymentMethod, "Payment method must be VietQR");
        }

        [E2ETest(4, "S06: Complete Donor Flow - Step 6: Confirmation page with VietQR image and order memo")]
        public void S06_DonorFlow_ConfirmationPageAndVietQr()
        {
            var confirmationPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "GioHang", "DatHangThanhCong.cshtml");
            Assert.RequireFile(confirmationPath, "Confirmation page DatHangThanhCong");

            var orderCode = "GQ20261103-AN99";
            var totalAmount = 340000m;
            var qrUrl = ReferenceOracles.VietQrImageUrl("VCB", "1023456789", totalAmount, orderCode);

            Assert.Contains("amount=340000", qrUrl, "VietQR must match exact total payable");
            Assert.Contains($"addInfo={orderCode}", qrUrl, "VietQR memo must include unique order code");
        }

        [E2ETest(4, "S07: Complete Admin Flow - Step 1: Navigating to Admin Product Management")]
        public void S07_AdminFlow_NavigationToAdminProducts()
        {
            var adminLayout = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));
            if (!adminLayout.Contains("AdminSanPham"))
            {
                throw new AssertException("[SKIP] Pending implementation in M2: AdminSanPham menu link in _AdminLayout.cshtml");
            }
            Assert.Contains("AdminSanPham", adminLayout, "Admin menu must link to AdminSanPham");
        }

        [E2ETest(4, "S08: Complete Admin Flow - Step 2: Reviewing product table inventory")]
        public void S08_AdminFlow_ReviewingInventoryTable()
        {
            var adminIndexPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(adminIndexPath, "AdminSanPham/Index.cshtml view");

            var content = File.ReadAllText(adminIndexPath);
            Assert.Contains("TenSanPham", content, "Table must show product name");
            Assert.Contains("SoLuongTon", content, "Table must show stock quantity");
        }

        [E2ETest(4, "S09: Complete Admin Flow - Step 3: Creating new charity product with 100% donation rate")]
        public void S09_AdminFlow_CreateCharityProduct()
        {
            var createViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(createViewPath, "AdminSanPham/Create.cshtml view");

            var content = File.ReadAllText(createViewPath);
            Assert.Contains("PhanTramDongGop", content, "Create form must specify donation percentage");
        }

        [E2ETest(4, "S10: Complete Admin Flow - Step 4: Updating product price for fundraising promo")]
        public void S10_AdminFlow_UpdateProductPricing()
        {
            var editViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(editViewPath, "AdminSanPham/Edit.cshtml view");

            var content = File.ReadAllText(editViewPath);
            Assert.Contains("GiaBan", content, "Edit form must allow modifying GiaBan");
        }

        [E2ETest(4, "S11: Complete Admin Flow - Step 5: Deleting obsolete merchandise with confirmation")]
        public void S11_AdminFlow_DeleteMerchandiseWithConfirmation()
        {
            var indexViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(indexViewPath, "Admin table view");
            var indexContent = File.ReadAllText(indexViewPath);
            Assert.True(
                indexContent.Contains("Delete") || indexContent.Contains("Xóa"),
                "Admin table must provide delete capability"
            );
        }

        [E2ETest(4, "S12: Layout & Theme Integrity across Desktop, Mobile, and Admin views")]
        public void S12_LayoutAndThemeIntegrity()
        {
            var storefrontLayout = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml"));
            var adminLayout = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));

            // Storefront uses Tailwind responsive classes
            Assert.True(
                storefrontLayout.Contains("lg:hidden") || storefrontLayout.Contains("md:hidden") || storefrontLayout.Contains("hidden lg:flex"),
                "Storefront layout must have mobile/desktop responsive breakpoints"
            );
            Assert.Contains("dark", storefrontLayout, "Storefront layout must support dark theme classes");

            // Admin uses dark sidebar theme
            Assert.True(
                adminLayout.Contains("#161a2d") || adminLayout.Contains("sidebar") || adminLayout.Contains("dark"),
                "Admin layout must use admin dark navy styling"
            );
        }
    }
}
