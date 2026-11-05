# Handoff Report: Admin Product Views Design (M2 Explorer 2)

- **Agent**: `teamwork_explorer_m2_2`
- **Milestone**: M2 (Admin Product Management)
- **Recipient**: Project Orchestrator (`108b9d81-1dd6-495f-84eb-99dbb197aebe`)
- **Status**: Completed

---

## 1. Observation

1. **_AdminLayout.cshtml Structure & Styles**:
   - Location: `WEB_TinhNguyen/WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`
   - Styles observed:
     - Tailwind CSS CDN loaded at line 9: `<script src="https://cdn.tailwindcss.com"></script>`.
     - FontAwesome 6.0 loaded at line 11: `<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">`.
     - Google Font *Be Vietnam Pro* loaded at line 13: `<link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700&display=swap" rel="stylesheet">`.
     - Body background: `#f4f7fe` (line 18).
     - Sidebar container: background `#161a2d` (dark navy, line 27), hover expandable width from 85px to 280px (line 42).
     - Accent highlights: Emerald `#4caf50` / `linear-gradient(135deg, #4caf50, #1b5e20)` (lines 67, 205, 211).
     - RenderBody inside main container: line 336 `<main class="flex-1 overflow-y-auto p-4 md:p-8 pt-4">@RenderBody()</main>`.
     - Scripts section: line 340 `@RenderSection("Scripts", required: false)`.

2. **Area Layout Routing in _ViewStart.cshtml**:
   - Location: `WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/_ViewStart.cshtml`
   - Lines 7-11:
     ```csharp
     if (controller.StartsWith("Admin", StringComparison.OrdinalIgnoreCase) ||
         viewPath.Contains("/Admin", StringComparison.OrdinalIgnoreCase))
     {
         Layout = "~/Views/Shared/_AdminLayout.cshtml";
     }
     ```
   - All Admin controllers and views within `Areas/CuaHang` automatically resolve to `_AdminLayout.cshtml`.

3. **TagHelpers & Namespaces in _ViewImports.cshtml**:
   - Location: `WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/_ViewImports.cshtml`
   - Imports:
     - `@using WEB_TinhNguyen`
     - `@using WEB_TinhNguyen.Models`
     - `@using WEB_TinhNguyen.Areas.CuaHang.Models`
     - `@using WEB_TinhNguyen.Areas.CuaHang.Helpers`
     - `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`

4. **Entity Model Properties (SanPhamGayQuy.cs)**:
   - Location: `WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamGayQuy.cs`
   - Keys and fields:
     - `int IDSanPham` (Identity PK)
     - `string TenSanPham` (Required, Max 255)
     - `decimal GiaBan` (Required, Range 0..1,000,000,000)
     - `decimal? GiaGoc` (Range 0..1,000,000,000)
     - `string? LinkHinhAnh` (Max 1000)
     - `string? MoTaNgan` (Max 500)
     - `string? MoTaChiTiet`
     - `string? ThongDiepGayQuy` (Max 500)
     - `int PhanTramDongGop` (Range 1..100, default 100)
     - `int SoLuongTon` (Required, Range 0..1,000,000, default 100)
     - `int DaBan` (default 0)
     - `string DanhMuc` (default "Quà lưu niệm")
     - `bool TrangThai` (default true)
     - `int ThuTu` (default 0)
     - `DateTime NgayTao`
     - `DateTime? NgayCapNhat`
     - Computed helpers: `bool ConHang => SoLuongTon > 0;`, `decimal TienDongGopTrenMoiSp => GiaBan * PhanTramDongGop / 100m;`.

5. **Form ViewModel Properties & Methods (SanPhamCrudViewModel.cs)**:
   - Location: `WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`
   - Properties:
     - `int IDSanPham`
     - `string TenSanPham` (Required)
     - `decimal GiaBan` (Required)
     - `decimal? GiaGoc`
     - `string? LinkHinhAnh`
     - `IFormFile? FileHinhAnh` (File upload input)
     - `string? MoTaNgan`
     - `string? MoTaChiTiet`
     - `string? ThongDiepGayQuy`
     - `int PhanTramDongGop` (default 100)
     - `int SoLuongTon` (default 50)
     - `string DanhMuc` (default "Quà lưu niệm")
     - `bool TrangThai` (default true)
     - `int ThuTu` (default 0)
     - `int? IDChienDich`
   - Static mapping helper: `FromEntity(SanPhamGayQuy entity)`.
   - Instance mapping helpers: `ToEntity(string? uploadedImagePath = null)`, `UpdateEntity(SanPhamGayQuy entity, string? uploadedImagePath = null)`.

6. **Controller Interface Contract from Explorer 1 Dispatch**:
   - `Index()`: Returns `List<SanPhamGayQuy>`.
   - `Create()` (GET): Returns `SanPhamCrudViewModel`.
   - `Create()` (POST): Accepts `SanPhamCrudViewModel`, multipart/form-data.
   - `Edit(int id)` (GET): Returns `SanPhamCrudViewModel`.
   - `Edit(int id, SanPhamCrudViewModel model)` (POST): Accepts `SanPhamCrudViewModel`, multipart/form-data.
   - `Delete(int id)` (POST): Deletes product and redirects to `Index`.

7. **Compilation Status**:
   - Executed: `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`.
   - Result: `166 Warning(s), 0 Error(s)`, exited with code 0.

---

## 2. Logic Chain

1. **Design Harmony**:
   - Existing admin views (`AdminTinTuc`, `AdminChuongTrinh`) use card containers with `bg-white rounded-2xl shadow-sm border border-gray-200`, emerald action badges (`bg-emerald-50 text-emerald-700`), and clean table rows.
   - The new admin product views (`Index`, `Create`, `Edit`) adopt identical visual hierarchy: `#161a2d` dark navy sidebar integration, emerald gradients for primary action buttons, Be Vietnam Pro typography, and soft hover transitions.

2. **Index View Logic**:
   - Provides an executive KPI dashboard at the top (Total Products, Active/Selling, Total Sold/Donated, Out of Stock/Low Stock warnings).
   - Shows a clean responsive data table with all 8 requested columns:
     1. ID `#`
     2. Thumbnail image (with automatic SVG fallback on error)
     3. Product Name & Category pill
     4. Selling price (formatted with currency) & strikethrough original price if present
     5. Stock count badge (Color-coded: Red for 0, Amber for <=10, Green for healthy) & Total Sold counter
     6. Fundraising percentage badge (e.g. `❤️ 100% Quỹ`) & short message
     7. Status badge (Active with animated pulse dot / Inactive gray)
     8. Action buttons (Preview on Storefront, Edit link, Delete POST form with JS confirm)
   - Supports client-side instant search filter by product name or category without requiring server round-trips.
   - Displays friendly empty state when `Model == null || !Model.Any()`.
   - Renders `TempData["Success"]` and `TempData["Error"]` dismissible alert banners.

3. **Create & Edit Views Logic**:
   - Form configuration uses `enctype="multipart/form-data"` and POST method with Anti-Forgery tokens.
   - Two-column responsive layout (8 cols for core content, 4 cols for pricing, inventory, image, status):
     - Left column:
       - Basic Information (Tên sản phẩm, Danh mục with `<datalist>` auto-suggestions, Thứ tự, Mô tả ngắn, Mô tả chi tiết).
       - Fundraising & Volunteer Significance (Synchronized dual Range Slider & Number Input for `PhanTramDongGop`, Thông điệp gây quỹ).
     - Right column:
       - Pricing & Stock (GiaBan, GiaGoc, SoLuongTon).
       - Image Management: Dual inputs (`FileHinhAnh` for direct file upload from PC/phone + `LinkHinhAnh` for external URL or local asset path).
       - Live Image Preview Container:
         - In `Create.cshtml`: Defaults to placeholder, previews selected file instantly via HTML5 `FileReader` or typed URL.
         - In `Edit.cshtml`: Previews current existing image (`currentImage`), switches preview dynamically when a new file or URL is chosen.
       - Status toggle switch: Checkbox for `TrangThai`.
       - Sticky action buttons: "Lưu sản phẩm" (Emerald gradient) and "Hủy bỏ / Quay lại" (Bordered gray).
   - Embeds `<partial name="_ValidationScriptsPartial" />` in `@section Scripts` to support unobtrusive jQuery client-side validation, along with file size check (< 5MB).

