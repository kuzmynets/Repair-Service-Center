using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Repair_Service_Center.Helpers;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Laboratory work 8. Switching the language of the interface.
/// The chosen culture is saved in the cookie ".AspNetCore.Culture" for one year;
/// CookieRequestCultureProvider reads it on every next request.
/// </summary>
public class LanguageController : Controller
{
    // POST: /Language/Set  (culture = "en-US" or "uk-UA")
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Set(string culture, string? returnUrl)
    {
        if (!AppCultures.IsSupported(culture))
        {
            culture = AppCultures.English;
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/"
            });

        // Back to the same page; only addresses of this site are allowed (open redirect protection)
        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }
}
