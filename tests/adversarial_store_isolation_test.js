/**
 * EMPIRICAL ADVERSARIAL STRESS TEST & ISOLATION VERIFICATION
 * Milestone M4 / Requirement R4: Store Area Isolation & Anti-FOUC Guard Verification
 * 
 * Verifies:
 * 1. Dark-mode active user navigates to /CuaHang/Index or /CuaHang/GioHang.
 * 2. Class 'dark' is strictly stripped and class 'light' is strictly applied.
 * 3. Toggle button is 100% absent from CuaHang storefront layout and views.
 * 4. toggleThemeMode() is blocked from mutating theme when in /cuahang routes.
 * 5. Path permutations, edge cases, and tamper resistance.
 */

const fs = require('fs');
const path = require('path');
const vm = require('vm');

let totalTests = 0;
let passedTests = 0;
let failedTests = 0;
const failureDetails = [];

function assert(condition, message, details = '') {
    totalTests++;
    if (condition) {
        passedTests++;
        console.log(`  [PASS] ${message}`);
    } else {
        failedTests++;
        const failMsg = details ? `${message} - Details: ${details}` : message;
        failureDetails.push(failMsg);
        console.log(`  [FAIL] ${failMsg}`);
    }
}

// Minimal DOM simulation environment
class MockClassList {
    constructor(initialClasses = []) {
        this.classes = new Set(initialClasses);
    }
    add(...names) {
        names.forEach(n => this.classes.add(n));
    }
    remove(...names) {
        names.forEach(n => this.classes.delete(n));
    }
    contains(name) {
        return this.classes.has(name);
    }
    toString() {
        return Array.from(this.classes).join(' ');
    }
}

class MockStorage {
    constructor(init = {}) {
        this.store = { ...init };
    }
    getItem(key) {
        return this.store[key] !== undefined ? this.store[key] : null;
    }
    setItem(key, val) {
        this.store[key] = String(val);
    }
    removeItem(key) {
        delete this.store[key];
    }
    clear() {
        this.store = {};
    }
}

function createMockBrowserEnv(pathname, storageData = {}, htmlClasses = ['dark']) {
    const classList = new MockClassList(htmlClasses);
    const storage = new MockStorage(storageData);
    const eventListeners = {};

    const mockWindow = {
        location: {
            pathname: pathname,
            href: `https://volunteer.local${pathname}`
        },
        localStorage: storage,
        document: {
            documentElement: {
                classList: classList,
                className: classList.toString()
            },
            addEventListener: (event, handler) => {
                if (!eventListeners[event]) eventListeners[event] = [];
                eventListeners[event].push(handler);
            },
            querySelectorAll: () => []
        },
        dispatchEvent: () => {},
        CustomEvent: function(name, opts) { this.name = name; this.detail = opts ? opts.detail : null; },
        console: console
    };
    mockWindow.window = mockWindow;
    mockWindow.globalThis = mockWindow;

    return {
        context: vm.createContext(mockWindow),
        classList: classList,
        storage: storage,
        eventListeners: eventListeners,
        window: mockWindow
    };
}

console.log('================================================================================');
console.log(' CHALLENGER 2: EMPIRICAL ADVERSARIAL STRESS TEST FOR R4 STORE ISOLATION');
console.log('================================================================================\n');

const repoRoot = path.resolve(__dirname, '..');
const cuaHangLayoutPath = path.join(repoRoot, 'WEB_TinhNguyen/Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml');
const mainLayoutPath = path.join(repoRoot, 'WEB_TinhNguyen/GiaoDien/Shared/_Layout.cshtml');
const cuaHangViewsDir = path.join(repoRoot, 'WEB_TinhNguyen/Areas/CuaHang/Views');

assert(fs.existsSync(cuaHangLayoutPath), `_CuaHangLayout.cshtml exists at ${cuaHangLayoutPath}`);
assert(fs.existsSync(mainLayoutPath), `_Layout.cshtml exists at ${mainLayoutPath}`);
assert(fs.existsSync(cuaHangViewsDir), `Areas/CuaHang/Views directory exists at ${cuaHangViewsDir}`);

