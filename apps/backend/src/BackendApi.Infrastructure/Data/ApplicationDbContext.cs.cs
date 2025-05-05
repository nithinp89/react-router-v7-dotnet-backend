using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Infrastructure.Identity;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Identity;

namespace BackendApi.Infrastructure.Data
{
  /// <summary>
  /// Entity Framework database context for the application.
  /// </summary>
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, ApplicationUserToken>(options), IApplicationDbContext
  {
    /// <summary>
    /// Gets or sets the permissions table.
    /// </summary>
    public required DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Gets or sets the user types table.
    /// </summary>
    public required DbSet<UserTypes> UserTypes { get; set; }

    /// <summary>
    /// Gets or sets the user sessions table.
    /// </summary>
    public required DbSet<UserSessions> UserSessions { get; set; }


    /// <summary>
    /// Configures the model and customizes Identity table names.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Set default schema for Identity tables
      builder.HasDefaultSchema("core");

      // Set the initial value for the users table ID sequence to 10001
      builder.HasSequence<int>("users_id_seq", "core")
          .StartsAt(10001)
          .IncrementsBy(1);

      // Set the initial value for the roles table ID sequence to 10001
      builder.HasSequence<int>("roles_id_seq", "core")
          .StartsAt(101)
          .IncrementsBy(1);

      // Set the initial value for the permissions table ID sequence to 10001
      builder.HasSequence<int>("permissions_id_seq", "core")
          .StartsAt(101)
          .IncrementsBy(1);

      // Set the initial value for the user types table ID sequence
      builder.HasSequence<int>("user_types_id_seq", "core")
          .StartsAt(101)
          .IncrementsBy(1);

      // No sequence needed for user sessions as it uses string IDs

      // Rename Identity tables to snake_case
      builder.Entity<ApplicationUser>(b => { 
        b.ToTable("users"); 
        b.Property(u => u.Id).HasDefaultValueSql("nextval('core.users_id_seq')");
        
        // Rename built-in IdentityUser columns to snake_case
        b.Property(u => u.UserName).HasColumnName("username");
        b.Property(u => u.NormalizedUserName).HasColumnName("normalized_username");
        b.Property(u => u.Email).HasColumnName("email");
        b.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
        b.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
        b.Property(u => u.PasswordHash).HasColumnName("password_hash");
        b.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
        b.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
        b.Property(u => u.PhoneNumber).HasColumnName("phone_number");
        b.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
        b.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
        b.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
        b.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
        b.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
        
        // Configure the Active property with default value
        b.Property(u => u.Active).HasColumnName("active").HasDefaultValue(true);
      });
      builder.Entity<ApplicationRole>(b => { 
        b.ToTable("roles"); 
        b.Property(r => r.Id).HasDefaultValueSql("nextval('core.roles_id_seq')");
        
        // Rename built-in IdentityRole columns to snake_case
        b.Property(r => r.Name).HasColumnName("name");
        b.Property(r => r.NormalizedName).HasColumnName("normalized_name");
        b.Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");
      });
      builder.Entity<IdentityUserRole<int>>(b => { 
        b.ToTable("user_roles"); 
        b.Property(ur => ur.UserId).HasColumnName("user_id");
        b.Property(ur => ur.RoleId).HasColumnName("role_id");
      });

      builder.Entity<IdentityUserClaim<int>>(b => { 
        b.ToTable("user_claims"); 
        b.Property(uc => uc.Id).HasColumnName("id");
        b.Property(uc => uc.UserId).HasColumnName("user_id");
        b.Property(uc => uc.ClaimType).HasColumnName("claim_type");
        b.Property(uc => uc.ClaimValue).HasColumnName("claim_value");
      });

      builder.Entity<IdentityUserLogin<int>>(b => { 
        b.ToTable("user_logins"); 
        b.Property(ul => ul.LoginProvider).HasColumnName("login_provider");
        b.Property(ul => ul.ProviderKey).HasColumnName("provider_key");
        b.Property(ul => ul.ProviderDisplayName).HasColumnName("provider_display_name");
        b.Property(ul => ul.UserId).HasColumnName("user_id");
      });

      builder.Entity<IdentityRoleClaim<int>>(b => { 
        b.ToTable("role_claims"); 
        b.Property(rc => rc.Id).HasColumnName("id");
        b.Property(rc => rc.RoleId).HasColumnName("role_id");
        b.Property(rc => rc.ClaimType).HasColumnName("claim_type");
        b.Property(rc => rc.ClaimValue).HasColumnName("claim_value");
      });

      builder.Entity<ApplicationUserToken>(b => { 
        b.ToTable("user_tokens"); 
        b.Property(ut => ut.UserId).HasColumnName("user_id");
        b.Property(ut => ut.LoginProvider).HasColumnName("login_provider");
        b.Property(ut => ut.Name).HasColumnName("name");
        b.Property(ut => ut.Value).HasColumnName("value");
        b.Property(ut => ut.CreatedAt).HasColumnName("created_at");
        b.Property(ut => ut.ExpiryAt).HasColumnName("expiry_at");
      });

      builder.Entity<Permission>(b =>
      {
        b.Property(p => p.Id).HasDefaultValueSql("nextval('core.permissions_id_seq')");
        b.HasIndex(p => p.NormalizedName).IsUnique();
      });
      
      builder.Entity<UserTypes>(b =>
      {
        b.Property(ut => ut.Id).HasDefaultValueSql("nextval('core.user_types_id_seq')");
      });
      
      builder.Entity<UserSessions>(b =>
      {
        b.Property(us => us.Id).HasMaxLength(50);
      });
      
      builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}
