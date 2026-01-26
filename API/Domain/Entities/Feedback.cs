using System;

namespace Domain.Entities
{
    public enum FeedbackStatus
    {
        New,
        Open,
        Closed
    }

    public class Feedback
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? CustomerId { get; set; }
        public string? UserEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public FeedbackStatus Status { get; set; } = FeedbackStatus.New;
        public string? Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResponseAt { get; set; }
        public string? Metadata { get; set; }
    }
}
