using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    /// <summary>
    /// Data transfer object representing the formal association between a user and a clan, including their specific role and membership history.
    /// </summary>
    public class ClanMemberDTO
    {
        /// <summary>
        /// The detailed information of the clan to which this member belongs.
        /// </summary>
        public ClanDTO Clan { get; set; } = null!;

        /// <summary>
        /// The profile information of the user participating in the clan.
        /// </summary>
        public UserDTO User { get; set; } = null!;

        /// <summary>
        /// The functional rank or position held by the user within this specific clan's hierarchy.
        /// </summary>
        public ClanRole ClanRole { get; set; }

        /// <summary>
        /// The precise date and time when the user's membership was established.
        /// </summary>
        public DateTime JoinedAt { get; set; }
    }
}
