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
    }
}
