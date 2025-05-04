using BackendApi.Api.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BackendApi.Infrastructure.Identity;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BackendApi.Api.DTOs;
using BackendApi.Api.DTOs.Auth;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Core.Common;
using BackendApi.Core.Models.Identity;

namespace BackendApi.Api.Features.Auth;

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
    /// <param name="userManager">The UserManager service for managing user accounts.</param>
    /// <param name="signInManager">The SignInManager service for handling sign-in operations.</param>
    /// <param name="jwtTokenService">The JWT token service for generating authentication tokens.</param>
    /// <returns>
    /// A result that can be:
    /// <see cref="Ok{GetTokenResponse}"/>,
    /// <see cref="ValidationProblem"/>, or
    /// <see cref="ProblemHttpResult"/>
    /// </returns>
    public static async Task<Results<Ok<GetTokenResponse>, ValidationProblem, ProblemHttpResult>> GetToken(
        GetTokenRequest request,
        IValidator<GetTokenRequest> validator,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        await Task.CompletedTask;

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        if (userManager == null || signInManager == null || jwtTokenService == null)
        {
            return GenericProblemResponse.ServerErrorProblem();
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return AuthProblemResponse.InvalidCredentialsProblem();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return AuthProblemResponse.InvalidCredentialsProblem();
        }

        // Prepare claims for JWT token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };
        var userRoles = await userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Generate JWT token using the service
        var tokenResult = await jwtTokenService.GenerateTokenAsync(user.Id.ToString(), claims);
        
        if (!tokenResult.Result.Succeeded)
        {
            return GenericProblemResponse.ServerErrorProblem("Failed to generate authentication token");
        }

        var response = new GetTokenResponse
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            RefreshToken = tokenResult.RefreshToken,
            UserId = user.Id,
            Email = user.Email ?? ""
        };
        return TypedResults.Ok(response);
    }
}
