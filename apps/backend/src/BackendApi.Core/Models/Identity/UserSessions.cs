using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Helpers;

namespace BackendApi.Core.Models.Identity
{
  /// <summary>
  /// Represents a user session in the system, tracking authentication tokens and related metadata.
  /// </summary>
  [Table("user_sessions")]
  public class UserSessions : BaseModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the user session.
    /// </summary>
    [Column("id")]
    [Key]
    [MaxLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets or sets the identifier of the user associated with this session.
    /// </summary>
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the browser information for this session.
    /// </summary>
    [Column("browser")]
    [Required]
    [MaxLength(255)]
    public string Browser { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the last token was issued for this session (UTC).
    /// </summary>
    [Column("last_token_issued_at")]
    [Required]
    public DateTime LastTokenIssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of tokens issued for this session.
    /// </summary>
    [Column("nb_tokens")]
    [Required]
    public int NbTokens { get; set; } = 1;
  }
}
