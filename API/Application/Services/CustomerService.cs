using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomerService : ICustomerRepository
    {
        public Task AddAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByIDAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
