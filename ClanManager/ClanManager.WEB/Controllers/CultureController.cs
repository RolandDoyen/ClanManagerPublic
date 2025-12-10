using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    public class CultureController : Controller
    {
        private readonly string[] _supportedCultures = new[] { "en", "fr" };

        public IActionResult SetCulture(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(culture) || !_supportedCultures.Contains(culture))
            {
                culture = "en";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
