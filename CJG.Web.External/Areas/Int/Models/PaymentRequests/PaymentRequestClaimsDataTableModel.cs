using System;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Int.Models.PaymentRequests
{
	public class PaymentRequestClaimsDataTableModel
	{
		public string ClaimNumber { get; set; }
		public string ClaimId { get; set; }
		public string ClaimVersion { get; set; }
		public string Applicant { get; set; }
		public string Assessed { get; set; }
		public string ClaimState { get; set; }
		public PaymentTypes Type { get; set; }
		public string TypeName { get; set; }
		public string Amount { get; set; }

		public PaymentRequestClaimsDataTableModel(Claim claim, DateTime fiscalYearStartDate)
		{
			ClaimNumber = claim.ClaimNumber;
			ClaimId = claim.Id.ToString();
			ClaimVersion = claim.ClaimVersion.ToString();
			Applicant = claim.GrantApplication.OrganizationLegalName;
			Assessed = claim.TotalAssessedReimbursement.ToDollarCurrencyString();
			ClaimState = claim.ClaimState.GetDescription();

			var paymentAmount = Math.Abs(claim.AmountPaidOrOwing());
			Amount = paymentAmount.ToDollarCurrencyString();
			Type = paymentAmount == 0
				 ? PaymentTypes.None
				 : claim.GrantApplication.StartDate < fiscalYearStartDate
				 ? PaymentTypes.Accrual
				 : PaymentTypes.Normal;
			TypeName = Type.GetDescription();
		}
	}
}
