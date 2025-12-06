using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    public class Clan
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Tag { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<ClanMember> Members { get; set; } = new();
    }
}
