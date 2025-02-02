using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.GoogleSheet;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;

namespace MoneyMind_BLL.Services.Implementations
{
	public class SheetService : ISheetService
	{
		private readonly ISheetTransactionRepository _sheetRepository;

		public SheetService(ISheetTransactionRepository sheetRepository)
		{
			_sheetRepository = sheetRepository;
		}

		public async Task<SheetTransction> AddSheetAsync(AddSheetRequest request)
		{
			var sheet = new SheetTransction
			{
				SheetId = request.SheetId,
				UserId = request.UserId
			};

			return await _sheetRepository.InsertAsync(sheet);
		}

		public async Task<IEnumerable<SheetTransction>> GetUserSheetsAsync(Guid userId)
		{
			var result = await _sheetRepository.GetAsync(
				filter: x => x.UserId == userId
			);
			return result.Item1;
		}
	}
}
