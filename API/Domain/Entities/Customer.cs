using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thông tin kết nối đến máy chủ dữ liệu của khách hàng
        public string IPAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string SqlLogin { get; set; } = string.Empty;
        public string SqlReport { get; set; } = string.Empty;
        public string SqlParameter { get; set; } = string.Empty;
        public string SqlColumnQuery { get; set; } = string.Empty; // Query lấy danh sách cột báo cáo

        public string? Note { get; set; }
    }
}
