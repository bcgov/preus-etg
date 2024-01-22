using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Application.Services
{
	public class FinanceInformationService : IFinanceInformationService
	{
		public (int NumberOfApplications, decimal TotalPaid) GetYearToDatePaidFor(IEnumerable<GrantApplication> applications)
		{
			var grantApplications = applications.ToList();
			if (!grantApplications.Any())
				return (0, 0);


			var applicationClaims = grantApplications
				.SelectMany(ga => ga.Claims)
				.Where(q => q.ClaimState
					.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.ClaimPaid, ClaimState.AmountReceived))
				.ToList();

			// Sum the two claim types, SingleAmendableClaim and the rest.
			var singleAmendablePayments = applicationClaims.Where(c => c.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
				.Sum(q => q.TotalAssessedReimbursement
				          - q.GrantApplication.PaymentRequests
					          .Where(o => o.ClaimVersion != q.ClaimVersion)
					          .Sum(o => o.PaymentAmount));

			var totalAmendablePayments = applicationClaims
				.Where(c => c.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
				.Sum(q => q.TotalAssessedReimbursement);

			var numberOfApplication = grantApplications.Count();
			var totalPaid = singleAmendablePayments + totalAmendablePayments;

			return (numberOfApplication, totalPaid);
		}
	}
}