using ClanManager.Core;
using System.ComponentModel.DataAnnotations;

namespace ClanManager.BLL.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        public Role Role { get; set; } = Role.User;

        public bool IsBanned { get; set; } = false;
    }
}
