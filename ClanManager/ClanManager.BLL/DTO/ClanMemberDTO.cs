using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    public class ClanMemberDTO
    {
        public ClanDTO Clan { get; set; } = null!;
        public UserDTO User { get; set; } = null!;

        public ClanRole ClanRole { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
