using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IGeminiAPIService
    {
        Task<string> GenerateResponseAsync(string prompt);
    }

}
