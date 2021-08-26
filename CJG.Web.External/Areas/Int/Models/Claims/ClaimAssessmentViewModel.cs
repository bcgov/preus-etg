using CJG.Core.Entities;
using System.Security.Principal;
using CJG.Application.Services;
using System;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Helpers;
using System.Collections.Generic;
using System.Linq;
using CJG.Infrastructure.Identity;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimAssessmentViewModel : BaseViewModel
	{
		#region Properties
		#region Claim
		public int Version { get; set; }
		public string RowVersion { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public ClaimState ClaimState { get; set; }
		public string ClaimStatus { get; set; }
		public string ClaimNumber { get; set; }
		public bool IsFinalClaim { get; set; }
		public DateTime? DateAssessed { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public int MaximumParticipants { get; set; }
		public decimal AgreedMaxCommittment { get; set; }
		public decimal AmountPaidOrOwing { get; set; }
		public decimal TotalClaimReimbursement { get; set; }
		public decimal TotalAssessedReimbursement { get; set; }
		public IEnumerable<ParticipantViewModel> Participants { get; set; }

		public int ParticipantsWithCostsAssigned { get; set; }
		public int ParticipantsReported { get; set; }
		#endregion

		#region Application
		public string GrantApplicationRowVersion { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public DateTime? GrantAgreementStartDate { get; set; }
		public double ReimbursementRate { get; set; }

		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public string ApplicationInternalStatus { get; set; }


		public int? AssessorId { get; set; }
		public InternalUserViewModel Assessor { get; set; }

		public string TrainingProvider { get; set; }
		public string DeliveryPartner { get; set; }
		#endregion

		#region Program
		public ProgramTypes ProgramType { get; set; }
		public string GrantProgram { get; set; }
		public string GrantStream { get; set; }

		public GrantOpeningStates GrantOpeningState { get; set; }
		public DateTime? GrantOpeningOpeningDate { get; set; }

		public DateTime TrainingPeriodStartDate { get; set; }
		public DateTime TrainingPeriodEndDate { get; set; }
		#endregion

		#region Applicant
		public string ApplicantName { get; set; }
		public string OrganizationLegalName { get; set; }
		#endregion

		public string ClaimAssessmentNotes { get; set; }
		public string ReimbursementAssessmentNotes { get; set; }
		public string EligibilityAssessmentNotes { get; set; }

		public bool CanEdit { get; set; }
		public bool CanUnlock { get; set; }
		public bool CanReassign { get; set; }
		public bool HasPriorApprovedClaim { get; set; }

		public WorkflowViewModel Workflow { get; set; }
		#endregion

		#region Constructors
		public ClaimAssessmentViewModel()
		{
		}

		public ClaimAssessmentViewModel(Claim claim, IPrincipal user, Func<string, string> GetWorkflowUrl)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = claim.Id;
			this.Version = claim.ClaimVersion;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.ClaimState = claim.ClaimState;
			this.ClaimStatus = claim.ClaimState.GetDescription();
			this.ClaimNumber = claim.ClaimNumber;
			this.ClaimType = claim.ClaimTypeId;
			this.IsFinalClaim = claim.IsFinalClaim;
			this.DateSubmitted = claim.DateSubmitted.HasValue ? claim.DateSubmitted.Value.ToLocalTime() : (DateTime?)null;
			this.DateAssessed = claim.DateAssessed.HasValue ? claim.DateAssessed.Value.ToLocalTime() : (DateTime?)null;
			this.MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			this.AgreedMaxCommittment = claim.GrantApplication.TrainingCost.AgreedCommitment;
			this.TotalClaimReimbursement = claim.TotalClaimReimbursement;
			this.TotalAssessedReimbursement = claim.TotalAssessedReimbursement;

			this.ParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			this.ParticipantsReported = claim.GrantApplication.ParticipantForms.Count();

			this.ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
			this.ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			this.EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;

			this.GrantApplicationRowVersion = Convert.ToBase64String(claim.GrantApplication.RowVersion);
			this.FileNumber = claim.GrantApplication.FileNumber;
			this.FileName = claim.GrantApplication.GetFileName();
			this.StartDate = claim.GrantApplication.StartDate.ToLocalTime();
			this.EndDate = claim.GrantApplication.EndDate.ToLocalTime();
			this.GrantAgreementStartDate = claim.GrantApplication.GrantAgreement.StartDate.ToLocalTime();
			this.ReimbursementRate = claim.GrantApplication.ReimbursementRate;
			this.ApplicationStateExternal = claim.GrantApplication.ApplicationStateExternal;
			this.ApplicationInternalStatus = this.ApplicationStateExternal.GetDescription();
			this.AssessorId = claim.GrantApplication.AssessorId;
			this.Assessor = claim.GrantApplication.AssessorId == null ? null : new InternalUserViewModel(claim.GrantApplication.Assessor);

			this.ProgramType = claim.GrantApplication.GetProgramType();
			this.GrantProgram = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.GrantStream = claim.GrantApplication.GrantOpening.GrantStream.Name;
			this.GrantOpeningState = claim.GrantApplication.GrantOpening.State;
			this.TrainingPeriodStartDate = claim.GrantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.TrainingPeriodEndDate = claim.GrantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();

			this.ApplicantName = $"{claim.GrantApplication.ApplicantFirstName} {claim.GrantApplication.ApplicantLastName}";
			this.OrganizationLegalName = claim.GrantApplication.OrganizationLegalName;

			this.CanEdit = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim);
			this.CanUnlock = user.HasPrivilege(Privilege.AM4);
			this.CanReassign = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ReassignAssessor);
			this.HasPriorApprovedClaim = claim.HasPriorApprovedClaim();

			switch (this.ProgramType)
			{
				case (ProgramTypes.EmployerGrant):
					this.TrainingProvider = claim.GrantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.Name;
					this.DeliveryPartner = claim.GrantApplication.DeliveryPartner?.Caption;
					this.Participants = claim.GrantApplication.ParticipantForms.Select(p => new ParticipantViewModel(p)).ToArray();
					break;
				case (ProgramTypes.WDAService):
					this.Participants = claim.ParticipantForms.Select(pf => new ParticipantViewModel(pf)).ToArray();
					break;
			}

			this.AmountPaidOrOwing = claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim ? claim.AmountPaidOrOwing() : claim.GrantApplication.AmountPaidOrOwing();

			this.Workflow = new WorkflowViewModel(claim, user, GetWorkflowUrl);
		}
		#endregion
	}
}