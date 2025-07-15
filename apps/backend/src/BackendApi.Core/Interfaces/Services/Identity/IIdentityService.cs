using BackendApi.Core.Common;

namespace BackendApi.Core.Interfaces.Services.Identity;

/// <summary>
/// Provides identity-related services for user authentication and authorization.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Retrieves the user name associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The user name if found; otherwise, null.</returns>
    Task<string?> GetUserNameAsync(string userId);

    /// <summary>
    /// Determines whether the specified user is in the given role.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The name of the role to check.</param>
    /// <returns>True if the user is in the role; otherwise, false.</returns>
    Task<bool> IsInRoleAsync(string userId, string role);

    /// <summary>
    /// Authorizes the specified user for a given policy.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="policyName">The name of the policy to authorize.</param>
    /// <returns>True if the user is authorized; otherwise, false.</returns>
    Task<bool> AuthorizeAsync(string userId, string policyName);

    /// <summary>
    /// Creates a new user with the specified user name and password.
    /// </summary>
    /// <param name="userName">The user name for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <returns>A tuple containing the result of the operation and the new user's ID.</returns>
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    /// <summary>
    /// Deletes the user with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    Task<Result> DeleteUserAsync(string userId);
}
