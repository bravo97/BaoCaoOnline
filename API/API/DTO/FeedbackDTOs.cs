using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class CreateFeedbackDto
    {
        public string? CustomerId { get; set; }

        [EmailAddress]
        public string? UserEmail { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Message { get; set; } = string.Empty;
    }

    public class RespondFeedbackDto
    {
        [Required]
        [StringLength(5000)]
        public string Response { get; set; } = string.Empty;
    }
}
