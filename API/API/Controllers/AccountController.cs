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
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
                var accounts = await _accountService.GetByCustomerAsync(customerId);
                return Ok(ApiResponse<object>.Ok(accounts, "Lấy danh sách tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi lấy danh sách khách hàng: {ex.Message}"));
            }
        }


        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(string customerId)
        {
            var accounts = await _accountService.GetByCustomerAsync(customerId);
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Account account)
        {
            var result = await _accountService.AddAccountAsync(account);
            if (!result) return BadRequest("Username đã tồn tại");
            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Account account)
        {
            account.Id = id;
            var result = await _accountService.UpdateAccountAsync(account);
            if (!result) return NotFound();
            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Xóa account thành công" });
        }
    }
}
