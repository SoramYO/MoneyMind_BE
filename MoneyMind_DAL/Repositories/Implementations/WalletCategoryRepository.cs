﻿using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class WalletCategoryRepository : GenericRepository<WalletCategory>, IWalletCategoryRepository
    {
        public WalletCategoryRepository(MoneyMindDbContext context) : base(context)
        {
        }
    }
}
