using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationGrantProgramReviewViewModel : BaseApplicationViewModel
	{
		#region Properties
		public string NextStepUrl { get; set; }
		public string PreviousStepUrl { get; set; }
		public int CurrentStep { get; set; } = 2;
		public int Steps { get; set; } = 2;
		#endregion

		#region Constructors
		public ApplicationGrantProgramReviewViewModel() : base()
		{

		}
		public ApplicationGrantProgramReviewViewModel(GrantApplication grantApplication) : base(grantApplication)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.EnableAttachments = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;

			this.StartDate = grantApplication.StartDate == null ? null : (DateTime?)((DateTime)grantApplication.StartDate).ToLocalMorning();
			this.EndDate = grantApplication.EndDate == null ? null : (DateTime?)((DateTime)grantApplication.EndDate).ToLocalMidnight();

			this.GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate;
			this.GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate;

			this.FullName = grantApplication.GrantOpening.GrantStream.FullName;


			this.PreviousStepUrl = string.Format("/Ext/Application/Review/View/{0}", grantApplication.Id);

			this.ProgramDescription = new OverviewProgramDescription(grantApplication);

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Any())
				{
					this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();
					var programConfiguration = grantApplication.GrantOpening.GrantStream.ProgramConfiguration;
					this.NextStepUrl = string.Format("/Ext/Application/Review/Skills/Training/View/{0}/{1}", grantApplication.Id, programConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).OrderBy(x => x.RowSequence).ThenBy(x => x.Caption).Select(x => x.Id).First());
				}

				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
				{
					this.Steps += 1;
					if (string.IsNullOrEmpty(this.NextStepUrl))
					{
						this.NextStepUrl = string.Format("/Ext/Application/Review/ESS/View/{0}", grantApplication.Id);
					}
				}

				if (string.IsNullOrEmpty(this.NextStepUrl))
				{
					this.NextStepUrl = string.Format("/Ext/Application/Review/Training/Cost/View/{0}", grantApplication.Id);
				}

				this.Steps += 1;
			}

			if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
			{
				this.Steps += 1;
			}

			this.Steps += 1;
		}
		#endregion
	}
}