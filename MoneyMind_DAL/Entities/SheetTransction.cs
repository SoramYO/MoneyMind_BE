using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
	public class SheetTransction
	{
		public SheetTransction()
		{
			Id = Guid.NewGuid();
		}
		public Guid Id { get; set; }
		public string SheetId { get; set; }

		public Guid UserId { get; set; }
	}
}
