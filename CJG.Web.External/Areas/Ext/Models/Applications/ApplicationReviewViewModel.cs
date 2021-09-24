using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Overview;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationReviewViewModel : BaseApplicationViewModel
	{
		#region Properties
		public OverviewApplicantContact ApplicantContact { get; set; }
		public OverviewApplicantViewModel Applicant { get; set; }
		public string NextStepUrl { get; set; }
		public int Steps { get; set; } = 2;
		#endregion

		#region Constructors
		public ApplicationReviewViewModel() : base()
		{

		}

		public ApplicationReviewViewModel(GrantApplication grantApplication, User user) : base(grantApplication)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.EnableAttachments = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;

			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();

			this.GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();

			this.FullName = grantApplication.GrantOpening.GrantStream.FullName;

			if (this.ProgramType == ProgramTypes.EmployerGrant)
			{
				var trianingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
				if (trianingProgram != null)
				{
					this.TrainingProgram = new OverviewTrainingProgram(trianingProgram);
				}

				var trainingProvider = grantApplication.TrainingProviders.FirstOrDefault() ?? trianingProgram?.TrainingProviders.FirstOrDefault();
				if (trainingProvider != null)
				{
					this.TrainingProvider = new OverviewTrainingProvider(trainingProvider);
				}

				if (grantApplication.TrainingCost != null)
				{
					this.TrainingCost = new OverviewTrainingCost(grantApplication.TrainingCost);
					if (grantApplication.TrainingCost.EligibleCosts != null)
					{
						var hasOfferBeenIssued = grantApplication.HasOfferBeenIssued();
						this.TrainingCost.EstimatedCosts = grantApplication.TrainingCost.EligibleCosts.Where(ec => hasOfferBeenIssued || !ec.AddedByAssessor)
							.OrderBy(ec => ec.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption)
							.Select(x => new EstimatedCostViewModel(x, grantApplication)).ToArray();
						this.TrainingCost.TotalCost = grantApplication.TrainingCost.TotalEstimatedCost;
						this.TrainingCost.TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
						this.TrainingCost.TotalEmployer = this.TrainingCost.TotalCost - this.TrainingCost.TotalRequest;
						this.TrainingCost.ESSAveragePerParticipant = this.TrainingCost.EstimatedCosts.Where(x => x.ServiceType == (int?)ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
					}
				}
			}

			this.ApplicantContact = new OverviewApplicantContact(grantApplication, user);
			this.Applicant = new OverviewApplicantViewModel(grantApplication);

			if (this.ProgramType == ProgramTypes.EmployerGrant)
			{
				if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
				{
					this.NextStepUrl = string.Format("/Ext/Application/Review/Delivery/Partner/View/{0}", grantApplication.Id);
				}
				else
				{
					this.NextStepUrl = string.Format("/Ext/Application/Review/Applicant/Declaration/View/{0}", grantApplication.Id);
				}
			}
			else if (this.ProgramType == ProgramTypes.WDAService)
			{
				this.NextStepUrl = string.Format("/Ext/Application/Review/Program/View/{0}", grantApplication.Id);
			}

			if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
			{
				this.Steps += 1;
			}

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				this.Steps += 2;

				this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();

				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
				{
					this.Steps += 1;
				}
			}
		}
		#endregion
	}
}
