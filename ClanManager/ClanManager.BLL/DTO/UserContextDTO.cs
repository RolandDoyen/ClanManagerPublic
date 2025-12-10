using ClanManager.Core;

namespace ClanManager.BLL.DTO
{
    public class UserContextDTO
    {
        public UserDTO User { get; set; } = null!;
        public bool IsOwnProfile { get; set; }
        public Role? SessionUserRole { get; set; }
        public bool IsUserSuperAdmin { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanBanUser { get; set; } = true;
    }
}
