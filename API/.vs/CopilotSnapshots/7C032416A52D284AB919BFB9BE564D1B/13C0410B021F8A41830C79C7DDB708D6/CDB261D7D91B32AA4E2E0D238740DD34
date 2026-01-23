using Application.Interfaces;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileAccountRepository : IAccountRepository
    {
        private readonly ICustomerRepository _customerRepository;
        private static List<Account> _accounts = new();
        // Cache connection string theo CustomerId
        private static ConcurrentDictionary<string, string> _connectionCache = new();
        private static ConcurrentDictionary<string, IEnumerable<Report>> _accountCache = new();

        public FileAccountRepository(ICustomerRepository fileCustomerRepository)
        {
            _customerRepository = fileCustomerRepository;
        }

        private async Task<string> GetConnectionStringAsync(Customer _customer)
        {
            if (_connectionCache.TryGetValue(_customer.Id, out var cachedConn))
                return cachedConn;

            if (_customer == null)
                throw new Exception($"Customer {_customer!.Id} not found");

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _customer.ServerName,
                InitialCatalog = _customer.DatabaseName,
                UserID = _customer.UserName,
                Password = _customer.Password, // có thể mã hóa trước khi lưu
                MultipleActiveResultSets = true,
                ConnectTimeout = 30,
                Encrypt = false,
                TrustServerCertificate = true
            };

            var connStr = builder.ConnectionString;
            _connectionCache[_customer.Id] = connStr;

            await Task.CompletedTask;
            return connStr;
        }

        public Task<IEnumerable<Account>> GetAllAsync() =>
            Task.FromResult(_accounts.AsEnumerable());

        public async Task<Account?> GetByUsernamePasswordAsync(string customerId, string username, string password)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            var connStr = await GetConnectionStringAsync(customer);
            
            try
            {
                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand(customer.SqlLogin, conn)
                {
                    CommandTimeout = 60
                };

                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

                if (!await reader.ReadAsync())
                    return null;

                var user = _accounts.FirstOrDefault(x => x.CustomerId == customerId && x.Username == username && x.Password == password);
                if (user == null)
                {
                    var newUser = new Account()
                    {
                        CustomerId = customerId,
                        UserId = reader["UserKey"]?.ToString() ?? string.Empty,
                        Username = reader["UserName"]?.ToString() ?? string.Empty,
                        Password = reader["Password"]?.ToString() ?? string.Empty,
                        Role = "Regular",
                        Note = reader["Note"]?.ToString() ?? string.Empty,
                        DateLogin = DateTime.Now,
                    };
                    _accounts.Add(newUser);
                    return newUser;
                }
                else
                {
                    user.DateLogin = DateTime.Now;
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                throw;
            }
        }
    }
}
