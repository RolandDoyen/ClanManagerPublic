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
    public class ClanBLL : IClanBLL
    {
        private readonly IClanRepository _clanRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public ClanBLL(IClanRepository clanRepository, IValidationService validationService, IMapper mapper)
        {
            _clanRepository = clanRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<List<ClanDTO>> GetAllAsync(bool activeOnly, bool myClansOnly, string? sessionUserId)
        {
            if (myClansOnly)
            {
                var sessionGuid = _validationService.Check_Session_User(sessionUserId);

                var sessionUser = await _validationService.Find_Session_User(sessionGuid);

                var members = await _clanRepository.GetUserMemberships(sessionGuid);
                var myClans = members.Select(x => x.Clan).OrderBy(y => y.Name).ToList();

                return _mapper.Map<List<ClanDTO>>(myClans);
            }

            List<Clan> clans;
            if (activeOnly)
                clans = await _clanRepository.GetAllActives();
            else
                clans = await _clanRepository.GetAll();

            clans = clans.OrderBy(x => x.Name).ToList();

            return _mapper.Map<List<ClanDTO>>(clans);
        }

        public async Task<ClanDTO> CreateAsync(ClanDTO clanDTO, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            if (await _clanRepository.ClanExistsAsync(clanDTO.Name, clanDTO.Tag))
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

            await _clanRepository.AddAsync(clanToAdd);
            await _clanRepository.AddMemberAsync(newMember);
            await _clanRepository.SaveChangesAsync();

            return _mapper.Map<ClanDTO>(clanToAdd);
        }

        public async Task<ClanUserContextDTO> GetByIdAsync(Guid clanId, string? sessionUserId, Role? sessionUserRole)
        {
            var clan = await _validationService.Find_Clan(clanId);

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
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var sessionUserMember = await _validationService.Find_Session_User_Member(sessionGuid, clanId);

            if (sessionUserMember.ClanRole != ClanRole.ClanLeader)
                throw new WrongRoleException();

            var clan = await _validationService.Find_Clan(clanId);

            clan.Description = description;

            await _clanRepository.SaveChangesAsync();
        }

        public async Task ToggleActiveAsync(Guid clanId, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            if (sessionUser.Role != Role.SuperAdmin && sessionUser.Role != Role.Admin)
                throw new WrongRoleException();

            var clan = await _validationService.Find_Clan(clanId);

            clan.IsActive = !clan.IsActive;

            await _clanRepository.SaveChangesAsync();
        }

        public async Task JoinAsync(Guid clanId, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var clan = await _validationService.Find_Clan(clanId);

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

            await _clanRepository.AddMemberAsync(newMember);
            await _clanRepository.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid clanId, string? sessionUserId, Guid? targetUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var clan = await _validationService.Find_Clan(clanId);

            var member = await _validationService.Find_User_Member((targetUserId ?? sessionGuid), clanId);

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
            _clanRepository.RemoveMember(member);

            await _clanRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid clanId, string? sessionUserId)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var clan = await _validationService.Find_Clan(clanId);

            var sessionUserMember = await _validationService.Find_Session_User_Member(sessionGuid, clanId);

            if (sessionUserMember.ClanRole != ClanRole.ClanLeader)
                throw new NotClanLeaderException();

            _clanRepository.Delete(clan);
            // TO DO in case of clan removing, make sur all clan members are also removed => should be because of cascade delete
            await _clanRepository.SaveChangesAsync();
        }

        public async Task ChangeMemberRole(Guid clanId, string? sessionUserId, Guid targetUserId, ClanRole newRole)
        {
            var sessionGuid = _validationService.Check_Session_User(sessionUserId);

            var sessionUser = await _validationService.Find_Session_User(sessionGuid);

            var clan = await _validationService.Find_Clan(clanId);

            var sessionUserMember = await _validationService.Find_Session_User_Member(sessionGuid, clanId);

            if (sessionUserMember.ClanRole != ClanRole.ClanLeader && sessionUserMember.ClanRole != ClanRole.CoLeader)
                throw new WrongRoleException();

            var member = await _validationService.Find_User_Member(targetUserId, clanId);

            if (!Enum.GetNames(typeof(ClanRole)).Contains(newRole.ToString()))
                throw new ClanRoleChangeException(Resources.Error_RoleChangeException_Invalid_ClanRole);

            if (sessionGuid == targetUserId)
                throw new ClanRoleChangeException(Resources.Error_CannotChangeOwnRoleException);

            if (sessionUserMember.ClanRole == ClanRole.CoLeader && member.ClanRole == ClanRole.CoLeader)
                throw new ClanRoleChangeException(Resources.Error_CannotChangeSameRoleException);

            if (newRole == ClanRole.ClanLeader)
                sessionUserMember.ClanRole = ClanRole.CoLeader;

            member.ClanRole = newRole;
            await _clanRepository.SaveChangesAsync();
        }
    }
}