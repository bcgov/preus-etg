using System;
using System.ComponentModel.DataAnnotations;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.IntakePeriods
{
	public class IntakePeriodsTrainingPeriodModel : BaseViewModel
	{
		public string Caption { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public bool IsActive { get; set; }
		public DateTime MinimumDate { get; set; }
		public DateTime MaximumDate { get; set; }

		[Required]
		public int FiscalId { get; set; }
		[Required]
		public int GrantProgramId { get; set; }
		[Required]
		public int GrantStreamId { get; set; }

		public bool StartDateDisabled { get; set; }
		public bool EndDateDisabled { get; set; }

		public string FormattedStartDate { get; set; }
		public string FormattedEndDate { get; set; }
		public string Status => IsActive ? "Enabled" : "Disabled";

		public string WarningMessage { get; set; }
	}
}