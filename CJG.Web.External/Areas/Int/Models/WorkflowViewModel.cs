using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models
{
	public class WorkflowViewModel : BaseViewModel
	{
		#region Properties
		public ApplicationWorkflowViewModel ApplicationWorkflowViewModel { get; set; }
		public ClaimWorkflowViewModel ClaimWorkflowViewModel { get; set; }
		public IEnumerable<ApplicationActionButton> WorkflowButtons { get; set; } = new List<ApplicationActionButton>();
		#endregion

		#region Constructors
		public WorkflowViewModel()
		{
		}

		public WorkflowViewModel(GrantApplication grantApplication, IPrincipal user, Func<string, string> getWorkflowUrl)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.WorkflowButtons = GetApplicationWorkflowButtons(user, grantApplication, getWorkflowUrl);
			this.ApplicationWorkflowViewModel = new ApplicationWorkflowViewModel(grantApplication);
		}

		public WorkflowViewModel(Claim claim, IPrincipal user, Func<string, string> getWorkflowUrl)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));
			if (user == null) throw new ArgumentNullException(nameof(user));
			this.ClaimWorkflowViewModel = new ClaimWorkflowViewModel(claim);
			this.WorkflowButtons = GetClaimWorkflowButtons(claim, user, getWorkflowUrl);
		}

		#endregion

		#region Methods

		private IEnumerable<ApplicationActionButton> GetClaimWorkflowButtons(Claim claim, IPrincipal user, Func<string, string> getWorkflowUrl)
		{
			var buttons = new List<ApplicationActionButton>();

			if (claim.GrantApplication.GetCurrentClaim() == claim)
			{
				var grantApplication = claim.GrantApplication;
				var permittedTriggers = grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers();

				foreach (var trigger in permittedTriggers)
				{
					if (!user.CanPerformAction(grantApplication, trigger))
					{
						continue;
					}

					switch (trigger)
					{
						case ApplicationWorkflowTrigger.ApproveClaim:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Approve Claim",
								Value = "ApproveClaim",
								Url = getWorkflowUrl("ApproveClaim"),
								IsDisabled = claim.TotalAssessedReimbursement == 0 || String.IsNullOrEmpty(claim.ClaimAssessmentNotes),
								Information = claim.TotalAssessedReimbursement == 0
									? "Assessed reimbursement must be greater than or less than $0.00."
									: String.IsNullOrEmpty(claim.ClaimAssessmentNotes)
									? "Assessment explanation to applicant is required."
									: ""
							});
							break;

						case ApplicationWorkflowTrigger.AssessEligibility:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Assess Eligibility",
								Value = "AssessEligibility",
								Url = getWorkflowUrl("AssessEligibility")
							});
							break;

						case ApplicationWorkflowTrigger.AssessReimbursement:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Assess Reimbursement",
								Value = "AssessReimbursement",
								Url = getWorkflowUrl("AssessReimbursement"),
								IsDisabled = claim.TotalAssessedReimbursement == 0,
								Information = claim.TotalAssessedReimbursement == 0 ? "Assessed reimbursement must be greater than or less than $0.00." : ""
							});
							break;

						case ApplicationWorkflowTrigger.DenyClaim:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Deny Claim",
								Value = "DenyClaim",
								Url = getWorkflowUrl("DenyClaim"),
								IsDisabled = String.IsNullOrEmpty(claim.ClaimAssessmentNotes),
								Information = String.IsNullOrEmpty(claim.ClaimAssessmentNotes) ? "Assessment explanation to applicant is required." : ""
							});
							break;

						case ApplicationWorkflowTrigger.RemoveClaimFromAssessment:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Remove From Assessment",
								Value = "RemoveClaimFromAssessment",
								Url = getWorkflowUrl("RemoveClaimFromAssessment")
							});
							break;

						case ApplicationWorkflowTrigger.ReturnClaimToApplicant:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Return To Applicant",
								Value = "ReturnClaimToApplicant",
								Url = getWorkflowUrl("ReturnClaimToApplicant")
							});
							break;

						case ApplicationWorkflowTrigger.SelectClaimForAssessment:
							buttons.Add(new ApplicationActionButton
							{
								Caption = "Select For Assessment",
								Value = "SelectClaimForAssessment",
								Url = getWorkflowUrl("SelectClaimForAssessment")
							});
							break;
					}
				}
			}
			return buttons;
		}

		private IEnumerable<ApplicationActionButton> GetApplicationWorkflowButtons(IPrincipal user, GrantApplication grantApplication, Func<string, string> getWorkflowUrl)
		{
			var allowedTriggers = new[] {
				ApplicationWorkflowTrigger.SelectForAssessment,
				ApplicationWorkflowTrigger.BeginAssessment,
				ApplicationWorkflowTrigger.RemoveFromAssessment,
				ApplicationWorkflowTrigger.RecommendForApproval,
				ApplicationWorkflowTrigger.RecommendForDenial,
				ApplicationWorkflowTrigger.IssueOffer,
				ApplicationWorkflowTrigger.ReturnToAssessment,
				ApplicationWorkflowTrigger.DenyApplication,
				ApplicationWorkflowTrigger.WithdrawOffer,
				ApplicationWorkflowTrigger.CancelAgreementMinistry,
				ApplicationWorkflowTrigger.RecommendChangeForApproval,
				ApplicationWorkflowTrigger.RecommendChangeForDenial,
				ApplicationWorkflowTrigger.ApproveChangeRequest,
				ApplicationWorkflowTrigger.ReturnChangeToAssessment,
				ApplicationWorkflowTrigger.DenyChangeRequest,
				ApplicationWorkflowTrigger.CloseClaimReporting,
				ApplicationWorkflowTrigger.EnableClaimReporting,
				ApplicationWorkflowTrigger.EnableCompletionReporting,
				ApplicationWorkflowTrigger.Close,
				ApplicationWorkflowTrigger.AmendClaim
			};

			return grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers()
				.Where(x => allowedTriggers.Contains(x)
						&& (user.CanPerformAction(grantApplication, x) || x == ApplicationWorkflowTrigger.ReturnToAssessment))
				.Select(x => new ApplicationActionButton
				{
					Caption = GetButtonCaption(x),
					Value = x.ToString(),
					Url = getWorkflowUrl(x.ToString()),
					IsDisabled = IsDisabled(grantApplication, x, user)
				}).ToArray();
		}

		private string GetButtonCaption(ApplicationWorkflowTrigger trigger)
		{
			if (trigger.Equals(ApplicationWorkflowTrigger.CancelAgreementMinistry))
				return "Cancel Agreement";
			else if (trigger.Equals(ApplicationWorkflowTrigger.AmendClaim))
				return "Initiate Claim Amendment";
			else
				return trigger.GetDescription();
		}
		
		private bool IsDisabled(GrantApplication grantApplication, ApplicationWorkflowTrigger trigger, IPrincipal user)
		{
			switch (trigger)
			{
				case (ApplicationWorkflowTrigger.RecommendForApproval):
				case (ApplicationWorkflowTrigger.IssueOffer):
					return grantApplication.RequiresTrainingProviderValidation() || grantApplication.RequiresEligibleServiceComponents() || grantApplication.TrainingCost.AgreedCommitment == 0;
				case (ApplicationWorkflowTrigger.RecommendForDenial):
					return grantApplication.RequiresTrainingProviderValidation();
				case (ApplicationWorkflowTrigger.RecommendChangeForApproval):
				case (ApplicationWorkflowTrigger.ApproveChangeRequest):
					return !grantApplication.CanApproveChangeRequest() || grantApplication.RequiresTrainingProviderValidation() || grantApplication.RequiresEligibleServiceComponents();
				case (ApplicationWorkflowTrigger.RecommendChangeForDenial):
				case (ApplicationWorkflowTrigger.DenyChangeRequest):
					return !grantApplication.CanDenyChangeRequest() || grantApplication.RequiresTrainingProviderValidation();
				case (ApplicationWorkflowTrigger.ReturnToAssessment):
					return !user.CanPerformAction(grantApplication, trigger);
			}
			return false;
		}
		#endregion
	}
}
