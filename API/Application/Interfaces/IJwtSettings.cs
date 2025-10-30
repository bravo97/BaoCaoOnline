using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IJwtSettings
    {
        string Secret { get; }
        int ExpirationMinutes { get; }
        string Issuer { get; }
        string Audience { get; }
    }
}
