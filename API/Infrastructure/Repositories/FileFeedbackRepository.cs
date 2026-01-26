using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileFeedbackRepository : IFeedbackRepository
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<Feedback> _items = new();
        private readonly ILogger<FileFeedbackRepository> _logger;

        public FileFeedbackRepository(ILogger<FileFeedbackRepository> logger)
        {
            _logger = logger;
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "feedbacks.json");
            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            Load();
        }

        private void Load()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                    File.WriteAllText(_filePath, "[]");
                var json = File.ReadAllText(_filePath);
                _items = JsonSerializer.Deserialize<List<Feedback>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Feedback>();
            }
        }

        private void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
                var temp = _filePath + ".tmp";
                File.WriteAllText(temp, json);
                File.Move(temp, _filePath, true);
            }
        }

        public Task<Feedback> AddAsync(Feedback feedback)
        {
            lock (_lock)
            {
                feedback.Id = Guid.NewGuid().ToString();
                feedback.CreatedAt = DateTime.UtcNow;
                _items.Add(feedback);
                Save();
                _logger.LogInformation("Added feedback {Id} by {Email}", feedback.Id, feedback.UserEmail);
                return Task.FromResult(feedback);
            }
        }

        public Task<Feedback?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                return Task.FromResult(item);
            }
        }

        public Task<IEnumerable<Feedback>> QueryAsync(int page, int pageSize)
        {
            lock (_lock)
            {
                var skip = Math.Max(0, page - 1) * pageSize;
                var result = _items.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(pageSize).ToArray().AsEnumerable();
                return Task.FromResult(result);
            }
        }

        public Task UpdateAsync(Feedback feedback)
        {
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(x => x.Id == feedback.Id);
                if (existing != null)
                {
                    _items.Remove(existing);
                    _items.Add(feedback);
                    Save();
                }
                return Task.CompletedTask;
            }
        }

        public Task DeleteAsync(string id)
        {
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    _items.Remove(existing);
                    Save();
                }
                return Task.CompletedTask;
            }
        }
    }
}
