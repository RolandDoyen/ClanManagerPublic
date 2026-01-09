using ClanManager.Core;
using ClanManager.WEB.Services.Interfaces;

namespace ClanManager.WEB.Services
{
    /// <summary>
    /// Service responsible for managing user state and authentication data within the HTTP Session.
    /// Acts as a wrapper around <see cref="IHttpContextAccessor"/> to provide type-safe access to session variables.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor to reach the current HTTP context and its session.</param>
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public string? GetUserId() => _httpContextAccessor.HttpContext?.Session.GetString("UserId");

        /// <inheritdoc />
        public string? GetUserEmail() => _httpContextAccessor.HttpContext?.Session.GetString("UserEmail");

        /// <inheritdoc />
        public Role? GetUserRole()
        {
            var roleStr = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (Enum.TryParse<Role>(roleStr, out var role))
                return role;

            return null;
        }

        /// <inheritdoc />
        public void SetUserSession(Guid id, string email, Role role)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetString("UserId", id.ToString());
            session.SetString("UserEmail", email);
            session.SetString("UserRole", role.ToString());
        }

        /// <inheritdoc />
        public void SetUserBan()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetString("UserIsBanned", "Banned");
        }

        /// <inheritdoc />
        public bool IsUserBanned()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return false;

            return !string.IsNullOrEmpty(session.GetString("UserIsBanned"));
        }

        /// <inheritdoc />
        public void ClearSession() => _httpContextAccessor.HttpContext?.Session.Clear();
    }
}