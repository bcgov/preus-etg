using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class FiscalGrantOpeningTrainingPeriod
	{
		public int Id { get; set; }
		public string Caption { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string TrainingPeriodDuration
		{
			get
			{
				return string.Format("{0} - {1}",
									 StartDate.ToLocalMorning().ToString("yyyy-MM-dd"),
									 EndDate.ToLocalMidnight().ToString("yyyy-MM-dd"));
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
	}
}
