using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Chat
    {
        public enum StatusChat
        {
            Actived ,
            Inactived 
        }
        public Chat()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Status = StatusChat.Actived;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public StatusChat Status { get; set; }
        public DateTime LastMessageTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
