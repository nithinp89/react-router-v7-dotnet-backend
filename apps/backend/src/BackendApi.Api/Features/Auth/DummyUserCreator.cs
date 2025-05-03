using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using BackendApi.Infrastructure.Identity;

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

        public DummyUserCreator(UserManager<ApplicationUser> userManager, ILogger<DummyUserCreator> logger, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a dummy user account if it does not already exist.
        /// </summary>
        /// <param name="email">The email for the dummy user.</param>
        /// <param name="password">The password for the dummy user.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task CreateDummyUserAsync(string email = "admin@example.com", string password = "Password123#@")
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await _userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
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

            // Seed standard permissions and Agent role
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BackendApi.Infrastructure.Data.ApplicationDbContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    // Seed permissions
                    var permissions = new[]
                    {
                        new Permission { Name = "tickets:read", NormalizedName = "TICKETS:READ" },
                        new Permission { Name = "tickets:write", NormalizedName = "TICKETS:WRITE" },
                        new Permission { Name = "tickets:delete", NormalizedName = "TICKETS:DELETE" }
                    };
                    foreach (var perm in permissions)
                    {
                        if (!dbContext.Permissions.Any(p => p.NormalizedName == perm.NormalizedName))
                        {
                            dbContext.Permissions.Add(perm);
                        }
                    }
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Standard permissions seeded.");

                    // Seed Agent role
                    var agentRoleName = "Agent";
                    if (!await roleManager.RoleExistsAsync(agentRoleName))
                    {
                        var role = new IdentityRole { Name = agentRoleName, NormalizedName = agentRoleName.ToUpper() };
                        var result = await roleManager.CreateAsync(role);
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
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var agentUser = await userManager.FindByEmailAsync(email);
                    if (agentUser != null && !await userManager.IsInRoleAsync(agentUser, agentRoleName))
                    {
                        var addToRoleResult = await userManager.AddToRoleAsync(agentUser, agentRoleName);
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
                    var agentRole = await roleManager.FindByNameAsync(agentRoleName);
                    if (agentRole != null)
                    {
                        var agentPermissions = dbContext.Permissions.Where(p => p.NormalizedName.StartsWith("TICKETS:")).ToList();
                        var existingClaims = await roleManager.GetClaimsAsync(agentRole);
                        foreach (var perm in agentPermissions)
                        {
                            if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == perm.NormalizedName))
                            {
                                var claimResult = await roleManager.AddClaimAsync(agentRole, new System.Security.Claims.Claim("Permission", perm.NormalizedName));
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed standard permissions or Agent role.");
            }
        }
    }
}
