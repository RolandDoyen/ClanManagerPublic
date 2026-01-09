using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Exceptions;
using ClanManager.Core.Resources;
using ClanManager.WEB.Attributes;
using ClanManager.WEB.Models.User;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClanManager.WEB.Controllers
{
    /// <summary>
    /// Controller responsible for managing users, including creation, authentication,
    /// listing, displaying details, role changes, and ban management.
    /// Access to some actions is restricted to Admin/SuperAdmin roles.
    /// Uses SessionAuthorize and CheckBan attributes to restrict access depending on login status and ban state.
    /// Exceptions are typically caught and converted to user-friendly messages via ModelState or TempData.
    /// </summary>
    public class UserController : BaseController<UserController>
    {
        private readonly IUserBLL _userBLL;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes the UserController.
        /// </summary>
        /// <param name="userBLL">Business Logic Layer for user operations.</param>
        /// <param name="mapper">AutoMapper instance for DTO/ViewModel mapping.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="sessionService">SessionService instance.</param>
        public UserController(IUserBLL userBLL, IMapper mapper, ILogger<UserController> logger, ISessionService sessionService) : base(logger, sessionService)
        {
            _userBLL = userBLL;
            _mapper = mapper;
        }

        /// <summary>
        /// Displays the user creation form.
        /// Accessible only by not logged-in users.
        /// </summary>
        /// <returns>Returns the view for creating a new user.</returns>
        [HttpGet]
        [SessionAuthorize(allowAnonymous: true)]
        public IActionResult Create() => View();

        /// <summary>
        /// Handles user creation form submission.
        /// Accessible only by not logged-in users.
        /// Validates input model, creates the user, and initializes the session.
        /// </summary>
        /// <param name="model">User creation form data.</param>
        /// <returns>Redirects on success or returns the same view with errors.</returns>
        [HttpPost]
        [SessionAuthorize(allowAnonymous: true)]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var newUser = await _userBLL.CreateAsync(_mapper.Map<UserDTO>(model));

                _sessionService.SetUserSession(newUser.Id, newUser.Email, newUser.Role);

                _logger.LogInformation("User create success for user with Id : {Id}", newUser.Id);

                TempData["SuccessMessage"] = Resources.Success_User_Create_Controller;

                return RedirectToAction("List", "Clan");
            }
            catch (UserAlreadyExistsException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogWarning("User Create failed for User with Email : {Email} already exists.", model.Email);

                model.Password = string.Empty;

                return View(model);
            }
        }

        /// <summary>
        /// Displays a list of all users.
        /// Accessible only by connected, non-banned users with Admin or SuperAdmin roles.
        /// </summary>
        /// <returns>Returns the user list view containing all users, or a view with error information if an exception occurs.</returns>
        [HttpGet]
        [SessionAuthorize]
        [CheckBan]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> List()
        {
            var userDTOs = await _userBLL.GetAllAsync();
            var model = new UserListViewModel { Users = _mapper.Map<List<UserDetailViewModel>>(userDTOs) };

            return View(model);
        }

        /// <summary>
        /// Displays the detail view of a specific user.
        /// Accessible only by connected, non-banned users with Admin or SuperAdmin roles.
        /// </summary>
        /// <param name="id">User unique identifier.</param>
        /// <returns>Returns the user detail view if the user is found and accessible, or redirects to the user list page in case of errors or insufficient permissions.</returns>
        [HttpGet]
        [SessionAuthorize]
        [CheckBan]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Detail(Guid id)
        {
            var userContextDTO = await _userBLL.GetByIdAsync(id, _sessionService.GetUserId());

            var model = _mapper.Map<UserDetailViewModel>(userContextDTO);

            return View(model);
        }

        /// <summary>
        /// Displays the login form.
        /// Accessible by all users.
        /// </summary>
        /// <returns>Returns the login view.</returns>
        [HttpGet]
        [SessionAuthorize(allowAnonymous: true)]
        public IActionResult Login() => View();

        /// <summary>
        /// Handles login submission.
        /// Accessible only by not logged-in users.
        /// Validates the input model, delegates authentication to the BLL, initializes user session upon success, and handles navigation or error messages.
        /// </summary>
        /// <param name="model">Login form data.</param>   
        /// <returns>Redirects on success or returns the same view with validation or authentication errors.</returns>
        [HttpPost]
        [SessionAuthorize(allowAnonymous: true)]
        public async Task<IActionResult> Login(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var loggedInUser = await _userBLL.LoginAsync(_mapper.Map<UserDTO>(model));

                _sessionService.SetUserSession(loggedInUser.Id, loggedInUser.Email, loggedInUser.Role);

                _logger.LogInformation("User login success for User with Email : {Email}", loggedInUser.Email);

                if (loggedInUser.IsBanned)
                {
                    _sessionService.SetUserBan();
                    return RedirectToAction("Index", "Home");
                }

                TempData["SuccessMessage"] = Resources.Success_User_Login_Controller;

                return RedirectToAction("List", "Clan");
            }
            catch (UserEmailNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogWarning("User login failed. User with Email : {Email} not found.", model.Email);
            }
            catch (PasswordValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogWarning("User login failed. Wrong password for User with Email : {Email}", model.Email);
            }

            model.Password = string.Empty;

            return View(model);
        }

        /// <summary>
        /// Logs out the current user and clears session.
        /// Accessible only by connected users.
        /// </summary>
        /// <returns>Returns the view of the clan list page after clearing the user session.</returns>
        [HttpGet]
        [SessionAuthorize]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logout for User with Id : {Id}", _sessionService.GetUserId());

            _sessionService.ClearSession();

            TempData["SuccessMessage"] = Resources.Success_User_Logout_Controller;

            return RedirectToAction("List", "Clan");
        }

        /// <summary>
        /// Changes the role of a specific user.
        /// Accessible only by connected, non-banned users with Admin or SuperAdmin roles.
        /// </summary>
        /// <param name="id">ID of the user whose role is being updated.</param>
        /// <param name="newRole">The new role to assign, represented as a value of the <see cref="Role"/> enum.</param>
        /// <returns>Redirects to the user detail page if the role change succeeds.</returns>
        [HttpPost]
        [SessionAuthorize]
        [CheckBan]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> ChangeRole(Guid id, Role newRole)
        {
            await _userBLL.ChangeRoleAsync(id, _sessionService.GetUserId(), newRole);

            TempData["SuccessMessage"] = Resources.Success_User_ChangeRole_Controller;

            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Toggles the ban status of a user.
        /// Accessible only by connected, non-banned users with Admin or SuperAdmin roles.
        /// </summary>
        /// <param name="id">ID of the user whose ban status is being toggled.</param>
        /// <returns>Redirects to the user detail page if the operation succeeds.</returns>
        [HttpPost]
        [SessionAuthorize]
        [CheckBan]
        [AuthorizeRole(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> ToggleBan(Guid id)
        {
            await _userBLL.ToggleBanAsync(id, _sessionService.GetUserId());

            TempData["SuccessMessage"] = Resources.Success_User_ToggleBan_Controller;

            return RedirectToAction("Detail", new { id });
        }
    }
}