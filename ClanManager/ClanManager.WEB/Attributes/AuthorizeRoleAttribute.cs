using ClanManager.Core;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeRoleAttribute : Attribute, IAsyncActionFilter
{
    private readonly Role[] _roles;

    public AuthorizeRoleAttribute(params Role[] roles)
    {
        _roles = roles;
    }

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