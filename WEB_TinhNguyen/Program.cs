using WEB_TinhNguyen.DuLieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
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

// Cấu hình Razor ViewEngine 2 tầng hỗ trợ cấu trúc thư mục Việt hóa (/GiaoDien/)
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // Định dạng View cho phân hệ MVC gốc (Root)
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/GiaoDien/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/GiaoDien/Shared/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");       // Fallback an toàn
    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");   // Fallback an toàn

    // Định dạng View cho Phân hệ (Areas: CuaHang)
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/GiaoDien/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/GiaoDien/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/GiaoDien/Shared/{0}.cshtml"); // Dùng chung layout từ GiaoDien/Shared
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

    // Đăng ký bộ mở rộng đường dẫn phân giải Razor View
    options.ViewLocationExpanders.Add(new VietnameseViewLocationExpander());
});

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

// Middleware Session: Dat sau UseRouting va truoc phan quyen
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
    if (context.Database.CanConnect())
    {
        try
        {
            context.Database.Migrate();
        }
        catch (Exception migrateEx)
        {
            app.Logger.LogWarning(migrateEx, "Bỏ qua bước tự động Migrate do cơ sở dữ liệu vật lý đã tồn tại trước đó.");
        }
    }
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Database initialization warning: SQL Server có thể đang offline, hệ thống sẽ sử dụng dữ liệu mẫu in-memory dự phòng.");
}

app.Run();
