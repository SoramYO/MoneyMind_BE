using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.GoogleSheet;

namespace MoneyMind_BLL.Services.Interfaces
{
	public interface ISheetService
	{
		Task<SheetTransction> AddSheetAsync(AddSheetRequest request);
		Task<IEnumerable<SheetTransction>> GetUserSheetsAsync(Guid userId);
	}
}
