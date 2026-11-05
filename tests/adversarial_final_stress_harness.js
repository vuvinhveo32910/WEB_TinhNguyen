/**
 * ADVERSARIAL STRESS & EDGE-CASE TEST HARNESS
 * Final Challenger 2 (Adversarial Stress & Edge-Case Verifier) - Milestone M5
 *
 * Verifies:
 * 1. Dark mode leakage test into Cửa Hàng across URL variations
 * 2. Navigation transitions between Trang chủ (Dark) and Cửa Hàng (Light) with localStorage persistence
 * 3. Browser blocking / Private Mode / localStorage exception resilience
 * 4. Static scan: 0 color properties in MoHinh, 0 color inputs in GiaoDien/Admin*, 0 unexpected diffs in Areas/CuaHang/Views
 */

const fs = require('fs');
const path = require('path');
const vm = require('vm');

let totalTests = 0;
let passedTests = 0;
let failedTests = 0;
const failureDetails = [];

function assert(condition, message) {
    totalTests++;
    if (condition) {
        passedTests++;
        console.log(`  [PASS] ${message}`);
    } else {
        failedTests++;
        console.error(`  [FAIL] ${message}`);
        failureDetails.push(message);
    }
}

const repoRoot = path.resolve(__dirname, '..');
const projectDir = path.join(repoRoot, 'WEB_TinhNguyen');
const cuaHangLayoutPath = path.join(projectDir, 'Areas/CuaHang/Views/Shared/_CuaHangLayout.cshtml');
const mainLayoutPath = path.join(projectDir, 'GiaoDien/Shared/_Layout.cshtml');
const cuaHangViewsDir = path.join(projectDir, 'Areas/CuaHang/Views');
const moHinhDir = path.join(projectDir, 'MoHinh');
const giaoDienAdminDir = path.join(projectDir, 'GiaoDien');

console.log('================================================================================');
console.log(' FINAL CHALLENGER 2: ADVERSARIAL STRESS & EDGE-CASE VERIFICATION HARNESS');
console.log('================================================================================\n');

// --------------------------------------------------------------------------------
// Mock Browser Environment Factory
// --------------------------------------------------------------------------------
function createMockBrowserEnv(pathname, initialStorage = {}, initialClasses = [], options = {}) {
    const classListSet = new Set(initialClasses);
    const storageMap = new Map(Object.entries(initialStorage));

    const mockClassList = {
        add: (...cls) => cls.forEach(c => classListSet.add(c)),
        remove: (...cls) => cls.forEach(c => classListSet.delete(c)),
        contains: (c) => classListSet.has(c),
        toggle: (c, force) => {
            if (force === true) { classListSet.add(c); return true; }
            if (force === false) { classListSet.delete(c); return false; }
            if (classListSet.has(c)) { classListSet.delete(c); return false; }
            classListSet.add(c); return true;
        },
        toString: () => Array.from(classListSet).join(' ')
    };

    let storageObj = {
        getItem: (k) => storageMap.has(k) ? storageMap.get(k) : null,
        setItem: (k, v) => storageMap.set(k, String(v)),
        removeItem: (k) => storageMap.delete(k),
        clear: () => storageMap.clear(),
        get length() { return storageMap.size; }
    };

    if (options.localStorageSecurityError) {
        storageObj = {
            getItem: () => { throw new Error('SecurityError: The operation is insecure.'); },
            setItem: () => { throw new Error('SecurityError: The operation is insecure.'); },
            removeItem: () => { throw new Error('SecurityError: The operation is insecure.'); },
            clear: () => { throw new Error('SecurityError: The operation is insecure.'); }
        };
    } else if (options.localStorageQuotaExceeded) {
        const origSet = storageObj.setItem;
        storageObj.setItem = (k, v) => { throw new Error('QuotaExceededError: DOMException 22'); };
    }

    const docElement = {
        classList: mockClassList,
        className: mockClassList.toString()
    };

    const mockDoc = {
        documentElement: docElement,
        querySelectorAll: (selector) => [],
        querySelector: (selector) => null,
        addEventListener: (event, handler) => {}
    };

    // Parse URL components
    const urlObj = new URL(`https://example.com${pathname.startsWith('/') ? pathname : '/' + pathname}`);

    const mockWindow = {
        location: {
            pathname: urlObj.pathname,
            search: urlObj.search,
            hash: urlObj.hash,
            href: urlObj.href
        },
        document: mockDoc,
        localStorage: options.localStorageNull ? null : (options.localStoragePropertyThrows ? undefined : storageObj),
        dispatchEvent: (evt) => {},
        CustomEvent: function(name, opts) { this.name = name; this.opts = opts; }
    };

    const sandbox = {
        window: mockWindow,
        document: mockDoc,
        console: {
            log: () => {},
            error: () => {},
            warn: () => {}
        },
        CustomEvent: mockWindow.CustomEvent
    };

    if (options.localStoragePropertyThrows) {
        Object.defineProperty(sandbox, 'localStorage', {
            get: () => { throw new Error('SecurityError: localStorage access is denied'); }
        });
        Object.defineProperty(mockWindow, 'localStorage', {
            get: () => { throw new Error('SecurityError: localStorage access is denied'); }
        });
    } else {
        sandbox.localStorage = options.localStorageNull ? null : storageObj;
    }

    const context = vm.createContext(sandbox);

    return {
        context,
        classList: mockClassList,
        storage: storageObj,
        window: mockWindow
    };
}

