using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Application.Business.Models
{
	public class ClaimEligibleCostModel
	{
		#region Properties
		public int Id { get; set; }
		public int EligibleExpenseTypeId { get; set; }
		public ExpenseTypes ExpenseType { get; set; }
		public ServiceTypes? ServiceType { get; set; }
		public bool AutoCalculated { get; set; }
		public string EligibleExpenseTypeCaption { get; set; }
		public string EligibleExpenseTypeDescription { get; set; }
		public decimal TotalClaimedReimbursement { get; set; }
		public decimal TotalAssessedReimbursement { get; set; }
		public decimal SumOfParticipantCostUnitsUnassigned { get; set; }
		public int? SourceId { get; set; }
		public int? EligibleCostId { get; set; }
		public int? ClaimParticipants { get; set; }
		public int ClaimId { get; set; }
		public int ClaimVersion { get; set; }
		public List<ClaimEligibleCostBreakdownModel> Breakdowns { get; set; } = new List<ClaimEligibleCostBreakdownModel>();
		public List<ParticipantCostModel> ParticipantCosts { get; set; } = new List<ParticipantCostModel>();
		public string ClaimRowVersion { get; set; }
		public bool ConcurrencyError { get; set; } = false;

		#region Agreed
		public decimal AgreedMaxCost { get; set; }
		public int AgreedMaxParticipants { get; set; }
		public double AgreedReimbursementRate { get; set; }
		public decimal AgreedMaxReimbursement { get; set; }
		public decimal AgreedMaxParticipantCost { get; set; }
		public decimal AgreedEmployerContribution { get; set; }
		public decimal AgreedParticipantEmployerContribution { get; set; }
		public decimal AgreedMaxParticipantReimbursementCost { get; set; }
		#endregion

		#region Claimed To Date
		public decimal TotalClaimedToDate { get; set; }
		public decimal ParticipantCostTotalClaimedToDate { get; set; }
		public decimal EmployerContributionTotalClaimedToDate { get; set; }
		public decimal MaxReibursementTotalClaimedToDate { get; set; }
		public decimal TotalPaidClaimedToDate { get; set; }
		#endregion

		#region Remaining
		public decimal RemainingToClaimed { get; set; }
		public decimal ParticipantCostRemainingToClaimed { get; set; }
		public decimal EmployerContributionRemainingToClaimed { get; set; }
		public decimal MaxReimbursementRemainingToClaimed { get; set; }
		#endregion

		#region New Claim
		public decimal ClaimCost { get; set; }
		public decimal ClaimMaxParticipantCost { get; set; }
		public decimal ClaimParticipantEmployerContribution { get; set; }
		public decimal ClaimMaxParticipantReimbursementCost { get; set; }
		public decimal ClaimEmployerContribution { get; set; }
		public decimal ClaimMaxReimbursement { get; set; }
		public decimal ClaimTotalPaid { get; set; }
		#endregion

		#region Assessed
		public decimal AssessedCost { get; set; }
		public int AssessedParticipants { get; set; }
		public decimal AssessedMaxParticipantCost { get; set; }
		public decimal AssessedMaxParticipantReimbursementCost { get; set; }
		public decimal AssessedParticipantEmployerContribution { get; set; }
		public decimal AssessedReimbursementCost { get; set; }
		public decimal AssessedEmployerContribution { get; set; }
		public decimal AssessedMaxReimbursement { get; set; }
		public bool AddedByAssessor { get; set; }
		#endregion

		public bool RemoveOverride { get; set; }
		#endregion

		#region Construtors
		public ClaimEligibleCostModel()
		{

		}
		public ClaimEligibleCostModel(ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost == null) throw new ArgumentNullException(nameof(claimEligibleCost));
			this.Id = claimEligibleCost.Id;
			this.EligibleExpenseTypeCaption = claimEligibleCost.EligibleExpenseType.Caption;
			this.EligibleExpenseTypeDescription = claimEligibleCost.EligibleExpenseType.Description;
			this.SourceId = claimEligibleCost.SourceId;
			this.TotalClaimedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.ClaimReimbursement);
			this.EligibleCostId = claimEligibleCost.EligibleCostId;
			this.ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseType.Id;

			this.AgreedReimbursementRate = claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			this.AgreedMaxCost = claimEligibleCost.EligibleCost?.AgreedMaxCost ?? claimEligibleCost.Source?.AssessedCost ?? 0;
			this.AgreedMaxParticipants = claimEligibleCost.EligibleCost?.AgreedMaxParticipants ?? claimEligibleCost.Source?.AssessedParticipants ?? 0;
			this.AgreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? 0;
			this.AgreedEmployerContribution = claimEligibleCost.EligibleCost?.AgreedEmployerContribution ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? 0;
			this.AgreedMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Source?.AssessedMaxParticipantCost ?? 0;
			this.AgreedMaxParticipantReimbursementCost = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantReimbursement() ?? claimEligibleCost.Source?.AssessedMaxParticipantReimbursementCost ?? 0;
			this.AgreedParticipantEmployerContribution = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantEmployerContribution() ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? 0;

			this.EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseType.Id;
			this.ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseTypeId;
			this.ServiceType = claimEligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;

			this.ClaimId = claimEligibleCost.ClaimId;
			this.ClaimVersion = claimEligibleCost.ClaimVersion;

			if (this.ExpenseType == ExpenseTypes.ParticipantAssigned)
			{
				this.ParticipantCosts = claimEligibleCost.ParticipantCosts.Select(x => new ParticipantCostModel(x, this.AgreedReimbursementRate)).ToList();
			}
			else if (this.ExpenseType.In(ExpenseTypes.NotParticipantLimited, ExpenseTypes.ParticipantLimited) && this.ServiceType != ServiceTypes.EmploymentServicesAndSupports)
			{
				this.Breakdowns = claimEligibleCost.Breakdowns.Where(x => x.EligibleCostBreakdown.IsEligible).Select(x => new ClaimEligibleCostBreakdownModel(x)).ToList();
			}

			this.ClaimCost = claimEligibleCost.ClaimCost;
			this.ClaimParticipants = claimEligibleCost.ClaimParticipants;
			this.ClaimMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
			this.ClaimMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
			this.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			this.ClaimEmployerContribution = claimEligibleCost.CalculateClaimEmployerContribution();
			this.ClaimMaxReimbursement = claimEligibleCost.CalculateClaimReimbursement();


			if (claimEligibleCost.EligibleCost?.TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				Calculate(claimEligibleCost.EligibleCost.TrainingCost.GrantApplication, claimEligibleCost.Claim, claimEligibleCost);

				this.ClaimTotalPaid = claimEligibleCost.GetRemainingReimbursement();
				this.TotalClaimedReimbursement = this.ClaimTotalPaid;

			}
			else
			{
				this.TotalClaimedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.ClaimReimbursement);
				this.TotalAssessedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.AssessedReimbursement);
				this.SumOfParticipantCostUnitsUnassigned = claimEligibleCost.ClaimCost - this.ParticipantCosts.Sum(pc => pc.ClaimParticipantCost);
			}

			this.AssessedCost = claimEligibleCost.AssessedCost;
			this.AssessedParticipants = claimEligibleCost.AssessedParticipants;
			this.AssessedMaxParticipantCost = claimEligibleCost.AssessedMaxParticipantCost;
			this.AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
			this.AssessedParticipantEmployerContribution = claimEligibleCost.AssessedParticipantEmployerContribution;
			this.AssessedEmployerContribution = claimEligibleCost.CalculateAssessedEmployerContribution();
			this.AssessedMaxReimbursement = claimEligibleCost.CalculateAssessedReimbursement();
			this.AssessedReimbursementCost = claimEligibleCost.AssessedReimbursementCost;
			this.AddedByAssessor = claimEligibleCost.AddedByAssessor;
			this.TotalAssessedReimbursement = claimEligibleCost.AssessedReimbursementCost;

			this.ClaimRowVersion = Convert.ToBase64String(claimEligibleCost.Claim.RowVersion);
		}
		#endregion

		#region Methods
		private void Calculate(GrantApplication grantApplication, Claim claim, ClaimEligibleCost claimEligibleCost)
		{
			var claims = grantApplication.Claims.Where(x => x.Id < claim.Id).GroupBy(x => x.Id).Select(x => new
			{
				ClaimId = x.Key,
				ClaimVersion = x.Max(y => y.ClaimVersion)
			}).ToList();

			this.TotalClaimedToDate = claimEligibleCost.GetTotalAssessed();

			if (this.ServiceType.HasValue)
			{
				this.MaxReibursementTotalClaimedToDate = TrainingCostExtensions.CalculateRoundedReimbursementAmount(this.TotalClaimedToDate, this.AgreedReimbursementRate);
				this.EmployerContributionTotalClaimedToDate = this.TotalClaimedToDate - this.MaxReibursementTotalClaimedToDate;
				this.ParticipantCostTotalClaimedToDate = TrainingCostExtensions.CalculatePerParticipantCost(this.TotalClaimedToDate, claimEligibleCost.ClaimParticipants);
				this.TotalPaidClaimedToDate = claimEligibleCost.AmountPaidOrOwing();
				this.RemainingToClaimed = this.AgreedMaxCost - this.TotalClaimedToDate;
				this.ParticipantCostRemainingToClaimed = TrainingCostExtensions.CalculatePerParticipantCost(this.RemainingToClaimed, claimEligibleCost.ClaimParticipants);
				this.MaxReimbursementRemainingToClaimed = TrainingCostExtensions.CalculateRoundedReimbursementAmount(this.RemainingToClaimed, this.AgreedReimbursementRate);
				this.EmployerContributionRemainingToClaimed = this.RemainingToClaimed - this.MaxReimbursementRemainingToClaimed;
			}

		}
		#endregion
	}
}
