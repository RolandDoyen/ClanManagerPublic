using ClanManager.WEB.Models;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClanManager.WEB.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, ISessionService sessionService) : base(logger, sessionService) { }

        public IActionResult Index() => View();

        public IActionResult Error404()
        {
            Response.StatusCode = 404;

            _logger.LogWarning("404 Error on path {Path}", HttpContext.Request.Path);

            return View();
        }

        public IActionResult Error500()
        {
            Response.StatusCode = 500;

            _logger.LogError("500 Internal Server Error on path {Path}", HttpContext.Request.Path);

            return View();
        }

        [Route("Error/{statusCode?}")]
        public IActionResult Error(int? statusCode = null)
        {
            Response.StatusCode = statusCode ?? 500;

            _logger.LogWarning("HTTP {StatusCode} error on {Path}", statusCode ?? 500, HttpContext.Request.Path);

            return statusCode switch
            {
                404 => View("Error404"),
                500 => View("Error500"),
                _ => View("Error", new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            };
        }
    }
}
