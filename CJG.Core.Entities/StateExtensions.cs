using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="StateExtensions"/> static class, provides extension methods for states.
	/// </summary>
	public static class StateExtensions
	{
		/// <summary>
		/// Returns the relevant internal state to the specified external state.
		/// They are not a one-to-one match, so some internal states are hidden to the external state.
		/// </summary>
		/// <param name="state"></param>
		/// <returns>Returns the relevant internal state to the specified external state.</returns>
		public static ApplicationStateInternal GetInternalState(this ApplicationStateExternal state)
		{
			switch (state)
			{
				case ApplicationStateExternal.NotStarted:
				case ApplicationStateExternal.Incomplete:
				case ApplicationStateExternal.Complete:
					return ApplicationStateInternal.Draft;
				case ApplicationStateExternal.Submitted:
					return ApplicationStateInternal.New;
				case ApplicationStateExternal.ApplicationWithdrawn:
					return ApplicationStateInternal.ApplicationWithdrawn;
				case ApplicationStateExternal.Approved:
				case ApplicationStateExternal.AmendClaim:
				case ApplicationStateExternal.ChangeRequestApproved:
					return ApplicationStateInternal.AgreementAccepted;
				case ApplicationStateExternal.ApplicationDenied:
					return ApplicationStateInternal.ApplicationDenied;
				case ApplicationStateExternal.CancelledByMinistry:
					return ApplicationStateInternal.CancelledByMinistry;
				case ApplicationStateExternal.CancelledByAgreementHolder:
					return ApplicationStateInternal.CancelledByAgreementHolder;
				case ApplicationStateExternal.AcceptGrantAgreement:
					return ApplicationStateInternal.OfferIssued;
				case ApplicationStateExternal.ChangeRequestSubmitted:
					return ApplicationStateInternal.ChangeRequest;
				case ApplicationStateExternal.ChangeRequestDenied:
					return ApplicationStateInternal.ChangeRequestDenied;
				case ApplicationStateExternal.NotAccepted:
					return ApplicationStateInternal.Unfunded;
				case ApplicationStateExternal.AgreementWithdrawn:
					return ApplicationStateInternal.OfferWithdrawn;
				case ApplicationStateExternal.ClaimSubmitted:
					return ApplicationStateInternal.NewClaim;
				case ApplicationStateExternal.ClaimReturned:
					return ApplicationStateInternal.ClaimReturnedToApplicant;
				case ApplicationStateExternal.ClaimDenied:
					return ApplicationStateInternal.ClaimDenied;
				case ApplicationStateExternal.ClaimApproved:
					return ApplicationStateInternal.ClaimApproved;
				case ApplicationStateExternal.AgreementRejected:
					return ApplicationStateInternal.AgreementRejected;
				case ApplicationStateExternal.Closed:
					return ApplicationStateInternal.Closed;
				case ApplicationStateExternal.ReportCompletion:
					return ApplicationStateInternal.CompletionReporting;
				case ApplicationStateExternal.ReturnedUnassessed:
					return ApplicationStateInternal.ReturnedUnassessed;
				default:
					throw new InvalidOperationException($"There is no internal state associated with the specified external state '{state.ToString("g")}'.");
			}
		}

		/// <summary>
		/// Returns the relevant external state to the specified internal state.
		/// They are not a one-to-one match, so some external states are represented by multiple internal states.
		/// </summary>
		/// <param name="state"></param>
		/// <returns>Returns the relevant external state to the specified internal state.</returns>
		public static ApplicationStateExternal GetExternalState(this ApplicationStateInternal state)
		{
			switch (state)
			{
				case ApplicationStateInternal.Draft:
					return ApplicationStateExternal.Incomplete;
				case ApplicationStateInternal.New:
				case ApplicationStateInternal.PendingAssessment:
				case ApplicationStateInternal.UnderAssessment:
				case ApplicationStateInternal.ReturnedToAssessment:
				case ApplicationStateInternal.RecommendedForApproval:
				case ApplicationStateInternal.RecommendedForDenial:
					return ApplicationStateExternal.Submitted;
				case ApplicationStateInternal.OfferIssued:
					return ApplicationStateExternal.AcceptGrantAgreement;
				case ApplicationStateInternal.OfferWithdrawn:
					return ApplicationStateExternal.AgreementWithdrawn;
				case ApplicationStateInternal.AgreementAccepted:
					return ApplicationStateExternal.Approved;
				case ApplicationStateInternal.ApplicationDenied:
					return ApplicationStateExternal.ApplicationDenied;
				case ApplicationStateInternal.Unfunded:
					return ApplicationStateExternal.NotAccepted;
				case ApplicationStateInternal.AgreementRejected:
					return ApplicationStateExternal.AgreementRejected;
				case ApplicationStateInternal.ApplicationWithdrawn:
					return ApplicationStateExternal.ApplicationWithdrawn;
				case ApplicationStateInternal.CancelledByMinistry:
					return ApplicationStateExternal.CancelledByMinistry;
				case ApplicationStateInternal.CancelledByAgreementHolder:
					return ApplicationStateExternal.CancelledByAgreementHolder;
				case ApplicationStateInternal.ChangeRequest:
				case ApplicationStateInternal.ChangeForApproval:
				case ApplicationStateInternal.ChangeForDenial:
				case ApplicationStateInternal.ChangeReturned:
					return ApplicationStateExternal.ChangeRequestSubmitted;
				case ApplicationStateInternal.ChangeRequestDenied:
					return ApplicationStateExternal.ChangeRequestDenied;
				case ApplicationStateInternal.NewClaim:
				case ApplicationStateInternal.ClaimAssessEligibility:
				case ApplicationStateInternal.ClaimAssessReimbursement:
					return ApplicationStateExternal.ClaimSubmitted;
				case ApplicationStateInternal.ClaimReturnedToApplicant:
					return ApplicationStateExternal.ClaimReturned;
				case ApplicationStateInternal.ClaimDenied:
					return ApplicationStateExternal.ClaimDenied;
				case ApplicationStateInternal.ClaimApproved:
					return ApplicationStateExternal.ClaimApproved;
				case ApplicationStateInternal.CompletionReporting:
					return ApplicationStateExternal.ReportCompletion;
				case ApplicationStateInternal.Closed:
					return ApplicationStateExternal.Closed;
				case ApplicationStateInternal.ReturnedUnassessed:
					return ApplicationStateExternal.ReturnedUnassessed;
				default:
					throw new InvalidOperationException($"There is not external state associated with the specified internal state '{state.ToString("g")}'.");
			}
		}

		/// <summary>
		/// Determine if the internal states is before an agreement is generated.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static bool IsBeforeAgreement(this ApplicationStateInternal state)
		{
			return state.In(
				ApplicationStateInternal.Draft,
				ApplicationStateInternal.New,
				ApplicationStateInternal.PendingAssessment,
				ApplicationStateInternal.UnderAssessment,
				ApplicationStateInternal.ReturnedToAssessment,
				ApplicationStateInternal.RecommendedForApproval,
				ApplicationStateInternal.RecommendedForDenial,
				ApplicationStateInternal.Unfunded,
				ApplicationStateInternal.ApplicationWithdrawn
			);
		}

		/// <summary>
		/// Provides all the valid workflow triggers for each internal state.
		/// </summary>
		private static readonly IDictionary<ApplicationStateInternal, IEnumerable<ApplicationWorkflowTrigger>> ValidWorkflowTriggers = new Dictionary<ApplicationStateInternal, IEnumerable<ApplicationWorkflowTrigger>>()
		{
			{ ApplicationStateInternal.Draft, new[] {                       ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription,                                                                                                        ApplicationWorkflowTrigger.SubmitApplication, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants } },
			{ ApplicationStateInternal.New, new[] {                         ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,       ApplicationWorkflowTrigger.SelectForAssessment,     ApplicationWorkflowTrigger.ReturnUnfundedApplications,                                      ApplicationWorkflowTrigger.ReturnUnassessed, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants } },
			{ ApplicationStateInternal.PendingAssessment, new[] {           ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,       ApplicationWorkflowTrigger.RemoveFromAssessment,    ApplicationWorkflowTrigger.BeginAssessment, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants } },
			{ ApplicationStateInternal.UnderAssessment, new[] {             ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.RecommendForApproval,   ApplicationWorkflowTrigger.RecommendForDenial, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.ReturnedToAssessment, new[] {        ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.RecommendForApproval,   ApplicationWorkflowTrigger.RecommendForDenial, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.RecommendedForApproval, new[] {      ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.ReturnToAssessment,     ApplicationWorkflowTrigger.IssueOffer, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.RecommendedForDenial, new[] {        ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider, ApplicationWorkflowTrigger.WithdrawApplication,   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.ReturnToAssessment,     ApplicationWorkflowTrigger.DenyApplication, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.OfferIssued, new[] {                 ApplicationWorkflowTrigger.ViewApplication,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                        ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.WithdrawOffer,          ApplicationWorkflowTrigger.AcceptGrantAgreement, ApplicationWorkflowTrigger.RejectGrantAgreement, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.UpdateParticipants, ApplicationWorkflowTrigger.ReturnOfferToAssessment } },
			{ ApplicationStateInternal.OfferWithdrawn, new[] {              ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment } },
			{ ApplicationStateInternal.AgreementAccepted, new[] {           ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,  ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.CreateClaim,    ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim, ApplicationWorkflowTrigger.SubmitClaim, ApplicationWorkflowTrigger.CloseClaimReporting, ApplicationWorkflowTrigger.AmendClaim,                                                                                                                  ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests} },
			{ ApplicationStateInternal.Unfunded, new[] {                    ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,   ApplicationWorkflowTrigger.SubmitApplication } },
			{ ApplicationStateInternal.ReturnedUnassessed, new[] {          ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.ReturnUnassessedToNew } },
			{ ApplicationStateInternal.ApplicationDenied, new[] {           ApplicationWorkflowTrigger.ViewApplication,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   ApplicationWorkflowTrigger.ReturnToAssessment,     ApplicationWorkflowTrigger.ViewParticipants } },
			{ ApplicationStateInternal.AgreementRejected, new[] {           ApplicationWorkflowTrigger.ViewApplication } },
			{ ApplicationStateInternal.ApplicationWithdrawn, new[] {        ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,                                                                                               ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                                                                                                                                                                                                                            ApplicationWorkflowTrigger.ViewParticipants } },
			{ ApplicationStateInternal.CancelledByMinistry, new[] {         ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication,                                                                                                                                                                                                                                                                                                                    ApplicationWorkflowTrigger.EditSummary,                                                          ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                        ApplicationWorkflowTrigger.ReassignAssessor,                                                                                                                                                        ApplicationWorkflowTrigger.ViewParticipants,                                                                                                                                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry } },
			{ ApplicationStateInternal.CancelledByAgreementHolder, new[] {  ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication,                                                                                                                                                                                                                                                                                                                    ApplicationWorkflowTrigger.EditSummary,                                                          ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                        ApplicationWorkflowTrigger.ReassignAssessor,                                                                                                                                                        ApplicationWorkflowTrigger.ViewParticipants } },
			{ ApplicationStateInternal.ChangeRequest, new[] {               ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,                                                  ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.RecommendChangeForApproval, ApplicationWorkflowTrigger.RecommendChangeForDenial,                                                                                                                                 ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.ChangeForApproval, new[] {           ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,                                                  ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.ReturnChangeToAssessment, ApplicationWorkflowTrigger.ApproveChangeRequest,                                                                                                                                       ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.ChangeForDenial, new[] {             ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,                                                  ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.ReturnChangeToAssessment, ApplicationWorkflowTrigger.DenyChangeRequest,                                                                                                                                          ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.ChangeReturned, new[] {              ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,                                                  ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.RecommendChangeForApproval, ApplicationWorkflowTrigger.RecommendChangeForDenial,                                                                                                                                 ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.ChangeRequestDenied, new[] {         ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationWorkflowTrigger.CancelAgreementHolder,  ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim, ApplicationWorkflowTrigger.SubmitClaim, ApplicationWorkflowTrigger.CloseClaimReporting,                                                                                                                                                         ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.NewClaim, new[] {                    ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                                                                    ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.WithdrawClaim, ApplicationWorkflowTrigger.SelectClaimForAssessment,                                                                                                                                              ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.EnableParticipantReporting, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.ClaimAssessEligibility, new[] {      ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                                                                    ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.WithdrawClaim, ApplicationWorkflowTrigger.ReturnClaimToApplicant, ApplicationWorkflowTrigger.DenyClaim, ApplicationWorkflowTrigger.RemoveClaimFromAssessment, ApplicationWorkflowTrigger.AssessReimbursement,    ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.EnableParticipantReporting, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.ClaimAssessReimbursement, new[] {    ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                                                                    ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.WithdrawClaim, ApplicationWorkflowTrigger.ReturnClaimToApplicant, ApplicationWorkflowTrigger.DenyClaim, ApplicationWorkflowTrigger.ApproveClaim, ApplicationWorkflowTrigger.AssessEligibility,                   ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.EnableParticipantReporting, ApplicationWorkflowTrigger.UpdateParticipants } },
			{ ApplicationStateInternal.ClaimReturnedToApplicant, new[] {    ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                    ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants,                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim, ApplicationWorkflowTrigger.SubmitClaim, ApplicationWorkflowTrigger.CloseClaimReporting,                                                                                                                                                         ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.ReverseClaimReturnedToApplicant } },
			{ ApplicationStateInternal.ClaimDenied, new[] {                 ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider,   ApplicationWorkflowTrigger.EditTrainingProvider, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram, ApplicationWorkflowTrigger.EditTrainingProgram, ApplicationWorkflowTrigger.EditTrainingCosts, ApplicationWorkflowTrigger.EditTrainingCostOverride, ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                    ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.CreateClaim,    ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim, ApplicationWorkflowTrigger.SubmitClaim, ApplicationWorkflowTrigger.CloseClaimReporting, ApplicationWorkflowTrigger.AmendClaim,                                                                                                                  ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.ReverseClaimDenied } },
			{ ApplicationStateInternal.ClaimApproved, new[] {               ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication,                                                         ApplicationWorkflowTrigger.EditTrainingProvider,                                                        ApplicationWorkflowTrigger.EditTrainingProgram,                                                                                                    ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton, ApplicationWorkflowTrigger.ValidateTrainingProvider,                                                   ApplicationWorkflowTrigger.ReassignAssessor, ApplicationWorkflowTrigger.CancelAgreementMinistry,                                                    ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationWorkflowTrigger.ViewParticipants, ApplicationWorkflowTrigger.EditParticipants, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants, ApplicationWorkflowTrigger.CreateClaim,    ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim, ApplicationWorkflowTrigger.SubmitClaim, ApplicationWorkflowTrigger.CloseClaimReporting, ApplicationWorkflowTrigger.AmendClaim,                                                                                                                  ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.ReverseClaimApproved } },
			{ ApplicationStateInternal.Closed, new[] {                      ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication,                                                                                                                                                                                                                                                                                                                    ApplicationWorkflowTrigger.EditSummary,                                                          ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                        ApplicationWorkflowTrigger.ReassignAssessor,                                                                                                                                                        ApplicationWorkflowTrigger.ViewParticipants,                                                                                                                                                            ApplicationWorkflowTrigger.ViewClaim,                                           ApplicationWorkflowTrigger.EnableClaimReporting, ApplicationWorkflowTrigger.EnableCompletionReporting,                                                                                                                                      ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests } },
			{ ApplicationStateInternal.CompletionReporting, new[] {         ApplicationWorkflowTrigger.ViewApplication, ApplicationWorkflowTrigger.EditApplication,                                                                                                                                                                                                                                                                                                                    ApplicationWorkflowTrigger.EditSummary,  ApplicationWorkflowTrigger.EditApplicationAttachments,  ApplicationWorkflowTrigger.EditApplicant, ApplicationWorkflowTrigger.EditProgramDescription, ApplicationWorkflowTrigger.EditApplicantContact, ApplicationWorkflowTrigger.ChangeApplicantContactButton,                                                                                                        ApplicationWorkflowTrigger.ReassignAssessor,                                                                                                                                                        ApplicationWorkflowTrigger.ViewParticipants,                                                                                                                                                            ApplicationWorkflowTrigger.ViewClaim, ApplicationWorkflowTrigger.EditClaim,     ApplicationWorkflowTrigger.EnableClaimReporting, ApplicationWorkflowTrigger.Close, ApplicationWorkflowTrigger.SubmitCompletionReport,                                                                                                       ApplicationWorkflowTrigger.GeneratePaymentRequest, ApplicationWorkflowTrigger.HoldPaymentRequests, ApplicationWorkflowTrigger.ReverseClaimApproved } }
		}.ToImmutableDictionary();

		/// <summary>
		/// Get all the valid workflow triggers that can be performed on the specified internal state.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IEnumerable<ApplicationWorkflowTrigger> GetValidWorkflowTriggers(this ApplicationStateInternal state)
		{
			return ValidWorkflowTriggers[state];
		}

		/// <summary>
		/// Get all the valid internal states that the specified workflow trigger is allowed to be performed on.
		/// </summary>
		/// <param name="trigger"></param>
		/// <returns></returns>
		public static IEnumerable<ApplicationStateInternal> GetValidInternalStates(this ApplicationWorkflowTrigger trigger)
		{
			return ValidWorkflowTriggers.Where(wft => wft.Value.Contains(trigger)).Select(wft => wft.Key).Distinct();
		}

		/// <summary>
		/// Provides the workflow trigger, claim state, external state and error message for each claim action to handle postbacks for ClaimAssessment
		/// </summary>
		public static readonly IDictionary<string, Tuple<ApplicationWorkflowTrigger, ApplicationStateExternal, string, string, string>> ClaimActionMappings = new Dictionary<string, Tuple<ApplicationWorkflowTrigger, ApplicationStateExternal, string, string, string>>()
		{
			{ "ApproveClaim", Tuple.Create(ApplicationWorkflowTrigger.ApproveClaim, ApplicationStateExternal.ClaimApproved, "Could not approve claim", string.Empty, string.Empty) },
			{ "AssessEligibility", Tuple.Create(ApplicationWorkflowTrigger.AssessEligibility, ApplicationStateExternal.ClaimSubmitted, "Could not select claim for assessment", string.Empty, string.Empty) },
			{ "AssessReimbursement", Tuple.Create(ApplicationWorkflowTrigger.AssessReimbursement, ApplicationStateExternal.ClaimSubmitted, "Could not select claim for reimbursement assessment", string.Empty, string.Empty) },
			{ "DenyClaim", Tuple.Create(ApplicationWorkflowTrigger.DenyClaim, ApplicationStateExternal.ClaimDenied, "Could not deny claim", string.Empty, string.Empty) },
			{ "RemoveClaimFromAssessment", Tuple.Create(ApplicationWorkflowTrigger.RemoveClaimFromAssessment, ApplicationStateExternal.ClaimSubmitted, "Could not remove claim from assessment", string.Empty, string.Empty) },
			{ "ReturnClaimToApplicant", Tuple.Create(ApplicationWorkflowTrigger.ReturnClaimToApplicant, ApplicationStateExternal.ClaimReturned, "'Return to applicant' action is no longer valid for current grant file state of 'Claim returned to applicant'", "ApplicationDetailsView", "Application") },
			{ "SelectClaimForAssessment", Tuple.Create(ApplicationWorkflowTrigger.SelectClaimForAssessment, ApplicationStateExternal.ClaimSubmitted, "Could not approve claim", string.Empty, string.Empty) }
		}.ToImmutableDictionary();

		/// <summary>
		/// Determine if all of the specified application workflow triggers are valid for the current state.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="triggers"></param>
		/// <returns></returns>
		public static bool IsValidWorkflowTrigger(this ApplicationStateInternal state, params ApplicationWorkflowTrigger[] triggers)
		{
			return state.GetValidWorkflowTriggers().Intersect(triggers).Count() == triggers.Count();
		}

		/// <summary>
		/// Provides all the internal state results of the specified workflow trigger.
		/// </summary>
		private static readonly IDictionary<ApplicationWorkflowTrigger, ApplicationStateInternal> WorkflowTriggerStateResult = new Dictionary<ApplicationWorkflowTrigger, ApplicationStateInternal>
		{
			{ ApplicationWorkflowTrigger.SubmitApplication, ApplicationStateInternal.New },
			{ ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft, ApplicationStateInternal.Draft },
			{ ApplicationWorkflowTrigger.SelectForAssessment, ApplicationStateInternal.PendingAssessment },
			{ ApplicationWorkflowTrigger.ReturnUnfundedApplications, ApplicationStateInternal.Unfunded },
			{ ApplicationWorkflowTrigger.ReturnUnassessed, ApplicationStateInternal.ReturnedUnassessed },
			{ ApplicationWorkflowTrigger.ReturnUnassessedToNew, ApplicationStateInternal.New },
			{ ApplicationWorkflowTrigger.RemoveFromAssessment, ApplicationStateInternal.New },
			{ ApplicationWorkflowTrigger.BeginAssessment, ApplicationStateInternal.UnderAssessment },
			{ ApplicationWorkflowTrigger.RecommendForApproval, ApplicationStateInternal.RecommendedForApproval },
			{ ApplicationWorkflowTrigger.RecommendForDenial, ApplicationStateInternal.RecommendedForDenial },
			{ ApplicationWorkflowTrigger.IssueOffer, ApplicationStateInternal.OfferIssued },
			{ ApplicationWorkflowTrigger.ReturnOfferToAssessment, ApplicationStateInternal.UnderAssessment },
			{ ApplicationWorkflowTrigger.ReturnToAssessment, ApplicationStateInternal.ReturnedToAssessment },
			{ ApplicationWorkflowTrigger.DenyApplication, ApplicationStateInternal.ApplicationDenied },
			{ ApplicationWorkflowTrigger.WithdrawOffer, ApplicationStateInternal.OfferWithdrawn },
			{ ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment, ApplicationStateInternal.UnderAssessment },
			{ ApplicationWorkflowTrigger.AcceptGrantAgreement, ApplicationStateInternal.AgreementAccepted },
			{ ApplicationWorkflowTrigger.RejectGrantAgreement, ApplicationStateInternal.AgreementRejected },
			{ ApplicationWorkflowTrigger.CancelAgreementMinistry, ApplicationStateInternal.CancelledByMinistry },
			{ ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry, ApplicationStateInternal.UnderAssessment },
			{ ApplicationWorkflowTrigger.CancelAgreementHolder, ApplicationStateInternal.CancelledByAgreementHolder },
			{ ApplicationWorkflowTrigger.WithdrawApplication, ApplicationStateInternal.ApplicationWithdrawn },
			{ ApplicationWorkflowTrigger.SubmitChangeRequest, ApplicationStateInternal.ChangeRequest },
			{ ApplicationWorkflowTrigger.RecommendChangeForApproval, ApplicationStateInternal.ChangeForApproval },
			{ ApplicationWorkflowTrigger.RecommendChangeForDenial, ApplicationStateInternal.ChangeForDenial },
			{ ApplicationWorkflowTrigger.ApproveChangeRequest, ApplicationStateInternal.AgreementAccepted },
			{ ApplicationWorkflowTrigger.ReturnChangeToAssessment, ApplicationStateInternal.ChangeReturned },
			{ ApplicationWorkflowTrigger.DenyChangeRequest, ApplicationStateInternal.ChangeRequestDenied },
			{ ApplicationWorkflowTrigger.SubmitClaim, ApplicationStateInternal.NewClaim },
			{ ApplicationWorkflowTrigger.SelectClaimForAssessment, ApplicationStateInternal.ClaimAssessEligibility },
			{ ApplicationWorkflowTrigger.RemoveClaimFromAssessment, ApplicationStateInternal.NewClaim },
			{ ApplicationWorkflowTrigger.ReturnClaimToApplicant, ApplicationStateInternal.ClaimReturnedToApplicant },
			{ ApplicationWorkflowTrigger.ReverseClaimReturnedToApplicant, ApplicationStateInternal.NewClaim },
			{ ApplicationWorkflowTrigger.ReverseClaimDenied, ApplicationStateInternal.NewClaim },
			{ ApplicationWorkflowTrigger.ReverseClaimApproved, ApplicationStateInternal.NewClaim },
			{ ApplicationWorkflowTrigger.AssessReimbursement, ApplicationStateInternal.ClaimAssessReimbursement },
			{ ApplicationWorkflowTrigger.AssessEligibility, ApplicationStateInternal.ClaimAssessEligibility },
			{ ApplicationWorkflowTrigger.ApproveClaim, ApplicationStateInternal.ClaimApproved },
			{ ApplicationWorkflowTrigger.DenyClaim, ApplicationStateInternal.ClaimDenied },
			{ ApplicationWorkflowTrigger.WithdrawClaim, ApplicationStateInternal.ClaimReturnedToApplicant },
			{ ApplicationWorkflowTrigger.Close, ApplicationStateInternal.Closed },
			{ ApplicationWorkflowTrigger.AmendClaim, ApplicationStateInternal.AgreementAccepted },
			{ ApplicationWorkflowTrigger.CloseClaimReporting, ApplicationStateInternal.CompletionReporting },
			{ ApplicationWorkflowTrigger.EnableCompletionReporting, ApplicationStateInternal.CompletionReporting },
			{ ApplicationWorkflowTrigger.EnableClaimReporting, ApplicationStateInternal.AgreementAccepted },
			{ ApplicationWorkflowTrigger.SubmitCompletionReport, ApplicationStateInternal.Closed }
		}.ToImmutableDictionary();

		/// <summary>
		/// Get the resulting internal state of the specified <typeparamref name="ApplicationWorkflowTrigger"/> action.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="trigger"></param>
		/// <returns></returns>
		public static ApplicationStateInternal GetResultingState(this ApplicationStateInternal state, ApplicationWorkflowTrigger trigger)
		{
			if (WorkflowTriggerStateResult.ContainsKey(trigger))
				return WorkflowTriggerStateResult[trigger];

			return state;
		}

		/// <summary>
		/// Provides all valid internal state transitions from and to.
		/// </summary>
		private static readonly IDictionary<ApplicationStateInternal, IEnumerable<ApplicationStateInternal>> InternalStateTransitions = new Dictionary<ApplicationStateInternal, IEnumerable<ApplicationStateInternal>>()
		{
			{ ApplicationStateInternal.Draft, new[] { ApplicationStateInternal.New } },
			{ ApplicationStateInternal.New, new[] { ApplicationStateInternal.PendingAssessment, ApplicationStateInternal.Unfunded, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.ReturnedUnassessed } },
			{ ApplicationStateInternal.PendingAssessment, new[] { ApplicationStateInternal.UnderAssessment, ApplicationStateInternal.New, ApplicationStateInternal.ApplicationWithdrawn } },
			{ ApplicationStateInternal.UnderAssessment, new[] { ApplicationStateInternal.Draft, ApplicationStateInternal.RecommendedForApproval, ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationWithdrawn } },
			{ ApplicationStateInternal.ReturnedToAssessment, new[] { ApplicationStateInternal.RecommendedForApproval, ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationWithdrawn } },
			{ ApplicationStateInternal.RecommendedForApproval, new[] { ApplicationStateInternal.OfferIssued, ApplicationStateInternal.ReturnedToAssessment, ApplicationStateInternal.ApplicationWithdrawn } },
			{ ApplicationStateInternal.RecommendedForDenial, new[] { ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ReturnedToAssessment, ApplicationStateInternal.ApplicationWithdrawn } },
			{ ApplicationStateInternal.OfferIssued, new[] { ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.AgreementRejected, ApplicationStateInternal.UnderAssessment } },
			{ ApplicationStateInternal.OfferWithdrawn, new[] { ApplicationStateInternal.UnderAssessment } },
			{ ApplicationStateInternal.AgreementAccepted, new[] { ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.NewClaim, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder, ApplicationStateInternal.CompletionReporting } },
			{ ApplicationStateInternal.Unfunded, new[] { ApplicationStateInternal.Draft } },
			{ ApplicationStateInternal.ReturnedUnassessed, new[] { ApplicationStateInternal.New } },
			{ ApplicationStateInternal.ApplicationDenied, new[] { ApplicationStateInternal.ReturnedToAssessment } },
			{ ApplicationStateInternal.AgreementRejected, new ApplicationStateInternal[0] },
			{ ApplicationStateInternal.ApplicationWithdrawn, new[] { ApplicationStateInternal.Draft } },
			{ ApplicationStateInternal.CancelledByMinistry, new [] { ApplicationStateInternal.UnderAssessment, ApplicationStateInternal.ClaimReturnedToApplicant } },
			{ ApplicationStateInternal.CancelledByAgreementHolder, new ApplicationStateInternal[0] },
			{ ApplicationStateInternal.ChangeRequest, new[] { ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder } },
			{ ApplicationStateInternal.ChangeForApproval, new[] { ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeReturned, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder } },
			{ ApplicationStateInternal.ChangeForDenial, new[] { ApplicationStateInternal.ChangeRequestDenied, ApplicationStateInternal.ChangeReturned, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder } },
			{ ApplicationStateInternal.ChangeReturned, new[] { ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder } },
			{ ApplicationStateInternal.ChangeRequestDenied, new[] { ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.NewClaim, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder, ApplicationStateInternal.CompletionReporting } },
			{ ApplicationStateInternal.NewClaim, new[] { ApplicationStateInternal.ClaimAssessEligibility, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.ClaimReturnedToApplicant } },
			{ ApplicationStateInternal.ClaimAssessEligibility, new[] { ApplicationStateInternal.ClaimReturnedToApplicant, ApplicationStateInternal.ClaimAssessReimbursement, ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.ClaimReturnedToApplicant, ApplicationStateInternal.NewClaim } },
			{ ApplicationStateInternal.ClaimAssessReimbursement, new[] { ApplicationStateInternal.ClaimReturnedToApplicant, ApplicationStateInternal.ClaimAssessEligibility, ApplicationStateInternal.ClaimApproved, ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.ClaimReturnedToApplicant } },
			{ ApplicationStateInternal.ClaimReturnedToApplicant, new[] { ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.NewClaim, ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.CompletionReporting } },
			{ ApplicationStateInternal.ClaimDenied, new[] { ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.NewClaim, ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.CompletionReporting } },
			{ ApplicationStateInternal.ClaimApproved, new[] { ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.NewClaim, ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.CompletionReporting } },
			{ ApplicationStateInternal.CompletionReporting, new [] { ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.Closed, ApplicationStateInternal.NewClaim } },
			{ ApplicationStateInternal.Closed, new [] { ApplicationStateInternal.CompletionReporting, ApplicationStateInternal.AgreementAccepted } }
		}.ToImmutableDictionary();

		/// <summary>
		/// Get all valid internal states that the specified state can be transitioned into.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IEnumerable<ApplicationStateInternal> GetValidTransitions(this ApplicationStateInternal state)
		{
			return InternalStateTransitions[state];
		}

		/// <summary>
		/// Determine whether the internal state change transition is valid.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">There is no valid transition mapped for the specified states.</exception>
		/// <param name="from">The current state.</param>
		/// <param name="to">The desired state.</param>
		/// <returns>True if the state transition is valid.</returns>
		public static bool IsValidStateTransition(this ApplicationStateInternal from, ApplicationStateInternal to)
		{
			if (!InternalStateTransitions.ContainsKey(from))
				throw new InvalidOperationException($"There is no valid state change from '{from.ToString("g")}' to '{to.ToString("g")}'.");

			return InternalStateTransitions[from].Contains(to);
		}

		/// <summary>
		/// Determines whether the internal state change transition is valid.
		/// This method calls IsValidStateTransition if the specified from and to states are different.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>True if the state transition is valid, or if the states are the same.</returns>
		public static bool AllowStateTransition(this ApplicationStateInternal from, ApplicationStateInternal to)
		{
			if (from != to)
				return from.IsValidStateTransition(to);

			return true;
		}

		/// <summary>
		/// Provides all valid external state transitions from and to.
		/// </summary>
		private static readonly IDictionary<ApplicationStateExternal, IEnumerable<ApplicationStateExternal>> ExternalStateTransitions = new Dictionary<ApplicationStateExternal, IEnumerable<ApplicationStateExternal>>()
		{
			{ ApplicationStateExternal.NotStarted, new[] { ApplicationStateExternal.Incomplete } },
			{ ApplicationStateExternal.Incomplete, new[] { ApplicationStateExternal.Complete } },
			{ ApplicationStateExternal.Complete, new[] { ApplicationStateExternal.Submitted, ApplicationStateExternal.Incomplete } },
			{ ApplicationStateExternal.Submitted, new[] { ApplicationStateExternal.Incomplete, ApplicationStateExternal.ApplicationWithdrawn, ApplicationStateExternal.ApplicationDenied, ApplicationStateExternal.AcceptGrantAgreement, ApplicationStateExternal.NotAccepted, ApplicationStateExternal.ReturnedUnassessed } },
			{ ApplicationStateExternal.ApplicationWithdrawn, new[] { ApplicationStateExternal.Incomplete } },
			{ ApplicationStateExternal.ApplicationDenied, new[] { ApplicationStateExternal.Submitted }},
			{ ApplicationStateExternal.AcceptGrantAgreement, new[] { ApplicationStateExternal.AgreementWithdrawn, ApplicationStateExternal.Approved, ApplicationStateExternal.AgreementRejected, ApplicationStateExternal.Submitted } },
			{ ApplicationStateExternal.AgreementWithdrawn, new [] { ApplicationStateExternal.Submitted } },
			{ ApplicationStateExternal.ReturnedUnassessed, new[] { ApplicationStateExternal.Submitted } },
			{ ApplicationStateExternal.NotAccepted, new[] { ApplicationStateExternal.Incomplete, ApplicationStateExternal.Complete } },
			{ ApplicationStateExternal.Approved, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.CancelledByAgreementHolder, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.ReportCompletion } },
			{ ApplicationStateExternal.CancelledByMinistry, new [] { ApplicationStateExternal.Approved, ApplicationStateExternal.ClaimReturned, ApplicationStateExternal.Submitted } },
			{ ApplicationStateExternal.CancelledByAgreementHolder, new ApplicationStateExternal[0] },
			{ ApplicationStateExternal.ChangeRequestSubmitted, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.CancelledByAgreementHolder, ApplicationStateExternal.ChangeRequestDenied, ApplicationStateExternal.ChangeRequestApproved } },
			{ ApplicationStateExternal.ChangeRequestDenied, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.CancelledByAgreementHolder, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.ReportCompletion } },
			{ ApplicationStateExternal.ChangeRequestApproved, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.CancelledByAgreementHolder, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.ReportCompletion } },
			{ ApplicationStateExternal.AgreementRejected, new ApplicationStateExternal[0] },
			{ ApplicationStateExternal.ClaimSubmitted, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.ClaimReturned, ApplicationStateExternal.ClaimDenied, ApplicationStateExternal.ClaimApproved } },
			{ ApplicationStateExternal.ClaimReturned, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.ReportCompletion } },
			{ ApplicationStateExternal.ClaimDenied, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.ReportCompletion, ApplicationStateExternal.Approved } },
			{ ApplicationStateExternal.ClaimApproved, new[] { ApplicationStateExternal.CancelledByMinistry, ApplicationStateExternal.ChangeRequestSubmitted, ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.Closed, ApplicationStateExternal.ReportCompletion, ApplicationStateExternal.Approved } },
			{ ApplicationStateExternal.Closed, new [] { ApplicationStateExternal.ReportCompletion, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.Approved } },
			{ ApplicationStateExternal.ReportCompletion, new [] { ApplicationStateExternal.Closed, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.Approved, ApplicationStateExternal.ClaimApproved, ApplicationStateExternal.ClaimSubmitted } },
			{ ApplicationStateExternal.AmendClaim, new[] { ApplicationStateExternal.ClaimSubmitted, ApplicationStateExternal.ReportCompletion, ApplicationStateExternal.ChangeRequestSubmitted } }
		}.ToImmutableDictionary();

		/// <summary>
		/// Get all valid external states that the specified state can be transitioned into.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IEnumerable<ApplicationStateExternal> GetValidTransitions(this ApplicationStateExternal state)
		{
			return ExternalStateTransitions[state];
		}

		/// <summary>
		/// Determine whether the external state change transition is valid.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">There is no valid transition mapped for the specified states.</exception>
		/// <param name="from">The current state.</param>
		/// <param name="to">The desired state.</param>
		/// <returns>True if the state transition is valid.</returns>
		public static bool IsValidStateTransition(this ApplicationStateExternal from, ApplicationStateExternal to)
		{
			if (!ExternalStateTransitions.ContainsKey(from))
				throw new InvalidOperationException($"There is no valid state change from '{from.ToString("g")}' to '{to.ToString("g")}'.");

			return ExternalStateTransitions[from].Contains(to);
		}

		/// <summary>
		/// Determines whether the external state change transition is valid.
		/// This method calls IsValidStateTransition if the specified from and to states are different.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>True if the state transition is valid, or if the states are the same.</returns>
		public static bool AllowStateTransition(this ApplicationStateExternal from, ApplicationStateExternal to)
		{
			if (from != to)
				return from.IsValidStateTransition(to);

			return true;
		}

		/// <summary>
		/// Determines if the specified application workflow trigger is for the external application.
		/// </summary>
		/// <param name="trigger"></param>
		/// <returns></returns>
		public static bool IsExternalWorkflowTrigger(this ApplicationWorkflowTrigger trigger)
		{
			switch (trigger)
			{
				case ApplicationWorkflowTrigger.SubmitApplication:
				case ApplicationWorkflowTrigger.WithdrawApplication:
				case ApplicationWorkflowTrigger.WithdrawClaim:
				case ApplicationWorkflowTrigger.AcceptGrantAgreement:
				case ApplicationWorkflowTrigger.RejectGrantAgreement:
				case ApplicationWorkflowTrigger.CancelAgreementHolder:
				case ApplicationWorkflowTrigger.SubmitChangeRequest:
				case ApplicationWorkflowTrigger.SubmitClaim:
				case ApplicationWorkflowTrigger.EditParticipants:
				case ApplicationWorkflowTrigger.SubmitCompletionReport:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Depending on state it should show agreed costs or estimated costs.
		/// </summary>
		/// <param name="applicationState"></param>
		/// <returns></returns>
		public static bool ShowAgreedCosts(this ApplicationStateInternal applicationState)
		{
			return !applicationState.In(
				ApplicationStateInternal.Draft,
				ApplicationStateInternal.ApplicationWithdrawn);
		}

		/// <summary>
		/// Returns an array of valid internal states to determine the number of applications an applicant has.
		/// </summary>
		/// <returns></returns>
		public static ApplicationStateInternal[] GetInternalStatesForSummary()
		{
			return new[]
			{
					ApplicationStateInternal.AgreementAccepted,
					ApplicationStateInternal.ChangeForApproval,
					ApplicationStateInternal.ChangeReturned,
					ApplicationStateInternal.ChangeRequest,
					ApplicationStateInternal.ChangeForDenial,
					ApplicationStateInternal.ChangeRequestDenied,
					ApplicationStateInternal.Closed,
					ApplicationStateInternal.ClaimReturnedToApplicant,
					ApplicationStateInternal.ClaimApproved,
					ApplicationStateInternal.NewClaim,
					ApplicationStateInternal.ClaimAssessEligibility,
					ApplicationStateInternal.ClaimAssessReimbursement,
					ApplicationStateInternal.ClaimDenied,
					ApplicationStateInternal.CompletionReporting
			};
		}
	}
}
