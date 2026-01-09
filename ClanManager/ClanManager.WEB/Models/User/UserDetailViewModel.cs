using ClanManager.Core;

namespace ClanManager.WEB.Models.User
{
    /// <summary>
    /// View model representing the comprehensive details of a user, 
    /// enriched with permission flags for conditional UI rendering.
    /// </summary>
    public class UserDetailViewModel
    {
        /// <summary>
        /// Unique identifier of the user being displayed.
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The current application-level role of the user (e.g., Admin, User).
        /// </summary>
        public Role Role { get; set; } = Role.User;

        /// <summary>
        /// Flag indicating if the user's account is currently suspended.
        /// </summary>
        public bool IsBanned { get; set; } = false;

        /// <summary>
        /// The role of the user currently browsing the page, used to determine available actions.
        /// </summary>
        public Role? SessionUserRole { get; set; } = null;

        /// <summary>
        /// Indicates if the profile being viewed is the one belonging to the current session user.
        /// </summary>
        public bool IsOwnProfile { get; set; } = false;

        /// <summary>
        /// Security flag identifying if the session user has absolute administrative authority.
        /// </summary>
        public bool IsUserSuperAdmin { get; set; } = false;

        /// <summary>
        /// Determines if the current session has the rights to perform management actions on this user.
        /// </summary>
        public bool CanManageUsers { get; set; } = false;

        /// <summary>
        /// Specifically determines if the "Ban/Unban" action should be available in the UI.
        /// </summary>
        public bool CanBanUser { get; set; } = true;
    }
}