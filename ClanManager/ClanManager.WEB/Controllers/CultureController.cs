using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    /// <summary>
    /// Controller responsible for updating the user's preferred language by storing the culture in a persistent cookie.
    /// </summary>
    public class CultureController : Controller
    {
        private readonly string[] _supportedCultures = { "en", "fr" };

        /// <summary> Updates the ASP.NET Core culture cookie and redirects the user back to their previous page. </summary>
        /// <param name="culture"> The target culture code (e.g., "en", "fr"). Defaults to "en" if invalid or unsupported. </param>
        /// <param name="returnUrl"> The URL to redirect to after setting the cookie, ensuring a seamless user experience. </param>
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
