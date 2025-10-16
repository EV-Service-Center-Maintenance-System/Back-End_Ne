using EVCenterService.Data; // Ho?c namespace c?a Context c?a b?n
using EVCenterService.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ??ng ký PasswordHasher và JwtService (n?u b?n v?n c?n JwtService cho m?c ?ích khác)
builder.Services.AddScoped<PasswordHasherService>();

// DI for migration
builder.Services.AddDbContext<EVServiceCenterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// *** THAY TH? C?U HÌNH JWT B?NG COOKIE AUTHENTICATION ***
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";      // Trang chuy?n ??n khi ch?a ??ng nh?p
        options.LogoutPath = "/Account/Logout";     // ???ng d?n ?? ??ng xu?t
        options.AccessDeniedPath = "/AccessDenied"; // Trang báo l?i khi không có quy?n
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// B?t Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// Xóa MapControllers() n?u b?n không dùng API Controller
// app.MapControllers(); 

app.MapRazorPages();

app.Run();