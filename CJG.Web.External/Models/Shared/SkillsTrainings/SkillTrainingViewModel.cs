using System;
using System.Configuration;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class SkillTrainingViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public int? EligibleExpenseTypeId { get; set; }
		public int? EligibleCostBreakdownId { get; set; }
		public int? ServiceLineId { get; set; }
		public SkillTrainingDetailsViewModel SkillTrainingDetails { get; set; } = new SkillTrainingDetailsViewModel();
		public string RowVersion { get; set; }
		public int MaxUploadSize { get; set; }

		public SkillTrainingViewModel()
		{
			var maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			MaxUploadSize = maxUploadSize / 1024 / 1024;
		}

		public SkillTrainingViewModel(GrantApplication grantApplication) : this()
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			GrantApplicationId = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			SkillTrainingDetails = new SkillTrainingDetailsViewModel(grantApplication);
			EligibleExpenseTypeId = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining).Select(x => x.Id).FirstOrDefault();
			SkillTrainingDetails.EligibleExpenseTypeId = EligibleExpenseTypeId;
		}

		public SkillTrainingViewModel(TrainingProgram trainingProgram) : this(trainingProgram?.GrantApplication)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			trainingProgram.IsSkillsTraining = true;
			SkillTrainingDetails = new SkillTrainingDetailsViewModel(trainingProgram);
			EligibleCostBreakdownId = trainingProgram.EligibleCostBreakdownId;
		}
	}
}
