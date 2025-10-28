using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
