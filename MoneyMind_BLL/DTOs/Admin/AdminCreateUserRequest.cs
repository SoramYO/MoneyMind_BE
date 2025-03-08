using System.ComponentModel.DataAnnotations;

namespace MoneyMind_BLL.DTOs.Admin
{
    public class AdminCreateUserRequest
    {
        
		[Required]
        [EmailAddress]
        public string Email { get; set; }

		public string FullName { get; set; }

		[Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
} 