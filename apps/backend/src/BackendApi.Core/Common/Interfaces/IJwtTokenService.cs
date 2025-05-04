using System.Security.Claims;
using BackendApi.Core.Models.Identity;

namespace BackendApi.Core.Common.Interfaces;

/// <summary>
/// Service for generating and validating JWT tokens for authentication.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user claims.
    /// </summary>
    /// <param name="userId">The user identifier for whom the token is being generated.</param>
    /// <param name="claims">The claims to include in the token.</param>
    /// <returns>A result containing the token information if successful, or error messages if failed.</returns>
    Task<(Result Result, string Token, DateTime ExpiresAt, string RefreshToken)> GenerateTokenAsync(string userId, IEnumerable<Claim> claims);

    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>A result indicating whether the token is valid.</returns>
    Task<Result> ValidateTokenAsync(string token);
}
