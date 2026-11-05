using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WEB_TinhNguyen.Areas.CuaHang.Controllers;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    /// <summary>
    /// Tier 1: Functional and Contract Integrity Tests for Feature 18: Admin Order Management (AdminDonHang).
    /// Covers Controller architecture, Layout links, Dashboard tiles, Razor Views, Action signatures,
    /// and In-Memory fallback data persistence.
    /// </summary>
    public class Tier1_AdminOrderTests
    {
        // =========================================================================
        // FEATURE 18: Admin Order Management (AdminDonHang)
        // =========================================================================

        [E2ETest(1, 18, "F18_T01: AdminDonHangController exists in CuaHang area")]
        public void F18_T01_AdminDonHangControllerExists()
        {
            var controllerType = typeof(AdminDonHangController);
            Assert.NotNull(controllerType, "AdminDonHangController class must exist");
            Assert.True(typeof(Controller).IsAssignableFrom(controllerType), "AdminDonHangController must inherit from Microsoft.AspNetCore.Mvc.Controller");

            var areaAttr = controllerType.GetCustomAttribute<AreaAttribute>();
            Assert.NotNull(areaAttr, "AdminDonHangController must be decorated with [Area(\"CuaHang\")]");
            Assert.Equal("CuaHang", areaAttr!.RouteValue, "AreaAttribute RouteValue must be 'CuaHang'");
        }

        [E2ETest(1, 18, "F18_T02: _AdminLayout.cshtml contains link to AdminDonHang")]
        public void F18_T02_AdminLayoutContainsOrderLink()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml");
            Assert.True(File.Exists(layoutPath), "_AdminLayout.cshtml must exist");

            var content = File.ReadAllText(layoutPath);
            Assert.True(
                content.Contains("/CuaHang/AdminDonHang") || content.Contains("AdminDonHang"),
                "_AdminLayout.cshtml must contain navigation link to AdminDonHang"
            );
        }

        [E2ETest(1, 18, "F18_T03: _AdminLayout.cshtml order link text and icon")]
        public void F18_T03_AdminLayoutOrderLinkTextAndIcon()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml");
            var content = File.ReadAllText(layoutPath);

            Assert.True(
                content.Contains("Đơn hàng Gây Quỹ") || content.Contains("Đơn hàng"),
                "_AdminLayout.cshtml order navigation must display 'Đơn hàng Gây Quỹ' or 'Đơn hàng'"
            );
            Assert.True(
                content.Contains("fa-receipt") || content.Contains("fa-clipboard-list"),
                "_AdminLayout.cshtml order link must have icon fa-receipt or fa-clipboard-list"
            );
        }

        [E2ETest(1, 18, "F18_T04: Admin/Index.cshtml contains quick link for AdminDonHang")]
        public void F18_T04_AdminDashboardContainsQuickLink()
        {
            var dashboardPath = Path.Combine(E2ETestContext.ViewsRoot, "Admin", "Index.cshtml");
            Assert.True(File.Exists(dashboardPath), "Admin/Index.cshtml must exist");

            var content = File.ReadAllText(dashboardPath);
            Assert.True(
                content.Contains("AdminDonHang") || content.Contains("/CuaHang/AdminDonHang"),
                "Admin/Index.cshtml dashboard must contain quick link to AdminDonHang"
            );
            Assert.True(
                content.Contains("Đơn hàng Gây Quỹ") || content.Contains("Đơn hàng"),
                "Admin/Index.cshtml quick link must display 'Đơn hàng Gây Quỹ' or 'Đơn hàng'"
            );
        }

        [E2ETest(1, 18, "F18_T05: AdminDonHang/Index.cshtml exists and renders table headers")]
        public void F18_T05_AdminDonHangIndexTableHeaders()
        {
            var indexPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "Index.cshtml");
            Assert.True(File.Exists(indexPath), "Areas/CuaHang/Views/AdminDonHang/Index.cshtml must exist");

            var content = File.ReadAllText(indexPath);
            Assert.Contains("Mã đơn", content, "Index view must display 'Mã đơn' table column");
            Assert.True(content.Contains("Người nhận") || content.Contains("Người ủng hộ") || content.Contains("Người đặt"), "Index view must display recipient column header");
            Assert.True(content.Contains("Tổng tiền"), "Index view must display 'Tổng tiền' column header");
            Assert.True(content.Contains("Trạng thái"), "Index view must display 'Trạng thái' column header");
            Assert.True(content.Contains("Thao tác"), "Index view must display 'Thao tác' column header");
        }

        [E2ETest(1, 18, "F18_T06: AdminDonHang/Index.cshtml contains 4 KPI cards")]
        public void F18_T06_AdminDonHangIndexKpiCards()
        {
            var indexPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "Index.cshtml");
            var content = File.ReadAllText(indexPath);

            Assert.True(
                content.Contains("Tổng đơn") || content.Contains("tongDonHang") || content.Contains("TongDonHang"),
                "Index view must display Total Orders KPI card"
            );
            Assert.True(
                content.Contains("chờ xác nhận") || content.Contains("Chờ xác nhận") || content.Contains("choXacNhan") || content.Contains("ChoXacNhan"),
                "Index view must display Pending Orders KPI card"
            );
            Assert.True(
                content.Contains("Doanh thu") || content.Contains("tongDoanhThu") || content.Contains("TongDoanhThu"),
                "Index view must display Revenue KPI card"
            );
            Assert.True(
                content.Contains("Quỹ") || content.Contains("quỹ") || content.Contains("tongTienQuy") || content.Contains("TongTienQuy"),
                "Index view must display Charity Funds KPI card"
            );
        }

        [E2ETest(1, 18, "F18_T07: AdminDonHang/ChiTiet.cshtml exists and displays donor, delivery, and items")]
        public void F18_T07_AdminDonHangChiTietDisplaysSections()
        {
            var detailPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "ChiTiet.cshtml");
            Assert.True(File.Exists(detailPath), "Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml must exist");

            var content = File.ReadAllText(detailPath);
            Assert.True(content.Contains("HoTenNguoiMua") && content.Contains("SoDienThoai"), "ChiTiet view must display donor contact");
            Assert.True(content.Contains("DiaChiNhanHang"), "ChiTiet view must display delivery address");
            Assert.True(content.Contains("ChiTietDonHangs"), "ChiTiet view must iterate order items breakdown");
            Assert.True(
                content.Contains("TienDongGop") || content.Contains("TienUngHoThem") || content.Contains("Quỹ") || content.Contains("quỹ"),
                "ChiTiet view must display charity donation breakdown"
            );
        }

        [E2ETest(1, 18, "F18_T08: AdminDonHangController.Index action method signature")]
        public void F18_T08_AdminDonHangControllerIndexAction()
        {
            var controllerType = typeof(AdminDonHangController);
            var indexMethod = controllerType.GetMethod("Index", new[] { typeof(string), typeof(string), typeof(string), typeof(string) });
            Assert.NotNull(indexMethod, "Index action with filter parameters (q, trangThai, thanhToan, sort) must exist");
            Assert.True(typeof(Task<IActionResult>).IsAssignableFrom(indexMethod!.ReturnType), "Index action must return Task<IActionResult>");
        }

        [E2ETest(1, 18, "F18_T09: AdminDonHangController.ChiTiet action method signature")]
        public void F18_T09_AdminDonHangControllerChiTietAction()
        {
            var controllerType = typeof(AdminDonHangController);
            var chiTietMethod = controllerType.GetMethod("ChiTiet", new[] { typeof(int) });
            Assert.NotNull(chiTietMethod, "ChiTiet action with parameter int id must exist");
            Assert.True(typeof(Task<IActionResult>).IsAssignableFrom(chiTietMethod!.ReturnType), "ChiTiet action must return Task<IActionResult>");
        }

        [E2ETest(1, 18, "F18_T10: AdminDonHangController.CapNhatTrangThai action method signature with [HttpPost]")]
        public void F18_T10_AdminDonHangControllerCapNhatTrangThaiAction()
        {
            var controllerType = typeof(AdminDonHangController);
            var method = controllerType.GetMethod("CapNhatTrangThai", new[] { typeof(int), typeof(string), typeof(string) });
            Assert.NotNull(method, "CapNhatTrangThai action with (int id, string trangThaiDonHang, string? trangThaiThanhToan) must exist");

            var postAttr = method!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(postAttr, "CapNhatTrangThai must be decorated with [HttpPost]");
            Assert.True(typeof(Task<IActionResult>).IsAssignableFrom(method.ReturnType), "CapNhatTrangThai must return Task<IActionResult>");
        }

        [E2ETest(1, 18, "F18_T11: AdminDonHangController.XoaDonHang action method signature with [HttpPost]")]
        public void F18_T11_AdminDonHangControllerXoaDonHangAction()
        {
            var controllerType = typeof(AdminDonHangController);
            var method = controllerType.GetMethod("XoaDonHang", new[] { typeof(int) });
            Assert.NotNull(method, "XoaDonHang action with parameter (int id) must exist");

            var postAttr = method!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(postAttr, "XoaDonHang must be decorated with [HttpPost]");
            Assert.True(typeof(Task<IActionResult>).IsAssignableFrom(method.ReturnType), "XoaDonHang must return Task<IActionResult>");
        }

        [E2ETest(1, 18, "F18_T12: DbInitializer.GetInMemoryOrders returns list of orders safely")]
        public void F18_T12_DbInitializerGetInMemoryOrdersReturnsList()
        {
            DbInitializer.ResetInMemoryStore();
            var orders = DbInitializer.GetInMemoryOrders();
            Assert.NotNull(orders, "DbInitializer.GetInMemoryOrders() must not return null");
            Assert.True(orders.Count > 0, "DbInitializer must provide sample seed orders");
            Assert.NotNull(orders[0].MaDonHang, "Orders must have non-null MaDonHang");
            Assert.True(orders.Any(o => o.ChiTietDonHangs != null && o.ChiTietDonHangs.Count > 0), "Orders must have associated item details");
        }

        [E2ETest(1, 18, "F18_T13: DbInitializer.UpdateInMemoryOrderStatus updates status and timestamp")]
        public void F18_T13_DbInitializerUpdateInMemoryOrderStatus()
        {
            var orders = DbInitializer.GetInMemoryOrders();
            Assert.True(orders.Count > 0, "Orders must exist in store");
            var targetId = orders[0].IDDonHang;

            var updated = DbInitializer.UpdateInMemoryOrderStatus(targetId, "DaXacNhan", "DaThanhToan");
            Assert.True(updated, "UpdateInMemoryOrderStatus must return true on existing order ID");

            var retrieved = DbInitializer.GetInMemoryOrderById(targetId);
            Assert.NotNull(retrieved, "Retrieved order must not be null");
            Assert.Equal("DaXacNhan", retrieved.TrangThaiDonHang, "Order status must be updated to 'DaXacNhan'");
            Assert.Equal("DaThanhToan", retrieved.TrangThaiThanhToan, "Payment status must be updated to 'DaThanhToan'");
            Assert.NotNull(retrieved.NgayCapNhat, "NgayCapNhat timestamp must be set upon update");
        }

        [E2ETest(1, 18, "F18_T14: DbInitializer.DeleteInMemoryOrder removes order safely")]
        public void F18_T14_DbInitializerDeleteInMemoryOrder()
        {
            var tempOrder = new DonHangGayQuy
            {
                HoTenNguoiMua = "Test Temporary Order For Deletion",
                SoDienThoai = "0987654321",
                DiaChiNhanHang = "Khuôn viên HUTECH E-Campus",
                TongThanhToan = 100000m,
                PhuongThucThanhToan = "COD",
                TrangThaiDonHang = "MoiDat",
                TrangThaiThanhToan = "ChuaThanhToan"
            };

            var added = DbInitializer.AddInMemoryOrder(tempOrder);
            Assert.NotNull(added, "Added order must not be null");
            Assert.True(added.IDDonHang > 0, "Added order must have generated positive ID");

            var deleted = DbInitializer.DeleteInMemoryOrder(added.IDDonHang);
            Assert.True(deleted, "DeleteInMemoryOrder must return true for existing order ID");

            var notFound = DbInitializer.GetInMemoryOrderById(added.IDDonHang);
            Assert.Null(notFound, "Deleted order must not be retrieved by GetInMemoryOrderById");
        }

        [E2ETest(1, 18, "F18_T15: AdminDonHang/ChiTiet.cshtml displays VietQR transfer instructions and bank account")]
        public void F18_T15_AdminDonHangChiTietDisplaysVietQrAndAccount()
        {
            var detailPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminDonHang", "ChiTiet.cshtml");
            var content = File.ReadAllText(detailPath);

            Assert.True(content.Contains("0924044283"), "ChiTiet.cshtml must display charity bank account 0924044283");
            Assert.True(
                content.Contains("vietqr") || content.Contains("VietQR"),
                "ChiTiet.cshtml must embed VietQR payment integration"
            );
            Assert.True(
                content.Contains("NGUYEN THAI VINH") || content.Contains("MBBank") || content.Contains("Quân Đội"),
                "ChiTiet.cshtml must display bank transfer recipient information"
            );
        }
    }
}
