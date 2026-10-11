using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Helpers;
using Repair_Service_Center.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database context (connection string is in appsettings.json)
builder.Services.AddDbContext<RepairServiceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RepairServiceContext")
        ?? throw new InvalidOperationException("Connection string 'RepairServiceContext' not found.")));

// ----- Laboratory work 6: users and roles (ASP.NET Core Identity) -----
// AddIdentity<TUser, TRole> registers the services of users AND roles
// (the same as AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>() in the methodical example)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<RepairServiceContext>()
    .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

// Where to send a user who is not logged in (login page) or has no rights (access denied page)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Policies: a requirement for roles is described once and then used by its name
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.RequireAdmin, policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy(AppPolicies.CanModerateReviews,
        policy => policy.RequireRole(AppRoles.Admin, AppRoles.Moderator));
});

var app = builder.Build();

// Data that depends on the current date (time slots for the next week)
using (var scope = app.Services.CreateScope())
{
    SeedData.Initialize(scope.ServiceProvider);
    // Roles and demo accounts (admin, moderator, customer)
    await IdentitySeed.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// One culture for the whole site: numbers use a dot (12.50), dates are parsed the same way
var culture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

app.UseHttpsRedirection();
app.UseRouting();

// First "who are you?" (authentication), then "what can you do?" (authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
