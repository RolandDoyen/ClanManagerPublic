using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.WEB.Models.User
{
    public class UserFormViewModel
    {
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = string.Empty;
    }
}
