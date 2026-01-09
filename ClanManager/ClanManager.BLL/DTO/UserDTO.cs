using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    /// <summary>
    /// Data Transfer Object representing a User.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Unique identifier of the User.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Email address used for identification.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed or raw password string depending on the context (Login/Registration).
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Security role defining the user's permissions within the application.
        /// </summary>
        public Role Role { get; set; } = Role.User;

        /// <summary> 
        /// Indicates whether the user's access has been revoked by an administrator.
        /// </summary>
        public bool IsBanned { get; set; } = false;
    }
}
