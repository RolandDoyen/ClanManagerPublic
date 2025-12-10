using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly ILogger<T> _logger;
        protected readonly ISessionService _sessionService;

        public BaseController(ILogger<T> logger, ISessionService sessionService)
        {
            _logger = logger;
            _sessionService = sessionService;
        }

        protected IActionResult HandleException<TModel>(Exception ex, string message, TModel model, string userMessage, params object[] args)
        {
            _logger.LogError(ex, message, args);

            if (!string.IsNullOrEmpty(userMessage))
            {
                ModelState.AddModelError(string.Empty, userMessage);
            }

            return View(model);
        }
    }
}
