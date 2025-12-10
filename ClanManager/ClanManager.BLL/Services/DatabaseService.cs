using ClanManager.DAL;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.BLL.Services
{
    public interface IDatabaseService
    {
        Task ResetDatabaseAsync();
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly DataContext _context;

        public DatabaseService(DataContext context)
        {
            _context = context;
        }

        public async Task ResetDatabaseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();
            await Seed.DataSeeder.SeedAsync(_context);
        }
    }
}
