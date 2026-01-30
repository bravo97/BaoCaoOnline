using System;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TokenHash { get; set; } = string.Empty; // SHA256 hash of token
        public string UserId { get; set; } = string.Empty; // or AccountId
        public string UserType { get; set; } = "User"; // "User" (Admin) or "Account" (Customer)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? DeviceInfo { get; set; }
    }
}
