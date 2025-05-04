namespace BackendApi.Core.Objects.Identity;

/// <summary>
/// Contains settings for JWT token generation and validation.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the secret key used to sign JWT tokens.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issuer of the JWT token.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience of the JWT token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 15;
}
