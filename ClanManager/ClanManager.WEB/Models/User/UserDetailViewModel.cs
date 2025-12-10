using ClanManager.Core;

namespace ClanManager.WEB.Models.User
{
    public class UserDetailViewModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
        public bool IsBanned { get; set; } = false;
        public Role? SessionUserRole { get; set; } = null;
        public bool IsOwnProfile { get; set; } = false;
        public bool IsUserSuperAdmin { get; set; } = false;
        public bool CanManageUsers { get; set; } = false;
        public bool CanBanUser { get; set; } = true;
    }
}