---

## 3. Caveats

1. **Source Code Write Restriction**:
   - Per explorer role guidelines, this explorer did not directly overwrite source code under `Areas/CuaHang/Views/`. Instead, complete, production-ready Razor templates are provided as artifacts in the explorer workspace (`proposed_Index.cshtml`, `proposed_Create.cshtml`, `proposed_Edit.cshtml`) and embedded verbatim below for the implementer/orchestrator.
2. **Sidebar Menu Integration**:
   - `_AdminLayout.cshtml` currently does not have the link to `AdminSanPham`. A proposed snippet is documented below for Milestone 2 completion.
3. **Image Assets**:
   - The views reference `/assets/journey.jpg` as the fallback thumbnail image, which already exists in `wwwroot/assets/`.

---

## 4. Conclusion & Proposed Razor Specifications

### A. View: `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml`
File artifact saved at: `.agents/teamwork/teamwork_explorer_m2_2/proposed_Index.cshtml`

```razor
@model IEnumerable<WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamGayQuy>
@{
    ViewData["Title"] = "Quản lý Sản phẩm Gây Quỹ";
    Layout = "~/Views/Shared/_AdminLayout.cshtml";

    var totalCount = Model?.Count() ?? 0;
    var activeCount = Model?.Count(x => x.TrangThai && x.SoLuongTon > 0) ?? 0;
    var totalSold = Model?.Sum(x => x.DaBan) ?? 0;
    var outOfStockCount = Model?.Count(x => x.SoLuongTon <= 0) ?? 0;
    var lowStockCount = Model?.Count(x => x.SoLuongTon > 0 && x.SoLuongTon <= 10) ?? 0;
}

<div class="max-w-7xl mx-auto my-6 px-4 sm:px-6 lg:px-8 space-y-6">

    <!-- BREADCRUMB & HEADER -->
    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
            <!-- Breadcrumbs -->
            <nav class="flex items-center gap-2 text-xs text-gray-500 mb-2 font-medium">
                <a href="@Url.Action("Index", "Admin", new { area = "" })" class="hover:text-emerald-600 transition flex items-center gap-1">
                    <i class="fas fa-home"></i> Quản trị
                </a>
                <span>/</span>
                <span class="text-emerald-700 font-semibold">Cửa Hàng Gây Quỹ</span>
                <span>/</span>
                <span class="text-gray-700">Danh sách sản phẩm</span>
            </nav>
            <h1 class="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight flex items-center gap-3">
                <span>Quản lý Sản phẩm Gây Quỹ</span>
                <span class="text-xs font-bold px-3 py-1 rounded-full bg-emerald-100 text-emerald-800 border border-emerald-200">
                    Chiến dịch 2026
                </span>
            </h1>
            <p class="mt-1 text-sm text-gray-500">
                Quản lý các ấn phẩm, quà tặng lưu niệm gây quỹ cho chiến dịch tình nguyện "Trăng Sáng Langbiang".
            </p>
        </div>

        <!-- ACTIONS -->
        <div class="flex items-center gap-3 flex-wrap">
            <a href="@Url.Action("Index", "CuaHang", new { area = "CuaHang" })" target="_blank"
               class="inline-flex items-center gap-2 px-4 py-2.5 text-sm font-semibold rounded-xl text-gray-700 bg-white hover:bg-gray-50 border border-gray-200 shadow-sm transition">
                <i class="fas fa-external-link-alt text-gray-400"></i>
                <span>Xem Cửa Hàng</span>
            </a>

            <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Create"
               class="inline-flex items-center gap-2 px-5 py-2.5 text-sm font-bold rounded-xl text-white bg-gradient-to-r from-emerald-600 to-green-700 hover:from-emerald-700 hover:to-green-800 shadow-md hover:shadow-lg transition">
                <i class="fas fa-plus-circle"></i>
                <span>Thêm sản phẩm mới</span>
            </a>
        </div>
    </div>

    <!-- FLASH ALERTS -->
    @if (TempData["Success"] != null)
    {
        <div class="flex items-center justify-between p-4 text-sm text-emerald-800 border border-emerald-200 rounded-2xl bg-emerald-50 shadow-sm transition">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-xl bg-emerald-500 text-white flex items-center justify-center font-bold text-sm shrink-0">
                    <i class="fas fa-check"></i>
                </div>
                <div>
                    <span class="font-bold">Thành công!</span> @TempData["Success"]
                </div>
            </div>
            <button type="button" onclick="this.parentElement.remove()" class="text-emerald-600 hover:text-emerald-800 text-sm font-bold p-1">
                <i class="fas fa-times"></i>
            </button>
        </div>
    }

    @if (TempData["Error"] != null)
    {
        <div class="flex items-center justify-between p-4 text-sm text-red-800 border border-red-200 rounded-2xl bg-red-50 shadow-sm transition">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-xl bg-red-500 text-white flex items-center justify-center font-bold text-sm shrink-0">
                    <i class="fas fa-exclamation-triangle"></i>
                </div>
                <div>
                    <span class="font-bold">Có lỗi xảy ra:</span> @TempData["Error"]
                </div>
            </div>
            <button type="button" onclick="this.parentElement.remove()" class="text-red-600 hover:text-red-800 text-sm font-bold p-1">
                <i class="fas fa-times"></i>
            </button>
        </div>
    }

    <!-- STATS CARDS -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <!-- Card 1 -->
        <div class="bg-white rounded-2xl p-5 border border-gray-200 shadow-sm flex items-center gap-4">
            <div class="w-12 h-12 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center text-xl shrink-0">
                <i class="fas fa-boxes"></i>
            </div>
            <div>
                <div class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Tổng sản phẩm</div>
                <div class="text-2xl font-black text-gray-900 mt-0.5">@totalCount</div>
            </div>
        </div>

        <!-- Card 2 -->
        <div class="bg-white rounded-2xl p-5 border border-gray-200 shadow-sm flex items-center gap-4">
            <div class="w-12 h-12 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center text-xl shrink-0">
                <i class="fas fa-store"></i>
            </div>
            <div>
                <div class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Đang mở bán</div>
                <div class="text-2xl font-black text-emerald-600 mt-0.5">@activeCount</div>
            </div>
        </div>

        <!-- Card 3 -->
        <div class="bg-white rounded-2xl p-5 border border-gray-200 shadow-sm flex items-center gap-4">
            <div class="w-12 h-12 rounded-xl bg-rose-50 text-rose-600 flex items-center justify-center text-xl shrink-0">
                <i class="fas fa-heart"></i>
            </div>
            <div>
                <div class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Tổng đã bán / Ủng hộ</div>
                <div class="text-2xl font-black text-rose-600 mt-0.5">@totalSold <span class="text-xs font-medium text-gray-500">vật phẩm</span></div>
            </div>
        </div>

        <!-- Card 4 -->
        <div class="bg-white rounded-2xl p-5 border border-gray-200 shadow-sm flex items-center gap-4">
            <div class="w-12 h-12 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center text-xl shrink-0">
                <i class="fas fa-warehouse"></i>
            </div>
            <div>
                <div class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Cảnh báo tồn kho</div>
                <div class="text-2xl font-black text-amber-600 mt-0.5">
                    @outOfStockCount <span class="text-xs font-medium text-gray-500">hết hàng (@lowStockCount sắp hết)</span>
                </div>
            </div>
        </div>
    </div>

    <!-- TABLE CARD -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
        <!-- TOOLBAR -->
        <div class="p-5 border-b border-gray-100 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 bg-gray-50/50">
            <div class="flex items-center gap-3">
                <span class="text-sm font-bold text-gray-800">Danh sách sản phẩm</span>
                <span class="text-xs font-semibold px-2.5 py-0.5 rounded-full bg-gray-200 text-gray-700">@totalCount</span>
            </div>

            <!-- Client Filter / Search Bar -->
            <div class="relative w-full sm:w-72">
                <i class="fas fa-search absolute left-3.5 top-1/2 -translate-y-1/2 text-gray-400 text-xs"></i>
                <input type="text" id="adminSearchInput" onkeyup="filterAdminTable()"
                       placeholder="Tìm kiếm theo tên, danh mục..."
                       class="w-full pl-9 pr-4 py-2 text-xs bg-white border border-gray-200 rounded-xl text-gray-800 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition shadow-xs" />
            </div>
        </div>

        @if (Model == null || !Model.Any())
        {
            <!-- EMPTY STATE -->
            <div class="p-12 text-center">
                <div class="w-20 h-20 mx-auto rounded-3xl bg-emerald-50 text-emerald-500 flex items-center justify-center text-3xl mb-4 shadow-inner">
                    <i class="fas fa-gift"></i>
                </div>
                <h3 class="text-lg font-bold text-gray-900 mb-1">Chưa có sản phẩm gây quỹ nào</h3>
                <p class="text-sm text-gray-500 max-w-md mx-auto mb-6">
                    Bắt đầu bằng việc thêm mới một sản phẩm (áo thun, móc khóa, quà lưu niệm...) để mở chiến dịch bán hàng gây quỹ cộng đồng.
                </p>
                <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Create"
                   class="inline-flex items-center gap-2 px-5 py-2.5 text-sm font-bold rounded-xl text-white bg-emerald-600 hover:bg-emerald-700 shadow-md transition">
                    <i class="fas fa-plus"></i> Thêm sản phẩm đầu tiên
                </a>
            </div>
        }
        else
        {
            <!-- TABLE -->
            <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200 text-left" id="productTable">
                    <thead class="bg-gray-50/80 text-gray-500 text-[11px] uppercase tracking-wider font-semibold">
                        <tr>
                            <th scope="col" class="py-3.5 px-4 text-center w-12">#</th>
                            <th scope="col" class="py-3.5 px-4 w-16">Hình ảnh</th>
                            <th scope="col" class="py-3.5 px-4">Tên sản phẩm & Danh mục</th>
                            <th scope="col" class="py-3.5 px-4">Giá bán</th>
                            <th scope="col" class="py-3.5 px-4 text-center">Tồn kho / Đã bán</th>
                            <th scope="col" class="py-3.5 px-4">Tỷ lệ quỹ & Ý nghĩa</th>
                            <th scope="col" class="py-3.5 px-4 text-center">Trạng thái</th>
                            <th scope="col" class="py-3.5 px-4 text-center w-36">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-100 bg-white text-sm">
                        @foreach (var item in Model.OrderBy(x => x.ThuTu).ThenByDescending(x => x.IDSanPham))
                        {
                            <tr class="hover:bg-gray-50/70 transition product-row">
                                <!-- ID -->
                                <td class="py-4 px-4 text-center text-xs font-mono font-bold text-gray-400">
                                    #@item.IDSanPham
                                </td>

                                <!-- IMAGE -->
                                <td class="py-4 px-4">
                                    <div class="w-14 h-14 rounded-xl overflow-hidden border border-gray-100 bg-gray-50 shadow-xs shrink-0 flex items-center justify-center">
                                        @if (!string.IsNullOrEmpty(item.LinkHinhAnh))
                                        {
                                            <img src="@item.LinkHinhAnh" alt="@item.TenSanPham"
                                                 class="w-full h-full object-cover"
                                                 onerror="this.onerror=null; this.src='/assets/journey.jpg';" />
                                        }
                                        else
                                        {
                                            <div class="text-gray-300 text-lg">
                                                <i class="fas fa-image"></i>
                                            </div>
                                        }
                                    </div>
                                </td>

                                <!-- NAME & CATEGORY -->
                                <td class="py-4 px-4">
                                    <div class="font-bold text-gray-900 leading-snug line-clamp-1 search-target-name">
                                        @item.TenSanPham
                                    </div>
                                    <div class="flex items-center gap-2 mt-1">
                                        <span class="inline-block px-2 py-0.5 rounded-md text-[11px] font-semibold bg-gray-100 text-gray-700 search-target-cat">
                                            <i class="fas fa-tag text-[9px] mr-1 text-gray-400"></i>@item.DanhMuc
                                        </span>
                                        @if (item.ThuTu > 0)
                                        {
                                            <span class="text-[10px] text-gray-400" title="Thứ tự hiển thị">
                                                TT: @item.ThuTu
                                            </span>
                                        }
                                    </div>
                                </td>

                                <!-- PRICE -->
                                <td class="py-4 px-4 whitespace-nowrap">
                                    <div class="font-extrabold text-emerald-600 text-sm">
                                        @item.GiaBan.ToString("N0") ₫
                                    </div>
                                    @if (item.GiaGoc.HasValue && item.GiaGoc.Value > item.GiaBan)
                                    {
                                        <div class="text-xs text-gray-400 line-through">
                                            @item.GiaGoc.Value.ToString("N0") ₫
                                        </div>
                                    }
                                </td>

                                <!-- INVENTORY & SALES -->
                                <td class="py-4 px-4 text-center whitespace-nowrap">
                                    <div class="font-bold text-xs">
                                        @if (item.SoLuongTon <= 0)
                                        {
                                            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold bg-red-100 text-red-700 border border-red-200">
                                                Hết hàng (0)
                                            </span>
                                        }
                                        else if (item.SoLuongTon <= 10)
                                        {
                                            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold bg-amber-100 text-amber-800 border border-amber-200">
                                                Sắp hết (@item.SoLuongTon)
                                            </span>
                                        }
                                        else
                                        {
                                            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold bg-emerald-50 text-emerald-700 border border-emerald-200">
                                                Còn: @item.SoLuongTon
                                            </span>
                                        }
                                    </div>
                                    <div class="text-[11px] text-gray-500 mt-1">
                                        Đã bán: <span class="font-bold text-gray-700">@item.DaBan</span>
                                    </div>
                                </td>

                                <!-- FUNDRAISING % & MESSAGE -->
                                <td class="py-4 px-4 max-w-xs">
                                    <div class="flex items-center gap-1.5 mb-1">
                                        <span class="px-2 py-0.5 rounded text-[10px] font-black bg-rose-50 text-rose-600 border border-rose-100">
                                            ❤️ @item.PhanTramDongGop% Quỹ
                                        </span>
                                        <span class="text-[11px] text-emerald-700 font-semibold">
                                            ~@item.TienDongGopTrenMoiSp.ToString("N0")₫/sp
                                        </span>
                                    </div>
                                    @if (!string.IsNullOrWhiteSpace(item.ThongDiepGayQuy))
                                    {
                                        <p class="text-[11px] text-gray-500 italic line-clamp-1" title="@item.ThongDiepGayQuy">
                                            "@item.ThongDiepGayQuy"
                                        </p>
                                    }
                                </td>

                                <!-- STATUS BADGE -->
                                <td class="py-4 px-4 text-center whitespace-nowrap">
                                    @if (item.TrangThai)
                                    {
                                        <span class="inline-flex items-center gap-1.5 py-1 px-3 rounded-full text-xs font-semibold bg-emerald-100 text-emerald-700">
                                            <span class="w-2 h-2 rounded-full bg-emerald-500 animate-pulse"></span>
                                            Hiển thị
                                        </span>
                                    }
                                    else
                                    {
                                        <span class="inline-flex items-center gap-1.5 py-1 px-3 rounded-full text-xs font-semibold bg-gray-100 text-gray-500">
                                            <span class="w-2 h-2 rounded-full bg-gray-400"></span>
                                            Đang ẩn
                                        </span>
                                    }
                                </td>

                                <!-- ACTIONS -->
                                <td class="py-4 px-4 whitespace-nowrap text-center">
                                    <div class="flex items-center justify-center gap-1.5">
                                        <!-- Preview -->
                                        <a href="@Url.Action("ChiTiet", "CuaHang", new { area = "CuaHang", id = item.IDSanPham })"
                                           target="_blank" title="Xem trước trên cửa hàng"
                                           class="w-8 h-8 rounded-lg bg-gray-50 text-gray-500 hover:text-emerald-600 hover:bg-emerald-50 flex items-center justify-center text-xs transition">
                                            <i class="fas fa-eye"></i>
                                        </a>

                                        <!-- Edit -->
                                        <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Edit" asp-route-id="@item.IDSanPham"
                                           title="Chỉnh sửa sản phẩm"
                                           class="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-semibold text-emerald-700 bg-emerald-50 rounded-lg hover:bg-emerald-600 hover:text-white transition">
                                            <i class="fas fa-edit"></i>
                                            <span>Sửa</span>
                                        </a>

                                        <!-- Delete POST Form -->
                                        <form asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Delete" asp-route-id="@item.IDSanPham"
                                              method="post" class="inline"
                                              onsubmit="return confirm('Bạn có chắc chắn muốn xóa sản phẩm \'@Html.Raw(Html.Encode(item.TenSanPham))\' không? Thao tác này sẽ xóa vĩnh viễn sản phẩm khỏi hệ thống!');">
                                            @Html.AntiForgeryToken()
                                            <button type="submit" title="Xóa sản phẩm"
                                                    class="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-semibold text-red-600 bg-red-50 rounded-lg hover:bg-red-600 hover:text-white transition">
                                                <i class="fas fa-trash-alt"></i>
                                                <span>Xóa</span>
                                            </button>
                                        </form>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        }
    </div>

</div>

@section Scripts {
    <script>
        // Client-side quick filter table
        function filterAdminTable() {
            var input = document.getElementById("adminSearchInput");
            var filter = input.value.toLowerCase().trim();
            var rows = document.querySelectorAll("#productTable .product-row");

            rows.forEach(function (row) {
                var nameEl = row.querySelector(".search-target-name");
                var catEl = row.querySelector(".search-target-cat");
                var name = nameEl ? nameEl.textContent.toLowerCase() : "";
                var cat = catEl ? catEl.textContent.toLowerCase() : "";

                if (name.indexOf(filter) > -1 || cat.indexOf(filter) > -1) {
                    row.style.display = "";
                } else {
                    row.style.display = "none";
                }
            });
        }
    </script>
}
```

