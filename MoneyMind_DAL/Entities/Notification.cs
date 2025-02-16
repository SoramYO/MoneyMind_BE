namespace MoneyMind_DAL.Entities
{
    public class Notification
    {
        public Notification()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 