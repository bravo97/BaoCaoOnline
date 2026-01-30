using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string? UserId { get; set; } // For targeted notifications
        public bool IsRead { get; set; } = false;
        public string? Type { get; set; } // e.g. "Feedback", "System"
        public string? Data { get; set; } // JSON data or ID linkage
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
