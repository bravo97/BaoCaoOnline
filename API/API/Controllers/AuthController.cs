using API.DTO;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CustomerService _customerService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(UserService userService, CustomerService customerService, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _customerService = customerService;
            _refreshTokenRepository = refreshTokenRepository;
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

            var accessToken = _jwtTokenService.GenerateToken(user);
            var refreshTokenPlain = _jwtTokenService.GenerateRefreshTokenValue();
            var refreshHash = _jwtTokenService.ComputeRefreshTokenHash(refreshTokenPlain);

            var refreshToken = new RefreshToken
            {
                TokenHash = refreshHash,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshToken);

            return Ok(new { accessToken, refreshToken = refreshTokenPlain });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Missing token");

            var hash = _jwtTokenService.ComputeRefreshTokenHash(refreshToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash);
            if (existing == null || existing.Revoked || existing.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid refresh token");

            // rotate
            existing.Revoked = true;
            var newPlain = _jwtTokenService.GenerateRefreshTokenValue();
            var newHash = _jwtTokenService.ComputeRefreshTokenHash(newPlain);
            var newToken = new RefreshToken
            {
                TokenHash = newHash,
                UserId = existing.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                ReplacedByToken = null
            };

            existing.ReplacedByToken = newToken.Id;
            await _refreshTokenRepository.UpdateAsync(existing);
            await _refreshTokenRepository.AddAsync(newToken);

            // issue new access token for user
            // find user
            var user = await _userService.GetByIdAsync(existing.UserId);
            if (user == null) return Unauthorized("User not found");

            var accessToken = _jwtTokenService.GenerateToken(user);
            return Ok(new { accessToken, refreshToken = newPlain });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Missing token");
            var hash = _jwtTokenService.ComputeRefreshTokenHash(refreshToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash);
            if (existing != null)
            {
                existing.Revoked = true;
                await _refreshTokenRepository.UpdateAsync(existing);
            }
            return NoContent();
        }

        [HttpGet("verification")]
        public async Task<IActionResult> Verification(string customerId)
        {
            var customer = await _customerService.GetByIdAsync(customerId);
            if (customer == null) return Unauthorized("Invalid credentials");
            return Ok(new { customer.Id });
        }

        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginAccount([FromBody] LoginDto dto)
        {
            var user = await _userService.LoginAccountAsync(dto.CustomerId, dto.Username, dto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
