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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Repositories
{
    public class OptimizedSqlReportRepository : IReportRepository
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDatabaseConnectionFactory _connectionFactory;

        // Cache connection string theo CustomerId
        private static ConcurrentDictionary<string, string> _connectionCache = new();

        // Cache metadata report + columns + param
        private static ConcurrentDictionary<string, IEnumerable<Report>> _reportCache = new();
        private static ConcurrentDictionary<string, IEnumerable<ReportColumn>> _columnCache = new();
        private static ConcurrentDictionary<string, IEnumerable<ReportParameter>> _paramCache = new();

        public OptimizedSqlReportRepository(ICustomerRepository customerRepository, IDatabaseConnectionFactory connectionFactory)
        {
            _customerRepository = customerRepository;
            _connectionFactory = connectionFactory;
        }

        private async Task<string> GetConnectionStringAsync(Customer _customer)
        {
            if (_connectionCache.TryGetValue(_customer.Id, out var cachedConn))
                return cachedConn;

            if (_customer == null)
                throw new Exception($"Customer {_customer!.Id} not found");

            var connStr = _connectionFactory.CreateConnectionString(_customer);
            _connectionCache[_customer.Id] = connStr;

            await Task.CompletedTask;
            return connStr;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string customerId)
        {
            return await GetReportsAsync(customerId, CancellationToken.None);
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string customerId, CancellationToken cancellationToken)
        {
            if (_reportCache.TryGetValue(customerId, out var cachedReports))
                return cachedReports;
            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);

            var reports = new List<Report>{ };
            try
            {
                await RetryHelper.RetryAsync(async () =>
                {
                    using var conn = new SqlConnection(connStr);
                    using var cmd = new SqlCommand(_customer!.SqlReport, conn)
                    {
                        CommandTimeout = 60
                    };
                    await conn.OpenAsync(cancellationToken);
                    using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    while (await reader.ReadAsync(cancellationToken))
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
                }, shouldRetry: ex => ex is SqlException);
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
            return await GetReportColumnsAsync(customerId, reportId, CancellationToken.None);
        }

        public async Task<IEnumerable<ReportColumn>> GetReportColumnsAsync(string customerId, string reportId, CancellationToken cancellationToken)
        {
            string cacheKey = $"{customerId}_{reportId}";
            if (_columnCache.TryGetValue(cacheKey, out var cachedColumns))
                return cachedColumns;

            var _reports = await GetReportsAsync(customerId, cancellationToken);
            var _report = _reports.FirstOrDefault(r => r.Id == reportId);
            if (_report == null) return Enumerable.Empty<ReportColumn>();
            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);
            var reportColumns = new List<ReportColumn> { };
            try
            {
                await RetryHelper.RetryAsync(async () =>
                {
                    string sqlColumnQuery = _customer!.SqlColumnQuery;
                    using var conn = new SqlConnection(connStr);
                    using var cmd = new SqlCommand(sqlColumnQuery, conn)
                    {
                        CommandTimeout = 60
                    };

                    cmd.Parameters.AddWithValue("@Name", _report!.Name);

                    await conn.OpenAsync(cancellationToken);
                    using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var item = new ReportColumn
                        {
                            ReportId = reportId,

                            ColumnName = reader["ColumnName"] as string ?? string.Empty,

                            DisplayName = reader["DisplayName"] as string ?? string.Empty,

                            ColumnWidth = reader["ColumnWidth"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["ColumnWidth"]),

                            DataType = reader["DataType"] as string ?? string.Empty
                        };

                        reportColumns.Add(item);
                    }
                }, shouldRetry: ex => ex is SqlException, cancellationToken: cancellationToken);
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

        public async Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsync(string customerId, string reportId, Dictionary<string, object> parameters)
        {
            return await GetReportDataAsync(customerId, reportId, parameters, CancellationToken.None);
        }

        public async Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsync(string customerId, string reportId, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            var reports = await GetReportsAsync(customerId, cancellationToken);
            var report = reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null) return Enumerable.Empty<Dictionary<string, object>>();
            if(report.SqlQuery == null || report.SqlQuery == "") return Enumerable.Empty<Dictionary<string, object>>();
            //Xử lý query với tham số nếu cần
            var (SqlQueryWithParams, sqlParams) = SqlParameterBinder.BuildSqlWithParams(report.SqlQuery, parameters);

            var _customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(_customer!);
            var result = new List<Dictionary<string, object>>();

            try
            {
                await RetryHelper.RetryAsync(async () =>
                {
                    using var conn = new SqlConnection(connStr);
                    using var cmd = new SqlCommand(SqlQueryWithParams, conn)
                    {
                        CommandTimeout = 60
                    };

                    foreach (var p in sqlParams)
                        cmd.Parameters.Add(p);

                    await conn.OpenAsync(cancellationToken);
                    using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[reader.GetName(i)] = value!;
                        }
                        result.Add(row);
                    }
                }, shouldRetry: ex => ex is SqlException, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine($"Error getting report data: {ex.Message}");
                throw;
            }

            return result;
        }

        public async Task<IEnumerable<ReportParameter>> GetParamDataAsync(string customerId, string reportId)
        {
            return await GetParamDataAsync(customerId, reportId, CancellationToken.None);
        }

        public async Task<IEnumerable<ReportParameter>> GetParamDataAsync(string customerId, string reportId, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{customerId}_{reportId}";
            if (_paramCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var reports = await GetReportsAsync(customerId, cancellationToken);
            var report = reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null) return Enumerable.Empty<ReportParameter>();

            var customer = await _customerRepository.GetByIdAsync(customerId);
            var connStr = await GetConnectionStringAsync(customer!);

            var result = new List<ReportParameter>();

            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(cancellationToken);

            using var cmd = new SqlCommand(customer!.SqlParameter, conn)
            {
                CommandTimeout = 60
            };
            cmd.Parameters.AddWithValue("@Name", report.Name);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var param = new ReportParameter
                {
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Param = reader["Param"]?.ToString() ?? string.Empty,
                    ParamName = reader["ParamName"]?.ToString() ?? string.Empty,
                    Query = new ReportParameterQuery
                    {
                        SqlQuery = reader["SqlQuery"]?.ToString() ?? string.Empty,
                        ColumnValue = reader["ColumnValue"]?.ToString() ?? string.Empty,
                        ColumnDisplay = reader["ColumnDisplay"]?.ToString() ?? string.Empty
                    }
                };

                // 👉 Chỉ chạy SQL nếu có SqlQuery
                if (!string.IsNullOrWhiteSpace(param.Query.SqlQuery))
                {
                    param.DataParameter = await LoadParameterDataAsync(
                        conn,
                        param.Query.SqlQuery,
                        cancellationToken
                    );
                }

                result.Add(param);
            }

            _paramCache[cacheKey] = result;
            return result;
        }

        private async Task<List<Dictionary<string, object>>> LoadParameterDataAsync(SqlConnection conn, string sql, CancellationToken cancellationToken)
        {
            sql = sql.Replace("{manv1}", "");
            using var cmd = new SqlCommand(sql, conn)
            {
                CommandTimeout = 60
            };

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var list = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] =
                        reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                list.Add(row);
            }

            return list;
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
