using BackendApi.Core.Common;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendApi.Core.Interfaces.Services.Identity
{
    /// <summary>
    /// Interface for JWT token generation and validation services.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token and refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to generate tokens for.</param>
        /// <param name="claims">The claims to include in the token.</param>
        /// <returns>A result containing the token, expiration date, and refresh token.</returns>
        Task<(Result Result, string Token, DateTime ExpiresAt, string RefreshToken)> GenerateTokensAsync(string userId, IEnumerable<Claim> claims);
        
        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>A result indicating success or failure of validation.</returns>
        Task<Result> ValidateTokenAsync(string token);
    }
}

