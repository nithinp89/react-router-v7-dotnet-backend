using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Constants;

namespace BackendApi.Core.Models
{
  /// <summary>
  /// Base model for entities that need protection from deletion.
  /// Extends BaseModel with additional protection functionality.
  /// </summary>
  public abstract class ProtectedModel : BaseModel, IProtectedModel
  {
    /// <summary>
    /// Gets or sets whether this entity is protected from deletion.
    /// Value must be either 'Y' or 'N'.
    /// </summary>
    [Column("protected")]
    [Required]
    public bool Protected { get; set; } = GenericContants.PROTECTED_NO;
  }
}
