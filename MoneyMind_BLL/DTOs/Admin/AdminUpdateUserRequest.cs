using System.ComponentModel.DataAnnotations;

namespace MoneyMind_BLL.DTOs.Admin
{
    public class AdminUpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
} 