using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetByCustomerIdAsync(string customerId);
        Task<Account?> GetByIdAsync(string id);
        Task<Account?> GetByUsernameAsync(string username);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
