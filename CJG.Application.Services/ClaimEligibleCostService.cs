using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class ClaimEligibleCostService : Service, IClaimEligibleCostService
	{
		#region Variables
		private readonly IClaimService _claimService;
		#endregion

		#region Constructors
		public ClaimEligibleCostService(IDataContext dbContext,
										HttpContextBase httpContext,
										IClaimService claimService,
										ILogger logger) : base(dbContext, httpContext, logger)
		{
			_claimService = claimService;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the eligible claim cost for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ClaimEligibleCost Get(int id)
		{
			var claimEligibleCost = Get<ClaimEligibleCost>(id);
			if (!_httpContext.User.CanPerformAction(claimEligibleCost.Claim.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException($"User does not have permission to access Grant Application '{claimEligibleCost.Claim.GrantApplication.Id}'.");

			return claimEligibleCost;
		}

		/// <summary>
		/// Update all eligible claim costs.
		/// </summary>
		/// <param name="eligibleCosts"></param>
		/// <param name="participantsPaidForExpenses"></param>
		/// <param name="participantsHaveBeenReimbursed"></param>
		public void Update(List<ClaimEligibleCostModel> eligibleCosts, bool? participantsPaidForExpenses, bool? participantsHaveBeenReimbursed)
		{
			var anyClaimEligibleCost = Get<ClaimEligibleCost>(eligibleCosts.FirstOrDefault().Id);

			if (!_httpContext.User.CanPerformAction(anyClaimEligibleCost.Claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException($"User does not have permission to access Grant Application '{anyClaimEligibleCost.Claim.GrantApplication.Id}'.");

			var isExternalUser = _httpContext.User.GetAccountType() == AccountTypes.External;
			var claim = Get<Claim>(anyClaimEligibleCost.ClaimId, anyClaimEligibleCost.ClaimVersion);

			if (claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant)
			{
				foreach (var viewModelClaimEligibleCost in eligibleCosts)
				{
					claim.RowVersion = System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion);
					var claimEligibleCost = Get<ClaimEligibleCost>(viewModelClaimEligibleCost.Id);

					// If RowVersion is different, update claim participants, update row version to match, and set error to true
					if (!claimEligibleCost.Claim.RowVersion.SequenceEqual(System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion)))
					{
						viewModelClaimEligibleCost.ClaimParticipants = claimEligibleCost.ClaimParticipants;
						viewModelClaimEligibleCost.ClaimCost = claimEligibleCost.ClaimCost;
						foreach (var breakdown in viewModelClaimEligibleCost.Breakdowns)
						{
							breakdown.ClaimCost = claimEligibleCost.Breakdowns.FirstOrDefault(b => b.Id == breakdown.Id).ClaimCost;
						}
						viewModelClaimEligibleCost.ConcurrencyError = true;
					}
					else
					{
						if (isExternalUser)
						{
							claim.ClaimState = ClaimState.Complete;
							claimEligibleCost.ClaimCost = viewModelClaimEligibleCost.ClaimCost;
							// Count the number of reported participants for the specified expense types.
							claimEligibleCost.ClaimParticipants = claimEligibleCost.EligibleExpenseType.ExpenseTypeId != ExpenseTypes.NotParticipantLimited ? viewModelClaimEligibleCost.ClaimParticipants ?? 0 : _dbContext.ParticipantForms.Count(pf => pf.GrantApplicationId == claim.GrantApplicationId && !pf.IsExcludedFromClaim);
							claimEligibleCost.ClaimReimbursementCost = viewModelClaimEligibleCost.TotalClaimedReimbursement;
						}
						else
						{
							claimEligibleCost.AssessedCost = viewModelClaimEligibleCost.AssessedCost;
							claimEligibleCost.AssessedParticipants = viewModelClaimEligibleCost.AssessedParticipants;
							claimEligibleCost.RecalculateAssessedCost();
						}

						if (viewModelClaimEligibleCost.ServiceType != null && viewModelClaimEligibleCost.ServiceType != ServiceTypes.EmploymentServicesAndSupports && viewModelClaimEligibleCost.Breakdowns.Any())
						{
							foreach (var original in claimEligibleCost.Breakdowns)
							{
								var updated = viewModelClaimEligibleCost.Breakdowns.FirstOrDefault(pc => pc.Id == original.Id);
								if (updated != null)
								{
									original.ClaimCost = updated.ClaimCost;
									if (!isExternalUser)
									{
										original.AssessedCost = updated.AssessedCost;
									}
								}
							}
							claimEligibleCost.ClaimCost = claimEligibleCost.Breakdowns.Sum(x => x.ClaimCost);
						}

						if (viewModelClaimEligibleCost.ExpenseType == ExpenseTypes.ParticipantAssigned)
						{
							foreach (var original in claimEligibleCost.ParticipantCosts)
							{
								var updated = viewModelClaimEligibleCost.ParticipantCosts.First(pc => pc.Id == original.Id);
								original.ClaimParticipantCost = updated.ClaimParticipantCost;
								if (!isExternalUser)
								{
									original.AssessedParticipantCost = updated.AssessedParticipantCost;
									original.AssessedReimbursement = updated.AssessedReimbursement;
									original.AssessedEmployerContribution = updated.AssessedEmployerContribution;
									original.RecalculatedAssessedCost();
								}

								original.RecalculateClaimCost();
							}
						}
					}
					claim.RecalculateClaimedCosts();
					claim.RecalculateAssessedCosts(viewModelClaimEligibleCost.RemoveOverride);
				}
			}
			else {
				foreach (var viewModelClaimEligibleCost in eligibleCosts)
				{
					claim.RowVersion = System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion);
					claim.ParticipantsPaidForExpenses = participantsPaidForExpenses;
					claim.ParticipantsHaveBeenReimbursed = participantsHaveBeenReimbursed;

					var claimEligibleCost = Get<ClaimEligibleCost>(viewModelClaimEligibleCost.Id);

					// If RowVersion is different, update claim participants, update row version to match, and set error to true
					if (!claimEligibleCost.Claim.RowVersion.SequenceEqual(System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion)))
					{
						viewModelClaimEligibleCost.ClaimParticipants = claimEligibleCost.ClaimParticipants;
						viewModelClaimEligibleCost.ClaimCost = claimEligibleCost.ClaimCost;
						viewModelClaimEligibleCost.ConcurrencyError = true;
					}
					else
					{
						if (isExternalUser)
						{
							claim.ClaimState = ClaimState.Complete;
							claimEligibleCost.ClaimCost = viewModelClaimEligibleCost.ClaimCost;
							// Count the number of for the specified expense types.
							var requiredPIFs = claim.GrantApplication.RequireAllParticipantsBeforeSubmission;
							claimEligibleCost.UpdateUpToMaxClaimParticipants(_dbContext.ParticipantForms.Count(pf => pf.GrantApplicationId == claim.GrantApplicationId && !pf.IsExcludedFromClaim &&
							                                                                                         ((pf.Attended.HasValue && pf.Attended.Value) || !requiredPIFs)));
							claimEligibleCost.ClaimReimbursementCost = viewModelClaimEligibleCost.TotalClaimedReimbursement;
						}
						else
						{
							claimEligibleCost.AssessedCost = viewModelClaimEligibleCost.AssessedCost;
							claimEligibleCost.AssessedParticipants = viewModelClaimEligibleCost.AssessedParticipants;
							claimEligibleCost.RecalculateAssessedCost();
						}

						if (viewModelClaimEligibleCost.ExpenseType == ExpenseTypes.ParticipantAssigned)
						{
							foreach (var original in claimEligibleCost.ParticipantCosts)
							{
								var updated = viewModelClaimEligibleCost.ParticipantCosts.First(pc => pc.Id == original.Id);
								original.ClaimParticipantCost = updated.ClaimParticipantCost;
								original.ClaimReimbursement = updated.ClaimReimbursement;
								original.ClaimEmployerContribution = updated.ClaimEmployerContribution;
								if (!isExternalUser)
								{
									original.AssessedParticipantCost = updated.AssessedParticipantCost;
									original.AssessedReimbursement = updated.AssessedReimbursement;
									original.AssessedEmployerContribution = updated.AssessedEmployerContribution;
									original.RecalculatedAssessedCost();
								}

								original.RecalculateClaimParticipantCostETG();
							}
						}
					}
					claim.RecalculateClaimedCosts();
					claim.RecalculateAssessedCosts(viewModelClaimEligibleCost.RemoveOverride);
				}
			}
			_claimService.Update(claim);

			eligibleCosts.Select(e => e.ClaimRowVersion = System.Convert.ToBase64String(claim.RowVersion)).ToList();
		}

		/// <summary>
		/// When a user updates the participant attendance (ETG) all claim amounts and costs are reset, recalculated
		/// </summary>
		/// <param name="claim"></param>
		public void ResetClaimAmounts(Claim claim)
		{			
			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException($"User does not have permission to access Grant Application '{claim.GrantApplication.Id}'.");

			var isExternalUser = _httpContext.User.GetAccountType() == AccountTypes.External;

			if (claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				//remove all existing claim costs/participant costs
				foreach (var item in claim.EligibleCosts.ToArray())
				{
					var participantCosts = item.ParticipantCosts.ToArray();

					foreach (var cost in participantCosts)
						_dbContext.ParticipantCosts.Remove(cost);
					_dbContext.ClaimEligibleCosts.Remove(item);
				}

				_claimService.Update(claim);

				// Re-Add line items to the claim for every line item in the estimate that has an AgreedMaxCost > 0.
				foreach (var eligibleCost in claim.GrantApplication.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0))
				{
					// Copy eligible cost agreed values into Claim eligible cost assessed values
					var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost);
					claim.EligibleCosts.Add(claimEligibleCost);
				}
				_claimService.Update(claim);
			}
		}
		#endregion
	}
}
