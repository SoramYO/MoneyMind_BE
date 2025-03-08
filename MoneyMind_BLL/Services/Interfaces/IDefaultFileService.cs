using MoneyMind_BLL.DTOs.DataDefaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IDefaultFileService
    {
        Task<DataDefaults> GetDefaultDataAsync();
        Task WriteDefaultDataAsync(DataDefaults data);
    }
}
