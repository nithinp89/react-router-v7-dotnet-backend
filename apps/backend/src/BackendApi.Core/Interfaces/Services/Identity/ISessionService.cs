namespace BackendApi.Core.Interfaces.Services.Identity
{
    /// <summary>
    /// Provides functionality for generating secure session identifiers.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Generates a cryptographically secure random session identifier string.
        /// </summary>
        /// <param name="length">The length of the session identifier to generate. Default is 32.</param>
        /// <returns>A secure, random session identifier string.</returns>
        string GenerateSecureSessionId(int length = 32);
    }
}
