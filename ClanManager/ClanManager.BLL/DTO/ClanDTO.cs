using System.ComponentModel.DataAnnotations;

namespace ClanManager.BLL.DTO
{
    public class ClanDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Tag { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<ClanMemberDTO> Members { get; set; } = new();
    }
}
