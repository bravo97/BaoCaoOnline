using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Infrastructure.Repositories
{
    public class FileRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<RefreshToken> _tokens = new();
        private readonly ILogger<FileRefreshTokenRepository> _logger;

        public FileRefreshTokenRepository(ILogger<FileRefreshTokenRepository> logger)
        {
            _logger = logger;
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "refresh_tokens.json");
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
                _tokens = JsonSerializer.Deserialize<List<RefreshToken>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<RefreshToken>();
            }
        }

        private void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_tokens, new JsonSerializerOptions { WriteIndented = true });
                var temp = _filePath + ".tmp";
                File.WriteAllText(temp, json);
                File.Move(temp, _filePath, true);
            }
        }

        public Task<RefreshToken> AddAsync(RefreshToken token)
        {
            lock (_lock)
            {
                _tokens.Add(token);
                Save();
                _logger.LogInformation("Added refresh token for user {UserId}", token.UserId);
                return Task.FromResult(token);
            }
        }

        public Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            lock (_lock)
            {
                var t = _tokens.FirstOrDefault(x => x.TokenHash == tokenHash);
                return Task.FromResult(t);
            }
        }

        public Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId)
        {
            lock (_lock)
            {
                var list = _tokens.Where(x => x.UserId == userId).ToArray().AsEnumerable();
                return Task.FromResult(list);
            }
        }

        public Task UpdateAsync(RefreshToken token)
        {
            lock (_lock)
            {
                var existing = _tokens.FirstOrDefault(x => x.Id == token.Id);
                if (existing != null)
                {
                    _tokens.Remove(existing);
                    _tokens.Add(token);
                    Save();
                }
                return Task.CompletedTask;
            }
        }

        public Task DeleteAsync(string id)
        {
            lock (_lock)
            {
                var existing = _tokens.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    _tokens.Remove(existing);
                    Save();
                }
                return Task.CompletedTask;
            }
        }
    }
}
