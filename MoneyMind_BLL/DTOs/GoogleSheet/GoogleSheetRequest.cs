using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoogleSheet
{
    public class GoogleSheetRequest
    {
        public required string SheetId { get; set; }
        public required Guid UserId { get; set; }
    }
}