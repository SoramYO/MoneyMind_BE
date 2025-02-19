using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Chats;
using MoneyMind_BLL.DTOs.Messages;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponse> AddMessageAsync(MessageRequest messageRequest);
        Task<ListDataResponse> GetMessageByChatIdAsync(Expression<Func<Message, bool>>? filter,
            Func<IQueryable<Message>, IOrderedQueryable<Message>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
    }
}