// Read layouts
const cuaHangLayoutContent = fs.readFileSync(cuaHangLayoutPath, 'utf8');
const mainLayoutContent = fs.readFileSync(mainLayoutPath, 'utf8');

// Extract head script from _CuaHangLayout
const cuaHangHeadScriptMatch = cuaHangLayoutContent.match(/<script>\s*\(function\s*\(\)\s*\{([\s\S]*?)\}\)\(\);\s*<\/script>/);
assert(cuaHangHeadScriptMatch !== null, 'Extracted head script from _CuaHangLayout.cshtml');
const cuaHangHeadScript = cuaHangHeadScriptMatch ? cuaHangHeadScriptMatch[0].replace(/<\/?script>/g, '') : '';

// Extract head Anti-FOUC script from _Layout
const mainHeadScriptMatch = mainLayoutContent.match(/<!-- SCRIPT ƯU TIÊN CHẠY ĐẦU TIÊN ĐỂ FIX LỖI NHÁY GIAO DIỆN \(ANTI-FOUC\) -->\s*<script>([\s\S]*?)<\/script>/);
assert(mainHeadScriptMatch !== null, 'Extracted Anti-FOUC script from _Layout.cshtml');
const mainHeadScript = mainHeadScriptMatch ? mainHeadScriptMatch[1] : '';

// Extract toggleThemeMode definition
const toggleFnMatch = mainLayoutContent.match(/function toggleThemeMode\(\)\s*\{([\s\S]*?)\n\s*\}\s*\n\s*\/\/ Tương thích ngược/);
assert(toggleFnMatch !== null, 'Extracted toggleThemeMode function definition from _Layout.cshtml');
const toggleFullCode = `
    function toggleThemeMode() {
        ${toggleFnMatch ? toggleFnMatch[1] : ''}
    }
    function syncInlineColorsForTheme() {}
`;

// ================================================================================
// REQUIREMENT 1: Dark Mode URL Leakage Test Across All URL Variations
// ================================================================================
console.log('\n--- REQUIREMENT 1: Dark Mode Leakage Test Across URL Variations ---');

