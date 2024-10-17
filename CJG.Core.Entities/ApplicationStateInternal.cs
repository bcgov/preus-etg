using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ApplicationStateInternal"/> enum, provides a way to manage the current Grant file for internal purpose.
	/// Internal states record more information about the progress through Ministry workflow and outcomes for reporting purposes.
	/// Often a change to the Internal state is NOT reflected in a similar change to the External state until an information exchange
	/// with the Application Administrator is required.
	/// </summary>
	public enum ApplicationStateInternal
	{
		/// <summary>
		/// Draft - The Grant Application is being created by the Application Administrator but is not submitted.
		/// </summary>
		[Description("Draft")]
		Draft = 0,
		/// <summary>
		/// New - The Grant Application has been submitted by the Application Administrator and is part of the intake for a grant opening.
		/// It is waiting to be selected for assessment.
		/// The external state is "Submitted"
		/// </summary>
		[Description("New")]
		New = 1,
		/// <summary>
		/// PendingAssessment - The Grant Application has been selected for assessment with funds reserved to allow a successful assessment.
		/// The grant file in this state has its application waiting in a queue to be assigned to an assessor for assessment.
		/// The external state is "Submitted"
		/// </summary>
		[Description("Pending Assessment")]
		PendingAssessment = 2,
		/// <summary>
		/// UnderAssessment - The Grant Application is currently under assessment by the Assessor assigned to the grant file.
		/// The external state is "Submitted"
		/// </summary>
		[Description("Under Assessment")]
		UnderAssessment = 3,
		/// <summary>
		/// ReturnedToAssessment - The Grant Application has been reviewed by the Director and returned to the Assessor for more information.
		/// The external state is "Submitted"
		/// </summary>
		[Description("Returned to Assessment")]
		ReturnedToAssessment = 4,
		/// <summary>
		/// RecommendedForApproval - The assessor has recommended to the director that the grant application be approved.
		/// The external state is "Submitted"
		/// </summary>
		[Description("Recommended for Approval")]
		RecommendedForApproval = 5,
		/// <summary>
		/// RecommendedForDenial - The assessor has recommended to the director that the grant application be denied.
		/// The external state is "Submitted"
		/// </summary>
		[Description("Recommended for Denial")]
		RecommendedForDenial = 6,
		/// <summary>
		/// OfferIssued - As the first step in approval for a grant, the Director has issued an offer in the form of an Agreement for acceptance by the application administrator.
		/// The agreement must be accepted by the application administrator for the grant to be approved and participant and claim reporting to begin.
		/// The external state is "AcceptGrantAgreement"
		/// </summary>
		[Description("Offer Issued")]
		OfferIssued = 7,
		/// <summary>
		/// OfferWithdrawn - The Agreement has been withdrawn by the Ministry before Application Administrator acceptance. This occurs at the discretion of
		/// the director when the deadline for accepting the agreement passes.  The director controls when the offer is withdrawn and may choose to provide
		/// more time for acceptance.  Notifications inform the application administrator of the need to accept the agreement by the deadline.
		/// The external state is "AgreementWithdrawn"
		/// </summary>
		[Description("Offer Withdrawn")]
		OfferWithdrawn = 8,
		/// <summary>
		/// AgreementAccepted - The Agreement has been accepted by the Application Administrator (who represents the Applicant business).
		/// This ends successful application submission and assessment and, externally, the grant file moves on to (stage 2) enabling reporting for participants, claims and completion.
		/// There is no internal activity required now until a change request or claim is reported.  The agreement may also be cancelled by either party.
		/// The external state is "Approved" because the grant is approved; this is more relevant than showing "Agreement Accepted" externally which the user already knows.
		/// </summary>
		[Description("Agreement Accepted")]
		AgreementAccepted = 9,
		/// <summary>
		/// Unfunded - The Grant Application did not receive funding and was returned to the application administrator in a state where it can be edited and resubmitted.
		/// The external state is "Not Accepted" because the application was not accepted due to insufficient funding.
		/// </summary>
		[Description("Unfunded")]
		Unfunded = 10,
		/// <summary>
		/// ApplicationDenied - The Grant Application has been denied.
		/// The external sate is "Application Denied"
		/// </summary>
		[Description("Application Denied")]
		ApplicationDenied = 11,
		/// <summary>
		/// AgreementRejected - The Agreement was rejected by the Application Administrator.
		/// The external state is "Grant File Closed" because the grant file is externally closed by this action.
		/// Messages to the application administrator advise them of this outcome should then choose to reject the agreement.
		/// </summary>
		[Description("Agreement Rejected")]
		AgreementRejected = 12,
		/// <summary>
		/// ApplicationWithdrawn - The Application was withdrawn by the Application Administrator before an Agreement was offered.
		/// The external state is "Application Withdrawn"
		/// </summary>
		[Description("Application Withdrawn")]
		ApplicationWithdrawn = 13,
		/// <summary>
		/// CancelledByMinistry - The Agreement was cancelled by the ministry after it was accepted by the Application Administrator.
		/// The external state is "Cancelled by Ministry"
		/// </summary>
		[Description("Cancelled by Ministry")]
		CancelledByMinistry = 14,
		/// <summary>
		/// CancelledByAgreementHolder - The Agreement was cancelled by the Application Administrator after accepting.
		/// The external state is "Cancelled by Agreement Holder"
		/// </summary>
		[Description("Cancelled by Agreement Holder")]
		CancelledByAgreementHolder = 15,
		/// <summary>
		/// ChangeRequest - The Application Administrator has made a change request on the accepted Grant Application and
		/// the request is new for Ministry assessment.
		/// The external state is "Change Request Submitted"
		/// </summary>
		[Description("Change Request")]
		ChangeRequest = 16,
		/// <summary>
		/// ChangeForApproval - The change request was recommended to the director for approval by the Assessor.
		/// The external state is "Change Request Submitted" until the change request has been assessed.
		/// </summary>
		[Description("Change for Approval")]
		ChangeForApproval = 17,
		/// <summary>
		/// ChangeForDenial - The change request was recommended to the director for denied by the director.
		/// The external state is "Change Request Submitted" until the change request has been assessed.
		/// </summary>
		[Description("Change for Denial")]
		ChangeForDenial = 18,
		/// <summary>
		/// ChangeReturned - The change request was returned to assessment by the director for more information.
		/// The external state is "Change Request Submitted" until the change request has been assessed.
		/// </summary>
		[Description("Change Returned")]
		ChangeReturned = 19,
		/// <summary>
		/// ChangeRequestDenied - The change request was denied by the director.
		/// The external state is "Change Request Denied" and allows another change request to be made.
		/// </summary>
		[Description("Change Request Denied")]
		ChangeRequestDenied = 20,
		/// <summary>
		/// NewClaim - A claim is submitted and claim assessment is the next action for assessors.
		/// Assessors assess and approve claims without director approval.
		/// The external state is "Claim Submitted"
		/// </summary>
		[Description("New Claim")]
		NewClaim = 21,
		/// <summary>
		/// ClaimAssessEligibility - An Assessor has selected a claim for eligibility assessment and is assigned to the grant file as the assessor.
		/// The external state is "Claim Submitted"
		/// </summary>
		[Description("Claim Assess Eligibility")]
		ClaimAssessEligibility = 22,
		/// <summary>
		/// ClaimReturnedToApplicant - The Claim needs more information and has been returned to the application administrator to edit and resubmit.
		/// The need for more information may include attached documentation required to substantiate the claim.
		/// This state is also used if the Application Administrator withdraws the claim before assessment is complete.
		/// The external state is "Claim Returned"
		/// </summary>
		[Description("Claim Returned to Applicant")]
		ClaimReturnedToApplicant = 23,
		/// <summary>
		/// ClaimDenied - The Claim has been denied and has a zero dollar assessment outcome and will show externally in the
		/// claim assessment block with the outcome of the assessment.
		/// The external state is "Claim Denied"
		/// </summary>
		[Description("Claim Denied")]
		ClaimDenied = 24,
		/// <summary>
		/// ClaimApproved - The Claim has been approved and will now show externally in the claim assessment block with the outcome of the assessment.
		/// The external state is "Claim Approved"
		/// </summary>
		[Description("Claim Approved")]
		ClaimApproved = 25,
		/// <summary>
		/// ReturnedToApplicantUnassessed - This application will be removed from the queue and applicants will receive the "Returned to Applicant Unassessed" notification. Application details will remain in the system but will not affect the budget.
		/// The external state is "Returned to Applicant Unassessed"
		/// </summary>
		[Description("Returned to Applicant Unassessed")]
		ReturnedUnassessed = 28,
		/// <summary>
		/// Closed - The Grant Application is closed and can no longer be edited.
		/// The external state is "Grant File Closed"
		/// </summary>
		[Description("Closed")]
		Closed = 30,
		/// <summary>
		/// ClaimAssessReimbursement - An Assessor has selected a claim for reimbursement assessment and is assigned to the grant file as the assessor.
		/// The external state is "Claim Submitted"
		/// </summary>
		[Description("Claim Assess Reimbursement")]
		ClaimAssessReimbursement = 31,
		/// <summary>
		/// Completion Reporting - Claim reporting is closed and only completion reporting is allowed at this time.
		/// The external state is "Report Completion"
		/// </summary>
		[Description("Completion Reporting")]
		CompletionReporting = 32
	}
}