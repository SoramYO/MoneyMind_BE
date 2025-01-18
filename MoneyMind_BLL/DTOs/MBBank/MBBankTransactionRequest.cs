using System.Text.Json.Serialization;

namespace MoneyMind_BLL.DTOs.MBBank
{
    public class MBBankTransactionRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("days")]
        public int Days { get; set; }
    }
}