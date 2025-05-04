namespace BackendApi.Core.Common;

/// <summary>
/// Represents the result of an operation, indicating success or failure along with any error messages.
/// This class is used throughout the application to provide a consistent way to handle operation results.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation was successful.</param>
    /// <param name="errors">The collection of error messages if the operation failed.</param>
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Gets the collection of error messages associated with a failed operation.
    /// If the operation was successful, this collection will be empty.
    /// </summary>
    public string[] Errors { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed result with the specified errors.
    /// </summary>
    /// <param name="errors">The errors associated with the failure.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}
