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

		// TODO: Rip this out and replace it with a standard entity function.
		/// <summary>
		/// Update all eligible claim costs.
		/// </summary>
		/// <param name="eligibleCosts"></param>
		public void Update(List<ClaimEligibleCostModel> eligibleCosts)
		{
			var anyClaimEligibleCost = Get<ClaimEligibleCost>(eligibleCosts.FirstOrDefault().Id);

			if (!_httpContext.User.CanPerformAction(anyClaimEligibleCost.Claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException($"User does not have permission to access Grant Application '{anyClaimEligibleCost.Claim.GrantApplication.Id}'.");

			var isExternalUser = _httpContext.User.GetAccountType() == AccountTypes.External;
			var claim = Get<Claim>(anyClaimEligibleCost.ClaimId, anyClaimEligibleCost.ClaimVersion);

			foreach (var viewModelClaimEligibleCost in eligibleCosts)
			{
				claim.RowVersion = System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion);
				var claimEligibleCost = Get<ClaimEligibleCost>(viewModelClaimEligibleCost.Id);

				// If RowVersion is different, update claim participants, update row version to match, and set error to true
				if (!claimEligibleCost.Claim.RowVersion.SequenceEqual(System.Convert.FromBase64String(viewModelClaimEligibleCost.ClaimRowVersion))) {
					viewModelClaimEligibleCost.ClaimParticipants = claimEligibleCost.ClaimParticipants;
					viewModelClaimEligibleCost.ClaimCost = claimEligibleCost.ClaimCost;
					foreach(var breakdown in viewModelClaimEligibleCost.Breakdowns) {
						breakdown.ClaimCost = claimEligibleCost.Breakdowns.FirstOrDefault(b => b.Id == breakdown.Id).ClaimCost;
					}
					viewModelClaimEligibleCost.ConcurrencyError = true;
				} else {
					if (isExternalUser) {
						claim.ClaimState = ClaimState.Complete;
						claimEligibleCost.ClaimCost = viewModelClaimEligibleCost.ClaimCost;
						// Count the number of reported participants for the specified expense types.
						claimEligibleCost.ClaimParticipants = claimEligibleCost.EligibleExpenseType.ExpenseTypeId != ExpenseTypes.NotParticipantLimited ? viewModelClaimEligibleCost.ClaimParticipants ?? 0 : _dbContext.ParticipantForms.Count(pf => pf.GrantApplicationId == claim.GrantApplicationId && !pf.IsExcludedFromClaim);
						claimEligibleCost.ClaimReimbursementCost = viewModelClaimEligibleCost.TotalClaimedReimbursement;
					} else {
						claimEligibleCost.AssessedCost = viewModelClaimEligibleCost.AssessedCost;
						claimEligibleCost.AssessedParticipants = viewModelClaimEligibleCost.AssessedParticipants;
						claimEligibleCost.RecalculateAssessedCost();
					}

					if (viewModelClaimEligibleCost.ServiceType != null && viewModelClaimEligibleCost.ServiceType != ServiceTypes.EmploymentServicesAndSupports && viewModelClaimEligibleCost.Breakdowns.Any()) {
						foreach (var original in claimEligibleCost.Breakdowns) {
							var updated = viewModelClaimEligibleCost.Breakdowns.FirstOrDefault(pc => pc.Id == original.Id);
							if (updated != null) {
								original.ClaimCost = updated.ClaimCost;
								if (!isExternalUser) {
									original.AssessedCost = updated.AssessedCost;
								}
							}
						}
						claimEligibleCost.ClaimCost = claimEligibleCost.Breakdowns.Sum(x => x.ClaimCost);
					}

					if (viewModelClaimEligibleCost.ExpenseType == ExpenseTypes.ParticipantAssigned) {
						foreach (var original in claimEligibleCost.ParticipantCosts) {
							var updated = viewModelClaimEligibleCost.ParticipantCosts.First(pc => pc.Id == original.Id);
							original.ClaimParticipantCost = updated.ClaimParticipantCost;
							if (!isExternalUser) {
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

			_claimService.Update(claim);

			eligibleCosts.Select(e => e.ClaimRowVersion = System.Convert.ToBase64String(claim.RowVersion)).ToList();
		}
		#endregion
	}
}
