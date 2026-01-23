using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Infrastructure.Repositories
{
    public class FileNotificationReponsitory : INotificationReponsitory
    {
        private readonly string _filePath;
        private List<Notification> notifications = new();
        private readonly object _lock = new();
        private readonly ILogger<FileNotificationReponsitory> _logger;

        public FileNotificationReponsitory(ILogger<FileNotificationReponsitory> logger)
        {
            _logger = logger;
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "notification.json");

            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            LoadData();
        }

        private void LoadData()
        {
            lock (_lock)
            {
                try
                {
                    _logger.LogInformation("Loading notifications from {FilePath}", _filePath);

                    if (!File.Exists(_filePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                        File.WriteAllText(_filePath, "[]");
                    }

                    var json = File.ReadAllText(_filePath);
                    notifications = JsonSerializer.Deserialize<List<Notification>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Notification>();

                    _logger.LogInformation("Loaded {Count} notifications", notifications.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load notifications from {FilePath}", _filePath);
                    throw;
                }
            }
        }
        private void SaveData()
        {
            lock (_lock)
            {
                try
                {
                    _logger.LogInformation("Saving {Count} notifications to {FilePath}", notifications.Count, _filePath);

                    var json = JsonSerializer.Serialize(notifications, new JsonSerializerOptions { WriteIndented = true });
                    var temp = _filePath + ".tmp";
                    File.WriteAllText(temp, json);
                    File.Move(temp, _filePath, true);

                    _logger.LogInformation("Saved notifications to {FilePath}", _filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save notifications to {FilePath}", _filePath);
                    throw;
                }
            }
        }

        public Task<Notification> AddAsync(Notification notification)
        {
            lock (_lock)
            {
                notification.Id = Guid.NewGuid().ToString();
                notifications.Add(notification);
                SaveData();
                _logger.LogInformation("Added notification {NotificationId}", notification.Id);
                return Task.FromResult(notification);
            }
        }

        public Task DeleteAsync(string id)
        {
            lock (_lock)
            {
                var existing = notifications.FirstOrDefault(c => c.Id == id);
                if (existing != null)
                {
                    notifications.Remove(existing);
                    SaveData();
                    _logger.LogInformation("Deleted notification {NotificationId}", id);
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existing notification {NotificationId}", id);
                }
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<Notification>> GetAllAsync()
        {
            lock (_lock)
            {
                _logger.LogDebug("Returning all notifications (count={Count})", notifications.Count);
                return Task.FromResult(notifications.ToArray().AsEnumerable());
            }
        }

        public Task<Notification?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var item = notifications.FirstOrDefault(c => c.Id == id);
                if (item != null)
                    _logger.LogDebug("Found notification {NotificationId}", id);
                else
                    _logger.LogDebug("Notification {NotificationId} not found", id);
                return Task.FromResult(item);
            }
        }


        public Task SaveChangesAsync()
        {
            SaveData();
            _logger.LogInformation("Saved changes to notifications file");
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Notification notification)
        {
            lock (_lock)
            {
                var existing = notifications.FirstOrDefault(c => c.Id == notification.Id);
                if (existing != null)
                {
                    notifications.Remove(existing);
                    notifications.Add(notification);
                    SaveData();
                    _logger.LogInformation("Updated notification {NotificationId}", notification.Id);
                }
                else
                {
                    _logger.LogWarning("Attempted to update non-existing notification {NotificationId}", notification.Id);
                }
                return Task.CompletedTask;
            }
        }
    }
}
