using System.ComponentModel.DataAnnotations;

namespace MoneyMind_BLL.DTOs.Admin
{
    public class AdminUpdateUserRequest
    {
        public string? FullName { get; set; }

		[EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
} 