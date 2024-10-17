using System.Collections.Generic;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.IntakePeriods
{
    public class IntakePeriodsListModel : BaseViewModel
	{
		public List<IntakePeriodsTrainingPeriodModel> TrainingPeriods { get; set; }

		public IntakePeriodsListModel()
		{
			TrainingPeriods = new List<IntakePeriodsTrainingPeriodModel>();
		}
	}
}