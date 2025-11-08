using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/customers/{customerId}/reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports(string customerId)
        {
            var reports = await _reportService.GetReportsAsync(customerId);
            return Ok(reports);
        }

        // GET: api/customers/{customerId}/reports/{reportId}
        [HttpGet("{reportId}")]
        public async Task<ActionResult<object>> GetReportData(string customerId, string reportId)
        {
            //var customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            var data = await _reportService.GetReportDataAsync(customerId, reportId);
            var columns = await _reportService.GetReportColumnsAsync(customerId, reportId);

            return Ok(new
            {
                Columns = columns,
                Data = data
            });
        }
    }
}
