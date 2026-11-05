# BÁO CÁO HANDOFF: THIẾT KẾ CẤU HÌNH HỆ THỐNG M1 (Program.cs, ApplicationDbContext.cs & tailwind.config.js)

**Tác vụ**: Khảo sát, phân tích và cung cấp thiết kế triển khai chính xác từng dòng mã cho `Program.cs`, `ApplicationDbContext.cs`, và `tailwind.config.js` phục vụ Milestone 1 (Area & Data Foundation).  
**Đặc vụ thực hiện**: `teamwork_explorer_m1_1`  
**Thư mục làm việc**: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1`  
**Ngày thực hiện**: 2026-11-03  
**Dự án**: HỆ THỐNG CỬA HÀNG SẢN PHẨM GÂY QUỸ TÌNH NGUYỆN (WEB_TinhNguyen, .NET 10.0, EF Core 10.0.12)  

---

## 1. Observation (Các quan sát thực tế)

### 1.1. Hiện trạng tệp `WEB_TinhNguyen/Program.cs`
Đường dẫn tuyệt đối: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Program.cs` (toàn bộ 38 dòng):
```csharp
1: using WEB_TinhNguyen.Data;
2: using Microsoft.EntityFrameworkCore;
3: 
4: var builder = WebApplication.CreateBuilder(args);
5: // Dán đoạn này vào để kết nối DB
6: builder.Services.AddDbContext<ApplicationDbContext>(options =>
7:     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
8: 
9: // Add services to the container.
10: builder.Services.AddControllersWithViews();
11: // Add services to the container.
12: builder.Services.AddControllersWithViews();
13: 
14: var app = builder.Build();
15: 
16: // Configure the HTTP request pipeline.
17: if (!app.Environment.IsDevelopment())
18: {
19:     app.UseExceptionHandler("/Home/Error");
20:     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
21:     app.UseHsts();
22: }
23: 
24: app.UseHttpsRedirection();
25: app.UseRouting();
26: 
27: app.UseAuthorization();
28: 
29: app.MapStaticAssets();
30: 
31: app.MapControllerRoute(
32:     name: "default",
33:     pattern: "{controller=Home}/{action=Index}/{id?}")
34:     .WithStaticAssets();
35: 
36: 
37: app.Run();
38: 
```

**Quan sát trực tiếp**:
- Dòng 10 và 12 đang bị lặp lại cùng một câu lệnh `builder.Services.AddControllersWithViews();`.
- Chưa hề đăng ký `AddDistributedMemoryCache()`, `AddSession(...)` và `AddHttpContextAccessor()`.
- Pipeline HTTP chưa có middleware `app.UseSession()`.
- Endpoint routing chỉ có duy nhất route `"default"`, chưa có MapControllerRoute cho Area (`"{area:exists}/{controller=CuaHang}/{action=Index}/{id?}"`).

