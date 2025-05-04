using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackendApi.Api.DTOs.Auth
{
  /// <summary>
  /// Represents a problem details response for invalid credentials.
  /// </summary>
  public class AuthProblemResponse : ValidationProblemDetails
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthProblemResponse"/> class with default values for invalid credentials errors.
    /// </summary>
    public static ProblemHttpResult InvalidCredentialsProblem(string? title = null)
    {
      var problemDetails = new ValidationProblemDetails()
      {
        Title = title ?? "Invalid Credentials. Please check your email and password.",
        Status = StatusCodes.Status400BadRequest,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
      };
      return TypedResults.Problem(problemDetails);
    }
  }
}
