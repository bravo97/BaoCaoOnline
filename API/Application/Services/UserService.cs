using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            var existing = await _userRepo.GetByUsernameAsync(username);
            if (existing != null) return false;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Email = email
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null) return null;

            if (user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
