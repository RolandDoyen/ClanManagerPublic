using ClanManager.Core;

namespace ClanManager.WEB.Services.Interfaces
{
    public interface ISessionService
    {
        string? GetUserId();
        string? GetUserEmail();
        Role? GetUserRole();
        void SetUserSession(Guid id, string email, Role role);
        void SetUserBan();
        bool IsUserBanned();
        void ClearSession();
    }
}