const cuaHangLayoutContent = fs.readFileSync(cuaHangLayoutPath, 'utf8');
const mainLayoutContent = fs.readFileSync(mainLayoutPath, 'utf8');

// ================================================================================
// SUITE 1: Razor & HTML Template Static Guarding
// ================================================================================
console.log('\n--- SUITE 1: Razor & HTML Static Isolation & Button Suppression ---');

assert(cuaHangLayoutContent.includes('<html lang="vi" class="light scroll-smooth">'),
    '_CuaHangLayout hard-locks class="light scroll-smooth" on <html> element');

assert(cuaHangLayoutContent.includes('body.cuahang-body') && 
       cuaHangLayoutContent.includes('background-color: #f8fafc !important;') &&
       cuaHangLayoutContent.includes('color: #0f172a !important;'),
    '_CuaHangLayout enforces hard-locked #f8fafc background and #0f172a text color');

assert(!cuaHangLayoutContent.includes('id="themeToggleBtn"'),
    '_CuaHangLayout contains NO desktop toggle button (themeToggleBtn)');

assert(!cuaHangLayoutContent.includes('id="themeToggleBtnMobile"'),
    '_CuaHangLayout contains NO mobile toggle button (themeToggleBtnMobile)');

assert(!cuaHangLayoutContent.includes('theme-toggle-btn'),
    '_CuaHangLayout contains NO .theme-toggle-btn CSS class');

assert(!cuaHangLayoutContent.includes('toggleThemeMode'),
    '_CuaHangLayout contains NO toggleThemeMode call or definition');

assert(!cuaHangLayoutContent.includes('toggleTheme'),
    '_CuaHangLayout contains NO toggleTheme call');

// Inspect all 9 views in Areas/CuaHang/Views
const storeViewFiles = [
    'AdminDonHang/ChiTiet.cshtml',
    'AdminDonHang/Index.cshtml',
    'AdminSanPham/Create.cshtml',
    'AdminSanPham/Edit.cshtml',
    'AdminSanPham/Index.cshtml',
    'CuaHang/ChiTiet.cshtml',
    'CuaHang/Index.cshtml',
    'GioHang/DatHangThanhCong.cshtml',
    'GioHang/Index.cshtml'
];

storeViewFiles.forEach(viewFile => {
    const fullPath = path.join(cuaHangViewsDir, viewFile);
    const content = fs.readFileSync(fullPath, 'utf8');
    assert(!content.includes('themeToggleBtn'), `${viewFile} does not contain themeToggleBtn`);
    assert(!content.includes('theme-toggle-btn'), `${viewFile} does not contain theme-toggle-btn`);
    assert(!content.includes('toggleThemeMode'), `${viewFile} does not invoke toggleThemeMode()`);
});

// Main layout Razor gating
assert(mainLayoutContent.includes('ViewContext.RouteData.Values["area"]?.ToString() != "CuaHang"'),
    '_Layout.cshtml suppresses theme toggle button when Area == "CuaHang"');

// ================================================================================
// SUITE 2: Dynamic Execution Simulation of _CuaHangLayout Anti-FOUC & DOM scripts
// ================================================================================
console.log('\n--- SUITE 2: Dynamic Execution of _CuaHangLayout Anti-FOUC Scripts ---');

// Extract the head script from _CuaHangLayout
const cuaHangHeadScriptMatch = cuaHangLayoutContent.match(/<!-- CÔ LẬP CỬA HÀNG: CƯỠNG CHẾ LIGHT MODE TUYỆT ĐỐI -->\s*<script>([\s\S]*?)<\/script>/);
assert(cuaHangHeadScriptMatch !== null, 'Found head isolation script in _CuaHangLayout');
const cuaHangHeadScript = cuaHangHeadScriptMatch ? cuaHangHeadScriptMatch[1] : '';

