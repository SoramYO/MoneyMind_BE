using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.ChatBots
{
    public class PeriodComparisonRequest<T>
    {
        public IEnumerable<T> CurrentPeriod { get; set; }
        public IEnumerable<T> PreviousPeriod { get; set; }
    }
}
