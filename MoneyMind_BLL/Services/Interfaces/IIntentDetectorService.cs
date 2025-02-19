using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.Utils;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IIntentDetectorService
    {
        Task<IntentType> DetectAsync(string message);
    }
}