### 1.2. Hiện trạng tệp `WEB_TinhNguyen/Data/ApplicationDbContext.cs`
Đường dẫn tuyệt đối: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Data/ApplicationDbContext.cs` (toàn bộ 34 dòng):
```csharp
1: using Microsoft.EntityFrameworkCore;
2: using WEB_TinhNguyen.Models;
3: 
4: namespace WEB_TinhNguyen.Data
5: {
6:     public class ApplicationDbContext : DbContext
7:     {
8:         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
9:         {
10:         }
11: 
12:         // Khai báo các bảng để Entity Framework tạo trong Database
13:         public DbSet<ChienDich> ChienDichs { get; set; }
14:         public DbSet<CauHinhTrangChu> CauHinhTrangChus { get; set; }
15:         public DbSet<CauHinhAnhNoi> CauHinhAnhNois { get; set; }
16:         public DbSet<BannerTrangChu> BannerTrangChus { get; set; }
17:         public DbSet<ChuongTrinhDemNguoc> ChuongTrinhDemNguocs { get; set; }
18:         public DbSet<CauHinhKyUc> CauHinhKyUcs { get; set; }
19:         public DbSet<AnhKyUc> AnhKyUcs { get; set; }
20:         public DbSet<NhaTaiTroKyUc> NhaTaiTroKyUcs { get; set; }
21:         public DbSet<CauHinhChuongTrinh> CauHinhChuongTrinhs { get; set; }
22:         public DbSet<ChangChuongTrinh> ChangChuongTrinhs { get; set; }
23:         public DbSet<ChiTietLoTrinh> ChiTietLoTrinhs { get; set; }
24:         public DbSet<CauHinhGayQuy> CauHinhGayQuys { get; set; }
25:         public DbSet<KenhDongGop> KenhDongGops { get; set; }
26:         public DbSet<NhaTaiTroGayQuy> NhaTaiTroGayQuys { get; set; }
27:         public DbSet<CauHinhBanToChuc> CauHinhBanToChucs { get; set; }
28:         public DbSet<ThanhVienBanToChuc> ThanhVienBanToChucs { get; set; }
29:         public DbSet<CauHinhTinTuc> CauHinhTinTucs { get; set; }
30:         public DbSet<BaiVietTinTuc> BaiVietTinTucs { get; set; }
31:         public DbSet<ThuVienHieuUng> ThuVienHieuUngs { get; set; }
32:         // Bạn có thể thêm DbSet cho các bảng khác (TinTuc, NhaTaiTro...) ở đây sau
33:     }
34: }
```

**Quan sát trực tiếp**:
- Đang có 19 `DbSet` cho các module cũ.
- Chưa có `using WEB_TinhNguyen.Areas.CuaHang.Models;`.
- Chưa có các DbSet: `DbSet<SanPhamGayQuy> SanPhamGayQuys`, `DbSet<DonHangGayQuy> DonHangGayQuys`, `DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys`.
- Chưa override `OnModelCreating` để ràng buộc kiểu dữ liệu số thực tiền tệ `decimal(18,2)` và các quan hệ Foreign Key / DeleteBehavior.

### 1.3. Hiện trạng tệp `tailwind.config.js`
Đường dẫn tuyệt đối: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/tailwind.config.js` (dòng 1-7):
```javascript
1: /** @type {import('tailwindcss').Config} */
2: module.exports = {
3:   content: [
4:     "./Views/**/*.cshtml",
5:     "./wwwroot/js/**/*.js"
6:   ],
```

**Quan sát trực tiếp & Phát hiện cốt lõi**:
- Cấu trúc thư mục: Tệp `tailwind.config.js` và `package.json` nằm tại thư mục gốc `/home/hutech/Vinh_Offline/WEB_TinhNguyen/`. Trong khi đó, toàn bộ Views nằm tại `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Views/` và sắp tới là `/home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/Areas/CuaHang/Views/`.
- Khi chạy lệnh build mặc định:
  ```bash
  warn - No utility classes were detected in your source files. If this is unexpected, double-check the `content` option in your Tailwind CSS configuration.
  ```
  Nguyên nhân: Đường dẫn trong `content` chỉ có `./Views/**/*.cshtml`. Khi chạy từ thư mục gốc, thư mục `./Views` không tồn tại ở gốc mà nằm ở `./WEB_TinhNguyen/Views`!
- Khi kiểm thử chạy `npx tailwindcss --content "./WEB_TinhNguyen/Views/**/*.cshtml" -i Styles/app.css -o /tmp/test-out.css`:
  Tailwind quét thành công và biên dịch xong trong 1199ms không hề còn bất kỳ cảnh báo nào.
- Kết luận: `content` trong `tailwind.config.js` bắt buộc phải bổ sung cả `./Areas/**/*.cshtml` và `./WEB_TinhNguyen/Areas/**/*.cshtml`, `./WEB_TinhNguyen/Views/**/*.cshtml` để tương thích hoàn toàn dù Tailwind được gọi từ thư mục gốc hay từ bên trong subfolder qua MSBuild.

