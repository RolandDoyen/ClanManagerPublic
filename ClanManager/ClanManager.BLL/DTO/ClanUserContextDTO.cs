namespace ClanManager.BLL.DTO
{
    /// <summary>
    /// Data Transfer Object representing the full state of a clan relative to the current session user.
    /// It encapsulates clan data, member details with permissions, and specific access flags.
    /// </summary>
    public class ClanUserContextDTO
    {
        /// <summary>
        /// The basic information and metadata of the clan.
        /// </summary>
        public ClanDTO Clan { get; set; } = null!;

        /// <summary>
        /// List of clan members wrapped in their respective permission contexts (can kick, can promote, etc.).
        /// </summary>
        public List<ClanMemberContextDTO> MembersContext { get; set; } = new();

        /// <summary>
        /// Flag indicating if the currently logged-in user is a member of this clan.
        /// </summary>
        public bool IsSessionUserMember { get; set; }

        /// <summary>
        /// Flag indicating if the session user is the primary Leader of the clan.
        /// </summary>
        public bool IsSessionUserLeader { get; set; }

        /// <summary>
        /// Flag indicating if the session user has administrative roles (Admin or SuperAdmin) at the application level.
        /// </summary>
        public bool IsSessionUserAdmin { get; set; }

        /// <summary>
        /// Authorization flag determining if the session user can perform a Join or Quit action on this clan.
        /// </summary>
        public bool CanSessionUserJoinQuit { get; set; }

        /// <summary>
        /// The email address of the clan's current Leader.
        /// </summary>
        public string? LeaderEmail { get; set; } = string.Empty;

        /// <summary>
        /// Flag indicating if the session user holds a Co-Leader rank within the clan.
        /// </summary>
        public bool IsSessionUserCoLeader { get; set; }

        /// <summary>
        /// The unique identifier of the user currently browsing the application.
        /// </summary>
        public Guid SessionUserId { get; set; }
    }
}
