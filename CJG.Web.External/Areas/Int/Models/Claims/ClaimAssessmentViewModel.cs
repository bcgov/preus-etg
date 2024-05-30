using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
    public class ClaimAssessmentViewModel : BaseViewModel
	{
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

		public ClaimAssessmentViewModel()
		{
		}

		public ClaimAssessmentViewModel(Claim claim, IPrincipal user, Func<string, string> GetWorkflowUrl)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			Id = claim.Id;
			Version = claim.ClaimVersion;
			RowVersion = Convert.ToBase64String(claim.RowVersion);
			ClaimState = claim.ClaimState;
			ClaimStatus = claim.ClaimState.GetDescription();
			ClaimNumber = claim.ClaimNumber;
			ClaimType = claim.ClaimTypeId;
			IsFinalClaim = claim.IsFinalClaim;
			DateSubmitted = claim.DateSubmitted.HasValue ? claim.DateSubmitted.Value.ToLocalTime() : (DateTime?)null;
			DateAssessed = claim.DateAssessed.HasValue ? claim.DateAssessed.Value.ToLocalTime() : (DateTime?)null;
			MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			AgreedMaxCommittment = claim.GrantApplication.TrainingCost.AgreedCommitment;
			TotalClaimReimbursement = claim.TotalClaimReimbursement;
			TotalAssessedReimbursement = claim.TotalAssessedReimbursement;

			ParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			ParticipantsReported = claim.GrantApplication.ParticipantForms.Count();

			ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
			ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;

			GrantApplicationRowVersion = Convert.ToBase64String(claim.GrantApplication.RowVersion);
			FileNumber = claim.GrantApplication.FileNumber;
			FileName = claim.GrantApplication.GetFileName();
			StartDate = claim.GrantApplication.StartDate.ToLocalTime();
			EndDate = claim.GrantApplication.EndDate.ToLocalTime();
			GrantAgreementStartDate = claim.GrantApplication.GrantAgreement.StartDate.ToLocalTime();
			ReimbursementRate = claim.GrantApplication.ReimbursementRate;
			ApplicationStateExternal = claim.GrantApplication.ApplicationStateExternal;
			ApplicationInternalStatus = ApplicationStateExternal.GetDescription();
			AssessorId = claim.GrantApplication.AssessorId;
			Assessor = claim.GrantApplication.AssessorId == null ? null : new InternalUserViewModel(claim.GrantApplication.Assessor);

			ProgramType = claim.GrantApplication.GetProgramType();
			GrantProgram = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			GrantStream = claim.GrantApplication.GrantOpening.GrantStream.Name;
			GrantOpeningState = claim.GrantApplication.GrantOpening.State;
			TrainingPeriodStartDate = claim.GrantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			TrainingPeriodEndDate = claim.GrantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();

			ApplicantName = $"{claim.GrantApplication.ApplicantFirstName} {claim.GrantApplication.ApplicantLastName}";
			OrganizationLegalName = claim.GrantApplication.OrganizationLegalName;

			CanEdit = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim);
			CanUnlock = user.HasPrivilege(Privilege.AM4);
			CanReassign = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ReassignAssessor);
			HasPriorApprovedClaim = claim.HasPriorApprovedClaim();

			switch (ProgramType)
			{
				case (ProgramTypes.EmployerGrant):
					TrainingProvider = claim.GrantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.Name;
					DeliveryPartner = claim.GrantApplication.DeliveryPartner?.Caption;
					Participants = claim.GrantApplication.ParticipantForms.Select(p => new ParticipantViewModel(p)).ToArray();
					break;
				case (ProgramTypes.WDAService):
					Participants = claim.ParticipantForms.Select(pf => new ParticipantViewModel(pf)).ToArray();
					break;
			}

			AmountPaidOrOwing = claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim ? claim.AmountPaidOrOwing() : claim.GrantApplication.AmountPaidOrOwing();

			Workflow = new WorkflowViewModel(claim, user, GetWorkflowUrl);
		}
	}
}