using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.IntakePeriods
{
	public class TrainingPeriodStatusModel : BaseViewModel
	{
		public string TargetStatus { get; set; }
		public bool CannotAffectIntakePeriod { get; set; }
		public string DialogMessage { get; set; }
		public bool ShowYesAndNoButtons { get; set; }
	}
}