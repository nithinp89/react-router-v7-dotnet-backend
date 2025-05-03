using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Infrastructure.Identity;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace BackendApi.Infrastructure.Data
{
  /// <summary>
  /// Entity Framework database context for the application.
  /// </summary>
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
  {

    /// <summary>
    /// Configures the model and customizes Identity table names.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Set default schema for Identity tables
      builder.HasDefaultSchema("core");

      // Rename Identity tables to snake_case
      builder.Entity<ApplicationUser>(b => { b.ToTable("users"); });
      builder.Entity<IdentityRole>(b => { b.ToTable("roles"); });
      builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("user_roles"); });
      builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("user_claims"); });
      builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("user_logins"); });
      builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("role_claims"); });
      builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("user_tokens"); });

      builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}
