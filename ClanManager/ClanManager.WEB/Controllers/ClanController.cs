using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Exceptions;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Resources;
using ClanManager.WEB.Attributes;
using ClanManager.WEB.Models.Clan;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    [CheckBan]
    public class ClanController : BaseController<ClanController>
    {
        private readonly IClanBLL _clanBLL;
        private readonly IMapper _mapper;

        public ClanController(IClanBLL clanBLL, IMapper mapper, ILogger<ClanController> logger, ISessionService sessionService) : base(logger, sessionService)
        {
            _clanBLL = clanBLL;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> List(bool activeOnly = true, bool myClansOnly = false)
        {
            try
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
            catch (Exception ex)
            {
                return HandleException(ex, "Clan List error", new ClanListViewModel(), Resources.Error_Clan_List_Controller);
            }
        }

        [HttpGet]
        [SessionAuthorize]
        public IActionResult Create() => View();

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
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot create Clan with Name : {name} and Tag : {tag}. No User in Session.", model.Name, model.Tag);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot create Clan with Name : {name} and Tag : {tag}. Session User not found.", model.Name, model.Tag);
            }
            catch (ClanAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot create Clan with Name : {name} and Tag : {tag}. Clan already exists.", model.Name, model.Tag);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Clan Create Error for Clan : {Name}", model, Resources.Error_Clan_Create_Controller, model.Name);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id, bool editDescription = false)
        {
            try
            {
                var clanUserContextDTO = await _clanBLL.GetByIdAsync(id, _sessionService.GetUserId(), _sessionService.GetUserRole());
                var model = _mapper.Map<ClanViewModel>(clanUserContextDTO);
                model.Members = _mapper.Map<List<ClanMemberViewModel>>(clanUserContextDTO.MembersContext);
                model.IsEditingDescription = editDescription;

                return View(model);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Clan with Id : {id} not found for Detail view.", id);

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Clan Detail error for Clan with Id : {Id}", new ClanListViewModel(), Resources.Error_Clan_Detail_Controller, id);
            }
        }

        [HttpPost]
        [SessionAuthorize]
        public IActionResult CancelDescriptionEdit(Guid id)
        {
            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> UpdateDescription(Guid id, string description)
        {
            try
            {
                await _clanBLL.UpdateDescriptionAsync(id, description, _sessionService.GetUserId());
            }
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Update Description of Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Update Description of Clan with Id : {id}. Session User not found.", id);
            }
            catch (MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Update Description of Clan with Id : {id}. Clan Member not found.", id);
            }
            catch (WrongRoleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Update Description of Clan with Id : {id}. Your role doesn't allow you to perform that action.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Update Description of Clan with Id : {id}. Clan not found.", id);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Clan Description Update error for Clan with Id : {Id}", new ClanListViewModel(), Resources.Error_Clan_UpdateDescription_Controller, id);
            }

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            try
            {
                await _clanBLL.ToggleActiveAsync(id, _sessionService.GetUserId());

                TempData["SuccessMessage"] = Resources.Success_Clan_ToggleActive_Controller;

                return RedirectToAction("Detail", new { id });
            }
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Toggle Active Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Toggle Active Clan with Id : {id}. Session User not found.", id);
            }
            catch (WrongRoleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Toggle Active Clan with Id : {id}. Your role doesn't allow you to perform that action.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Toggle Active Clan with Id : {id}. Clan not found.", id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Clan_ToggleActive_Controller;
                _logger.LogError(ex, ex.Message);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Join(Guid id)
        {
            try
            {
                await _clanBLL.JoinAsync(id, _sessionService.GetUserId());

                TempData["SuccessMessage"] = Resources.Success_Clan_Join_Controller;

                return RedirectToAction("Detail", new { id });
            }
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot join Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot join Clan with Id : {id}. Session User not found.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot join Clan with Id : {id}. Clan not found.", id);
            }
            catch (MemberAlreadyInClanException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot join Clan with Id : {id}. User already Clan Member.", id);
            }
            catch (ClanInactiveException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot join Clan with Id : {id}. Clan is not Active.", id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Clan_Join_Controller;
                _logger.LogError(ex, ex.Message);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> RemoveMember(Guid id, Guid? targetUserId)
        {
            try
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
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. Session User not found.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. Clan not found.", id);

                return RedirectToAction("List");
            }
            catch (MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. Clan Member not found.", id);
            }
            catch (WrongRoleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. Your role doesn't allow you to perform that action.", id);
            }
            catch (ClanLeaderRemovalException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot remove Member of Clan with Id : {id}. Cannot remove Clan Leader.", id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Clan_RemoveMember_Controller;
                _logger.LogError(ex, ex.Message);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _clanBLL.DeleteAsync(id, _sessionService.GetUserId());

                TempData["SuccessMessage"] = Resources.Success_Clan_Delete_Controller;
            }
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. Session User not found.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. Clan not found.", id);
            }
            catch (SessionUserMemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. Session User Member not found.", id);
            }
            catch (MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. Clan Member not found.", id);
            }
            catch (NotClanLeaderException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Delete Clan with Id : {id}. Only Clan Leader can Delete Clan.", id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Clan_Delete_Controller;
                _logger.LogError(ex, ex.Message);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> ChangeMemberRole(Guid id, Guid targetUserId, ClanRole newRole)
        {
            try
            {
                await _clanBLL.ChangeMemberRole(id, _sessionService.GetUserId(), targetUserId, newRole);

                TempData["SuccessMessage"] = Resources.Success_Clan_ChangeMemberRole_Controller;

                return RedirectToAction("Detail", new { id });
            }
            catch (NoSessionUserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. No User in Session.", id);
            }
            catch (SessionUserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. Session User not found.", id);
            }
            catch (ClanNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. Clan not found.", id);
            }
            catch (SessionUserMemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. Session User Member not found.", id);
            }
            catch (WrongRoleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. Your role doesn't allow you to perform that action.", id);
            }
            catch (MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. Member not found.", id);
            }
            catch (ClanRoleChangeException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Cannot Change Member Role of Clan with Id : {id}. ", id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Error_Clan_ChangeMemberRole_Controller;
                _logger.LogError(ex, ex.Message);
            }

            return RedirectToAction("List");
        }
    }
}