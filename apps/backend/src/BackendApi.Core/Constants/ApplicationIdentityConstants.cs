using System.Security.Cryptography.X509Certificates;

namespace BackendApi.Core.Constants
{
    /// <summary>
    /// Contains identity-related constants used across the application.
    /// </summary>
    public static class ApplicationIdentityConstants
    {
        /// <summary>
        /// Represents a default user ID to update CreatedBy and ModifiedBy fields.
        /// </summary>
        public const int DEFAULT_USER_ID = 10001;

        /// <summary>
        /// The name of the refresh token used for authentication.
        /// </summary>
        public const string REFRESH_TOKEN = "RefreshToken";

        /// <summary>
        /// The name of the access token used for authentication.
        /// </summary>
        public const string ACCESS_TOKEN = "AccessToken";

        /// <summary>
        /// The name of the login provider for session-based authentication.
        /// </summary>
        public const string SESSION_LOGIN_PROVIDER = "Session";

        /// <summary>
        /// The key name used for storing session information.
        /// </summary>
        public const string SESSION_KEY_NAME = "_Session_Core";
    }
}
