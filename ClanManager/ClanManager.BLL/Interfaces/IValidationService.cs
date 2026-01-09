using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Interfaces
{
    /// <summary>
    /// Contract for centralized business validation logic. Ensures entity existence, session integrity, and permission compliance across the BLL.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates that a session user ID string is present and follows a valid Guid format.
        /// </summary>
        /// <param name="sessionUserId">The raw string ID from the session.</param>
        /// <returns>The parsed <see cref="Guid"/> if successful.</returns>
        /// <exception cref="NoSessionUserException">Thrown when the ID is null, empty, or an invalid format.</exception>
        Guid Check_Session_User(string? sessionUserId);

        /// <summary>
        /// Attempts to find the current session user and automatically validates their administrative role.
        /// </summary>
        /// <param name="sessionGuid">The unique identifier of the session user.</param>
        /// <returns>The <see cref="User"/> entity associated with the session.</returns>
        /// <exception cref="SessionUserNotFoundException">Thrown when the session user no longer exists in the database.</exception>
        /// <exception cref="WrongRoleException">Thrown via <see cref="Check_If_Admin"/> if the user lacks administrative privileges.</exception>
        Task<User> Find_Session_User(Guid sessionGuid);

        /// <summary>
        /// Finds a specific user by their identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the target user.</param>
        /// <returns>The <see cref="User"/> entity if found.</returns>
        /// <exception cref="UserNotFoundException">Thrown when no user matches the provided ID.</exception>
        Task<User> Find_User(Guid userId);

        /// <summary>
        /// Finds a specific clan by its identifier, including its members and their related user data.
        /// </summary>
        /// <param name="clanId">The unique identifier of the clan.</param>
        /// <returns>The <see cref="Clan"/> entity enriched with membership information.</returns>
        /// <exception cref="ClanNotFoundException">Thrown when no clan matches the provided ID.</exception>
        Task<Clan> Find_Clan(Guid clanId);

        /// <summary>
        /// Finds the membership record of the session user within a specific clan.
        /// </summary>
        /// <param name="sessionGuid">The session user's unique identifier.</param>
        /// <param name="clanId">The unique identifier of the clan.</param>
        /// <returns>The <see cref="ClanMember"/> association record.</returns>
        /// <exception cref="SessionUserMemberNotFoundException">Thrown when the session user is not a member of the specified clan.</exception>
        Task<ClanMember> Find_Session_User_Member(Guid sessionGuid, Guid clanId);

        /// <summary>
        /// Finds the membership record of a target user within a specific clan.
        /// </summary>
        /// <param name="targetUserId">The identifier of the user to look for.</param>
        /// <param name="clanId">The unique identifier of the clan.</param>
        /// <returns>The <see cref="ClanMember"/> association record.</returns>
        /// <exception cref="MemberNotFoundException">Thrown when the target user is not found within the clan members.</exception>
        Task<ClanMember> Find_User_Member(Guid targetUserId, Guid clanId);
    }
}
