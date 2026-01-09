using ClanManager.BLL.DTO;
using ClanManager.Core;

namespace ClanManager.WEB.Models.Clan
{
    /// <summary>
    /// View model representing a single clan member within the UI, enriched with contextual permission flags for management actions.
    /// </summary>
    public class ClanMemberViewModel
    {
        /// <summary>
        /// Gets or sets the data transfer object containing the profile details of the user.
        /// </summary>
        public UserDTO User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's current rank or position within the clan's hierarchy.
        /// </summary>
        public ClanRole ClanRole { get; set; }

        /// <summary>
        /// UI flag indicating if the viewer is authorized to promote or demote this specific member.
        /// </summary>
        public bool CanChangeMemberRole { get; set; }

        /// <summary>
        /// UI flag indicating if the viewer is authorized to kick this member out of the clan.
        /// </summary>
        public bool CanRemoveMember { get; set; }
    }
}