using BackendApi.Api.DTOs;
using BackendApi.Api.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BackendApi.Infrastructure.Identity;
using System.Linq;

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
    /// <returns>
    /// A result that can be:
    /// <see cref="Ok{GetTokenResponse}"/>,
    /// <see cref="ValidationProblem"/>, or
    /// <see cref="ProblemHttpResult"/>
    /// </returns>
    public static async Task<Results<Ok<GetTokenResponse>, ValidationProblem, ProblemHttpResult>> GetToken(GetTokenRequest request, IValidator<GetTokenRequest> validator, HttpContext httpContext)
    {
        await Task.CompletedTask;

        ValidationResult validationResult = await validator.ValidateAsync(request);


        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        // Resolve UserManager, SignInManager, and IConfiguration from DI
        var userManager = httpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
        var signInManager = httpContext.RequestServices.GetService(typeof(SignInManager<ApplicationUser>)) as SignInManager<ApplicationUser>;
        var configuration = httpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

        if (userManager == null || signInManager == null || configuration == null)
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Title = "Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            return TypedResults.Problem(problemDetails);
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Title = "Invalid Credentials",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
            return TypedResults.Problem(problemDetails);
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Title = "Invalid Credentials",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
            return TypedResults.Problem(problemDetails);
        }

        // JWT generation
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? ""),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? "")
        };
        var userRoles = await userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)));

        var key = configuration["Jwt:Key"] ?? "f02aecc8457e71afc1bdef98da64a1d0e9591c68945868afb60c2eb45ede7258";
        var issuer = configuration["Jwt:Issuer"] ?? "MyAppAuth";
        var audience = configuration["Jwt:Audience"] ?? "MyApp";
        var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(30);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

        var response = new GetTokenResponse
        {
            Token = jwt,
            ExpiresAt = expires,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Email = user.Email ?? ""
        };
        return TypedResults.Ok(response);
    }
}