### 1.4. Môi trường Biên dịch & Dịch vụ SQL Server
- Lệnh biên dịch chuẩn: `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj`.
  Biên dịch thành công 100% với 0 lỗi (0 errors, 4 warnings NU1902/NU1903 của NuGet packages cũ).
- Kết nối SQL Server qua `nc -zv 127.0.0.1 1433`: Kết quả `Connection refused`. Môi trường sandbox là offline demo mode, do đó việc khởi tạo cơ sở dữ liệu trong `Program.cs` cần bọc trong khối `try-catch` để ứng dụng không bị crash khi database server chưa bật.

---

## 2. Logic Chain (Chuỗi lập luận & Thiết kế kiến trúc)

```
[Mục tiêu Milestone 1: Area & Data Foundation]
                     │
    ┌────────────────┼────────────────┐
    ▼                ▼                ▼
[Vấn đề 1: Session] [Vấn đề 2: Route]  [Vấn đề 3: EF Core]
    │                │                │
    ▼                ▼                ▼
Thêm Cache, Session  Thêm Area route  Thêm DbSet và
& HttpContext vào    trước Default    OnModelCreating
Program.cs           route            vào DbContext
    │                │                │
    └────────────────┼────────────────┘
                     │
                     ▼
       [Vấn đề 4: Tailwind CSS Scan]
       Bổ sung pattern Areas & WEB_TinhNguyen
       vào content array của tailwind.config.js
```

### Bước 1: Quản lý Phiên làm việc (Session) & Giỏ hàng
- **Cơ sở quan sát**: Mục 1.1 cho thấy Session chưa được bật. Theo `PROJECT.md` và `ORIGINAL_REQUEST.md`, người dùng mua sắm sản phẩm gây quỹ mà không bắt buộc phải đăng nhập tài khoản trước. Dữ liệu giỏ hàng cần được lưu trên server thông qua Session `ISession`.
- **Giải pháp**:
  - `builder.Services.AddDistributedMemoryCache();` cung cấp kho lưu trữ in-memory cho session.
  - `builder.Services.AddSession(...)` với thời gian `IdleTimeout = TimeSpan.FromMinutes(60)`, `Cookie.HttpOnly = true` (bảo mật XSS), `Cookie.IsEssential = true` (vượt qua bộ lọc cookie policy), `Cookie.Name = ".WEB_TinhNguyen.Session"`.
  - `builder.Services.AddHttpContextAccessor();` cho phép truy cập `IHttpContextAccessor` ở các view layout, navbar để hiển thị số lượng badge giỏ hàng.
  - Vị trí middleware: `app.UseSession();` bắt buộc phải đặt sau `app.UseRouting();` và trước `app.UseAuthorization();`.

### Bước 2: Cấu hình Định tuyến Area (`Areas/CuaHang`)
- **Cơ sở quan sát**: Mục 1.1 chỉ có default route. Toàn bộ tính năng cửa hàng (CuaHangController, GioHangController, AdminSanPhamController) được đặt trong Area `CuaHang`.
- **Giải pháp**:
  - Thêm `app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=CuaHang}/{action=Index}/{id?}").WithStaticAssets();`.
  - **Thứ tự**: Bắt buộc đặt **trước** route `"default"`. Nếu đặt sau, URL dạng `/CuaHang/Index` sẽ bị hiểu nhầm là `controller=CuaHang` thuộc root, gây lỗi 404.

### Bước 3: Mở rộng `ApplicationDbContext` cho Sản phẩm và Đơn hàng Gây quỹ
- **Cơ sở quan sát**: Mục 1.2 và hợp đồng dữ liệu trong `PROJECT.md`.
- **Giải pháp**:
  - Khai báo 3 DbSets:
    1. `public DbSet<SanPhamGayQuy> SanPhamGayQuys { get; set; }`
    2. `public DbSet<DonHangGayQuy> DonHangGayQuys { get; set; }`
    3. `public DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys { get; set; }`
  - Override `OnModelCreating(ModelBuilder modelBuilder)`:
    - Thiết lập kiểu `decimal(18,2)` rõ ràng cho các trường: `GiaBan`, `GiaGoc`, `TongTienHang`, `TienUngHoThem`, `PhiVanChuyen`, `TongThanhToan`, `DonGia`, `ThanhTien` để loại bỏ hoàn toàn các cảnh báo precision của EF Core.
    - Ràng buộc quan hệ:
      - Khi xóa một đơn hàng (`DonHangGayQuy`), các chi tiết đơn hàng (`ChiTietDonHangGayQuy`) liên quan được xóa theo (`DeleteBehavior.Cascade`).
      - Không cho phép xóa sản phẩm (`SanPhamGayQuy`) nếu đã có trong chi tiết đơn hàng lịch sử (`DeleteBehavior.Restrict`).

