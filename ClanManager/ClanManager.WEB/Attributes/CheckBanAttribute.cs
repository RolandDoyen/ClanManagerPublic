using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClanManager.WEB.Attributes
{
    /// <summary>
    /// Action filter that prevents banned users from accessing specific controller actions.
    /// Redirects restricted users to the home page to maintain application security.
    /// </summary>
    public class CheckBanAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Intercepts the request before the action execution to verify the user's ban status from the current session.
        /// </summary>
        /// <param name="context">The context for the action being filtered.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();

            if (sessionService.IsUserBanned())
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<CheckBanAttribute>>();
                logger?.LogInformation("Banned user tried to access {Path}", context.HttpContext.Request.Path);

                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
