using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="ApplicationStateExternal"/> enum, provides all possible external states for a Grant File.
    /// This includes recording the steps for application creation, submission, acceptance of an agreement, approval and reporting claims.
    /// </summary>
    public enum ApplicationStateExternal
    {
        /// <summary>
        /// NotStarted - The Grant Application has not been started.  This will probably never happen...
        /// </summary>
        [Description("Not Started")]
        NotStarted = 0,
        /// <summary>
        /// Incomplete - The Grant Application has been created but needs more information.
        /// </summary>
        [Description("Incomplete")]
        Incomplete = 1,
        /// <summary>
        /// Complete - The Grant Application has been created and is ready to be submitted.
        /// </summary>
        [Description("Not Submitted")]
        Complete = 2,
        /// <summary>
        /// Submitted - The Grant Application has been submitted for assessment.
        /// </summary>
        [Description("Complete")]
        Submitted = 3,
        /// <summary>
        /// ApplicationWithdrawn - Application Administrator can withdrawn their Grant Application before an offer is issued.
        /// In this state, it can/should be edited and returned to a Complete or Incomplete state to allow it to be resubmitted.
        /// </summary>
        [Description("Application Withdrawn")]
        ApplicationWithdrawn = 4,
        /// <summary>
        /// Approved - An agreement has been accepted by the application administrator and as a consequence of this, the grant is approved.
        /// Grant file management moves on to reporting participants and claims.
        /// </summary>
        [Description("Approved")]
        Approved = 5,
        /// <summary>
        /// ApplicationDenied - The Grant Application was denied and is returned to the application administrator read only.
        /// A file in this state cannot be edited and resubmitted; it is like a Closed state in this regard.
        /// </summary>
        [Description("Application Denied")]
        ApplicationDenied = 6,
        /// <summary>
        /// CancelledByMinistry - The Grant Agreement was cancelled by the ministry with a good business reason (like no participant or claim reporting activity has occurred).
        /// </summary>
        [Description("Cancelled by Ministry")]
        CancelledByMinistry = 7,
        /// <summary>
        /// CancelledByAgreementHolder - The Grant Agreement was cancelled by the Application Administrator. Note, this can only be done before a claim has been submitted.
        /// </summary>
        [Description("Cancelled by Agreement Holder")]
        CancelledByAgreementHolder = 8,
        /// <summary>
        /// AcceptGrantAgreement - In this state, the next action for the Application Administrator is to accept the grant agreement which is presented to them as an offer from the Ministry.
        /// </summary>
        [Description("Accept Grant Agreement")]
        AcceptGrantAgreement = 9,
        /// <summary>
        /// ChangeRequestSubmitted - A change request has been submitted for the Grant Agreement.
        /// </summary>
        [Description("Change Request Submitted")]
        ChangeRequestSubmitted = 10,
        /// <summary>
        /// ChangeRequestApproved - The change request has been approved for the Grant Agreement.
        /// In this state, Claims may be submitted; this state is functionally similar to the Approved state.
        /// </summary>
        [Description("Change Request Approved")]
        ChangeRequestApproved = 11,
        /// <summary>
        /// ChangeRequestDenied - The change request has been denied.
        /// In this state Claims may be submitted; the change request has had no impact on what is approved in the agreement.
        /// </summary>
        [Description("Change Request Denied")]
        ChangeRequestDenied = 12,
        /// <summary>
        /// NotAccepted - The Grant Application has been returned to the Application Administrator because the funding for
        /// the grant opening was fully committed before this application could be assessed.
        /// </summary>
        [Description("Not Accepted")]
        NotAccepted = 13,
        /// <summary>
        /// AgreementWithdrawn - The Grant Agreement (offer) was withdrawn by the ministry before it could be accepted by the Application Administrator.
        /// This might come about because the application administrator did not accept the agreement by the deadline.
        /// </summary>
        [Description("Agreement Withdrawn")]
        AgreementWithdrawn = 14,
        /// <summary>
        /// AgreementRejected - The offer of a grant agreement was rejected by the Application Administrator.  This is a closing state for a grant file.
        /// </summary>
        [Description("Agreement Rejected")]
        AgreementRejected = 15,
		/// <summary>
		/// ClaimSubmitted - The Application Administrator has submitted a Claim.  Only one Claim can be active at one time.  New Claims cannot be created when the grant file is in this state.
		/// </summary>
		[Description("Claim Submitted")]
        ClaimSubmitted = 21,
        /// <summary>
        /// ClaimReturned - The ministry has returned a submitted Claim and it becomes the current claim for editing and resubmission
        /// (a new claim version is not created for this).  This Claim can be resubmitted when it has been edited.
        /// </summary>
        [Description("Claim Returned")]
        ClaimReturned = 23,
        /// <summary>
        /// ClaimDenied - The ministry has denied the Claim.  This Claim is readonly and cannot be resubmitted; it assessment value is zero dollars.
        /// This closes the claim workflow states and becomes the first and only child claim state assigned to this claim.
        /// A claim inherits its first claim child state of ClaimApproved OR ClaimDenied from the assessment outcome.
        /// A claim can be amended and in this case will not be an amendment but will be a blank claim (next version) and will have the grant file progress through the claim states again.
        /// A second version of a claim does not change the claim child states on the first claim.
        /// </summary>
        [Description("Claim Denied")]
        ClaimDenied = 24,
        /// <summary>
        /// ClaimApproved - The ministry has approved the Claim.  This ends the claim workflow states (for this claim version) and child claim states for
        /// payment processing will be used to mark the progress of claim payment in the assessment block.
        /// Payment processing states are NOT ApplicationExternalStates.  They apply only to individual claims and will reflect the progress of payments.
        /// A claim inherits its first claim child state of ClaimApproved OR ClaimDenied from the assessment outcome.
        /// A claim can be amended and will result in a new claim version being created that will have the grant file progress through the claim states again.
        /// A second version of a claim does not alter the claim child states of the first claim.
        /// </summary>
        [Description("Claim Approved")]
        ClaimApproved = 25,
        /// <summary>
        /// AmendClaim - This state provides a way for the Assessor to notify the Application Administrator that they should complete a Claim amendment.
        /// </summary>
        [Description("Amend Claim")]
        AmendClaim = 26,
        /// <summary>
        /// ReportCompletion - This state has closed claim reporting and allows only completion reporting for the grant file.
        /// </summary>
        [Description("Report Completion")]
        ReportCompletion = 27,
		/// <summary>
		/// Returned to Applicant Unassessed - This application will be removed from the queue and applicants will receive the "Returned to Applicant Unassessed" notification. Application details will remain in the system but will not affect the budget
		/// </summary>
		[Description("Returned to Applicant Unassessed")]
		ReturnedUnassessed = 28,
		/// <summary>
		/// Closed - The Grant Application is closed and can no longer be edited.
		/// </summary>
		[Description("Grant File Closed")]
        Closed = 30
    }
}