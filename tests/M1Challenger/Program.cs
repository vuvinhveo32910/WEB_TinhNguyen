using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using WEB_TinhNguyen.DuLieu;
using WEB_TinhNguyen.MoHinh;
using WEB_TinhNguyen.BoDieuKhien;

namespace WEB_TinhNguyen.Tests.M1Challenger
{
    public class Program
    {
        private static int passed = 0;
        private static int failed = 0;
        private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../WEB_TinhNguyen"));

        public static int Main(string[] args)
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine(" EMPIRICAL CHALLENGER 2: MILESTONE M1 STRESS HARNESS");
            Console.WriteLine($" Target Project Root: {ProjectRoot}");
            Console.WriteLine("================================================================================");

            Test_01_VietnameseViewLocationExpander_Registration();
            Test_02_RootControllerViewsResolution();
            Test_03_AreaControllerViewsResolution();
            Test_04_SharedLayoutsResolutionFromRoot();
            Test_05_SharedLayoutsResolutionFromArea();
            Test_06_ErrorViewResolutionUnderBothContexts();
            Test_07_LinuxFileSystemCaseSensitivity();
            Test_08_OrphanedFilesCheck();
            Test_09_ViewImportsAndCompatibility();

            Console.WriteLine("================================================================================");
            Console.WriteLine($" RESULTS: {passed} PASSED, {failed} FAILED");
            Console.WriteLine("================================================================================");

            return failed == 0 ? 0 : 1;
        }

        private static void Assert(bool condition, string testName, string details = "")
        {
            if (condition)
            {
                passed++;
                Console.WriteLine($" [PASS] {testName}");
            }
            else
            {
                failed++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [FAIL] {testName} - {details}");
                Console.ResetColor();
            }
        }

