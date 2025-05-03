using BackendApi.Api.DTOs;
using BackendApi.Api.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Api.Features;

/// <summary>
/// Provides authentication-related endpoint handlers for the API.
/// </summary>
public static class AuthHandlers
{
    /// <summary>
    /// Handles the POST request to authenticate a user and return a JWT token response.
    /// </summary>
    /// <param name="request">The authentication request containing user credentials.</param>
    /// <param name="validator">The validator for the authentication request.</param>
    /// <param name="httpContext">The current HTTP context, used to access request headers such as x-correlation-id.</param>
    /// <returns>A <see cref="Results{Ok{GetTokenResponse}, ValidationProblem, ProblemHttpResult}"/> representing the authentication outcome.</returns>
    public static async Task<Results<Ok<GetTokenResponse>, ValidationProblem, ProblemHttpResult>> GetToken(GetTokenRequest request, IValidator<GetTokenRequest> validator, HttpContext httpContext)
    {
        await Task.CompletedTask;

        ValidationResult validationResult = await validator.ValidateAsync(request);


        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        if (request.Email != "admin@example.com" || request.Password != "password") {
            var problemDetails = new ValidationProblemDetails()
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
            return TypedResults.Problem(problemDetails);
        }

        var response = new GetTokenResponse
        {
            Token = "",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RefreshToken = "",
            UserId = "",
            Email = ""
        };
        return TypedResults.Ok(response);
    }
}
