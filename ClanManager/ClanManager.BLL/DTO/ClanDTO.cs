using System.ComponentModel.DataAnnotations;

namespace ClanManager.BLL.DTO
{
    public class ClanDTO
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Tag { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<ClanMemberDTO> Members { get; set; } = new();
    }
}
