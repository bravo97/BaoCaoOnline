using Application.Models;
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
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            try
            {
                var customerId = User.Identity?.Name;
                var reports = await _reportService.GetReportsAsync(customerId!);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/customers/{customerId}/reports/{reportId}
        [HttpGet("{reportId}")]
        public async Task<ActionResult<object>> GetReportColums(string reportId)
        {
            try
            {
                var customerId = User.Identity?.Name;
                var columns = await _reportService.GetReportColumnsAsync(customerId!, reportId);

                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("data/{reportId}")]
        public async Task<ActionResult<object>> GetReportData(string reportId, [FromQuery] ReportParameters parameters)
        {
            try
            {
                var customerId = User.Identity?.Name;
                var data = await _reportService.GetReportDataAsync(customerId!, reportId, parameters);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
