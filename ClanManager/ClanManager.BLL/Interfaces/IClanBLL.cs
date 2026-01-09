using ClanManager.BLL.DTO;
using ClanManager.Core;

namespace ClanManager.BLL.Interfaces
{
    public interface IClanBLL
    {
        Task<List<ClanDTO>> GetAllAsync(bool activeOnly, bool myClansOnly, string? sessionUserId);
        Task<ClanDTO> CreateAsync(ClanDTO clanDTO, string? sessionUserId);
        Task<ClanUserContextDTO> GetByIdAsync(Guid clanId, string? sessionUserId, Role? sessionUserRole);
        Task UpdateDescriptionAsync(Guid clanId, string description, string? sessionUserId);
        Task ToggleActiveAsync(Guid clanId, string? sessionUserId);
        Task JoinAsync(Guid clanId, string? sessionUserId);
        Task RemoveMemberAsync(Guid clanId, string? sessionUserId, Guid? targetUserId);
        Task DeleteAsync(Guid clanId, string? sessionUserId);
        Task ChangeMemberRole(Guid clanId, string? sessionUserId, Guid targetUserId, ClanRole newRole);
    }
}
