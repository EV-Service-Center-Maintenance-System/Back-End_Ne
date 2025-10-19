using EVCenterService.Data;
using EVCenterService.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ??ng ký PasswordHasher và JwtService (n?u b?n v?n c?n JwtService cho m?c ?ích khác)
builder.Services.AddScoped<PasswordHasherService>();

// DI for migrationcd EVCenterService
builder.Services.AddDbContext<EVServiceCenterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";     
        options.LogoutPath = "/Account/Logout";   
        options.AccessDeniedPath = "/AccessDenied";
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