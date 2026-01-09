using ClanManager.BLL.Services;
using ClanManager.Core.Resources;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    /// <summary>
    /// Controller providing administrative and maintenance tools, such as database management and system resets.
    /// </summary>
    public class AdminController : BaseController<HomeController>
    {
        private readonly IDatabaseService _databaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/>.
        /// </summary>
        /// <param name="databaseService">The service responsible for low-level database maintenance operations.</param>
        /// <param name="logger">The logging provider for tracking administrative actions.</param>
        /// <param name="sessionService">The service used to manage and clear sessions during system resets.</param>
        public AdminController(IDatabaseService databaseService, ILogger<HomeController> logger, ISessionService sessionService) : base(logger, sessionService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Performs a complete reset of the database and clears the current user's session to ensure data consistency.
        /// </summary>
        /// <returns>A redirect to the Home page with a success or error message in TempData.</returns>
        /// <remarks>This operation is destructive and should typically be protected by high-level authorization filters.</remarks>
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Admin_ResetDB;
                _logger.LogError(ex, "An error occurred while resetting the database.");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}