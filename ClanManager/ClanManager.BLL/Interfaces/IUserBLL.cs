using ClanManager.BLL.DTO;
using ClanManager.Core;

namespace ClanManager.BLL.Interfaces
{
    public interface IUserBLL
    {
        Task<UserDTO> CreateAsync(UserDTO model);
        Task<List<UserDTO>> GetAllAsync();
        Task<UserContextDTO> GetByIdAsync(Guid userId, string? sessionUserId);
        Task<UserDTO> LoginAsync(UserDTO model);
        Task ChangeRoleAsync(Guid userId, string? sessionUserId, Role newRole);
        Task ToggleBanAsync(Guid userId, string? sessionUserId);
    }
}
