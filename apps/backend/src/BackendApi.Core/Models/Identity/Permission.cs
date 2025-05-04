using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Constants;
using BackendApi.Core.Models;

namespace BackendApi.Core.Models.Identity
{
  /// <summary>
  /// Represents a standard permission in the system.
  /// </summary>
  [Table("permissions")]
  public class Permission : BaseModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the permission.
    /// </summary>
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the permission.
    /// </summary>
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the normalized name of the permission (typically uppercase, unique).
    /// </summary>
    [Column("normalized_name")]
    [Required]
    [MaxLength(100)]
    public string NormalizedName { get; set; } = null!;

    /// <summary>
    /// Gets or sets a description for the role.
    /// </summary>
    [Column("description")]
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = null!;
  }
}
