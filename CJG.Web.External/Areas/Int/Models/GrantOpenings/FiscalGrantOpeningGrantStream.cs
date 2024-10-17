using CJG.Web.External.Helpers;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class FiscalGrantOpeningGrantStream
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public List<GrantStreamInformation> GrantStreamInformation { get; set; }
		public decimal TotalTargetAmount { get; set; }

		public string DisplayTotalTargetAmount
		{
			get
			{
				return TotalTargetAmount.ToDollarCurrencyString(0);
			}
		}
	}
}
