using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Messages;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }
        public async Task<MessageResponse> AddMessageAsync(MessageRequest messageRequest)
        {
            var messageDomain = mapper.Map<Message>(messageRequest);
            // Use Domain Model to create Author
            messageDomain = await messageRepository.InsertAsync(messageDomain);

            return mapper.Map<MessageResponse>(messageDomain);
        }

        public async Task<ListDataResponse> GetMessageByChatIdAsync(Expression<Func<Message, bool>>? filter, Func<IQueryable<Message>, IOrderedQueryable<Message>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await messageRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var messages = response.Item1.Reverse();
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var messagesResponse = mapper.Map<List<MessageResponse>>(messages);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = messagesResponse
            };

            return listResponse;
        }
    }
}
