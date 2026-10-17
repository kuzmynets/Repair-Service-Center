using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Registration, login, logout and profile.
/// The whole controller needs a logged-in user ([Authorize]),
/// but registration, login and the "access denied" page are open for guests ([AllowAnonymous]).
/// </summary>
[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    // Laboratory work 8: messages of the controller in the language of the user
    private readonly IStringLocalizer<SharedResource> _localizer;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FullName = $"{model.FirstName.Trim()} {model.LastName.Trim()}",
            PhoneNumber = model.PhoneNumber.Trim(),
            Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim()
        };

        // UserManager checks the password rules and saves the hash of the password
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // Every new user is a customer; other roles are given by the administrator
            await _userManager.AddToRoleAsync(user, AppRoles.Customer);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(SafeUrl(returnUrl));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    // GET: /Account/Login?returnUrl=/RepairRequest/Create
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // After 5 wrong passwords the account is locked for 5 minutes (see Program.cs)
        var result = await _signInManager.PasswordSignInAsync(model.Email.Trim(), model.Password,
            model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(SafeUrl(returnUrl));
        }

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? _localizer["Too many failed attempts. Please try again in 5 minutes."].Value
            : _localizer["Wrong e-mail or password."].Value);
        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/AccessDenied - the user is logged in, but his role does not allow the page
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // GET: /Account/Profile
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (firstName, lastName) = SplitName(user.FullName);
        return View(new ProfileViewModel
        {
            Email = user.Email ?? string.Empty,
            // Laboratory work 8: the names of the roles are translated (Customer -> Клієнт)
            Role = string.Join(", ", roles.Select(r => _localizer[r].Value)),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Address = user.Address
        });
    }

    // POST: /Account/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Email = user.Email ?? string.Empty;
            model.Role = string.Join(", ", (await _userManager.GetRolesAsync(user)).Select(r => _localizer[r].Value));
            return View(model);
        }

        user.FullName = $"{model.FirstName.Trim()} {model.LastName.Trim()}";
        user.PhoneNumber = model.PhoneNumber.Trim();
        user.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim();
        await _userManager.UpdateAsync(user);

        // The new name must appear in the menu, so the login cookie is created again
        await _signInManager.RefreshSignInAsync(user);

        TempData["Message"] = _localizer["Your profile is saved."].Value;
        return RedirectToAction(nameof(Profile));
    }

    // "Anna Shevchenko" -> ("Anna", "Shevchenko"); the full name is stored in one field
    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[1])
        };
    }

    // Only addresses of this site are allowed after login (protection from open redirect)
    private string SafeUrl(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
}
