using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OptimizedSqlReportRepository : IReportRepository
    {
        private readonly ICustomerRepository _customerRepository;

        // Cache connection string theo CustomerId
        private static ConcurrentDictionary<string, string> _connectionCache = new();

        // Cache metadata report + columns
        private static ConcurrentDictionary<string, IEnumerable<Report>> _reportCache = new();
        private static ConcurrentDictionary<string, IEnumerable<ReportColumn>> _columnCache = new();

        public OptimizedSqlReportRepository(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        private async Task<string> GetConnectionStringAsync(Customer _customer)
        {
            if (_connectionCache.TryGetValue(_customer.Id, out var cachedConn))
                return cachedConn;

            if (_customer == null)
                throw new Exception($"Customer {_customer!.Id} not found");

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _customer.ServerName,
                InitialCatalog = _customer.DatabaseName,
                UserID = _customer.UserName,
                Password = _customer.Password, // có thể mã hóa trước khi lưu
                MultipleActiveResultSets = true,
                ConnectTimeout = 30
            };

            var connStr = builder.ConnectionString;
            _connectionCache[_customer.Id] = connStr;

            await Task.CompletedTask;
            return connStr;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string customerId)
        {
            if (_reportCache.TryGetValue(customerId, out var cachedReports))
                return cachedReports;
            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);

            var reports = new List<Report>{ };
            try
            {
                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand(_customer!.SqlReport, conn)
                {
                    CommandTimeout = 60
                };
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var item = new Report
                    {
                        Name = reader["Name"] as string ?? string.Empty,
                        FullName = reader["FullName"] as string ?? string.Empty,
                        Group = reader["Group"] as string ?? string.Empty,
                        SqlQuery = reader["SqlQuery"] as string ?? string.Empty
                    };

                    reports.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine($"Error getting report data: {ex.Message}");
                throw;
            }

            _reportCache[customerId] = reports;
            return reports;
        }

        public async Task<IEnumerable<ReportColumn>> GetReportColumnsAsync(string customerId, string reportId)
        {
            string cacheKey = $"{customerId}_{reportId}";
            if (_columnCache.TryGetValue(cacheKey, out var cachedColumns))
                return cachedColumns;

            var _reports = await GetReportsAsync(customerId);
            var _report = _reports.FirstOrDefault(r => r.Id == reportId);
            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);
            var reportColumns = new List<ReportColumn> { };
            try
            {
                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand("select doituong as 'ColumnName',hienthi as 'DisplayName', kieudl as 'DataType' from app_ctchucnang where chucnang ='" + _report!.Name + "'", conn)
                {
                    CommandTimeout = 60
                };
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var item = new ReportColumn
                    {
                        ReportId = reportId,
                        ColumnName = reader["ColumnName"] as string ?? string.Empty,
                        DisplayName = reader["DisplayName"] as string ?? string.Empty,
                        DataType = reader["DataType"] as string ?? string.Empty
                    };

                    reportColumns.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine($"Error getting report data: {ex.Message}");
                throw;
            }

            

            _columnCache[cacheKey] = reportColumns;
            return reportColumns;
        }

        public async Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsync(string customerId, string reportId,ReportParameters parameters)
        {
            var reports = await GetReportsAsync(customerId);
            var report = reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null) return Enumerable.Empty<Dictionary<string, object>>();

            //Xử lý query với tham số nếu cần
            var (SqlQueryWithParams, sqlParams) = SqlParameterBinder.BuildSqlWithParams(report.SqlQuery, parameters);

            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);
            var result = new List<Dictionary<string, object>>();

            try
            {
                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand(SqlQueryWithParams, conn)
                {
                    CommandTimeout = 60
                };

                foreach (var p in sqlParams)
                    cmd.Parameters.Add(p);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[reader.GetName(i)] = value!;
                    }
                    result.Add(row);
                }
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine($"Error getting report data: {ex.Message}");
                throw;
            }

            return result;
        }

        // Optional: xóa cache khi cần refresh
        public void ClearCache(string customerId)
        {
            _connectionCache.TryRemove(customerId, out _);
            _reportCache.TryRemove(customerId, out _);
            foreach (var key in _columnCache.Keys.Where(k => k.StartsWith(customerId + "_")).ToList())
            {
                _columnCache.TryRemove(key, out _);
            }
        }
    }
}
