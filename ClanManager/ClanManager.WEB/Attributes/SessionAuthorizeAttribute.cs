using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Custom authorization filter that manages access based on the presence or absence of a user session.
/// Can be configured to either require a login or, conversely, to restrict access to non-authenticated users only.
/// </summary>
public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly bool _allowAnonymous;

    /// <summary>
    /// Initializes the attribute. 
    /// If <paramref name="allowAnonymous"/> is true, only non-connected users can access the action.
    /// If false (default), only connected users with a valid session ID can access it.
    /// </summary>
    /// <param name="allowAnonymous">Flag to invert the authorization logic for guest-only pages.</param>
    public SessionAuthorizeAttribute(bool allowAnonymous = false)
    {
        _allowAnonymous = allowAnonymous;
    }

    /// <summary>
    /// Evaluates the session state against the security requirements before the action executes.
    /// </summary>
    /// <param name="context">The context for the action being filtered.</param>
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