        private static ViewLocationExpanderContext CreateContext(string? area, string controller, string view)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ViewLocationExpanderContext(actionContext, view, controller, area, null, false);
        }

        private static string? ResolveExistingFile(IEnumerable<string> formatPatterns, string view, string controller, string? area)
        {
            foreach (var pattern in formatPatterns)
            {
                string relPath;
                if (!string.IsNullOrEmpty(area))
                {
                    relPath = pattern
                        .Replace("{0}", view)
                        .Replace("{1}", controller)
                        .Replace("{2}", area);
                }
                else
                {
                    relPath = pattern
                        .Replace("{0}", view)
                        .Replace("{1}", controller);
                }

                if (relPath.StartsWith("/")) relPath = relPath.Substring(1);
                var fullPath = Path.Combine(ProjectRoot, relPath);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        // =========================================================================
        // TEST SUITE 1: Razor View Engine Expander Contract
        // =========================================================================
        private static void Test_01_VietnameseViewLocationExpander_Registration()
        {
            var expander = new VietnameseViewLocationExpander();
            
            // Root context
            var rootCtx = CreateContext(null, "Home", "Index");
            var rootFormats = expander.ExpandViewLocations(rootCtx, Enumerable.Empty<string>()).ToList();
            
            Assert(rootFormats.Contains("/GiaoDien/{1}/{0}.cshtml"), "Root formats include /GiaoDien/{1}/{0}.cshtml");
            Assert(rootFormats.Contains("/GiaoDien/Shared/{0}.cshtml"), "Root formats include /GiaoDien/Shared/{0}.cshtml");
            Assert(rootFormats.Contains("/Views/{1}/{0}.cshtml"), "Root formats include fallback /Views/{1}/{0}.cshtml");
            Assert(rootFormats.Contains("/Views/Shared/{0}.cshtml"), "Root formats include fallback /Views/Shared/{0}.cshtml");

            // Area context
            var areaCtx = CreateContext("CuaHang", "CuaHang", "Index");
            var areaFormats = expander.ExpandViewLocations(areaCtx, Enumerable.Empty<string>()).ToList();

            Assert(areaFormats.Contains("/Areas/{2}/Views/{1}/{0}.cshtml"), "Area formats include /Areas/{2}/Views/{1}/{0}.cshtml");
            Assert(areaFormats.Contains("/Areas/{2}/Views/Shared/{0}.cshtml"), "Area formats include /Areas/{2}/Views/Shared/{0}.cshtml");
            Assert(areaFormats.Contains("/GiaoDien/Shared/{0}.cshtml"), "Area formats include cross-area fallback /GiaoDien/Shared/{0}.cshtml");
        }

        // =========================================================================
        // TEST SUITE 2: Root Controllers View Resolution
        // =========================================================================
        private static void Test_02_RootControllerViewsResolution()
        {
            var expander = new VietnameseViewLocationExpander();
            var actionsToTest = new (string Controller, string Action, string ExpectedSubPath)[]
            {
                ("Home", "Index", "GiaoDien/Home/Index.cshtml"),
                ("Home", "ChuongTrinh", "GiaoDien/Home/ChuongTrinh.cshtml"),
                ("Home", "GayQuy", "GiaoDien/Home/GayQuy.cshtml"),
                ("Home", "BanToChuc", "GiaoDien/Home/BanToChuc.cshtml"),
                ("Home", "TinTuc", "GiaoDien/Home/TinTuc.cshtml"),
                ("Home", "LoiChuc", "GiaoDien/Home/LoiChuc.cshtml"),
                ("Home", "KyUc", "GiaoDien/Home/KyUc.cshtml"),
                ("Home", "Privacy", "GiaoDien/Home/Privacy.cshtml"),
                ("Home", "ComingSoon", "GiaoDien/Home/ComingSoon.cshtml"),
                ("Admin", "Index", "GiaoDien/Admin/Index.cshtml"),
                ("AdminTrangChu", "Index", "GiaoDien/AdminTrangChu/Index.cshtml"),
                ("AdminTrangChu", "Edit", "GiaoDien/AdminTrangChu/Edit.cshtml"),
                ("AdminChuongTrinh", "Index", "GiaoDien/AdminChuongTrinh/Index.cshtml"),
                ("AdminChuongTrinh", "Edit", "GiaoDien/AdminChuongTrinh/Edit.cshtml"),
                ("AdminGayQuy", "Index", "GiaoDien/AdminGayQuy/Index.cshtml"),
                ("AdminGayQuy", "Edit", "GiaoDien/AdminGayQuy/Edit.cshtml"),
                ("AdminBanToChuc", "Index", "GiaoDien/AdminBanToChuc/Index.cshtml"),
                ("AdminBanToChuc", "Edit", "GiaoDien/AdminBanToChuc/Edit.cshtml"),
                ("AdminTinTuc", "Index", "GiaoDien/AdminTinTuc/Index.cshtml"),
                ("AdminTinTuc", "Edit", "GiaoDien/AdminTinTuc/Edit.cshtml"),
                ("AdminKyUc", "Index", "GiaoDien/AdminKyUc/Index.cshtml"),
                ("AdminKyUc", "Edit", "GiaoDien/AdminKyUc/Edit.cshtml"),
                ("AdminHieuUng", "Index", "GiaoDien/AdminHieuUng/Index.cshtml")
            };

            foreach (var (controller, action, expectedSubPath) in actionsToTest)
            {
                var ctx = CreateContext(null, controller, action);
                var formats = expander.ExpandViewLocations(ctx, Enumerable.Empty<string>());
                var resolved = ResolveExistingFile(formats, action, controller, null);
                
                var expectedFullPath = Path.Combine(ProjectRoot, expectedSubPath);
                Assert(resolved != null && string.Equals(resolved, expectedFullPath, StringComparison.Ordinal),
                    $"Resolve Root View: {controller}/{action}",
                    $"Expected {expectedFullPath}, got {resolved ?? "NULL"}");
            }
        }

        // =========================================================================
        // TEST SUITE 3: Area Controller View Resolution
        // =========================================================================
        private static void Test_03_AreaControllerViewsResolution()
        {
            var expander = new VietnameseViewLocationExpander();
            var areaActions = new (string Controller, string Action, string ExpectedSubPath)[]
            {
                ("CuaHang", "Index", "Areas/CuaHang/Views/CuaHang/Index.cshtml"),
                ("CuaHang", "ChiTiet", "Areas/CuaHang/Views/CuaHang/ChiTiet.cshtml"),
                ("GioHang", "Index", "Areas/CuaHang/Views/GioHang/Index.cshtml"),
                ("GioHang", "DatHangThanhCong", "Areas/CuaHang/Views/GioHang/DatHangThanhCong.cshtml"),
                ("AdminSanPham", "Index", "Areas/CuaHang/Views/AdminSanPham/Index.cshtml"),
                ("AdminSanPham", "Create", "Areas/CuaHang/Views/AdminSanPham/Create.cshtml"),
                ("AdminSanPham", "Edit", "Areas/CuaHang/Views/AdminSanPham/Edit.cshtml"),
                ("AdminDonHang", "Index", "Areas/CuaHang/Views/AdminDonHang/Index.cshtml"),
                ("AdminDonHang", "ChiTiet", "Areas/CuaHang/Views/AdminDonHang/ChiTiet.cshtml")
            };

            foreach (var (controller, action, expectedSubPath) in areaActions)
            {
                var ctx = CreateContext("CuaHang", controller, action);
                var formats = expander.ExpandViewLocations(ctx, Enumerable.Empty<string>());
                var resolved = ResolveExistingFile(formats, action, controller, "CuaHang");

                var expectedFullPath = Path.Combine(ProjectRoot, expectedSubPath);
                Assert(resolved != null && string.Equals(resolved, expectedFullPath, StringComparison.Ordinal),
                    $"Resolve Area View: CuaHang/{controller}/{action}",
                    $"Expected {expectedFullPath}, got {resolved ?? "NULL"}");
            }
        }

        // =========================================================================
        // TEST SUITE 4: Shared Layout Resolution from Root Context
        // =========================================================================
        private static void Test_04_SharedLayoutsResolutionFromRoot()
        {
            var expander = new VietnameseViewLocationExpander();
            var sharedViews = new[] { "_Layout", "_AdminLayout", "_ValidationScriptsPartial" };

            foreach (var viewName in sharedViews)
            {
                var ctx = CreateContext(null, "Home", viewName);
                var formats = expander.ExpandViewLocations(ctx, Enumerable.Empty<string>());
                var resolved = ResolveExistingFile(formats, viewName, "Home", null);

                var expectedFullPath = Path.Combine(ProjectRoot, "GiaoDien/Shared", $"{viewName}.cshtml");
                Assert(resolved != null && string.Equals(resolved, expectedFullPath, StringComparison.Ordinal),
                    $"Resolve Root Shared View: {viewName}",
                    $"Expected {expectedFullPath}, got {resolved ?? "NULL"}");
            }
        }

        // =========================================================================
        // TEST SUITE 5: Shared Layout Resolution from Area Context
        // =========================================================================
        private static void Test_05_SharedLayoutsResolutionFromArea()
        {
            var expander = new VietnameseViewLocationExpander();
            var sharedViews = new[] { "_Layout", "_AdminLayout", "_ValidationScriptsPartial" };

            foreach (var viewName in sharedViews)
            {
                var ctx = CreateContext("CuaHang", "CuaHang", viewName);
                var formats = expander.ExpandViewLocations(ctx, Enumerable.Empty<string>());
                var resolved = ResolveExistingFile(formats, viewName, "CuaHang", "CuaHang");

                // Since Area CuaHang does NOT have its own _Layout.cshtml yet, it must fallback to GiaoDien/Shared/
                var expectedFullPath = Path.Combine(ProjectRoot, "GiaoDien/Shared", $"{viewName}.cshtml");
                Assert(resolved != null && string.Equals(resolved, expectedFullPath, StringComparison.Ordinal),
                    $"Resolve Area Shared Fallback: {viewName}",
                    $"Expected {expectedFullPath}, got {resolved ?? "NULL"}");
            }
        }

        // =========================================================================
        // TEST SUITE 6: Error View Resolution Under Both Contexts
        // =========================================================================
        private static void Test_06_ErrorViewResolutionUnderBothContexts()
        {
            var expander = new VietnameseViewLocationExpander();
            var expectedErrorPath = Path.Combine(ProjectRoot, "GiaoDien/Shared/Error.cshtml");

            // 1. Root context (e.g. HomeController.Error())
            var rootCtx = CreateContext(null, "Home", "Error");
            var rootFormats = expander.ExpandViewLocations(rootCtx, Enumerable.Empty<string>());
            var rootResolved = ResolveExistingFile(rootFormats, "Error", "Home", null);
            Assert(rootResolved != null && string.Equals(rootResolved, expectedErrorPath, StringComparison.Ordinal),
                "Resolve Error View from Root Context (/Home/Error)",
                $"Expected {expectedErrorPath}, got {rootResolved ?? "NULL"}");

            // 2. Area context (e.g. unhandled error inside /CuaHang/...)
            var areaCtx = CreateContext("CuaHang", "CuaHang", "Error");
            var areaFormats = expander.ExpandViewLocations(areaCtx, Enumerable.Empty<string>());
            var areaResolved = ResolveExistingFile(areaFormats, "Error", "CuaHang", "CuaHang");
            Assert(areaResolved != null && string.Equals(areaResolved, expectedErrorPath, StringComparison.Ordinal),
                "Resolve Error View from Area Context (/CuaHang/Error fallback)",
                $"Expected {expectedErrorPath}, got {areaResolved ?? "NULL"}");
        }

        // =========================================================================
        // TEST SUITE 7: Linux Case Sensitivity Verification
        // =========================================================================
        private static void Test_07_LinuxFileSystemCaseSensitivity()
        {
            // Verify that all directories in GiaoDien/ strictly match PascalCase
            string[] expectedDirs = { "Admin", "AdminBanToChuc", "AdminChuongTrinh", "AdminGayQuy", 
                                      "AdminHieuUng", "AdminKyUc", "AdminTinTuc", "AdminTrangChu", 
                                      "Home", "Shared" };

            var giaoDienDir = Path.Combine(ProjectRoot, "GiaoDien");
            Assert(Directory.Exists(giaoDienDir), "Directory GiaoDien exists exactly with correct case");

            var actualSubDirs = Directory.GetDirectories(giaoDienDir).Select(Path.GetFileName).ToHashSet();
            foreach (var expected in expectedDirs)
            {
                Assert(actualSubDirs.Contains(expected),
                    $"GiaoDien subfolder case sensitivity: {expected}",
                    $"Missing or mis-cased: {expected}");
            }
        }

        // =========================================================================
        // TEST SUITE 8: Orphaned Files Check
        // =========================================================================
        private static void Test_08_OrphanedFilesCheck()
        {
            string[] forbiddenDirs = { "Controllers", "Models", "Views", "Data" };
            foreach (var dir in forbiddenDirs)
            {
                var fullPath = Path.Combine(ProjectRoot, dir);
                Assert(!Directory.Exists(fullPath),
                    $"Forbidden root directory non-existence: {dir}",
                    $"Directory {fullPath} still exists on disk!");
            }

            // Verify no .cs or .cshtml files in root directory outside authorized folders
            var rootFiles = Directory.GetFiles(ProjectRoot, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".cs") || f.EndsWith(".cshtml"))
                .Select(Path.GetFileName)
                .ToList();

            // Only Program.cs should be in root
            bool onlyProgramCs = rootFiles.Count == 1 && rootFiles[0] == "Program.cs";
            Assert(onlyProgramCs,
                "Root folder contains no rogue .cs or .cshtml files (only Program.cs allowed)",
                $"Found: {string.Join(", ", rootFiles)}");
        }

        // =========================================================================
        // TEST SUITE 9: ViewImports and Compatibility
        // =========================================================================
        private static void Test_09_ViewImportsAndCompatibility()
        {
            var rootImportsPath = Path.Combine(ProjectRoot, "GiaoDien/_ViewImports.cshtml");
            Assert(File.Exists(rootImportsPath), "GiaoDien/_ViewImports.cshtml exists");
            var rootImports = File.ReadAllText(rootImportsPath);
            Assert(rootImports.Contains("WEB_TinhNguyen.MoHinh"), "GiaoDien/_ViewImports.cshtml imports WEB_TinhNguyen.MoHinh");

            var areaImportsPath = Path.Combine(ProjectRoot, "Areas/CuaHang/Views/_ViewImports.cshtml");
            Assert(File.Exists(areaImportsPath), "Areas/CuaHang/Views/_ViewImports.cshtml exists");
            var areaImports = File.ReadAllText(areaImportsPath);
            Assert(areaImports.Contains("WEB_TinhNguyen.MoHinh"), "Areas/CuaHang/Views/_ViewImports.cshtml imports WEB_TinhNguyen.MoHinh");
        }
    }
}
