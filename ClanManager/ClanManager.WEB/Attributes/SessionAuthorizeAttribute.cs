using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly bool _allowAnonymous;

    public SessionAuthorizeAttribute(bool allowAnonymous = false)
    {
        _allowAnonymous = allowAnonymous;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();
        var userId = sessionService.GetUserId();

        if (_allowAnonymous)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var logger = context.HttpContext.RequestServices
                    .GetService<ILogger<SessionAuthorizeAttribute>>();
                logger?.LogInformation("Connected user tried to access {Path}", context.HttpContext.Request.Path);

                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(userId))
            {
                var logger = context.HttpContext.RequestServices
                    .GetService<ILogger<SessionAuthorizeAttribute>>();
                logger?.LogInformation("Anonymous user tried to access {Path}", context.HttpContext.Request.Path);

                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