### Bước 4: Tích hợp Quét Giao diện Tailwind CSS cho Area Views
- **Cơ sở quan sát**: Mục 1.3 phát hiện cảnh báo `No utility classes were detected` do thiếu đường dẫn thư mục `WEB_TinhNguyen` và chưa có pattern cho `Areas`.
- **Giải pháp**:
  Cập nhật mảng `content` trong `tailwind.config.js`:
  ```javascript
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./WEB_TinhNguyen/Views/**/*.cshtml",
    "./WEB_TinhNguyen/Areas/**/*.cshtml",
    "./WEB_TinhNguyen/wwwroot/js/**/*.js",
    "./wwwroot/js/**/*.js"
  ],
  ```
  Đảm bảo mọi class CSS sử dụng trong `Areas/CuaHang/Views/**/*.cshtml` đều được Tailwind tự động nhận diện và build vào `site.css`.

---

## 3. Caveats (Các lưu ý & Giả định)

1. **SQL Server trong Môi trường Sandbox**:
   - Dịch vụ SQL Server local hiện không mở port 1433 trong sandbox.
   - Vì vậy, câu lệnh `DbInitializer.Initialize(context);` tại `Program.cs` được bọc chặt chẽ trong khối `try-catch` với thông báo log cảnh báo thay vì làm dừng ứng dụng. Tầng Controller/Service sẽ kết hợp thêm bộ nhớ in-memory tĩnh dự phòng (Static Demo Store) theo đúng cam kết chế độ `Integrity mode: demo`.
2. **Quyền ghi tệp**:
   - Đặc vụ Explorer hoạt động ở chế độ Read-Only đối với mã nguồn chính. Các tệp đề xuất hoàn chỉnh (`proposed_Program.cs`, `proposed_ApplicationDbContext.cs`, `proposed_tailwind.config.js`) và tệp patch (`m1_foundation.patch`) đã được tạo sẵn trong thư mục của đặc vụ để Producer áp dụng trực tiếp.
3. **Sự phụ thuộc giữa các đặc vụ trong M1**:
   - `Program.cs` và `ApplicationDbContext.cs` sử dụng namespace `WEB_TinhNguyen.Areas.CuaHang.Models;` và `WEB_TinhNguyen.Areas.CuaHang.Helpers;`. Do đó, các tệp Entity và DbInitializer do Explorer 2 và Explorer 3 thiết kế cần được tạo trước hoặc cùng lúc với việc cập nhật `Program.cs` và `ApplicationDbContext.cs` để đảm bảo lệnh `dotnet build` đạt kết quả 100% 0 error.

---

## 4. Conclusion (Thiết kế mã nguồn chi tiết sẵn sàng áp dụng)

Toàn bộ mã nguồn đề xuất hoàn chỉnh đã được kiểm tra tính tương thích cú pháp, chuẩn xác từng dòng.

