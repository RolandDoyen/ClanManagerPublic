using ClanManager.WEB.Models;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClanManager.WEB.Controllers
{
    /// <summary>
    /// Core navigation controller managing the landing page and centralized HTTP error redirection logic.
    /// </summary>
    public class HomeController : BaseController<HomeController>
    {
        /// <summary>
        /// Initializes the home controller with logging and session capabilities inherited from BaseController.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, ISessionService sessionService) : base(logger, sessionService) { }

        /// <summary>
        /// Renders the application's main landing page.
        /// </summary>
        public IActionResult Index() => View();

        /// <summary>
        /// Explicitly handles 404 Not Found results, setting the response status and logging the missing path.
        /// </summary>
        public IActionResult Error404()
        {
            Response.StatusCode = 404;

            _logger.LogWarning("404 Error on path {Path}", HttpContext.Request.Path);

            return View();
        }

        /// <summary>
        /// Explicitly handles 500 Internal Server Error results, logging the critical failure for diagnostic purposes.
        /// </summary>
        public IActionResult Error500()
        {
            Response.StatusCode = 500;

            _logger.LogError("500 Internal Server Error on path {Path}", HttpContext.Request.Path);

            return View();
        }

        /// <summary>
        /// Catch-all error action that routes to specific views based on the provided HTTP status code.
        /// </summary>
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
