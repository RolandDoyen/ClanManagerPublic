using System.ComponentModel.DataAnnotations;

namespace ClanManager.WEB.Models.Clan
{
    public class ClanViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Tag { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public List<ClanMemberViewModel> Members { get; set; } = new();

        public bool IsSessionUserMember { get; set; } = false;
        public bool IsSessionUserLeader { get; set; } = false;
        public bool IsSessionUserAdmin { get; set; } = false;
        public bool CanSessionUserJoinQuit { get; set; } = false;
        public string? LeaderEmail { get; set; } = string.Empty;
        public bool IsSessionUserCoLeader { get; set; } = false;
        public Guid SessionUserId { get; set; } = Guid.Empty;
        public bool IsEditingDescription { get; set; } = false;
    }
}
