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
    public class FileCustomerRepository : ICustomerRepository
    {
        private readonly string _filePath;
        private List<Customer> _customers = new();

        public FileCustomerRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "customers.json");

            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

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
            _customers = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Customer>();
        }
        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_customers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public Task<Customer> AddAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid().ToString();
            _customers.Add(customer);
            SaveData();
            return Task.FromResult(customer);
        }

        public Task DeleteAsync(string id)
        {
            var existing = _customers.FirstOrDefault(c => c.Id == id);
            if (existing != null)
            {
                _customers.Remove(existing);
                SaveData();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Customer>> GetAllAsync() =>
            Task.FromResult(_customers.AsEnumerable());

        public Task<Customer?> GetByIdAsync(string id) =>
            Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));

        public Task SaveChangesAsync()
        {
            SaveData();
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Customer customer)
        {
            var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existing != null)
            {
                _customers.Remove(existing);
                _customers.Add(customer);
                SaveData();
            }
            return Task.CompletedTask;
        }
    }
}
