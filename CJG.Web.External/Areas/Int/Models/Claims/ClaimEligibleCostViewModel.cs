using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimEligibleCostViewModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public bool AddedByAssessor { get; set; }
		public IEnumerable<ClaimBreakdownCostViewModel> Breakdowns { get; set; }
		public IEnumerable<ParticipantCostViewModel> ParticipantCosts { get; set; }

		#region Expense Type
		public int EligibleExpenseTypeId { get; set; }
		public ExpenseTypes ExpenseType { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public ServiceTypes? ServiceType { get; set; }
		public double ReimbursementRate { get; set; }
		public double OverrideRate { get; set; }
		#endregion

		#region Original Source
		public int? SourceId { get; set; }
		public decimal PreviousAssessedCost { get; set; }
		#endregion

		#region Eligible Cost Agreed Values
		public int? EligibleCostId { get; set; }
		public int AgreedMaxParticipants { get; set; }
		public decimal AgreedMaxCost { get; set; }
		public decimal AgreedMaxParticipantCost { get; set; }
		public decimal AgreedMaxReimbursement { get; set; }
		public decimal AgreedEmployerContribution { get; set; }
		public decimal AgreedMaxParticipantReimbursement { get; set; }
		public decimal AgreedParticipantEmployerContribution { get; set; }
		#endregion

		#region Claimed Values
		public int ClaimParticipants { get; set; }
		public decimal ClaimCost { get; set; }
		public decimal ClaimReimbursementCost { get; set; }
		public decimal ClaimEmployerContribution { get; set; }
		public decimal ClaimMaxParticipantCost { get; set; }
		public decimal ClaimMaxParticipantReimbursementCost { get; set; }
		public decimal ClaimParticipantEmployerContribution { get; set; }

		public decimal TotalClaimParticipantReimbursement { get; set; }
		#endregion

		#region Assessed Values
		public int AssessedParticipants { get; set; }
		public decimal AssessedCost { get; set; }
		public decimal AssessedReimbursementCost { get; set; }
		public decimal AssessedEmployerContribution { get; set; }
		public decimal AssessedMaxParticipantCost { get; set; }
		public decimal AssessedMaxParticipantReimbursementCost { get; set; }
		public decimal AssessedParticipantEmployerContribution { get; set; }

		public decimal TotalAssessedParticipantReimbursement { get; set; }

		public decimal PreviouslyAssessedReimbursement { get; set; }
		public decimal PreviousAssessedParticipantReimbusement { get; set; }
		#endregion

		#region Assessed to Date
		public decimal AssessedToDate { get; set; }
		public decimal AssessedToDateParticipantCost { get; set; }
		public decimal ReimbursedToDate { get; set; }
		public decimal EmployerContributionToDate { get; set; }

		public decimal RemainingToBeClaimed { get; set; }
		public decimal RemainingToBeClaimedParticipantCost { get; set; }
		public decimal RemainingToBeClaimedMaxReimbursement { get; set; }
		public decimal RemainingToBeClaimedEmployerContribution { get; set; }

		public decimal PaidToDate { get; set; }
		#endregion

		public ClaimEligibleCostViewModel() { }

		public ClaimEligibleCostViewModel(ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost == null)
				throw new ArgumentNullException(nameof(claimEligibleCost));

			Id = claimEligibleCost.Id;
			RowVersion = Convert.ToBase64String(claimEligibleCost.RowVersion);
			AddedByAssessor = claimEligibleCost.AddedByAssessor;
			Breakdowns = claimEligibleCost.Breakdowns.Select(b => new ClaimBreakdownCostViewModel(b)).ToArray();
			ParticipantCosts = claimEligibleCost.ParticipantCosts.Select(pc => new ParticipantCostViewModel(pc)).ToArray();

			EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseTypeId;
			ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseTypeId;
			Caption = claimEligibleCost.EligibleExpenseType.Caption;
			Description = claimEligibleCost.EligibleExpenseType.Description;
			ServiceType = claimEligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;
			ReimbursementRate = claimEligibleCost.EligibleExpenseType?.Rate ?? claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			OverrideRate = ReimbursementRate;

			SourceId = claimEligibleCost.SourceId;
			PreviousAssessedCost = claimEligibleCost.Source?.AssessedCost ?? 0;

			var agreedMaxCost = claimEligibleCost.EligibleCost?.AgreedMaxCost ?? claimEligibleCost.Source?.AssessedCost ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.TotalAgreedMaxCost;
			EligibleCostId = claimEligibleCost.EligibleCostId;
			AgreedMaxParticipants = claimEligibleCost.EligibleCost?.AgreedMaxParticipants ?? claimEligibleCost.Source?.AssessedParticipants ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants;
			AgreedMaxCost = agreedMaxCost;
			AgreedMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Source?.AssessedMaxParticipantCost ?? TrainingCostExtensions.CalculatePerParticipantCost(agreedMaxCost, AgreedMaxParticipants);
			AgreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment;
			AgreedEmployerContribution = claimEligibleCost.EligibleCost?.AgreedEmployerContribution ?? agreedMaxCost - AgreedMaxReimbursement;
			AgreedMaxParticipantReimbursement = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantReimbursement() ?? claimEligibleCost.Source?.AssessedMaxParticipantReimbursementCost ?? TrainingCostExtensions.CalculateTruncatedReimbursementAmount(AgreedMaxParticipantCost, ReimbursementRate);
			AgreedParticipantEmployerContribution = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantEmployerContribution() ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? AgreedMaxParticipantCost - AgreedMaxParticipantReimbursement;

			ClaimParticipants = claimEligibleCost.ClaimParticipants;
			ClaimCost = claimEligibleCost.ClaimCost;
			ClaimReimbursementCost = claimEligibleCost.ClaimReimbursementCost;
			ClaimEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			ClaimMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
			ClaimMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
			ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			TotalClaimParticipantReimbursement = ParticipantCosts.Sum(pc => pc.ClaimReimbursement);

			AssessedParticipants = claimEligibleCost.AssessedParticipants;
			AssessedCost = claimEligibleCost.AssessedCost;
			AssessedReimbursementCost = claimEligibleCost.AssessedReimbursementCost;
			AssessedEmployerContribution = claimEligibleCost.AssessedCost - claimEligibleCost.AssessedReimbursementCost;
			AssessedMaxParticipantCost = claimEligibleCost.AssessedMaxParticipantCost;
			AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
			AssessedParticipantEmployerContribution = claimEligibleCost.AssessedParticipantEmployerContribution;
			TotalAssessedParticipantReimbursement = ParticipantCosts.Sum(pc => pc.AssessedReimbursement);

			PreviouslyAssessedReimbursement = claimEligibleCost.Source?.AssessedReimbursementCost ?? 0;
			PreviousAssessedParticipantReimbusement = claimEligibleCost.Source?.ParticipantCosts.Sum(pc => pc.AssessedReimbursement) ?? 0;

			AssessedToDate = claimEligibleCost.GetTotalAssessed();
			AssessedToDateParticipantCost = TrainingCostExtensions.CalculatePerParticipantCost(AssessedToDate, ClaimParticipants);
			ReimbursedToDate = claimEligibleCost.GetTotalReimbursement();
			EmployerContributionToDate = ReimbursedToDate - ReimbursedToDate;

			RemainingToBeClaimed = agreedMaxCost - ReimbursedToDate;
			RemainingToBeClaimedParticipantCost = TrainingCostExtensions.CalculatePerParticipantCost(RemainingToBeClaimed, ClaimParticipants); ;
			RemainingToBeClaimedMaxReimbursement = TrainingCostExtensions.CalculateRoundedReimbursementAmount(RemainingToBeClaimed, ReimbursementRate);
			RemainingToBeClaimedEmployerContribution = RemainingToBeClaimed - RemainingToBeClaimedMaxReimbursement;

			PaidToDate = claimEligibleCost.AmountPaidOrOwing();
		}
	}
}