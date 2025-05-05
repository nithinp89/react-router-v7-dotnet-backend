using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackendApi.Core.Constants;
using System.Threading.Tasks;
using System;
using BackendApi.Infrastructure.Data;
using BackendApi.Core.Models.Identity;
using BackendApi.Infrastructure.Identity;
using BackendApi.Core.Interfaces.Services.Identity;

namespace BackendApi.Application.Services.Identity;

/// <summary>
/// Implementation of the token store service that manages user authentication tokens with expiration dates.
/// </summary>
public class TokenStoreService : ITokenStoreService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenStoreService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for managing user accounts.</param>
    /// <param name="dbContext">The application database context.</param>
    public TokenStoreService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Sets a user token with an expiration date.
    /// </summary>
    /// <param name="userId">The ID of the user to set the token for.</param>
    /// <param name="loginProvider">The authentication provider for the token.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="value">The token value.</param>
    /// <param name="expiryDate">The expiration date for the token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetUserTokenWithExpiryAsync(string userId, string loginProvider, string name, string value, DateTime expiryDate)
    {
        // Find the user by ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found", nameof(userId));
        }

        // First, set the token using the standard UserManager method
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider, name, value);

        // Then, update the token with the expiry date
        var token = await _dbContext.Set<ApplicationUserToken>()
            .FirstOrDefaultAsync(t => 
                t.UserId == int.Parse(userId) && 
                t.LoginProvider == loginProvider && 
                t.Name == name);

        if (token != null)
        {
            token.ExpiryAt = expiryDate;
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Sets a refresh token for the specified user with an expiration date.
    /// </summary>
    /// <param name="userId">The ID of the user to set the refresh token for.</param>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <param name="expiryDate">The expiration date for the token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetRefreshTokenWithExpiryAsync(string userId, string refreshToken, DateTime expiryDate)
    {
        return SetUserTokenWithExpiryAsync(
            userId, 
            ApplicationIdentityConstants.SESSION_LOGIN_PROVIDER, 
            ApplicationIdentityConstants.REFRESH_TOKEN, 
            refreshToken, 
            expiryDate);
    }
}