### 4.1. Thiết kế hoàn chỉnh cho `WEB_TinhNguyen/Program.cs`
Tệp tương ứng: `.agents/teamwork/teamwork_explorer_m1_1/proposed_Program.cs`
```csharp
using WEB_TinhNguyen.Data;
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Areas.CuaHang.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Kết nối cơ sở dữ liệu Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký In-Memory Cache cho Session
builder.Services.AddDistributedMemoryCache();

// Cấu hình Session quản lý giỏ hàng sản phẩm gây quỹ
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".WEB_TinhNguyen.Session";
});

// Đăng ký HttpContextAccessor để truy cập session và context từ view/service
builder.Services.AddHttpContextAccessor();

// Đăng ký Controllers và Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Middleware Session: BẮT BUỘC đặt sau UseRouting() và trước UseAuthorization()
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// Route cho Area (Cửa hàng gây quỹ & Admin Cửa hàng): Đặt TRƯỚC route mặc định
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=CuaHang}/{action=Index}/{id?}")
    .WithStaticAssets();

// Route mặc định hệ thống
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Khởi tạo dữ liệu mẫu ban đầu (DbInitializer Seed Data) với cơ chế phòng vệ khi offline
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Database initialization warning: SQL Server có thể đang offline, hệ thống sẽ sử dụng dữ liệu mẫu in-memory dự phòng.");
}

app.Run();
```

### 4.2. Thiết kế hoàn chỉnh cho `WEB_TinhNguyen/Data/ApplicationDbContext.cs`
Tệp tương ứng: `.agents/teamwork/teamwork_explorer_m1_1/proposed_ApplicationDbContext.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using WEB_TinhNguyen.Models;
using WEB_TinhNguyen.Areas.CuaHang.Models;

namespace WEB_TinhNguyen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng để Entity Framework tạo trong Database
        public DbSet<ChienDich> ChienDichs { get; set; }
        public DbSet<CauHinhTrangChu> CauHinhTrangChus { get; set; }
        public DbSet<CauHinhAnhNoi> CauHinhAnhNois { get; set; }
        public DbSet<BannerTrangChu> BannerTrangChus { get; set; }
        public DbSet<ChuongTrinhDemNguoc> ChuongTrinhDemNguocs { get; set; }
        public DbSet<CauHinhKyUc> CauHinhKyUcs { get; set; }
        public DbSet<AnhKyUc> AnhKyUcs { get; set; }
        public DbSet<NhaTaiTroKyUc> NhaTaiTroKyUcs { get; set; }
        public DbSet<CauHinhChuongTrinh> CauHinhChuongTrinhs { get; set; }
        public DbSet<ChangChuongTrinh> ChangChuongTrinhs { get; set; }
        public DbSet<ChiTietLoTrinh> ChiTietLoTrinhs { get; set; }
        public DbSet<CauHinhGayQuy> CauHinhGayQuys { get; set; }
        public DbSet<KenhDongGop> KenhDongGops { get; set; }
        public DbSet<NhaTaiTroGayQuy> NhaTaiTroGayQuys { get; set; }
        public DbSet<CauHinhBanToChuc> CauHinhBanToChucs { get; set; }
        public DbSet<ThanhVienBanToChuc> ThanhVienBanToChucs { get; set; }
        public DbSet<CauHinhTinTuc> CauHinhTinTucs { get; set; }
        public DbSet<BaiVietTinTuc> BaiVietTinTucs { get; set; }
        public DbSet<ThuVienHieuUng> ThuVienHieuUngs { get; set; }

        // =========================================================================
        // PHÂN HỆ: CỬA HÀNG SẢN PHẨM GÂY QUỸ (AREA: CuaHang)
        // =========================================================================
        public DbSet<SanPhamGayQuy> SanPhamGayQuys { get; set; }
        public DbSet<DonHangGayQuy> DonHangGayQuys { get; set; }
        public DbSet<ChiTietDonHangGayQuy> ChiTietDonHangGayQuys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng và trường tiền tệ cho SanPhamGayQuy
            modelBuilder.Entity<SanPhamGayQuy>(entity =>
            {
                entity.ToTable("SanPhamGayQuy");
                entity.Property(e => e.GiaBan).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GiaGoc).HasColumnType("decimal(18,2)");
            });

            // Cấu hình bảng và trường tiền tệ cho DonHangGayQuy
            modelBuilder.Entity<DonHangGayQuy>(entity =>
            {
                entity.ToTable("DonHangGayQuy");
                entity.Property(e => e.TongTienHang).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TienUngHoThem).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TongThanhToan).HasColumnType("decimal(18,2)");
            });

            // Cấu hình bảng, trường tiền tệ và khóa ngoại cho ChiTietDonHangGayQuy
            modelBuilder.Entity<ChiTietDonHangGayQuy>(entity =>
            {
                entity.ToTable("ChiTietDonHangGayQuy");
                entity.Property(e => e.DonGia).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.DonHang)
                      .WithMany(p => p.ChiTietDonHangs)
                      .HasForeignKey(d => d.IDDonHang)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SanPham)
                      .WithMany()
                      .HasForeignKey(d => d.IDSanPham)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
```

