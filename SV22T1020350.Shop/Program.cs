using Microsoft.Extensions.FileProviders;
var builder = WebApplication.CreateBuilder(args);

// =======================================================
// ĐOẠN NÀY ĐÃ ĐƯỢC THÊM ĐỂ SỬA LỖI DATABASE:
// Khởi tạo cấu hình cho BusinessLayer kết nối SQL Server
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
SV22T1020350.BusinessLayers.Configuration.Initialize(connectionString);
// =======================================================

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("CustomerCookie")
    .AddCookie("CustomerCookie", option =>
    {
        option.Cookie.Name = "SV22T1020350.Shop.Cookie";
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
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

// Thêm vào sau app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// ==========================================================
// BỔ SUNG ĐOẠN CODE NÀY ĐỂ KẾT NỐI SANG THƯ MỤC ẢNH CỦA ADMIN
// Giúp Shop đọc được ảnh do Admin upload
// ==========================================================
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "SV22T1020350.Admin", "wwwroot", "images")),
    RequestPath = new PathString("/images")
});
// ==========================================================

app.MapStaticAssets(); // Map static assets của project Shop

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();