using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReportParameter
    {
        public string Name { get; set; }
        public string Param { get; set; }
        public string ParamName { get; set; }
        public ReportParameterQuery Query { get; set; }
        public List<Dictionary<string, object>> DataParameter { get; set; }
    }

    public class ReportParameterQuery
    {
        public string SqlQuery { get; set; }
        public string ColumnValue { get; set; }
        public string ColumnDisplay { get; set; }
    }
}
