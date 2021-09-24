using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// TrainingCost static class, extension methods.
	/// </summary>
	public static class TrainingCostExtensions
	{
		/// <summary>
		/// Calculate the per participant reimbursement (reimbursement / participants).
		/// If participants is less-than-or-equal to 0 it will return 0.
		/// </summary>
		/// <param name="reimbursment"></param>
		/// <param name="participants"></param>
		/// <returns></returns>
		public static decimal CalculatePerParticipantReimbursement(decimal reimbursment, int participants)
		{
			if (participants <= 0)
				return 0;

			return Math.Truncate(100 * (reimbursment / participants)) / 100;
		}

		/// <summary>
		/// Calculate the participant costs (cost / participants).
		/// If participants is less-than-or-equal to 0 it will return 0.
		/// </summary>
		/// <param name="cost"></param>
		/// <param name="participants"></param>
		/// <returns></returns>
		public static decimal CalculatePerParticipantCost(decimal cost, int participants)
		{
			if (participants <= 0)
				return 0;

			return Math.Truncate(100 * (cost / participants)) / 100;
		}

		/// <summary>
		/// Calcualte the reimbursement amount (cost * rate).
		/// </summary>
		/// <param name="cost"></param>
		/// <param name="rate"></param>
		/// <returns></returns>
		public static decimal CalculateRoundedReimbursementAmount(decimal cost, double rate)
		{
			return Math.Round(cost * (decimal)rate, 2);
		}

		/// <summary>
		/// Calcualte the reimbursement amount (cost * rate).
		/// </summary>
		/// <param name="cost"></param>
		/// <param name="rate"></param>
		/// <returns></returns>
		public static decimal CalculateTruncatedReimbursementAmount(decimal cost, double rate)
		{
			return Math.Truncate(100 * cost * (decimal)rate) / 100;
		}

		/// <summary>
		/// Calculate the estimated per participant reimbursement amount.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedPerParticipantReimbursement(this EligibleCost eligibleCost)
		{
			if (eligibleCost.TrainingCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)}.{nameof(TrainingCost)}.{nameof(GrantApplication)} cannot be null.");
			return CalculatePerParticipantReimbursement(eligibleCost.EstimatedReimbursement, eligibleCost.EstimatedParticipants);
		}

		/// <summary>
		/// Calculate the estimated requested government contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedReimbursement(this EligibleCost eligibleCost)
		{
			if (eligibleCost.TrainingCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)}.{nameof(TrainingCost)}.{nameof(GrantApplication)} cannot be null.");
			return CalculateRoundedReimbursementAmount(eligibleCost.EstimatedCost, eligibleCost.TrainingCost.GrantApplication.ReimbursementRate);
		}

		/// <summary>
		/// Calculate the estimated employer contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedEmployerContribution(this EligibleCost eligibleCost)
		{
			return Math.Round(eligibleCost.EstimatedCost - eligibleCost.CalculateEstimatedReimbursement(), 2);
		}

		/// <summary>
		/// Calculate the estimated cost per participant.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedParticipantCost(this EligibleCost eligibleCost)
		{
			return CalculatePerParticipantCost(eligibleCost.EstimatedCost, eligibleCost.EstimatedParticipants);
		}

		/// <summary>
		/// Calculate the estimated cost.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimateCost(this EligibleCost eligibleCost)
		{
			if (eligibleCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)} cannot be null.");

			return Math.Round(eligibleCost.Breakdowns.Sum(x => x.EstimatedCost), 2);
		}

		/// <summary>
		/// Calculate the estimated cost.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedMaxCost(this EligibleCost eligibleCost)
		{
			if (eligibleCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)} cannot be null.");

			return Math.Round(eligibleCost.Breakdowns.Sum(x => x.AssessedCost), 2);
		}

		/// <summary>
		/// Calculate the agreed cost per participant.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedParticipantCost(this EligibleCost eligibleCost)
		{
			return CalculatePerParticipantCost(eligibleCost.AgreedMaxCost, eligibleCost.AgreedMaxParticipants);
		}

		/// <summary>
		/// Calculate the agreed requested government contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedReimbursement(this EligibleCost eligibleCost)
		{
			if (eligibleCost.TrainingCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)}.{nameof(TrainingCost)}.{nameof(GrantApplication)} cannot be null.");

			return CalculateRoundedReimbursementAmount(eligibleCost.AgreedMaxCost, eligibleCost.TrainingCost.GrantApplication.ReimbursementRate);
		}

		/// <summary>
		/// Calculate the agreed per participant reimbursement amount.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedPerParticipantReimbursement(this EligibleCost eligibleCost)
		{
			if (eligibleCost.TrainingCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)}.{nameof(TrainingCost)}.{nameof(GrantApplication)} cannot be null.");
			return CalculatePerParticipantReimbursement(eligibleCost.AgreedMaxReimbursement, eligibleCost.AgreedMaxParticipants);
		}

		/// <summary>
		/// Calculate the agreed employer contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedEmployerContribution(this EligibleCost eligibleCost)
		{
			return Math.Round(eligibleCost.AgreedMaxCost - eligibleCost.CalculateAgreedReimbursement(), 2);
		}

		/// <summary>
		/// Calculate the agreed employer contribution per participant
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedPerParticipantEmployerContribution(this EligibleCost eligibleCost)
		{
			return eligibleCost.AgreedMaxParticipantCost - CalculateAgreedPerParticipantReimbursement(eligibleCost);
		}

		/// <summary>
		/// Calculate the estimated participant cost, reimbursement and employer contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		public static void RecalculateEstimatedCost(this EligibleCost eligibleCost)
		{
			eligibleCost.EstimatedParticipantCost = eligibleCost.CalculateEstimatedParticipantCost();
			eligibleCost.EstimatedReimbursement = eligibleCost.CalculateEstimatedReimbursement();
			eligibleCost.EstimatedEmployerContribution = eligibleCost.CalculateEstimatedEmployerContribution();

			if (eligibleCost.TrainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{

				eligibleCost.EstimatedEmployerContribution = eligibleCost.EstimatedCost - eligibleCost.EstimatedReimbursement;
			}
		}

		/// <summary>
		/// Calculate the agreed participant cost, reimbursement and employer contribution.
		/// </summary>
		/// <param name="eligibleCost"></param>
		public static void RecalculateAgreedCosts(this EligibleCost eligibleCost)
		{
			eligibleCost.AgreedMaxParticipantCost = eligibleCost.CalculateAgreedParticipantCost();
			eligibleCost.AgreedMaxReimbursement = eligibleCost.CalculateAgreedReimbursement();
			eligibleCost.AgreedEmployerContribution = eligibleCost.CalculateAgreedEmployerContribution();

			if (eligibleCost.TrainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{

				eligibleCost.AgreedEmployerContribution = eligibleCost.AgreedMaxCost - eligibleCost.AgreedMaxReimbursement;
			}
		}

		/// <summary>
		/// Calculates the estimated cost limit based on the elgible expense type and rate.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedCostLimit(this TrainingCost trainingCost, EligibleCost eligibleCost)
		{
			if (eligibleCost == null)
				throw new ArgumentNullException(nameof(eligibleCost), $"The argument {nameof(eligibleCost)} cannot be null.");

			// Only AutoLimitEstimatedCosts are valid expense types to use this function.
			if (!eligibleCost.EligibleExpenseType?.ExpenseTypeId.In(ExpenseTypes.AutoLimitEstimatedCosts) ?? false)
				throw new InvalidOperationException("Invalid eligible expense type.");

			return Math.Round((trainingCost.EligibleCosts.Sum(x => x.EstimatedCost) - eligibleCost.EstimatedCost) * (decimal)(eligibleCost.EligibleExpenseType?.Rate ?? 0), 2);
		}

		/// <summary>
		/// Calculate the total estimated cost.  Sum all eligible costs.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculatedTotalEstimatedCost(this TrainingCost trainingCost)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost), $"The argument {nameof(trainingCost)} cannot be null.");

			return Math.Round(trainingCost.EligibleCosts.Sum(x => x.EstimatedCost), 2);
		}

		/// <summary>
		/// Calculate the total estimated reimbursement cost.  Sum all eligible reimbursement costs.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculateTotalEstimatedReimbursement(this TrainingCost trainingCost)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost), $"The argument {nameof(trainingCost)} cannot be null.");

			return trainingCost.EligibleCosts.Sum(x => x.EstimatedReimbursement);
		}

		
		/// <summary>
		/// Calculate the approved amount based on current application status.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculateApprovedAmount(this TrainingCost trainingCost)
		{
			var approvedStatuses = new Dictionary<ApplicationStateInternal, bool>
			{
				[ApplicationStateInternal.Draft] = false,
				[ApplicationStateInternal.New] = false,
				[ApplicationStateInternal.PendingAssessment] = false,
				[ApplicationStateInternal.UnderAssessment] = false,
				[ApplicationStateInternal.ReturnedToAssessment] = false,
				[ApplicationStateInternal.RecommendedForApproval] = false,
				[ApplicationStateInternal.RecommendedForDenial] = false,
				[ApplicationStateInternal.OfferIssued] = true,
				[ApplicationStateInternal.OfferWithdrawn] = false,
				[ApplicationStateInternal.AgreementAccepted] = true,
				[ApplicationStateInternal.Unfunded] = false,
				[ApplicationStateInternal.ApplicationDenied] = false,
				[ApplicationStateInternal.AgreementRejected] = false,
				[ApplicationStateInternal.ApplicationWithdrawn] = false,
				[ApplicationStateInternal.CancelledByMinistry] = false,
				[ApplicationStateInternal.CancelledByAgreementHolder] = false,
				[ApplicationStateInternal.ChangeRequest] = true,
				[ApplicationStateInternal.ChangeForApproval] = true,
				[ApplicationStateInternal.ChangeForDenial] = true,
				[ApplicationStateInternal.ChangeReturned] = true,
				[ApplicationStateInternal.ChangeRequestDenied] = true,
				[ApplicationStateInternal.NewClaim] = true,
				[ApplicationStateInternal.ClaimAssessEligibility] = true,
				[ApplicationStateInternal.ClaimReturnedToApplicant] = true,
				[ApplicationStateInternal.ClaimDenied] = false,
				[ApplicationStateInternal.ClaimApproved] = true,
				[ApplicationStateInternal.ReturnedUnassessed] = false,
				[ApplicationStateInternal.Closed] = true,
				[ApplicationStateInternal.ClaimAssessReimbursement] = true,
				[ApplicationStateInternal.CompletionReporting] = true
			};

			var applicationStatus = trainingCost.GrantApplication.ApplicationStateInternal;

			if (approvedStatuses.ContainsKey(applicationStatus) && approvedStatuses[applicationStatus])
			{
				return trainingCost.AgreedCommitment;
			}

			return decimal.Zero;
		}

		/// <summary>
		/// Get estimated participant.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static int GetEstimatedParticipants(this TrainingCost trainingCost)
		{
			if (trainingCost.EstimatedParticipants == 0)
				return 1;

			return trainingCost.EstimatedParticipants;
		}
		/// <summary>
		/// Calculate the total agreed cost.  Sum all eligible costs.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculatedTotalAgreedMaxCost(this TrainingCost trainingCost)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost), $"The argument {nameof(trainingCost)} cannot be null.");

			return Math.Round(trainingCost.EligibleCosts.Sum(x => x.AgreedMaxCost), 2);
		}

		/// <summary>
		/// Calculate the total agreed cost of all eligible costs.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedMaxReimbursement(this TrainingCost trainingCost)
		{
			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				return trainingCost.AgreedCommitment;
			}

			return trainingCost.EligibleCosts.Sum(ec => ec.AgreedMaxReimbursement);
		}

		/// <summary>
		/// Calculate the total agreed employer contribution of all eligible costs.
		/// </summary>s
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedEmployerContribution(this TrainingCost trainingCost)
		{
			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				return trainingCost.TotalAgreedMaxCost - trainingCost.AgreedCommitment;
			}

			return trainingCost.EligibleCosts.Sum(ec => ec.AgreedEmployerContribution);
		}

		/// <summary>
		/// Calculate the total estimated reimbursement costs for the service type.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateServiceTypeTotal(this TrainingCost trainingCost, ServiceTypes serviceType)
		{
			return trainingCost.EligibleCosts.Where(x => x.EligibleExpenseType.ServiceCategory.ServiceTypeId == serviceType).Sum(ec => ec.EstimatedReimbursement);
		}

		/// <summary>
		/// Compare new ServiceType cost to old and compare against the limit. Returns true if limit exceeded.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static bool HasExceededServiceLimit(this TrainingCost trainingCost, ServiceTypes serviceType, int newParticipants, decimal serviceLimitPerParticipant)
		{
			var serviceTotal = CalculateServiceTypeTotal(trainingCost, serviceType);
			var newServiceTotalPerParticipant = CalculatePerParticipantCost(serviceTotal, newParticipants);
			if (newServiceTotalPerParticipant > serviceLimitPerParticipant)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Recalculate the estimated costs when the number of participants is changed.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <param name="numberOfParticiants"></param>
		public static void RecalculateEstimatedCostsFor(this TrainingCost trainingCost, int numberOfParticiants)
		{
			if (trainingCost == null || trainingCost.GrantApplication == null) throw new ArgumentNullException(nameof(trainingCost));
			if (numberOfParticiants < 0) throw new ArgumentException($"Argument '{nameof(numberOfParticiants)}' cannot be less than 0.");

			var grantApplication = trainingCost.GrantApplication;
			var skillsTrainingMaxCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
			var ESSMaxParticipantCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
			var hasExceededSkillsTrainingLimit = grantApplication.TrainingCost.HasExceededServiceLimit(ServiceTypes.SkillsTraining, numberOfParticiants, skillsTrainingMaxCostPerParticipant);
			var hasExceededESSLimit = grantApplication.TrainingCost.HasExceededServiceLimit(ServiceTypes.EmploymentServicesAndSupports, numberOfParticiants, ESSMaxParticipantCostPerParticipant);

			grantApplication.TrainingCost.EstimatedParticipants = numberOfParticiants;
			grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Incomplete;

			if (hasExceededSkillsTrainingLimit || hasExceededESSLimit)
			{
				foreach (var program in grantApplication.TrainingPrograms)
				{
					if (program.EligibleCostBreakdown?.EligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining && hasExceededSkillsTrainingLimit
						|| program.EligibleCostBreakdown?.EligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && hasExceededESSLimit)
					{
						// If the number of participants changed and the costs must be reentered.
						program.TrainingProgramState = TrainingProgramStates.Incomplete;
					}
				}
			}

			foreach (var cost in grantApplication.TrainingCost.EligibleCosts)
			{
				if (cost.EligibleExpenseType.ExpenseTypeId != ExpenseTypes.ParticipantAssigned)
				{
					cost.EstimatedParticipants = numberOfParticiants;
				}

				// If the limits have been exceed they will need to be reset to $0.
				if (cost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining && hasExceededSkillsTrainingLimit
					|| cost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && hasExceededESSLimit)
				{
					cost.EstimatedCost = 0;
					foreach (var costBreakdown in cost.Breakdowns)
					{
						costBreakdown.EstimatedCost = 0;
					}
				}

				cost.RecalculateEstimatedCost();
			}
			grantApplication.TrainingCost.RecalculateEstimatedCosts();
		}

		/// <summary>
		/// Reset the eligible estimated cost calculations.
		/// This will also set all agreed values to 0.
		/// </summary>
		/// <param name="trainingCost"></param>
		public static void ResetEstimatedCosts(this TrainingCost trainingCost)
		{
			foreach (var item in trainingCost.EligibleCosts)
			{
				item.AgreedEmployerContribution = 0;
				item.AgreedMaxCost = 0;
				item.AgreedMaxParticipantCost = 0;
				item.AgreedMaxParticipants = 0;
				item.AgreedMaxReimbursement = 0;

				foreach (var breakdown in item.Breakdowns)
				{
					breakdown.AssessedCost = 0;
				}
			}

			trainingCost.AgreedParticipants = 0;
			trainingCost.TotalAgreedMaxCost = 0;
			trainingCost.AgreedCommitment = 0;

			trainingCost.RecalculateEstimatedCosts();
		}

		/// <summary>
		/// Calculate the estimated costs for the specified training program.
		/// </summary>
		/// <param name="trainingCost"></param>
		public static void RecalculateEstimatedCosts(this TrainingCost trainingCost)
		{
			foreach (var item in trainingCost.EligibleCosts)
			{
				item.RecalculateEstimatedCost();
			}

			trainingCost.TotalEstimatedCost = trainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost);

			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				var maxReimbursementAmount = trainingCost.GrantApplication.MaxReimbursementAmt;
				var rate = trainingCost.GrantApplication.ReimbursementRate;

				var perParticipantCost = CalculatePerParticipantCost(Math.Round(trainingCost.TotalEstimatedCost * (decimal)rate, 2), trainingCost.EstimatedParticipants);
				var estimatedReimbursement = perParticipantCost >= maxReimbursementAmount ? maxReimbursementAmount : perParticipantCost;
				trainingCost.TotalEstimatedReimbursement = estimatedReimbursement * trainingCost.EstimatedParticipants;
			}
			else
			{
				trainingCost.TotalEstimatedReimbursement = trainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement);
			}
		}

		/// <summary>
		/// Calculate the agreed cots for the specified training program.
		/// </summary>
		/// <param name="trainingCost"></param>
		public static void RecalculateAgreedCosts(this TrainingCost trainingCost)
		{
			trainingCost.TotalAgreedMaxCost = trainingCost.EligibleCosts.Sum(ec => ec.AgreedMaxCost);
			trainingCost.AgreedCommitment = trainingCost.EligibleCosts.Sum(ec => ec.AgreedMaxReimbursement);
			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				var maxReimbursementAmount = trainingCost.GrantApplication.MaxReimbursementAmt;
				var rate = trainingCost.GrantApplication.ReimbursementRate;

				var perParticipantCost = CalculatePerParticipantCost(Math.Round(trainingCost.TotalAgreedMaxCost * (decimal)rate, 2), trainingCost.AgreedParticipants);
				var reimbursementAmount = perParticipantCost >= maxReimbursementAmount ? maxReimbursementAmount : perParticipantCost;
				trainingCost.AgreedCommitment = reimbursementAmount * trainingCost.AgreedParticipants;
			}
		}

		/// <summary>
		/// Copy the estimated values into the agreed values.
		/// </summary>
		/// <param name="trainingCost"></param>
		public static void CopyEstimatedIntoAgreed(this TrainingCost trainingCost)
		{
			trainingCost.AgreedParticipants = trainingCost.EstimatedParticipants;
			trainingCost.TotalAgreedMaxCost = trainingCost.TotalEstimatedCost;
			trainingCost.AgreedCommitment = trainingCost.TotalEstimatedReimbursement;

			foreach (var ec in trainingCost.EligibleCosts)
			{
				ec.AgreedMaxParticipants = ec.EstimatedParticipants;
				ec.AgreedMaxCost = ec.EstimatedCost;
				ec.AgreedMaxParticipantCost = ec.EstimatedParticipantCost;
				ec.AgreedEmployerContribution = ec.EstimatedEmployerContribution;
				ec.AgreedMaxReimbursement = ec.EstimatedReimbursement;

				foreach (var breakdown in ec.Breakdowns)
				{
					breakdown.AssessedCost = breakdown.EstimatedCost;
				}
			}
		}

		/// <summary>
		/// Get the maximum amount of participants based on the grant application state.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static int GetMaxParticipants(this TrainingCost trainingCost)
		{
			var grantApplication = trainingCost.GrantApplication;
			var stateInternal = grantApplication.ApplicationStateInternal;

			return stateInternal == ApplicationStateInternal.OfferIssued || stateInternal == ApplicationStateInternal.AgreementAccepted ||
				stateInternal == ApplicationStateInternal.AgreementRejected || stateInternal > ApplicationStateInternal.CancelledByMinistry
				? trainingCost.AgreedParticipants
				: trainingCost.EstimatedParticipants;
		}

		/// <summary>
		/// Calculate the total agreed cost of all eligible costs less any travel expenses.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal AgreedNonTravelGovernmentReimbursement(this TrainingCost trainingCost)
		{
			return trainingCost.EligibleCosts.Where(ec => !ec.EligibleExpenseTypeId.In(7, 8, 9)).Sum(ec => ec.AgreedMaxReimbursement);
		}


		/// <summary>
		/// Calculate the total agreed cost of all eligible costs less any travel expenses.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <returns></returns>
		public static decimal EstimatedNonTravelGovernmentReimbursement(this TrainingCost trainingCost)
		{
			return trainingCost.EligibleCosts.Where(ec => !ec.EligibleExpenseTypeId.In(7, 8, 9)).Sum(ec => ec.EstimatedReimbursement);
		}

		/// <summary>
		/// Check that agreed commitments cannot exceed 10% of estimated contribution unless the user is a Director
		/// </summary>
		/// <param name="trainingCost"></param>
		public static bool DoesAgreedCommitmentExceedEstimatedContribution(this TrainingCost trainingCost)
		{
			var totalAgreedCommitment = trainingCost.EligibleCosts.Sum(ec => ec.AgreedMaxReimbursement);
			var totalEstimatedReimbursement = trainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement);

			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
			    totalAgreedCommitment = trainingCost.AgreedCommitment;
				totalEstimatedReimbursement = trainingCost.TotalEstimatedReimbursement;
			}

			return totalAgreedCommitment > totalEstimatedReimbursement * 1.1M;
		}
	}
}
