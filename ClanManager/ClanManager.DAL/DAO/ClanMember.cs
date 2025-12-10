using ClanManager.Core;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    public class ClanMember
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ClanId { get; set; }
        public Clan Clan { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public ClanRole ClanRole { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; }
    }
}
