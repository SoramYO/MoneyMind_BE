using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs.TransactionTags;
using MoneyMind_BLL.DTOs.Users;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<WalletType, WalletTypeResponse>().ReverseMap();

            CreateMap<SubWalletType, SubWalletTypeResponse>().ReverseMap();
            CreateMap<SubWalletTypeRequest, SubWalletType>().ReverseMap();

            CreateMap<Wallet, Attachment_WalletResponse>().ReverseMap();
            CreateMap<Wallet, WalletResponse>().ReverseMap();
            CreateMap<WalletRequest, Wallet>().ReverseMap();

            CreateMap<TransactionRequest, Transaction>().ReverseMap();
            CreateMap<Transaction, TransactionResponse>().ReverseMap();
            CreateMap<Transaction, Attachment_TransactionResponse>().ReverseMap();

            CreateMap<TransactionTag, Attachment_TransactionsTagResponse>().ReverseMap();

            CreateMap<Tag, TagResponse>().ReverseMap();

            CreateMap<MonthlyGoalRequest, MonthlyGoal>();
            CreateMap<MonthlyGoal, MonthlyGoalResponse>();

            CreateMap<GoalItemRequest, GoalItem>();
            CreateMap<GoalItem, GoalItemResponse>();

			CreateMap<IdentityUser, UserProfileResponse>();
			CreateMap<UserProfileRequest, IdentityUser>();
		}
    }
}
