using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public enum MessageType
    {
        Text,
        Audio,
        Video,
        File,
    }
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
            SentTime = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string MessageContent { get; set; } = null!;
        public DateTime SentTime { get; set; }
        public MessageType MessageType { get; set; }
        public bool IsBotResponse { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid ChatId { get; set; }
        public virtual Chat Chat { get; set; } = null!;
    }
}