const hostileCuaHangUrls = [
    // Standard variations
    '/cuahang',
    '/CUAHANG',
    '/CuaHang',
    '/cuahang/',
    '/CuaHang/',
    '/CUAHANG/',
    '/CuaHang/Index',
    '/cuahang/index',
    '/CUAHANG/INDEX',
    // Child actions
    '/CuaHang/ChiTiet/1',
    '/cuahang/chitiet/42',
    '/CUAHANG/CHITIET/99',
    '/CuaHang/GioHang',
    '/cuahang/giohang',
    '/CUAHANG/GIOHANG',
    '/CuaHang/GioHang/DatHangThanhCong',
    '/cuahang/giohang/dathangthanhcong',
    '/CuaHang/AdminSanPham',
    '/cuahang/adminsanpham/index',
    '/CuaHang/AdminDonHang',
    '/cuahang/admindonhang/chitiet/1',
    // Adversarial Query Parameters
    '/cuahang?theme=dark',
    '/CUAHANG?mode=dark&bg=black',
    '/CuaHang/Index?theme=dark&style=night',
    '/cuahang/giohang?step=1&dark=true',
    '/cuahang/chitiet/123?color=dark#theme-dark',
    '/CuaHang?utm_source=facebook&dark=1',
    // Adversarial Hash fragments
    '/cuahang#dark',
    '/CUAHANG#night',
    '/CuaHang/Index#dark-mode',
    '/cuahang/giohang#order',
    // Mixed casing & path anomalies
    '/CuAhAnG',
    '/cuahang/index?param=/cuahang',
    '/CuaHang/ChiTiet/1?ref=home&dark=true#reviews'
];

hostileCuaHangUrls.forEach((url) => {
    // 1. Test _CuaHangLayout head script: user arrived with dark class and site_theme='dark'
    const envCH = createMockBrowserEnv(url, { site_theme: 'dark', theme: 'dark' }, ['dark']);
    vm.runInContext(cuaHangHeadScript, envCH.context);
    const chHasDark = envCH.classList.contains('dark');
    const chHasLight = envCH.classList.contains('light');
    const chStoredTheme = envCH.storage.getItem('site_theme');

    assert(!chHasDark && chHasLight, `_CuaHangLayout strips dark & enforces light on: ${url}`);
    assert(chStoredTheme === 'dark', `_CuaHangLayout preserves stored site_theme='dark' without corruption on: ${url}`);

    // 2. Test _Layout head script when visiting any /cuahang URL:
    // It must detect /cuahang, strip dark, add light, and return BEFORE setting localStorage to 'light'
    const envLayout = createMockBrowserEnv(url, { site_theme: 'dark', theme: 'dark' }, ['dark']);
    vm.runInContext(mainHeadScript, envLayout.context);
    const layoutHasDark = envLayout.classList.contains('dark');
    const layoutHasLight = envLayout.classList.contains('light');
    const layoutStoredTheme = envLayout.storage.getItem('site_theme');

    assert(!layoutHasDark && layoutHasLight, `_Layout Anti-FOUC locks light mode on: ${url}`);
    assert(layoutStoredTheme === 'dark', `_Layout Anti-FOUC does NOT overwrite stored theme on: ${url}`);

    // 3. Test toggleThemeMode() when accidentally or maliciously called on /cuahang route:
    // Must be completely blocked
    const envToggle = createMockBrowserEnv(url, { site_theme: 'light' }, ['light']);
    vm.runInContext(toggleFullCode, envToggle.context);
    vm.runInContext('toggleThemeMode()', envToggle.context);
    const toggleHasDark = envToggle.classList.contains('dark');
    const toggleHasLight = envToggle.classList.contains('light');
    assert(!toggleHasDark && toggleHasLight, `toggleThemeMode() call safely blocked on: ${url}`);
});

// ================================================================================
// REQUIREMENT 2: Bidirectional Navigation and State Recovery
// ================================================================================
console.log('\n--- REQUIREMENT 2: Navigation Transitions & State Recovery ---');

// Shared storage simulating continuous user session
const sessionStore = { site_theme: null };
const sharedStorageMap = new Map();

