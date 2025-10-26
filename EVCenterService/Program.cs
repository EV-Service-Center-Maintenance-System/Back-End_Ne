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

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMemoryCache();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();