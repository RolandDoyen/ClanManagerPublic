using ClanManager.Core;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        public Role Role { get; set; }

        public bool IsBanned { get; set; }

        public List<ClanMember> Clans { get; set; } = new();
    }
}
