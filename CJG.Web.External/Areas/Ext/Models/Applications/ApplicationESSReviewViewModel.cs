using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationESSReviewViewModel : BaseApplicationViewModel
	{
		#region Properties
		public string NextStepUrl { get; set; }
		public string PreviousStepUrl { get; set; }
		public int CurrentStep { get; set; } = 2;
		public int Steps { get; set; } = 2;
		#endregion

		#region Constructors
		public ApplicationESSReviewViewModel()
		{

		}

		public ApplicationESSReviewViewModel(GrantApplication grantApplication)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.DynamicProcesses = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive).Select(x => new DynamicProcess(grantApplication, x)).OrderBy(x => x.RowSequence).ThenBy(x => x.ServiceCategoryCaption).ToArray();

			this.NextStepUrl = string.Format("/Ext/Application/Review/Training/Cost/View/{0}", grantApplication.Id);

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Any())
				{
					this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();
					var programConfiguration = grantApplication.GrantOpening.GrantStream.ProgramConfiguration;
					this.PreviousStepUrl = string.Format("/Ext/Application/Review/Skills/Training/View/{0}/{1}", grantApplication.Id, programConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).OrderBy(x => x.RowSequence).ThenBy(x => x.Caption).Select(x => x.Id).Last());
				}
				else
				{
					this.PreviousStepUrl = string.Format("/Ext/Application/Review/Program/View/{0}", grantApplication.Id);
				}

				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
				{
					this.Steps += 1;
					this.CurrentStep = this.Steps;
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