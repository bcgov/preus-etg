using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimEligibleCostViewModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public ClaimEligibleCostViewModel() { }

		public ClaimEligibleCostViewModel(ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost == null) throw new ArgumentNullException(nameof(claimEligibleCost));

			this.Id = claimEligibleCost.Id;
			this.RowVersion = Convert.ToBase64String(claimEligibleCost.RowVersion);
			this.AddedByAssessor = claimEligibleCost.AddedByAssessor;
			this.Breakdowns = claimEligibleCost.Breakdowns.Select(b => new ClaimBreakdownCostViewModel(b)).ToArray();
			this.ParticipantCosts = claimEligibleCost.ParticipantCosts.Select(pc => new ParticipantCostViewModel(pc)).ToArray();

			this.EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseTypeId;
			this.ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseTypeId;
			this.Caption = claimEligibleCost.EligibleExpenseType.Caption;
			this.Description = claimEligibleCost.EligibleExpenseType.Description;
			this.ServiceType = claimEligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;
			this.ReimbursementRate = claimEligibleCost.EligibleExpenseType?.Rate ?? claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			this.OverrideRate = this.ReimbursementRate;

			this.SourceId = claimEligibleCost.SourceId;
			this.PreviousAssessedCost = claimEligibleCost.Source?.AssessedCost ?? 0;

			var agreedMaxCost = claimEligibleCost.EligibleCost?.AgreedMaxCost ?? claimEligibleCost.Source?.AssessedCost ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.TotalAgreedMaxCost;
			this.EligibleCostId = claimEligibleCost.EligibleCostId;
			this.AgreedMaxParticipants = claimEligibleCost.EligibleCost?.AgreedMaxParticipants ?? claimEligibleCost.Source?.AssessedParticipants ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants;
			this.AgreedMaxCost = agreedMaxCost;
			this.AgreedMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Source?.AssessedMaxParticipantCost ?? TrainingCostExtensions.CalculatePerParticipantCost(agreedMaxCost, this.AgreedMaxParticipants);
			this.AgreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment;
			this.AgreedEmployerContribution = claimEligibleCost.EligibleCost?.AgreedEmployerContribution ?? agreedMaxCost - this.AgreedMaxReimbursement;
			this.AgreedMaxParticipantReimbursement = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantReimbursement() ?? claimEligibleCost.Source?.AssessedMaxParticipantReimbursementCost ?? TrainingCostExtensions.CalculateTruncatedReimbursementAmount(this.AgreedMaxParticipantCost, this.ReimbursementRate);
			this.AgreedParticipantEmployerContribution = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantEmployerContribution() ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? this.AgreedMaxParticipantCost - this.AgreedMaxParticipantReimbursement;

			this.ClaimParticipants = claimEligibleCost.ClaimParticipants;
			this.ClaimCost = claimEligibleCost.ClaimCost;
			this.ClaimReimbursementCost = claimEligibleCost.ClaimReimbursementCost;
			this.ClaimEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			this.ClaimMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
			this.ClaimMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
			this.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			this.TotalClaimParticipantReimbursement = this.ParticipantCosts.Sum(pc => pc.ClaimReimbursement);

			this.AssessedParticipants = claimEligibleCost.AssessedParticipants;
			this.AssessedCost = claimEligibleCost.AssessedCost;
			this.AssessedReimbursementCost = claimEligibleCost.AssessedReimbursementCost;
			this.AssessedEmployerContribution = claimEligibleCost.AssessedCost - claimEligibleCost.AssessedReimbursementCost;
			this.AssessedMaxParticipantCost = claimEligibleCost.AssessedMaxParticipantCost;
			this.AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
			this.AssessedParticipantEmployerContribution = claimEligibleCost.AssessedParticipantEmployerContribution;
			this.TotalAssessedParticipantReimbursement = this.ParticipantCosts.Sum(pc => pc.AssessedReimbursement);

			this.PreviouslyAssessedReimbursement = claimEligibleCost.Source?.AssessedReimbursementCost ?? 0;
			this.PreviousAssessedParticipantReimbusement = claimEligibleCost.Source?.ParticipantCosts.Sum(pc => pc.AssessedReimbursement) ?? 0;

			this.AssessedToDate = claimEligibleCost.GetTotalAssessed();
			this.AssessedToDateParticipantCost = TrainingCostExtensions.CalculatePerParticipantCost(this.AssessedToDate, this.ClaimParticipants);
			this.ReimbursedToDate = claimEligibleCost.GetTotalReimbursement();
			this.EmployerContributionToDate = this.ReimbursedToDate - this.ReimbursedToDate;

			this.RemainingToBeClaimed = agreedMaxCost - this.ReimbursedToDate;
			this.RemainingToBeClaimedParticipantCost = TrainingCostExtensions.CalculatePerParticipantCost(this.RemainingToBeClaimed, this.ClaimParticipants); ;
			this.RemainingToBeClaimedMaxReimbursement = TrainingCostExtensions.CalculateRoundedReimbursementAmount(this.RemainingToBeClaimed, this.ReimbursementRate);
			this.RemainingToBeClaimedEmployerContribution = this.RemainingToBeClaimed - this.RemainingToBeClaimedMaxReimbursement;

			this.PaidToDate = claimEligibleCost.AmountPaidOrOwing();
		}
		#endregion
	}
}