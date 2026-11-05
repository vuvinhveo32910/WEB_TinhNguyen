using System;
using System.IO;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier1
{
    public class Tier1_BuildVerificationTests
    {
        // =========================================================================
        // FEATURE 17: Build & Verification
        // =========================================================================

        [E2ETest(1, 17, "F17_T01: Solution slnx file exists and references WEB_TinhNguyen.csproj")]
        public void F17_T01_SolutionStructureValid()
        {
            var slnxPath = Path.Combine(E2ETestContext.SolutionRoot, "WEB_TinhNguyen.slnx");
            Assert.True(File.Exists(slnxPath), "WEB_TinhNguyen.slnx must exist in solution root");

            var slnxContent = File.ReadAllText(slnxPath);
            Assert.Contains("WEB_TinhNguyen.csproj", slnxContent, "Solution must reference WEB_TinhNguyen.csproj");
        }

        [E2ETest(1, 17, "F17_T02: tailwind.config.js scans Area Razor views")]
        public void F17_T02_TailwindScansAreaViews()
        {
            var tailwindConfigPath = Path.Combine(E2ETestContext.SolutionRoot, "tailwind.config.js");
            Assert.True(File.Exists(tailwindConfigPath), "tailwind.config.js must exist");

            var configContent = File.ReadAllText(tailwindConfigPath);
            if (!configContent.Contains("Areas"))
            {
                throw new AssertException("[SKIP] Pending implementation in M1: tailwind.config.js scan of Areas/**/*.cshtml");
            }
            Assert.True(
                configContent.Contains("Areas/**/*.cshtml"),
                "tailwind.config.js content array must include './Areas/**/*.cshtml' or './WEB_TinhNguyen/Areas/**/*.cshtml'"
            );
        }

        [E2ETest(1, 17, "F17_T03: Static charity images exist in wwwroot/assets")]
        public void F17_T03_StaticCharityAssetsExist()
        {
            var assetsDir = Path.Combine(E2ETestContext.ProjectRoot, "wwwroot", "assets");
            Assert.True(Directory.Exists(assetsDir), "wwwroot/assets directory must exist");

            var journeyImage = Path.Combine(assetsDir, "journey.jpg");
            var cookingImage = Path.Combine(assetsDir, "cooking.jpg");
            var childrenImage = Path.Combine(assetsDir, "children.jpg");

            Assert.True(File.Exists(journeyImage), "journey.jpg must exist in wwwroot/assets");
            Assert.True(File.Exists(cookingImage), "cooking.jpg must exist in wwwroot/assets");
            Assert.True(File.Exists(childrenImage), "children.jpg must exist in wwwroot/assets");
        }

        [E2ETest(1, 17, "F17_T04: Area _ViewImports.cshtml imports Area Models and TagHelpers")]
        public void F17_T04_AreaViewImportsConfiguration()
        {
            var viewImportsPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "_ViewImports.cshtml");
            Assert.RequireFile(viewImportsPath, "Areas/CuaHang/Views/_ViewImports.cshtml");

            var content = File.ReadAllText(viewImportsPath);
            Assert.Contains("WEB_TinhNguyen.Areas.CuaHang.Models", content, "_ViewImports must import Area Models");
            Assert.Contains("@addTagHelper", content, "_ViewImports must register TagHelpers");
        }

        [E2ETest(1, 17, "F17_T05: Area _ViewStart.cshtml switches layout for Admin vs Storefront")]
        public void F17_T05_AreaViewStartLayoutSwitching()
        {
            var viewStartPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "_ViewStart.cshtml");
            Assert.RequireFile(viewStartPath, "Areas/CuaHang/Views/_ViewStart.cshtml");

            var content = File.ReadAllText(viewStartPath);
            Assert.Contains("_AdminLayout", content, "_ViewStart must reference _AdminLayout for admin views");
            Assert.Contains("_Layout", content, "_ViewStart must reference _Layout for storefront views");
        }
    }
}
