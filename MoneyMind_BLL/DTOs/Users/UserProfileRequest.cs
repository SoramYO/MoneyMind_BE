namespace MoneyMind_BLL.DTOs.Users
{
    public class UserProfileRequest
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
} 