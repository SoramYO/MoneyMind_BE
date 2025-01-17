using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string MessageContent { get; set; } = null!;
        public DateTime SentTime { get; set; }
        public string MessageType { get; set; } = null!;
        public bool IsBotResponse { get; set; }
        public virtual Chat Chat { get; set; } = null!;
    }
}
