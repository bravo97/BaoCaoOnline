using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDeviceLogRepository
    {
        Task LogDeviceAsync(string deviceType, string ipAddress, string endpoint);
        Task<Dictionary<string, int>> GetDeviceStatsAsync(int days);
        Task<int> GetActiveUserCountAsync(int days);
    }
}
