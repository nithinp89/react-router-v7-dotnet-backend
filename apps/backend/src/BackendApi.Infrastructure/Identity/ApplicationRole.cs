using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Models;
using BackendApi.Core.Constants;

namespace BackendApi.Infrastructure.Identity;

/// <summary>
/// Represents an application role with extended audit information.
/// </summary>
public class ApplicationRole : IdentityRole<int>, IBaseModel
{
  /// <summary>
  /// Gets or sets a description for the role.
  /// </summary>
  [Column("description")]
  public string? Description { get; set; }

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
