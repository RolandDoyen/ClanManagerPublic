using ClanManager.DAL.DAO;

namespace ClanManager.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string normalizedEmail);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task<List<User>> GetAllAsync();
        Task<ClanMember?> GetClanLeaderAsync(Guid userId);
        Task<List<ClanMember?>> GetMemberShipsAsync(Guid userId);
        void RemoveAllMemberships(Guid userId);
        Task<User?> GetByIdAsync(Guid userId);
    }
}
