using ClanManager.Core;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.DAL.DAO
{
    /// <summary>
    /// Database entity representing a registered user within the system.
    /// Maps to the physical 'Users' table.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key for the user record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique and required email address used for authentication.
        /// Maximum length constrained to 150 characters for database optimization.
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Securely hashed password string. 
        /// Maximum length constrained to 200 characters to accommodate various hashing algorithms.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Global security role determining the user's administrative level.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Persistent flag indicating if the user is prohibited from accessing the system.
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// Navigation property for the join table representing the clans this user has joined.
        /// </summary>
        public List<ClanMember> Clans { get; set; } = new();
    }
}
