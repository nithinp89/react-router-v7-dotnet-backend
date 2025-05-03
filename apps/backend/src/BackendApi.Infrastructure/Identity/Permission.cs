using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Infrastructure.Identity
{
    /// <summary>
    /// Represents a standard permission in the system.
    /// </summary>
    [Table("permissions")]
    public class Permission
    {
        /// <summary>
        /// Gets or sets the unique identifier for the permission.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the permission.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the normalized name of the permission (typically uppercase, unique).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string NormalizedName { get; set; } = null!;
    }
}
