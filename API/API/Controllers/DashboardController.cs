using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDeviceLogRepository _deviceLogRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;

        public DashboardController(
            IDeviceLogRepository deviceLogRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IReportRepository reportRepository)
        {
            _deviceLogRepository = deviceLogRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _reportRepository = reportRepository;
        }

        [HttpGet("device-stats")]
        public async Task<IActionResult> GetDeviceStats([FromQuery] int days = 7)
        {
            var stats = await _deviceLogRepository.GetDeviceStatsAsync(days);
            
            return Ok(new
            {
                labels = stats.Keys.ToList(),
                data = stats.Values.ToList()
            });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryStats()
        {
            var totalCustomers = (await _customerRepository.GetAllAsync()).Count();
            var totalAccounts = (await _userRepository.GetAllAsync()).Count();
            var activeUsers = await _deviceLogRepository.GetActiveUserCountAsync(30);

            return Ok(new
            {
                totalCustomers,
                totalAccounts,
                onlineUsers = activeUsers,
                totalReports = await GetTotalReportsAsync()
            });
        }

        private async Task<int> GetTotalReportsAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var count = 0;
                // Use default cancellation token
                var tasks = customers.Select(async c => 
                {
                    try
                    {
                        var reports = await _reportRepository.GetReportsAsync(c.Id);
                        return reports.Count();
                    }
                    catch
                    {
                        return 0;
                    }
                });

                var results = await Task.WhenAll(tasks);
                return results.Sum();
            }
            catch
            {
                return 0;
            }
        }
    }
}
