using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReportRepository
    {
        // Lấy danh sách report có sẵn từ customer
        Task<IEnumerable<Report>> GetReportsAsync(string customerId, CancellationToken cancellationToken = default);

        // Lấy dữ liệu report
        Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsync(string customerId, string reportId,ReportParameters parameters, CancellationToken cancellationToken = default);

        // Lấy danh sách cột của report
        Task<IEnumerable<ReportColumn>> GetReportColumnsAsync(string customerId, string reportId, CancellationToken cancellationToken = default);
        void ClearCache(string customerId);
    }
}
