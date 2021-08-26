using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.Claims;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantFileViewModel : BaseViewModel
	{
		#region Properties
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
		public DateTime CompletionDueDate { get; set; }
		public CompletionReportStatusViewModel CompletionReport { get; set; } = new CompletionReportStatusViewModel();
		public bool AllowReportCompletion { get; set; }
		public bool EnableSubmit { get; set; }
		public bool IsFinalClaim { get; set; }
		#endregion

		#region Constructors
		public GrantFileViewModel()
		{
		}

		public GrantFileViewModel(GrantApplication grantApplication, IParticipantService participantService, ICompletionReportService completionReportService, IPrincipal user)
		{
			if (participantService == null) throw new ArgumentNullException(nameof(participantService));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.ClaimType = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;

			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication, true);
			this.ParticipantDueDate = grantApplication.StartDate.AddDays(-5);
			this.MaxParticipants = grantApplication.TrainingCost.GetMaxParticipants();
			this.ParticipantCount = grantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim);

			var claim = grantApplication.GetCurrentClaim();
			if (this.ClaimType == ClaimTypes.SingleAmendableClaim)
			{
				this.ParticipantsWithCostCount = claim == null ? 0 : participantService.GetParticipantsWithClaimEligibleCostCount(claim.Id, claim.ClaimVersion);
			}

			if (claim != null)
			{
				this.HasClaim = true;
				this.ClaimId = claim.Id;
				this.ClaimVersion = claim.ClaimVersion;
				this.CurrentClaimState = claim.ClaimState;
				this.IsFinalClaim = claim.IsFinalClaim;
			}
			this.ClaimDueDate = grantApplication.StartDate.AddDays(30);
			this.CompletionDueDate = grantApplication.StartDate.AddDays(30);

			var completionReportStatus = completionReportService.GetCompletionReportStatus(this.Id);
			this.CompletionReport.CompletionReportStatus = (CompletionReportStatus)Enum.Parse(typeof(CompletionReportStatus), completionReportStatus.Replace(" ", string.Empty));
			this.AllowReportCompletion = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitCompletionReport)
				&& AppDateTime.UtcNow.ToUtcMidnight() >= grantApplication.EndDate
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.CompletionReporting;
			this.ClaimAssessmentOutcome = new ClaimAssessmentOutcomeViewModel(grantApplication);
		}
		#endregion
	}
}