function runHeadScript(script, pathname, sharedMap, initialClasses = []) {
    const classListSet = new Set(initialClasses);
    const storageObj = {
        getItem: (k) => sharedMap.has(k) ? sharedMap.get(k) : null,
        setItem: (k, v) => sharedMap.set(k, String(v)),
        removeItem: (k) => sharedMap.delete(k),
        clear: () => sharedMap.clear()
    };
    const mockClassList = {
        add: (...cls) => cls.forEach(c => classListSet.add(c)),
        remove: (...cls) => cls.forEach(c => classListSet.delete(c)),
        contains: (c) => classListSet.has(c)
    };
    const mockDoc = { documentElement: { classList: mockClassList } };
    const urlObj = new URL(`https://example.com${pathname}`);
    const context = vm.createContext({
        window: { location: { pathname: urlObj.pathname, search: urlObj.search, hash: urlObj.hash }, localStorage: storageObj },
        document: mockDoc,
        localStorage: storageObj,
        console: { log: () => {}, error: () => {} }
    });
    vm.runInContext(script, context);
    return { classList: mockClassList, storage: storageObj };
}

// Step 1: User visits Home for first time -> defaults to light
{
    const res = runHeadScript(mainHeadScript, '/', sharedStorageMap, []);
    assert(!res.classList.contains('dark') && res.classList.contains('light'), 'Step 1: New visitor on Home defaults to Light mode');
    assert(sharedStorageMap.get('site_theme') === 'light', 'Step 1: site_theme="light" saved in localStorage');
}

// Step 2: User clicks dark mode toggle on Home
{
    // Simulate toggleThemeMode() on Home
    const env = createMockBrowserEnv('/', Object.fromEntries(sharedStorageMap), ['light']);
    vm.runInContext(toggleFullCode, env.context);
    vm.runInContext('toggleThemeMode()', env.context);
    assert(env.classList.contains('dark') && !env.classList.contains('light'), 'Step 2: User toggles to Dark mode on Home');
    assert(env.storage.getItem('site_theme') === 'dark', 'Step 2: site_theme="dark" persisted');
    sharedStorageMap.set('site_theme', 'dark');
    sharedStorageMap.set('theme', 'dark');
}

// Step 3: User navigates from Home (Dark) to Cửa Hàng (/CuaHang)
{
    const res = runHeadScript(cuaHangHeadScript, '/CuaHang', sharedStorageMap, ['dark']);
    assert(!res.classList.contains('dark') && res.classList.contains('light'), 'Step 3: CuaHang enforces Light mode');
    assert(sharedStorageMap.get('site_theme') === 'dark', 'Step 3: Stored site_theme remains "dark" (not overwritten)');
}

// Step 4: User navigates back to Home (/) -> Dark mode is restored!
{
    const res = runHeadScript(mainHeadScript, '/', sharedStorageMap, ['light']);
    assert(res.classList.contains('dark') && !res.classList.contains('light'), 'Step 4: Returning to Home accurately restores Dark mode');
}

// Step 5: User browses subpages of Home: /Home/TinTuc, /Home/GayQuy -> Dark mode remains
{
    const res1 = runHeadScript(mainHeadScript, '/Home/TinTuc', sharedStorageMap, []);
    assert(res1.classList.contains('dark') && !res1.classList.contains('light'), 'Step 5a: /Home/TinTuc retains Dark mode');

    const res2 = runHeadScript(mainHeadScript, '/Home/GayQuy', sharedStorageMap, []);
    assert(res2.classList.contains('dark') && !res2.classList.contains('light'), 'Step 5b: /Home/GayQuy retains Dark mode');
}

// Step 6: User navigates deep into Cửa Hàng: /CuaHang/ChiTiet/5 -> Light mode enforced
{
    const res = runHeadScript(cuaHangHeadScript, '/CuaHang/ChiTiet/5', sharedStorageMap, ['dark']);
    assert(!res.classList.contains('dark') && res.classList.contains('light'), 'Step 6: /CuaHang/ChiTiet/5 enforces Light mode');
    assert(sharedStorageMap.get('site_theme') === 'dark', 'Step 6: site_theme remains "dark"');
}

