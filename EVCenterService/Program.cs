using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Repository.Repositories;
using EVCenterService.Service.Interfaces;
using EVCenterService.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<PasswordHasherService>();

builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddScoped<ICustomerBookingRepository, CustomerBookingRepository>();
builder.Services.AddScoped<ICustomerBookingService, CustomerBookingService>();

builder.Services.AddScoped<IStaffAppointmentRepository, StaffAppointmentRepository>();
builder.Services.AddScoped<IStaffAppointmentService, StaffAppointmentService>();

builder.Services.AddScoped<ITechnicianJobRepository, TechnicianJobRepository>();
builder.Services.AddScoped<ITechnicianJobService, TechnicianJobService>();

builder.Services.AddScoped<IServiceCatalogRepository, ServiceCatalogRepository>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();

builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

builder.Services.AddScoped<IAdminEmployeeRepository, AdminEmployeeRepository>();
builder.Services.AddScoped<IAdminEmployeeService, AdminEmployeeService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMemoryCache();

// --- ??NG KÝ L?I EmailSender (dùng Mailjet API) ---
builder.Services.AddTransient<IEmailSender, EmailSender>();

// DI for migrationcd EVCenterService
builder.Services.AddDbContext<EVServiceCenterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // ??t Cookie là scheme m?c ??nh
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    })
    .AddGoogle(googleOptions => // Thêm Google
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();