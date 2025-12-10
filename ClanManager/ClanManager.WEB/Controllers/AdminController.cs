using ClanManager.BLL.Services;
using ClanManager.Core.Resources;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    public class AdminController : BaseController<HomeController>
    {
        private readonly IDatabaseService _databaseService;

        public AdminController(IDatabaseService databaseService, ILogger<HomeController> logger, ISessionService sessionService) : base(logger, sessionService)
        {
            _databaseService = databaseService;
        }

        [HttpPost]
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                if (!string.IsNullOrEmpty(_sessionService.GetUserId()))
                {
                    _sessionService.ClearSession();
                }

                await _databaseService.ResetDatabaseAsync();

                TempData["SuccessMessage"] = Resources.Success_Admin_ResetDB;
                _logger.LogInformation("Database has been reset.");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Admin_ResetDB;
                _logger.LogError(ex, "An error occurred while resetting the database.");

                return RedirectToAction("Index", "Home");
            }
        }
    }
}