using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    /// <summary>
    /// Data transfer object representing a clan member's information within a specific operational context, 
    /// including permissions for UI action authorization.
    /// </summary>
    public class ClanMemberContextDTO
    {
        /// <summary>
        /// The profile data of the user associated with this membership.
        /// </summary>
        public UserDTO User { get; set; } = null!;

        /// <summary>
        /// The specific role held by the user within the clan (e.g., Leader, Co-Leader, Member).
        /// </summary>
        public ClanRole ClanRole { get; set; }

        /// <summary>
        /// UI permission flag indicating if the current session user is authorized to modify this member's role.
        /// </summary>
        public bool CanChangeMemberRole { get; set; }

        /// <summary>
        /// UI permission flag indicating if the current session user is authorized to expel this member from the clan.
        /// </summary>
        public bool CanRemoveMember { get; set; }
    }
}
