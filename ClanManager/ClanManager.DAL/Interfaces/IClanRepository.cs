using ClanManager.DAL.DAO;

namespace ClanManager.DAL.Interfaces
{
    public interface IClanRepository
    {
        Task<List<ClanMember?>> GetUserMemberships(Guid clanId);
        Task<List<Clan?>> GetAll();
        Task<List<Clan?>> GetAllActives();
        Task<bool> ClanExistsAsync(string clanName, string clanTag);
        Task AddAsync(Clan clan);
        Task AddMemberAsync(ClanMember clanMember);
        Task SaveChangesAsync();
        void RemoveMember(ClanMember clanMember);
        void Delete(Clan clan);
        Task<Clan?> GetByIdAsync(Guid clanId);
        Task<ClanMember> GetClanMemberByIdAsync(Guid userId, Guid clanId);
    }
}
