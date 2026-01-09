using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Exceptions;
using ClanManager.DAL;
using ClanManager.DAL.DAO;
using ClanManager.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.BLL.BLL
{
    /// <summary>
    /// Implementation of the validation service handling cross-cutting entity and session checks.
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IClanRepository _clanRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationService"/>.
        /// </summary>
        /// <param name="context">The database context for validation queries.</param>
        public ValidationService(IUserRepository userRepository, IClanRepository clanRepository)
        {
            _userRepository = userRepository;
            _clanRepository = clanRepository;
        }

        /// <inheritdoc />
        public Guid Check_Session_User(string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            return sessionGuid;
        }

        /// <inheritdoc />
        public async Task<User> Find_Session_User(Guid sessionGuid)
        {
            var sessionUser = await _userRepository.GetByIdAsync(sessionGuid);

            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            return sessionUser;
        }

        /// <inheritdoc />
        public async Task<User> Find_User(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new UserNotFoundException();

            return user;
        }

        /// <inheritdoc />
        public async Task<Clan> Find_Clan(Guid clanId)
        {
            var clan = await _clanRepository.GetByIdAsync(clanId);

            if (clan == null)
                throw new ClanNotFoundException();

            return clan;
        }

        /// <inheritdoc />
        public async Task<ClanMember> Find_Session_User_Member(Guid sessionGuid, Guid clanId)
        {
            var sessionUserMember = await _clanRepository.GetClanMemberByIdAsync(sessionGuid, clanId);
            if (sessionUserMember == null)
                throw new SessionUserMemberNotFoundException();

            return sessionUserMember;
        }

        /// <inheritdoc />
        public async Task<ClanMember> Find_User_Member(Guid targetUserId, Guid clanId)
        {
            var member = await _clanRepository.GetClanMemberByIdAsync(targetUserId, clanId);
            if (member == null)
                throw new MemberNotFoundException();

            return member;
        }
    }
}
