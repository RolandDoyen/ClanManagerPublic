using ClanManager.Core;

namespace ClanManager.WEB.Services.Interfaces
{
    /// <summary>
    /// Contract for managing user session data, providing an abstraction layer 
    /// over the web server's session storage.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Retrieves the unique identifier of the authenticated user from the session.
        /// </summary>
        /// <returns>A string representation of the User GUID, or null if no user is logged in.</returns>
        string? GetUserId();

        /// <summary>
        /// Retrieves the email address associated with the current session.
        /// </summary>
        /// <returns>The user's email as a string, or null if unavailable.</returns>
        string? GetUserEmail();

        /// <summary>
        /// Extracts and identifies the global security role assigned to the session user.
        /// </summary>
        /// <returns>The <see cref="Role"/> enum value, or null if the session is unauthenticated.</returns>
        Role? GetUserRole();

        /// <summary>
        /// Persists the core identity of a user into the session state.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="email">The user's registered email address.</param>
        /// <param name="role">The application-level role assigned to the user.</param>
        void SetUserSession(Guid id, string email, Role role);

        /// <summary>
        /// Marks the current session as belonging to a restricted or banned user.
        /// </summary>
        void SetUserBan();

        /// <summary>
        /// Evaluates whether the current session carries a ban restriction flag.
        /// </summary>
        /// <returns>True if the user is identified as banned; otherwise, false.</returns>
        bool IsUserBanned();

        /// <summary>
        /// Invalidates the current session and removes all stored user data.
        /// </summary>
        void ClearSession();
    }
}