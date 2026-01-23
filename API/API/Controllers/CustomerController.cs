using API.DTO;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            // map to DTO
            var dtos = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                IPAddress = c.IPAddress,
                Port = c.Port,
                ServerName = c.ServerName,
                DatabaseName = c.DatabaseName,
                SqlLogin = c.SqlLogin,
                SqlReport = c.SqlReport,
                SqlColumnQuery = c.SqlColumnQuery,
                Note = c.Note
            });

            return Ok(ApiResponse<object>.Ok(dtos, "Lấy danh sách khách hàng thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));

            var dto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                IPAddress = customer.IPAddress,
                Port = customer.Port,
                ServerName = customer.ServerName,
                DatabaseName = customer.DatabaseName,
                SqlLogin = customer.SqlLogin,
                SqlReport = customer.SqlReport,
                SqlColumnQuery = customer.SqlColumnQuery,
                Note = customer.Note
            };

            return Ok(ApiResponse<object>.Ok(dto, "Lấy thông tin khách hàng thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu không hợp lệ"));

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                IPAddress = dto.IPAddress,
                Port = dto.Port,
                ServerName = dto.ServerName,
                UserName = dto.UserName,
                Password = dto.Password,
                DatabaseName = dto.DatabaseName,
                SqlLogin = dto.SqlLogin,
                SqlReport = dto.SqlReport,
                SqlColumnQuery = dto.SqlColumnQuery,
                Note = dto.Note
            };

            var created = await _customerService.AddCustomerAsync(customer);

            var resultDto = new CustomerDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email,
                IPAddress = created.IPAddress,
                Port = created.Port,
                ServerName = created.ServerName,
                DatabaseName = created.DatabaseName,
                SqlLogin = created.SqlLogin,
                SqlReport = created.SqlReport,
                SqlColumnQuery = created.SqlColumnQuery,
                Note = created.Note
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<object>.Ok(resultDto, "Tạo khách hàng thành công"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu không hợp lệ"));

            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));

            existingCustomer.Name = dto.Name;
            existingCustomer.Email = dto.Email;
            existingCustomer.IPAddress = dto.IPAddress;
            existingCustomer.Port = dto.Port;
            existingCustomer.ServerName = dto.ServerName;
            existingCustomer.UserName = dto.UserName;
            existingCustomer.Password = dto.Password;
            existingCustomer.DatabaseName = dto.DatabaseName;
            existingCustomer.SqlLogin = dto.SqlLogin;
            existingCustomer.SqlReport = dto.SqlReport;
            existingCustomer.SqlColumnQuery = dto.SqlColumnQuery;
            existingCustomer.Note = dto.Note;

            var ok = await _customerService.UpdateCustomerAsync(existingCustomer);
            if (!ok)
                return BadRequest(ApiResponse<object>.Fail("Cập nhật thất bại"));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(ApiResponse<object>.Fail("Khách hàng không tồn tại"));
            var ok = await _customerService.DeleteCustomerAsync(id);
            if (!ok)
                return BadRequest(ApiResponse<object>.Fail("Xóa thất bại"));
            return NoContent();
        }
    }
}