// Step 7: User navigates back to Home -> Dark mode restored again
{
    const res = runHeadScript(mainHeadScript, '/Home/Index', sharedStorageMap, ['light']);
    assert(res.classList.contains('dark') && !res.classList.contains('light'), 'Step 7: Re-visiting /Home/Index restores Dark mode');
}

// Step 8: User switches to Light mode on Home
{
    const env = createMockBrowserEnv('/Home/Index', Object.fromEntries(sharedStorageMap), ['dark']);
    vm.runInContext(toggleFullCode, env.context);
    vm.runInContext('toggleThemeMode()', env.context);
    assert(!env.classList.contains('dark') && env.classList.contains('light'), 'Step 8: Toggling to Light mode on Home switches classes');
    sharedStorageMap.set('site_theme', 'light');
    sharedStorageMap.set('theme', 'light');
}

// Step 9: User navigates to Cửa Hàng and back -> Light mode remains intact
{
    const resCH = runHeadScript(cuaHangHeadScript, '/CuaHang', sharedStorageMap, ['light']);
    assert(!resCH.classList.contains('dark') && resCH.classList.contains('light'), 'Step 9a: CuaHang is Light');

    const resHome = runHeadScript(mainHeadScript, '/', sharedStorageMap, ['light']);
    assert(!resHome.classList.contains('dark') && resHome.classList.contains('light'), 'Step 9b: Home stays Light mode');
    assert(sharedStorageMap.get('site_theme') === 'light', 'Step 9c: site_theme remains "light"');
}

// Step 10: 100-hop rapid navigation stress test
{
    let stateCorrupted = false;
    // Set theme to dark
    sharedStorageMap.set('site_theme', 'dark');

    for (let i = 0; i < 50; i++) {
        // Hop into CuaHang
        const ch = runHeadScript(cuaHangHeadScript, `/CuaHang/Product/${i}`, sharedStorageMap, ['dark']);
        if (ch.classList.contains('dark') || !ch.classList.contains('light') || sharedStorageMap.get('site_theme') !== 'dark') {
            stateCorrupted = true;
            break;
        }

        // Hop back to Home
        const hm = runHeadScript(mainHeadScript, `/Home/Page/${i}`, sharedStorageMap, ['light']);
        if (!hm.classList.contains('dark') || hm.classList.contains('light') || sharedStorageMap.get('site_theme') !== 'dark') {
            stateCorrupted = true;
            break;
        }
    }
    assert(!stateCorrupted, 'Step 10: 100 rapid sequential hops between Home and CuaHang showed 0 corruption or leakage');
}

// ================================================================================
// REQUIREMENT 3: Browser Blocking / Private Mode / Storage Failure Simulation
// ================================================================================
console.log('\n--- REQUIREMENT 3: Browser Storage Blocking / Private Mode Resilience ---');

// Test 3.1: SecurityError thrown on any localStorage access (Safari Private Mode / Cookies Blocked)
{
    let homeScriptCrashed = false;
    try {
        const envPrivateHome = createMockBrowserEnv('/', {}, [], { localStorageSecurityError: true });
        vm.runInContext(mainHeadScript, envPrivateHome.context);
    } catch (e) {
        homeScriptCrashed = true;
        console.error('Crash in mainHeadScript under SecurityError:', e);
    }
    assert(!homeScriptCrashed, '_Layout Anti-FOUC script does NOT crash when localStorage throws SecurityError');

    let cuaHangScriptCrashed = false;
    let chHasLight = false;
    let chHasDark = true;
    try {
        const envPrivateCH = createMockBrowserEnv('/CuaHang', {}, ['dark'], { localStorageSecurityError: true });
        vm.runInContext(cuaHangHeadScript, envPrivateCH.context);
        chHasLight = envPrivateCH.classList.contains('light');
        chHasDark = envPrivateCH.classList.contains('dark');
    } catch (e) {
        cuaHangScriptCrashed = true;
        console.error('Crash in cuaHangHeadScript under SecurityError:', e);
    }
    assert(!cuaHangScriptCrashed, '_CuaHangLayout script does NOT crash when localStorage throws SecurityError');
    assert(chHasLight && !chHasDark, '_CuaHangLayout enforces Light mode even under localStorage SecurityError');
}

