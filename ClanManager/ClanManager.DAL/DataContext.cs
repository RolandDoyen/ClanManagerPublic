using ClanManager.DAL.DAO;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.DAL
{
    /// <summary>
    /// Database context for the ClanManager application.
    /// Handles entity configuration and database connection.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by this context (connection string, provider, etc.).</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the model definitions, relationships, and constraints using the Fluent API.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Default values
            builder.Entity<ClanMember>()
                   .Property(x => x.JoinedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Clan>()
                   .Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Entity<User>()
                   .Property(x => x.IsBanned)
                   .HasDefaultValue(false);

            // Unique constraints
            builder.Entity<User>()
                   .HasIndex(x => x.Email)
                   .IsUnique();

            builder.Entity<ClanMember>()
                   .HasIndex(x => new { x.ClanId, x.UserId })
                   .IsUnique();

            // Property constraints
            builder.Entity<Clan>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Clan>()
                .Property(x => x.Tag)
                .IsRequired()
                .HasMaxLength(20);

            builder.Entity<Clan>()
                .Property(x => x.Description)
                .HasMaxLength(500);

            builder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Entity<User>()
                .Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(200);

            // Relationships
            builder.Entity<ClanMember>()
                .HasOne(x => x.Clan)
                .WithMany(y => y.Members)
                .HasForeignKey(x => x.ClanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClanMember>()
                .HasOne(x => x.User)
                .WithMany(y => y.Clans)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation collections init (avoid null refs)
            builder.Entity<Clan>()
                .Navigation(x => x.Members)
                .AutoInclude();

            builder.Entity<User>()
                .Navigation(x => x.Clans)
                .AutoInclude();
        }

        /// <summary>
        /// Gets or sets the collection of Clans.
        /// </summary>
        public DbSet<Clan> Clans { get; set; }

        /// <summary>
        /// Gets or sets the collection of Users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the collection of ClanMembers (Join table).
        /// </summary>
        public DbSet<ClanMember> Members { get; set; }
    }
}
