using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class ClaimModel
	{
		#region Properties
		public int Id { get; set; }
		public int Version { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public bool IsEditable { get; set; }
		public bool IsFinalClaim { get; set; }
		public decimal TotalClaimReimbursement { get; set; }
		public decimal TotalAssessedReimbursement { get; set; }
		public List<ClaimEligibleCostModel> EligibleCosts { get; set; } = new List<ClaimEligibleCostModel>();
		public List<ParticipantFormModel> Participants { get; set; } = new List<ParticipantFormModel>();
		public int CountParticipantsWithCostsAssigned { get; set; }
		public int CountParticipants { get; set; }
		public int MaximumParticipants { get; set; }
		public DateTime? DateAssessed { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public string ClaimAssessmentNotes { get; set; }
		public string ReimbursementAssessmentNotes { get; set; }
		public string EligibilityAssessmentNotes { get; set; }
		public string RowVersion { get; set; }

		public string ClaimStateText { get; set; }
		public ClaimState ClaimState { get; set; }
		#endregion

		#region Constructors
		public ClaimModel()
		{

		}

		public ClaimModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			this.Id = claim.Id;
			this.Version = claim.ClaimVersion;
			this.ClaimType = claim.ClaimTypeId;
			this.ProgramType = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.EligibleCosts = claim.EligibleCosts.OrderBy(ec => ec.EligibleExpenseType.RowSequence).Select(x => new ClaimEligibleCostModel(x)).ToList();
			this.IsEditable = false;
			this.TotalClaimReimbursement = this.EligibleCosts.Sum(x => x.TotalClaimedReimbursement);
			this.TotalAssessedReimbursement = this.EligibleCosts.Sum(x => x.TotalAssessedReimbursement);

			this.CountParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			this.CountParticipants = claim.GrantApplication.ParticipantForms.Count;
			this.MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			this.DateAssessed = claim.DateAssessed?.ToLocalMorning();
			this.ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;
			this.ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			this.ClaimState = claim.ClaimState;
			this.ClaimStateText = claim.ClaimState.GetDescription();
			this.DateSubmitted = claim.DateSubmitted;
		}
		#endregion
	}
}