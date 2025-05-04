using BackendApi.Core.Common.Interfaces;
using BackendApi.Core.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackendApi.Core.Objects.Identity;
using Microsoft.AspNetCore.Identity;
using BackendApi.Core.Constants;
using BackendApi.Core.Models.Identity;

namespace BackendApi.Infrastructure.Identity;

/// <summary>
/// Implementation of the JWT token service for generating and validating JWT tokens.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
  private readonly JwtSettings _jwtSettings;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly TokenStoreService _tokenStoreService;

  /// <summary>
  /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
  /// </summary>
  /// <param name="configuration">The configuration service for accessing app settings.</param>
  /// <param name="userManager">The user manager for managing user accounts.</param>
  /// <param name="tokenStoreService">The token store service for managing authentication tokens.</param>
  public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, TokenStoreService tokenStoreService)
  {
    _userManager = userManager;
    _tokenStoreService = tokenStoreService;
    _jwtSettings = new JwtSettings
    {
      Key = configuration["Jwt:Key"] ?? "f02aecc8457e71afc1bdef98da64a1d0e9591c68945868afb60c2eb45ede7258",
      Issuer = configuration["Jwt:Issuer"] ?? "MyAppAuth",
      Audience = configuration["Jwt:Audience"] ?? "MyApp",
      ExpirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out int minutes) ? minutes : 30
    };
  }

  /// <inheritdoc/>
  public async Task<(Result Result, string Token, DateTime ExpiresAt, string RefreshToken)> GenerateTokenAsync(string userId, IEnumerable<Claim> claims)
  {
    try
    {
      await Task.CompletedTask; // Ensure method is async

      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
      var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

      var token = new JwtSecurityToken(
          issuer: _jwtSettings.Issuer,
          audience: _jwtSettings.Audience,
          claims: claims,
          expires: expires,
          signingCredentials: creds
      );

      var jwt = new JwtSecurityTokenHandler().WriteToken(token);
      var refreshToken = GenerateRefreshToken();

      // Find the user
      var identityUser = await _userManager.FindByIdAsync(userId);
      if (identityUser == null)
      {
        return (Result.Failure(new[] { "User not found in identity store" }), string.Empty, DateTime.UtcNow, string.Empty);
      }

      // Set the refresh token with an expiration date matching the JWT token's expiration
      await _tokenStoreService.SetRefreshTokenWithExpiryAsync(identityUser, refreshToken, expires);

      return (Result.Success(), jwt, expires, refreshToken);
    }
    catch (Exception ex)
    {
      return (Result.Failure(new[] { $"Token generation failed: {ex.Message}" }), string.Empty, DateTime.UtcNow, string.Empty);
    }
  }

  /// <inheritdoc/>
  public async Task<Result> ValidateTokenAsync(string token)
  {
    try
    {
      await Task.CompletedTask; // Ensure method is async

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

      tokenHandler.ValidateToken(token, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrEmpty(_jwtSettings.Issuer),
        ValidIssuer = _jwtSettings.Issuer,
        ValidateAudience = !string.IsNullOrEmpty(_jwtSettings.Audience),
        ValidAudience = _jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      }, out _);

      return Result.Success();
    }
    catch (Exception ex)
    {
      return Result.Failure(new[] { $"Token validation failed: {ex.Message}" });
    }
  }

  /// <summary>
  /// Generates a random refresh token.
  /// </summary>
  /// <returns>A base64-encoded refresh token.</returns>
  private static string GenerateRefreshToken()
  {
    return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
  }
}
