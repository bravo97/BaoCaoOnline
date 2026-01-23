using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileNotificationReponsitory : INotificationReponsitory
    {
        private readonly string _filePath;
        private List<Notification> notifications = new();

        public FileNotificationReponsitory()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "notification.json");

            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            LoadData();
        }

        private void LoadData()
        {
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
        }
        private void SaveData()
        {
            var json = JsonSerializer.Serialize(notifications, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public Task<Notification> AddAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid().ToString();
            notifications.Add(notification);
            SaveData();
            return Task.FromResult(notification);
        }

        public Task DeleteAsync(string id)
        {
            var existing = notifications.FirstOrDefault(c => c.Id == id);
            if (existing != null)
            {
                notifications.Remove(existing);
                SaveData();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Notification>> GetAllAsync() =>
            Task.FromResult(notifications.AsEnumerable());

        public Task<Notification?> GetByIdAsync(string id) =>
            Task.FromResult(notifications.FirstOrDefault(c => c.Id == id));


        public Task SaveChangesAsync()
        {
            SaveData();
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Notification notification)
        {
            var existing = notifications.FirstOrDefault(c => c.Id == notification.Id);
            if (existing != null)
            {
                notifications.Remove(existing);
                notifications.Add(notification);
                SaveData();
            }
            return Task.CompletedTask;
        }
    }
}
