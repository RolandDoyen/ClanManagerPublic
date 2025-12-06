using ClanManager.BLL.DTO;
using ClanManager.Core;

namespace ClanManager.WEB.Models.Clan
{
    public class ClanMemberViewModel
    {
        public UserDTO User { get; set; } = null!;
        public ClanRole ClanRole { get; set; }
        public bool CanChangeMemberRole { get; set; }
        public bool CanRemoveMember { get; set; }
    }
}