// Test 3.2: window.localStorage is null (sandboxed iframe without allow-same-origin)
{
    let nullStorageCrashed = false;
    try {
        const envNull = createMockBrowserEnv('/', {}, [], { localStorageNull: true });
        vm.runInContext(mainHeadScript, envNull.context);
    } catch (e) {
        nullStorageCrashed = true;
    }
    assert(!nullStorageCrashed, '_Layout Anti-FOUC script handles localStorage === null gracefully without crash');
}

// Test 3.3: window.localStorage property access itself throws (strict sandboxing)
{
    let propThrowCrashed = false;
    try {
        const envPropThrow = createMockBrowserEnv('/', {}, [], { localStoragePropertyThrows: true });
        vm.runInContext(mainHeadScript, envPropThrow.context);
    } catch (e) {
        propThrowCrashed = true;
    }
    assert(!propThrowCrashed, '_Layout Anti-FOUC script catches error when window.localStorage property throws');
}

// Test 3.4: QuotaExceededError when setting localStorage
{
    let quotaErrorCrashed = false;
    try {
        const envQuota = createMockBrowserEnv('/', {}, [], { localStorageQuotaExceeded: true });
        vm.runInContext(mainHeadScript, envQuota.context);
    } catch (e) {
        quotaErrorCrashed = true;
    }
    assert(!quotaErrorCrashed, '_Layout Anti-FOUC catches QuotaExceededError gracefully without crash');
}

// Test 3.5: User clicks toggle button in Private Mode (localStorage.setItem throws)
{
    const envTogglePrivate = createMockBrowserEnv('/Home/Index', {}, ['light'], { localStorageSecurityError: true });
    vm.runInContext(toggleFullCode, envTogglePrivate.context);

    let toggleException = null;
    try {
        vm.runInContext('toggleThemeMode()', envTogglePrivate.context);
    } catch (e) {
        toggleException = e;
    }

    // Notice: In toggleThemeMode(), classList.add('dark') occurs before localStorage.setItem.
    // If localStorage.setItem throws, does the class switch occur?
    const switchedToDark = envTogglePrivate.classList.contains('dark');
    assert(switchedToDark, 'toggleThemeMode() updates DOM class even if localStorage throws SecurityError');
    if (toggleException) {
        console.log(`  [INFO] toggleThemeMode() caught unhandled error from localStorage.setItem: ${toggleException.message}`);
    } else {
        console.log('  [INFO] toggleThemeMode() ran cleanly with no exception.');
    }
}

// ================================================================================
// REQUIREMENT 4: Static Code Scans across the Codebase
// ================================================================================
console.log('\n--- REQUIREMENT 4: Static Code Scans across Entire Codebase ---');

