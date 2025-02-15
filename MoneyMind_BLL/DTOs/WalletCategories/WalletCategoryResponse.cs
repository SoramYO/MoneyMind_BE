using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.WalletCategories
{
    public class WalletCategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string Color { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public Guid WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public string WalletTypeDescription { get; set; }
        public List<ActivityResponse> Activities { get; set; } = new List<ActivityResponse>();
    }
}
