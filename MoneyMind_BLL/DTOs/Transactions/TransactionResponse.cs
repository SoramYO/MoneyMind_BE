﻿using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.TransactionTags;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public string RecipientName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public Guid UserId { get; set; }
        public Guid? WalletId { get; set; }
        public List<TagResponse> Tags { get; set; } = new();
        public List<ActivityResponse> Activities { get; set; } = new();
    }
}
