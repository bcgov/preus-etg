using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationMilestoneDateName enum, provides a way to identify notification milestone date name rules.
	/// </summary>
	public enum NotificationMilestoneDateName
	{
		/// <summary>
		/// NotApplicable - Participant reported not applicable.
		/// </summary>
		[Description("Not Applicable")]
		NotApplicable = 0,
		/// <summary>
		/// DateSubmitted - Milestone of date submitted.
		/// </summary>
		[Description("Date Submitted")]
		DateSubmitted = 1,
		/// <summary>
		/// StartDate - Milestone of training start date.
		/// </summary>
		[Description("Training Start Date")]
		TrainingStartDate = 2,
		/// <summary>
		/// EndDate - Milestone of training end date.
		/// </summary>
		[Description("Training End Date")]
		TrainingEndDate = 3,
		/// <summary>
		/// DateCancelled - Milestone of date cancelled.
		/// </summary>
		[Description("Date Cancelled")]
		DateCancelled = 4,
		/// <summary>
		/// DateAccepted - Milestone of date cancelled.
		/// </summary>
		[Description("Date Accepted")]
		DateAccepted = 5,
		/// <summary>
		/// ParticipantReportingDueDate - Milestone of participant reporting due date.
		/// </summary>
		[Description("Participant Reporting Due Date")]
		ParticipantReportingDueDate = 6,
		/// <summary>
		/// ReimbursementClaimDueDate - Milestone of reimbursement claim due date.
		/// </summary>
		[Description("Reimbursement Claim Due Date")]
		ReimbursementClaimDueDate = 7,
		/// <summary>
		/// CompletionReportingDueDate - Milestone of completion reporting due date.
		/// </summary>
		[Description("Completion Reporting Due Date")]
		CompletionReportingDueDate = 8,
		/// <summary>
		/// DeliveryStartDate - Milestone of delivery start date.
		/// </summary>
		[Description("Delivery Start Date")]
		DeliveryStartDate = 9,
		/// <summary>
		/// DeliveryEndDate - Milestone of delivery end date.
		/// </summary>
		[Description("Delivery End Date")]
		DeliveryEndDate = 10,
		/// <summary>
		/// DeliveryEndDate - Milestone of todays date.
		/// </summary>
		[Description("Todays Date")]
		TodaysDate = 11,
		/// <summary>
		/// AgreementIssuedDate - The date the agreement was issued.
		/// </summary>
		[Description("Agreement Issued Date")]
		AgreementIssuedDate = 12
	}
}
