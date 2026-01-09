using System.ComponentModel.DataAnnotations;

namespace ClanManager.WEB.Models.Clan
{
    /// <summary>
    /// Comprehensive view model for clan data, handling both the core information and the current user's interaction rights.
    /// </summary>
    public class ClanViewModel
    {
        /// <summary>
        /// Unique identifier of the clan.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the clan. Required for display and creation forms.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unique clan tag (shorthand).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Tag { get; set; } = string.Empty;

        /// <summary>
        /// Long-form description of the clan's identity and rules.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

        /// <summary>
        /// Status flag indicating if the clan is operational.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Enriched list of members belonging to this clan for display.
        /// </summary>
        public List<ClanMemberViewModel> Members { get; set; } = new();

        /// <summary>
        /// Logic flag: True if the current visitor is already part of the clan.
        /// </summary>
        public bool IsSessionUserMember { get; set; } = false;

        /// <summary>
        /// Permission flag: True if the visitor has the Leader role in this clan.
        /// </summary>
        public bool IsSessionUserLeader { get; set; } = false;

        /// <summary>
        /// Permission flag: True if the visitor has the Admin role in this clan.
        /// </summary>
        public bool IsSessionUserAdmin { get; set; } = false;

        /// <summary>
        /// UI flag: Determines if the Join/Quit action buttons should be visible to the user.
        /// </summary>
        public bool CanSessionUserJoinQuit { get; set; } = false;

        /// <summary>
        /// Email of the clan leader, used for contact or identification.
        /// </summary>
        public string? LeaderEmail { get; set; } = string.Empty;

        /// <summary>
        /// Permission flag: True if the visitor has the Co-Leader role in this clan.
        /// </summary>
        public bool IsSessionUserCoLeader { get; set; } = false;

        /// <summary>
        /// The unique ID of the current visitor, used to highlight their own row in the member list.
        /// </summary>
        public Guid SessionUserId { get; set; } = Guid.Empty;

        /// <summary>
        /// UI state flag: Indicates if the view should render the description field as an editable text area.
        /// </summary>
        public bool IsEditingDescription { get; set; } = false;
    }
}