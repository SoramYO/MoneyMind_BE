using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_BLL.Utils;

namespace MoneyMind_BLL.Services.Implementations
{
    public class GeminiAPIService : IGeminiAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiAPIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API key cannot be null");
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "Không thể tạo phản hồi từ nội dung trống.";

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var requestJson = JsonSerializerHelper.SerializeData(requestBody);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiKey}",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return $"Xin lỗi chúng tôi chưa hiểu ý của bạn. Vui lòng thử lại.";
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var botResponse = jsonResponse.GetProperty("candidates")[0]
                                              .GetProperty("content")
                                              .GetProperty("parts")[0]
                                              .GetProperty("text")
                                              .GetString();

                return botResponse ?? "Xin lỗi chúng tôi chưa hiểu ý của bạn. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                return $"Xin lỗi chúng tôi chưa hiểu ý của bạn. Vui lòng thử lại.";
            }
        }
    }
}
