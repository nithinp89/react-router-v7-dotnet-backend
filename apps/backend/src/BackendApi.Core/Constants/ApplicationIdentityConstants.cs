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

        /// <summary>
        /// User type for system administrators with full access to the system.
        /// </summary>
        public const string USER_TYPE_ADMINISTRATOR = "Administrator";

        /// <summary>
        /// User type for technical operations and not allowed to login.
        /// </summary>
        public const string USER_TYPE_TECHNICAL = "Technical";

        /// <summary>
        /// User type for agents who handle customer requests and provide support.
        /// </summary>
        public const string USER_TYPE_AGENT = "Agent";

        /// <summary>
        /// User type for customers who use the system's services.
        /// </summary>
        public const string USER_TYPE_CUSTOMER = "Customer";

        /// <summary>
        /// User type for external collaborators who work with the system on specific projects.
        /// </summary>
        public const string USER_TYPE_COLLABORATOR = "Collaborator";
    }
}
