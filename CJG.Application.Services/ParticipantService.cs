using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class ParticipantService : Service, IParticipantService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ParticipantService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add the specified <typeparamref name="ParticipantForm"/> if the invitation has not expired.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">The invitation has expired.</exception>
		/// <param name="participantForm"></param>
		public ParticipantForm Add(ParticipantForm participantForm)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			var grantApplication = Get<GrantApplication>(participantForm.GrantApplicationId);

			if (grantApplication?.InvitationKey == null || grantApplication.IsInvitationExpired())
				throw new InvalidOperationException("The invitation key is invalid or has expired.");

			_dbContext.ParticipantForms.Add(participantForm);

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);

				var programType = grantApplication.GetProgramType();
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					// Add the participant to each eligible expense that must be participant assigned.
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						var newParticipantCost = new ParticipantCost(claimEligibleCost, participantForm);
						claimEligibleCost.ParticipantCosts.Add(newParticipantCost);
					}
					else if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited && programType == ProgramTypes.WDAService)
					{
						// Only allow the maximum agreed participants.
						if (numberOfParticipants > grantApplication.TrainingCost.AgreedParticipants)
						{
							if (grantApplication.TrainingCost.AgreedParticipants != claimEligibleCost.ClaimParticipants)
							{
								claimEligibleCost.ClaimParticipants = grantApplication.TrainingCost.AgreedParticipants;
							}
						}
						else if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
						{
							claimEligibleCost.ClaimParticipants = numberOfParticipants;
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts(true);
			}

			_dbContext.CommitTransaction();
			return participantForm;
		}

		public ParticipantCost Add(ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			_dbContext.ParticipantCosts.Add(participantCost);
			_dbContext.Commit();

			return participantCost;
		}

		public ParticipantCost Update(ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			_dbContext.Update<ParticipantCost>(participantCost);
			_dbContext.Commit();

			return participantCost;
		}

		/// <summary>
		/// Get the participant form for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ParticipantForm Get(int id)
		{
			var participant = Get<ParticipantForm>(id);

			if (!_httpContext.User.CanPerformAction(participant.GrantApplication, ApplicationWorkflowTrigger.ViewParticipants))
				throw new NotAuthorizedException($"User does not have permission to view participants from grant application '{participant.GrantApplication}'.");

			return participant;
		}

		/// <summary>
		/// Get all the <typeparamref name="ParticipantForm"/> objects for the specified <typeparamref name="GrantApplciation"/>.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		public IEnumerable<ParticipantForm> GetParticipantFormsForGrantApplication(int grantApplicationId)
		{
			return Get<GrantApplication>(grantApplicationId).ParticipantForms.ToArray();
		}

		public IEnumerable<ParticipantCost> GetParticipantCostsForClaimEligibleCost(int eligibleCostId)
		{
			return _dbContext.ParticipantCosts.Where(pc => pc.ClaimEligibleCostId == eligibleCostId);
		}

		public IEnumerable<ParticipantCost> GetParticipantCosts(ClaimEligibleCost eligibleCost)
		{
			return _dbContext.ParticipantCosts.Where(pc => pc.ClaimEligibleCostId == eligibleCost.Id);
		}

		/// <summary>
		/// The number of participants that have costs associated with the specified Claim.
		/// </summary>
		/// <param name="claimId">The id of the Claim.</param>
		/// <param name="claimVersion">The version of the Claim.</param>
		/// <returns>Number of participants that have costs associated with the specified Claim.</returns>
		public int GetParticipantsWithClaimEligibleCostCount(int claimId, int claimVersion)
		{
			var claim = _dbContext.Claims.Include(c => c.EligibleCosts).Include("EligibleCosts.ParticipantCosts").FirstOrDefault(c => c.Id == claimId && c.ClaimVersion == claimVersion);

			if (claim == null)
				throw new InvalidOperationException("Claim version does not exist.");

			return claim.ParticipantsWithEligibleCosts();
		}

		/// <summary>
		/// Get enrollments for Unemployed participant who are receiving EI
		/// </summary>
		/// <param name="currentDate"></param>
		/// <returns>Collection of ParticipantEnrollment</returns>
		public IEnumerable<ParticipantForm> GetUnemployedParticipantEnrollments(DateTime currentDate, int take)
		{
			var universalTime = currentDate.ToUniversalTime();

			// Unemployed && Currently receiving EI benefits
			return _dbContext.ParticipantForms
				.Where(x => x.EmploymentStatusId == 1 && x.EIBenefitId == 1 && (!x.ReportedOn.HasValue || x.ReportedOn > universalTime))
				.Include(x => x.GrantApplication)
				.Take(take);
		}

		/// <summary>
		/// Update ParticipantEnrolments with date when they were reported 
		/// </summary>
		/// <param name="participantEnrollments"></param>
		/// <param name="reportedDate"></param>
		public void UpdateReportedDate(IEnumerable<ParticipantForm> participantEnrollments, DateTime reportedDate)
		{
			foreach (var participantEnrollment in participantEnrollments)
			{
				participantEnrollment.ReportedOn = reportedDate.ToUniversalTime();
			}

			_dbContext.Commit();
		}

		/// <summary>
		/// Remove the participant from the grant application.
		/// Remove any costs associated with them.
		/// Remove an completion report answers assocaited with them.
		/// </summary>
		/// <param name="participant"></param>
		public void RemoveParticipant(ParticipantForm participant)
		{
			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("Can only remove participants if there are no claims in a Denied or Approved state and there is only one existing claim in the Incomplete, Complete or ClaimReturnedToApplication state.");

			if (participant.ClaimReported)
				throw new NotAuthorizedException($"The participant may not be removed from this application, it may be part of an prior claim.");

			participant.IsExcludedFromClaim = true;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				// Remove all participant costs associated with this participant.
				var participantCosts = _dbContext.ParticipantCosts.Where(pc => pc.ParticipantFormId == participant.Id && pc.ClaimEligibleCost.ClaimId == claim.Id && pc.ClaimEligibleCost.ClaimVersion == claim.ClaimVersion);
				foreach (var participantCost in participantCosts)
				{
					_dbContext.ParticipantCosts.Remove(participantCost);
				}


				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					// WDA Services must sync the number of participants with each eligible expense.
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited && grantApplication.GetProgramType() == ProgramTypes.WDAService)
					{
						if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
						{
							// Re-count the number of participants reported so far, for example 5
							claimEligibleCost.UpdateUpToMaxClaimParticipants(numberOfParticipants);
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();

				_dbContext.Update(claim);

				// CJG-476 - remove any related claim participants prior to removing the particpant form
				var claimWithParticipants = _dbContext.Claims
					.Include(c => c.ParticipantForms)
					.FirstOrDefault(c => c.Id == claim.Id && c.ClaimVersion == claim.ClaimVersion);

				if (claimWithParticipants?.ParticipantForms?.Any() == true)
				{
					foreach (var participantForm in claimWithParticipants.ParticipantForms.ToArray())
					{
						claimWithParticipants.ParticipantForms.Remove(participantForm);
						_dbContext.ParticipantForms.Remove(participantForm);
					}
				}

				//if (_dbContext.ClaimParticipants != null)
				//{
				//	var claimParticipants = _dbContext.ClaimParticipants.Where(cp => cp.ClaimId == claim.Id
				//																	&& cp.ClaimVersion == claim.ClaimVersion
				//																	&& cp.ParticipantFormId == participant.Id);
				//	if (claimParticipants.Any())
				//	{
				//		_dbContext.ClaimParticipants.RemoveRange(claimParticipants);
				//	}
				//}
		}

		// also need to check to see if this participant has been included on a completion report, in which case their answers need to be removed
		// get all of the answers submitted by the participant
		var participantAnswers = _dbContext.ParticipantCompletionReportAnswers.Where(pcra => pcra.ParticipantFormId == participant.Id);
			foreach (var participantAnswer in participantAnswers)
			{
				_dbContext.ParticipantCompletionReportAnswers.Remove(participantAnswer);
			}

			participant.EligibleCostBreakdowns.Clear();


			_dbContext.ParticipantForms.Remove(participant);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Include the participant and update the current claim.
		/// </summary>
		/// <param name="participant"></param>
		public void IncludeParticipant(ParticipantForm participant)
		{
			if (participant == null) throw new ArgumentNullException(nameof(participant));

			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("User is not allowed to update participants.");

			participant.IsExcludedFromClaim = false;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						// Only add them if they aren't already added.
						var costs = claimEligibleCost.ParticipantCosts.Any(pc => pc.ParticipantFormId == participant.Id);
						if (!costs)
						{
							var participantCost = new ParticipantCost(claimEligibleCost, participant.Id);
							claimEligibleCost.ParticipantCosts.Add(participantCost);
						}
					}
					// WDA Services must sync the number of participants with each eligible expense.
					else if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited && grantApplication.GetProgramType() == ProgramTypes.WDAService)
					{
						if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
						{
							// Re-count the number of participants reported so far, for example 5
							claimEligibleCost.UpdateUpToMaxClaimParticipants(numberOfParticipants);
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();
			}

			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Exclude the participant and update the current claim.
		/// </summary>
		/// <param name="participant"></param>
		public void ExcludeParticipant(ParticipantForm participant)
		{
			if (participant == null) throw new ArgumentNullException(nameof(participant));

			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("User is not allowed to update participants.");

			participant.IsExcludedFromClaim = true;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						// Remove participant costs from the claim.
						var participantCostIds = claimEligibleCost.ParticipantCosts.Where(pc => pc.ParticipantFormId == participant.Id).Select(pc => pc.Id).ToArray(); // It should only return one, but the database supports multiple.
						var participantCosts = _dbContext.ParticipantCosts.Where(pc => participantCostIds.Contains(pc.Id));
						participantCosts.ForEach(participantCost =>
						{
							Remove(participantCost);
						});
					}
					// WDA Services must sync the number of participants with each eligible expense.
					else if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited && grantApplication.GetProgramType() == ProgramTypes.WDAService)
					{
						if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
						{
							// Re-count the number of participants reported so far, for example 5
							claimEligibleCost.UpdateUpToMaxClaimParticipants(numberOfParticipants);
							// Re-calculate the maximum cost per participant. for example 400/5 = 80 assuming 400 is the total cost
							claimEligibleCost.ClaimMaxParticipantCost = claimEligibleCost.CalculateClaimParticipantCost();
							// Re-calculate the maximum participant reimbursement cost, for example 24 assuming 30% is the reimbursement rate
							claimEligibleCost.ClaimMaxParticipantReimbursementCost = claimEligibleCost.CalculateClaimMaxParticipantReimbursement();
							// Re-calculate employer contribution, for example 56 
							claimEligibleCost.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimMaxParticipantCost - claimEligibleCost.ClaimMaxParticipantReimbursementCost;
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();
			}

			_dbContext.CommitTransaction();
		}
		#endregion
	}
}
