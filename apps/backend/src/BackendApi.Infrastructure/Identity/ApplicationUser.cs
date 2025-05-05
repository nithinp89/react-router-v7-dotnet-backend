using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Identity;
using BackendApi.Core.Constants;

namespace BackendApi.Infrastructure.Identity;

/// <summary>
/// Represents an application user with extended profile information.
/// </summary>
public class ApplicationUser : IdentityUser<int>, IBaseModel
{
  /// <summary>
  /// Gets or sets the first name of the user.
  /// </summary>
  [Column("first_name")]
  [Required]
  public required string FirstName { get; set; }

  /// <summary>
  /// Gets or sets the last name of the user.
  /// </summary>
  [Column("last_name")]
  [Required]
  public required string LastName { get; set; }

  /// <summary>
  /// Gets or sets the user type identifier.
  /// </summary>
  [Column("user_type_id")]
  [Required]
  public int UserTypeId { get; set; }

  /// <summary>
  /// Gets or sets the user type associated with this user.
  /// </summary>
  [ForeignKey("UserTypeId")]
  public UserTypes? UserType { get; set; }

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

  /// <summary>
  /// Gets or sets whether the user account is active.
  /// Active accounts can log in and access the system.
  /// </summary>
  [Column("active")]
  [Required]
  public bool Active { get; set; } = true;
}