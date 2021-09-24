using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationClaimReportRules enum, provides a way to identify notification claim reporting rules.
	/// </summary>
	public enum NotificationClaimReportRules
	{
		/// <summary>
		/// NotApplicable - Claim not applicable.
		/// </summary>
		[Description("Not Applicable")]
		NotApplicable = 0,
		/// <summary>
		/// NoneReported - No claims were reported.
		/// </summary>
		[Description("None Reported")]
		NoneReported = 1,
		/// <summary>
		/// ClaimReported - A claim has been reported.
		/// </summary>
		[Description("Claim Reported")]
		ClaimReported = 2,
		/// <summary>
		/// FinalClaimReported - Final claim has been reported.
		/// </summary>
		[Description("Final Claim Reported")]
		FinalClaimReported = 3,
		/// <summary>
		/// AmountRemaining - Claim amount remaining.
		/// </summary>
		[Description("Amount Remaining")]
		AmountRemaining = 4,
		/// <summary>
		/// AmountOwing - Amount still owing on claim.
		/// </summary>
		[Description("Amount Owing")]
		AmountOwing = 5,
		/// <summary>
		/// PaymentRequestIssued - A payment request was issued for the current claim.
		/// </summary>
		[Description("Payment Request Issued")]
		PaymentRequestIssued = 6,
		/// <summary>
		/// ClaimPaid - The current claim was paid.
		/// </summary>
		[Description("Claim Paid")]
		ClaimPaid = 7
	}
}
