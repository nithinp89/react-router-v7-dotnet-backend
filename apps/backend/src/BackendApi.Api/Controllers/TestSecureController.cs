using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Api.Controllers
{
    [ApiController]
    [Route("secure")]
    public class SecureController : ControllerBase
    {
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Hello, Admin!");
        }

        [HttpGet("jwt-only")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult JwtAccess()
        {
            return Ok("This endpoint requires a JWT.");
        }
    }
}
