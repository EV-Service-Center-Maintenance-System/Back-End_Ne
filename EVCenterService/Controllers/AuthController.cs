using EVCenterService.Dtos.Request;
using EVCenterService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVCenterService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request.Email == "admin@gmail.com" && request.Password == "123456")
            {
                var token = _jwtService.GenerateToken(Guid.NewGuid(), "Admin");
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }


        [Authorize] 
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {   
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(new { message = $"Welcome user {userId}" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminData()
        {
            return Ok("Only Admin can see this!");
        }

    }
}
