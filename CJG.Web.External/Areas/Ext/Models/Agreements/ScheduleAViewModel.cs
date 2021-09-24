using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class ScheduleAViewModel : GrantAgreementDocumentViewModel
	{
		#region Properties
		public string AgreementNumber { get; set; }
		public string ApplicantName { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public DateTime ParticipantReportingDueDate { get; set; }
		public DateTime ReimbursementClaimDueDate { get; set; }
		public int NumberOfParticipant { get; set; }

		public DeliveryDateViewModel DeliveryDate { get; set; }

		public IEnumerable<AgreementTrainingProgramViewModel> TrainingPrograms { get; set; }

		public IEnumerable<AgreementTrainingProviderViewModel> TrainingProviders { get; set; }

		public string CourseTitle { get; set; }
		public string TrainingProviderName { get; set; }
		public int TrainingProviderId { get; set; }

		public string RequestTrainingProviderRowVersion { get; set; }
		public string RequestTrainingProviderName { get; set; }
		public int RequestTrainingProviderId { get; set; }

		public IEnumerable<ChangeRequest.ChangeRequestSkillTrainingViewModel> SkillTrainings { get; set; } = new List<ChangeRequest.ChangeRequestSkillTrainingViewModel>();
		public IEnumerable<ChangeRequest.ChangeRequestESSViewModel> ESSComponents { get; set; } = new List<ChangeRequest.ChangeRequestESSViewModel>();

		public IEnumerable<EligibleCostModel> EligibleCosts { get; set; }
		public decimal TotalAgreedEmployerContribution { get; set; }
		public decimal TotalAgreedCost { get; set; }
		public decimal TotalAgreedMaxReimbursement { get; set; }
		public decimal ESSAveragePerParticipant { get; set; }
		public bool ShowAgreedCosts { get; set; }
		public bool ShowContributionColumn { get; set; }
		public DateTime GrantOpeningTrainingPeriodStartDate { get; set; }
		public DateTime GrantOpeningTrainingPeriodEndDate { get; set; }
		public string FiscalYearDisplay { get; set; }
		public DateTime ClaimDeadline { get; set; } = DateTime.Now;

		#endregion

		#region Constructors
		public ScheduleAViewModel()
		{

		}

		public ScheduleAViewModel(GrantApplication grantApplication) : base(grantApplication, ga => ga.ScheduleA)
		{
			this.Confirmation = grantApplication.GrantAgreement.ScheduleAConfirmed;
			this.AgreementNumber = grantApplication.FileNumber;
			this.ApplicantName = grantApplication.Organization?.LegalName;

			this.ProgramType = grantApplication.GetProgramType();
			this.ShowAgreedCosts = grantApplication.ApplicationStateInternal.ShowAgreedCosts();
			this.NumberOfParticipant = this.ShowAgreedCosts ? grantApplication.TrainingCost.AgreedParticipants : grantApplication.TrainingCost.EstimatedParticipants;
			this.DeliveryDate = new DeliveryDateViewModel(grantApplication);

			if (ProgramType == ProgramTypes.EmployerGrant)
			{
				this.CourseTitle = grantApplication.TrainingPrograms.FirstOrDefault().CourseTitle;

				this.TrainingProviderName = grantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.Name;
				this.TrainingProviderId = grantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.Id;

				var requestTrainingProvider = grantApplication.TrainingPrograms.FirstOrDefault().RequestedTrainingProvider;
				if (requestTrainingProvider != null)
				{
					this.RequestTrainingProviderName = requestTrainingProvider.Name;
					this.RequestTrainingProviderId = requestTrainingProvider.Id;
					this.RequestTrainingProviderRowVersion = Convert.ToBase64String(requestTrainingProvider.RowVersion);
				}
			}
			else
			{
				this.SkillTrainings = grantApplication.TrainingPrograms.Where(x => x.EligibleCostBreakdown.IsEligible).Select(x => new ChangeRequest.ChangeRequestSkillTrainingViewModel(x)).ToArray();
				this.ESSComponents = grantApplication.TrainingCost.EligibleCosts
					.Where(x => x.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports)
					.OrderBy(ec => ec.EligibleExpenseType.RowSequence)
					.Select(x => new ChangeRequest.ChangeRequestESSViewModel(x)).ToArray();
				this.ESSAveragePerParticipant = grantApplication.TrainingCost.EligibleCosts.Where(x => x.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
			}

			this.TrainingPrograms = grantApplication.TrainingPrograms.Where(tp => tp.EligibleCostBreakdown?.IsEligible ?? true).Select(tp => new AgreementTrainingProgramViewModel(tp)).ToArray();
			this.TrainingProviders = grantApplication.TrainingProviders.Where(tp => tp.ApprovedTrainingProvider.IsValidated()).Select(tp => tp.ApprovedTrainingProvider).Distinct().Select(tp => new AgreementTrainingProviderViewModel(tp)).ToArray();

			this.TotalAgreedCost = grantApplication.TrainingCost.TotalAgreedMaxCost;
			this.TotalAgreedEmployerContribution = grantApplication.CalculateAgreedEmployerContribution();
			this.TotalAgreedMaxReimbursement = grantApplication.CalculateAgreedMaxReimbursement();

			this.EligibleCosts = grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxReimbursement > 0).Select(x => new EligibleCostModel(x)).ToArray();
			if (ProgramType == ProgramTypes.WDAService)
			{
				this.EligibleCosts = grantApplication.TrainingCost.EligibleCosts.Select(x => new EligibleCostModel(x)).ToArray();
			}
			this.ShowContributionColumn = grantApplication.ReimbursementRate != 1;
			this.GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			this.FiscalYearDisplay = $"{grantApplication.GrantOpening.TrainingPeriod.FiscalYear.StartDate.ToLocalTime().ToString("yyyy-MM-dd")} to {grantApplication.GrantOpening.TrainingPeriod.FiscalYear.EndDate.ToLocalTime().ToString("yyyy-MM-dd")}";
			this.ClaimDeadline = grantApplication.GrantAgreement.GetClaimSubmissionDeadline();
		}
		#endregion
	}
}