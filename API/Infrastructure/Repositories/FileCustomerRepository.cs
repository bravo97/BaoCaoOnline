using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Infrastructure.Repositories
{
    public class FileCustomerRepository : ICustomerRepository
    {
        private readonly string _filePath;
        private List<Customer> _customers = new();
        private readonly object _lock = new();
        private readonly ILogger<FileCustomerRepository> _logger;
        private readonly IDataProtector _protector;
        private bool _needResave = false;

        public FileCustomerRepository(ILogger<FileCustomerRepository> logger, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _protector = dataProtectionProvider.CreateProtector("FileCustomerRepository.Password");
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "customers.json");

            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            LoadData();

            if (_needResave)
            {
                // Re-save to ensure passwords are stored protected
                SaveData();
            }
        }

        private void LoadData()
        {
            lock (_lock)
            {
                try
                {
                    _logger.LogInformation("Loading customers from {FilePath}", _filePath);

                    if (!File.Exists(_filePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                        File.WriteAllText(_filePath, "[]");
                    }

                    var json = File.ReadAllText(_filePath);
                    _customers = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Customer>();

                    // Try to unprotect passwords; if fail, mark for resave
                    foreach (var c in _customers)
                    {
                        if (string.IsNullOrEmpty(c.Password)) continue;
                        try
                        {
                            var un = _protector.Unprotect(c.Password);
                            c.Password = un;
                        }
                        catch
                        {
                            // Password appears to be plaintext (not protected)
                            _logger.LogInformation("Customer {CustomerId} password not protected; will protect on next save", c.Id);
                            _needResave = true;
                        }
                    }

                    _logger.LogInformation("Loaded {Count} customers", _customers.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load customers from {FilePath}", _filePath);
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
                    _logger.LogInformation("Saving {Count} customers to {FilePath}", _customers.Count, _filePath);

                    // Create a copy where passwords are protected
                    var saveList = _customers.Select(c => new Customer
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        IPAddress = c.IPAddress,
                        Port = c.Port,
                        ServerName = c.ServerName,
                        UserName = c.UserName,
                        Password = string.IsNullOrEmpty(c.Password) ? string.Empty : _protector.Protect(c.Password),
                        DatabaseName = c.DatabaseName,
                        SqlLogin = c.SqlLogin,
                        SqlReport = c.SqlReport,
                        SqlParameter = c.SqlParameter,
                        SqlColumnQuery = c.SqlColumnQuery,
                        Note = c.Note
                    }).ToList();

                    var json = JsonSerializer.Serialize(saveList, new JsonSerializerOptions { WriteIndented = true });
                    var temp = _filePath + ".tmp";
                    File.WriteAllText(temp, json);
                    // Atomic replace (overwrite) of target file
                    File.Move(temp, _filePath, true);

                    _logger.LogInformation("Saved customers to {FilePath}", _filePath);
                    _needResave = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save customers to {FilePath}", _filePath);
                    throw;
                }
            }
        }

        public Task<Customer> AddAsync(Customer customer)
        {
            lock (_lock)
            {
                customer.Id = Guid.NewGuid().ToString();
                _customers.Add(customer);
                SaveData();
                _logger.LogInformation("Added customer {CustomerId} ({Name})", customer.Id, customer.Name);
                return Task.FromResult(customer);
            }
        }

        public Task DeleteAsync(string id)
        {
            lock (_lock)
            {
                var existing = _customers.FirstOrDefault(c => c.Id == id);
                if (existing != null)
                {
                    _customers.Remove(existing);
                    SaveData();
                    _logger.LogInformation("Deleted customer {CustomerId}", id);
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existing customer {CustomerId}", id);
                }
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            lock (_lock)
            {
                _logger.LogDebug("Returning all customers (count={Count})", _customers.Count);
                // return a copy to avoid callers modifying internal collection
                return Task.FromResult(_customers.Select(c => new Customer
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    IPAddress = c.IPAddress,
                    Port = c.Port,
                    ServerName = c.ServerName,
                    UserName = c.UserName,
                    Password = c.Password, // in-memory password is plaintext
                    DatabaseName = c.DatabaseName,
                    SqlLogin = c.SqlLogin,
                    SqlReport = c.SqlReport,
                    SqlParameter = c.SqlParameter,
                    SqlColumnQuery = c.SqlColumnQuery,
                    Note = c.Note
                }).ToArray().AsEnumerable());
            }
        }

        public Task<Customer?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var customer = _customers.FirstOrDefault(c => c.Id == id);
                if (customer != null)
                    _logger.LogDebug("Found customer {CustomerId}", id);
                else
                    _logger.LogDebug("Customer {CustomerId} not found", id);
                return Task.FromResult(customer);
            }
        }

        public Task SaveChangesAsync()
        {
            SaveData();
            _logger.LogInformation("Saved changes to customers file");
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Customer customer)
        {
            lock (_lock)
            {
                var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
                if (existing != null)
                {
                    _customers.Remove(existing);
                    _customers.Add(customer);
                    SaveData();
                    _logger.LogInformation("Updated customer {CustomerId}", customer.Id);
                }
                else
                {
                    _logger.LogWarning("Attempted to update non-existing customer {CustomerId}", customer.Id);
                }
                return Task.CompletedTask;
            }
        }
    }
}
