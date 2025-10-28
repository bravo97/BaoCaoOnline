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
        private const string FilePath = "Data/users.json";
        private List<User> _users = new List<User>();

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.Find(user => user.Username == username));
        }

        public async Task AddAsync(User user)
        {
            _users.Add(user);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Task.FromResult(_users);
        }

        public async Task SaveChangesAsync()
        {
            // Logic to save _users to FilePath  
            await Task.CompletedTask;
        }
    }
}
