using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationReponsitory _notificationRepository;

        public NotificationController(INotificationReponsitory notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _notificationRepository.GetAllAsync();
                return Ok(ApiResponse<object>.Ok(customers, "Lấy danh sách tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi lấy danh sách tài khoản: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var customer = await _notificationRepository.GetByIdAsync(id);
                if (customer == null)
                    return NotFound(ApiResponse<object>.Fail("Tài khoản không tồn tại"));
                return Ok(ApiResponse<object>.Ok(customer, "Lấy thông tin tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi lấy thông tin tài khoản: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
        {
            try
            {
                await _notificationRepository.AddAsync(notification);
                return Ok(ApiResponse<object>.Ok(notification, "Tạo tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi tạo tài khoản: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Notification notification)
        {
            try
            {
                var existingCustomer = await _notificationRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound(ApiResponse<object>.Fail("Tài khoản không tồn tại"));
                notification.Id = id; // đảm bảo giữ ID cũ
                await _notificationRepository.UpdateAsync(notification);
                return Ok(ApiResponse<object>.Ok(existingCustomer, "Cập nhật tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi cập nhật tài khoản: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existingCustomer = await _notificationRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    return NotFound(ApiResponse<object>.Fail("Tài khoản không tồn tại"));
                await _notificationRepository.DeleteAsync(id);
                return Ok(ApiResponse<object>.Ok(existingCustomer, "Xóa tài khoản thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi khi xóa tài khoản: {ex.Message}"));
            }
        }
    }
}
