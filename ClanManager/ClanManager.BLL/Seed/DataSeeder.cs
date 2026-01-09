using ClanManager.BLL.Services;
using ClanManager.Core;
using ClanManager.DAL;
using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Seed
{
    /// <summary>
    /// Provides utility methods to populate the database with initial seed data.
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Asynchronously seeds the database with default users, clans, and roles 
        /// if the database is empty, ensuring a functional environment upon deployment.
        /// </summary>
        /// <param name="context">The database context used to persist the seed data.</param>
        /// <returns>A task representing the asynchronous seeding operation.</returns>
        public static async Task SeedAsync(DataContext context)
        {
            try
            {
                if (!context.Users.Any())
                {
                    var users = new List<User>
                    {
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "superadmin@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("superadmin"),
                            Role = Role.SuperAdmin,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "admin@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                            Role = Role.Admin,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "user@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("user"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "alphaLeader@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("alphaLeader"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "gammaLeader@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("gammaLeader"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "omegaLeader@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("omegaLeader"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "alphaCoLeader@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("alphaCoLeader"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "alphaElder@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("alphaElder"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "alphaMember@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("alphaMember"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "alphaRecruit@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("alphaRecruit"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "omegaElder@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("omegaElder"),
                            Role = Role.User,
                            IsBanned = false
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Email = "multiMember@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("multiMember"),
                            Role = Role.User,
                            IsBanned = false
                        }
                    };

                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }

                if (!context.Clans.Any())
                {
                    var clans = new List<Clan>
                    {
                        new Clan
                        {
                            Id = Guid.NewGuid(),
                            Name = "Alpha Clan",
                            Tag = "ALPHA",
                            Description = "The elite Alpha Clan.",
                            IsActive = true
                        },
                        new Clan
                        {
                            Id = Guid.NewGuid(),
                            Name = "Gamma Clan",
                            Tag = "GAMMA",
                            Description = "The less elite Gamma Clan.",
                            IsActive = true
                        },
                        new Clan
                        {
                            Id = Guid.NewGuid(),
                            Name = "Omega Clan",
                            Tag = "OMEGA",
                            Description = "The mysterious Omega Clan.",
                            IsActive = false
                        }
                    };

                    await context.Clans.AddRangeAsync(clans);
                    await context.SaveChangesAsync();
                }

                if (!context.Members.Any())
                {
                    var members = new List<ClanMember>
                    {
                        // Clan Leaders
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "alphaLeader@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.ClanLeader,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "gammaLeader@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Gamma Clan" && y.Tag == "GAMMA").Id,
                            ClanRole = ClanRole.ClanLeader,
                            JoinedAt = DateTime.UtcNow.AddMonths(-2)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "omegaLeader@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Omega Clan" && y.Tag == "OMEGA").Id,
                            ClanRole = ClanRole.ClanLeader,
                            JoinedAt = DateTime.UtcNow.AddMonths(-1)
                        },
                        // Additional Members
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "alphaCoLeader@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.CoLeader,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "alphaElder@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.Elder,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "alphaMember@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.Member,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "alphaRecruit@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.Recruit,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "multiMember@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Alpha Clan" && y.Tag == "ALPHA").Id,
                            ClanRole = ClanRole.Member,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "multiMember@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Gamma Clan" && y.Tag == "GAMMA").Id,
                            ClanRole = ClanRole.Member,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "omegaElder@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Omega Clan" && y.Tag == "OMEGA").Id,
                            ClanRole = ClanRole.Elder,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        },
                        new ClanMember
                        {
                            Id = Guid.NewGuid(),
                            UserId = context.Users.First(x => x.Email == "multiMember@example.com").Id,
                            ClanId = context.Clans.First(y => y.Name == "Omega Clan" && y.Tag == "OMEGA").Id,
                            ClanRole = ClanRole.Recruit,
                            JoinedAt = DateTime.UtcNow.AddMonths(-3)
                        }
                    };

                    await context.Members.AddRangeAsync(members);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var db = new DatabaseService(context);
                await db.ResetDatabaseAsync();
            }
        }
    }
}