using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.Contains("/refresh") || path.Contains("/refresh-token")))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                try
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
                    var handler = new JwtSecurityTokenHandler();
                    handler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = _config["JwtSettings:Issuer"],
                        ValidAudience = _config["JwtSettings:Audience"],
                        IssuerSigningKey = key,
                        ValidateLifetime = true
                    }, out _);
                }
                catch
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token không hợp lệ hoặc hết hạn");
                    return;
                }
            }

            await _next(context);
        }
    }
}
