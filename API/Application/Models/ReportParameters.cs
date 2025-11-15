using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ReportParameters
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? SoCT { get; set; }
        public string? MaHang { get; set; }
        public string? KhoHang { get; set; }
        public string? NhomHang { get; set; }
        public string? ChungLoai { get; set; }
        public string? KhachHang { get; set; }
        public string? NhaCungCap { get; set; }
        public string? KichCo { get; set; }
        public string? MauSac { get; set; }
        public string? Serial { get; set; }
        public string? ThuongHieu { get; set; }
    }
}
