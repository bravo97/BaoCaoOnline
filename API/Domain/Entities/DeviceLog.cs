using System;

namespace Domain.Entities
{
    public class DeviceLog
    {
        public DateTime Timestamp { get; set; }
        public string? DeviceType { get; set; } // "Desktop" or "Mobile"
        public string? IpAddress { get; set; }
        public string? Endpoint { get; set; }
    }
}
