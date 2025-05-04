using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Constants;

namespace BackendApi.Core.Models
{
  /// <summary>
  /// Base model for tracking creation, modification, and soft deletion metadata.
  /// </summary>
  public abstract class BaseModel : IBaseModel
{
  /// <summary>
  /// Gets or sets whether this permission is protected from deletion.
  /// Value must be either 'Y' or 'N'.
  /// </summary>
  [Column("protected")]
  [Required]
  public bool Protected { get; set; } = GenericContants.PROTECTED_NO;

  /// <summary>
  /// The date and time when the entity was created (UTC).
  /// </summary>
  [Column("created_at")]
  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// The identifier of the user who created the entity.
  /// </summary>
  [Column("created_by")]
  [Required]
  public required int CreatedBy { get; set; }

  /// <summary>
  /// The date and time when the entity was last modified (UTC).
  /// </summary>
  [Column("modified_at")]
  [Required]
  public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// The identifier of the user who last modified the entity.
  /// </summary>
  [Column("modified_by")]
  [Required]
  public required int ModifiedBy { get; set; }

  /// <summary>
  /// The date and time when the entity was soft-deleted (UTC), or null if not deleted.
  /// </summary>
  [Column("deleted_at")]
  public DateTime? DeletedAt { get; set; }
}
}