// Adversarial test cases: user comes with dark mode active
const testPaths = [
    '/CuaHang',
    '/CuaHang/',
    '/CuaHang/Index',
    '/CuaHang/GioHang',
    '/CuaHang/ChiTiet/5',
    '/cuahang/index',
    '/cuahang/giohang',
    '/CUAHANG/INDEX',
    '/Areas/CuaHang/Views/CuaHang/Index',
    '/CuaHang/GioHang?step=checkout',
    '/CuaHang/Index#products'
];

testPaths.forEach(p => {
    // Adversarial precondition: user had dark mode on previously
    const env = createMockBrowserEnv(p, { site_theme: 'dark', theme: 'dark' }, ['dark', 'foo-class']);
    
    // Execute head IIFE
    vm.runInContext(cuaHangHeadScript, env.context);
    
    const hasDark = env.classList.contains('dark');
    const hasLight = env.classList.contains('light');
    
    assert(!hasDark && hasLight, 
        `_CuaHangLayout head script on "${p}": dark stripped=${!hasDark}, light added=${hasLight}`);
    
    // Check that user localStorage was NOT corrupted (so if they return to main site, their dark preference survives)
    assert(env.storage.getItem('site_theme') === 'dark', 
        `_CuaHangLayout head script on "${p}" preserves user preference in localStorage without corrupting it`);
});

// Test DOMContentLoaded reinforcement in _CuaHangLayout
const cuaHangDomLoadedMatch = cuaHangLayoutContent.match(/document\.addEventListener\('DOMContentLoaded',\s*function\(\)\s*\{([\s\S]*?)\}\);/);
assert(cuaHangDomLoadedMatch !== null, 'Found DOMContentLoaded listener in _CuaHangLayout');

testPaths.slice(0, 3).forEach(p => {
    // Adversarial scenario: an external script maliciously injected class 'dark' after head script ran
    const env = createMockBrowserEnv(p, { site_theme: 'dark' }, ['light']);
    env.classList.add('dark'); // malicious tamper
    
    // Run the DOMContentLoaded handler body
    const domLoadedHandler = new Function('document', cuaHangDomLoadedMatch[1]);
    domLoadedHandler(env.window.document);
    
    const hasDark = env.classList.contains('dark');
    const hasLight = env.classList.contains('light');
    assert(!hasDark && hasLight, 
        `_CuaHangLayout DOMContentLoaded recovers from tamper on "${p}": dark stripped=${!hasDark}, light enforced=${hasLight}`);
});

// ================================================================================
// SUITE 3: Dynamic Execution Simulation of _Layout Anti-FOUC & toggleThemeMode Guard
// ================================================================================
console.log('\n--- SUITE 3: Dynamic Execution of _Layout Anti-FOUC & Guard Scripts ---');

// Extract head script from _Layout
const mainHeadScriptMatch = mainLayoutContent.match(/<!-- SCRIPT ƯU TIÊN CHẠY ĐẦU TIÊN ĐỂ FIX LỖI NHÁY GIAO DIỆN \(ANTI-FOUC\) -->\s*<script>([\s\S]*?)<\/script>/);
assert(mainHeadScriptMatch !== null, 'Found Anti-FOUC script in _Layout.cshtml');
const mainHeadScript = mainHeadScriptMatch ? mainHeadScriptMatch[1] : '';

// Test main layout head script on /cuahang routes
testPaths.forEach(p => {
    const env = createMockBrowserEnv(p, { site_theme: 'dark', theme: 'dark' }, ['dark']);
    
    vm.runInContext(mainHeadScript, env.context);
    
    const hasDark = env.classList.contains('dark');
    const hasLight = env.classList.contains('light');
    
    assert(!hasDark && hasLight, 
        `_Layout Anti-FOUC script on "${p}": dark stripped=${!hasDark}, light added=${hasLight}`);
});

