using System;
using System.Threading.Tasks;

namespace BackendApi.Core.Interfaces.Services.Identity
{
    /// <summary>
    /// Interface for managing user authentication tokens with expiration dates.
    /// </summary>
    public interface ITokenStoreService
    {
        /// <summary>
        /// Sets a user token with an expiration date.
        /// </summary>
        /// <param name="userId">The ID of the user to set the token for.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The token value.</param>
        /// <param name="expiryDate">The expiration date for the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetUserTokenWithExpiryAsync(string userId, string loginProvider, string name, string value, DateTime expiryDate);
        
        /// <summary>
        /// Sets a refresh token for the specified user with an expiration date.
        /// </summary>
        /// <param name="userId">The ID of the user to set the refresh token for.</param>
        /// <param name="refreshToken">The refresh token value.</param>
        /// <param name="expiryDate">The expiration date for the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetRefreshTokenWithExpiryAsync(string userId, string refreshToken, DateTime expiryDate);
    }
}
