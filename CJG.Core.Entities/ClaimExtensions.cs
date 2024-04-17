using System;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ClaimExtensions"/> static class, provides extension methods for <typeparamref name="Claim"/> and related objects.
	/// </summary>
	public static class ClaimExtensions
	{
		/// <summary>
		/// Calculate the claim eligible cost reimbursement amount per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			var rate = claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			return TrainingCostExtensions.CalculateRoundedReimbursementAmount(claimEligibleCost.ClaimCost, rate);
		}

		/// <summary>
		/// Calculate the claim eligible cost employer contribution.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimMaxParticipantReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			var rate = claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			return TrainingCostExtensions.CalculateTruncatedReimbursementAmount(claimEligibleCost.ClaimMaxParticipantCost, rate);
		}

		/// <summary>
		/// Calculate the claim eligible cost employer contribution.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimMaxParticipantReimbursementETG(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			//var PIFCount = claimEligibleCost.Claim.GrantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim);
			//var participantCount = (PIFCount != 0) ? PIFCount : claimEligibleCost.ParticipantCosts.Count();

			var rule1 = claimEligibleCost.ClaimCost == 0 ? 0 : (claimEligibleCost.ClaimCost / claimEligibleCost.ClaimParticipants);
			var rule2 = claimEligibleCost.ClaimCost == 0 ? 0 : (claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment / claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants);
			var result = Math.Min(Math.Min(rule1, rule2), claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Claim.GrantApplication.MaxReimbursementAmt);

			return result;
		}

		/// <summary>
		/// Calculate the claim eligible cost employer contribution amount per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimEmployerContribution(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.ClaimCost - claimEligibleCost.CalculateClaimReimbursement();
		}

		/// <summary>
		/// Calculate the claim eligible cost employer contribution.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimParticipantEmployerContribution(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.ClaimMaxParticipantCost - claimEligibleCost.CalculateClaimMaxParticipantReimbursement();
		}

		/// <summary>
		/// Calculate the claim cost per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimParticipantCost(this ClaimEligibleCost claimEligibleCost)
		{
			return TrainingCostExtensions.CalculatePerParticipantCost(claimEligibleCost.ClaimCost, claimEligibleCost.ClaimParticipants);
		}

		/// <summary>
		/// Calculate the claim eligible cost assessed reimbursement amount per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			return TrainingCostExtensions.CalculateRoundedReimbursementAmount(claimEligibleCost.AssessedCost, claimEligibleCost.Claim.GrantApplication.ReimbursementRate);
		}

		/// <summary>
		/// Calculate the claim eligible cost max participant reimbursement.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedMaxParticipantReimbursementETG(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			if (claimEligibleCost.AssessedMaxParticipantCost == 0)
				return claimEligibleCost.AssessedMaxParticipantCost;
			else
				return claimEligibleCost.ClaimMaxParticipantReimbursementCost;
		}

		/// <summary>
		/// Calculate the claim eligible cost max participant reimbursement.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedMaxParticipantReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			if (claimEligibleCost.Claim == null || claimEligibleCost.Claim.GrantApplication == null || claimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(claimEligibleCost), $"The argument {nameof(claimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			return TrainingCostExtensions.CalculateTruncatedReimbursementAmount(claimEligibleCost.AssessedMaxParticipantCost, claimEligibleCost.Claim.GrantApplication.ReimbursementRate);
		}

		public static decimal CalculateAssessedMaxParticipantEmployerContributionETG(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.AssessedMaxParticipantCost - claimEligibleCost.CalculateAssessedMaxParticipantReimbursementETG();
		}

		/// <summary>
		/// Calculate the claim eligible cost assessed max participant employer contribution.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedMaxParticipantEmployerContribution(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.AssessedMaxParticipantCost - claimEligibleCost.CalculateAssessedMaxParticipantReimbursement();
		}

		/// <summary>
		/// Calculate the claim eligible cost assessed employer contribution amount per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedEmployerContribution(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.AssessedCost - claimEligibleCost.CalculateAssessedReimbursement();
		}

		/// <summary>
		/// Calculate the claim assessed cost per participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedParticipantCost(this ClaimEligibleCost claimEligibleCost)
		{
			return TrainingCostExtensions.CalculatePerParticipantCost(claimEligibleCost.AssessedCost, claimEligibleCost.AssessedParticipants);
		}

		/// <summary>
		/// Calculate and update the claimed participant cost, reimbursement and employer contribution for the specified claim eligible cost.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		public static void RecalculateClaimCost(this ClaimEligibleCost claimEligibleCost)
		{
			claimEligibleCost.ClaimMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? 0;
			//claimEligibleCost.ClaimMaxParticipantCost = claimEligibleCost.CalculateClaimParticipantCost();

			var rate = (decimal) claimEligibleCost.Claim.GrantApplication.ReimbursementRate;

			var ruleMaxReimbursement = Math.Round(claimEligibleCost.ClaimMaxParticipantCost * rate, 2);
			var rule1 = claimEligibleCost.ClaimCost == 0 || claimEligibleCost.ClaimParticipants == 0
				? 0
				: Math.Round(((claimEligibleCost.ClaimCost / claimEligibleCost.ClaimParticipants) * rate), 2);
			var rule2 = claimEligibleCost.ClaimCost == 0
				? 0
				: Math.Round(claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment / claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants, 2);
			var result = Math.Min(ruleMaxReimbursement, Math.Min(Math.Min(rule1, rule2), claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Claim.GrantApplication.MaxReimbursementAmt));
			
			claimEligibleCost.ClaimMaxParticipantReimbursementCost = result;
			claimEligibleCost.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipants == 0 ? 0 : (claimEligibleCost.ClaimCost / claimEligibleCost.ClaimParticipants) - result;
		}

		/// <summary>
		/// Calculate and update the claim assessed max participant cost and participant employer contribution for the specified claim eligible cost.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <param name="overrideRates">Whether to override the default rate calculations</param>
		public static void RecalculateAssessedCost(this ClaimEligibleCost claimEligibleCost, bool overrideRates = false)
		{
			var maxAssessedParticipantCost = claimEligibleCost.EligibleCost.AgreedMaxParticipantCost;
			claimEligibleCost.AssessedMaxParticipantCost = Math.Min(maxAssessedParticipantCost, claimEligibleCost.CalculateAssessedParticipantCost());

			if (!overrideRates)
			{
				claimEligibleCost.AssessedReimbursementCost = claimEligibleCost.AssessedCost;

				var rate = (decimal) claimEligibleCost.Claim.GrantApplication.ReimbursementRate;

				var ruleMaxReimbursement = Math.Round(claimEligibleCost.AssessedMaxParticipantCost * rate, 2);
				var rule1 = (claimEligibleCost.AssessedCost == 0 || claimEligibleCost.ClaimParticipants == 0) ? 0 : Math.Round(((claimEligibleCost.AssessedCost / claimEligibleCost.ClaimParticipants) * rate), 2);
				var rule2 = (claimEligibleCost.AssessedCost == 0) ? 0 : Math.Round((claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment / claimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants), 2);
				var result = Math.Min(ruleMaxReimbursement, Math.Min(Math.Min(rule1, rule2), claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Claim.GrantApplication.MaxReimbursementAmt));

				claimEligibleCost.AssessedMaxParticipantReimbursementCost = result;
				claimEligibleCost.AssessedParticipantEmployerContribution = (claimEligibleCost.ClaimParticipants == 0) ? 0 : (claimEligibleCost.AssessedCost / claimEligibleCost.ClaimParticipants) - result;

				foreach (var participantCost in claimEligibleCost.ParticipantCosts)
				{
					participantCost.RecalculatedAssessedCost();
				}
			}
		}

		/// <summary>
		/// Calculate the total claim reimbursements by summing the total from each participant cost.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static decimal CalculateTotalClaimReimbursement(this Claim claim)
		{
			var totalClaimReimbursement = (decimal)0.0;
			foreach (var eligibleCost in claim.EligibleCosts)
			{
				if (eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					// sum up the total cost from the "Participant Costs" table if this claim is of type single amendable
					totalClaimReimbursement += eligibleCost.ParticipantCosts.Sum(pc => pc.ClaimReimbursement);
				else if ((eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited))
					// otherwise sum up the total ClaimReimbursementCost from the "Claim Eligible Costs" table
					totalClaimReimbursement += eligibleCost.ClaimReimbursementCost;
			}

			return totalClaimReimbursement;
		}

		/// <summary>
		/// Calculate the total claim assessed reimbursement by summing the total from each participant cost.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static decimal CalculateTotalAssessedReimbursement(this Claim claim)
		{
			var totalAssessedReimbursement = (decimal)0.0;
			foreach (var eligibleCost in claim.EligibleCosts)
			{
				if (eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					totalAssessedReimbursement += eligibleCost.ParticipantCosts.Sum(pc => pc.AssessedReimbursement);

				else if (eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited)
					totalAssessedReimbursement += eligibleCost.AssessedReimbursementCost;
			}

			return totalAssessedReimbursement;
		}

		/// <summary>
		/// Calculate and update the claimed participant cost, reimbursement and employer contributions for all claim eligible costs for the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		public static void RecalculateClaimedCosts(this Claim claim)
		{
			foreach (var eligibleCost in claim.EligibleCosts)
			{
				eligibleCost.RecalculateClaimCost();

				foreach (var participantCost in eligibleCost.ParticipantCosts)
					participantCost.RecalculateClaimParticipantCostETG();
			}

			claim.TotalClaimReimbursement = claim.CalculateTotalClaimReimbursement();
		}

		/// <summary>
		/// Calculate and update the claim assessment participant cost for all claim eligible costs for the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="overrideRates">Whether to override the reimbursement rates.</param>
		public static void RecalculateAssessedCosts(this Claim claim, bool overrideRates = false)
		{
			foreach (var eligibleCost in claim.EligibleCosts)
			{
				eligibleCost.RecalculateAssessedCost(overrideRates);

				if (!overrideRates)
				{
					foreach (var participantCost in eligibleCost.ParticipantCosts)
					{
						participantCost.RecalculatedAssessedCost();
					}
				}
			}

			claim.TotalAssessedReimbursement = claim.CalculateTotalAssessedReimbursement();
		}

		/// <summary>
		/// Calculate the claim participant cost reimbursement amount.
		/// </summary>
		/// <param name="participantCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimReimbursement(this ParticipantCost participantCost)
		{
			if (participantCost.ClaimEligibleCost == null || participantCost.ClaimEligibleCost.Claim == null || participantCost.ClaimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(participantCost), $"The argument {nameof(participantCost)}.{nameof(ClaimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			if (participantCost.ClaimEligibleCost.Claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant) // TODO: this calculation is currently not being used. Server calculation should be used.
			{
				var maxPerParticipantCost = (participantCost.ClaimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment / participantCost.ClaimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants);
				var reimbursementRate = (decimal)participantCost?.ClaimEligibleCost?.Claim?.GrantApplication?.ReimbursementRate;
				var rule1 = (participantCost.ClaimParticipantCost == 0 || participantCost.ClaimEligibleCost?.ClaimParticipants == 0) ? 0 : Math.Round((participantCost.ClaimParticipantCost / participantCost.ClaimEligibleCost.ClaimParticipants), 2);
				var rule2 = (participantCost.ClaimParticipantCost == 0) ? 0 : Math.Round((participantCost.ClaimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedCommitment / participantCost.ClaimEligibleCost.Claim.GrantApplication.TrainingCost.AgreedParticipants), 2);
				var rule3 = (participantCost.ClaimParticipantCost == 0 || participantCost.ClaimEligibleCost?.ClaimParticipants == 0) ? 0 : Math.Round(((maxPerParticipantCost * participantCost.ClaimEligibleCost.ClaimParticipants) + participantCost.ClaimReimbursement - participantCost.ClaimEligibleCost.Claim.TotalClaimReimbursement), 2);
				var rule4 = (participantCost.ClaimParticipantCost == 0) ? 0 : Math.Round(participantCost.ClaimParticipantCost * reimbursementRate, 2);
				var agreedMaxParticipantCost = participantCost.ClaimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? participantCost.ClaimEligibleCost.Claim.GrantApplication.MaxReimbursementAmt;
				var claimMaxParticipantReimbursementCost = participantCost.ClaimEligibleCost?.ClaimMaxParticipantReimbursementCost ?? participantCost.ClaimEligibleCost.Claim.GrantApplication.MaxReimbursementAmt;

				var result = Math.Min(Math.Min(rule1, Math.Min(rule2, Math.Min(rule3, Math.Min(rule4, agreedMaxParticipantCost)))), claimMaxParticipantReimbursementCost);
				return result;
			}

			var rate = participantCost.ClaimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			return TrainingCostExtensions.CalculateTruncatedReimbursementAmount(participantCost.ClaimParticipantCost, rate);
		}

		/// <summary>
		/// Calculate the claim participant cost employer contribution.
		/// </summary>
		/// <param name="participantCost"></param>
		/// <returns></returns>
		public static decimal CalculateClaimEmployerContribution(this ParticipantCost participantCost)
		{
			return participantCost.ClaimParticipantCost - participantCost.CalculateClaimReimbursement();
		}

		public static decimal CalculateClaimEmployerContributionETG(this ParticipantCost participantCost)
		{
			return participantCost.ClaimParticipantCost - participantCost.ClaimReimbursement;
		}

		/// <summary>
		/// Calculate the assessed participant cost reimbursement amount.
		/// </summary>
		/// <param name="participantCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedReimbursement(this ParticipantCost participantCost)
		{
			if (participantCost.ClaimEligibleCost == null || participantCost.ClaimEligibleCost.Claim == null || participantCost.ClaimEligibleCost.Claim.GrantApplication == null || participantCost.ClaimEligibleCost.Claim.GrantApplication == null)
				throw new ArgumentNullException(nameof(participantCost), $"The argument {nameof(participantCost)}.{nameof(ClaimEligibleCost)}.{nameof(Claim)}.{nameof(TrainingProgram)}.{nameof(GrantApplication)} cannot be null.");

			var rate = participantCost.ClaimEligibleCost.Claim.GrantApplication.ReimbursementRate;
			return TrainingCostExtensions.CalculateTruncatedReimbursementAmount(participantCost.AssessedParticipantCost, rate);
		}

		/// <summary>
		/// Calculate the assessed participant cost employer contribution.
		/// </summary>
		/// <param name="participantCost"></param>
		/// <returns></returns>
		public static decimal CalculateAssessedEmployerContribution(this ParticipantCost participantCost)
		{
			if (participantCost.ClaimEligibleCost.Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant)
				return participantCost.AssessedParticipantCost - participantCost.CalculateAssessedReimbursement();
			else
				return participantCost.AssessedParticipantCost - participantCost.AssessedReimbursement;
		}

		/// <summary>
		/// Calculate and update the claimed participant cost, reimbursement and employer contribution for the specified claim eligible cost.
		/// </summary>
		/// <param name="participantCost"></param>
		public static void RecalculateClaimCost(this ParticipantCost participantCost)
		{
			participantCost.ClaimReimbursement = participantCost.CalculateClaimReimbursement();
			participantCost.ClaimEmployerContribution = participantCost.CalculateClaimEmployerContribution();
		}

		public static void RecalculateClaimParticipantCostETG(this ParticipantCost participantCost)
		{
			participantCost.ClaimReimbursement = participantCost.ClaimReimbursement;
			participantCost.ClaimEmployerContribution = participantCost.CalculateClaimEmployerContributionETG();
		}

		/// <summary>
		/// Calculate and update the assessed participant employer contribution.
		/// </summary>
		/// <param name="participantCost"></param>
		public static void RecalculatedAssessedCost(this ParticipantCost participantCost)
		{
			if (participantCost.ClaimEligibleCost.Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant)
				participantCost.AssessedReimbursement = participantCost.CalculateAssessedReimbursement();
			//participantCost.AssessedReimbursement = participantCost.AssessedReimbursement;
			participantCost.AssessedEmployerContribution = participantCost.CalculateAssessedEmployerContribution();
		}

		/// <summary>
		/// The number of participants that have costs associated with the specified Claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static int ParticipantsWithEligibleCosts(this Claim claim)
		{
			return (from pc in claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
					where pc.ClaimParticipantCost > 0
					select pc.ParticipantFormId).Distinct().Count();
		}

		/// <summary>
		/// The number of participants that do not have costs associated with the specified Claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="excludeClaimReported"></param>
		/// <returns></returns>
		public static int ParticipantsWithoutEligibleCosts(this Claim claim, bool excludeClaimReported = false)
		{
			bool requirePIFs = claim.GrantApplication.RequireAllParticipantsBeforeSubmission;

			var participantCosts = claim.EligibleCosts.SelectMany(x => x.ParticipantCosts);

			var costPerParticipantForm =
				from pf in claim.GrantApplication.ParticipantForms.Where(w=> (w.Attended.HasValue && w.Attended.Value) || !requirePIFs)
				join pc in participantCosts on pf.Id equals pc.ParticipantFormId into ps
				from p in ps.DefaultIfEmpty()
				select new
				{
					ParticipantFormId = pf.Id,
					ParticipantCost = p?.ClaimParticipantCost ?? 0,
					pf.ClaimReported
				};

			var resultCostPerParticipantForm = excludeClaimReported ? costPerParticipantForm.Where(x => !x.ClaimReported) : costPerParticipantForm;

			return resultCostPerParticipantForm
				.GroupBy(x => x.ParticipantFormId)
				.Select(g => new
				{
					TotalCost = g.Sum(x => x.ParticipantCost)
				}).Count(x => x.TotalCost == 0);

		}

		/// <summary>
		/// The number of participants that do not have costs associated with the specified Claim and do not have costs associated with the prior claim version.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="priorClaimVersion"></param>
		/// <returns></returns>
		public static int ParticipantsWithoutEligibleCosts(this Claim claim, Claim priorClaimVersion)
		{
			if (priorClaimVersion == null)
				return claim.ParticipantsWithoutEligibleCosts(true);

			if (claim.Id != priorClaimVersion.Id || claim.ClaimVersion != priorClaimVersion.ClaimVersion + 1)
				throw new InvalidOperationException("Prior claim version is invalid.");

			var participants = (from pc in claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
								group pc by pc.ParticipantFormId into g
								where g.Sum(a => a.ClaimParticipantCost) == 0
								select g.Key).Distinct().ToArray();

			// Fetch all participants costs from prior claim version that are greater than 0.
			// Ensure that the current participants that are claiming $0 have a claim >$0 in the prior claim.
			return (from pcv in priorClaimVersion.EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
					join pc in participants on pcv.ParticipantFormId equals pc into pe
					from pc in pe
					where pcv.ClaimParticipantCost > 0
					select pc).Distinct().Count();
		}

		/// <summary>
		/// Copy the claimed values into the assessed values.
		/// </summary>
		/// <param name="claim"></param>
		public static void CopyClaimValuesToAssessed(this Claim claim)
		{
			foreach (var claimEligibleCost in claim.EligibleCosts)
			{
				claimEligibleCost.AssessedCost = claimEligibleCost.ClaimCost;
				claimEligibleCost.AssessedParticipants = claimEligibleCost.ClaimParticipants;
				claimEligibleCost.AssessedMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
				claimEligibleCost.AssessedMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
				claimEligibleCost.AssessedParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;
				claimEligibleCost.AssessedReimbursementCost = (decimal)claimEligibleCost.ClaimReimbursementCost;

				foreach (var participantCost in claimEligibleCost.ParticipantCosts)
				{
					participantCost.AssessedEmployerContribution = participantCost.ClaimEmployerContribution;
					participantCost.AssessedParticipantCost = participantCost.ClaimParticipantCost;
					participantCost.AssessedReimbursement = participantCost.ClaimReimbursement;
				}

				foreach (var breakdown in claimEligibleCost.Breakdowns)
				{
					breakdown.AssessedCost = breakdown.ClaimCost;
				}
			}

			// make sure that TotalAssessedReimbursement is calculated correctly
			claim.RecalculateAssessedCosts();
		}

		/// <summary>
		/// Sets the ParticipantEnrollment.ClaimReported flag to 'true' for all participants.
		/// This ensures they are note removed in subsequent claim amendments.
		/// </summary>
		/// <param name="claim"></param>
		public static void LockParticipants(this Claim claim)
		{
			foreach (var eligibleCost in claim.EligibleCosts)
			{
				if (claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
				{
					claim.GrantApplication.ParticipantForms.ToList().ForEach(pf => pf.ClaimReported = true);
				}
				else
				{
					foreach (var participantCost in eligibleCost.ParticipantCosts)
					{
						participantCost.ParticipantForm.ClaimReported = true;
					}
				}
			}
		}

		/// <summary>
		/// Reset all the assessment values on the claim back to zero.
		/// </summary>
		/// <param name="claim"></param>
		public static void ResetAssessment(this Claim claim)
		{
			foreach (var claimEligibleCost in claim.EligibleCosts)
			{
				claimEligibleCost.AssessedCost = 0;
				claimEligibleCost.AssessedParticipants = 0;
				claimEligibleCost.AssessedMaxParticipantCost = 0;
				claimEligibleCost.AssessedMaxParticipantReimbursementCost = 0;
				claimEligibleCost.AssessedParticipantEmployerContribution = 0;
				claimEligibleCost.AssessedReimbursementCost = 0;

				foreach (var participantCost in claimEligibleCost.ParticipantCosts)
				{
					participantCost.AssessedEmployerContribution = 0;
					participantCost.AssessedParticipantCost = 0;
					participantCost.AssessedReimbursement = 0;
				}

				foreach (var breakdown in claimEligibleCost.Breakdowns)
				{
					breakdown.AssessedCost = 0;
				}
			}

			// make sure that TotalAssessedReimbursement is calculated correctly
			claim.RecalculateAssessedCosts();
		}

		/// <summary>
		/// Get the prior approved claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static Claim GetPriorApprovedClaim(this Claim claim)
		{
			return claim.GrantApplication.Claims.Where(c => c.IsApproved() && c.ClaimVersion < claim.ClaimVersion).OrderByDescending(c => c.ClaimVersion).FirstOrDefault();
		}

		/// <summary>
		/// Determine if the training program has a prior claim that has been approved.
		/// If there is only one claim, even if it has been approved it will be false.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static bool HasPriorApprovedClaim(this Claim claim)
		{
			return claim.GrantApplication.Claims.Count() <= 1
				? false
				: claim.GrantApplication.Claims.Any(c => c.IsApproved() && c.ClaimVersion < claim.ClaimVersion);
		}

		/// <summary>
		/// Determine whether the Claim is in an approved state.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static bool IsApproved(this Claim claim)
		{
			return claim.ClaimState.In(ClaimState.ClaimApproved, ClaimState.ClaimPaid, ClaimState.AmountReceived, ClaimState.AmountOwing, ClaimState.PaymentRequested, ClaimState.ClaimAmended);
		}

		/// <summary>
		/// Determine if the specified claim eligible cost originated from the source.
		/// This occurs when a new claim is generated.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <param name="compare"></param>
		/// <returns></returns>
		private static bool IsSameEligibleCost(ClaimEligibleCost claimEligibleCost, ClaimEligibleCost compare)
		{
			if (claimEligibleCost == null)
				throw new ArgumentNullException(nameof(claimEligibleCost));

			if (compare == null)
				return false;

			return claimEligibleCost.Id == compare.Id
			       || claimEligibleCost.SourceId == compare.Id
			       || (claimEligibleCost.EligibleCostId != null && claimEligibleCost.EligibleCostId == compare.EligibleCostId)
			       || IsSameEligibleCost(claimEligibleCost, compare.Source);
		}

		/// <summary>
		/// Returns the amount paid or owing by summing up the payment requests.
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static decimal AmountPaidOrOwing(this Claim claim)
		{
			if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
				return claim.TotalAssessedReimbursement - claim.GrantApplication.PaymentRequests
					       .Where(o => o.ClaimVersion != claim.ClaimVersion)
					       .Sum(o => o.PaymentAmount);

			return claim.TotalAssessedReimbursement;
		}

		/// <summary>
		/// Calculate the total amount paid or owing for this line item.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal AmountPaidOrOwing(this ClaimEligibleCost claimEligibleCost)
		{
			var grantApplication = claimEligibleCost.Claim.GrantApplication;
			var claim = claimEligibleCost.Claim;

			// Find all prior claims.
			var claims = grantApplication.Claims.Where(c => c.Id < claim.Id
				&& c.ClaimState.In(ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.GroupBy(x => x.Id).Select(x => new
				{
					ClaimId = x.Key,
					ClaimVersion = x.Max(y => y.ClaimVersion)
				}).ToArray();

			return grantApplication.Claims.Where(c => claims.Any(y => y.ClaimId == c.Id && y.ClaimVersion == c.ClaimVersion))
				.Sum(c => c.EligibleCosts.Where(ec => ec.Id == claimEligibleCost.Id || IsSameEligibleCost(claimEligibleCost, ec)).Sum(ec => ec.AssessedReimbursementCost));
		}

		/// <summary>
		/// Calculate the total amount reimbursed for this line item.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal GetTotalReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			var grantApplication = claimEligibleCost.Claim.GrantApplication;
			var claim = claimEligibleCost.Claim;

			// Find all prior claims.
			var claims = grantApplication.Claims.Where(c => c.Id < claim.Id
				&& c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.GroupBy(x => x.Id).Select(x => new
				{
					ClaimId = x.Key,
					ClaimVersion = x.Max(y => y.ClaimVersion)
				}).ToArray();

			return grantApplication.Claims.Where(c => claims.Any(y => y.ClaimId == c.Id && y.ClaimVersion == c.ClaimVersion))
				.Sum(c => c.EligibleCosts.Where(ec => ec.Id == claimEligibleCost.Id || IsSameEligibleCost(claimEligibleCost, ec)).Sum(ec => ec.AssessedReimbursementCost));
		}

		/// <summary>
		/// Calculate the total amount assessed for this line item breakdown.
		/// </summary>
		/// <param name="claimBreakdownCost"></param>
		/// <returns></returns>
		public static decimal GetTotalAssessed(this ClaimBreakdownCost claimBreakdownCost)
		{
			var claimEligibleCost = claimBreakdownCost.ClaimEligibleCost;
			var grantApplication = claimEligibleCost.Claim.GrantApplication;
			var claim = claimEligibleCost.Claim;

			// Find all prior claims.
			var claims = grantApplication.Claims.Where(c => c.Id < claim.Id
				&& c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.GroupBy(x => x.Id).Select(x => new
				{
					ClaimId = x.Key,
					ClaimVersion = x.Max(y => y.ClaimVersion)
				}).ToArray();

			return grantApplication.Claims.Where(c => claims.Any(y => y.ClaimId == c.Id && y.ClaimVersion == c.ClaimVersion))
				.Sum(c => c.EligibleCosts.Where(ec => ec.Id == claimEligibleCost.Id || IsSameEligibleCost(claimEligibleCost, ec)).Sum(ec => ec.Breakdowns.Where(b => b.EligibleCostBreakdownId == claimBreakdownCost.EligibleCostBreakdownId).Sum(b => b.AssessedCost)));
		}

		/// <summary>
		/// Calculate the total amount assessed for this line item.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal GetTotalAssessed(this ClaimEligibleCost claimEligibleCost)
		{
			var grantApplication = claimEligibleCost.Claim.GrantApplication;
			var claim = claimEligibleCost.Claim;

			// Find all prior claims.
			var claims = grantApplication.Claims.Where(c => c.Id < claim.Id
				&& c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.GroupBy(x => x.Id).Select(x => new
				{
					ClaimId = x.Key,
					ClaimVersion = x.Max(y => y.ClaimVersion)
				}).ToArray();

			return grantApplication.Claims.Where(c => claims.Any(y => y.ClaimId == c.Id && y.ClaimVersion == c.ClaimVersion))
				.Sum(x => x.EligibleCosts.Where(ec => ec.Id == claimEligibleCost.Id || IsSameEligibleCost(claimEligibleCost, ec)).Sum(y => y.AssessedCost));
		}

		/// <summary>
		/// Calculate the remaining amount that can be reimbursed.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static decimal GetRemainingReimbursement(this ClaimEligibleCost claimEligibleCost)
		{
			var totalReimbursement = GetTotalReimbursement(claimEligibleCost);
			var agreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? 0;
			return Math.Min(claimEligibleCost.ClaimReimbursementCost, agreedMaxReimbursement - totalReimbursement); // TODO: Why use Min here?
		}

		/// <summary>
		/// check the participant form table to see if there are more participants added after the form was previously saved
		/// </summary>
		/// <param name="claim"></param>
		/// <returns></returns>
		public static bool HasNumberOfParticipantsChanged(this Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			// If there are any expense lines that are incorrectly totaled, then return true.
			var count = claim.GrantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim);
			return claim.EligibleCosts.Any(ec => ec.EligibleExpenseType.ExpenseType.Id == ExpenseTypes.NotParticipantLimited && ec.ClaimParticipants < count);
		}

		/// <summary>
		/// Get the max number of claim eligible cost participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static int MaxClaimParticipants(this ClaimEligibleCost claimEligibleCost)
		{
			return claimEligibleCost.EligibleCost?.AgreedMaxParticipants
			       ?? claimEligibleCost.Source?.AssessedParticipants
			       ?? claimEligibleCost.AssessedParticipants;
		}

		/// <summary>
		/// Get the max number of claim eligible cost participant.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <returns></returns>
		public static void UpdateUpToMaxClaimParticipants(this ClaimEligibleCost claimEligibleCost, int numberOfParticipants)
		{
			var maxClaimParticipants = claimEligibleCost.MaxClaimParticipants();

			claimEligibleCost.ClaimParticipants = numberOfParticipants > maxClaimParticipants
				? maxClaimParticipants
				: numberOfParticipants;
		}
	}
}
