using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationApprovalRules enum, provides a way to identify when to approve the notification.
	/// </summary>
	public enum NotificationApprovalRules
	{
		/// <summary>
		/// NotApplicable - Notification approval is not applicable.
		/// </summary>
		[Description("Not Applicable")]
		NotApplicable = 0,
		/// <summary>
		/// OfferIssued - Notification is approved when offer is issued.
		/// </summary>
		[Description("Offer Issued")]
		OfferIssued = 1,
		/// <summary>
		/// AgreementAccepted - Notification is approved when the agreement is accepted.
		/// </summary>
		[Description("Agreement Accepted")]
		AgreementAccepted = 2
	}
}
