using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateToken(Account account);
        string GenerateRefreshTokenValue(int size = 64);
        string ComputeRefreshTokenHash(string refreshToken);
    }
}
