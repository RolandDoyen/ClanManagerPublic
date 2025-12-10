using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    public class ClanMemberContextDTO
    {
        public UserDTO User { get; set; } = null!;
        public ClanRole ClanRole { get; set; }
        public bool CanChangeMemberRole { get; set; }
        public bool CanRemoveMember { get; set; }
    }
}
