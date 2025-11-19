using Application.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class SqlParameterBinder
    {
        public static (string sql, List<SqlParameter> parameters) BuildSqlWithParams(
    string originalSql, ReportParameters input)
        {
            var paramList = new List<SqlParameter>();

            // Regex tìm tất cả dạng {?Name}
            var regex = new Regex(@"\{\?(\w+)\}", RegexOptions.Compiled);

            string sql = regex.Replace(originalSql, match =>
            {
                var paramName = match.Groups[1].Value; // "TuNgay"

                // Tìm property trong ReportParameter
                var prop = input.GetType().GetProperty(paramName);
                if (prop == null) return match.Value; // không tìm thấy -> giữ nguyên

                var value = prop.GetValue(input);

                // Nếu người dùng không truyền param => thay bằng NULL
                if (value == null)
                {
                    paramList.Add(new SqlParameter($"@{paramName}", DBNull.Value));
                }
                else
                {
                    paramList.Add(new SqlParameter($"@{paramName}", value));
                }

                return $"@{paramName}";
            });

            return (sql, paramList);
        }

    }
}
