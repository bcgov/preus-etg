using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimAssessmentDetailsViewModel : BaseViewModel
	{
		#region Properties
		#region Claim
		public int Version { get; set; }
		public string RowVersion { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public bool IsFinalClaim { get; set; }

		public double ReimbursementRate { get; set; }
		public int MaximumParticipants { get; set; }
		public decimal AgreedMaxCost { get; set; }
		public decimal AgreedMaxCommittment { get; set; }

		public decimal TotalClaimReimbursement { get; set; }
		public decimal TotalAssessedReimbursement { get; set; }
		public IEnumerable<ClaimEligibleCostViewModel> EligibleCosts { get; set; } = new List<ClaimEligibleCostViewModel>();

		public int ParticipantsWithCostsAssigned { get; set; }
		public int ParticipantsReported { get; set; }
		#endregion
		#endregion

		#region Constructors
		public ClaimAssessmentDetailsViewModel()
		{
		}

		public ClaimAssessmentDetailsViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			this.Id = claim.Id;
			this.Version = claim.ClaimVersion;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.ClaimType = claim.ClaimTypeId;
			this.IsFinalClaim = claim.IsFinalClaim;
			this.MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			this.AgreedMaxCost = claim.GrantApplication.TrainingCost.TotalAgreedMaxCost;
			this.AgreedMaxCommittment = claim.GrantApplication.TrainingCost.AgreedCommitment;
			this.TotalClaimReimbursement = claim.TotalClaimReimbursement;
			this.TotalAssessedReimbursement = claim.TotalAssessedReimbursement;
			this.EligibleCosts = claim.EligibleCosts.Select(ec => new ClaimEligibleCostViewModel(ec)).ToArray();

			this.ParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			this.ParticipantsReported = claim.GrantApplication.ParticipantForms.Count();

			this.ProgramType = claim.GrantApplication.GetProgramType();
			this.ReimbursementRate = claim.GrantApplication.ReimbursementRate;
		}
		#endregion
	}
}