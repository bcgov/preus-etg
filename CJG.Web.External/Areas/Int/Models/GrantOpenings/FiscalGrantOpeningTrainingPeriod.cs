using CJG.Web.External.Helpers;
using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class FiscalGrantOpeningTrainingPeriod
	{
		public int Id { get; set; }
		public string Caption { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int GrantStreamId { get; set; }
		public string TrainingPeriodDuration
		{
			get
			{
				return $"{StartDate.ToLocalMorning().ToString("yyyy-MM-dd")} - {EndDate.ToLocalMidnight().ToString("yyyy-MM-dd")}";
			}
		}
		public decimal TotalTrainingPeriodAmount { get; set; }
		public string DisplayTotalTrainingPeriodAmount
		{
			get
			{
				return TotalTrainingPeriodAmount.ToDollarCurrencyString(0);
			}
		}

		public bool IsActive { get; set; }
	}
}
