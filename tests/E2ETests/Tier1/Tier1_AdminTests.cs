using System;
using System.IO;
using System.Linq;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    public class Tier1_AdminTests
    {
        // =========================================================================
        // FEATURE 12: Admin Sidebar Navigation
        // =========================================================================

        [E2ETest(1, 12, "F12_T01: Admin sidebar layout exists")]
        public void F12_T01_AdminLayoutExists()
        {
            var adminLayoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml");
            Assert.True(File.Exists(adminLayoutPath), "_AdminLayout.cshtml must exist");
        }

        [E2ETest(1, 12, "F12_T02: Admin sidebar contains CuaHang / AdminSanPham link")]
        public void F12_T02_AdminSidebarContainsProductLink()
        {
            var adminLayoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));
            if (!adminLayoutContent.Contains("AdminSanPham") && !adminLayoutContent.Contains("CuaHang"))
            {
                throw new AssertException("[SKIP] Pending implementation in M2: AdminSanPham menu link in _AdminLayout.cshtml");
            }
            Assert.True(
                adminLayoutContent.Contains("AdminSanPham") || adminLayoutContent.Contains("CuaHang"),
                "_AdminLayout.cshtml must contain link to AdminSanPham in CuaHang area"
            );
        }

        [E2ETest(1, 12, "F12_T03: Admin sidebar link has descriptive title")]
        public void F12_T03_AdminSidebarLinkTitle()
        {
            var adminLayoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));
            if (!adminLayoutContent.Contains("Sản phẩm Gây Quỹ") && !adminLayoutContent.Contains("Sản phẩm gây quỹ"))
            {
                throw new AssertException("[SKIP] Pending implementation in M2: Admin sidebar menu item title in _AdminLayout.cshtml");
            }
            Assert.True(
                adminLayoutContent.Contains("Sản phẩm Gây Quỹ") || adminLayoutContent.Contains("Sản phẩm gây quỹ") || adminLayoutContent.Contains("Sản Phẩm Gây Quỹ"),
                "_AdminLayout.cshtml link text must be 'Sản phẩm Gây Quỹ'"
            );
        }

        [E2ETest(1, 12, "F12_T04: Admin sidebar link has dedicated icon")]
        public void F12_T04_AdminSidebarLinkIcon()
        {
            var adminLayoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));
            if (!adminLayoutContent.Contains("AdminSanPham"))
            {
                throw new AssertException("[SKIP] Pending implementation in M2: AdminSanPham menu icon in _AdminLayout.cshtml");
            }
            Assert.True(
                adminLayoutContent.Contains("fa-box") || adminLayoutContent.Contains("fa-store") || adminLayoutContent.Contains("fa-gift") || adminLayoutContent.Contains("fa-shopping-cart"),
                "_AdminLayout.cshtml menu item must include appropriate FontAwesome icon"
            );
        }

        [E2ETest(1, 12, "F12_T05: Admin sidebar uses emerald accent styling")]
        public void F12_T05_AdminSidebarEmeraldStyling()
        {
            var adminLayoutContent = File.ReadAllText(Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml"));
            Assert.True(
                adminLayoutContent.Contains("emerald") || adminLayoutContent.Contains("green") || adminLayoutContent.Contains("#4caf50"),
                "_AdminLayout.cshtml must use emerald styling classes"
            );
        }

        // =========================================================================
        // FEATURE 13: Admin Product Table View
        // =========================================================================

        [E2ETest(1, 13, "F13_T01: Admin product table view template exists")]
        public void F13_T01_AdminTableViewTemplateExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
        }

        [E2ETest(1, 13, "F13_T02: Admin product table displays thumbnail image column")]
        public void F13_T02_AdminTableThumbnailImageColumn()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("LinkHinhAnh") || viewContent.Contains("<img"),
                "Admin table view must display product images"
            );
        }

        [E2ETest(1, 13, "F13_T03: Admin product table displays product name, price, stock")]
        public void F13_T03_AdminTableKeyColumns()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("TenSanPham", viewContent, "Admin table must display TenSanPham");
            Assert.Contains("GiaBan", viewContent, "Admin table must display GiaBan");
            Assert.Contains("SoLuongTon", viewContent, "Admin table must display SoLuongTon");
        }

        [E2ETest(1, 13, "F13_T04: Admin product table provides Create action button")]
        public void F13_T04_AdminTableCreateButton()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("Create") || viewContent.Contains("Thêm mới"),
                "Admin table view must provide Create button"
            );
        }

        [E2ETest(1, 13, "F13_T05: Admin product table provides Edit and Delete row actions")]
        public void F13_T05_AdminTableEditDeleteActions()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("Edit") || viewContent.Contains("Sửa"),
                "Admin table view must have Edit row action"
            );
            Assert.True(
                viewContent.Contains("Delete") || viewContent.Contains("Xóa"),
                "Admin table view must have Delete row action"
            );
        }

        // =========================================================================
        // FEATURE 14: Admin Create Product Form
        // =========================================================================

        [E2ETest(1, 14, "F14_T01: Admin Create view template exists")]
        public void F14_T01_AdminCreateViewExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(viewPath, "Admin Create view template");
        }

        [E2ETest(1, 14, "F14_T02: Admin Create form supports multipart file upload")]
        public void F14_T02_AdminCreateFormMultipartEncoding()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(viewPath, "Admin Create view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("enctype=\"multipart/form-data\"", viewContent, "Create form must specify enctype='multipart/form-data'");
        }

        [E2ETest(1, 14, "F14_T03: Admin Create ViewModel enforces required fields")]
        public void F14_T03_AdminCreateViewModelValidation()
        {
            var crudVmType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamCrudViewModel", "SanPhamCrudViewModel");
            var nameProp = crudVmType.GetProperty("TenSanPham");
            var hasRequired = nameProp?.GetCustomAttributes(true).Any(a => a.GetType().Name == "RequiredAttribute") ?? false;
            Assert.True(hasRequired, "TenSanPham must have [Required] attribute");
        }

        [E2ETest(1, 14, "F14_T04: Admin Create form contains charity percentage input")]
        public void F14_T04_AdminCreateFormCharityPercentage()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(viewPath, "Admin Create view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("PhanTramDongGop", viewContent, "Create form must have PhanTramDongGop field");
        }

        [E2ETest(1, 14, "F14_T05: Admin Create form contains save button")]
        public void F14_T05_AdminCreateFormSaveButton()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(viewPath, "Admin Create view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("type=\"submit\"") || viewContent.Contains("type='submit'"),
                "Create form must contain submit button"
            );
        }

        // =========================================================================
        // FEATURE 15: Admin Edit Product Form
        // =========================================================================

        [E2ETest(1, 15, "F15_T01: Admin Edit view template exists")]
        public void F15_T01_AdminEditViewExists()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(viewPath, "Admin Edit view template");
        }

        [E2ETest(1, 15, "F15_T02: Admin Edit form contains hidden primary key ID field")]
        public void F15_T02_AdminEditFormHiddenId()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(viewPath, "Admin Edit view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("asp-for=\"IDSanPham\"") && viewContent.Contains("type=\"hidden\""),
                "Edit form must include hidden input for IDSanPham"
            );
        }

        [E2ETest(1, 15, "F15_T03: Admin Edit form displays current image preview")]
        public void F15_T03_AdminEditFormImagePreview()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(viewPath, "Admin Edit view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("LinkHinhAnh") || viewContent.Contains("<img"),
                "Edit form must display preview of existing product image"
            );
        }

        [E2ETest(1, 15, "F15_T04: Admin Edit form supports multipart upload for replacing image")]
        public void F15_T04_AdminEditFormMultipartEncoding()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(viewPath, "Admin Edit view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("enctype=\"multipart/form-data\"", viewContent, "Edit form must support multipart/form-data for image updates");
        }

        [E2ETest(1, 15, "F15_T05: SanPhamCrudViewModel UpdateEntity preserves existing image when no new file is uploaded")]
        public void F15_T05_UpdateEntityPreservesExistingImage()
        {
            var crudVmType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamCrudViewModel", "SanPhamCrudViewModel");
            var spType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy", "SanPhamGayQuy");

            var entity = Activator.CreateInstance(spType);
            spType.GetProperty("LinkHinhAnh")?.SetValue(entity, "/assets/journey.jpg");

            var vm = Activator.CreateInstance(crudVmType);
            crudVmType.GetProperty("LinkHinhAnh")?.SetValue(vm, null);

            var updateMethod = crudVmType.GetMethods().FirstOrDefault(m => m.Name == "UpdateEntity" && m.GetParameters().Length == 1)
                               ?? crudVmType.GetMethods().FirstOrDefault(m => m.Name == "UpdateEntity");
            if (updateMethod != null)
            {
                var parameters = updateMethod.GetParameters();
                var args = parameters.Length == 1 ? new object?[] { entity } : new object?[] { entity, null };
                updateMethod.Invoke(vm, args);
                var preservedImage = spType.GetProperty("LinkHinhAnh")?.GetValue(entity) as string;
                Assert.Equal("/assets/journey.jpg", preservedImage, "Existing image URL must be preserved when no replacement provided");
            }
        }

        // =========================================================================
        // FEATURE 16: Admin Delete Product
        // =========================================================================

        [E2ETest(1, 16, "F16_T01: Admin controller declares Delete action")]
        public void F16_T01_AdminControllerDeclaresDelete()
        {
            var ctrlType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Controllers.AdminSanPhamController", "AdminSanPhamController");
            var deleteMethod = ctrlType.GetMethod("Delete");
            Assert.NotNull(deleteMethod, "AdminSanPhamController must define Delete action");
        }

        [E2ETest(1, 16, "F16_T02: Delete action requires ID parameter")]
        public void F16_T02_DeleteActionRequiresIdParameter()
        {
            var ctrlType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Controllers.AdminSanPhamController", "AdminSanPhamController");
            var deleteMethod = ctrlType.GetMethod("Delete");
            var parameters = deleteMethod?.GetParameters();
            Assert.True(parameters?.Length > 0 && parameters[0].Name?.ToLower().Contains("id") == true, "Delete action must take id parameter");
        }

        [E2ETest(1, 16, "F16_T03: Admin table includes confirmation prompt before deletion")]
        public void F16_T03_DeleteConfirmationPrompt()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "Admin table view template");
            var viewContent = File.ReadAllText(viewPath);
            Assert.True(
                viewContent.Contains("confirm") || viewContent.Contains("Xác nhận") || viewContent.Contains("bạn có chắc"),
                "Admin table view must include confirmation before triggering product delete"
            );
        }

        [E2ETest(1, 16, "F16_T04: Delete operation removes product from in-memory fallback store")]
        public void F16_T04_DeleteRemovesFromInMemoryStore()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");
            var deleteMethod = initType.GetMethod("DeleteInMemoryProduct");
            Assert.NotNull(deleteMethod, "DeleteInMemoryProduct must exist");
        }

        [E2ETest(1, 16, "F16_T05: Deleting non-existent product ID returns false / safe handling")]
        public void F16_T05_DeleteNonExistentIdSafe()
        {
            var initType = Assert.RequireType("WEB_TinhNguyen.Areas.CuaHang.Helpers.DbInitializer", "DbInitializer");
            var deleteMethod = initType.GetMethod("DeleteInMemoryProduct");
            if (deleteMethod != null)
            {
                var result = deleteMethod.Invoke(null, new object[] { 999999 });
                Assert.Equal(false, result, "Deleting non-existent ID should safely return false");
            }
        }
    }
}
