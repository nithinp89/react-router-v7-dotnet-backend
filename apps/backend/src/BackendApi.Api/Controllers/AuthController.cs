using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace BackendApi.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        // Login model to accept JSON input
        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserModel
        {
            public string Email { get; set; }
            public string Jwt { get; set; }
            public string RefreshToken { get; set; }
            public long RefreshTokenExpiry { get; set; } // Assuming Unix timestamp (Epoch time)
            public string Id { get; set; }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string email = model.Email;
            string password = model.Password;
            //System.Threading.Thread.Sleep(2000); // Simulate a delay for demonstration purposes
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest(new { message = "Email and password are required" });
            // Replace with actual user validation
            if (email != "admin" || password != "password")
                return Unauthorized(new { message = "Invalid Email or password" });

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, "Admin"),
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in with cookie
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Also issue JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("f02aecc8457e71afc1bdef98da64a1d0e9591c68945868afb60c2eb45ede7258"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
            issuer: "MyAppAuth",
            audience: "MyApp",
            claims: claims,
            expires: expires,
            signingCredentials: creds);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    //issuer= "MyAppBackend",
            //    //audience= "MyApp",
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddHours(1),
            //    SigningCredentials = creds,
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenExpiry = EpochTime.GetIntDate(expires.AddMinutes(-5).ToUniversalTime()); // Calculate refreshBy as 15 minutes before expires

            return Ok(
                new
                {
                    jwt,
                    email,
                    refresh_token = refreshToken,
                    refresh_token_expiry = refreshTokenExpiry,
                    id = "9b452c2f-261a-40cb-963d-00dfd67c524d"
                }
                );
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

        [HttpPost("renew-session")]
        public async Task<IActionResult> RenewSession([FromBody] UserModel user)
        {
            //System.Threading.Thread.Sleep(2000); // Simulate a delay for demonstration purposes
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Jwt))
                return BadRequest(new { message = "Email and Jwt are required" });
            // Replace with actual user validation
            if (user.Email != "admin")
                return Unauthorized(new { message = "Invalid Email" });

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, "Admin"),
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in with cookie
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Also issue JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("f02aecc8457e71afc1bdef98da64a1d0e9591c68945868afb60c2eb45ede7258"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
            issuer: "MyAppAuth",
            audience: "MyApp",
            claims: claims,
            expires: expires,
            signingCredentials: creds);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    //issuer= "MyAppBackend",
            //    //audience= "MyApp",
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddHours(1),
            //    SigningCredentials = creds,
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenExpiry = EpochTime.GetIntDate(expires.AddMinutes(-5).ToUniversalTime()); // Calculate refreshBy as 15 minutes before expires

            return Ok(
                new
                {
                    jwt,
                    user.Email,
                    refresh_token = refreshToken,
                    refresh_token_expiry = refreshTokenExpiry,
                    id = "9b452c2f-261a-40cb-963d-00dfd67c524d"
                }
                );
        }
    }
}
