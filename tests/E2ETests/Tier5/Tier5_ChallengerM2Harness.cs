using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WEB_TinhNguyen.Areas.CuaHang.Controllers;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;
using WEB_TinhNguyen.Areas.CuaHang.Models;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    /// <summary>
    /// Tier 5: Adversarial Challenge & Stress Harness for Milestone 2 (Admin Product Management)
    /// Empirically challenges:
    /// 1. Validation rules and boundary limits on Admin forms (empty name, negative price, boundary stock, charity %)
    /// 2. Malicious script injection, XSS vectors, and SQL injection strings
    /// 3. File upload restrictions (executable/script extension rejection, double extensions, uppercase extensions, size limit)
    /// 4. Controller action lifecycle and security boundaries (Anti-forgery, ID mismatch tampering, invalid IDs)
    /// 5. In-Memory fallback resilience and concurrency under stress
    /// 6. Razor views layout and CSRF token integrity
    /// </summary>
    public class Tier5_ChallengerM2Harness
    {
        #region Test Helpers & Mocks

        public class MockWebHostEnvironment : IWebHostEnvironment
        {
            public string WebRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "web_tinhnguyen_m2_tests_" + Guid.NewGuid().ToString("N"));
            public IFileProvider WebRootFileProvider { get; set; } = null!;
            public string EnvironmentName { get; set; } = "Development";
            public string ApplicationName { get; set; } = "WEB_TinhNguyen";
            public string ContentRootPath { get; set; } = Path.GetTempPath();
            public IFileProvider ContentRootFileProvider { get; set; } = null!;
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

        private static bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        }

        private static T AssertIsType<T>(object? obj, string message) where T : class
        {
            Assert.NotNull(obj, message);
            Assert.True(obj is T, $"Expected type {typeof(T).Name}, got {obj!.GetType().Name}. {message}");
            return (T)obj;
        }

        private static AdminSanPhamController CreateTestController(out ApplicationDbContext context, out MockWebHostEnvironment env)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=127.0.0.1,1;Database=dummy;Connect Timeout=1;")
                .Options;
            context = new ApplicationDbContext(options);
            env = new MockWebHostEnvironment();

            var controller = new AdminSanPhamController(context, env);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.TempData = new TempDataDictionary(httpContext, new MockTempDataProvider());
            return controller;
        }

        private static IFormFile CreateMockFormFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "FileHinhAnh", fileName);
        }

        #endregion

        // =========================================================================
        // SECTION 1: ADMIN FORM VALIDATIONS & BOUNDARY CONDITIONS (FEATURE 14)
        // =========================================================================

        [E2ETest(5, 14, "M2_C01: Empty or whitespace-only product name fails validation")]
        public void M2_C01_TenSanPham_EmptyOrWhitespaceOnly_FailsValidation()
        {
            var vmEmpty = new SanPhamCrudViewModel { TenSanPham = "", GiaBan = 50000m, SoLuongTon = 10 };
            Assert.False(ValidateModel(vmEmpty, out var r1), "Empty TenSanPham must fail validation");
            Assert.True(r1.Any(r => r.MemberNames.Contains("TenSanPham")), "Error must target TenSanPham");

            var vmWhitespace = new SanPhamCrudViewModel { TenSanPham = "   \t\n   ", GiaBan = 50000m, SoLuongTon = 10 };
            var entity = vmWhitespace.ToEntity();
            Assert.Equal("", entity.TenSanPham, "ToEntity must trim whitespace-only name to empty");
            Assert.False(ValidateModel(entity, out var r2), "Trimmed empty name entity must fail validation");
        }

        [E2ETest(5, 14, "M2_C02: Product name exceeding 255 chars boundary rejected by StringLength(255)")]
        public void M2_C02_TenSanPham_Exceeding255Chars_FailsValidation()
        {
            var name256 = new string('A', 256);
            var vm = new SanPhamCrudViewModel { TenSanPham = name256, GiaBan = 50000m, SoLuongTon = 10 };
            Assert.False(ValidateModel(vm, out var results), "Name with 256 characters must fail [StringLength(255)]");
            Assert.True(results.Any(r => r.MemberNames.Contains("TenSanPham")), "Error must target TenSanPham");
        }

        [E2ETest(5, 14, "M2_C03: Product name exactly 255 chars accepted at boundary")]
        public void M2_C03_TenSanPham_Boundary255Chars_PassesValidation()
        {
            var name255 = new string('A', 255);
            var vm = new SanPhamCrudViewModel { TenSanPham = name255, GiaBan = 50000m, SoLuongTon = 10 };
            Assert.True(ValidateModel(vm, out _), "Name with exactly 255 characters must pass validation");
        }

        [E2ETest(5, 14, "M2_C04: Negative price rejected by Range(0, 1000000000)")]
        public void M2_C04_GiaBan_Negative_FailsValidation()
        {
            var vm = new SanPhamCrudViewModel { TenSanPham = "Sản phẩm", GiaBan = -1m, SoLuongTon = 10 };
            Assert.False(ValidateModel(vm, out var results), "Negative price (-1 VNĐ) must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaBan")), "Error must target GiaBan");
        }

        [E2ETest(5, 14, "M2_C04_Adv: Adversarial finding - fractional negative price (-0.01) bypasses Range(int, int)")]
        public void M2_C04_Adv_FractionalNegativePrice_BypassesIntRange()
        {
            var vm = new SanPhamCrudViewModel { TenSanPham = "Sản phẩm", GiaBan = -0.01m, SoLuongTon = 10 };
            // Empirical proof: Range(int, int) converts decimal -0.01m to int 0, passing validation unexpectedly!
            var isValid = ValidateModel(vm, out _);
            Assert.True(isValid, "Adversarial finding: decimal -0.01m is truncated to int 0, bypassing [Range(0, 1000000000)]");
        }

        [E2ETest(5, 14, "M2_C05: Zero price accepted for free promotional / tri ân gifts")]
        public void M2_C05_GiaBan_Zero_PassesValidation()
        {
            var vm = new SanPhamCrudViewModel { TenSanPham = "Quà Tặng 0đ Tri Ân Tình Nguyện", GiaBan = 0m, SoLuongTon = 100 };
            Assert.True(ValidateModel(vm, out _), "0 VNĐ price must be accepted for charity giveaway");
        }

        [E2ETest(5, 14, "M2_C06: Price ceiling boundary at 1,000,000,000 VND")]
        public void M2_C06_GiaBan_UpperCeiling1Billion_Boundary()
        {
            var vmAtCeiling = new SanPhamCrudViewModel { TenSanPham = "Vật phẩm đấu giá từ thiện", GiaBan = 1000000000m, SoLuongTon = 1 };
            Assert.True(ValidateModel(vmAtCeiling, out _), "1,000,000,000 VNĐ must pass validation");

            var vmOverCeiling = new SanPhamCrudViewModel { TenSanPham = "Vật phẩm vượt trần", GiaBan = 1000000001m, SoLuongTon = 1 };
            Assert.False(ValidateModel(vmOverCeiling, out var results), "1,000,000,001 VNĐ must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaBan")), "Error must target GiaBan");
        }

        [E2ETest(5, 14, "M2_C07: Negative original price (GiaGoc) rejected")]
        public void M2_C07_GiaGoc_Negative_FailsValidation()
        {
            var vm = new SanPhamCrudViewModel { TenSanPham = "Sản phẩm", GiaBan = 10000m, GiaGoc = -1000m, SoLuongTon = 5 };
            Assert.False(ValidateModel(vm, out var results), "Negative GiaGoc must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("GiaGoc")), "Error must target GiaGoc");
        }

        [E2ETest(5, 14, "M2_C08: Negative stock quantity (SoLuongTon) rejected")]
        public void M2_C08_SoLuongTon_Negative_FailsValidation()
        {
            var vm = new SanPhamCrudViewModel { TenSanPham = "Sản phẩm", GiaBan = 10000m, SoLuongTon = -1 };
            Assert.False(ValidateModel(vm, out var results), "-1 stock must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("SoLuongTon")), "Error must target SoLuongTon");
        }

        [E2ETest(5, 14, "M2_C09: Stock quantity boundary at 0 and 1,000,000")]
        public void M2_C09_SoLuongTon_Boundaries()
        {
            var vmZero = new SanPhamCrudViewModel { TenSanPham = "Sản phẩm hết hàng", GiaBan = 10000m, SoLuongTon = 0 };
            Assert.True(ValidateModel(vmZero, out _), "0 stock (sold out state) must be valid");

            var vmMax = new SanPhamCrudViewModel { TenSanPham = "Kho lớn", GiaBan = 10000m, SoLuongTon = 1000000 };
            Assert.True(ValidateModel(vmMax, out _), "1,000,000 stock must be valid");

            var vmOver = new SanPhamCrudViewModel { TenSanPham = "Kho tràn", GiaBan = 10000m, SoLuongTon = 1000001 };
            Assert.False(ValidateModel(vmOver, out var results), ">1M stock must fail validation");
            Assert.True(results.Any(r => r.MemberNames.Contains("SoLuongTon")), "Error must target SoLuongTon");
        }

        [E2ETest(5, 14, "M2_C10: Charity percentage boundaries [1, 100]")]
        public void M2_C10_PhanTramDongGop_Boundaries()
        {
            var vm0 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, PhanTramDongGop = 0 };
            Assert.False(ValidateModel(vm0, out _), "0% donation must fail validation (minimum 1%)");

            var vm1 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, PhanTramDongGop = 1 };
            Assert.True(ValidateModel(vm1, out _), "1% donation must pass validation");

            var vm100 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, PhanTramDongGop = 100 };
            Assert.True(ValidateModel(vm100, out _), "100% donation must pass validation");

            var vm101 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, PhanTramDongGop = 101 };
            Assert.False(ValidateModel(vm101, out _), "101% donation must fail validation");

            var vmNeg = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, PhanTramDongGop = -10 };
            Assert.False(ValidateModel(vmNeg, out _), "-10% donation must fail validation");
        }

        [E2ETest(5, 14, "M2_C11: Short description length boundary at 500 chars")]
        public void M2_C11_MoTaNgan_Exceeding500Chars_FailsValidation()
        {
            var desc500 = new string('M', 500);
            var vm500 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, MoTaNgan = desc500 };
            Assert.True(ValidateModel(vm500, out _), "500 chars MoTaNgan must pass");

            var desc501 = new string('M', 501);
            var vm501 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, MoTaNgan = desc501 };
            Assert.False(ValidateModel(vm501, out var results), "501 chars MoTaNgan must fail [StringLength(500)]");
            Assert.True(results.Any(r => r.MemberNames.Contains("MoTaNgan")), "Error must target MoTaNgan");
        }

        [E2ETest(5, 14, "M2_C12: Charity message length boundary at 500 chars")]
        public void M2_C12_ThongDiepGayQuy_Exceeding500Chars_FailsValidation()
        {
            var msg500 = new string('T', 500);
            var vm500 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, ThongDiepGayQuy = msg500 };
            Assert.True(ValidateModel(vm500, out _), "500 chars ThongDiepGayQuy must pass");

            var msg501 = new string('T', 501);
            var vm501 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, ThongDiepGayQuy = msg501 };
            Assert.False(ValidateModel(vm501, out var results), "501 chars ThongDiepGayQuy must fail [StringLength(500)]");
            Assert.True(results.Any(r => r.MemberNames.Contains("ThongDiepGayQuy")), "Error must target ThongDiepGayQuy");
        }

        [E2ETest(5, 14, "M2_C13: Category length boundary at 100 chars")]
        public void M2_C13_DanhMuc_Exceeding100Chars_FailsValidation()
        {
            var cat100 = new string('D', 100);
            var vm100 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, DanhMuc = cat100 };
            Assert.True(ValidateModel(vm100, out _), "100 chars DanhMuc must pass");

            var cat101 = new string('D', 101);
            var vm101 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, DanhMuc = cat101 };
            Assert.False(ValidateModel(vm101, out var results), "101 chars DanhMuc must fail [StringLength(100)]");
            Assert.True(results.Any(r => r.MemberNames.Contains("DanhMuc")), "Error must target DanhMuc");
        }

        [E2ETest(5, 14, "M2_C14: Image link URL boundary at 1000 chars")]
        public void M2_C14_LinkHinhAnh_Exceeding1000Chars_FailsValidation()
        {
            var link1000 = "https://example.com/" + new string('x', 980);
            var vm1000 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, LinkHinhAnh = link1000 };
            Assert.True(ValidateModel(vm1000, out _), "1000 chars LinkHinhAnh must pass");

            var link1001 = "https://example.com/" + new string('x', 981);
            var vm1001 = new SanPhamCrudViewModel { TenSanPham = "A", GiaBan = 10000m, SoLuongTon = 10, LinkHinhAnh = link1001 };
            Assert.False(ValidateModel(vm1001, out var results), "1001 chars LinkHinhAnh must fail [StringLength(1000)]");
            Assert.True(results.Any(r => r.MemberNames.Contains("LinkHinhAnh")), "Error must target LinkHinhAnh");
        }

        // =========================================================================
        // SECTION 2: MALICIOUS SCRIPTS, XSS & SQL INJECTION RESILIENCE
        // =========================================================================

        [E2ETest(5, 14, "M2_C15: Malicious script tags in TenSanPham preserved safely without exception")]
        public void M2_C15_XSS_Payload_ScriptTagInTenSanPham_SafelyHandled()
        {
            var xssPayload = "<script>alert('xss_attack')</script>";
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = xssPayload,
                GiaBan = 50000m,
                SoLuongTon = 10,
                MoTaNgan = "Test XSS"
            };

            var entity = vm.ToEntity();
            Assert.Equal(xssPayload, entity.TenSanPham, "Payload must be preserved safely in data model");

            var encoded = WebUtility.HtmlEncode(entity.TenSanPham ?? "");
            Assert.Contains("&lt;script&gt;", encoded, "Angle brackets must be HTML-encoded");
            Assert.False(encoded.Contains("<script>"), "Raw unescaped script tag must not remain");
        }

        [E2ETest(5, 14, "M2_C16: Script injection with img/iframe tags in MoTaChiTiet preserved safely without crash")]
        public void M2_C16_XSS_Payload_HtmlEntitiesInMoTaChiTiet_PreservedWithoutCrash()
        {
            var dangerousHtml = "<img src='x' onerror='alert(\"hacked\")' /><iframe src='javascript:alert(1)'></iframe>";
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Sản phẩm an toàn",
                GiaBan = 50000m,
                SoLuongTon = 10,
                MoTaChiTiet = dangerousHtml
            };

            var entity = vm.ToEntity();
            Assert.Equal(dangerousHtml, entity.MoTaChiTiet, "MoTaChiTiet preserves text in entity");

            var encoded = WebUtility.HtmlEncode(entity.MoTaChiTiet ?? "");
            Assert.Contains("&lt;img", encoded, "Img tag must be encoded");
            Assert.Contains("&lt;iframe", encoded, "Iframe tag must be encoded");
        }

        [E2ETest(5, 14, "M2_C17: SQL Injection strings in TenSanPham and DanhMuc do not crash model or store")]
        public void M2_C17_SqlInjection_QuotesAndDropTable_HandledSafelyInEntity()
        {
            var sqlPayload = "Áo Thun'; DROP TABLE SanPhamGayQuy; SELECT * FROM Users WHERE ''='";
            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = sqlPayload,
                GiaBan = 129000m,
                SoLuongTon = 50,
                DanhMuc = "Quà tặng'; DROP TABLE DonHang; --"
            };

            var entity = vm.ToEntity();
            Assert.Equal(sqlPayload, entity.TenSanPham, "SQL payload is treated as literal data");

            DbInitializer.ResetInMemoryStore();
            var added = DbInitializer.AddInMemoryProduct(entity);
            Assert.NotNull(added, "In-memory store must accept entity with SQL characters");

            var retrieved = DbInitializer.GetInMemoryProductById(added.IDSanPham);
            Assert.NotNull(retrieved, "Retrieved product must exist");
            Assert.Equal(sqlPayload, retrieved!.TenSanPham, "SQL string must remain unexecuted literal text");
        }

        [E2ETest(5, 13, "M2_C18: Razor view index confirmation dialog safely encodes product names with quotes")]
        public void M2_C18_RazorEscape_AdminIndexConfirmationDialog_UsesHtmlEncode()
        {
            var viewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(viewPath, "AdminSanPham/Index.cshtml");

            var content = File.ReadAllText(viewPath);
            Assert.Contains("Html.Encode", content, "Index.cshtml must use Html.Encode in delete confirm dialog to prevent JS quote breakout");

            var singleQuoteTest = "Áo 'Trăng Sáng' Langbiang";
            var encodedQuote = WebUtility.HtmlEncode(singleQuoteTest);
            Assert.Contains("&#39;", encodedQuote, "Single quote must be encoded as &#39; so it cannot break out of JS '...' string literal");
        }

        // =========================================================================
        // SECTION 3: FILE UPLOAD RESTRICTIONS & MALICIOUS EXTENSIONS (FEATURE 14 & 15)
        // =========================================================================

        [E2ETest(5, 14, "M2_C19: Valid image file extensions (.jpg, .jpeg, .png, .webp, .gif, .svg) accepted")]
        public async Task M2_C19_FileUpload_ValidExtensions_Allowed()
        {
            var validExts = new[] { "photo.jpg", "photo.jpeg", "logo.png", "banner.webp", "anim.gif", "vector.svg" };

            foreach (var fileName in validExts)
            {
                var controller = CreateTestController(out var context, out var env);
                var fakeContent = Encoding.UTF8.GetBytes("fake_image_bytes");
                var file = CreateMockFormFile(fileName, fakeContent);

                var vm = new SanPhamCrudViewModel
                {
                    TenSanPham = "Sản phẩm ảnh " + fileName,
                    GiaBan = 50000m,
                    SoLuongTon = 10,
                    FileHinhAnh = file
                };

                var result = await controller.Create(vm);
                Assert.True(controller.ModelState.IsValid, $"Valid extension '{fileName}' must NOT add ModelState errors, got: {string.Join(", ", controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                AssertIsType<RedirectToActionResult>(result, $"Valid extension '{fileName}' must redirect to Index");
            }
        }

        [E2ETest(5, 14, "M2_C20: Malicious / Executable extensions (.exe, .dll, .sh, .php, .bat, .asp, .js, .phtml, .py) rejected by controller")]
        public async Task M2_C20_FileUpload_MaliciousExtensions_RejectedByController()
        {
            var maliciousExts = new[] { "trojan.exe", "library.dll", "script.sh", "webshell.php", "batch.bat", "legacy.asp", "exploit.js", "backdoor.phtml", "attack.py" };

            foreach (var fileName in maliciousExts)
            {
                var controller = CreateTestController(out var context, out var env);
                var fakeContent = Encoding.UTF8.GetBytes("<?php echo 'hacked'; ?>");
                var file = CreateMockFormFile(fileName, fakeContent);

                var vm = new SanPhamCrudViewModel
                {
                    TenSanPham = "Sản phẩm độc hại " + fileName,
                    GiaBan = 50000m,
                    SoLuongTon = 10,
                    FileHinhAnh = file
                };

                var result = await controller.Create(vm);
                Assert.False(controller.ModelState.IsValid, $"Malicious file '{fileName}' must trigger ModelState error");
                Assert.True(controller.ModelState.ContainsKey(nameof(vm.FileHinhAnh)), $"ModelState error must target FileHinhAnh for '{fileName}'");
                AssertIsType<ViewResult>(result, $"Rejected upload for '{fileName}' must return ViewResult without redirecting");
            }
        }

        [E2ETest(5, 14, "M2_C21: Double extension bypass attempts (.png.php, .jpg.exe) rejected")]
        public async Task M2_C21_FileUpload_DoubleExtensionTrick_Rejected()
        {
            var doubleExtTricks = new[] { "image.png.php", "avatar.jpg.exe", "gift.webp.sh", "banner.gif.asp" };

            foreach (var fileName in doubleExtTricks)
            {
                var controller = CreateTestController(out var context, out var env);
                var fakeContent = Encoding.UTF8.GetBytes("malicious double ext content");
                var file = CreateMockFormFile(fileName, fakeContent);

                var vm = new SanPhamCrudViewModel
                {
                    TenSanPham = "Double Ext Test " + fileName,
                    GiaBan = 50000m,
                    SoLuongTon = 10,
                    FileHinhAnh = file
                };

                var result = await controller.Create(vm);
                Assert.False(controller.ModelState.IsValid, $"Double extension '{fileName}' must be rejected");
                AssertIsType<ViewResult>(result, $"Double extension '{fileName}' must return ViewResult");
            }
        }

        [E2ETest(5, 14, "M2_C22: Uppercase image extensions (.PNG, .JPG, .WEBP) accepted via case-insensitivity")]
        public async Task M2_C22_FileUpload_CaseInsensitiveExtensions_Accepted()
        {
            var upperExts = new[] { "PHOTO.PNG", "PHOTO.JPG", "PICTURE.JPEG", "BANNER.WEBP", "LOGO.SVG" };

            foreach (var fileName in upperExts)
            {
                var controller = CreateTestController(out var context, out var env);
                var fakeContent = Encoding.UTF8.GetBytes("uppercase_image_bytes");
                var file = CreateMockFormFile(fileName, fakeContent);

                var vm = new SanPhamCrudViewModel
                {
                    TenSanPham = "Upper Ext " + fileName,
                    GiaBan = 50000m,
                    SoLuongTon = 10,
                    FileHinhAnh = file
                };

                var result = await controller.Create(vm);
                Assert.True(controller.ModelState.IsValid, $"Uppercase extension '{fileName}' must be accepted via ToLowerInvariant()");
                AssertIsType<RedirectToActionResult>(result, $"Uppercase extension '{fileName}' must redirect to Index");
            }
        }

        [E2ETest(5, 14, "M2_C23: Oversized file upload (> 10MB) rejected with ModelState error")]
        public async Task M2_C23_FileUpload_OversizedFile_Rejected()
        {
            var controller = CreateTestController(out var context, out var env);
            var oversizedLength = 11 * 1024 * 1024;
            var stream = new MemoryStream(new byte[100]);
            var file = new FormFile(stream, 0, oversizedLength, "FileHinhAnh", "huge_photo.jpg");

            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Ảnh siêu to",
                GiaBan = 50000m,
                SoLuongTon = 10,
                FileHinhAnh = file
            };

            var result = await controller.Create(vm);
            Assert.False(controller.ModelState.IsValid, "Oversized file (>10MB) must trigger ModelState error");
            Assert.True(controller.ModelState[nameof(vm.FileHinhAnh)]?.Errors.Any(e => e.ErrorMessage.Contains("10MB")) == true, "Error message must mention 10MB limit");
            AssertIsType<ViewResult>(result, "Oversized file must return ViewResult");
        }

        [E2ETest(5, 14, "M2_C24: Zero-length or null file safely ignored and does not crash")]
        public async Task M2_C24_FileUpload_ZeroLengthOrNullFile_SafelyIgnored()
        {
            var controller = CreateTestController(out var context, out var env);
            var vmNull = new SanPhamCrudViewModel
            {
                TenSanPham = "Sản phẩm không có file",
                GiaBan = 50000m,
                SoLuongTon = 10,
                LinkHinhAnh = "/assets/journey.jpg",
                FileHinhAnh = null
            };

            var resultNull = await controller.Create(vmNull);
            Assert.True(controller.ModelState.IsValid, "Null file must not cause error");
            AssertIsType<RedirectToActionResult>(resultNull, "Null file should save with LinkHinhAnh and redirect");

            var controller2 = CreateTestController(out _, out _);
            var zeroFile = CreateMockFormFile("empty.jpg", Array.Empty<byte>());
            var vmZero = new SanPhamCrudViewModel
            {
                TenSanPham = "Sản phẩm file 0 byte",
                GiaBan = 50000m,
                SoLuongTon = 10,
                FileHinhAnh = zeroFile
            };

            var resultZero = await controller2.Create(vmZero);
            Assert.True(controller2.ModelState.IsValid, "0-byte file must be safely skipped");
            AssertIsType<RedirectToActionResult>(resultZero, "0-byte file should redirect to Index");
        }

        [E2ETest(5, 15, "M2_C25: Edit action with malicious file upload is rejected and preserves existing product")]
        public async Task M2_C25_EditPost_MaliciousFileUpload_RejectedAndPreservesEntity()
        {
            DbInitializer.ResetInMemoryStore();
            var p1 = DbInitializer.GetInMemoryProductById(1)!;
            var originalPrice = p1.GiaBan;
            var originalImg = p1.LinkHinhAnh;

            var controller = CreateTestController(out var context, out var env);
            var badFile = CreateMockFormFile("virus.exe", Encoding.UTF8.GetBytes("fake exe"));

            var vm = new SanPhamCrudViewModel
            {
                IDSanPham = 1,
                TenSanPham = "Cố tình hack",
                GiaBan = 999999m,
                SoLuongTon = 10,
                FileHinhAnh = badFile
            };

            var result = await controller.Edit(1, vm);
            Assert.False(controller.ModelState.IsValid, "Edit with virus.exe must fail ModelState");
            AssertIsType<ViewResult>(result, "Failed edit must return ViewResult");

            var checkItem = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal(originalPrice, checkItem.GiaBan, "Existing product price must not be updated on validation failure");
            Assert.Equal(originalImg, checkItem.LinkHinhAnh, "Existing product image must not be updated on validation failure");
        }

        // =========================================================================
        // SECTION 4: CONTROLLER ACTION LIFECYCLE & SECURITY BOUNDARIES (FEATURE 14, 15, 16)
        // =========================================================================

        [E2ETest(5, 12, "M2_C26: AdminSanPhamController declares Area('CuaHang') attribute")]
        public void M2_C26_AdminController_HasAreaAttribute()
        {
            var ctrlType = typeof(AdminSanPhamController);
            var areaAttr = ctrlType.GetCustomAttribute<AreaAttribute>();
            Assert.NotNull(areaAttr, "AdminSanPhamController must have [Area] attribute");
            Assert.Equal("CuaHang", areaAttr!.RouteValue, "Area attribute route value must be 'CuaHang'");
        }

        [E2ETest(5, 14, "M2_C27: All POST actions enforce [ValidateAntiForgeryToken]")]
        public void M2_C27_AdminController_PostActions_EnforceValidateAntiForgeryToken()
        {
            var ctrlType = typeof(AdminSanPhamController);
            var postMethods = ctrlType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<HttpPostAttribute>() != null)
                .ToList();

            Assert.True(postMethods.Count >= 3, "Controller must declare at least 3 POST actions (Create, Edit, Delete)");

            foreach (var method in postMethods)
            {
                var afAttr = method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();
                Assert.NotNull(afAttr, $"POST action '{method.Name}' must be protected with [ValidateAntiForgeryToken]");
            }
        }

        [E2ETest(5, 14, "M2_C28: Create POST with invalid model returns View(model) without persisting")]
        public async Task M2_C28_AdminController_CreatePost_InvalidModel_ReturnsViewWithoutPersisting()
        {
            DbInitializer.ResetInMemoryStore();
            var countBefore = DbInitializer.GetInMemoryProducts().Count;

            var controller = CreateTestController(out _, out _);
            controller.ModelState.AddModelError("TenSanPham", "Tên sản phẩm bắt buộc");

            var vm = new SanPhamCrudViewModel { TenSanPham = "", GiaBan = 10000m };
            var result = await controller.Create(vm);

            var viewResult = AssertIsType<ViewResult>(result, "Create with invalid ModelState must return ViewResult");
            Assert.True(object.ReferenceEquals(vm, viewResult.Model), "Returned view model must be the same submitted model");

            var countAfter = DbInitializer.GetInMemoryProducts().Count;
            Assert.Equal(countBefore, countAfter, "No new product should be persisted when ModelState is invalid");
        }

        [E2ETest(5, 15, "M2_C29: Edit POST with ID mismatch (tampering) redirects to Index with error")]
        public async Task M2_C29_AdminController_EditPost_IdMismatch_RedirectsWithError()
        {
            var controller = CreateTestController(out _, out _);
            var vm = new SanPhamCrudViewModel { IDSanPham = 2, TenSanPham = "Tampered ID", GiaBan = 50000m };

            var result = await controller.Edit(1, vm);

            var redirectResult = AssertIsType<RedirectToActionResult>(result, "ID mismatch must redirect");
            Assert.Equal(nameof(AdminSanPhamController.Index), redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Error"], "TempData['Error'] must be set on ID mismatch");
        }

        [E2ETest(5, 15, "M2_C30: Edit GET with non-positive ID (id <= 0) redirects to Index with error")]
        public async Task M2_C30_AdminController_EditGet_NonPositiveId_RedirectsWithError()
        {
            var controller = CreateTestController(out _, out _);
            var resultZero = await controller.Edit(0);
            var redirectResultZero = AssertIsType<RedirectToActionResult>(resultZero, "Edit(0) must redirect");
            Assert.Equal("Index", redirectResultZero.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Error"], "TempData['Error'] must be set on id <= 0");

            var resultNeg = await controller.Edit(-5);
            var redirectResultNeg = AssertIsType<RedirectToActionResult>(resultNeg, "Edit(-5) must redirect");
            Assert.Equal("Index", redirectResultNeg.ActionName, "Must redirect to Index");
        }

        [E2ETest(5, 15, "M2_C31: Edit GET with non-existent ID redirects to Index with error")]
        public async Task M2_C31_AdminController_EditGet_NonExistentId_RedirectsWithError()
        {
            DbInitializer.ResetInMemoryStore();
            var controller = CreateTestController(out _, out _);
            var result = await controller.Edit(99999);

            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Edit(99999) must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Error"], "TempData['Error'] must notify not found");
        }

        [E2ETest(5, 16, "M2_C32: Delete POST with non-positive ID (id <= 0) redirects to Index with error")]
        public async Task M2_C32_AdminController_DeletePost_NonPositiveId_RedirectsWithError()
        {
            var controller = CreateTestController(out _, out _);
            var result = await controller.Delete(0);

            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Delete(0) must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Error"], "TempData['Error'] must be set on id <= 0");
        }

        [E2ETest(5, 16, "M2_C33: Delete POST with non-existent ID handled safely without throwing NullReferenceException")]
        public async Task M2_C33_AdminController_DeletePost_NonExistentId_SafeHandling()
        {
            DbInitializer.ResetInMemoryStore();
            var controller = CreateTestController(out _, out _);
            var result = await controller.Delete(99999);

            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Delete non-existent ID must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Error"], "TempData['Error'] must inform product was not found");
        }

        [E2ETest(5, 14, "M2_C34: Create POST valid model persists and redirects with Success message")]
        public async Task M2_C34_AdminController_CreatePost_ValidModel_PersistsAndRedirects()
        {
            DbInitializer.ResetInMemoryStore();
            var controller = CreateTestController(out _, out _);

            var vm = new SanPhamCrudViewModel
            {
                TenSanPham = "Nón Tai Bèo Tình Nguyện",
                GiaBan = 69000m,
                GiaGoc = 89000m,
                SoLuongTon = 40,
                PhanTramDongGop = 100,
                DanhMuc = "Phụ kiện",
                ThongDiepGayQuy = "Góp quỹ nụ cười tuổi thơ Langbiang"
            };

            var result = await controller.Create(vm);
            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Valid Create must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Success"], "TempData['Success'] must be set on creation");

            var products = DbInitializer.GetInMemoryProducts();
            Assert.True(products.Any(p => p.TenSanPham == "Nón Tai Bèo Tình Nguyện"), "New product must exist in store");
        }

        [E2ETest(5, 15, "M2_C35: Edit POST valid model updates entity and redirects with Success message")]
        public async Task M2_C35_AdminController_EditPost_ValidModel_UpdatesAndRedirects()
        {
            DbInitializer.ResetInMemoryStore();
            var controller = CreateTestController(out _, out _);

            var vm = new SanPhamCrudViewModel
            {
                IDSanPham = 1,
                TenSanPham = "Áo Thun Trăng Sáng Bản Cải Tiến 2026",
                GiaBan = 139000m,
                GiaGoc = 160000m,
                SoLuongTon = 80,
                PhanTramDongGop = 100,
                DanhMuc = "Trang phục",
                LinkHinhAnh = "/assets/journey.jpg"
            };

            var result = await controller.Edit(1, vm);
            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Valid Edit must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Success"], "TempData['Success'] must be set on update");

            var updated = DbInitializer.GetInMemoryProductById(1)!;
            Assert.Equal("Áo Thun Trăng Sáng Bản Cải Tiến 2026", updated.TenSanPham, "Product name must be updated");
            Assert.Equal(139000m, updated.GiaBan, "Price must be updated");
            Assert.Equal(80, updated.SoLuongTon, "Stock must be updated");
        }

        [E2ETest(5, 16, "M2_C36: Delete POST valid ID removes entity and redirects with Success message")]
        public async Task M2_C36_AdminController_DeletePost_ValidModel_DeletesAndRedirects()
        {
            DbInitializer.ResetInMemoryStore();
            var controller = CreateTestController(out _, out _);

            var result = await controller.Delete(4);
            var redirectResult = AssertIsType<RedirectToActionResult>(result, "Valid Delete must redirect");
            Assert.Equal("Index", redirectResult.ActionName, "Must redirect to Index");
            Assert.NotNull(controller.TempData["Success"], "TempData['Success'] must be set on deletion");

            var deleted = DbInitializer.GetInMemoryProductById(4);
            Assert.Null(deleted, "Product #4 must be deleted from store");
        }

        // =========================================================================
        // SECTION 5: IN-MEMORY FALLBACK CONCURRENCY & RELATIONAL RESILIENCE
        // =========================================================================

        [E2ETest(5, 16, "M2_C37: Rapid Admin CRUD under concurrent multi-threaded stress maintains store integrity")]
        public void M2_C37_DbInitializer_AdminCrudConcurrentBombardment()
        {
            DbInitializer.ResetInMemoryStore();

            Parallel.For(0, 60, i =>
            {
                if (i % 3 == 0)
                {
                    DbInitializer.AddInMemoryProduct(new SanPhamGayQuy
                    {
                        TenSanPham = $"Sản phẩm stress #{i}",
                        GiaBan = 50000m + i,
                        SoLuongTon = 20
                    });
                }
                else if (i % 3 == 1)
                {
                    var prod = DbInitializer.GetInMemoryProductById(1);
                    if (prod != null)
                    {
                        prod.SoLuongTon += 1;
                        DbInitializer.UpdateInMemoryProduct(prod);
                    }
                }
                else
                {
                    var all = DbInitializer.GetInMemoryProducts();
                    Assert.NotNull(all, "GetInMemoryProducts must not be null during concurrent bombardment");
                }
            });

            var finalProducts = DbInitializer.GetInMemoryProducts();
            Assert.True(finalProducts.Count >= 4, "Store must remain healthy and intact after concurrent admin operations");
        }

        // =========================================================================
        // SECTION 6: VIEW TEMPLATES CONFORMANCE & LAYOUT INTEGRATION (FEATURE 12, 13, 14, 15)
        // =========================================================================

        [E2ETest(5, 14, "M2_C38: Create view template conforms to multipart encoding, anti-forgery, and styling")]
        public void M2_C38_CreateView_ContainsMultipartAndAntiForgeryToken()
        {
            var createViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Create.cshtml");
            Assert.RequireFile(createViewPath, "Create.cshtml view");

            var content = File.ReadAllText(createViewPath);
            Assert.Contains("enctype=\"multipart/form-data\"", content, "Create.cshtml must have enctype='multipart/form-data'");
            Assert.Contains("AntiForgeryToken", content, "Create.cshtml must render AntiForgeryToken");
            Assert.Contains("FileHinhAnh", content, "Create.cshtml must have FileHinhAnh input");
            Assert.Contains("PhanTramDongGop", content, "Create.cshtml must have PhanTramDongGop input");
        }

        [E2ETest(5, 15, "M2_C39: Edit view template conforms to multipart encoding, hidden ID, and anti-forgery")]
        public void M2_C39_EditView_ContainsMultipartAntiForgeryTokenAndHiddenId()
        {
            var editViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Edit.cshtml");
            Assert.RequireFile(editViewPath, "Edit.cshtml view");

            var content = File.ReadAllText(editViewPath);
            Assert.Contains("enctype=\"multipart/form-data\"", content, "Edit.cshtml must have enctype='multipart/form-data'");
            Assert.Contains("AntiForgeryToken", content, "Edit.cshtml must render AntiForgeryToken");
            Assert.Contains("IDSanPham", content, "Edit.cshtml must bind IDSanPham");
            Assert.Contains("FileHinhAnh", content, "Edit.cshtml must have FileHinhAnh input for replacement");
        }

        [E2ETest(5, 13, "M2_C40: Index view table has action buttons, search bar, and delete post form")]
        public void M2_C40_IndexView_ContainsPostDeleteFormWithAntiForgeryToken()
        {
            var indexViewPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "AdminSanPham", "Index.cshtml");
            Assert.RequireFile(indexViewPath, "Index.cshtml view");

            var content = File.ReadAllText(indexViewPath);
            Assert.Contains("asp-action=\"Delete\"", content, "Index.cshtml must have delete action");
            Assert.Contains("method=\"post\"", content, "Delete action must use POST method");
            Assert.Contains("AntiForgeryToken", content, "Delete form must include AntiForgeryToken");
            Assert.Contains("confirm(", content, "Delete action must have confirmation prompt");
            Assert.Contains("adminSearchInput", content, "Index.cshtml must have instant search filter");
        }

        [E2ETest(5, 12, "M2_C41: Admin Sidebar in _AdminLayout.cshtml links to /CuaHang/AdminSanPham with boxes icon")]
        public void M2_C41_AdminLayout_SidebarContainsCorrectLinkAndIcon()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_AdminLayout.cshtml");
            Assert.RequireFile(layoutPath, "_AdminLayout.cshtml");

            var content = File.ReadAllText(layoutPath);
            Assert.Contains("AdminSanPham", content, "_AdminLayout.cshtml must link to AdminSanPham");
            Assert.Contains("Sản phẩm", content, "_AdminLayout.cshtml link text must contain 'Sản phẩm'");
            Assert.Contains("fa-boxes", content, "_AdminLayout.cshtml must use fa-boxes icon");
        }
    }
}
