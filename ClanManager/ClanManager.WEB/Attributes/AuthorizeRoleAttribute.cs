using ClanManager.Core;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Custom authorization attribute that restricts access to actions based on allowed <see cref="Role"/> values.
/// This filter intercepts the request before the action executes to verify session-based permissions.
/// </summary>
public class AuthorizeRoleAttribute : Attribute, IAsyncActionFilter
{
    private readonly Role[] _roles;

    /// <summary>
    /// Initializes the attribute with one or more roles authorized to access the decorated resource.
    /// </summary>
    /// <param name="roles">The list of <see cref="Role"/> allowed to pass the filter.</param>
    public AuthorizeRoleAttribute(params Role[] roles)
    {
        _roles = roles;
    }

    /// <summary>
    /// Core logic that resolves the session service and evaluates if the current user has the required privileges.
    /// </summary>
    /// <param name="context">The context for the action being executed.</param>
    /// <param name="next">The delegate to call to continue the execution pipeline.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();

        var userRole = sessionService.GetUserRole();

        if (userRole == null || !_roles.Contains(userRole.Value))
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizeRoleAttribute>>();
            logger?.LogWarning("User {UserEmail} tried to access unauthorized function", sessionService.GetUserEmail() ?? "[No email]");

            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}