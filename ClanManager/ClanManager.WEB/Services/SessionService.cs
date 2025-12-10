using ClanManager.Core;
using ClanManager.WEB.Services.Interfaces;

namespace ClanManager.WEB.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId() => _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        public string? GetUserEmail() => _httpContextAccessor.HttpContext?.Session.GetString("UserEmail");

        public Role? GetUserRole()
        {
            var roleStr = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (Enum.TryParse<Role>(roleStr, out var role))
                return role;

            return null;
        }

        public void SetUserSession(Guid id, string email, Role role)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetString("UserId", id.ToString());
            session.SetString("UserEmail", email);
            session.SetString("UserRole", role.ToString());
        }

        public void SetUserBan()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetString("UserIsBanned", "Banned");
        }

        public bool IsUserBanned()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return false;

            return !string.IsNullOrEmpty(session.GetString("UserIsBanned"));
        }

        public void ClearSession() => _httpContextAccessor.HttpContext?.Session.Clear();
    }
}
