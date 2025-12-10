using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Exceptions;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Resources;
using ClanManager.DAL;
using ClanManager.DAL.DAO;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.BLL.BLL
{
    /// <summary>
    /// Provides business logic operations related to users, including creation, retrieval,
    /// authentication, role management, and ban toggling.
    /// Ensures all operations respect business rules and roles (Admin/SuperAdmin).
    /// </summary>
    public class UserBLL : IUserBLL
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="UserBLL"/>.
        /// </summary>
        /// <param name="context">Database context for data access.</param>
        /// <param name="mapper">AutoMapper instance for mapping between entities and DTOs.</param>
        public UserBLL(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new user with a normalized email and hashed password.
        /// </summary>
        /// <param name="model">User data including Email and Password.</param>
        /// <returns>The created <see cref="UserDTO"/>.</returns>
        /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same email exists.</exception>
        public async Task<UserDTO> CreateAsync(UserDTO model)
        {
            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == normalizedEmail);
            if (user != null)
                throw new UserAlreadyExistsException();

            var userToAdd = _mapper.Map<User>(model);
            userToAdd.Email = normalizedEmail;
            userToAdd.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            await _context.Users.AddAsync(userToAdd);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(userToAdd);
        }

        /// <summary>
        /// Retrieves all users sorted by email.
        /// </summary>
        /// <returns>A list of <see cref="UserDTO"/>.</returns>
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _context.Users.OrderBy(x => x.Email).ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);
        }

        /// <summary>
        /// Retrieves a user and provides context about the session user's relation and permissions.
        /// </summary>
        /// <param name="userId">The ID of the target user.</param>
        /// <param name="sessionUserId">The ID of the session user (nullable).</param>
        /// <returns>A <see cref="UserContextDTO"/> with user data and session context.</returns>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if the target user does not exist.</exception>
        public async Task<UserContextDTO> GetByIdAsync(Guid userId, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();
            else if (sessionUser.Role != Role.SuperAdmin &&
                     sessionUser.Role != Role.Admin)
                throw new WrongRoleException();

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var result = new UserContextDTO { User = _mapper.Map<UserDTO>(user) };

            result.IsOwnProfile = result.User.Id == sessionGuid;
            result.SessionUserRole = sessionUser.Role;
            result.IsUserSuperAdmin = user.Role == Role.SuperAdmin;

            if (!(sessionUser.Role == Role.Admin && user.Role == Role.Admin))
            {
                result.CanManageUsers = true;

                var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ClanRole == ClanRole.ClanLeader);
                if (member != null)
                    result.CanBanUser = false;
            }

            return result;
        }

        /// <summary>
        /// Authenticates a user by email and password.
        /// Email is normalized before lookup, and password is verified using bcrypt.
        /// </summary>
        /// <param name="model">User data containing Email and Password.</param>
        /// <returns>The authenticated <see cref="UserDTO"/>.</returns>
        /// <exception cref="UserEmailNotFoundException">Thrown if no user matches the email.</exception>
        /// <exception cref="PasswordValidationException">Thrown if the password is incorrect.</exception>
        public async Task<UserDTO> LoginAsync(UserDTO model)
        {
            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == normalizedEmail);
            if (user == null)
                throw new UserEmailNotFoundException();

            var isValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isValid)
                throw new PasswordValidationException();

            return _mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Updates the role of a user after validating business rules.
        /// Rules include: cannot change own role, cannot assign or modify SuperAdmin, role must exist.
        /// </summary>
        /// <param name="userId">ID of the user whose role is being updated.</param>
        /// <param name="sessionUserId">ID of the session user performing the action.</param>
        /// <param name="newRole">The new role to assign, represented as a value of the <see cref="Role"/> enum.</param>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if target user does not exist.</exception>
        /// <exception cref="RoleChangeException">Thrown if rules for role change are violated.</exception>
        public async Task ChangeRoleAsync(Guid userId, string? sessionUserId, Role newRole)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();
            else if (sessionUser.Role != Role.SuperAdmin &&
                     sessionUser.Role != Role.Admin)
                throw new WrongRoleException();

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            if (userId == sessionGuid)
                throw new RoleChangeException(Resources.Error_RoleChangeException_OwnRole);

            if (newRole == Role.SuperAdmin)
                throw new RoleChangeException(Resources.Error_RoleChangeException_To_SuperAdmin);

            if (user.Role == Role.SuperAdmin)
                throw new RoleChangeException(Resources.Error_RoleChangeException_SuperAdmin);

            if (!Enum.GetNames(typeof(Role)).Contains(newRole.ToString()))
                throw new RoleChangeException(Resources.Error_RoleChangeException_Invalid_Role);

            user.Role = newRole;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Toggles the ban status of a user.
        /// Also handles role downgrade, member removal, and business rules regarding banning.
        /// Business rules includes role-based authorization, protection against self-ban, and restrictions related to Admin, SuperAdmin, and clan leadership.
        /// </summary>
        /// <param name="userId">ID of the user whose ban status is being toggled.</param>
        /// <param name="sessionUserId">ID of the session user performing the action.</param>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if target user does not exist.</exception>
        /// <exception cref="ToggleBanException">Thrown if the ban action violates business rules (self, clan leader, SuperAdmin, etc.).</exception>
        public async Task ToggleBanAsync(Guid userId, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();
            else if (sessionUser.Role != Role.SuperAdmin &&
                     sessionUser.Role != Role.Admin)
                throw new WrongRoleException();

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var userMember = await _context.Members.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (userMember != null && userMember.ClanRole == ClanRole.ClanLeader)
                throw new ToggleBanException(Resources.Error_ToggleBanException_ClanLeader);

            if (user.Id == sessionGuid)
                throw new ToggleBanException(Resources.Error_ToggleBanException_Self);

            if (user.Role == Role.SuperAdmin)
                throw new ToggleBanException(Resources.Error_ToggleBanException_SuperAdmin);

            if ((sessionUser.Role == Role.Admin || sessionUser.Role == Role.User) && user.Role == Role.Admin)
                throw new ToggleBanException(Resources.Error_ToggleBanException_AdminByAdmin);

            sessionUser.Role = Role.SuperAdmin;
            if (sessionUser.Role == Role.SuperAdmin && user.Role == Role.Admin)
                user.Role = Role.User;

            user.IsBanned = !user.IsBanned;

            if (user.IsBanned)
                _context.Members.RemoveRange(_context.Members.Where(x => x.UserId == user.Id));

            await _context.SaveChangesAsync();
        }
    }
}