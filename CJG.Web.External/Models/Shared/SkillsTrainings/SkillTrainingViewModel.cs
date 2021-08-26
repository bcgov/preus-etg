using CJG.Core.Entities;
using System;
using System.Configuration;
using System.Linq;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class SkillTrainingViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public int? EligibleExpenseTypeId { get; set; }
		public int? EligibleCostBreakdownId { get; set; }
		public int? ServiceLineId { get; set; }
		public SkillTrainingDetailsViewModel SkillTrainingDetails { get; set; } = new SkillTrainingDetailsViewModel();
		public string RowVersion { get; set; }
		public int MaxUploadSize { get; set; }
		#endregion

		#region Constructors
		public SkillTrainingViewModel()
		{
			var maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			MaxUploadSize = maxUploadSize / 1024 / 1024;
		}

		public SkillTrainingViewModel(GrantApplication grantApplication) : this()
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.SkillTrainingDetails = new SkillTrainingDetailsViewModel(grantApplication);
			this.EligibleExpenseTypeId = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining).Select(x => x.Id).FirstOrDefault();
			this.SkillTrainingDetails.EligibleExpenseTypeId = this.EligibleExpenseTypeId;
		}

		public SkillTrainingViewModel(TrainingProgram trainingProgram) : this(trainingProgram?.GrantApplication)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));

			this.SkillTrainingDetails = new SkillTrainingDetailsViewModel(trainingProgram);
			this.EligibleCostBreakdownId = trainingProgram.EligibleCostBreakdownId;
		}
		#endregion
	}
}
