using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IClassificationService
    {
        Task<Tag> ClassificationTag(string description);
        Task<int> ClassificationIntent(string message);
    }
}
