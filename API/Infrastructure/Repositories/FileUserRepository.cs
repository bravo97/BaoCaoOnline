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
    public class FileUserRepository : IUserRepository
    {
        private readonly string _filePath;
        private List<User> _users = new List<User>();
        private readonly object _lock = new();
        private readonly ILogger<FileUserRepository> _logger;

        public FileUserRepository(ILogger<FileUserRepository> logger)
        {
            _logger = logger;
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "users.json");
            LoadData();
        }
        private void LoadData()
        {
            lock (_lock)
            {
                try
                {
                    _logger.LogInformation("Loading users from {FilePath}", _filePath);

                    if (!File.Exists(_filePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                        File.WriteAllText(_filePath, "[]");
                    }

                    var json = File.ReadAllText(_filePath);
                    _users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<User>();

                    _logger.LogInformation("Loaded {Count} users", _users.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load users from {FilePath}", _filePath);
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
                    _logger.LogInformation("Saving {Count} users to {FilePath}", _users.Count, _filePath);

                    var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                    var temp = _filePath + ".tmp";
                    File.WriteAllText(temp, json);
                    File.Move(temp, _filePath, true);

                    _logger.LogInformation("Saved users to {FilePath}", _filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save users to {FilePath}", _filePath);
                    throw;
                }
            }
        }
        public Task<User?> GetByUsernameAsync(string username)
        {
            lock (_lock)
            {
                var user = _users.Find(u => u.Username == username);
                _logger.LogDebug("GetByUsername {Username} -> {Found}", username, user != null);
                return Task.FromResult(user);
            }
        }

        public Task<User?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                _logger.LogDebug("GetById {Id} -> {Found}", id, user != null);
                return Task.FromResult(user);
            }
        }

        public Task AddAsync(User user)
        {
            lock (_lock)
            {
                _users.Add(user);
                SaveData();
                _logger.LogInformation("Added user {Username}", user.Username);
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            lock (_lock)
            {
                _logger.LogDebug("Returning all users (count={Count})", _users.Count);
                return Task.FromResult(_users.ToArray().AsEnumerable());
            }
        }

        public Task SaveChangesAsync()
        {
            // Logic to save _users to FilePath  
            SaveData();
            _logger.LogInformation("Saved changes to users file");
            return Task.CompletedTask;
        }
    }
}
