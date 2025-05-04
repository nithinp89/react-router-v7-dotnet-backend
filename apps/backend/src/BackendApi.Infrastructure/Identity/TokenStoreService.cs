using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackendApi.Core.Constants;
using System.Threading.Tasks;
using System;
using BackendApi.Infrastructure.Data;

namespace BackendApi.Infrastructure.Identity;

/// <summary>
/// Service for managing authentication tokens with extended functionality.
/// </summary>
public class TokenStoreService
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
    /// Sets an User token for the specified user with an expiration date.
    /// </summary>
    /// <param name="user">The user to set the token for.</param>
    /// <param name="loginProvider">The authentication provider for the token.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="value">The token value.</param>
    /// <param name="expiryDate">The expiration date for the token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetUserTokenWithExpiryAsync(ApplicationUser user, string loginProvider, string name, string value, DateTime expiryDate)
    {
        // First, set the token using the standard UserManager method
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider, name, value);

        // Then, update the token with the expiry date
        var token = await _dbContext.Set<ApplicationUserToken>()
            .FirstOrDefaultAsync(t => 
                t.UserId == user.Id && 
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
    /// <param name="user">The user to set the refresh token for.</param>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <param name="expiryDate">The expiration date for the token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetRefreshTokenWithExpiryAsync(ApplicationUser user, string refreshToken, DateTime expiryDate)
    {
        return SetUserTokenWithExpiryAsync(
            user, 
            ApplicationIdentityConstants.SESSION_LOGIN_PROVIDER, 
            ApplicationIdentityConstants.REFRESH_TOKEN, 
            refreshToken, 
            expiryDate);
    }
}
