using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<Account>> GetAccounts()=>
            await _accountRepository.GetAllAsync();

        public async Task<Account?> GetByUsernamePasswordAsync(string customerId, string username, string password) =>
            await _accountRepository.GetByUsernamePasswordAsync(customerId, username, password);

    }
}
