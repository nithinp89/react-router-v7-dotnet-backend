using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BackendApi.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        // Login model to accept JSON input
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string username = model.Username;
            string password = model.Password;
            //System.Threading.Thread.Sleep(2000); // Simulate a delay for demonstration purposes
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest(new { message = "Username and password are required" });
            // Replace with actual user validation
            if (username != "admin" || password != "password")
                return Unauthorized(new { message = "Invalid username or password" });

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin"),
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in with cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Also issue JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("f02aecc8457e71afc1bdef98da64a1d0e9591c68945868afb60c2eb45ede7258");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt, message = "Login successful" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized(new { message = "Not authenticated" });

            return Ok(new
            {
                Username = User.Identity.Name,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
            });
        }
    }
}
