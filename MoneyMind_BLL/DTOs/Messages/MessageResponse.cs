using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Messages
{
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string MessageContent { get; set; } = null!;
        public DateTime SentTime { get; set; }
        public MessageType MessageType { get; set; }
        public bool IsBotResponse { get; set; }
        public Guid ChatId { get; set; }
    }
}
