using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var accounts = await _accountService.GetAccounts(cancellationToken);
            // simple paging in-memory
            var skip = Math.Max(0, page - 1) * pageSize;
            var paged = accounts.Skip(skip).Take(pageSize);
            return Ok(ApiResponse<object>.Ok(paged, "Lấy danh sách tài khoản thành công"));
        }
    }
}
