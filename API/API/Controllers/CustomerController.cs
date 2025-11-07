using API.DTO;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();
            return Ok(customers);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            await _customerRepository.AddAsync(customer);
            return Ok(new { message = "Thêm khách hàng thành công", customer });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Customer customer)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            customer.Id = id; // đảm bảo giữ ID cũ
            await _customerRepository.UpdateAsync(customer);
            return Ok(new { message = "Cập nhật khách hàng thành công" });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _customerRepository.DeleteAsync(id);
            return Ok(new { message = "Xóa khách hàng thành công" });
        }


    }
}
