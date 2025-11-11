using API.DTO;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserService userService,AccountService accountService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
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

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginAccount([FromBody] LoginDto dto)
        {
            var user = await _userService.LoginAccountAsync(dto.Username, dto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
