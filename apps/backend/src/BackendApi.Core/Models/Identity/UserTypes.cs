using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Constants;
using BackendApi.Core.Models;

namespace BackendApi.Core.Models.Identity
{
  /// <summary>
  /// Represents a user type in the system (e.g., Admin, Customer, Agent).
  /// </summary>
  [Table("user_types")]
  public class UserTypes : ProtectedModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the user type.
    /// </summary>
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the user type.
    /// </summary>
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets a description for the user type.
    /// </summary>
    [Column("description")]
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = null!;
  }
}
