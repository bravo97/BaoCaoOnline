using API.DTO;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get()
        {
            var success = await _customerService.GetAllAsync();
            if (success == null) return BadRequest("Customer not exists");
            return Ok(success);
        }

        
    }
}
