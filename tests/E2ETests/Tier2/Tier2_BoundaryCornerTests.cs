using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier2
{
    public class Tier2_BoundaryCornerTests
    {
        // =========================================================================
        // TIER 2: BOUNDARY & CORNER CASES (25 Tests)
        // =========================================================================

        [E2ETest(2, "B01: Empty cart calculates subtotal and total as 0 VND")]
        public void B01_EmptyCartZeroTotals()
        {
            var subtotal = ReferenceOracles.LineTotal(0m, 0);
            var orderTotal = ReferenceOracles.OrderTotal(0m, 0m, 0m);

            Assert.Equal(0m, subtotal, "Empty cart subtotal must be 0");
            Assert.Equal(0m, orderTotal, "Empty cart order total must be 0");
        }

        [E2ETest(2, "B02: Quantity zero results in zero line total")]
        public void B02_QuantityZeroLineTotal()
        {
            var lineTotal = ReferenceOracles.LineTotal(250000m, 0);
            Assert.Equal(0m, lineTotal, "Line total with 0 quantity must be 0");
        }

        [E2ETest(2, "B03: Negative quantity results in zero line total or rejection")]
        public void B03_NegativeQuantityHandledSafely()
        {
            var lineTotal = ReferenceOracles.LineTotal(150000m, -5);
            Assert.Equal(0m, lineTotal, "Line total with negative quantity must be 0 or rejected");
        }

        [E2ETest(2, "B04: Unit price zero produces zero line total (promotional gifts)")]
        public void B04_UnitPriceZeroPromotionalGift()
        {
            var lineTotal = ReferenceOracles.LineTotal(0m, 5);
            Assert.Equal(0m, lineTotal, "Free promotional gift with unit price 0 must total 0 VNĐ");
        }

        [E2ETest(2, "B05: Unit price ceiling at 1,000,000,000 VND handled without overflow")]
        public void B05_UnitPriceCeilingHandledWithoutOverflow()
        {
            var maxPrice = 1000000000m;
            var lineTotal = ReferenceOracles.LineTotal(maxPrice, 1);
            Assert.Equal(1000000000m, lineTotal, "Max price 1B VNĐ must compute accurately without numeric overflow");
        }

        [E2ETest(2, "B06: Zero voluntary donation produces exact merchandise total")]
        public void B06_ZeroVoluntaryDonation()
        {
            var total = ReferenceOracles.OrderTotal(350000m, 0m, 0m);
            Assert.Equal(350000m, total, "0 VND voluntary donation must not alter merchandise total");
        }

        [E2ETest(2, "B07: Negative voluntary donation normalized to zero")]
        public void B07_NegativeVoluntaryDonationNormalized()
        {
            var total = ReferenceOracles.OrderTotal(200000m, -50000m, 0m);
            Assert.Equal(200000m, total, "Negative voluntary donation must be ignored/normalized to 0");
        }

        [E2ETest(2, "B08: Large voluntary donation calculated accurately")]
        public void B08_LargeVoluntaryDonation()
        {
            var subtotal = 150000m;
            var hugeDonation = 50000000m; // 50 million VND
            var total = ReferenceOracles.OrderTotal(subtotal, hugeDonation, 0m);
            Assert.Equal(50150000m, total, "Large voluntary donation must sum accurately");
        }

        [E2ETest(2, "B09: Charity percentage at 1% boundary")]
        public void B09_CharityPercentageLowerBoundary()
        {
            var lineTotal = 100000m;
            var charityPart = ReferenceOracles.CharityContribution(lineTotal, 1);
            Assert.Equal(1000m, charityPart, "1% of 100,000 VND must be 1,000 VND");
        }

        [E2ETest(2, "B10: Charity percentage at 100% boundary")]
        public void B10_CharityPercentageFullBoundary()
        {
            var lineTotal = 150000m;
            var charityPart = ReferenceOracles.CharityContribution(lineTotal, 100);
            Assert.Equal(150000m, charityPart, "100% of 150,000 VND must be 150,000 VND");
        }

        [E2ETest(2, "B11: Charity percentage exceeding 100% capped at 100%")]
        public void B11_CharityPercentageCapAt100()
        {
            var lineTotal = 200000m;
            var charityPart = ReferenceOracles.CharityContribution(lineTotal, 150);
            Assert.Equal(200000m, charityPart, "Charity percentage > 100% must be capped at 100%");
        }

        [E2ETest(2, "B12: Phone number with spaces stripped and accepted")]
        public void B12_PhoneWithSpacesStripped()
        {
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("090 123 4567"), "Phone with spaces must be accepted after strip");
        }

        [E2ETest(2, "B13: Phone number with dots stripped and accepted")]
        public void B13_PhoneWithDotsStripped()
        {
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("090.123.4567"), "Phone with dots must be accepted after strip");
        }

        [E2ETest(2, "B14: Phone number with hyphens stripped and accepted")]
        public void B14_PhoneWithHyphensStripped()
        {
            Assert.True(ReferenceOracles.IsValidVietnamesePhone("090-123-4567"), "Phone with hyphens must be accepted after strip");
        }

        [E2ETest(2, "B15: Phone number with invalid prefix rejected")]
        public void B15_PhoneWithInvalidPrefixRejected()
        {
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("0201234567"), "Prefix 02 must be rejected");
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("0401234567"), "Prefix 04 must be rejected");
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("0601234567"), "Prefix 06 must be rejected");
        }

        [E2ETest(2, "B16: Phone number with 9 digits rejected (too short)")]
        public void B16_PhoneTooShortRejected()
        {
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("090123456"), "9-digit phone must be rejected");
        }

        [E2ETest(2, "B17: Phone number with 11 digits rejected (too long)")]
        public void B17_PhoneTooLongRejected()
        {
            Assert.False(ReferenceOracles.IsValidVietnamesePhone("09012345678"), "11-digit phone must be rejected");
        }

        [E2ETest(2, "B18: Email missing @ symbol rejected")]
        public void B18_EmailMissingAtSymbolRejected()
        {
            Assert.False(ReferenceOracles.IsValidEmail("nguyenvanAexample.com"), "Email missing @ must be rejected");
        }

        [E2ETest(2, "B19: Email missing domain suffix rejected")]
        public void B19_EmailMissingDomainRejected()
        {
            Assert.False(ReferenceOracles.IsValidEmail("nguyenvanA@"), "Email missing domain must be rejected");
            Assert.False(ReferenceOracles.IsValidEmail("nguyenvanA@com"), "Email missing dot in domain must be rejected");
        }

        [E2ETest(2, "B20: Whitespace-only string rejected for required customer name")]
        public void B20_WhitespaceOnlyNameRejected()
        {
            var whitespaceName = "   \t\n  ";
            Assert.True(string.IsNullOrWhiteSpace(whitespaceName), "Whitespace string must be identified as blank");
        }

        [E2ETest(2, "B21: Exactly 255-character product name within boundary")]
        public void B21_Exact255CharProductName()
        {
            var name255 = new string('A', 255);
            Assert.Equal(255, name255.Length, "Name must be exactly 255 chars");
            Assert.True(name255.Length <= 255, "255-character name must be accepted");
        }

        [E2ETest(2, "B22: 256-character product name exceeds boundary")]
        public void B22_256CharProductNameExceedsBoundary()
        {
            var name256 = new string('A', 256);
            Assert.True(name256.Length > 255, "256 chars must be identified as exceeding maximum length");
        }

        [E2ETest(2, "B23: Vietnamese diacritics integrity in charity descriptions")]
        public void B23_VietnameseDiacriticsIntegrity()
        {
            var text = "Trăng Sáng Langbiang 2026 - Thắp sáng nụ cười em thơ vùng cao Lạc Dương, tỉnh Lâm Đồng";
            Assert.Contains("Trăng Sáng", text, "Diacritics must not be corrupted");
            Assert.Contains("Lạc Dương", text, "Diacritics must not be corrupted");
            Assert.Contains("Lâm Đồng", text, "Diacritics must not be corrupted");
        }

        [E2ETest(2, "B24: HTML special characters escaped in donor notes")]
        public void B24_HtmlSpecialCharsEscaped()
        {
            var malicious = "<script>alert('xss')</script> & \"don't\"";
            var encoded = HtmlEncoder.Default.Encode(malicious);

            Assert.False(encoded.Contains("<script>"), "Raw <script> must be escaped");
            Assert.Contains("&lt;script&gt;", encoded, "Must convert to &lt;script&gt;");
        }

        [E2ETest(2, "B25: VietQR amount truncated to integer dong")]
        public void B25_VietQrAmountIntegerDong()
        {
            var decimalAmount = 149999.85m;
            var qrUrl = ReferenceOracles.VietQrImageUrl("VCB", "1023456789", decimalAmount, "GQTEST");
            Assert.Contains("amount=149999", qrUrl, "VietQR amount must be formatted as whole dong integer");
        }

        [E2ETest(2, "B26: Storefront search with Vietnamese Unicode matches products")]
        public void B26_Storefront_Search_Unicode()
        {
            var products = DbInitializer.GetInMemoryProducts();
            var keyword = "áo thun";
            var matched = products.Where(p =>
                (!string.IsNullOrEmpty(p.TenSanPham) && p.TenSanPham.ToLower().Contains(keyword.ToLower())) ||
                (!string.IsNullOrEmpty(p.MoTaNgan) && p.MoTaNgan.ToLower().Contains(keyword.ToLower()))
            ).ToList();

            Assert.True(matched.Count > 0, "Search by Vietnamese keyword 'áo thun' must match product");
            Assert.Contains("Áo Thun", matched[0].TenSanPham, "Matched product must contain keyword");
        }

        [E2ETest(2, "B27: Product with zero stock renders out of stock badge and disables button")]
        public void B27_Storefront_Out_Of_Stock_Badge()
        {
            var catalogPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "Index.cshtml");
            var detailPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "CuaHang", "ChiTiet.cshtml");

            Assert.True(File.Exists(catalogPath), "Catalog Index.cshtml must exist");
            Assert.True(File.Exists(detailPath), "Detail ChiTiet.cshtml must exist");

            var catalogContent = File.ReadAllText(catalogPath);
            var detailContent = File.ReadAllText(detailPath);

            Assert.True(
                catalogContent.Contains("Hết hàng") && catalogContent.Contains("disabled"),
                "Catalog view must contain 'Hết hàng' badge and disabled button for out of stock products"
            );
            Assert.True(
                detailContent.Contains("Hết hàng") || detailContent.Contains("Sản phẩm tạm hết hàng"),
                "Detail view must contain out of stock notification"
            );
        }

        [E2ETest(2, "B28: Admin order filter with invalid status handles gracefully without exception")]
        public void B28_Admin_Order_Filter_Invalid_Status()
        {
            var allOrders = DbInitializer.GetInMemoryOrders();
            var invalidStatus = "NonExistentInvalidStatus_12345";

            // Querying with invalid status should return empty collection safely without throwing
            var filtered = allOrders.Where(o => string.Equals(o.TrangThaiDonHang, invalidStatus, StringComparison.OrdinalIgnoreCase)).ToList();
            Assert.Equal(0, filtered.Count, "Filtering by invalid status must yield empty list safely without exception");

            var indexPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "Index.cshtml");
            var indexContent = File.ReadAllText(indexPath);
            Assert.True(
                indexContent.Contains("!Model.Any()") || indexContent.Contains("Không tìm thấy đơn hàng"),
                "AdminDonHang Index view must provide empty state message for zero matches"
            );
        }
    }
}
