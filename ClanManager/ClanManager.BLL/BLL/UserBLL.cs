using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Exceptions;
using ClanManager.Core.Resources;
using ClanManager.DAL.DAO;
using ClanManager.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.BLL.BLL
{
    /// <inheritdoc cref="IUserBLL"/>
    public class UserBLL : IUserBLL
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Initializes a new instance of <see cref="UserBLL"/>.
        /// </summary>
        /// <param name="context">Database context for data access.</param>
        /// <param name="mapper">AutoMapper instance for mapping between entities and DTOs.</param>
        /// <param name="validationService">The centralized service responsible for shared business rule validations and identity checks.</param>
        public UserBLL(IUserRepository userRepository, IValidationService validationService, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <inheritdoc />
        public async Task<UserDTO> CreateAsync(UserDTO model)
        {
            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user != null)
                throw new UserAlreadyExistsException();

            var userToAdd = _mapper.Map<User>(model);
            userToAdd.Email = normalizedEmail;
            userToAdd.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            await _userRepository.AddAsync(userToAdd);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDTO>(userToAdd);
        }

        /// <inheritdoc />
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            users = users.OrderBy(x => x.Email).ToList();

            return _mapper.Map<List<UserDTO>>(users);
        }

        /// <inheritdoc />
        public async Task<UserContextDTO> GetByIdAsync(Guid userId, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var user = await _validationService.Find_User(userId);

            var result = new UserContextDTO { User = _mapper.Map<UserDTO>(user) };

            result.IsOwnProfile = result.User.Id == sessionGuid;
            result.SessionUserRole = sessionUser.Role;
            result.IsUserSuperAdmin = user.Role == Role.SuperAdmin;

            if (!(sessionUser.Role == Role.Admin && user.Role == Role.Admin))
            {
                result.CanManageUsers = true;

                var member = await _userRepository.GetClanLeaderAsync(user.Id);
                if (member != null)
                    result.CanBanUser = false;
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<UserDTO> LoginAsync(UserDTO model)
        {
            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null)
                throw new UserEmailNotFoundException();

            var isValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isValid)
                throw new PasswordValidationException();

            return _mapper.Map<UserDTO>(user);
        }

        /// <inheritdoc />
        public async Task ChangeRoleAsync(Guid userId, string? sessionUserId, Role newRole)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var user = await _validationService.Find_User(userId);

            if (userId == sessionGuid)
                throw new RoleChangeException(Resources.Error_RoleChangeException_OwnRole);

            if (newRole == Role.SuperAdmin)
                throw new RoleChangeException(Resources.Error_RoleChangeException_To_SuperAdmin);

            if (user.Role == Role.SuperAdmin)
                throw new RoleChangeException(Resources.Error_RoleChangeException_SuperAdmin);

            if (!Enum.GetNames(typeof(Role)).Contains(newRole.ToString()))
                throw new RoleChangeException(Resources.Error_RoleChangeException_Invalid_Role);

            user.Role = newRole;

            await _userRepository.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ToggleBanAsync(Guid userId, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var user = await _validationService.Find_User(userId);

            var userMembers = await _userRepository.GetMemberShipsAsync(userId);
            foreach (var member in userMembers)
            {
                if (member.ClanRole == ClanRole.ClanLeader)
                    throw new ToggleBanException(Resources.Error_ToggleBanException_ClanLeader);
            }

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
                _userRepository.RemoveAllMemberships(user.Id);

            await _userRepository.SaveChangesAsync();
        }
    }
}