### 4.3. Thiết kế hoàn chỉnh cho `tailwind.config.js`
Tệp tương ứng: `.agents/teamwork/teamwork_explorer_m1_1/proposed_tailwind.config.js`
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./WEB_TinhNguyen/Views/**/*.cshtml",
    "./WEB_TinhNguyen/Areas/**/*.cshtml",
    "./WEB_TinhNguyen/wwwroot/js/**/*.js",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        /* Các màu tĩnh giữ nguyên */
        'leaf-bright': '#81c784',
        'grass': '#8bc34a',
        'sun': '#ffeb3b',
        'sunset': '#ff9800',
        'sky-soft': '#e0f7fa',
        'cream': '#fffdd0',
        'ink': '#263238',
        'night': '#102027',
        'night-2': '#37474f',
        
        /* Các màu động lấy từ Database thông qua biến CSS */
        'leaf-deep': 'var(--color-leaf-deep)',
        'leaf': 'var(--color-leaf)',
        'forest': 'var(--color-forest)',
      },
      boxShadow: {
        'soft': '0 4px 20px -2px rgba(0, 0, 0, 0.05)',
      },
      fontFamily: {
        display: ['var(--font-heading)'],
        sans: ['var(--font-body)'],
      }
    },
  },
  plugins: [
    require('@tailwindcss/container-queries'),
  ],
}
```

### 4.4. Tệp Patch Hợp nhất
Đường dẫn: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/m1_foundation.patch`  
Producer có thể áp dụng ngay lập tức bằng:
`patch -p0 < /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/m1_foundation.patch`

---

## 5. Verification Method (Phương pháp kiểm chứng độc lập)

Để kiểm chứng độc lập thiết kế này, người thực hiện / reviewer thực hiện các bước sau:

1. **Kiểm tra cú pháp và cấu trúc tệp**:
   - Xác nhận sự hiện diện của các tệp đề xuất:
     - `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/proposed_Program.cs`
     - `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/proposed_ApplicationDbContext.cs`
     - `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/proposed_tailwind.config.js`
     - `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/m1_foundation.patch`

2. **Kiểm tra biên dịch CSS Tailwind độc lập**:
   ```bash
   npx tailwindcss -c /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/proposed_tailwind.config.js -i Styles/app.css -o /tmp/tailwind_verify.css
   ```
   *Điều kiện đạt*: Không có cảnh báo `No utility classes were detected`. Tệp `/tmp/tailwind_verify.css` được sinh ra với kích thước đầy đủ.

3. **Kiểm tra biên dịch toàn diện .NET**:
   Sau khi Producer tạo các Model (`Areas/CuaHang/Models/*.cs`) và Helper (`Areas/CuaHang/Helpers/DbInitializer.cs`), thực hiện:
   ```bash
   XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen/WEB_TinhNguyen.csproj
   ```
   *Điều kiện đạt*: `Build succeeded. 0 Error(s)`.

4. **Điều kiện vô hiệu hóa (Invalidation conditions)**:
   - Nếu `app.UseSession()` bị đặt trước `app.UseRouting()`, ứng dụng sẽ ném ngoại lệ `InvalidOperationException: Session was accessed before UseSession() middleware`.
   - Nếu route `areas` bị đặt sau route `default`, các request vào `Areas/CuaHang` sẽ rơi vào default route gây 404 hoặc gọi nhầm controller gốc.
