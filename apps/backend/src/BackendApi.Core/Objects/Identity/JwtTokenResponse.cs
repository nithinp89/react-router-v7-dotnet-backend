using System;

namespace BackendApi.Core.Common
{
    /// <summary>
    /// Represents a response containing JWT token information.
    /// </summary>
    public class JwtTokenResponse
    {
        /// <summary>
        /// Gets or sets the JWT access token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the access token.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the refresh token.
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
