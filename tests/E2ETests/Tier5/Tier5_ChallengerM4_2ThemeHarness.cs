using System;
using System.IO;
using WEB_TinhNguyen.Tests.E2E.Framework;

namespace WEB_TinhNguyen.Tests.E2E.Tier5
{
    /// <summary>
    /// Tier 5: Adversarial & Verification Harness for Milestone M4 (Requirement R4: Light/Dark Mode)
    /// Validates:
    /// 1. Toggle button on Header Desktop and Mobile with fa-moon / fa-sun icons
    /// 2. Toggle button conditional rendering outside CuaHang area
    /// 3. Anti-FOUC script in <head> checking site_theme and enforcing light mode on /cuahang
    /// 4. JavaScript toggleThemeMode() controlling dark/light classes and localStorage
    /// 5. CSS variable palette for Western Highlands night theme (html.dark) and volunteer light theme
    /// 6. Complete isolation and hard-locked light mode in CuaHang layout
    /// </summary>
    public class Tier5_ChallengerM4_2ThemeHarness
    {
        private static string GetLayoutContent()
        {
            var layoutPath = Path.Combine(E2ETestContext.ViewsRoot, "Shared", "_Layout.cshtml");
            Assert.True(File.Exists(layoutPath), "_Layout.cshtml must exist at " + layoutPath);
            return File.ReadAllText(layoutPath);
        }

        private static string GetCuaHangLayoutContent()
        {
            var layoutPath = Path.Combine(E2ETestContext.AreasRoot, "Views", "Shared", "_CuaHangLayout.cshtml");
            Assert.True(File.Exists(layoutPath), "_CuaHangLayout.cshtml must exist at " + layoutPath);
            return File.ReadAllText(layoutPath);
        }

        [E2ETest(5, 19, "M4_01: Header Desktop contains Light/Dark mode toggle button with id themeToggleBtn")]
        public void M4_01_Header_Desktop_Contains_ThemeToggleBtn()
        {
            var content = GetLayoutContent();
            Assert.Contains("id=\"themeToggleBtn\"", content, "_Layout.cshtml must declare desktop button with id='themeToggleBtn'");
            Assert.Contains("onclick=\"toggleThemeMode()\"", content, "Theme toggle button must invoke toggleThemeMode()");
        }

        [E2ETest(5, 19, "M4_02: Header Mobile contains Light/Dark mode toggle button with id themeToggleBtnMobile")]
        public void M4_02_Header_Mobile_Contains_ThemeToggleBtnMobile()
        {
            var content = GetLayoutContent();
            Assert.Contains("id=\"themeToggleBtnMobile\"", content, "_Layout.cshtml must declare mobile button with id='themeToggleBtnMobile'");
            Assert.Contains("md:hidden", content, "Mobile toggle button container must use md:hidden breakpoint");
        }

        [E2ETest(5, 19, "M4_03: Toggle button contains Moon and Sun icons with rotation transitions")]
        public void M4_03_Toggle_Button_Contains_Moon_And_Sun_Icons()
        {
            var content = GetLayoutContent();
            Assert.Contains("fa-moon", content, "Theme toggle button must include fa-moon icon for light mode");
            Assert.Contains("fa-sun", content, "Theme toggle button must include fa-sun icon for dark mode");
            Assert.Contains("rotate-0", content, "Toggle icon must specify transition rotation classes");
        }

        [E2ETest(5, 19, "M4_04: Toggle button is conditionally rendered outside CuaHang area")]
        public void M4_04_Toggle_Button_Gated_Outside_CuaHang()
        {
            var content = GetLayoutContent();
            Assert.True(
                content.Contains("ViewContext.RouteData.Values[\"area\"]?.ToString() != \"CuaHang\"") ||
                content.Contains("ViewContext.RouteData.Values[\"Area\"]?.ToString() != \"CuaHang\""),
                "Toggle button must be guarded with @if checking that area != 'CuaHang'"
            );
        }

        [E2ETest(5, 19, "M4_05: Head contains Anti-FOUC script checking site_theme and path isolation")]
        public void M4_05_Head_Contains_Anti_FOUC_Script()
        {
            var content = GetLayoutContent();
            Assert.Contains("site_theme", content, "_Layout.cshtml must inspect site_theme in localStorage");
            Assert.Contains("/cuahang", content, "Anti-FOUC script must check for /cuahang route");
            Assert.Contains("document.documentElement.classList.remove('dark')", content,
                "Anti-FOUC script must remove dark class when in /cuahang or in light mode");
            Assert.Contains("document.documentElement.classList.add('light')", content,
                "Anti-FOUC script must add light class for default light mode");
        }

