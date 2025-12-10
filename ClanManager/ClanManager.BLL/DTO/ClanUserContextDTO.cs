namespace ClanManager.BLL.DTO
{
    public class ClanUserContextDTO
    {
        public ClanDTO Clan { get; set; } = null!;
        public List<ClanMemberContextDTO> MembersContext { get; set; } = new();
        public bool IsSessionUserMember { get; set; }
        public bool IsSessionUserLeader { get; set; }
        public bool IsSessionUserAdmin { get; set; }
        public bool CanSessionUserJoinQuit { get; set; }
        public string? LeaderEmail { get; set; } = string.Empty;
        public bool IsSessionUserCoLeader { get; set; }
        public Guid SessionUserId { get; set; }
    }
}
