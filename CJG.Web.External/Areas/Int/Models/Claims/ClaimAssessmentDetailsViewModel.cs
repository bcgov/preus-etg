using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimAssessmentDetailsViewModel : BaseViewModel
	{
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

		public ClaimAssessmentDetailsViewModel()
		{
		}

		public ClaimAssessmentDetailsViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			Id = claim.Id;
			Version = claim.ClaimVersion;
			RowVersion = Convert.ToBase64String(claim.RowVersion);
			ClaimType = claim.ClaimTypeId;
			IsFinalClaim = claim.IsFinalClaim;
			MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			AgreedMaxCost = claim.GrantApplication.TrainingCost.TotalAgreedMaxCost;
			AgreedMaxCommittment = claim.GrantApplication.TrainingCost.AgreedCommitment;
			TotalClaimReimbursement = claim.TotalClaimReimbursement;
			TotalAssessedReimbursement = claim.TotalAssessedReimbursement;
			EligibleCosts = claim.EligibleCosts.Select(ec => new ClaimEligibleCostViewModel(ec)).ToArray();

			ParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			ParticipantsReported = claim.GrantApplication.ParticipantForms.Count();

			ProgramType = claim.GrantApplication.GetProgramType();
			ReimbursementRate = claim.GrantApplication.ReimbursementRate;
		}
	}
}