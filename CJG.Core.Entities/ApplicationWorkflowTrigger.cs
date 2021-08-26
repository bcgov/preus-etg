using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ApplicationWorkflowTrigger"/> enum, provides all the valid workflow triggers that can be actioned against the GrantApplication.
	/// </summary>
	public enum ApplicationWorkflowTrigger
	{
		/// <summary>
		/// SubmitApplication - Application Administrator submits the application to be assessed.
		/// </summary>
		[Description("Submit Application")]
		SubmitApplication = 0,
		/// <summary>
		/// SelectForAssessment - Assessor places the application into the assessment queue  Funds are reserved.
		/// </summary>
		[Description("Select for Assessment")]
		SelectForAssessment = 1,
		/// <summary>
		/// ReturnUnfundedApplications - Director returns all unfunded applications.
		/// </summary>
		[Description("Return Unfunded Applications")]
		ReturnUnfundedApplications = 2,
		/// <summary>
		/// RemoveFromAssessment - Assessor removes the application from the assessment queue.  Funds are released.
		/// </summary>
		[Description("Remove from Assessment")]
		RemoveFromAssessment = 3,
		/// <summary>
		/// BeginAssessment - Assessor is assigned to the application.  Intake targets are updated..
		/// </summary>
		[Description("Begin Assessment")]
		BeginAssessment = 4,
		/// <summary>
		/// RecommendedForApproval - Assessor recommends the application to the Director for approval.
		/// </summary>
		[Description("Recommend for Approval")]
		RecommendForApproval = 5,
		/// <summary>
		/// RecommendedForDenial - Assessor recommends the application to the Director for denial.
		/// </summary>
		[Description("Recommend for Denial")]
		RecommendForDenial = 6,
		/// <summary>
		/// IssueOffer - Director creates an Agreement and issues an offer to the Application Administrator.  Intake targets are updated.
		/// </summary>
		[Description("Issue Offer")]
		IssueOffer = 7,
		/// <summary>
		/// ReturnToAssessment - Director returns the application to the Assessor for additional information. 
		/// </summary>
		[Description("Return to Assessment")]
		ReturnToAssessment = 8,
		/// <summary>
		/// DenyApplication - Director denies the application and returns it to the Application Administrator.  Intake targets are updated.
		/// </summary>
		[Description("Deny Application")]
		DenyApplication = 9,
		/// <summary>
		/// WithdrawOffer - Director withdraws the offer before the Agreement can be accepted.  Intake targets are updated.
		/// </summary>
		[Description("Withdraw Offer")]
		WithdrawOffer = 10,
		/// <summary>
		/// AcceptGrantAgreement - Application Administrator accepts the Agreement.
		/// </summary>
		[Description("Accept Grant Agreement")]
		AcceptGrantAgreement = 11,
		/// <summary>
		/// RejectGrantAgreement - Application Administrator has rejected the Agreement.  Intake targets are updated.
		/// </summary>
		[Description("Reject Grant Agreement")]
		RejectGrantAgreement = 12,
		/// <summary>
		/// CancelAgreementMinistry - Ministry has cancelled the Agreement.  Intake targets are updated.
		/// </summary>
		[Description("Cancel Agreement Ministry")]
		CancelAgreementMinistry = 13,
		/// <summary>
		/// CancelAgreementHolder - Application Administrator has cancelled the Agreement.  Intake targets are updated.
		/// </summary>
		[Description("Cancel Agreement Holder")]
		CancelAgreementHolder = 14,
		/// <summary>
		/// WithdrawApplication - Application Administrator has withdrawn the application before an Agreement could be issued.  Intake targets are updated.
		/// </summary>
		[Description("Withdraw Application")]
		WithdrawApplication = 15,
		/// <summary>
		/// SubmitChangeRequest - Application Administrator has submitted a Change Request.
		/// </summary>
		[Description("Submit Change Request")]
		SubmitChangeRequest = 16,
		/// <summary>
		/// RecommendChangeForApproval - Assessor recommends the Change Request to the Director for approval.
		/// </summary>
		[Description("Recommend Change for Approval")]
		RecommendChangeForApproval = 17,
		/// <summary>
		/// RecommendChangeForDenial - Assessor recommends the Change Request to the Director for denial.
		/// </summary>
		[Description("Recommend Change for Denial")]
		RecommendChangeForDenial = 18,
		/// <summary>
		/// ApproveChangeRequest - Director approves the Change Request and returns it to the Application Administrator.
		/// </summary>
		[Description("Approve Change Request")]
		ApproveChangeRequest = 19,
		/// <summary>
		/// ReturnChangeToAssessment - Director returns the Change Request to the Assessor for additional information.
		/// </summary>
		[Description("Return Change to Assessment")]
		ReturnChangeToAssessment = 20,
		/// <summary>
		/// DenyChangeRequest - Director denies the Change Request and returns it to the Application Administrator.
		/// </summary>
		[Description("Deny Change Request")]
		DenyChangeRequest = 21,
		/// <summary>
		/// SubmitClaim - Application Administrator has submitted a Claim.  Grant Opening Financials are updated.
		/// </summary>
		[Description("Submit Claim")]
		SubmitClaim = 22,
		/// <summary>
		/// SelectClaimForAssessment - Assessor selects the Claim for eligibility assessment.  Grant Opening Financials are updated.
		/// </summary>
		[Description("Select for Assessment")]
		SelectClaimForAssessment = 23,
		/// <summary>
		/// RemoveClaimFromAssessment - Assessor has removed the Claim from assessment and returned it to the state of NewClaim
		/// </summary>
		[Description("Remove Claim from Assessment")]
		RemoveClaimFromAssessment = 24,
		/// <summary>
		/// AssessEligibility - Assessor returns the Claim from reimbursment assessment to eligibility assessment.
		/// </summary>
		[Description("Assess Eligibility")]
		AssessEligibility = 25,
		/// <summary>
		/// AssessReimbursement - Assessor selects the Claim for reimbursement assessment.
		/// </summary>
		[Description("Assess Reimbursement")]
		AssessReimbursement = 26,
		/// <summary>
		/// ReturnClaimToApplicant - Assessor/Director has returned the Claim to the Application Administrator for additional information.  Grant Opening Financials are updated.
		/// </summary>
		[Description("Return Claim to Applicant")]
		ReturnClaimToApplicant = 27,
		/// <summary>
		/// ApproveClaim - Assessor/Director has approved the Claim and returns it to the Application Administrator.  Grant Opening Financials are updated.
		/// </summary>
		[Description("Approve Claim")]
		ApproveClaim = 28,
		/// <summary>
		/// DenyClaim - Assessor/Director has denied the Claim and returns it to the Application Administrator.  Grant Opening Financials are updated.
		/// </summary>
		[Description("Deny Claim")]
		DenyClaim = 29,
		/// <summary>
		/// ReassignAssessor - Assessor/Director reassign the current assessor to another assessor on the Grant File and possibly the current Claim..
		/// </summary>
		[Description("Reassign Assessor")]
		ReassignAssessor = 30,
		/// <summary>
		/// EditApplication - Assessor/Director edits the application details.
		/// </summary>
		[Description("Edit Application")]
		EditApplication = 31,
		/// <summary>
		/// EditTrainingCosts - Assessor/Director edits the training cost for the Grant Application before or after an agreement has been offered.
		/// </summary>
		[Description("Edit Training Costs")]
		EditTrainingCosts = 32,
		/// <summary>
		/// EditTrainingProvider - Assessor/Director edits Training Provider and/or attachments for the Grant Application before or after an agreement has been offered.
		/// </summary>
		[Description("Edit Training Provider")]
		EditTrainingProvider = 33,
		/// <summary>
		/// AmendClaim - Assessor/Director informs the Application Administrator they need to make a claim amendment.
		/// </summary>
		[Description("Amend Claim")]
		AmendClaim = 34,
		/// <summary>
		/// AddNote - Assessor/Director adds a note to the Grant Application.
		/// </summary>
		[Description("Add Note")]
		AddNote = 35,
		/// <summary>
		/// DeleteNote - Director deletes a note from the Grant Application.
		/// </summary>
		[Description("Delete Note")]
		DeleteNote = 36,
		/// <summary>
		/// WithdrawClaim - Application Administrator withdraws a submitted claim.
		/// </summary>
		[Description("Withdraw Claim")]
		WithdrawClaim = 37,
		/// <summary>
		/// EditClaim - Assessor/Director edits the training cost estimate for the Grant Application after an agreement has been offered.
		/// </summary>
		[Description("Edit Claim")]
		EditClaim = 38,
		/// <summary>
		/// ViewApplication - Application Administrator/Employer Administrator or Assessor/Director views the grant application.
		/// </summary>
		[Description("View Application")]
		ViewApplication = 39,
		/// <summary>
		/// ViewClaim - Application Administrator or Assessor/Director views the claim.
		/// </summary>
		[Description("View Claim")]
		ViewClaim = 40,
		/// <summary>
		/// ValidateTrainingProvider - Assessor/Director validates the training provider.
		/// </summary>
		[Description("Validate Training Provider")]
		ValidateTrainingProvider = 41,
		/// <summary>
		/// CloseClaimReporting - Director (AM3) or Assign Assessor (AM2 or AM5) closes for participant and claim reporting.
		/// </summary>
		[Description("Close Claim Reporting")]
		CloseClaimReporting = 42,
		/// <summary>
		/// EnableClaimReporting - Director (AM3) or Assign Assessor (AM2 or AM5) reopens the file for claim reporting.
		/// </summary>
		[Description("Enable Claim Reporting")]
		EnableClaimReporting = 43,
		/// <summary>
		/// EnableCompletionReporting - Director (AM3) or Assign Assessor (AM2 or AM5) opens the file for completion reporting only.
		/// </summary>
		[Description("Enable Completion Reporting")]
		EnableCompletionReporting = 44,
		/// <summary>
		/// EditParticipants - AA or AE invites, removes, includes/excludes participants.
		/// </summary>
		[Description("Enable Participant Reporting")]
		EditParticipants = 45,
		/// <summary>
		/// EditTrainingProgram - Assessor/Director edits Training Program and/or attachments for the Grant Application before or after an agreement has been offered.
		/// </summary>
		[Description("Edit Training Program")]
		EditTrainingProgram = 46,
		/// <summary>
		/// SubmitCompletionReport - When a grant file is in the state Completion Reporting and the Employer Administrator saves/submits the completion report the grant file state is changed to Closed.
		/// </summary>
		[Description("Submit Completion Report")]
		SubmitCompletionReport = 47,
		/// <summary>
		/// Close - Director closed the Grant Application, it can no longer be acted upon.
		/// </summary>
		[Description("Close Grant File")]
		Close = 50,
		/// <summary>
		/// GeneratePaymentRequest - Financial Clerk is listing claims for managing payment requests.
		/// </summary>
		[Description("Generate Payment Requests")]
		GeneratePaymentRequest = 51,
		/// <summary>
		/// EditApplicationAttachments - Applicant/Assessor/Director edits the application attachments.
		/// </summary>
		[Description("Edit Application Attachments")]
		EditApplicationAttachments = 52,
		/// <summary>
		/// ViewParticipants - Directors can view individual participants.
		/// </summary>
		[Description("View Participants")]
		ViewParticipants = 53,
		/// <summary>
		/// AddRemoveTrainingProvider - Add/Remove Training Provider
		/// </summary>
		[Description("Add/Remove Training Provider")]
		AddRemoveTrainingProvider = 54,
		/// <summary>
		/// EditApplicantContact - Whether the applicant contact can be edited.
		/// </summary>
		[Description("Edit the Applicant Contact")]
		EditApplicantContact = 55,
		/// <summary>
		/// CreateClaim - Allows the application administrator to create a new claim.
		/// </summary>
		[Description("Create New Claim")]
		CreateClaim = 56,
		/// <summary>
		/// EnableApplicantReportingOfParticipants - Allows the director/assessor/assigned to enable/disable applicant reporting of participants.
		/// </summary>
		[Description("Enable Applicant Reporting Of Participants")]
		EnableApplicantReportingOfParticipants = 57,
		/// <summary>
		/// EditTrainingCostOverride - Allows the Director to edit the training cost and exceed the 10% total government contribution limit
		/// </summary>
		[Description("Edit Training Cost Override")]
		EditTrainingCostOverride = 58,
		/// <summary>
		/// AddOrRemoveTrainingProgram - Assessor/Director add and/or remove Training Component for the Grant Application before or after an agreement has been offered.
		/// </summary>
		[Description("Add or Remove Training Program")]
		AddOrRemoveTrainingProgram = 59,
		/// <summary>
		/// EditApplicant - Assessor/Director can update the applicant information.
		/// </summary>
		[Description("Edit Applicant")]
		EditApplicant = 60,
		/// <summary>
		/// EnableParticipantReporting - Whether participant can be reported.
		/// </summary>
		[Description("Enable Reporting of Participants")]
		EnableParticipantReporting = 61,
		/// <summary>
		/// EditSummary - Whether the summary section can be edited.
		/// </summary>
		[Description("Edit Application Summary")]
		EditSummary = 62,
		/// <summary>
		/// EditProgramDescription - Whether the program description can be edited.
		/// </summary>
		[Description("Edit Application Program Description")]
		EditProgramDescription = 63,
		/// <summary>
		/// HoldPaymentRequests - Stop payment requests from being generated for the specified grant application.
		/// </summary>
		[Description("Hold payment requests")]
		HoldPaymentRequests = 64
	}
}
