using System;

namespace BackendApi.Core.Models
{
  /// <summary>
  /// Interface for tracking creation, modification, and soft deletion metadata.
  /// </summary>
  public interface IBaseModel
  {
    /// <summary>
    /// Gets or sets the date and time when the entity was created (UTC).
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    int CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified (UTC).
    /// </summary>
    DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    int ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was soft-deleted (UTC), or null if not deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }
  }
}
