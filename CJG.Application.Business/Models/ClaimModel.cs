using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class ClaimModel
	{
		public int Id { get; set; }
		public int Version { get; set; }

		public ClaimTypes ClaimType { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public string ClaimStateText { get; set; }
		public ClaimState ClaimState { get; set; }
		public bool IsEditable { get; set; }
		public bool IsFinalClaim { get; set; }

		public decimal TotalClaimReimbursement { get; set; }
		public decimal TotalAssessedReimbursement { get; set; }
		public decimal TotalApprovedAmount { get; set; }

		public int CountParticipantsWithCostsAssigned { get; set; }
		public int CountParticipants { get; set; }
		public int CountAttended { get; set; }
		public int MaximumParticipants { get; set; }
		public bool AttendanceCompleted { get; set; }

		public DateTime? DateAssessed { get; set; }
		public DateTime? DateSubmitted { get; set; }

		public string ClaimAssessmentNotes { get; set; }
		public string ReimbursementAssessmentNotes { get; set; }
		public string EligibilityAssessmentNotes { get; set; }
		public string ApplicantNotes { get; set; }

		public string RowVersion { get; set; }

		public List<ClaimEligibleCostModel> EligibleCosts { get; set; } = new List<ClaimEligibleCostModel>();
		public List<ParticipantFormModel> Participants { get; set; } = new List<ParticipantFormModel>();
		public List<ParticipantFormModel> Attended { get; set; } = new List<ParticipantFormModel>();

		public ClaimModel() { }

		public ClaimModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			Id = claim.Id;
			Version = claim.ClaimVersion;
			ClaimType = claim.ClaimTypeId;
			ProgramType = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			EligibleCosts = claim.EligibleCosts.OrderBy(ec => ec.EligibleExpenseType.RowSequence).Select(x => new ClaimEligibleCostModel(x)).ToList();
			IsEditable = false;
			TotalClaimReimbursement = EligibleCosts.Sum(x => x.TotalClaimedReimbursement);
			TotalAssessedReimbursement = EligibleCosts.Sum(x => x.TotalAssessedReimbursement);
			TotalApprovedAmount = claim.GrantApplication.TrainingCost.AgreedCommitment;
			CountParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();

			if (claim.GrantApplication.RequireAllParticipantsBeforeSubmission)
			{
				Participants = claim.GrantApplication.ParticipantForms
					.Where(c => c.Approved.HasValue && c.Approved.Value)
					.Select(s => new ParticipantFormModel(s))
					.OrderBy(o => o.Name)
					.ToList();

				Attended = claim.GrantApplication.ParticipantForms
					.Where(c => c.Approved.HasValue && c.Approved.Value && c.Attended.HasValue && c.Attended.Value)
					.Select(s => new ParticipantFormModel(s))
					.ToList();
			}
			else
			{
				Participants = claim.GrantApplication.ParticipantForms
					.Select(s => new ParticipantFormModel(s))
					.OrderBy(o => o.Name)
					.ToList();

				Attended = Participants;
			}

			CountAttended = Attended.Count;
			CountParticipants = Participants.Count;

			MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			DateAssessed = claim.DateAssessed?.ToLocalMorning();
			ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
			RowVersion = Convert.ToBase64String(claim.RowVersion);
			EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;
			ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			ApplicantNotes = claim.ApplicantNotes;
			ClaimState = claim.ClaimState;
			ClaimStateText = claim.ClaimState.GetDescription();
			DateSubmitted = claim.DateSubmitted;

			//attendance is complete when all participants have the Attended property set
			AttendanceCompleted = Participants.All(a => a.Attended.HasValue) || !claim.GrantApplication.RequireAllParticipantsBeforeSubmission;
		}
	}
}