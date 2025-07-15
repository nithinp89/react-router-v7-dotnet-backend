using BackendApi.Core.Common;
using BackendApi.Core.Constants;
using BackendApi.Core.Interfaces;
using BackendApi.Core.Objects.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BackendApi.Infrastructure.Identity;
using BackendApi.Core.Interfaces.Services.Identity;

namespace BackendApi.Application.Services.Identity
{
    /// <inheritdoc/>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenStoreService _tokenStoreService;

        /// <inheritdoc/>
        public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ITokenStoreService tokenStoreService)
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
        public async Task<(Result Result, string Token, DateTime ExpiresAt, string RefreshToken)> GenerateTokensAsync(string userId, IEnumerable<Claim> claims)
        {
            try
            {
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
                await _tokenStoreService.SetRefreshTokenWithExpiryAsync(userId, refreshToken, expires);

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

        /// <inheritdoc/>
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}

