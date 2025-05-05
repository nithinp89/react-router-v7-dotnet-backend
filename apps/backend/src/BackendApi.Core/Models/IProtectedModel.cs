namespace BackendApi.Core.Models
{
  /// <summary>
  /// Interface for entities that need protection from deletion.
  /// Extends IBaseModel with additional protection functionality.
  /// </summary>
  public interface IProtectedModel : IBaseModel
  {
    /// <summary>
    /// Gets or sets whether this entity is protected from deletion.
    /// Value must be either 'Y' or 'N'.
    /// </summary>
    bool Protected { get; set; }
  }
}
