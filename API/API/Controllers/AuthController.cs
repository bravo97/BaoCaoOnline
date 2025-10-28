using API.DTO;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var success = await _userService.RegisterAsync(dto.Username, dto.Password, dto.Email);
            if (!success) return BadRequest("Username already exists");
            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.LoginAsync(dto.Username, dto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            return Ok(new
            {
                user.Username,
                user.Email
            });
        }
    }
}
