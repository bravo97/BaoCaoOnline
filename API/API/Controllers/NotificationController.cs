using Application.Models;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _notificationService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(items, "Lấy danh sách thông báo thành công"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _notificationService.GetByIdAsync(id);
            if (item == null)
                return NotFound(ApiResponse<object>.Fail("Thông báo không tồn tại"));
            return Ok(ApiResponse<object>.Ok(item, "Lấy thông tin thông báo thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu không hợp lệ"));

            var notification = new Notification
            {
                Title = dto.Title,
                Description = dto.Description,
                DateCreate = DateTime.UtcNow,
                DateUpdate = DateTime.UtcNow
            };

            var created = await _notificationService.AddNotificationAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<object>.Ok(created, "Tạo thông báo thành công"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu không hợp lệ"));

            var existing = await _notificationService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<object>.Fail("Thông báo không tồn tại"));

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.DateUpdate = DateTime.UtcNow;

            var ok = await _notificationService.UpdateNotificationAsync(existing);
            if (!ok) return BadRequest(ApiResponse<object>.Fail("Cập nhật thất bại"));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _notificationService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<object>.Fail("Thông báo không tồn tại"));

            var ok = await _notificationService.DeleteNotificationAsync(id);
            if (!ok) return BadRequest(ApiResponse<object>.Fail("Xóa thất bại"));
            return NoContent();
        }
    }
}
