using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public enum DisplayMode
	{
		GM1 = 0,
		OpeningLocated = 1,
		OpeningBlocked = 2,
		None = 3
	}
	public class GrantStreamInformation
	{
		public int TrainingPeriodId { get; set; }
		public DisplayMode DisplayMode { get; set; }
		public decimal TargetAmount { get; set; }
		public string DisplayTargetAmount
		{
			get
			{
				return TargetAmount.ToDollarCurrencyString(0);
			}
		}
		public CJG.Core.Entities.GrantOpeningStates OpenState { get; set; }
		public int GrantOpeningId { get; set; }
	}
}
