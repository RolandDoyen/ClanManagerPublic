using ClanManager.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ClanManager.WEB.Middleware
{
    /// <summary>
    /// Global exception interceptor that standardizes error handling across the application.
    /// Redirects all failures to the Clan List to ensure a consistent user fallback path.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        /// <summary>
        /// Initializes the filter with logging and TempData access services.
        /// </summary>
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, ITempDataDictionaryFactory tempDataFactory)
        {
            _logger = logger;
            _tempDataFactory = tempDataFactory;
        }

        /// <summary>
        /// Handles exceptions by logging them, storing the message in TempData, 
        /// and redirecting the user to the central Clan List view.
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            var action = context.RouteData.Values["action"]?.ToString();

            if (exception is ClanManagerBaseException)
                _logger.LogWarning(exception, "Business logic exception in {Action}: {Message}", action, exception.Message);
            else
                _logger.LogError(exception, "Unhandled technical exception in {Action}", action);

            tempData["ErrorMessage"] = exception.Message;

            context.Result = new RedirectToActionResult("List", "Clan", null);

            context.ExceptionHandled = true;
        }
    }
}