---

### B. View: `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml`
File artifact saved at: `.agents/teamwork/teamwork_explorer_m2_2/proposed_Create.cshtml`

```razor
@model WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamCrudViewModel
@{
    ViewData["Title"] = "Thêm Mới Sản Phẩm Gây Quỹ";
    Layout = "~/Views/Shared/_AdminLayout.cshtml";
}

<div class="max-w-7xl mx-auto my-6 px-4 sm:px-6 lg:px-8 space-y-6">

    <!-- BREADCRUMB & HEADER -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-gray-200 pb-5">
        <div>
            <!-- Breadcrumbs -->
            <nav class="flex items-center gap-2 text-xs text-gray-500 mb-2 font-medium">
                <a href="@Url.Action("Index", "Admin", new { area = "" })" class="hover:text-emerald-600 transition flex items-center gap-1">
                    <i class="fas fa-home"></i> Quản trị
                </a>
                <span>/</span>
                <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index" class="hover:text-emerald-600 transition">
                    Sản phẩm gây quỹ
                </a>
                <span>/</span>
                <span class="text-emerald-700 font-semibold">Thêm mới</span>
            </nav>
            <h1 class="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight">
                Thêm Mới Sản Phẩm Gây Quỹ
            </h1>
            <p class="mt-1 text-sm text-gray-500">
                Nhập thông tin sản phẩm, định giá, thông điệp thiện nguyện và tải hình ảnh lên hệ thống.
            </p>
        </div>

        <div class="flex items-center gap-3">
            <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index"
               class="inline-flex items-center gap-2 px-4 py-2.5 text-sm font-semibold rounded-xl text-gray-700 bg-white hover:bg-gray-50 border border-gray-300 shadow-sm transition">
                <i class="fas fa-arrow-left text-gray-400"></i>
                <span>Quay lại</span>
            </a>

            <button type="submit" form="createProductForm"
                    class="inline-flex items-center gap-2 px-6 py-2.5 text-sm font-bold rounded-xl text-white bg-gradient-to-r from-emerald-600 to-green-700 hover:from-emerald-700 hover:to-green-800 shadow-md hover:shadow-lg transition">
                <i class="fas fa-save"></i>
                <span>Lưu Sản Phẩm</span>
            </button>
        </div>
    </div>

    <!-- MAIN FORM -->
    <form asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Create"
          method="post" enctype="multipart/form-data" id="createProductForm" class="space-y-6">

        @Html.AntiForgeryToken()

        <!-- Validation Summary -->
        <div asp-validation-summary="ModelOnly" class="p-4 rounded-2xl bg-red-50 border border-red-200 text-sm text-red-700 shadow-sm" id="validationSummaryBox">
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">

            <!-- LEFT COLUMN (8/12) - THÔNG TIN CHÍNH -->
            <div class="lg:col-span-8 space-y-6">

                <!-- CARD 1: THÔNG TIN CƠ BẢN -->
                <div class="bg-white rounded-2xl p-6 sm:p-7 border border-gray-200 shadow-sm space-y-5">
                    <div class="flex items-center justify-between border-b border-gray-100 pb-3">
                        <h2 class="text-base font-bold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-info-circle text-emerald-600"></i>
                            <span>1. Thông tin cơ bản</span>
                        </h2>
                        <span class="text-xs text-red-500 font-semibold">* Trường bắt buộc</span>
                    </div>

                    <!-- Tên sản phẩm -->
                    <div>
                        <label asp-for="TenSanPham" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Tên sản phẩm <span class="text-red-500">*</span>
                        </label>
                        <input asp-for="TenSanPham"
                               placeholder="Ví dụ: Áo Thun Tình Nguyện 'Trăng Sáng Langbiang 2026'..."
                               class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm font-semibold focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="TenSanPham" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Danh mục & Thứ tự hiển thị -->
                    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div>
                            <label asp-for="DanhMuc" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                                Danh mục sản phẩm <span class="text-red-500">*</span>
                            </label>
                            <input asp-for="DanhMuc" list="danhMucGoiY"
                                   placeholder="Chọn hoặc nhập danh mục mới"
                                   class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <datalist id="danhMucGoiY">
                                <option value="Trang phục" />
                                <option value="Phụ kiện" />
                                <option value="Quà lưu niệm" />
                                <option value="Văn phòng phẩm" />
                                <option value="Đặc sản vùng cao" />
                            </datalist>
                            <span asp-validation-for="DanhMuc" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                        </div>

                        <div>
                            <label asp-for="ThuTu" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                                Thứ tự ưu tiên hiển thị
                            </label>
                            <input asp-for="ThuTu" type="number" min="0" value="0"
                                   class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <p class="text-[11px] text-gray-400 mt-1">Số nhỏ hơn sẽ hiển thị trước (0, 1, 2...)</p>
                            <span asp-validation-for="ThuTu" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                        </div>
                    </div>

                    <!-- Mô tả ngắn gọn -->
                    <div>
                        <label asp-for="MoTaNgan" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Mô tả ngắn gọn (Hiển thị thẻ danh mục)
                        </label>
                        <textarea asp-for="MoTaNgan" rows="3"
                                  placeholder="Tóm tắt điểm nổi bật nhất của vật phẩm (chất liệu, công dụng, quy cách)..."
                                  class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none resize-none"></textarea>
                        <p class="text-[11px] text-gray-400 mt-1">Khuyên dùng từ 1 đến 2 câu ngắn, tối đa 500 ký tự.</p>
                        <span asp-validation-for="MoTaNgan" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Mô tả chi tiết -->
                    <div>
                        <label asp-for="MoTaChiTiet" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Mô tả chi tiết sản phẩm
                        </label>
                        <textarea asp-for="MoTaChiTiet" rows="7"
                                  placeholder="Mô tả cụ thể về chất liệu, kích thước, nguồn gốc, quy trình sản xuất thủ công, ý nghĩa thiết kế..."
                                  class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none leading-relaxed"></textarea>
                        <span asp-validation-for="MoTaChiTiet" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 2: Ý NGHĨA THIỆN NGUYỆN & GÂY QUỸ -->
                <div class="bg-white rounded-2xl p-6 sm:p-7 border border-emerald-100 shadow-sm space-y-5 bg-gradient-to-br from-white to-emerald-50/20">
                    <div class="flex items-center justify-between border-b border-gray-100 pb-3">
                        <h2 class="text-base font-bold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-hand-holding-heart text-rose-500"></i>
                            <span>2. Ý nghĩa thiện nguyện & Mục đích gây quỹ</span>
                        </h2>
                        <span class="text-xs font-bold px-2.5 py-0.5 rounded-full bg-rose-50 text-rose-600 border border-rose-100">
                            Tính năng gây quỹ
                        </span>
                    </div>

                    <!-- Tỷ lệ trích quỹ -->
                    <div>
                        <div class="flex items-center justify-between mb-2">
                            <label asp-for="PhanTramDongGop" class="text-xs font-bold uppercase tracking-wider text-gray-700">
                                Tỷ lệ doanh thu đóng góp vào quỹ thiện nguyện (%)
                            </label>
                            <span id="labelPhanTram" class="px-2.5 py-0.5 rounded-md bg-rose-100 text-rose-700 font-black text-xs">
                                100%
                            </span>
                        </div>
                        <div class="flex items-center gap-4">
                            <input asp-for="PhanTramDongGop" type="range" min="1" max="100" value="100" id="rangePhanTram"
                                   oninput="updatePhanTramDisplay(this.value)"
                                   class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-emerald-600" />
                            <input asp-for="PhanTramDongGop" type="number" min="1" max="100" value="100" id="numberPhanTram"
                                   oninput="updatePhanTramDisplay(this.value)"
                                   class="w-20 px-3 py-1.5 bg-gray-50 border border-gray-300 rounded-lg text-sm text-center font-bold" />
                        </div>
                        <p class="text-[11px] text-gray-500 mt-1.5">
                            * Mặc định 100% toàn bộ tiền bán sản phẩm sẽ được chuyển thẳng vào quỹ "Trăng Sáng Langbiang 2026".
                        </p>
                        <span asp-validation-for="PhanTramDongGop" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Thông điệp gây quỹ -->
                    <div>
                        <label asp-for="ThongDiepGayQuy" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Thông điệp gây quỹ / Ý nghĩa nhân văn
                        </label>
                        <textarea asp-for="ThongDiepGayQuy" rows="2"
                                  placeholder="Ví dụ: Đóng góp 50.000₫/áo vào quỹ quà tặng Trung thu cho 200 em nhỏ đồng bào K'Ho tại Langbiang."
                                  class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none resize-none"></textarea>
                        <span asp-validation-for="ThongDiepGayQuy" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

            </div>

            <!-- RIGHT COLUMN (4/12) - GIÁ, TỒN KHO & ẢNH -->
            <div class="lg:col-span-4 space-y-6">

                <!-- CARD 3: GIÁ BÁN & TỒN KHO -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-tag text-emerald-600"></i>
                        <span>3. Giá bán & Kho hàng</span>
                    </h2>

                    <!-- Giá bán -->
                    <div>
                        <label asp-for="GiaBan" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Giá bán ủng hộ (VNĐ) <span class="text-red-500">*</span>
                        </label>
                        <div class="relative">
                            <input asp-for="GiaBan" type="number" step="1000" min="0" placeholder="129000"
                                   class="w-full pl-4 pr-12 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-base font-bold text-emerald-700 focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-bold text-gray-400">₫</span>
                        </div>
                        <span asp-validation-for="GiaBan" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Giá gốc -->
                    <div>
                        <label asp-for="GiaGoc" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Giá niêm yết cũ (nếu có để gạch ngang)
                        </label>
                        <div class="relative">
                            <input asp-for="GiaGoc" type="number" step="1000" min="0" placeholder="150000"
                                   class="w-full pl-4 pr-12 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-bold text-gray-400">₫</span>
                        </div>
                        <span asp-validation-for="GiaGoc" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Số lượng tồn kho -->
                    <div>
                        <label asp-for="SoLuongTon" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Số lượng tồn kho ban đầu <span class="text-red-500">*</span>
                        </label>
                        <input asp-for="SoLuongTon" type="number" min="0" value="50"
                               class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm font-semibold focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="SoLuongTon" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 4: HÌNH ẢNH SẢN PHẨM -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-camera text-emerald-600"></i>
                        <span>4. Hình ảnh sản phẩm</span>
                    </h2>

                    <!-- PREVIEW BOX -->
                    <div>
                        <div class="text-xs font-bold text-gray-600 uppercase mb-2">Xem trước hình ảnh</div>
                        <div class="w-full h-48 rounded-xl bg-gray-50 border-2 border-dashed border-gray-300 overflow-hidden flex items-center justify-center relative group">
                            <img id="imagePreview" src="/assets/journey.jpg" alt="Preview"
                                 class="w-full h-full object-cover transition group-hover:scale-105"
                                 onerror="this.onerror=null; this.src='/assets/journey.jpg';" />
                            <div class="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition flex items-center justify-center text-white text-xs font-semibold">
                                <i class="fas fa-search-plus mr-1"></i> Ảnh hiển thị
                            </div>
                        </div>
                    </div>

                    <!-- File Upload Input -->
                    <div>
                        <label asp-for="FileHinhAnh" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Tải tệp từ thiết bị (Ưu tiên)
                        </label>
                        <input asp-for="FileHinhAnh" type="file" accept="image/*" id="fileUploadInput"
                               class="block w-full text-xs text-gray-500 file:mr-3 file:py-2 file:px-3 file:rounded-xl file:border-0 file:text-xs file:font-semibold file:bg-emerald-50 file:text-emerald-700 hover:file:bg-emerald-100 cursor-pointer border border-gray-200 rounded-xl" />
                        <span asp-validation-for="FileHinhAnh" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <div class="text-center text-xs font-semibold text-gray-400">--- HOẶC ---</div>

                    <!-- Direct URL Link Input -->
                    <div>
                        <label asp-for="LinkHinhAnh" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Đường dẫn URL / Tệp có sẵn
                        </label>
                        <input asp-for="LinkHinhAnh" id="linkHinhAnhInput"
                               placeholder="/assets/lanterns.jpg hoặc https://..."
                               class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-xs focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="LinkHinhAnh" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 5: TRẠNG THÁI HIỂN THỊ -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-eye text-emerald-600"></i>
                        <span>5. Trạng thái</span>
                    </h2>

                    <label class="flex items-start gap-3 cursor-pointer select-none">
                        <input asp-for="TrangThai" type="checkbox" checked
                               class="w-5 h-5 mt-0.5 rounded text-emerald-600 focus:ring-emerald-500 border-gray-300 cursor-pointer" />
                        <div>
                            <span class="text-sm font-bold text-gray-900 block">Kích hoạt hiển thị</span>
                            <span class="text-xs text-gray-500">Cho phép người dùng thấy và đặt mua sản phẩm này trên cửa hàng gây quỹ.</span>
                        </div>
                    </label>
                </div>

                <!-- ACTION BUTTONS -->
                <div class="pt-2 flex flex-col gap-3">
                    <button type="submit"
                            class="w-full flex items-center justify-center gap-2 py-3.5 px-6 rounded-xl text-white font-bold text-sm bg-gradient-to-r from-emerald-600 to-green-700 hover:from-emerald-700 hover:to-green-800 shadow-md hover:shadow-lg transition">
                        <i class="fas fa-check-circle"></i>
                        <span>Tạo Mới Sản Phẩm</span>
                    </button>

                    <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index"
                       class="w-full flex items-center justify-center gap-2 py-2.5 px-4 rounded-xl text-gray-700 font-semibold text-xs bg-white hover:bg-gray-50 border border-gray-300 shadow-sm transition">
                        <i class="fas fa-times"></i>
                        <span>Hủy bỏ & Quay lại</span>
                    </a>
                </div>

            </div>

        </div>

    </form>

</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />

    <script>
        // Đồng bộ Tỷ lệ % đóng góp giữa Range Slider và Input Number
        function updatePhanTramDisplay(val) {
            val = Math.min(100, Math.max(1, parseInt(val) || 100));
            document.getElementById('rangePhanTram').value = val;
            document.getElementById('numberPhanTram').value = val;
            document.getElementById('labelPhanTram').textContent = val + '%';
        }

        // Preview ảnh khi chọn file từ máy
        document.getElementById('fileUploadInput').addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                if (file.size > 5 * 1024 * 1024) {
                    alert('Kích thước ảnh vượt quá 5MB. Vui lòng chọn ảnh nhẹ hơn!');
                    e.target.value = '';
                    return;
                }
                const reader = new FileReader();
                reader.onload = function(event) {
                    document.getElementById('imagePreview').src = event.target.result;
                };
                reader.readAsDataURL(file);
            }
        });

        // Preview ảnh khi gõ Link URL trực tiếp
        document.getElementById('linkHinhAnhInput').addEventListener('input', function(e) {
            const url = e.target.value.trim();
            if (url) {
                document.getElementById('imagePreview').src = url;
            }
        });
    </script>
}
```

