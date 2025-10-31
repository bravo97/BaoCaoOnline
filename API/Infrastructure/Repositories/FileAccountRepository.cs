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
    public class FileAccountRepository : IAccountRepository
    {
        private readonly string _filePath;
        private List<Account> _accounts = new();

        public FileAccountRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "accounts.json");

            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                _accounts = new List<Account>();
                SaveToFile();
            }
            else
            {
                var json = File.ReadAllText(_filePath);
                _accounts = string.IsNullOrWhiteSpace(json)
                    ? new List<Account>()
                    : JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public Task<IEnumerable<Account>> GetAllAsync() =>
            Task.FromResult(_accounts.AsEnumerable());

        public Task<IEnumerable<Account>> GetByCustomerIdAsync(string customerId) =>
            Task.FromResult(_accounts.Where(a => a.CustomerId == customerId).AsEnumerable());

        public Task<Account?> GetByIdAsync(string id) =>
            Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));

        public Task<Account?> GetByUsernameAsync(string username) =>
            Task.FromResult(_accounts.FirstOrDefault(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(Account account)
        {
            account.Id = Guid.NewGuid().ToString();
            _accounts.Add(account);
            SaveToFile();
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Account account)
        {
            var existing = _accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existing != null)
            {
                _accounts.Remove(existing);
                _accounts.Add(account);
                SaveToFile();
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var existing = _accounts.FirstOrDefault(a => a.Id == id);
            if (existing != null)
            {
                _accounts.Remove(existing);
                SaveToFile();
            }
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            SaveToFile();
            return Task.CompletedTask;
        }
    }
}
