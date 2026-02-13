using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileAccountRepository : IAccountRepository
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<FileAccountRepository> _logger;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private static ConcurrentBag<Account> _accounts = new();
        // Cache connection string theo CustomerId
        private static ConcurrentDictionary<string, string> _connectionCache = new();
        private static ConcurrentDictionary<string, IEnumerable<Report>> _accountCache = new();

        public FileAccountRepository(ICustomerRepository fileCustomerRepository, ILogger<FileAccountRepository> logger, IDatabaseConnectionFactory connectionFactory)
        {
            _customerRepository = fileCustomerRepository;
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        private Task<string> GetConnectionStringAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (_connectionCache.TryGetValue(customer.Id, out var cachedConn))
                return Task.FromResult(cachedConn);

            var connStr = _connectionFactory.CreateConnectionString(customer);
            _connectionCache[customer.Id] = connStr;

            return Task.FromResult(connStr);
        }

        public static String Md5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
            byte[] data = md5Hasher.ComputeHash(System.Text.Encoding.Unicode.GetBytes(input));
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
            int i = 0;
            for (i = 0; i <= data.Length - 1; i++)
            {
                sBuilder.Append(data[i].ToString("x1"));
            }
            return sBuilder.ToString();
        }

        public Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_accounts.ToArray().AsEnumerable());

        public async Task<Account?> GetByUsernamePasswordAsync(string customerId, string username, string password, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            var connStr = await GetConnectionStringAsync(customer);

            try
            {
                return await RetryHelper.RetryAsync(async () =>
                {
                    using var conn = new SqlConnection(connStr);
                    using var cmd = new SqlCommand(customer.SqlLogin, conn)
                    {
                        CommandTimeout = 60
                    };

                    if (customer.useMD5) password = Md5Hash(password); 

                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    await conn.OpenAsync(cancellationToken);

                    using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);

                    if (!await reader.ReadAsync(cancellationToken))
                        return null;

                    var user = _accounts.FirstOrDefault(x => x.CustomerId == customerId && x.Username == username);
                    if (user == null)
                    {
                        var newUser = new Account()
                        {
                            CustomerId = customerId,
                            UserId = reader["UserKey"]?.ToString() ?? string.Empty,
                            Username = reader["UserName"]?.ToString() ?? string.Empty,
                            FullName = reader["FullName"]?.ToString() ?? string.Empty,
                            Password = string.Empty, // don't cache plaintext password
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
                }, shouldRetry: ex => ex is SqlException, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for customer {CustomerId}: {Message}", customerId, ex.Message);
                throw;
            }
        }
        
        public Task<Account?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var account = _accounts.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(account);
        }
    }
}
