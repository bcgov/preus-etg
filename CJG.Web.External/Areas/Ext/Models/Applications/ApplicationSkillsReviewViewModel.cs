using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationSkillsReviewViewModel : BaseApplicationViewModel
	{
		#region Properties
		public string Caption { get; set; }
		public string NextStepUrl { get; set; }
		public string PreviousStepUrl { get; set; }
		public int CurrentStep { get; set; } = 2;
		public int Steps { get; set; } = 2;
		public int EligibleExpenseTypeId { get; set; }
		#endregion

		#region Constructors
		public ApplicationSkillsReviewViewModel()
		{

		}

		public ApplicationSkillsReviewViewModel(GrantApplication grantApplication, EligibleExpenseType eligibleExpenseType)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.Caption = eligibleExpenseType.Caption;
			int index = 0;
			bool found = false;

			var programConfiguration = grantApplication.GrantOpening.GrantStream.ProgramConfiguration;

			var eligibleExpenseTypes = programConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).OrderBy(x => x.RowSequence).ThenBy(x => x.Caption).Select(x => x.Id).Distinct().ToArray();

			foreach (var eligibleExpenseTypeId in eligibleExpenseTypes)
			{

				if (found)
				{
					index = eligibleExpenseTypeId;
					break;
				}

				this.CurrentStep += 1;

				if (eligibleExpenseTypeId == eligibleExpenseType.Id)
				{
					if (index == 0)
					{
						PreviousStepUrl = string.Format("/Ext/Application/Review/Program/View/{0}", grantApplication.Id);
					}
					else
					{
						PreviousStepUrl = string.Format("/Ext/Application/Review/Skills/Training/View/{0}/{1}", grantApplication.Id, index);
					}

					found = true;
				}

				index = eligibleExpenseTypeId;
			}

			if (index != eligibleExpenseType.Id)
			{
				this.NextStepUrl = string.Format("/Ext/Application/Review/Skills/Training/View/{0}/{1}", grantApplication.Id, index);
			}

			this.DynamicProcesses = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.Id == eligibleExpenseType.Id && x.IsActive).Select(x => new DynamicProcess(grantApplication, x)).OrderBy(x => x.RowSequence).ThenBy(x => x.ServiceCategoryCaption).ToArray();

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();

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