using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Utils
{
    public static class JsonSerializerHelper
    {
        public static string SerializeData<T>(IEnumerable<T> items)
        {
            var result = new
            {
                Data = items,
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Giữ nguyên Unicode, hỗ trợ tiếng Việt
            });
        }

        public static string SerializeData<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Giữ nguyên Unicode
            };

            return JsonSerializer.Serialize(data, options);
        }
    }
}
