using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Exceptions;
using ClanManager.Core.Resources;
using ClanManager.WEB.Attributes;
using ClanManager.WEB.Models.Clan;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    /// <summary>
    /// Controller handling all clan-related interactions, including lifecycle management, 
    /// membership, and administrative actions. Protected by ban checks.
    /// </summary>
    [CheckBan]
    public class ClanController : BaseController<ClanController>
    {
        private readonly IClanBLL _clanBLL;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the ClanController with required business logic and mapping services.
        /// </summary>
        public ClanController(IClanBLL clanBLL, IMapper mapper, ILogger<ClanController> logger, ISessionService sessionService) : base(logger, sessionService)
        {
            _clanBLL = clanBLL;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves and displays a list of clans with optional filtering by status or ownership.
        /// </summary>
        /// <param name="activeOnly">If true, filters out inactive clans.</param>
        /// <param name="myClansOnly">If true, only displays clans where the current user is a member.</param>
        [HttpGet]
        public async Task<IActionResult> List(bool activeOnly = true, bool myClansOnly = false)
        {
            var clanDTOs = await _clanBLL.GetAllAsync(activeOnly, myClansOnly, _sessionService.GetUserId());

            var model = new ClanListViewModel
            {
                Clans = _mapper.Map<List<ClanViewModel>>(clanDTOs),
                ActiveOnly = activeOnly,
                MyClansOnly = myClansOnly,
                IsLogedIn = !string.IsNullOrEmpty(_sessionService.GetUserId())
            };

            return View(model);
        }

        /// <summary>
        /// Displays the clan creation form. Requires an active user session.
        /// </summary>
        [HttpGet]
        [SessionAuthorize]
        public IActionResult Create() => View();

        /// <summary>
        /// Processes the creation of a new clan and redirects to its detail page.
        /// </summary>
        /// <param name="model">The clan data submitted via the form.</param>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Create(ClanViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var newClan = await _clanBLL.CreateAsync(_mapper.Map<ClanDTO>(model), _sessionService.GetUserId());

                _logger.LogInformation("New Clan created with Id : {Id}", newClan.Id);

                TempData["SuccessMessage"] = Resources.Success_Clan_Create_Controller;

                return RedirectToAction("Detail", new { id = newClan.Id });
            }
            catch (ClanAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot create Clan with Name : {name} and Tag : {tag}. Clan already exists.", model.Name, model.Tag);

                return View(model);
            }
        }

        /// <summary>
        /// Displays detailed information about a specific clan, including its members.
        /// </summary>
        /// <param name="id">The unique identifier of the clan.</param>
        /// <param name="editDescription">If true, renders the description field in edit mode.</param>
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id, bool editDescription = false)
        {
            var clanUserContextDTO = await _clanBLL.GetByIdAsync(id, _sessionService.GetUserId(), _sessionService.GetUserRole());
            var model = _mapper.Map<ClanViewModel>(clanUserContextDTO);
            model.Members = _mapper.Map<List<ClanMemberViewModel>>(clanUserContextDTO.MembersContext);
            model.IsEditingDescription = editDescription;

            return View(model);
        }

        /// <summary>
        /// Cancels the description editing process and returns to the standard detail view.
        /// </summary>
        [HttpPost]
        [SessionAuthorize]
        public IActionResult CancelDescriptionEdit(Guid id) => RedirectToAction("Detail", new { id });

        /// <summary>
        /// Updates the description of a clan. Restricted to members with sufficient permissions.
        /// </summary>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> UpdateDescription(Guid id, string description)
        {
            await _clanBLL.UpdateDescriptionAsync(id, description, _sessionService.GetUserId());

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Toggles the active status of a clan. Restricted to SuperAdmins and Admins.
        /// </summary>
        [HttpPost]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            await _clanBLL.ToggleActiveAsync(id, _sessionService.GetUserId());

            TempData["SuccessMessage"] = Resources.Success_Clan_ToggleActive_Controller;

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Allows the current user to join a specific clan if rules permit.
        /// </summary>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Join(Guid id)
        {
            await _clanBLL.JoinAsync(id, _sessionService.GetUserId());

            TempData["SuccessMessage"] = Resources.Success_Clan_Join_Controller;

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Removes a member from a clan or allows a user to leave.
        /// </summary>
        /// <param name="id">Clan ID.</param>
        /// <param name="targetUserId">User ID to remove (null if leaving self).</param>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> RemoveMember(Guid id, Guid? targetUserId)
        {
            await _clanBLL.RemoveMemberAsync(id, _sessionService.GetUserId(), targetUserId);

            if (id == targetUserId)
            {
                TempData["SuccessMessage"] = Resources.Success_Clan_RemoveSelf_Controller;

                return RedirectToAction("List");
            }

            TempData["SuccessMessage"] = Resources.Success_Clan_RemoveMember_Controller;

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Deletes a clan entirely. Only permitted for the Clan Leader.
        /// </summary>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _clanBLL.DeleteAsync(id, _sessionService.GetUserId());

            TempData["SuccessMessage"] = Resources.Success_Clan_Delete_Controller;

            return RedirectToAction("List");
        }

        /// <summary>
        /// Changes the internal role of a clan member (e.g., promoting to Moderator).
        /// </summary>
        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> ChangeMemberRole(Guid id, Guid targetUserId, ClanRole newRole)
        {
            await _clanBLL.ChangeMemberRole(id, _sessionService.GetUserId(), targetUserId, newRole);

            TempData["SuccessMessage"] = Resources.Success_Clan_ChangeMemberRole_Controller;

            return RedirectToAction("Detail", new { id });
        }
    }
}