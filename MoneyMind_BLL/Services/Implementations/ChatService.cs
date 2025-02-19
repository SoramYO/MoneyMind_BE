using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Chats;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_BLL.Utils;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.Utils;

using MoneyMind_BLL.DTOs.ChatBots;

namespace MoneyMind_BLL.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepository;
        private readonly IIntentDetectorService intentDetectorService;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly IGeminiAPIService geminiApiService;
        private readonly IMapper mapper;

        public ChatService(IChatRepository chatRepository, 
            IIntentDetectorService intentDetectorService,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            IGeminiAPIService geminiApiService,
            IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.intentDetectorService = intentDetectorService;
            this.transactionRepository = transactionRepository;
            this.walletRepository = walletRepository;
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.geminiApiService = geminiApiService;
            this.mapper = mapper;
        }
        public async Task<ChatResponse> AddChatAsync(Guid userId, ChatRequest chatRequest)
        {
            var messageDomain = mapper.Map<Chat>(chatRequest);
            messageDomain.UserId = userId;
            // Use Domain Model to create Author
            messageDomain = await chatRepository.InsertAsync(messageDomain);

            return mapper.Map<ChatResponse>(messageDomain);
        }

        public async Task<string> GenerateResponseAsync(Guid userId, string message)
        {
            // Bước 1: Phân tích nội dung tin nhắn để xác định ý định
            var intent =await intentDetectorService.DetectAsync(message);

            // Bước 2: Thu thập dữ liệu cần thiết từ DB hoặc ngữ cảnh
            // tuỳ theo intent
            string contextData = string.Empty;

            switch (intent)
            {
                // --- Đưa ra lời khuyên / tư vấn ---
                case IntentType.AskSpendingAdvice:
                    {
                        var response = await transactionRepository.GetAsync(
                            filter: t => t.UserId == userId,
                            orderBy: q => q.OrderByDescending(m => m.TransactionDate),
                            includeProperties: "",
                            pageIndex: 1,
                            pageSize: 20
                        );

                        var transactions = response.Item1;
                        var transactionsBotRequest = mapper.Map<List<TransactionBotRequest>>(transactions);

                        contextData = JsonSerializerHelper.SerializeData(transactionsBotRequest);
                        break;
                    }

                case IntentType.AskSavingAdvice:
                    {
                        Expression<Func<MonthlyGoal, bool>> filterExpression = g => g.UserId == userId 
                                    && (g.Month == DateTime.Now.Month && g.Year == DateTime.Now.Year);

                        var response = await monthlyGoalRepository.GetAsync(
                            filter: filterExpression,
                            orderBy: null,
                            includeProperties: "GoalItems.WalletType",
                            pageIndex: 1,
                            pageSize: 10
                        );

                        var monthlyGoals = response.Item1;
                        var monthlyGoalsBotRequest = mapper.Map<List<MonthlyGoalBotRequest>>(monthlyGoals);
                        contextData = JsonSerializerHelper.SerializeData(monthlyGoalsBotRequest);
                        break;
                    }

                case IntentType.AskBudgetAdvice:
                    {
                        var response = await walletRepository.GetAsync(
                                                    filter: t => t.UserId == userId,
                                                    orderBy: q => q.OrderByDescending(m => m.Balance),
                                                    includeProperties: "WalletCategory",
                                                    pageIndex: 1,
                                                    pageSize: 10
                                                );

                        var wallets = response.Item1;
                        var walletsBotRequest = mapper.Map<List<Wallet>>(wallets);
                        contextData = JsonSerializerHelper.SerializeData(walletsBotRequest);
                        break;
                    }
                case IntentType.AskGoalAdvice:
                    {
                        // Lấy tổng thu/chi, ngân sách mong muốn của user
                        Expression<Func<MonthlyGoal, bool>> filterExpression = g => g.UserId == userId
                                    && (g.Month == DateTime.Now.Month && g.Year == DateTime.Now.Year);
                        var response = await monthlyGoalRepository.GetAsync(
                            filter: filterExpression,
                            orderBy: null,
                            includeProperties: "GoalItems.WalletType",
                            pageIndex: 1,
                            pageSize: 10
                        );

                        var monthlyGoals = response.Item1;
                        var monthlyGoalsBotRequest = mapper.Map<List<MonthlyGoalBotRequest>>(monthlyGoals);
                        contextData = JsonSerializerHelper.SerializeData(monthlyGoalsBotRequest);
                        break;
                    }
                case IntentType.ComparePeriodMonthlyGoal:
                    {
                        // Tính toán thông tin tháng hiện tại và tháng trước đó
                        int currentYear = DateTime.Now.Year;
                        int currentMonth = DateTime.Now.Month;
                        DateTime previousMonthDate = DateTime.Now.AddMonths(-1);
                        int previousMonth = previousMonthDate.Month;
                        int previousYear = previousMonthDate.Year;

                        // Xây dựng biểu thức lọc cho mục tiêu của user
                        Expression<Func<MonthlyGoal, bool>> currentFilterExpression = g => g.UserId == userId
                            && (g.Year == currentYear && g.Month == currentMonth);

                        Expression<Func<MonthlyGoal, bool>> previousFilterExpression = g => g.UserId == userId
                             && (g.Year == previousYear && g.Month == previousMonth);

                        // Lấy dữ liệu mục tiêu từ repository với include các thông tin liên quan
                        var currentResponse = await monthlyGoalRepository.GetAsync(
                            filter: currentFilterExpression,
                            orderBy: null,
                            includeProperties: "GoalItems.WalletType",
                            pageIndex: 1,
                            pageSize: 10
                        );
                        var previousResponse = await monthlyGoalRepository.GetAsync(
                            filter: previousFilterExpression,
                            orderBy: null,
                            includeProperties: "GoalItems.WalletType",
                            pageIndex: 1,
                            pageSize: 10
                        );

                        // Lấy danh sách mục tiêu
                        var monthlyGoalsCurrent = currentResponse.Item1;
                        var monthlyGoalsPrevious = previousResponse.Item1;

                        // Map dữ liệu sang DTO phù hợp
                        var currentPeriodDto = mapper.Map<List<MonthlyGoalBotRequest>>(monthlyGoalsCurrent);
                        var previousPeriodDto = mapper.Map<List<MonthlyGoalBotRequest>>(monthlyGoalsPrevious);

                        // Đóng gói dữ liệu vào một đối tượng
                        var periodComparisonResult = new PeriodComparisonRequest<MonthlyGoalBotRequest>
                        {
                            CurrentPeriod = currentPeriodDto,
                            PreviousPeriod = previousPeriodDto
                        };


                        // Serialize dữ liệu thành JSON sử dụng hàm SerializeData chung
                        contextData = JsonSerializerHelper.SerializeData(periodComparisonResult);

                        break;
                    }

                case IntentType.CompareTransaction3DayLatest:
                    {
                        var response = await transactionRepository.GetAsync(
                            filter: t => t.UserId == userId &&
                                        t.TransactionDate > DateTime.UtcNow.AddDays(-3) &&
                                        t.TransactionDate <= DateTime.UtcNow,
                            orderBy: q => q.OrderByDescending(m => m.TransactionDate),
                            includeProperties: "",
                            pageIndex: 1,
                            pageSize: 10
                        );

                        var transactions = response.Item1;
                        var transactionsBotRequest = mapper.Map<List<TransactionBotRequest>>(transactions);
                        contextData = JsonSerializerHelper.SerializeData(transactionsBotRequest);
                        break;
                    }
                // Nếu không xác định được ý định hoặc chưa xử lý
                default:
                    {
                        contextData = "";
                        break;
                    }
            }

            var finalPrompt = BuildPrompt(intent, message, contextData);

            // Bước 4: Gọi API Gemini (hoặc bất kỳ dịch vụ AI nào)
            var geminiResponse = await geminiApiService.GenerateResponseAsync(finalPrompt);

            // Bước 5: Trả kết quả về cho client
            return geminiResponse;
        }

        private string BuildPrompt(IntentType intent, string userMessage, string contextData)
        {
            return $@"
        Người dùng vừa hỏi: {userMessage}
        Ý định đã xác định: {intent}
        Dữ liệu liên quan:
        {contextData}

        Vui lòng đưa ra phản hồi/lời khuyên hữu ích nhất dựa trên dữ liệu trên.
        ";
        }

        public async Task<ChatResponse> GetChatByIdAsync(Guid chatId)
        {
            var chat = await chatRepository.GetByIdAsync(chatId);

            if (chat == null)
            {
                return null;
            }

            return mapper.Map<ChatResponse>(chat);
        }

        public async Task<ChatResponse> GetChatByUserIdAsync(Guid userId)
        {
            Expression<Func<Chat, bool>> filterExpression = s => s.UserId == userId && s.Status == 0;
            var response = await chatRepository.GetAsync(
                        filter: filterExpression,
                        orderBy: null,
                        includeProperties: "",
                        pageIndex: 1,
                        pageSize: 10
                        );
            var chat = response.Item1.FirstOrDefault();

            return mapper.Map<ChatResponse>(chat);
        }

        public async Task<ChatResponse> UpdateChatAsync(Guid userId, Guid chatId, ChatRequest chatRequest)
        {
            var existingChat = await chatRepository.GetByIdAsync(chatId);
            if (existingChat == null || existingChat.UserId != userId )
            {
                return null;
            }

            existingChat.LastMessageTime = chatRequest.LastMessageTime;
            existingChat.UpdatedAt = DateTime.UtcNow;

            existingChat = await chatRepository.UpdateAsync(existingChat);

            return mapper.Map<ChatResponse>(existingChat);
        }
    }
}
