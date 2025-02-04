namespace MoneyMind_BLL.DTOs.Users
{
    public class UserProfileResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 