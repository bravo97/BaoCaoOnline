using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // Lấy tất cả khách hàng
        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _customerRepository.GetAllAsync();
        }

        // Lấy khách hàng theo ID
        public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        // Thêm khách hàng, kiểm tra code trùng
        public async Task<Customer> AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            var all = await _customerRepository.GetAllAsync();

            var result = await _customerRepository.AddAsync(customer);
            return result;
        }

        // Cập nhật khách hàng
        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            var existing = await _customerRepository.GetByIdAsync(customer.Id);
            if (existing == null) return false;

            await _customerRepository.UpdateAsync(customer);
            return true;
        }

        // Xóa khách hàng
        public async Task<bool> DeleteCustomerAsync(string id, CancellationToken cancellationToken = default)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _customerRepository.DeleteAsync(id);
            return true;
        }
    }
}