        [E2ETest(5, 19, "M4_06: toggleThemeMode function handles class switching and localStorage persistence")]
        public void M4_06_ToggleThemeMode_Function_Implementation()
        {
            var content = GetLayoutContent();
            Assert.Contains("function toggleThemeMode()", content, "_Layout.cshtml must declare toggleThemeMode()");
            Assert.Contains("localStorage.setItem('site_theme'", content,
                "toggleThemeMode must persist theme preference to site_theme key in localStorage");
            Assert.Contains("themeChanged", content,
                "toggleThemeMode should dispatch themeChanged custom event for reactive UI updates");
        }

        [E2ETest(5, 19, "M4_07: toggleThemeMode guards against execution on CuaHang routes")]
        public void M4_07_ToggleThemeMode_Guards_CuaHang()
        {
            var content = GetLayoutContent();
            var fnIndex = content.IndexOf("function toggleThemeMode()");
            Assert.True(fnIndex > 0, "toggleThemeMode function must exist");
            var fnBody = content.Substring(fnIndex, Math.Min(600, content.Length - fnIndex));
            Assert.Contains("/cuahang", fnBody.ToLower(),
                "toggleThemeMode function body must contain safety guard against /cuahang routes");
        }

        [E2ETest(5, 19, "M4_08: CSS variables define Western Highlands night theme in html.dark")]
        public void M4_08_CSS_Variables_Dark_Mode_Palette()
        {
            var content = GetLayoutContent();
            Assert.Contains("--bg-page: #0a140e", content, "html.dark must define --bg-page as #0a140e");
            Assert.Contains("--bg-surface: #13241b", content, "html.dark must define --bg-surface as #13241b");
            Assert.Contains("--bg-surface-soft: #0d1d14", content, "html.dark must define --bg-surface-soft as #0d1d14");
            Assert.Contains("--text-main: #f8fafc", content, "html.dark must define --text-main as #f8fafc");
            Assert.Contains("--border-card: rgba(74, 222, 128, 0.2)", content,
                "html.dark must define --border-card with forest green accent border");
        }

        [E2ETest(5, 19, "M4_09: CSS variables define default volunteer light theme")]
        public void M4_09_CSS_Variables_Light_Mode_Palette()
        {
            var content = GetLayoutContent();
            Assert.Contains("--bg-page: #ffffff", content, ":root must define --bg-page as #ffffff");
            Assert.Contains("--text-main: #143823", content, ":root must define --text-main as #143823 (Deep Volunteer Pine)");
        }

        [E2ETest(5, 19, "M4_10: Theme toggle button has .theme-toggle-btn styling and dark mode glow")]
        public void M4_10_Theme_Toggle_Button_Styling()
        {
            var content = GetLayoutContent();
            Assert.Contains(".theme-toggle-btn", content, "CSS must define .theme-toggle-btn styles");
            Assert.Contains("rgba(251, 191, 36", content,
                "CSS must define glowing effect for toggle button in dark mode with warm amber color");
        }

        [E2ETest(5, 19, "M4_11: CuaHang layout remains 100% isolated with hard-locked light mode and 0 toggles")]
        public void M4_11_CuaHang_Layout_Hard_Locked_Light_Mode()
        {
            var content = GetCuaHangLayoutContent();
            Assert.Contains("class=\"light", content, "_CuaHangLayout must enforce class='light'");
            Assert.False(content.Contains("themeToggleBtn"), "_CuaHangLayout must not contain themeToggleBtn");
            Assert.False(content.Contains("toggleThemeMode"), "_CuaHangLayout must not contain toggleThemeMode");
        }

        [E2ETest(5, 19, "M4_12: Backward compatibility alias window.toggleTheme is maintained")]
        public void M4_12_Backward_Compatibility_Alias()
        {
            var content = GetLayoutContent();
            Assert.Contains("window.toggleTheme", content, "_Layout.cshtml must provide window.toggleTheme alias");
        }
    }
}
