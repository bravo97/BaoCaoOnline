using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Report
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string SqlQuery { get; set; } = string.Empty; // Query lấy danh sách báo cáo
    }

    public class ReportColumn
    {
        public string? ReportId { get; set; }
        public string ColumnName { get; set; } = string.Empty;      // Tên cột trong SQL
        public string DisplayName { get; set; } = string.Empty;     // Tên hiển thị trên UI
        public int ColumnWidth { get; set; }
        public string DataType { get; set; } = string.Empty;
    }
}
