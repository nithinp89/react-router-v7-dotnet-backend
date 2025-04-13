
namespace BackendApi.Core.Objects.Core
{
    /// <summary>
    /// Represents a unique identifier for a record, including TenantId, ObjectId, and RowId.
    /// </summary>
    public class RecordId
    {
        /// <summary>
        /// Gets or sets the Tenant ID.
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Gets or sets the Object ID.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the Row ID.
        /// </summary>
        public int RowId { get; set; }
    }
}
