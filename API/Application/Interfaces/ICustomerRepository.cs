using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIDAsync(Guid id);
        Task AddAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
