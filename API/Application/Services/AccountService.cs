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

        public async Task<IEnumerable<Account>> GetByCustomerAsync(string customerId) =>
            await _accountRepository.GetByCustomerIdAsync(customerId);

        public async Task<Account?> GetByUsernameAsync(string username) =>
            await _accountRepository.GetByUsernameAsync(username);

        public async Task<bool> AddAccountAsync(Account account)
        {
            var exists = await _accountRepository.GetByUsernameAsync(account.Username);
            if (exists != null) return false;

            await _accountRepository.AddAsync(account);
            return true;
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            var existing = await _accountRepository.GetByIdAsync(account.Id);
            if (existing == null) return false;

            await _accountRepository.UpdateAsync(account);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var existing = await _accountRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _accountRepository.DeleteAsync(id);
            return true;
        }
    }
}
