using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

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
			Id = claimEligibleCost.Id;
			EligibleExpenseTypeCaption = claimEligibleCost.EligibleExpenseType.Caption;
			EligibleExpenseTypeDescription = claimEligibleCost.EligibleExpenseType.Description;
			SourceId = claimEligibleCost.SourceId;
			TotalClaimedReimbursement = claimEligibleCost.ParticipantCosts
				//.Where(x => x.ParticipantForm.Approved.HasValue && x.ParticipantForm.Approved.Value)
				//.Where(x => x.ParticipantForm.Attended.HasValue && x.ParticipantForm.Attended.Value)
				.Sum(x => x.ClaimReimbursement);
			EligibleCostId = claimEligibleCost.EligibleCostId;
			ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseType.Id;
			
			AgreedReimbursementRate = claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			AgreedMaxCost = claimEligibleCost.EligibleCost?.AgreedMaxCost ?? claimEligibleCost.Source?.AssessedCost ?? 0;
			AgreedMaxParticipants = claimEligibleCost.EligibleCost?.AgreedMaxParticipants ?? claimEligibleCost.Source?.AssessedParticipants ?? 0;
			AgreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? 0;
			AgreedEmployerContribution = claimEligibleCost.EligibleCost?.AgreedEmployerContribution ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? 0;
			AgreedMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Source?.AssessedMaxParticipantCost ?? 0;
			AgreedMaxParticipantReimbursementCost = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantReimbursement() ?? claimEligibleCost.Source?.AssessedMaxParticipantReimbursementCost ?? 0;
			AgreedParticipantEmployerContribution = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantEmployerContribution() ?? claimEligibleCost.Source?.AssessedParticipantEmployerContribution ?? 0;

			EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseType.Id;
			ExpenseType = claimEligibleCost.EligibleExpenseType.ExpenseTypeId;
			ServiceType = claimEligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;

			ClaimId = claimEligibleCost.ClaimId;
			ClaimVersion = claimEligibleCost.ClaimVersion;

			if (ExpenseType == ExpenseTypes.ParticipantAssigned)
			{
				if (claimEligibleCost.Claim.GrantApplication.RequireAllParticipantsBeforeSubmission)
				{
					ParticipantCosts = claimEligibleCost.ParticipantCosts.Where(w => w.ParticipantForm.Approved.HasValue && w.ParticipantForm.Approved.Value).Select(x => new ParticipantCostModel(x, AgreedReimbursementRate)).ToList();
				}
				else
				{
					ParticipantCosts = claimEligibleCost.ParticipantCosts.Select(x => new ParticipantCostModel(x, AgreedReimbursementRate)).ToList();
				}
			}
			else if (ExpenseType.In(ExpenseTypes.NotParticipantLimited, ExpenseTypes.ParticipantLimited) && ServiceType != ServiceTypes.EmploymentServicesAndSupports)
			{
				Breakdowns = claimEligibleCost.Breakdowns.Where(x => x.EligibleCostBreakdown.IsEligible).Select(x => new ClaimEligibleCostBreakdownModel(x)).ToList();
			}

			ClaimCost = claimEligibleCost.ClaimCost;
			ClaimParticipants = claimEligibleCost.ClaimParticipants;
			ClaimMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
			ClaimMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
			ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
			ClaimEmployerContribution = claimEligibleCost.CalculateClaimEmployerContribution();
			ClaimMaxReimbursement = claimEligibleCost.CalculateClaimReimbursement();


			if (claimEligibleCost.EligibleCost?.TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				Calculate(claimEligibleCost.EligibleCost.TrainingCost.GrantApplication, claimEligibleCost.Claim, claimEligibleCost);

				ClaimTotalPaid = claimEligibleCost.GetRemainingReimbursement();
				TotalClaimedReimbursement = ClaimTotalPaid;

			}
			else
			{
				TotalClaimedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.ClaimReimbursement);
				TotalAssessedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.AssessedReimbursement);
				SumOfParticipantCostUnitsUnassigned = claimEligibleCost.ClaimCost - ParticipantCosts.Sum(pc => pc.ClaimParticipantCost);
			}

			AssessedCost = claimEligibleCost.AssessedCost;
			AssessedParticipants = claimEligibleCost.AssessedParticipants;
			AssessedMaxParticipantCost = claimEligibleCost.AssessedMaxParticipantCost;
			AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
			AssessedParticipantEmployerContribution = claimEligibleCost.AssessedParticipantEmployerContribution;
			AssessedEmployerContribution = claimEligibleCost.CalculateAssessedEmployerContribution();
			AssessedMaxReimbursement = claimEligibleCost.CalculateAssessedReimbursement();
			AssessedReimbursementCost = claimEligibleCost.AssessedReimbursementCost;
			AddedByAssessor = claimEligibleCost.AddedByAssessor;
			TotalAssessedReimbursement = claimEligibleCost.AssessedReimbursementCost;
			if (claimEligibleCost.Claim?.GrantApplication?.GetProgramType() == ProgramTypes.EmployerGrant) {
				TotalAssessedReimbursement = claimEligibleCost.ParticipantCosts.Sum(x => x.AssessedReimbursement);
			}

			ClaimRowVersion = Convert.ToBase64String(claimEligibleCost.Claim.RowVersion);
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

			TotalClaimedToDate = claimEligibleCost.GetTotalAssessed();

			if (ServiceType.HasValue)
			{
				MaxReibursementTotalClaimedToDate = TrainingCostExtensions.CalculateRoundedReimbursementAmount(TotalClaimedToDate, AgreedReimbursementRate);
				EmployerContributionTotalClaimedToDate = TotalClaimedToDate - MaxReibursementTotalClaimedToDate;
				ParticipantCostTotalClaimedToDate = TrainingCostExtensions.CalculatePerParticipantCost(TotalClaimedToDate, claimEligibleCost.ClaimParticipants);
				TotalPaidClaimedToDate = claimEligibleCost.AmountPaidOrOwing();
				RemainingToClaimed = AgreedMaxCost - TotalClaimedToDate;
				ParticipantCostRemainingToClaimed = TrainingCostExtensions.CalculatePerParticipantCost(RemainingToClaimed, claimEligibleCost.ClaimParticipants);
				MaxReimbursementRemainingToClaimed = TrainingCostExtensions.CalculateRoundedReimbursementAmount(RemainingToClaimed, AgreedReimbursementRate);
				EmployerContributionRemainingToClaimed = RemainingToClaimed - MaxReimbursementRemainingToClaimed;
			}

		}
		#endregion
	}
}
