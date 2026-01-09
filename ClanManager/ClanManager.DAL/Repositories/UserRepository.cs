using ClanManager.Core;
using ClanManager.DAL.DAO;
using ClanManager.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string normalizedEmail) => await _context.Users.SingleOrDefaultAsync(x => x.Email == normalizedEmail);

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<ClanMember?> GetClanLeaderAsync(Guid userId) => await _context.Members.FirstOrDefaultAsync(x => x.UserId == userId && x.ClanRole == ClanRole.ClanLeader);

        public async Task<List<ClanMember?>> GetMemberShipsAsync(Guid userId) => await _context.Members.Where(x => x.UserId == userId).ToListAsync();

        public void RemoveAllMemberships(Guid userId) => _context.Members.RemoveRange(_context.Members.Where(x => x.UserId == userId));

        public async Task<User?> GetByIdAsync(Guid userId) => await _context.Users.FindAsync(userId);
    }
}
