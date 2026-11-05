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
