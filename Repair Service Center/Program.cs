using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Repair_Service_Center;
using Repair_Service_Center.Data;
using Repair_Service_Center.Helpers;
using Repair_Service_Center.Models;

var builder = WebApplication.CreateBuilder(args);

// ----- Laboratory work 8: localization (English and Ukrainian) -----
// The .resx files with translations are in the folder Resources
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container
builder.Services.AddControllersWithViews()
    // IViewLocalizer in views + views for one language (Privacy.uk.cshtml)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    // [Display] names and error messages of the attributes are translated
    // with one shared file Resources/SharedResource.uk.resx
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)));

// Messages of the model binding ("The value 'abc' is not valid for Price") are translated too
builder.Services.AddOptions<MvcOptions>()
    .Configure<IStringLocalizerFactory>((options, factory) =>
    {
        var localizer = factory.Create(typeof(SharedResource));
        var messages = options.ModelBindingMessageProvider;
        messages.SetAttemptedValueIsInvalidAccessor((value, field) =>
            localizer["The value '{0}' is not valid for {1}.", value, field].Value);
        messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value =>
            localizer["The value '{0}' is not valid.", value].Value);
        messages.SetValueIsInvalidAccessor(value => localizer["The value '{0}' is invalid.", value].Value);
        messages.SetValueMustNotBeNullAccessor(value => localizer["The value '{0}' is invalid.", value].Value);
        messages.SetUnknownValueIsInvalidAccessor(field => localizer["The value is not valid for {0}.", field].Value);
        messages.SetValueMustBeANumberAccessor(field => localizer["The field {0} must be a number.", field].Value);
        messages.SetNonPropertyValueMustBeANumberAccessor(() => localizer["The field must be a number."].Value);
        messages.SetMissingKeyOrValueAccessor(() => localizer["A value is required."].Value);
    });

// Supported languages and the ways to choose the language
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(AppCultures.Supported[0]);
    options.SupportedCultures = AppCultures.Supported.ToList();     // dates, numbers
    options.SupportedUICultures = AppCultures.Supported.ToList();   // texts from .resx
    // 1) ?culture=uk-UA in the address (handy for testing); 2) the cookie of the language switcher.
    // The language of the browser is not used, so the site opens in English until the user chooses.
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider { Options = options },
        new CookieRequestCultureProvider { Options = options }
    };
});

// Database context
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
    // Laboratory work 8: messages of Identity (password rules, taken e-mail) in two languages
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
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

// Data that depends on the current date
using (var scope = app.Services.CreateScope())
{
    SeedData.Initialize(scope.ServiceProvider);
    // Roles and demo accounts (admin, moderator, customer)
    await IdentitySeed.InitializeAsync(scope.ServiceProvider);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Laboratory work 8: the culture of every request is chosen here (options are set above).
// Before LR8 the site had one culture (en-US); now it is en-US or uk-UA.
app.UseRequestLocalization();

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
