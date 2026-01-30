using Application.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class SqlParameterBinder
    {
        public static (string sql, List<SqlParameter> parameters) BuildSqlWithParams(
            string originalSql, Dictionary<string, object> input)
        {
            var paramList = new List<SqlParameter>();

            // Regex tìm tất cả dạng {?Name}
            var regex = new Regex(@"\{\?(\w+)\}", RegexOptions.Compiled);

            string sql = regex.Replace(originalSql, match =>
            {
                var paramName = match.Groups[1].Value; // "TuNgay", "Kho"

                // Tìm key trong Dictionary (case-insensitive search cho linh hoạt)
                // Hoặc strict tùy requirement. Ở đây dùng CaseInsensitive cho an toàn.
                var key = input.Keys.FirstOrDefault(k => string.Equals(k, paramName, StringComparison.OrdinalIgnoreCase));

                if (key == null) return match.Value; // không tìm thấy -> giữ nguyên placeholder

                var value = input[key];

                // Nếu người dùng không truyền param hoặc value null => thay bằng NULL
                if (value == null || (value is string s && string.IsNullOrEmpty(s)))
                {
                    paramList.Add(new SqlParameter($"@{paramName}", DBNull.Value));
                }
                else
                {
                    // Handle JSON Element types if coming from System.Text.Json deserialization
                    if (value is JsonElement jsonElement)
                    {
                        object? realValue = jsonElement.ValueKind switch
                        {
                            JsonValueKind.String => jsonElement.GetString(),
                            JsonValueKind.Number => (object)jsonElement.GetDouble(), // Use Double to be safe, or logic to check int/long
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => DBNull.Value,
                            _ => jsonElement.ToString() // Fallback
                        };
                        
                         // Refine Number handling if needed (int vs double)
                         if (jsonElement.ValueKind == JsonValueKind.Number)
                         {
                             if (jsonElement.TryGetInt32(out int i)) realValue = i;
                             else if (jsonElement.TryGetInt64(out long l)) realValue = l;
                             else realValue = jsonElement.GetDouble();
                         }

                        paramList.Add(new SqlParameter($"@{paramName}", realValue ?? DBNull.Value));
                    }
                    else
                    {
                        paramList.Add(new SqlParameter($"@{paramName}", value));
                    }
                }

                return $"@{paramName}";
            });

            return (sql, paramList);
        }

    }
}
