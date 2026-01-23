using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class NotificationDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}
