using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class JwtSettings : IJwtSettings
    {
        public string Secret { get; set; } = null!;
        public int ExpirationMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}