// Test main layout head script on normal routes (non-storefront)
const normalPaths = ['/', '/Home/Index', '/Home/ChuongTrinh', '/Home/TinTuc'];
normalPaths.forEach(p => {
    // Dark mode user
    const darkEnv = createMockBrowserEnv(p, { site_theme: 'dark' }, []);
    vm.runInContext(mainHeadScript, darkEnv.context);
    assert(darkEnv.classList.contains('dark') && !darkEnv.classList.contains('light'),
        `_Layout Anti-FOUC on "${p}" loads dark mode when site_theme='dark'`);

    // Light mode user
    const lightEnv = createMockBrowserEnv(p, { site_theme: 'light' }, ['dark']);
    vm.runInContext(mainHeadScript, lightEnv.context);
    assert(!lightEnv.classList.contains('dark') && lightEnv.classList.contains('light'),
        `_Layout Anti-FOUC on "${p}" loads light mode when site_theme='light'`);
        
    // First-time visitor (no stored theme) -> defaults to light mode
    const firstEnv = createMockBrowserEnv(p, {}, ['dark']);
    vm.runInContext(mainHeadScript, firstEnv.context);
    assert(!firstEnv.classList.contains('dark') && firstEnv.classList.contains('light'),
        `_Layout Anti-FOUC on "${p}" defaults to light mode for new visitors`);
    assert(firstEnv.storage.getItem('site_theme') === 'light',
        `_Layout Anti-FOUC sets site_theme='light' in localStorage for new visitors`);
});

// ================================================================================
// SUITE 4: Adversarial Attack on toggleThemeMode() inside CuaHang Area
// ================================================================================
console.log('\n--- SUITE 4: Adversarial Invocations of toggleThemeMode() on /cuahang ---');

// Extract toggleThemeMode definition
const toggleFnMatch = mainLayoutContent.match(/function toggleThemeMode\(\)\s*\{([\s\S]*?)\n\s*\}\s*\n\s*\/\/ Tương thích ngược/);
assert(toggleFnMatch !== null, 'Found toggleThemeMode function definition in _Layout.cshtml');

const toggleFullCode = `
    function toggleThemeMode() {
        ${toggleFnMatch ? toggleFnMatch[1] : ''}
    }
    function syncInlineColorsForTheme() {}
`;

// Attempt to invoke toggleThemeMode while browsing CuaHang
testPaths.forEach(p => {
    const env = createMockBrowserEnv(p, { site_theme: 'light' }, ['light']);
    vm.runInContext(toggleFullCode, env.context);
    
    // Call toggleThemeMode()
    vm.runInContext('toggleThemeMode()', env.context);
    
    // It must NOT have switched to dark!
    const hasDark = env.classList.contains('dark');
    const hasLight = env.classList.contains('light');
    const storedTheme = env.storage.getItem('site_theme');
    
    assert(!hasDark && hasLight && storedTheme === 'light',
        `Hostile toggleThemeMode() call blocked on "${p}": dark rejected=${!hasDark}, light maintained=${hasLight}`);
});

// Normal invocation on /Home/Index switches theme correctly
{
    const env = createMockBrowserEnv('/Home/Index', { site_theme: 'light' }, ['light']);
    vm.runInContext(toggleFullCode, env.context);
    vm.runInContext('toggleThemeMode()', env.context);
    
    assert(env.classList.contains('dark') && !env.classList.contains('light'),
        'Normal toggleThemeMode() on /Home/Index switches light -> dark');
    assert(env.storage.getItem('site_theme') === 'dark',
        'Normal toggleThemeMode() saves site_theme = "dark"');
        
    // Toggle again -> dark to light
    vm.runInContext('toggleThemeMode()', env.context);
    assert(!env.classList.contains('dark') && env.classList.contains('light'),
        'Normal toggleThemeMode() on /Home/Index switches dark -> light');
    assert(env.storage.getItem('site_theme') === 'light',
        'Normal toggleThemeMode() saves site_theme = "light"');
}

// ================================================================================
// SUMMARY
// ================================================================================
console.log('\n================================================================================');
console.log(` RESULTS: ${passedTests} PASSED | ${failedTests} FAILED | TOTAL: ${totalTests}`);
console.log('================================================================================\n');

if (failedTests > 0) {
    console.error('FAILURES DETECTED:');
    failureDetails.forEach(f => console.error(` - ${f}`));
    process.exit(1);
} else {
    console.log('ALL ADVERSARIAL STRESS TESTS PASSED WITH 100% SUCCESS.');
    process.exit(0);
}
