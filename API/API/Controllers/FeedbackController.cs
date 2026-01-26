using API.DTO;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid data"));

            var fb = new Feedback
            {
                CustomerId = dto.CustomerId,
                UserEmail = dto.UserEmail,
                Subject = dto.Subject,
                Message = dto.Message
            };

            var created = await _feedbackService.CreateAsync(fb);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<object>.Ok(created, "Feedback created"));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var items = await _feedbackService.QueryAsync(page, pageSize);
            return Ok(ApiResponse<object>.Ok(items, "Feedback list"));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _feedbackService.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.Fail("Not found"));
            return Ok(ApiResponse<object>.Ok(item, "Feedback detail"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/response")]
        public async Task<IActionResult> Respond(string id, [FromBody] RespondFeedbackDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Fail("Invalid data"));
            var ok = await _feedbackService.RespondAsync(id, dto.Response);
            if (!ok) return NotFound(ApiResponse<object>.Fail("Not found"));
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] FeedbackStatus status)
        {
            var ok = await _feedbackService.UpdateStatusAsync(id, status);
            if (!ok) return NotFound(ApiResponse<object>.Fail("Not found"));
            return NoContent();
        }
    }
}
