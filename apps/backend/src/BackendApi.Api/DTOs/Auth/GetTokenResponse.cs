namespace BackendApi.Api.DTOs
{
    /// <summary>
    /// Represents the response returned after a successful authentication request, containing user and token information.
    /// </summary>
    public class GetTokenResponse
    {
      /// <summary>
      /// Unique identifier of the authenticated user.
      /// </summary>
      public string UserId { get; set; } = null!;
      /// <summary>
      /// Email address of the authenticated user.
      /// </summary>
      public string Email { get; set; } = null!;
        /// <summary>
        /// JWT access token issued for the authenticated user.
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// Expiration date and time of the access token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        /// <summary>
        /// Refresh token used to obtain new access tokens.
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
