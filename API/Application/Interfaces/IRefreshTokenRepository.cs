using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);
        Task UpdateAsync(RefreshToken token);
        Task DeleteAsync(string id);
    }
}
