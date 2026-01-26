using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Account?> GetByUsernamePasswordAsync(string customerId, string username, string password, CancellationToken cancellationToken = default);
    }
}
