using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Overview;
using CJG.Web.External.Areas.Int.Models.GrantStreams;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationReviewViewModel : BaseApplicationViewModel
	{
		public OverviewApplicantContact ApplicantContact { get; set; }
		public OverviewApplicantViewModel Applicant { get; set; }
		public List<GrantStreamQuestionViewModel> StreamEligibilityQuestions { get; set; }

		public string NextStepUrl { get; set; }
		public int Steps { get; set; } = 2;

		public ApplicationReviewViewModel()
		{
		}

		public ApplicationReviewViewModel(GrantApplication grantApplication, User user) : base(grantApplication)
		{
			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			EnableAttachments = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;

			StartDate = grantApplication.StartDate.ToLocalTime();
			EndDate = grantApplication.EndDate.ToLocalTime();

			GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();

			FullName = grantApplication.GrantOpening.GrantStream.FullName;

			ApplicantContact = new OverviewApplicantContact(grantApplication, user);
			Applicant = new OverviewApplicantViewModel(grantApplication);

			var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

			if (trainingProgram != null)
				TrainingProgram = new OverviewTrainingProgram(trainingProgram);

			var trainingProvider = grantApplication.TrainingProviders.FirstOrDefault() ?? trainingProgram?.TrainingProviders.FirstOrDefault();
			if (trainingProvider != null)
				TrainingProvider = new OverviewTrainingProvider(trainingProvider);

			if (grantApplication.TrainingCost != null)
			{
				TrainingCost = new OverviewTrainingCost(grantApplication.TrainingCost);

				if (grantApplication.TrainingCost.EligibleCosts != null)
				{
					var hasOfferBeenIssued = grantApplication.HasOfferBeenIssued();

					TrainingCost.EstimatedCosts = grantApplication.TrainingCost.EligibleCosts.Where(ec => hasOfferBeenIssued || !ec.AddedByAssessor)
						.OrderBy(ec => ec.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption)
						.Select(x => new EstimatedCostViewModel(x, grantApplication)).ToArray();

					TrainingCost.TotalCost = grantApplication.TrainingCost.TotalEstimatedCost;
					TrainingCost.TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
					TrainingCost.TotalEmployer = TrainingCost.TotalCost - TrainingCost.TotalRequest;
					TrainingCost.ESSAveragePerParticipant = TrainingCost.EstimatedCosts.Where(x => x.ServiceType == (int?) ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
				}
			}

			NextStepUrl = grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner
				? $"/Ext/Application/Review/Delivery/Partner/View/{grantApplication.Id}"
				: $"/Ext/Application/Review/Applicant/Declaration/View/{grantApplication.Id}";

			if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
				Steps += 1;
		}
	}
}
