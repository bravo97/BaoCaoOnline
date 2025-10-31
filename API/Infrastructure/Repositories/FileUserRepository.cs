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
    public class FileUserRepository : IUserRepository
    {
        private readonly string _filePath;
        private List<User> _users = new List<User>();

        public FileUserRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "users.json");
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
            _users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<User>();
        }
        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.Find(user => user.Username == username));
        }

        public async Task AddAsync(User user)
        {
            _users.Add(user);
            SaveData();
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Task.FromResult(_users);
        }

        public async Task SaveChangesAsync()
        {
            // Logic to save _users to FilePath  
            SaveData();
            await Task.CompletedTask;
        }
    }
}
