using ClanManager.DAL.DAO;
using Microsoft.EntityFrameworkCore;

namespace ClanManager.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Default values
            builder.Entity<ClanMember>()
                   .Property(cm => cm.JoinedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Clan>()
                   .Property(c => c.IsActive)
                   .HasDefaultValue(true);

            builder.Entity<User>()
                   .Property(u => u.IsBanned)
                   .HasDefaultValue(false);

            // Unique constraints
            builder.Entity<User>()
                   .HasIndex(u => u.Email)
                   .IsUnique();

            builder.Entity<ClanMember>()
                   .HasIndex(cm => new { cm.ClanId, cm.UserId })
                   .IsUnique();

            // Property constraints
            builder.Entity<Clan>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Clan>()
                .Property(c => c.Tag)
                .IsRequired()
                .HasMaxLength(20);

            builder.Entity<Clan>()
                .Property(c => c.Description)
                .HasMaxLength(500);

            builder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(200);

            // Relationships
            builder.Entity<ClanMember>()
                .HasOne(cm => cm.Clan)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ClanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClanMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.Clans)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation collections init (avoid null refs)
            builder.Entity<Clan>()
                .Navigation(c => c.Members)
                .AutoInclude();

            builder.Entity<User>()
                .Navigation(u => u.Clans)
                .AutoInclude();
        }

        public DbSet<Clan> Clans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClanMember> Members { get; set; }
    }
}
