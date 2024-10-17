using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Application.Business.Models
{
	public class ClaimEligibleCostBreakdownModel
	{
		#region Properties
		public int Id { get; set; }
		public string BreakdownCaption { get; set; }
		public decimal ClaimCost { get; set; }
		public decimal TotalClaimedToDate { get; set; }
		public decimal AssessedCost { get; set; }
		public string RowVersion { get; set; }
		public bool AddedByAssessor { get; set; }
		#endregion

		#region Constructors
		public ClaimEligibleCostBreakdownModel()
		{

		}

		public ClaimEligibleCostBreakdownModel(ClaimBreakdownCost claimBreakdownCost)
		{
			if (claimBreakdownCost == null)
				throw new ArgumentNullException(nameof(claimBreakdownCost));

			this.Id = claimBreakdownCost.Id;
			this.BreakdownCaption = claimBreakdownCost.EligibleCostBreakdown.TrainingPrograms.FirstOrDefault().CourseTitle;
			this.ClaimCost = claimBreakdownCost.ClaimCost;
			this.AssessedCost = claimBreakdownCost.AssessedCost;
			this.RowVersion = Convert.ToBase64String(claimBreakdownCost.RowVersion);

			// get the current claim
			var currentClaim = claimBreakdownCost.ClaimEligibleCost.Claim;
			var grantApplication = currentClaim.GrantApplication;

			// get all claims created before the current claim using Id as the comparision point
			// as there could be multiple versions for the same claim, a groupby operation is used here to return the unique claim Ids.
			var allPreviousClaims = grantApplication.Claims.Where(x => x.Id < currentClaim.Id).GroupBy(x => x.Id).Select(x => new
			{
				ClaimId = x.Key,
				ClaimVersion = x.Max(y => y.ClaimVersion),
			}).ToList();


			// calculate the total of assessed (not claimed) value for all claims in approved or post approved.
			this.TotalClaimedToDate = grantApplication.Claims.Where(x => allPreviousClaims.Any(y => y.ClaimId == x.Id && y.ClaimVersion == x.ClaimVersion) &&
				 x.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.SelectMany(c => c.EligibleCosts)
				.SelectMany(ec => ec.Breakdowns.Where(b => b.EligibleCostBreakdownId == claimBreakdownCost.EligibleCostBreakdownId)).Sum(b => b.AssessedCost);
		}
		#endregion
	}
}
