using System.Security.Cryptography;
using BackendApi.Core.Interfaces.Services.Identity;

namespace BackendApi.Application.Services.Identity
{
    /// <inheritdoc/>
    public class SessionService : ISessionService
    {
        /// <inheritdoc/>
        public string GenerateSecureSessionId(int length = 32)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Session ID length must be positive.");

            // Use a secure random number generator to generate bytes
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            // Convert to a URL-safe base64 string
            string sessionId = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            // Ensure the string is the requested length (may be shorter due to base64 encoding)
            return sessionId.Length > length ? sessionId.Substring(0, length) : sessionId;
        }
    }
}
