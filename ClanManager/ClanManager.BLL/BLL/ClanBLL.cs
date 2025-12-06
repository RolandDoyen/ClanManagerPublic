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
    public class ClanBLL : IClanBLL
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ClanBLL(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ClanDTO>> GetAllAsync(bool activeOnly, bool myClansOnly, string? sessionUserId)
        {
            if (myClansOnly)
            {
                if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                    throw new NoSessionUserException();

                var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
                if (sessionUser == null)
                    throw new SessionUserNotFoundException();

                var members = await _context.Members.Where(x => x.UserId == sessionGuid)
                                                    .Include(y => y.Clan)
                                                    .ToListAsync();
                var myClans = members.Select(x => x.Clan).OrderBy(y => y.Name).ToList();

                return _mapper.Map<List<ClanDTO>>(myClans);
            }

            IQueryable<Clan> query = _context.Clans;

            if (activeOnly)
                query = query.Where(x => x.IsActive);

            var clans = await query.OrderBy(x => x.Name).ToListAsync();

            return _mapper.Map<List<ClanDTO>>(clans);
        }

        public async Task<ClanDTO> CreateAsync(ClanDTO clanDTO, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            if (await _context.Clans.AnyAsync(x => x.Name == clanDTO.Name && x.Tag == clanDTO.Tag))
                throw new ClanAlreadyExistsException();

            var clanToAdd = new Clan
            {
                Name = clanDTO.Name,
                Tag = clanDTO.Tag,
                Description = clanDTO.Description,
                IsActive = clanDTO.IsActive
            };

            var newMember = new ClanMember
            {
                Clan = clanToAdd,
                User = sessionUser,
                ClanRole = ClanRole.ClanLeader,
                JoinedAt = DateTime.UtcNow
            };

            clanToAdd.Members = new List<ClanMember> { newMember };

            await _context.Clans.AddAsync(clanToAdd);
            await _context.Members.AddAsync(newMember);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClanDTO>(clanToAdd);
        }

        public async Task<ClanUserContextDTO> GetByIdAsync(Guid clanId, string? sessionUserId, Role? sessionUserRole)
        {
            var clan = await _context.Clans.Include(x => x.Members)
                                           .ThenInclude(y => y.User)
                                           .SingleOrDefaultAsync(z => z.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            var result = new ClanUserContextDTO { Clan = _mapper.Map<ClanDTO>(clan) };

            if (Guid.TryParse(sessionUserId, out var sessionGuid))
            {
                result.IsSessionUserMember = clan.Members.Any(x => x.User.Id == sessionGuid);
                result.IsSessionUserCoLeader = clan.Members.Any(x => x.User.Id == sessionGuid && x.ClanRole == ClanRole.CoLeader);
            }

            var clanMemberLeader = clan.Members.SingleOrDefault(x => x.ClanRole == ClanRole.ClanLeader);
            if (clanMemberLeader != null)
            {
                result.LeaderEmail = clanMemberLeader.User.Email;

                if (sessionGuid != Guid.Empty && clanMemberLeader.User.Id == sessionGuid)
                    result.IsSessionUserLeader = true;
            }

            if (sessionUserRole != null)
            {
                result.IsSessionUserAdmin = (sessionUserRole == Role.Admin) || (sessionUserRole == Role.SuperAdmin);
                result.CanSessionUserJoinQuit = clan.IsActive;
            }

            result.Clan.Members = result.Clan.Members.OrderBy(x => (int)x.ClanRole).ThenBy(x => x.User.Email).ToList();
            result.SessionUserId = sessionGuid;

            foreach (var member in result.Clan.Members)
            {
                var clanMemberContextDTO = new ClanMemberContextDTO();

                clanMemberContextDTO.User = member.User;
                clanMemberContextDTO.ClanRole = member.ClanRole;

                clanMemberContextDTO.CanChangeMemberRole = false;
                clanMemberContextDTO.CanRemoveMember = false;

                if (sessionGuid != Guid.Empty && sessionGuid != member.User.Id)
                {
                    var sessionMember = clan.Members.SingleOrDefault(x => x.User.Id == sessionGuid);
                    if (sessionMember != null)
                    {
                        if (sessionMember.ClanRole == ClanRole.ClanLeader ||
                            (sessionMember.ClanRole == ClanRole.CoLeader && member.ClanRole != ClanRole.CoLeader && member.ClanRole != ClanRole.ClanLeader))
                        {
                            clanMemberContextDTO.CanChangeMemberRole = true;
                        }
                        if (member.ClanRole != ClanRole.ClanLeader)
                        {
                            if (sessionMember.ClanRole == ClanRole.ClanLeader ||
                                (sessionMember.ClanRole == ClanRole.CoLeader && member.ClanRole != ClanRole.CoLeader))
                            {
                                clanMemberContextDTO.CanRemoveMember = true;
                            }
                        }
                    }
                }

                result.MembersContext.Add(clanMemberContextDTO);
            }

            return result;
        }

        public async Task UpdateDescriptionAsync(Guid clanId, string description, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            var sessionUserMember = _context.Members.SingleOrDefault(x => x.User.Id == sessionGuid && x.ClanId == clanId);
            if (sessionUserMember == null)
                throw new MemberNotFoundException();

            if (sessionUserMember.ClanRole != ClanRole.ClanLeader)
                throw new WrongRoleException();

            var clan = await _context.Clans.SingleOrDefaultAsync(x => x.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            clan.Description = description;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleActiveAsync(Guid clanId, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            if (sessionUser.Role != Role.Admin && sessionUser.Role != Role.SuperAdmin)
                throw new WrongRoleException();

            var clan = await _context.Clans.SingleOrDefaultAsync(x => x.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            clan.IsActive = !clan.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task JoinAsync(Guid clanId, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            var clan = await _context.Clans.Include(x => x.Members)
                                            .ThenInclude(y => y.User)
                                            .SingleOrDefaultAsync(z => z.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            if (clan.Members.Any(m => m.User.Id == sessionGuid))
                throw new MemberAlreadyInClanException();

            if (!clan.IsActive)
                throw new ClanInactiveException();

            var newMember = new ClanMember
            {
                Clan = clan,
                User = sessionUser,
                ClanRole = ClanRole.Recruit
            };

            clan.Members.Add(newMember);

            await _context.Members.AddAsync(newMember);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid clanId, string? sessionUserId, Guid? targetUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            var clan = await _context.Clans.Include(x => x.Members)
                                           .ThenInclude(y => y.User)
                                           .SingleOrDefaultAsync(z => z.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            var member = clan.Members.SingleOrDefault(x => x.User.Id == (targetUserId ?? sessionGuid) && x.ClanId == clanId);
            if (member == null)
                throw new MemberNotFoundException();

            if (member.User.Id != sessionGuid)
            {
                var isAdminOrSuperAdmin = sessionUser.Role == Role.Admin || sessionUser.Role == Role.SuperAdmin;

                var isLeaderOrCoLeader = false;
                var sessionMember = clan.Members.SingleOrDefault(x => x.User.Id == sessionGuid);
                if (sessionMember != null)
                {
                    isLeaderOrCoLeader = sessionMember.ClanRole == ClanRole.ClanLeader || sessionMember.ClanRole == ClanRole.CoLeader;
                }

                if (!isAdminOrSuperAdmin && !isLeaderOrCoLeader)
                    throw new WrongRoleException();
            }

            if (member.ClanRole == ClanRole.ClanLeader)
                throw new ClanLeaderRemovalException();

            clan.Members.Remove(member);
            _context.Members.Remove(member);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid clanId, string? sessionUserId)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            var clan = await _context.Clans.Include(x => x.Members)
                                           .ThenInclude(y => y.User)
                                           .SingleOrDefaultAsync(z => z.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            var member = clan.Members.SingleOrDefault(x => x.User.Id == sessionGuid && x.ClanId == clanId);
            if (member == null)
                throw new MemberNotFoundException();

            if (member.ClanRole != ClanRole.ClanLeader)
                throw new NotClanLeaderException();

            _context.Clans.Remove(clan);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeMemberRole(Guid clanId, string? sessionUserId, Guid targetUserId, ClanRole newRole)
        {
            if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var sessionGuid))
                throw new NoSessionUserException();

            var sessionUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == sessionGuid);
            if (sessionUser == null)
                throw new SessionUserNotFoundException();

            var clan = await _context.Clans.Include(x => x.Members)
                                           .ThenInclude(y => y.User)
                                           .SingleOrDefaultAsync(z => z.Id == clanId);
            if (clan == null)
                throw new ClanNotFoundException();

            var sessionUserMember = await _context.Members.SingleOrDefaultAsync(x => x.UserId == sessionGuid && x.ClanId == clanId);
            if (sessionUserMember == null)
                throw new SessionUserMemberNotFoundException();

            if (sessionUserMember.ClanRole != ClanRole.ClanLeader && sessionUserMember.ClanRole != ClanRole.CoLeader)
                throw new WrongRoleException();

            var member = clan.Members.SingleOrDefault(x => x.User.Id == targetUserId && x.ClanId == clanId);
            if (member == null)
                throw new MemberNotFoundException();

            if (!Enum.GetNames(typeof(ClanRole)).Contains(newRole.ToString()))
                throw new ClanRoleChangeException(Resources.Error_RoleChangeException_Invalid_ClanRole);

            if (sessionGuid == targetUserId)
                throw new ClanRoleChangeException(Resources.Error_CannotChangeOwnRoleException);

            if (sessionUserMember.ClanRole == ClanRole.CoLeader && member.ClanRole == ClanRole.CoLeader)
                throw new ClanRoleChangeException(Resources.Error_CannotChangeSameRoleException);

            if (newRole == ClanRole.ClanLeader)
                sessionUserMember.ClanRole = ClanRole.CoLeader;

            member.ClanRole = newRole;
            await _context.SaveChangesAsync();
        }
    }
}