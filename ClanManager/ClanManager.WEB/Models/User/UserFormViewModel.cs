using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.WEB.Models.User
{
    /// <summary>
    /// View model representing the data structure for user entry forms, 
    /// such as registration and authentication.
    /// </summary>
    public class UserFormViewModel
    {
        /// <summary>
        /// User's email address. Validated for format and length to ensure 
        /// compatibility with the identity system and database constraints.
        /// </summary>
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's plain-text password. 
        /// Decorated with <see cref="PasswordPropertyTextAttribute"/> to ensure secure 
        /// handling and masked display in UI components.
        /// </summary>
        [Required]
        [MaxLength(200)]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = string.Empty;
    }
}
