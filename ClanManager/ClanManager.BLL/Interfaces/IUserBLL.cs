using ClanManager.BLL.DTO;
using ClanManager.Core;
using ClanManager.Core.Exceptions;

namespace ClanManager.BLL.Interfaces
{    
    /// <summary>
    /// Provides business logic operations related to users, including creation, retrieval,
    /// authentication, role management, and ban toggling.
    /// Ensures all operations respect business rules and roles (Admin/SuperAdmin).
    /// </summary>
    public interface IUserBLL
    {
        /// <summary>
        /// Creates a new user with a normalized email and hashed password.
        /// </summary>
        /// <param name="model">User data including Email and Password.</param>
        /// <returns>The created <see cref="UserDTO"/>.</returns>
        /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same email exists.</exception>
        Task<UserDTO> CreateAsync(UserDTO model);

        /// <summary>
        /// Retrieves all users sorted by email.
        /// </summary>
        /// <returns>A list of <see cref="UserDTO"/>.</returns>
        Task<List<UserDTO>> GetAllAsync();

        /// <summary>
        /// Retrieves a user and provides context about the session user's relation and permissions.
        /// </summary>
        /// <param name="userId">The ID of the target user.</param>
        /// <param name="sessionUserId">The ID of the session user (nullable).</param>
        /// <returns>A <see cref="UserContextDTO"/> with user data and session context.</returns>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if the target user does not exist.</exception>
        Task<UserContextDTO> GetByIdAsync(Guid userId, string? sessionUserId);

        /// <summary>
        /// Authenticates a user by email and password.
        /// Email is normalized before lookup, and password is verified using bcrypt.
        /// </summary>
        /// <param name="model">User data containing Email and Password.</param>
        /// <returns>The authenticated <see cref="UserDTO"/>.</returns>
        /// <exception cref="UserEmailNotFoundException">Thrown if no user matches the email.</exception>
        /// <exception cref="PasswordValidationException">Thrown if the password is incorrect.</exception>
        Task<UserDTO> LoginAsync(UserDTO model);

        /// <summary>
        /// Updates the role of a user after validating business rules.
        /// Rules include: cannot change own role, cannot assign or modify SuperAdmin, role must exist.
        /// </summary>
        /// <param name="userId">ID of the user whose role is being updated.</param>
        /// <param name="sessionUserId">ID of the session user performing the action.</param>
        /// <param name="newRole">The new role to assign, represented as a value of the <see cref="Role"/> enum.</param>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if target user does not exist.</exception>
        /// <exception cref="RoleChangeException">Thrown if rules for role change are violated.</exception>
        Task ChangeRoleAsync(Guid userId, string? sessionUserId, Role newRole);

        /// <summary>
        /// Toggles the ban status of a user.
        /// Also handles role downgrade, member removal, and business rules regarding banning.
        /// Business rules includes role-based authorization, protection against self-ban, and restrictions related to Admin, SuperAdmin, and clan leadership.
        /// </summary>
        /// <param name="userId">ID of the user whose ban status is being toggled.</param>
        /// <param name="sessionUserId">ID of the session user performing the action.</param>
        /// <exception cref="NoSessionUserException">Thrown if sessionUserId is missing or invalid.</exception>
        /// <exception cref="SessionUserNotFoundException">Thrown if session user does not exist.</exception>
        /// <exception cref="WrongRoleException">Thrown if session user does not have Admin/SuperAdmin privileges.</exception>
        /// <exception cref="UserNotFoundException">Thrown if target user does not exist.</exception>
        /// <exception cref="ToggleBanException">Thrown if the ban action violates business rules (self, clan leader, SuperAdmin, etc.).</exception>
        Task ToggleBanAsync(Guid userId, string? sessionUserId);
    }
}
