using ClanManager.DAL;
using ClanManager.DAL.DAO;
using ClanManager.DAL.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClanManager.Tests.Integration.Repository
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly UserRepository _repository;
        private readonly SqliteConnection _connection;

        public UserRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new DataContext(options);
            _context.Database.EnsureCreated();

            _repository = new UserRepository(_context);
        }

        #region GetByIdAsync()
        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Email = "test@mail.com", PasswordHash = "hashedPassword" };
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
        #endregion

        public void Dispose()
        {
            _connection.Close();
            _context.Dispose();
        }
    }
}
