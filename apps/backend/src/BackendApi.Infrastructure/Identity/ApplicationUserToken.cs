using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BackendApi.Infrastructure.Identity
{
    /// <summary>
    /// Custom implementation of IdentityUserToken with additional fields for tracking creation and expiration.
    /// </summary>
    public class ApplicationUserToken : IdentityUserToken<int>
    {
        /// <summary>
        /// Gets or sets the date and time when the token was created.
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the token expires.
        /// </summary>
        [Column("expiry_at")]
        public DateTime? ExpiryAt { get; set; }
    }
}
