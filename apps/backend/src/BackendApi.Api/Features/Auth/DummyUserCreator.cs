using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using BackendApi.Infrastructure.Identity;
using BackendApi.Core.Models.Identity;
using BackendApi.Core.Common;
using BackendApi.Core.Constants;
using System.Linq;
using BackendApi.Infrastructure.Data;

namespace BackendApi.Api.Features.Auth
{
  /// <summary>
  /// Provides a method to create a dummy user account using ASP.NET Core Identity.
  /// </summary>
  public class DummyUserCreator
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DummyUserCreator> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<ApplicationRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyUserCreator"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for managing user accounts.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="serviceProvider">The service provider for resolving services.</param>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="roleManager">The role manager for managing roles.</param>
    public DummyUserCreator(
        UserManager<ApplicationUser> userManager,
        ILogger<DummyUserCreator> logger,
        IServiceProvider serviceProvider,
        ApplicationDbContext dbContext,
        RoleManager<ApplicationRole> roleManager)
    {
      _userManager = userManager;
      _logger = logger;
      _serviceProvider = serviceProvider;
      _dbContext = dbContext;
      _roleManager = roleManager;
    }

    /// <summary>
    /// Creates a dummy user account if it does not already exist.
    /// </summary>
    /// <param name="email">The email for the dummy user.</param>
    /// <param name="password">The password for the dummy user.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task CreateDummyUserAsync(string email = "admin@example.com", string password = "Password123#@")
    {
      // Seed user types
      var userTypes = new[]
      {
            new UserTypes { Name = "Administrator", Description = "System administrator with full access", Protected = true, CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID },
            new UserTypes { Name = "Technical", Description = "Technical user with full access", Protected = true, CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID },
            new UserTypes { Name = "Agent", Description = "Support agent with access to tickets", Protected = true, CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID },
            new UserTypes { Name = "Collaborator", Description = "Support collaborator with access to their tickets and can provide internal notes", Protected = true, CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID },
            new UserTypes { Name = "Customer", Description = "Regular customer with limited access", Protected = true, CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID },
        };

      foreach (var userType in userTypes)
      {
        if (!_dbContext.UserTypes.Any(ut => ut.Name == userType.Name))
        {
          _dbContext.UserTypes.Add(userType);
        }
      }
      await _dbContext.SaveChangesAsync();
      _logger.LogInformation("User types seeded.");

      // Get admin user type for the admin user
      var adminUserType = _dbContext.UserTypes.FirstOrDefault(ut => ut.Name == "Administrator");

      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        var newUser = new ApplicationUser
        {
          UserName = email,
          Email = email,
          EmailConfirmed = true,
          FirstName = "Admin",
          LastName = "User",
          UserTypeId = adminUserType.Id,
          CreatedBy = ApplicationIdentityConstants.DEFAULT_USER_ID, // Temporary value
          ModifiedBy = ApplicationIdentityConstants.DEFAULT_USER_ID,  // Temporary value
          Protected = GenericContants.PROTECTED_YES,
        };
        var result = await _userManager.CreateAsync(newUser, password);
        if (result.Succeeded)
        {
          // Update the user with its own ID for audit fields
          newUser.CreatedBy = newUser.Id;
          newUser.ModifiedBy = newUser.Id;
          await _userManager.UpdateAsync(newUser);
          _logger.LogInformation("Dummy user created: {Email}", email);
        }
        else
        {
          _logger.LogWarning("Failed to create dummy user: {Errors}", string.Join(", ", result.Errors));
        }
      }
      else
      {
        _logger.LogInformation("Dummy user already exists: {Email}", email);
      }

      user = await _userManager.FindByEmailAsync(email);

      // Seed standard permissions and Agent role
      try
      {
        // Seed permissions
        var permissions = new[]
        {
                        new Permission { Name = "tickets:read", NormalizedName = "TICKETS:READ", Description = "Read tickets", Protected = true, CreatedBy = user.Id, ModifiedBy = user.Id },
                        new Permission { Name = "tickets:write", NormalizedName = "TICKETS:WRITE", Description = "Write tickets", Protected = true, CreatedBy = user.Id, ModifiedBy = user.Id },
                        new Permission { Name = "tickets:delete", NormalizedName = "TICKETS:DELETE", Description = "Delete tickets", Protected = true, CreatedBy = user.Id, ModifiedBy = user.Id },
                    };
        foreach (var perm in permissions)
        {
          if (!_dbContext.Permissions.Any(p => p.NormalizedName == perm.NormalizedName))
          {
            _dbContext.Permissions.Add(perm);
          }
        }
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Standard permissions seeded.");

        // Seed Agent role
        var agentRoleName = "Agent";
        if (!await _roleManager.RoleExistsAsync(agentRoleName))
        {
          var role = new ApplicationRole
          {
            Name = agentRoleName,
            NormalizedName = agentRoleName.ToUpper(),
            Description = "Agent role with access to tickets",
            CreatedBy = user.Id,
            ModifiedBy = user.Id
          };
          var result = await _roleManager.CreateAsync(role);
          if (result.Succeeded)
          {
            _logger.LogInformation("Role 'Agent' created.");
          }
          else
          {
            _logger.LogWarning("Failed to create 'Agent' role: {Errors}", string.Join(", ", result.Errors));
          }
        }
        else
        {
          _logger.LogInformation("Role 'Agent' already exists.");
        }

        // Assign dummy user to Agent role
        var agentUser = await _userManager.FindByEmailAsync(email);
        if (agentUser != null && !await _userManager.IsInRoleAsync(agentUser, agentRoleName))
        {
          var addToRoleResult = await _userManager.AddToRoleAsync(agentUser, agentRoleName);
          if (addToRoleResult.Succeeded)
          {
            _logger.LogInformation("Dummy user assigned to 'Agent' role.");
          }
          else
          {
            _logger.LogWarning("Failed to assign dummy user to 'Agent' role: {Errors}", string.Join(", ", addToRoleResult.Errors));
          }
        }

        // Assign permissions to Agent role as claims
        var agentRole = await _roleManager.FindByNameAsync(agentRoleName);
        if (agentRole != null)
        {
          var agentPermissions = _dbContext.Permissions.Where(p => p.NormalizedName.StartsWith("TICKETS:")).ToList();
          var existingClaims = await _roleManager.GetClaimsAsync(agentRole);
          foreach (var perm in agentPermissions)
          {
            if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == perm.NormalizedName))
            {
              var claimResult = await _roleManager.AddClaimAsync(agentRole, new System.Security.Claims.Claim("Permission", perm.NormalizedName));
              if (claimResult.Succeeded)
              {
                _logger.LogInformation($"Permission '{perm.NormalizedName}' assigned to 'Agent' role as claim.");
              }
              else
              {
                _logger.LogWarning($"Failed to assign permission '{perm.NormalizedName}' to 'Agent' role: {string.Join(", ", claimResult.Errors)}");
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to seed standard permissions or Agent role.");
      }
    }
  }
}
