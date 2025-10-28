using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileCustomerRepository : ICustomerRepository
    {
        private const string FilePath = "Data/customers.json";
        private List<Customer> customers = new List<Customer>();
        public async Task AddAsync(Customer customer)
        {
            customers.Add(customer);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await Task.FromResult(customers);
        }

        public async Task<Customer?> GetByIDAsync(Guid id)
        {
            return await Task.FromResult(customers.Find(cus => cus.Id == id));
        }

        public async Task SaveChangesAsync()
        {
            // Logic to save customers to FilePath  
            await Task.CompletedTask;
        }
    }
}
