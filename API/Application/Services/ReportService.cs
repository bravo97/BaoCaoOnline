using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string customerId)
        {
            return await _reportRepository.GetReportsAsync(customerId);
        }

        public async Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsync(string customerId, string reportId, ReportParameters parameters)
        {
            return await _reportRepository.GetReportDataAsync(customerId, reportId, parameters);
        }

        public async Task<IEnumerable<ReportColumn>> GetReportColumnsAsync(string customerId, string reportId)
        {
            return await _reportRepository.GetReportColumnsAsync(customerId, reportId);
        }

        public void ClearCache(string customerId)
        {
            _reportRepository.ClearCache(customerId);
        }
    }
}
