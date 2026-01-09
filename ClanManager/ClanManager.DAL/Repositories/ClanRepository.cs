using ClanManager.DAL.DAO;
using ClanManager.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClanManager.DAL.Repositories
{
    public class ClanRepository : IClanRepository
    {
        private readonly DataContext _context;

        public ClanRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ClanMember?>> GetUserMemberships(Guid sessionGuid) => await _context.Members.Where(x => x.UserId == sessionGuid)
                                                                                                           .Include(y => y.Clan)
                                                                                                           .ToListAsync();
        public async Task<List<Clan?>> GetAll() => await _context.Clans.ToListAsync();
        public async Task<List<Clan?>> GetAllActives() => await _context.Clans.Where(x => x.IsActive).ToListAsync();
        public async Task<bool> ClanExistsAsync(string clanName, string clanTag) => await _context.Clans.AnyAsync(x => x.Name == clanName && x.Tag == clanTag);
        public async Task AddAsync(Clan clan) => await _context.Clans.AddAsync(clan);
        public async Task AddMemberAsync(ClanMember clanMember) => await _context.Members.AddAsync(clanMember);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public void RemoveMember(ClanMember clanMember) => _context.Members.Remove(clanMember);
        public void Delete(Clan clan) => _context.Clans.Remove(clan);
        public async Task<Clan?> GetByIdAsync(Guid clanId) => await _context.Clans.Include(x => x.Members)
                                                                                  .ThenInclude(y => y.User)
                                                                                  .SingleOrDefaultAsync(z => z.Id == clanId);
        public async Task<ClanMember> GetClanMemberByIdAsync(Guid userId, Guid clanId) => await _context.Members.SingleOrDefaultAsync(x => x.UserId == userId && x.ClanId == clanId);
    }
}
