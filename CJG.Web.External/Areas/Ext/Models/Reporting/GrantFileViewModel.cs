using System;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.Claims;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Reporting
{
    public class GrantFileViewModel : BaseViewModel
	{
		public int ClaimId { get; set; }
		public int ClaimVersion { get; set; }
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		public ClaimAssessmentOutcomeViewModel ClaimAssessmentOutcome { get; set; }
		public SidebarViewModel SidebarViewModel { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public bool AllowParticipantReport { get; set; }
		public DateTime ParticipantDueDate { get; set; }
		public int MaxParticipants { get; set; }
		public int ParticipantCount { get; set; }
		public int ParticipantsWithCostCount { get; set; }
		public bool HasClaim { get; set; }
		public ClaimState? CurrentClaimState { get; set; }
		public bool AllowClaimReport { get; set; }
		public DateTime ClaimDueDate { get; set; }
		public bool AllowReviewAndSubmit { get; set; }
		public bool ReportingPeriodIsOpen { get; set; }
		public DateTime CompletionDueDate { get; set; }
		public CompletionReportStatusViewModel CompletionReport { get; set; } = new CompletionReportStatusViewModel();
		public bool AllowReportCompletion { get; set; }
		public bool EnableSubmit { get; set; }
		public bool IsFinalClaim { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public bool RequireAllParticipantsBeforeSubmission { get; set; }
		public bool HasRequiredAttachments { get; set; }

		public GrantFileViewModel()
		{
		}

		public GrantFileViewModel(GrantApplication grantApplication, IParticipantService participantService, ICompletionReportService completionReportService, IPrincipal user)
		{
			if (participantService == null) throw new ArgumentNullException(nameof(participantService));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			ClaimType = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
			ParticipantDueDate = grantApplication.GetParticipantReportingDueDate();
			MaxParticipants = grantApplication.TrainingCost.GetMaxParticipants();

            if (grantApplication.RequireAllParticipantsBeforeSubmission)
            {
				ParticipantCount = grantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim && pf.Approved.HasValue && pf.Approved.Value);
			}
            else
            {
				ParticipantCount = grantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim);
			}			

			RequireAllParticipantsBeforeSubmission = grantApplication.RequireAllParticipantsBeforeSubmission;

			var claim = grantApplication.GetCurrentClaim();
			if (ClaimType == ClaimTypes.SingleAmendableClaim)
			{
				ParticipantsWithCostCount = claim == null ? 0 : participantService.GetParticipantsWithClaimEligibleCostCount(claim.Id, claim.ClaimVersion);
			}

			if (claim != null)
			{
				HasClaim = true;
				ClaimId = claim.Id;
				ClaimVersion = claim.ClaimVersion;
				CurrentClaimState = claim.ClaimState;
				IsFinalClaim = claim.IsFinalClaim;
				HasRequiredAttachments = claim.Receipts.Any();
			}
			ClaimDueDate = grantApplication.StartDate.AddDays(30);
			CompletionDueDate = grantApplication.StartDate.AddDays(30);

			var completionReportStatus = completionReportService.GetCompletionReportStatus(Id);
			CompletionReport.CompletionReportStatus = (CompletionReportStatus)Enum.Parse(typeof(CompletionReportStatus), completionReportStatus.Replace(" ", string.Empty));
			AllowReportCompletion = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitCompletionReport)
				&& AppDateTime.UtcNow.ToUtcMidnight() >= grantApplication.EndDate
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.CompletionReporting;
			ClaimAssessmentOutcome = new ClaimAssessmentOutcomeViewModel(grantApplication);
		}
	}
}
