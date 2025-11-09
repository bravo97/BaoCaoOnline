using API.DTO;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                return Ok(ApiResponse<object>.Ok(customers, "Lấy danh sách khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi lấy danh sách khách hàng: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));
                return Ok(ApiResponse<object>.Ok(customer, "Lấy thông tin khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi lấy thông tin khách hàng: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            try
            {
                await _customerRepository.AddAsync(customer);
                return Ok(ApiResponse<object>.Ok(customer, "Tạo khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi tạo khách hàng: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Customer customer)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));
                customer.Id = id; // đảm bảo giữ ID cũ
                await _customerRepository.UpdateAsync(customer);
                return Ok(ApiResponse<object>.Ok(existingCustomer, "Cập nhật khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi cập nhật khách hàng: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));
                await _customerRepository.DeleteAsync(id);
                return Ok(ApiResponse<object>.Ok(existingCustomer, "Xóa khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi xóa khách hàng: {ex.Message}"));
            }
        }
    }
}
