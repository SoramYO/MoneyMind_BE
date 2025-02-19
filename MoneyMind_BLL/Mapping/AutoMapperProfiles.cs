using AutoMapper;
using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.Chats;
using MoneyMind_BLL.DTOs.Messages;
using MoneyMind_BLL.DTOs.ChatBots;

namespace MoneyMind_BLL.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Activity, ActivityResponse>().ReverseMap();
            CreateMap<ActivityRequest, Activity>().ReverseMap();
            CreateMap<Attachment_ActivityRequest, Activity>().ReverseMap();

            CreateMap<WalletType, WalletTypeResponse>().ReverseMap();
            CreateMap<WalletTypeRequest, WalletType>().ReverseMap();

            CreateMap<WalletCategory, WalletCategoryResponse>().ReverseMap();
            CreateMap<WalletCategoryRequest, WalletCategory>().ReverseMap();

            CreateMap<Wallet, Attachment_WalletResponse>().ReverseMap();
            CreateMap<Wallet, WalletResponse>().ReverseMap();
            CreateMap<WalletRequest, Wallet>().ReverseMap();

            CreateMap<TransactionRequest, Transaction>().ReverseMap();
            CreateMap<Transaction, TransactionResponse>().ReverseMap();
            CreateMap<Transaction, Attachment_TransactionResponse>().ReverseMap();

            CreateMap<Tag, TagResponse>().ReverseMap();

            CreateMap<GoalItemRequest, GoalItem>().ReverseMap();
            CreateMap<GoalItem, GoalItemResponse>().ReverseMap();
            CreateMap<GoalItemDefault, GoalItem>().ReverseMap();

            CreateMap<MonthlyGoalRequest, MonthlyGoal>().ReverseMap();
            CreateMap<MonthlyGoal, MonthlyGoalResponse>().ReverseMap();

            CreateMap<ChatRequest, Chat>().ReverseMap();
            CreateMap<Chat, ChatResponse>().ReverseMap();

            CreateMap<MessageRequest, Message>().ReverseMap();
            CreateMap<Message, MessageResponse>().ReverseMap();

            CreateMap<Transaction, TransactionBotRequest>().ReverseMap();
            CreateMap<Wallet, WalletBotRequest>().ReverseMap();
            CreateMap<MonthlyGoal, MonthlyGoalBotRequest>().ReverseMap();
            CreateMap<GoalItem, GoalItemBotRequest>().ReverseMap();
        }
    }
}
