using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClanManager.WEB.Attributes
{
    public class CheckBanAttribute : ActionFilterAttribute
    {
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
