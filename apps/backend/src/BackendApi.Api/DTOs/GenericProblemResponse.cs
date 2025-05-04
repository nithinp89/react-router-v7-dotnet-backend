using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackendApi.Api.DTOs
{
  /// <summary>
  /// Represents a generic problem details response for server errors.
  /// </summary>
  public class GenericProblemResponse : ValidationProblemDetails
  {

    /// <summary>
    /// Creates a ProblemHttpResult representing a generic server error (HTTP 500).
    /// </summary>
    public static ProblemHttpResult ServerErrorProblem(string? title = null)
    {
      var problemDetails = new ValidationProblemDetails()
      {
        Title = title ?? "A technical issue has been occurred.",
        Status = StatusCodes.Status500InternalServerError,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
      };
      return TypedResults.Problem(problemDetails);
    }
  }
}