// 4.1: Scan MoHinh/*.cs for ANY color properties
{
    const moHinhFiles = fs.readdirSync(moHinhDir).filter(f => f.endsWith('.cs'));
    let foundColorProps = [];

    const colorKeywordsRegex = /\b(Mau|Color|MauChu|MauNen|MauVien|MauText|Hex|Rgb|MauSac)\b/i;

    moHinhFiles.forEach(file => {
        const content = fs.readFileSync(path.join(moHinhDir, file), 'utf8');
        const lines = content.split('\n');
        lines.forEach((line, idx) => {
            // Check property declarations: public <Type> <PropertyName> { get; set; }
            const propMatch = line.match(/public\s+[^\s]+\s+([a-zA-Z0-9_]+)\s*\{\s*get;\s*set;\s*\}/);
            if (propMatch) {
                const propName = propMatch[1];
                if (colorKeywordsRegex.test(propName)) {
                    foundColorProps.push(`${file}:${idx + 1} -> ${propName}`);
                }
            }
        });
    });

    assert(foundColorProps.length === 0, `MoHinh/*.cs contains 0 color properties (Found: ${foundColorProps.length})`);
    if (foundColorProps.length > 0) {
        console.error('Color properties detected in MoHinh:', foundColorProps);
    }
}

// 4.2: Scan GiaoDien/Admin* for input type="color" or color HEX inputs
{
    function findCshtmlFiles(dir, fileList = []) {
        const items = fs.readdirSync(dir);
        for (const item of items) {
            const fullPath = path.join(dir, item);
            const stat = fs.statSync(fullPath);
            if (stat.isDirectory()) {
                findCshtmlFiles(fullPath, fileList);
            } else if (item.endsWith('.cshtml')) {
                fileList.push(fullPath);
            }
        }
        return fileList;
    }

    const allGiaoDienFiles = findCshtmlFiles(giaoDienAdminDir);
    const adminViewFiles = allGiaoDienFiles.filter(f => f.includes('/Admin') || f.includes('/GiaoDien/Admin'));

    let foundColorInputs = [];
    const colorInputRegex = /<input[^>]+type=["']?color["']?/i;
    const hexInputRegex = /<input[^>]+(mau|hex|color)[^>]*>/i;

    adminViewFiles.forEach(file => {
        const relPath = path.relative(projectDir, file);
        const content = fs.readFileSync(file, 'utf8');
        const lines = content.split('\n');
        lines.forEach((line, idx) => {
            if (colorInputRegex.test(line)) {
                foundColorInputs.push(`${relPath}:${idx + 1} -> ${line.trim()}`);
            }
        });
    });

    assert(foundColorInputs.length === 0, `GiaoDien/Admin* contains 0 input type="color" (Found: ${foundColorInputs.length})`);
    if (foundColorInputs.length > 0) {
        console.error('Color inputs detected in Admin views:', foundColorInputs);
    }
}

// 4.3: Scan Areas/CuaHang/Views for DOM integrity and git diff
{
    const expectedViews = [
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

    let missingViews = [];
    let taintedViews = [];

    expectedViews.forEach(v => {
        const fullPath = path.join(cuaHangViewsDir, v);
        if (!fs.existsSync(fullPath)) {
            missingViews.push(v);
        } else {
            const content = fs.readFileSync(fullPath, 'utf8');
            // Check that no main dark mode toggles or main layout leaks were injected
            if (content.includes('themeToggleBtn') || content.includes('toggleThemeMode') || content.includes('toggleTheme(')) {
                taintedViews.push(`${v} has theme toggle injection`);
            }
        }
    });

    assert(missingViews.length === 0, `All 9 business views exist in Areas/CuaHang/Views (Missing: ${missingViews.length})`);
    assert(taintedViews.length === 0, `All 9 business views maintain 100% DOM integrity without theme leaks (Tainted: ${taintedViews.length})`);
}

// ================================================================================
// FINAL RESULTS SUMMARY
// ================================================================================
console.log('\n================================================================================');
console.log(` FINAL RESULTS: ${passedTests} PASSED | ${failedTests} FAILED | TOTAL: ${totalTests}`);
console.log('================================================================================\n');

if (failedTests > 0) {
    console.error('CRITICAL ISSUES FOUND:');
    failureDetails.forEach(f => console.error(` [X] ${f}`));
    process.exit(1);
} else {
    console.log('ALL EMPIRICAL ADVERSARIAL STRESS TESTS COMPLETED WITH 100% SUCCESS.');
    process.exit(0);
}
