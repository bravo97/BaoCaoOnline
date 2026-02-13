using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAPI.Middlewares
{
    public class DeviceLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDeviceLogRepository _repository;

        public DeviceLogMiddleware(RequestDelegate next, IDeviceLogRepository repository)
        {
            _next = next;
            _repository = repository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only log API requests, ignore OPTIONS and static files
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && path.StartsWith("/api") && context.Request.Method != HttpMethods.Options)
            {
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                var deviceType = IsMobile(userAgent) ? "Mobile" : "Desktop";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                // Fire and forget logging
                _ = _repository.LogDeviceAsync(deviceType, ip, path);
            }

            await _next(context);
        }

        private bool IsMobile(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return false;
            return Regex.IsMatch(userAgent, "Mobi|Android|iPhone|iPad|iPod", RegexOptions.IgnoreCase);
        }
    }
}
