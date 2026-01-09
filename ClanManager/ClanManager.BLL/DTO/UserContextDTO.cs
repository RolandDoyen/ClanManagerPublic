using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    /// <summary>
    /// Data Transfer Object providing a user's details wrapped with permission flags 
    /// relative to the current active session.
    /// </summary>
    public class UserContextDTO
    {
        /// <summary>
        /// The profile data of the user being viewed or managed.
        /// </summary>
        public UserDTO User { get; set; } = null!;

        /// <summary>
        /// Flag indicating if the profile being viewed belongs to the currently logged-in user.
        /// </summary>
        public bool IsOwnProfile { get; set; }

        /// <summary>
        /// The global application role of the user currently browsing the application.
        /// </summary>
        public Role? SessionUserRole { get; set; }

        /// <summary>
        /// Flag indicating if the session user possesses SuperAdmin privileges.
        /// </summary>
        public bool IsUserSuperAdmin { get; set; }

        /// <summary>
        /// Authorization flag determining if the session user has administrative rights over other users.
        /// </summary>
        public bool CanManageUsers { get; set; }

        /// <summary>
        /// Specific permission flag allowing the session user to revoke access for the targeted user.
        /// </summary>
        public bool CanBanUser { get; set; } = true;
    }
}