---

### C. View: `WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml`
File artifact saved at: `.agents/teamwork/teamwork_explorer_m2_2/proposed_Edit.cshtml`

```razor
@model WEB_TinhNguyen.Areas.CuaHang.Models.SanPhamCrudViewModel
@{
    ViewData["Title"] = "Chỉnh sửa: " + Model.TenSanPham;
    Layout = "~/Views/Shared/_AdminLayout.cshtml";
    string currentImage = !string.IsNullOrEmpty(Model.LinkHinhAnh) ? Model.LinkHinhAnh : "/assets/journey.jpg";
}

<div class="max-w-7xl mx-auto my-6 px-4 sm:px-6 lg:px-8 space-y-6">

    <!-- BREADCRUMB & HEADER -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-gray-200 pb-5">
        <div>
            <!-- Breadcrumbs -->
            <nav class="flex items-center gap-2 text-xs text-gray-500 mb-2 font-medium">
                <a href="@Url.Action("Index", "Admin", new { area = "" })" class="hover:text-emerald-600 transition flex items-center gap-1">
                    <i class="fas fa-home"></i> Quản trị
                </a>
                <span>/</span>
                <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index" class="hover:text-emerald-600 transition">
                    Sản phẩm gây quỹ
                </a>
                <span>/</span>
                <span class="text-emerald-700 font-semibold">Chỉnh sửa #@Model.IDSanPham</span>
            </nav>
            <h1 class="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight flex items-center gap-2.5 flex-wrap">
                <span>Chỉnh Sửa Sản Phẩm</span>
                <span class="text-emerald-600 font-black">#@Model.IDSanPham</span>
            </h1>
            <p class="mt-1 text-sm text-gray-500">
                Cập nhật thông tin chi tiết, giá bán, số lượng tồn kho và hình ảnh sản phẩm.
            </p>
        </div>

        <div class="flex items-center gap-3 flex-wrap">
            <a href="@Url.Action("ChiTiet", "CuaHang", new { area = "CuaHang", id = Model.IDSanPham })" target="_blank"
               class="inline-flex items-center gap-2 px-4 py-2.5 text-sm font-semibold rounded-xl text-gray-700 bg-white hover:bg-gray-50 border border-gray-300 shadow-sm transition">
                <i class="fas fa-external-link-alt text-gray-400"></i>
                <span>Xem trên Cửa Hàng</span>
            </a>

            <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index"
               class="inline-flex items-center gap-2 px-4 py-2.5 text-sm font-semibold rounded-xl text-gray-700 bg-white hover:bg-gray-50 border border-gray-300 shadow-sm transition">
                <i class="fas fa-arrow-left text-gray-400"></i>
                <span>Quay lại</span>
            </a>

            <button type="submit" form="editProductForm"
                    class="inline-flex items-center gap-2 px-6 py-2.5 text-sm font-bold rounded-xl text-white bg-gradient-to-r from-emerald-600 to-green-700 hover:from-emerald-700 hover:to-green-800 shadow-md hover:shadow-lg transition">
                <i class="fas fa-save"></i>
                <span>Lưu Thay Đổi</span>
            </button>
        </div>
    </div>

    <!-- FLASH ALERTS -->
    @if (TempData["Success"] != null)
    {
        <div class="flex items-center justify-between p-4 text-sm text-emerald-800 border border-emerald-200 rounded-2xl bg-emerald-50 shadow-sm">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-xl bg-emerald-500 text-white flex items-center justify-center font-bold text-sm shrink-0">
                    <i class="fas fa-check"></i>
                </div>
                <div><span class="font-bold">Thành công!</span> @TempData["Success"]</div>
            </div>
            <button type="button" onclick="this.parentElement.remove()" class="text-emerald-600 hover:text-emerald-800 text-sm font-bold p-1">
                <i class="fas fa-times"></i>
            </button>
        </div>
    }

    <!-- MAIN FORM -->
    <form asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Edit" asp-route-id="@Model.IDSanPham"
          method="post" enctype="multipart/form-data" id="editProductForm" class="space-y-6">

        @Html.AntiForgeryToken()
        <input type="hidden" asp-for="IDSanPham" />
        <input type="hidden" asp-for="IDChienDich" />

        <!-- Validation Summary -->
        <div asp-validation-summary="ModelOnly" class="p-4 rounded-2xl bg-red-50 border border-red-200 text-sm text-red-700 shadow-sm" id="validationSummaryBox">
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">

            <!-- LEFT COLUMN (8/12) - THÔNG TIN CHÍNH -->
            <div class="lg:col-span-8 space-y-6">

                <!-- CARD 1: THÔNG TIN CƠ BẢN -->
                <div class="bg-white rounded-2xl p-6 sm:p-7 border border-gray-200 shadow-sm space-y-5">
                    <div class="flex items-center justify-between border-b border-gray-100 pb-3">
                        <h2 class="text-base font-bold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-info-circle text-emerald-600"></i>
                            <span>1. Thông tin cơ bản</span>
                        </h2>
                        <span class="text-xs text-red-500 font-semibold">* Trường bắt buộc</span>
                    </div>

                    <!-- Tên sản phẩm -->
                    <div>
                        <label asp-for="TenSanPham" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Tên sản phẩm <span class="text-red-500">*</span>
                        </label>
                        <input asp-for="TenSanPham"
                               placeholder="Tên sản phẩm..."
                               class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm font-semibold focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="TenSanPham" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Danh mục & Thứ tự hiển thị -->
                    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div>
                            <label asp-for="DanhMuc" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                                Danh mục sản phẩm <span class="text-red-500">*</span>
                            </label>
                            <input asp-for="DanhMuc" list="danhMucGoiY"
                                   placeholder="Chọn hoặc nhập danh mục"
                                   class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <datalist id="danhMucGoiY">
                                <option value="Trang phục" />
                                <option value="Phụ kiện" />
                                <option value="Quà lưu niệm" />
                                <option value="Văn phòng phẩm" />
                                <option value="Đặc sản vùng cao" />
                            </datalist>
                            <span asp-validation-for="DanhMuc" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                        </div>

                        <div>
                            <label asp-for="ThuTu" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                                Thứ tự ưu tiên hiển thị
                            </label>
                            <input asp-for="ThuTu" type="number" min="0"
                                   class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <p class="text-[11px] text-gray-400 mt-1">Số nhỏ hơn sẽ hiển thị trước (0, 1, 2...)</p>
                            <span asp-validation-for="ThuTu" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                        </div>
                    </div>

                    <!-- Mô tả ngắn gọn -->
                    <div>
                        <label asp-for="MoTaNgan" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Mô tả ngắn gọn (Hiển thị thẻ danh mục)
                        </label>
                        <textarea asp-for="MoTaNgan" rows="3"
                                  placeholder="Tóm tắt điểm nổi bật nhất của vật phẩm..."
                                  class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none resize-none"></textarea>
                        <p class="text-[11px] text-gray-400 mt-1">Khuyên dùng từ 1 đến 2 câu ngắn, tối đa 500 ký tự.</p>
                        <span asp-validation-for="MoTaNgan" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Mô tả chi tiết -->
                    <div>
                        <label asp-for="MoTaChiTiet" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Mô tả chi tiết sản phẩm
                        </label>
                        <textarea asp-for="MoTaChiTiet" rows="7"
                                  placeholder="Mô tả cụ thể về chất liệu, kích thước, nguồn gốc, quy trình sản xuất..."
                                  class="w-full px-4 py-3 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none leading-relaxed"></textarea>
                        <span asp-validation-for="MoTaChiTiet" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 2: Ý NGHĨA THIỆN NGUYỆN & GÂY QUỸ -->
                <div class="bg-white rounded-2xl p-6 sm:p-7 border border-emerald-100 shadow-sm space-y-5 bg-gradient-to-br from-white to-emerald-50/20">
                    <div class="flex items-center justify-between border-b border-gray-100 pb-3">
                        <h2 class="text-base font-bold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-hand-holding-heart text-rose-500"></i>
                            <span>2. Ý nghĩa thiện nguyện & Mục đích gây quỹ</span>
                        </h2>
                        <span class="text-xs font-bold px-2.5 py-0.5 rounded-full bg-rose-50 text-rose-600 border border-rose-100">
                            Tính năng gây quỹ
                        </span>
                    </div>

                    <!-- Tỷ lệ trích quỹ -->
                    <div>
                        <div class="flex items-center justify-between mb-2">
                            <label asp-for="PhanTramDongGop" class="text-xs font-bold uppercase tracking-wider text-gray-700">
                                Tỷ lệ doanh thu đóng góp vào quỹ thiện nguyện (%)
                            </label>
                            <span id="labelPhanTram" class="px-2.5 py-0.5 rounded-md bg-rose-100 text-rose-700 font-black text-xs">
                                @Model.PhanTramDongGop%
                            </span>
                        </div>
                        <div class="flex items-center gap-4">
                            <input asp-for="PhanTramDongGop" type="range" min="1" max="100" id="rangePhanTram"
                                   oninput="updatePhanTramDisplay(this.value)"
                                   class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-emerald-600" />
                            <input asp-for="PhanTramDongGop" type="number" min="1" max="100" id="numberPhanTram"
                                   oninput="updatePhanTramDisplay(this.value)"
                                   class="w-20 px-3 py-1.5 bg-gray-50 border border-gray-300 rounded-lg text-sm text-center font-bold" />
                        </div>
                        <p class="text-[11px] text-gray-500 mt-1.5">
                            * Mặc định 100% toàn bộ tiền bán sản phẩm sẽ được chuyển thẳng vào quỹ "Trăng Sáng Langbiang 2026".
                        </p>
                        <span asp-validation-for="PhanTramDongGop" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Thông điệp gây quỹ -->
                    <div>
                        <label asp-for="ThongDiepGayQuy" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Thông điệp gây quỹ / Ý nghĩa nhân văn
                        </label>
                        <textarea asp-for="ThongDiepGayQuy" rows="2"
                                  placeholder="Ví dụ: Đóng góp 50.000₫/áo vào quỹ quà tặng Trung thu cho 200 em nhỏ đồng bào K'Ho tại Langbiang."
                                  class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none resize-none"></textarea>
                        <span asp-validation-for="ThongDiepGayQuy" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

            </div>

            <!-- RIGHT COLUMN (4/12) - GIÁ, TỒN KHO & ẢNH -->
            <div class="lg:col-span-4 space-y-6">

                <!-- CARD 3: GIÁ BÁN & TỒN KHO -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-tag text-emerald-600"></i>
                        <span>3. Giá bán & Kho hàng</span>
                    </h2>

                    <!-- Giá bán -->
                    <div>
                        <label asp-for="GiaBan" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Giá bán ủng hộ (VNĐ) <span class="text-red-500">*</span>
                        </label>
                        <div class="relative">
                            <input asp-for="GiaBan" type="number" step="1000" min="0"
                                   class="w-full pl-4 pr-12 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-base font-bold text-emerald-700 focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-bold text-gray-400">₫</span>
                        </div>
                        <span asp-validation-for="GiaBan" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Giá gốc -->
                    <div>
                        <label asp-for="GiaGoc" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Giá niêm yết cũ (nếu có để gạch ngang)
                        </label>
                        <div class="relative">
                            <input asp-for="GiaGoc" type="number" step="1000" min="0"
                                   class="w-full pl-4 pr-12 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                            <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-bold text-gray-400">₫</span>
                        </div>
                        <span asp-validation-for="GiaGoc" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <!-- Số lượng tồn kho -->
                    <div>
                        <label asp-for="SoLuongTon" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Số lượng tồn kho <span class="text-red-500">*</span>
                        </label>
                        <input asp-for="SoLuongTon" type="number" min="0"
                               class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-sm font-semibold focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="SoLuongTon" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 4: HÌNH ẢNH SẢN PHẨM -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-camera text-emerald-600"></i>
                        <span>4. Hình ảnh sản phẩm</span>
                    </h2>

                    <!-- PREVIEW BOX CÓ SẴN ẢNH CŨ -->
                    <div>
                        <div class="flex items-center justify-between text-xs font-bold text-gray-600 uppercase mb-2">
                            <span>Ảnh hiển thị hiện tại</span>
                            <span class="text-[10px] text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full font-bold">Hiện hữu</span>
                        </div>
                        <div class="w-full h-48 rounded-xl bg-gray-50 border-2 border-dashed border-gray-300 overflow-hidden flex items-center justify-center relative group">
                            <img id="imagePreview" src="@currentImage" alt="@Model.TenSanPham"
                                 class="w-full h-full object-cover transition group-hover:scale-105"
                                 onerror="this.onerror=null; this.src='/assets/journey.jpg';" />
                            <div class="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition flex items-center justify-center text-white text-xs font-semibold">
                                <i class="fas fa-search-plus mr-1"></i> Xem to ảnh
                            </div>
                        </div>
                    </div>

                    <!-- File Upload Input -->
                    <div>
                        <label asp-for="FileHinhAnh" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Thay ảnh mới từ thiết bị
                        </label>
                        <input asp-for="FileHinhAnh" type="file" accept="image/*" id="fileUploadInput"
                               class="block w-full text-xs text-gray-500 file:mr-3 file:py-2 file:px-3 file:rounded-xl file:border-0 file:text-xs file:font-semibold file:bg-emerald-50 file:text-emerald-700 hover:file:bg-emerald-100 cursor-pointer border border-gray-200 rounded-xl" />
                        <p class="text-[11px] text-gray-400 mt-1">Để trống nếu muốn giữ nguyên ảnh hiện tại.</p>
                        <span asp-validation-for="FileHinhAnh" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>

                    <div class="text-center text-xs font-semibold text-gray-400">--- HOẶC ---</div>

                    <!-- Direct URL Link Input -->
                    <div>
                        <label asp-for="LinkHinhAnh" class="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                            Đường dẫn URL / Tệp có sẵn
                        </label>
                        <input asp-for="LinkHinhAnh" id="linkHinhAnhInput"
                               placeholder="/assets/lanterns.jpg hoặc https://..."
                               class="w-full px-4 py-2.5 bg-gray-50 border border-gray-300 rounded-xl text-gray-900 text-xs focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 focus:bg-white transition-all outline-none" />
                        <span asp-validation-for="LinkHinhAnh" class="text-xs text-red-600 mt-1.5 block font-medium"></span>
                    </div>
                </div>

                <!-- CARD 5: TRẠNG THÁI HIỂN THỊ -->
                <div class="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm space-y-4">
                    <h2 class="text-base font-bold text-gray-900 border-b border-gray-100 pb-3 flex items-center gap-2">
                        <i class="fas fa-eye text-emerald-600"></i>
                        <span>5. Trạng thái</span>
                    </h2>

                    <label class="flex items-start gap-3 cursor-pointer select-none">
                        <input asp-for="TrangThai" type="checkbox"
                               class="w-5 h-5 mt-0.5 rounded text-emerald-600 focus:ring-emerald-500 border-gray-300 cursor-pointer" />
                        <div>
                            <span class="text-sm font-bold text-gray-900 block">Kích hoạt hiển thị</span>
                            <span class="text-xs text-gray-500">Cho phép người dùng thấy và đặt mua sản phẩm này trên cửa hàng gây quỹ.</span>
                        </div>
                    </label>
                </div>

                <!-- ACTION BUTTONS -->
                <div class="pt-2 flex flex-col gap-3">
                    <button type="submit"
                            class="w-full flex items-center justify-center gap-2 py-3.5 px-6 rounded-xl text-white font-bold text-sm bg-gradient-to-r from-emerald-600 to-green-700 hover:from-emerald-700 hover:to-green-800 shadow-md hover:shadow-lg transition">
                        <i class="fas fa-save"></i>
                        <span>Lưu Các Thay Đổi</span>
                    </button>

                    <a asp-area="CuaHang" asp-controller="AdminSanPham" asp-action="Index"
                       class="w-full flex items-center justify-center gap-2 py-2.5 px-4 rounded-xl text-gray-700 font-semibold text-xs bg-white hover:bg-gray-50 border border-gray-300 shadow-sm transition">
                        <i class="fas fa-times"></i>
                        <span>Hủy bỏ & Quay lại</span>
                    </a>
                </div>

            </div>

        </div>

    </form>

</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />

    <script>
        // Đồng bộ Tỷ lệ % đóng góp giữa Range Slider và Input Number
        function updatePhanTramDisplay(val) {
            val = Math.min(100, Math.max(1, parseInt(val) || 100));
            document.getElementById('rangePhanTram').value = val;
            document.getElementById('numberPhanTram').value = val;
            document.getElementById('labelPhanTram').textContent = val + '%';
        }

        // Preview ảnh mới khi chọn file từ máy
        document.getElementById('fileUploadInput').addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                if (file.size > 5 * 1024 * 1024) {
                    alert('Kích thước ảnh vượt quá 5MB. Vui lòng chọn ảnh nhẹ hơn!');
                    e.target.value = '';
                    return;
                }
                const reader = new FileReader();
                reader.onload = function(event) {
                    document.getElementById('imagePreview').src = event.target.result;
                };
                reader.readAsDataURL(file);
            }
        });

        // Preview ảnh khi đổi Link URL trực tiếp
        document.getElementById('linkHinhAnhInput').addEventListener('input', function(e) {
            const url = e.target.value.trim();
            if (url) {
                document.getElementById('imagePreview').src = url;
            }
        });
    </script>
}
```

