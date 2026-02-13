using Application.Interfaces;
using Domain.Entities;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;

namespace Infrastructure.Repositories
{
    public class FileDeviceLogRepository : IDeviceLogRepository, IDisposable
    {
        private readonly string _logDirectory;
        private readonly ConcurrentQueue<DeviceLog> _logQueue = new();
        private readonly Timer _timer;
        private readonly object _lock = new();

        public FileDeviceLogRepository()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            // Flush every 30 seconds
            _timer = new Timer(FlushLogs, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public Task LogDeviceAsync(string deviceType, string ipAddress, string endpoint)
        {
            var log = new DeviceLog
            {
                Timestamp = DateTime.UtcNow,
                DeviceType = deviceType,
                IpAddress = ipAddress,
                Endpoint = endpoint
            };

            _logQueue.Enqueue(log);
            return Task.CompletedTask;
        }

        private void FlushLogs(object? state)
        {
            if (_logQueue.IsEmpty) return;

            var logsToWrite = new List<DeviceLog>();
            while (_logQueue.TryDequeue(out var log))
            {
                logsToWrite.Add(log);
            }

            if (logsToWrite.Count == 0) return;

            // Group by date to write to correct files (in case flush crosses midnight)
            var groups = logsToWrite.GroupBy(l => l.Timestamp.ToString("yyyyMMdd"));

            foreach (var group in groups)
            {
                var filePath = Path.Combine(_logDirectory, $"access_log_{group.Key}.json");
                var sb = new StringBuilder();

                foreach (var log in group)
                {
                    // NDJSON format
                    sb.AppendLine(JsonSerializer.Serialize(log));
                }

                lock (_lock)
                {
                    File.AppendAllText(filePath, sb.ToString());
                }
            }
        }

        public async Task<Dictionary<string, int>> GetDeviceStatsAsync(int days)
        {
            var stats = new Dictionary<string, int>
            {
                { "Desktop", 0 },
                { "Mobile", 0 }
            };

            var today = DateTime.UtcNow;
            for (int i = 0; i < days; i++)
            {
                var date = today.AddDays(-i);
                var filePath = Path.Combine(_logDirectory, $"access_log_{date:yyyyMMdd}.json");

                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            var log = JsonSerializer.Deserialize<DeviceLog>(line);
                            if (log != null && stats.ContainsKey(log.DeviceType))
                            {
                                stats[log.DeviceType]++;
                            }
                        }
                        catch { /* Ignore corrupted lines */ }
                    }
                }
            }

            return stats;
        }

        public async Task<int> GetActiveUserCountAsync(int days)
        {
            var uniqueIps = new HashSet<string>();
            var today = DateTime.UtcNow;

            for (int i = 0; i < days; i++)
            {
                var date = today.AddDays(-i);
                var filePath = Path.Combine(_logDirectory, $"access_log_{date:yyyyMMdd}.json");

                if (File.Exists(filePath))
                {
                    try
                    {
                        var lines = await File.ReadAllLinesAsync(filePath);
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                var log = JsonSerializer.Deserialize<DeviceLog>(line);
                                if (log != null && !string.IsNullOrEmpty(log.IpAddress))
                                {
                                    uniqueIps.Add(log.IpAddress);
                                }
                            }
                            catch { /* Ignore corrupted lines */ }
                        }
                    }
                    catch { /* Ignore file read errors */ }
                }
            }

            return uniqueIps.Count;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            FlushLogs(null); // Valid attempt to flush remaining logs
        }
    }
}
