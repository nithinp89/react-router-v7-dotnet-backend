using Microsoft.AspNetCore.Identity;

namespace BackendApi.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}