---

### D. Sidebar Navigation Integration in `_AdminLayout.cshtml`
In `WEB_TinhNguyen/WEB_TinhNguyen/Views/Shared/_AdminLayout.cshtml`, under the `<h4><span>Nội dung</span>...</h4>` section (around line 282), insert:

```html
                <li>
                    <a href="@Url.Action("Index", "AdminSanPham", new { area = "CuaHang" })" class="@(ViewContext.RouteData.Values["Controller"]?.ToString() == "AdminSanPham" ? "active" : "")">
                        <i class="fas fa-boxes"></i> <span class="link-text">Sản phẩm Gây Quỹ</span>
                    </a>
                </li>
```

---

## 5. Verification Method

1. **Files Presence**:
   Verify the 3 template artifacts exist in the explorer directory:
   - `file:///home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Index.cshtml`
   - `file:///home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Create.cshtml`
   - `file:///home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Edit.cshtml`

2. **Destination Paths for Implementation**:
   Copy or create the files at their destination paths:
   ```bash
   mkdir -p /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham
   cp /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Index.cshtml /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Index.cshtml
   cp /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Create.cshtml /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Create.cshtml
   cp /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m2_2/proposed_Edit.cshtml /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/AdminSanPham/Edit.cshtml
   ```

3. **Compilation Command**:
   Execute:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   **Expected result**: 0 Compilation errors.

4. **Functional Checklist**:
   - `Index.cshtml`: Table loads with product rows, action links point to `Edit` and `Delete`, client-side search box filters table rows instantaneously, and `TempData["Success"]` renders if populated.
   - `Create.cshtml`: Submitting form validates required fields (`TenSanPham`, `GiaBan`, `SoLuongTon`), file upload previews selected file in image container, and range slider updates percentage display.
   - `Edit.cshtml`: Loads existing data, shows current image preview with badge, and submits update back to `AdminSanPhamController.Edit`.
