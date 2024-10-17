using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class IntakeManagementViewModel
	{
		public List<TrainingPeriodViewModel> TrainingPeriods { get; set; }
		public int? TrainingPeriodId { get; set; }

		public int? FiscalYearId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> FiscalYears { get; set; }

		public int? GrantProgramId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> GrantPrograms { get; set; }

		public int? GrantStreamId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> GrantStreams { get; set; }

		public int? BudgetTypeId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> BudgetTypes { get; set; }

		public IEnumerable<KeyValuePair<int, string>> StateNames
		{
			get
			{
				return TrainingPeriods
					.SelectMany(x => x.GrantOpeningIntakes)
					.Select(x => new { x.Key, x.Value.StateName })
					.Distinct()
					.ToDictionary(x => x.Key, x => x.StateName);
			}
		}

		public class TrainingPeriodViewModel
		{
			public int Id { get; set; }
			public string FiscalYearName { get; set; }
			public string TrainingPeriodName { get; set; }
			public DateTime StartDate { get; set; }
			public DateTime EndDate { get; set; }
			public string Status { get; set; }

			public Dictionary<int, GrantOpeningIntakeViewModel> GrantOpeningIntakes { get; set; }

			public int TotalApplicationsIntake { get; set; }
			public decimal TotalApplicationsIntakeAmt { get; set; }

			public decimal WithdrawnRate { get; set; }
			public decimal RefusalRate { get; set; }
			public decimal SlippageApprovedAmount { get; set; }
			public decimal SlippageClaimedAmount { get; set; }
		}

		public class GrantOpeningIntakeViewModel
		{
			public GrantOpeningIntakeViewModel(string stateName, int number, decimal value, bool valueIsMoney = true)
			{
				StateName = stateName;
				Number = number;
				Value = value;
				ValueIsMoney = valueIsMoney;
			}

			public string StateName { get; }
			public int Number { get; set; }
			public decimal Value { get; set; }
			public bool ValueIsMoney { get; }
		}
	}
}