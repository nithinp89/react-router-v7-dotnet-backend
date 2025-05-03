namespace BackendApi.Api.DTOs
{
    /// <summary>
    /// Represents a request to obtain a JWT token using user credentials.
    /// </summary>
    public class GetTokenRequest : BaseRequest
    {
        /// <summary>
        /// Email of the user requesting authentication.
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Password of the user requesting authentication.
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
