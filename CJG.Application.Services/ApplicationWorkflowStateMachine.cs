using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using Stateless;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ApplicationWorkflowStateMachine"/> class, provides valid workflow transitions and validation for the application.
	/// </summary>
	public class ApplicationWorkflowStateMachine : Service
	{
		GrantApplication _grantApplication;
		ApplicationStateInternal _originalState;

		private readonly INotificationService _notificationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IPrioritizationService _prioritizationService;
		private readonly INoteService _noteService;
		private readonly IUserService _userService;

		StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger> _stateMachine;

		// Create parametrized triggers
		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<InternalUser>
			_beginAssessmentTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_submitClaimTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim, string, IClaimService>
			_withdrawClaimTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_withdrawApplicationTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_returnToAssessmentTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_recommendForApprovalTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_recommendForDenialTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_rejectGrantAgreementTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_denyApplicationTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_withdrawOfferTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_cancelAgreementMinistryTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_closeGrantFileTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_cancelAgreementHolderTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_returnChangeToAssessmentTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_recommendChangeForDenialTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_denyChangeRequestTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<string>
			_submitChangeRequestTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_selectClaimForAssessmentTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_removeClaimFromAssessmentTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_assessClaimReimbursementTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_assessClaimEligibilityTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_approveClaimTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_denyClaimTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim, string, IClaimService>
			_returnClaimToApplicantTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim, IClaimService>
			_returnClaimToNewTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim, IClaimService>
			_reverseClaimDeniedTrigger;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>.TriggerWithParameters<Claim>
			_initiateClaimAmendmentTrigger;

		#region Constructors
		public ApplicationWorkflowStateMachine(
			GrantApplication grantApplication,
			IDataContext dbContext,
			INotificationService notificationService,
			IGrantAgreementService grantAgreementService,
			IGrantOpeningService grantOpeningService,
			IPrioritizationService prioritizationService,
			INoteService noteService,
			IUserService userService,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			_grantApplication = grantApplication;
			_notificationService = notificationService;
			_grantAgreementService = grantAgreementService;
			_grantOpeningService = grantOpeningService;
			_prioritizationService = prioritizationService;
			_noteService = noteService;
			_userService = userService;

			// Instantiate state machine based on current Grant Application state
			_stateMachine = new StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>(() => _grantApplication.ApplicationStateInternal,
																									s => _grantApplication.ApplicationStateInternal = s);
			_originalState = _stateMachine.State;

			// Configure parametrized triggers
			_beginAssessmentTrigger = _stateMachine.SetTriggerParameters<InternalUser>(ApplicationWorkflowTrigger.BeginAssessment);
			_withdrawApplicationTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.WithdrawApplication);
			_returnToAssessmentTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.ReturnToAssessment);
			_recommendForApprovalTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.RecommendForApproval);
			_recommendForDenialTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.RecommendForDenial);
			_rejectGrantAgreementTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.RejectGrantAgreement);
			_denyApplicationTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.DenyApplication);
			_withdrawOfferTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.WithdrawOffer);
			_cancelAgreementMinistryTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.CancelAgreementMinistry);
			_closeGrantFileTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.Close);
			_cancelAgreementHolderTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.CancelAgreementHolder);
			_returnChangeToAssessmentTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.ReturnChangeToAssessment);
			_recommendChangeForDenialTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.RecommendChangeForDenial);
			_denyChangeRequestTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.DenyChangeRequest);
			_submitChangeRequestTrigger = _stateMachine.SetTriggerParameters<string>(ApplicationWorkflowTrigger.SubmitChangeRequest);
			_submitClaimTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.SubmitClaim);
			_withdrawClaimTrigger = _stateMachine.SetTriggerParameters<Claim, string, IClaimService>(ApplicationWorkflowTrigger.WithdrawClaim);
			_selectClaimForAssessmentTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.SelectClaimForAssessment);
			_removeClaimFromAssessmentTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.RemoveClaimFromAssessment);
			_assessClaimEligibilityTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.AssessEligibility);
			_assessClaimReimbursementTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.AssessReimbursement);
			_approveClaimTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.ApproveClaim);
			_denyClaimTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.DenyClaim);
			_returnClaimToApplicantTrigger = _stateMachine.SetTriggerParameters<Claim, string, IClaimService>(ApplicationWorkflowTrigger.ReturnClaimToApplicant);
			_returnClaimToNewTrigger = _stateMachine.SetTriggerParameters<Claim, IClaimService>(ApplicationWorkflowTrigger.ReverseClaimReturnedToApplicant);
			_reverseClaimDeniedTrigger = _stateMachine.SetTriggerParameters<Claim, IClaimService>(ApplicationWorkflowTrigger.ReverseClaimDenied);
			_initiateClaimAmendmentTrigger = _stateMachine.SetTriggerParameters<Claim>(ApplicationWorkflowTrigger.AmendClaim);

			ConfigureStateTransitions();
		}

		private void ConfigureStateTransitions()
		{
			_stateMachine.Configure(ApplicationStateInternal.Draft)
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft, t => OnUnderAssessmentReturnedToDraft())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.New)
				.OnEntryFrom(ApplicationWorkflowTrigger.SubmitApplication, t => OnSubmitApplication())
				.OnEntryFrom(ApplicationWorkflowTrigger.RemoveFromAssessment, t => OnRemoveFromAssessment())
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnUnassessedToNew, t => OnReturnUnassessedToNew())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.PendingAssessment)
				.OnEntryFrom(ApplicationWorkflowTrigger.SelectForAssessment, t => OnSelectForAssessment())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.UnderAssessment)
				.OnEntryFrom(_beginAssessmentTrigger, assessor => OnBeginAssessment(assessor))
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnOfferToAssessment, t => OnReturnOfferToAssessment())
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment, t => OnReturnWithdrawnOfferToAssessment())
				.OnEntryFrom(ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry, t => OnReverseCancelAgreementMinistry())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ReturnedToAssessment)
				.OnEntryFrom(_returnToAssessmentTrigger, reason => OnReturnedToAssessment(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.RecommendedForApproval)
				.OnEntryFrom(_recommendForApprovalTrigger, reason => OnRecommendForApproval(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.RecommendedForDenial)
				.OnEntryFrom(_recommendForDenialTrigger, reason => OnRecommendForDenial(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.OfferIssued)
				.OnEntryFrom(ApplicationWorkflowTrigger.IssueOffer, grantAgreement => OnIssueOffer())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.AgreementAccepted)
				.OnEntryFrom(ApplicationWorkflowTrigger.AcceptGrantAgreement, t => OnAcceptGrantAgreement())
				.OnEntryFrom(ApplicationWorkflowTrigger.ApproveChangeRequest, t => OnApproveChangeRequest())
				.OnEntryFrom(_initiateClaimAmendmentTrigger, claim => OnInitiateClaimAmendment(claim))
				.OnEntryFrom(ApplicationWorkflowTrigger.EnableClaimReporting, t => OnEnableClaimReporting())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.AgreementRejected)
				.OnEntryFrom(_rejectGrantAgreementTrigger, (reason) => OnRejectGrantAgreement(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ApplicationWithdrawn)
				.OnEntryFrom(_withdrawApplicationTrigger, (reason) => OnWithdrawApplication(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ApplicationDenied)
				.OnEntryFrom(_denyApplicationTrigger, reason => OnDenyApplication(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.OfferWithdrawn)
				.OnEntryFrom(_withdrawOfferTrigger, reason => OnWithdrawOffer(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.CancelledByMinistry)
				.OnEntryFrom(_cancelAgreementMinistryTrigger, reason => OnCancelAgreementMinistry(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.CancelledByAgreementHolder)
				.OnEntryFrom(_cancelAgreementHolderTrigger, reason => OnCancelAgreementHolder(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.Unfunded)
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnUnfundedApplications, t => OnReturnUnfundedApplications())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ReturnedUnassessed)
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnUnassessed, t => OnReturnApplicationUnassessed())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ChangeRequest)
				.OnEntryFrom(_submitChangeRequestTrigger, reason => OnSubmitChangeRequest(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ChangeReturned)
				.OnEntryFrom(_returnChangeToAssessmentTrigger, reason => OnReturnChangeToAssessment(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ChangeForApproval)
				.OnEntryFrom(ApplicationWorkflowTrigger.RecommendChangeForApproval, t => OnRecommendChangeForApprovalOrDenial())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ChangeForDenial)
				.OnEntryFrom(_recommendChangeForDenialTrigger, reason => OnRecommendChangeForApprovalOrDenial(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ChangeRequestDenied)
				.OnEntryFrom(_denyChangeRequestTrigger, reason => OnDenyChangeRequest(reason))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.NewClaim)
				.OnEntryFrom(_submitClaimTrigger, claim => OnSubmitClaim(claim))
				.OnEntryFrom(_removeClaimFromAssessmentTrigger, claim => OnRemoveClaimFromAssessment(claim))
				.OnEntryFrom(_returnClaimToNewTrigger, (claim, service) => OnReturnClaimToNew(claim, service))
				.OnEntryFrom(_reverseClaimDeniedTrigger, (claim, service) => OnReverseClaimDenied(claim, service))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ClaimReturnedToApplicant)
				.OnEntryFrom(_withdrawClaimTrigger, (claim, reason, service) => OnWithdrawClaim(claim, reason, service))
				.OnEntryFrom(_returnClaimToApplicantTrigger, (claim, reason, service) => OnReturnClaimToApplicant(claim, reason, service))
				.OnEntryFrom(ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry, t => OnReverseCancelAgreementMinistry())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ClaimAssessEligibility)
				.OnEntryFrom(_selectClaimForAssessmentTrigger, claim => OnSelectClaimForAssessment(claim))
				.OnEntryFrom(_assessClaimEligibilityTrigger, claim => OnAssessClaimEligibility(claim))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ClaimAssessReimbursement)
				.OnEntryFrom(_assessClaimReimbursementTrigger, claim => OnAssessClaimReimbursement(claim))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ClaimApproved)
				.OnEntryFrom(_approveClaimTrigger, claim => OnApproveClaim(claim))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ClaimDenied)
				.OnEntryFrom(_denyClaimTrigger, claim => OnDenyClaim(claim))
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.CompletionReporting)
				.OnEntryFrom(ApplicationWorkflowTrigger.CloseClaimReporting, t => OnCloseClaimReporting())
				.OnEntryFrom(ApplicationWorkflowTrigger.EnableCompletionReporting, t => OnEnableCompletionReporting())
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.Closed)
				.OnEntryFrom(_closeGrantFileTrigger, reason => OnCloseGrantFile(reason))
				.OnEntryFrom(ApplicationWorkflowTrigger.SubmitCompletionReport,
					t => OnCloseGrantFile("Submitted Completion Report"))
				.Permits(_grantApplication.ApplicationStateInternal);
		}

		#endregion

		#region Methods
		/// <summary>
		/// Update the current grant application.
		/// </summary>
		private void UpdateGrantApplication()
		{
			HandleWorkflowNotifications();

			_dbContext.SetModified(_grantApplication);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Validates whether the current user can perform the specified trigger.
		/// If they are not allowed it will throw a NotAuthorizedException.
		/// </summary>
		/// <exception cref="NotAuthorizedException">The user is not allowed to perform the specified trigger.</exception>
		/// <param name="trigger"></param>
		private void CanPerformTrigger(ApplicationWorkflowTrigger trigger)
		{
			if (!_httpContext.User.CanPerformAction(_grantApplication, trigger))
			{
				if (!_grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers().Contains(trigger))
					throw new NotAuthorizedException($"The action '{trigger.GetDescription()}' is not valid for the current application state '{_grantApplication.ApplicationStateInternal.GetDescription()}'.");

				throw new NotAuthorizedException($"User is not authorized to perform the action '{trigger.GetDescription()}'.");
			}
		}

		#region State Change Events
		/// <summary>
		/// Confirmation:
		/// If you withdraw your grant application then it will be closed and will not be given further consideration by the Ministry.
		/// Are you sure you want to withdraw your application?
		/// Application Administrator provides reason.
		/// Financial: Release a reservation for applications in states marked(R).
		/// Change external state to “Withdrawn”.
		/// Application Administrator Withdraws Application -- If an application is withdrawn prior to completing assessment then we can
		/// have the system allow this and automatically return the application to the application administrator in an “Unsubmitted” state.
		/// This would allow it to be edited and resubmitted with minimal effort and it provides for fixing an omission in an application very quickly.
		/// If the application was resubmitted then it would be a new application(new file number) and would have a new submit date and time and
		/// a new position in the queue for assessment.It is also possible that the Grant Opening may close and the application cannot be resubmitted.
		/// The thought on this approach is that you do not need a record of the assessment or lifecycle of the application.
		/// It did not have a completed assessment and so the Ministry’s record is only a log entry of the application being withdrawn.
		/// The content of the application is not retained as part of the log.  It does not need to continue to appear in active application queues.
		/// </summary>
		/// <param name="reason"></param>
		private void OnWithdrawApplication(string reason)
		{
			// Remove all line items added by Assessor.
			RemoveEligibleCostAddedByAssessor(_grantApplication);

			foreach (var trainingProgram in _grantApplication.TrainingPrograms)
			{
				trainingProgram.TrainingProgramState = TrainingProgramStates.Incomplete;

				var trainingProvider = trainingProgram?.TrainingProvider;
				if (trainingProvider != null)
				{
					trainingProvider.TrainingProviderState = TrainingProviderStates.Incomplete;
				}
			}

			foreach (var trainingProvider in _grantApplication.TrainingProviders)
			{
				trainingProvider.TrainingProviderState = TrainingProviderStates.Incomplete;
			}

			_grantApplication.ProgramDescription.DescriptionState = ProgramDescriptionStates.Incomplete;
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ApplicationWithdrawn;
			_grantApplication.TrainingCost.TrainingCostState = (int)TrainingCostStates.Incomplete;
			_grantApplication.DisableParticipantReporting();
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.WithdrawApplication);

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been withdrawn.");
		}

		/// <summary>
		/// Change external state to “Submitted”
		/// Financial:
		/// Note: Application.CostEstimate is set to the sum of total estimated costs identified in the application file whenever
		/// the application is saved in draft state by the Application Administrator.
		/// Application.AssessedCommitment is set to zero for a new application.
		/// </summary>
		private void OnSubmitApplication()
		{
			try
			{
				if (_grantApplication.IsSubmittable())
				{
					_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.SubmitApplication);
					_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
					_grantApplication.DateSubmitted = AppDateTime.UtcNow;
					_grantApplication.TrainingCost.CopyEstimatedIntoAgreed();

					var breakdown = _prioritizationService.GetBreakdown(_grantApplication);
					_grantApplication.PrioritizationScoreBreakdown = breakdown;
					_grantApplication.PrioritizationScore = breakdown?.GetTotalScore() ?? 0;
					LogStateChanges();

					if (_grantApplication.PrioritizationScoreBreakdown != null
					    &&_grantApplication.PrioritizationScoreBreakdown.HasRegionalException())
						_noteService.AddWorkflowNote(_grantApplication, "Priority list region lookup Exception. Postal Code not found in region list.");

					UpdateGrantApplication();

					_logger.Info($"Grant application {_grantApplication.Id} has been submitted.");
				}
				else
				{
					throw new InvalidOperationException("Application can not be submitted in current State.");
				}
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// Business rule: Begin Assessment cannot be clicked until an assessor is selected from a dropdown list of assessors.
		/// The default section for an assessor is set to me if I am an assessor.Otherwise, there is no current selection and one must be made prior to the action button click.
		/// </summary>
		/// <param name="assessor"></param>
		private void OnBeginAssessment(InternalUser assessor)
		{
			_grantApplication.Assessor = assessor;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.BeginAssessment);

			LogStateChanges();

			_noteService.AddNote(_grantApplication, NoteTypes.AA, $"{_grantApplication.Assessor?.FirstName} {_grantApplication.Assessor?.LastName} assigned as assessor");

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been begun assessment.");
		}

		/// <summary>
		/// Director annotation to file records any director notes related to approval to issue offer.
		/// Generate agreement and reveal it in application file for Application Administrator review and acceptance.
		/// The system will need to generate the variable parts of the agreement in Schedule A from the application information.
		/// Agreement parts that may change over time need to be tracked for what version of master agreement or schedules were issued.
		/// Change external application state to “Accept Grant Agreement”.
		/// Notification:
		/// "Your Canada-BC Job Grant application <fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and<TrainingPeriod.enddate> has been assessed and an offer in the form of a Grant Agreement has been added to your file.
		/// Please login to the system at: <url> and review and confirm your acceptance of the grant agreement by<FiveDaysFromTodayDate>
		/// in order to be eligible for grant benefits."
		/// Financial: The ApplicationID.AssessedCommitment is the amount that will become the commitment for this agreement when the agreement is accepted.
		/// AM4 PRIVILEGE - OVERRIDE
		/// Rule: Action buttons are only visible to user with AM4 privilege.
		/// Mark the file with an attribute recording the workflow override
		/// for application assessment.
		/// Record the override to the notes log as well: <Action> completed for application in state <PreviousState> by <username> on <date>.
		/// </summary>
		private void OnIssueOffer()
		{
			try
			{
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.AcceptGrantAgreement;
				_grantApplication.GrantAgreement = new GrantAgreement(_grantApplication);
				_grantAgreementService.GenerateDocuments(_grantApplication);

				LogStateChanges();

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} has an offer issued.");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}


		/// <summary>
		/// Reverse offer issued back to Under Assessment
		/// </summary>
		private void OnReturnOfferToAssessment()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
			_grantAgreementService.RemoveGrantAgreement(_grantApplication);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned to Under Assessment.");
		}

		/// <summary>
		/// Financial: Make reservations for total estimated costs for the application selected and all applications in the queue before the one selected;
		/// If reservation is successful then allow state change, otherwise do not allow state change and report the following user message:
		/// "Insufficient grant opening funds are available for the application(s)you have selected.Complete applications under assessment first
		/// and then try to select for assessment again."
		/// </summary>
		private void OnSelectForAssessment()
		{
			var reservedApplicationIds = _grantOpeningService.MakeReservation(_grantApplication);

			LogStateChangesForReservedApplications(reservedApplicationIds);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been selected for assessment.");
		}


		/// <summary>
		/// File annotation contains director instructions.
		/// </summary>
		private void OnUnderAssessmentReturnedToDraft()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;
			_grantApplication.DateSubmitted = null;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned to draft.");
		}

		/// <summary>
		/// Financial: Release reservation for the application selected.
		/// </summary>
		private void OnRemoveFromAssessment()
		{
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.RemoveFromAssessment);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been removed from assessment.");
		}

		/// <summary>
		/// Rule: Action button is only visible to assigned assessor.
		/// Rule: Application has been linked to training service registry to record the identity of the training service provider.
		/// Assessor annotation to file provides the outcome of assessment for the Director.
		/// Financial: The assessor editing and saving the application file with assessed eligible expenses saves place the sum of assessed
		/// costs in the attribute ApplicationID.AssessedCommitment.This amount becomes the assessed commitment when an agreement is accepted.
		/// The attribute ApplicationID.CostEstimates records the total submitted request of the Application Administrator and is not changed in the assessment process.
		/// </summary>
		private void OnRecommendForApproval(string response)
		{
			if (string.IsNullOrEmpty(response))
				LogStateChanges(response, response);
			else
			{
				dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(response, new ExpandoObjectConverter());
				string approvedReason = data.approvedReason;
				
				LogStateChanges(approvedReason, approvedReason);
			}

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been recommended for approval.");
		}

		/// <summary>
		/// Rule: Action button is only visible to assigned assessor.
		/// Rule: Application has been linked to training service provider in the training service provider list.
		/// Capture external reason for denial.
		/// Assessor annotation provides the outcome of assessment for the Director.
		/// Financial: The assessor editing and saving the application file with assessed eligible expenses saves place the sum of assessed costs
		/// in the attribute ApplicationID.AssessedCommitment.This amount becomes the assessed commitment when an agreement is accepted.
		/// The attribute ApplicationID.CostEstimates records the total submitted request of the Application Administrator and is not changed in the assessment process.
		/// </summary>
		/// <param name="response"></param>
		private void OnRecommendForDenial(string response)
		{
			if (string.IsNullOrEmpty(response))
				LogStateChanges(response, response);
			else
			{
				dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(response, new ExpandoObjectConverter());
				string denialReason = data.deniedReason;
				IEnumerable<dynamic> selectedReasons = data.selectedReasons;

				if (selectedReasons?.Count() > 0)
				{
					var selectedReasonsArray = selectedReasons.Select(r => (int)r).ToArray();
					var filteredSelectedReasons = _dbContext.DenialReasons.Where(r => selectedReasonsArray.Contains(r.Id));
					_grantApplication.GrantApplicationDenialReasons.Clear();
					filteredSelectedReasons.ForEach(r => _grantApplication.GrantApplicationDenialReasons.Add(r));
				}
				LogStateChanges(denialReason, denialReason);
			}

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been recommended for denial.");
		}

		/// <summary>
		/// File annotation contains director instructions.
		/// </summary>
		/// <param name="reason"></param>
		private void OnReturnedToAssessment(string reason = null)
		{
			if (_originalState == ApplicationStateInternal.ApplicationDenied)
			{
				if (!_grantOpeningService.CanMakeReservation(_grantApplication))
					throw new DbEntityValidationException("Insufficient funds to make the reservation requested");

				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
				_grantApplication.TrainingCost.CopyEstimatedIntoAgreed();
				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnToAssessment);
			}

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned to assessment.");
		}

		/// <summary>
		/// Confirmation: Are you sure you want to deny the application?
		/// Enter: Collect the reason for denying the application
		/// Notification:
		/// Your Canada-BC Job Grant application<fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate> and<TrainingPeriod.enddate>
		/// has been denied for the following reason:  <reason>.
		/// Financial: Release reservation.
		/// AM4 PRIVILEDGE - OVERRIDE
		/// Rule: Action buttons are only visible to user with AM4 privilege.
		/// Mark the file with an attribute recording the workflow override
		/// for application assessment.
		/// Record the override to the notes log as well: <Action> completed for application in state <PreviousState> by <username> on <date>.
		/// </summary>
		/// <param name="reason"></param>
		private void OnDenyApplication(string reason)
		{
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.DenyApplication);

			RemoveEligibleCostAddedByAssessor(_grantApplication);

			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ApplicationDenied;

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been denied.");
		}

		/// <summary>
		/// Confirmation: Are you sure you want to withdraw the offer?
		/// Other: Set external state to “Agreement Withdrawn”.
		/// Collect: Reason text or dropdown list:
		/// You did not agree to the grant agreement before the deadline.
		/// Other ??
		/// Record reason to application file.
		/// Notification:
		/// The offer to you for your Canada-BC Job Grant application<fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and<TrainingPeriod.enddate> has been withdrawn because<reason>.Your application file status shows Agreement Withdrawn and the file has been closed.
		/// See your file for more information.
		/// Financial: Release reservation.
		/// Business Description:
		/// If an application completes assessment and an offer in the form of an agreement is sent to the application administrator (AA) for acceptance and
		/// they fail to accept the application by the deadline, then this is a situation where the life cycle of the application needs to be retained.
		/// The application file will be locked for the application administrator until they accept the agreement and if not, it becomes an application file without a
		///	completed agreement.At any time past the deadline for agreement acceptance, the Ministry may withdraw the offer and the agreement would appear as withdrawn to the AA.
		///	In this case, the application file remains locked in this state and can be moved to the archive list by the AA.  As for any application,
		///	it may be copied to create a new application
		///	Five business days are permitted for the application administrator to accept the agreement or the participant reporting deadline whichever is sooner.
		/// </summary>
		/// <param name="reason"></param>
		private void OnWithdrawOffer(string reason)
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.AgreementWithdrawn;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.WithdrawOffer);

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} had it's offer withdrawn.");
		}

		private void OnReturnWithdrawnOfferToAssessment()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;

			_grantAgreementService.RemoveGrantAgreement(_grantApplication);
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} with withdrawn offer heen returned to Under Assessment.");
		}

		/// <summary>
		/// Confirmation: Are you sure you want to cancel this agreement; you will not be able to undo this?
		/// Change external state to “Cancelled by Ministry”
		/// Collect: Reason for Cancellation
		/// Record reason to application file
		/// Financial: Subtract commitment and record cancellation:
		/// GrantOpeningID.CurrentCommitments -= ApplicationID.AssessedCommitment
		/// GrantOpeningID.Cancellations += ApplicationID.AssessedCommitment
		/// Notification:
		/// Your Canada-BC Job Grant Agreement <fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and<TrainingPeriod.enddate> has been cancelled by the Ministry because <reason>.
		/// </summary>
		/// <param name="reason"></param>
		private void OnCancelAgreementMinistry(string reason)
		{
			try
			{
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.CancelledByMinistry;
				_grantApplication.DateCancelled = AppDateTime.UtcNow;
				_grantApplication.DisableParticipantReporting();
				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.CancelAgreementMinistry);

				LogStateChanges(reason, reason);

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} has been cancelled by the ministry.");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		private void OnReverseCancelAgreementMinistry()
		{
			// Figure out which of the two cases we can reverse from came last, and try to reverse that step.
			var previousState = _grantApplication.StateChanges
				.Where(s => s.FromState == ApplicationStateInternal.ClaimReturnedToApplicant || s.FromState == ApplicationStateInternal.AgreementAccepted)
				.OrderByDescending(s => s.Id)  // Order by Id to account for odd date ordering based on fake 'current date'
				.FirstOrDefault();

			var logMessage = $"Grant application {_grantApplication.Id} has been restored to Under Assessment (previously cancelled by the ministry).";

			if (previousState != null)
			{
				if (previousState.FromState == ApplicationStateInternal.ClaimReturnedToApplicant)
				{
					_grantApplication.DateCancelled = null;

					_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimReturned;
					_grantApplication.ApplicationStateInternal = ApplicationStateInternal.ClaimReturnedToApplicant;
					logMessage = $"Grant application {_grantApplication.Id} has been restored to Claim Returned To Applicant (previously cancelled by the ministry).";
				}

				if (previousState.FromState == ApplicationStateInternal.AgreementAccepted)
				{
					_grantApplication.DateCancelled = null;

					// Since we're going back to Under Assessment, we have to remove the previous Grant Agreement
					_grantAgreementService.RemoveGrantAgreement(_grantApplication);

					_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
					_grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;

					_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry);
					_grantApplication.EnableParticipantReporting();
				}
			}

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info(logMessage);
		}

		/// <summary>
		/// Confirmation: If you cancel your agreement you will not receive a grant to cover costs of training.Are you sure you want to cancel this agreement?
		/// Application Administrator provides reason.
		/// Financial: Subtract commitment as follows:
		/// GrantOpening.CurrentCommitments -= AgreementID.AssessedCommitment
		/// GrantOpening.Cancellations += AgreementID.AssessedCommitment
		/// Change external state to “Cancelled by Agreement Holder”.
		/// </summary>
		/// <param name="reason"></param>
		private void OnCancelAgreementHolder(string reason)
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.CancelledByAgreementHolder;
			_grantApplication.DateCancelled = AppDateTime.UtcNow;
			_grantApplication.DisableParticipantReporting();
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.CancelAgreementHolder);

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been cancelled by the agreement holder.");
		}

		/// <summary>
		/// Notification:
		/// To all application administrators for the application set.
		/// Your Canada-BC Job Grant application <fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and<TrainingPeriod.enddate> has not been assessed. All grant funds have been committed for this grant opening. Please reapply for the next opening.
		/// Change application external state for all in the application set to: “Not Accepted”; Reason: Grant Opening funds have been committed.
		/// </summary>
		private void OnReturnUnfundedApplications()
		{
			// Remove all line items added by Assessor.
			RemoveEligibleCostAddedByAssessor(_grantApplication);

			_grantApplication.FileNumber = null;

			foreach (var trainingProgram in _grantApplication.TrainingPrograms)
			{
				trainingProgram.TrainingProgramState = TrainingProgramStates.Incomplete;

				var trainingProvider = trainingProgram?.TrainingProvider;
				if (trainingProvider != null)
				{
					trainingProvider.TrainingProviderState = TrainingProviderStates.Incomplete;
					trainingProvider.TrainingProviderInventory = null;
					trainingProvider.TrainingProviderInventoryId = null;
				}
			}

			foreach (var trainingProvider in _grantApplication.TrainingProviders)
			{
				trainingProvider.TrainingProviderState = TrainingProviderStates.Incomplete;
				trainingProvider.TrainingProviderInventory = null;
				trainingProvider.TrainingProviderInventoryId = null;
			}

			_grantApplication.ProgramDescription.DescriptionState = ProgramDescriptionStates.Incomplete;
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.NotAccepted;
			_grantApplication.TrainingCost.TrainingCostState = (int)TrainingCostStates.Incomplete;
			_grantApplication.DisableParticipantReporting();
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnUnfundedApplications);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned as unfunded.");
		}

		private void OnReturnApplicationUnassessed()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ReturnedUnassessed;
			_grantApplication.DisableParticipantReporting();
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnUnassessed);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned as Returned to Applicant Unassessed.");
		}

		private void OnReturnUnassessedToNew()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
			_grantApplication.EnableParticipantReporting();
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnUnassessedToNew);

			LogStateChanges();

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} has been returned from Unassessed to New.");
		}

		/// <summary>
		/// Financial: Release reservation
		/// Add commitment:
		/// GrantOpeningID.CurrentCommitments += ApplicationID.AssessedCommitment
		/// GrantOpeningID.AssessedCommitments += ApplicationID.AssessedCommitment
		/// </summary>
		private void OnAcceptGrantAgreement()
		{
			try
			{
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Approved;
				_grantApplication.GrantAgreement.DateAccepted = AppDateTime.UtcNow;
				if (!_grantApplication.CanReportParticipants)
					_grantApplication.EnableParticipantReporting();

				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.AcceptGrantAgreement);

				LogStateChanges();

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} agreement has been accepted.");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// The grant application is closed and will no longer be worked on.
		/// </summary>
		/// <param name="reason"></param>
		private void OnCloseGrantFile(string reason)
		{
			try
			{
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Closed;
				_grantApplication.DisableParticipantReporting();

				LogStateChanges();

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} is closed.");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// Confirmation: If you reject the grant agreement your application will be withdrawn and you will not receive a grant.
		/// Are you sure you want to reject your grant agreement?
		/// Financial: Release reservation.
		/// Change external state to “Agreement Rejected”.
		/// Application Administrator provides reason.
		/// </summary>
		/// <param name="reason"></param>
		private void OnRejectGrantAgreement(string reason)
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.AgreementRejected;
			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.RejectGrantAgreement);

			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} agreement has been rejected.");
		}

		/// <summary>
		/// The action button only appears after a validate request has been added to the agreement file by the Application Administrator.
		/// See UI Design for requesting a change to a Training Provider. Submitting the Change Request is the final and necessary step or the change request is cancelled.
		/// </summary>
		/// <param name="reason"></param>
		private void OnSubmitChangeRequest(string reason)
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ChangeRequestSubmitted;

			LogStateChanges(reason, reason, addReason: true);

			var changeRequests = _grantApplication.GetChangeRequests();
			var requested = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Requested);

			var changes = new List<string>();
			foreach (var request in requested)
				changes.Add(string.Join(Environment.NewLine, request.GetChangeRequestDifferences()));

			if (changes.Any())
			{
				changes.Insert(0, "Change request details:");
				_noteService.AddSystemNote(_grantApplication, string.Join(Environment.NewLine, changes));
			}

			UpdateGrantApplication();
			
			_logger.Info($"Grant application {_grantApplication.Id} has submitted a change request.");
		}

		/// <summary>
		/// Business Rules:
		/// Requested Training Provider is linked to training provider list ready for approval.
		/// Generate note to record state change action to the notes log
		/// </summary>
		/// <param name="reason"></param>
		private void OnRecommendChangeForApprovalOrDenial(string reason = null)
		{
			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			var recommendation = _grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeForApproval ? "approval" : "denial";
			_logger.Info($"Grant application {_grantApplication.Id} change request has been recommended for {recommendation}.");
		}

		/// <summary>
		/// Generate change note to record change approval:
		/// 		<username> approved a change request for training provider from: <TrainingProviderName> to<RequestedTrainingProviderName> on<date/time>.
		/// Change Training Provider
		/// Set TrainingProvider = RequestedTrainingProvider
		/// Set TrainingProviderLink to RequestedTrainingProviderLink
		/// Set RequestedTrainingProvider = Null
		/// Set RequestedTrainingProviderLink = Null
		/// Set ApplicationStateExternal = “Approved”.
		/// Notification:
		/// Your change request for your Canada-BC Job Grant agreement number<fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and <TrainingPeriod.enddate> has been accepted. Please login to the system at: <url> and review the change to your grant agreement.
		/// </summary>
		private void OnApproveChangeRequest()
		{
			try
			{
				if (!_grantApplication.CanApproveChangeRequest())
					throw new InvalidOperationException("At least one requested training provider must be in approval.");

				var changeRequests = _grantApplication.GetChangeRequests();
				changeRequests.Where(tp => tp.TrainingProviderState.In(TrainingProviderStates.Requested, TrainingProviderStates.RequestApproved)).ForEach(request => request.TrainingProviderState = TrainingProviderStates.Complete);
				changeRequests.Where(tp => tp.TrainingProviderState.In(TrainingProviderStates.RequestDenied)).ForEach(request => request.TrainingProviderState = TrainingProviderStates.Denied);
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ChangeRequestApproved;
				_grantAgreementService.GenerateDocuments(_grantApplication);

				LogStateChanges();

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} change request has been approved");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// Generate note to record state change action to the notes log.
		/// </summary>
		/// <param name="reason"></param>
		private void OnReturnChangeToAssessment(string reason = null)
		{
			LogStateChanges(reason, reason);

			UpdateGrantApplication();

			_logger.Info($"Grant application {_grantApplication.Id} change request has been returned to assessment");
		}

		/// <summary>
		/// Generate change note to record change denial:
		/// 			<username> denied a change request for training provider from: <TrainingProviderName> to<RequestedTrainingProviderName> on<date/time>.
		/// Change Training Provider
		/// Set RequestedTrainingProvider = Null
		/// Set RequestedTrainingProviderLink = Null
		/// Set ApplicationStateExternal = “Change Request Denied”.
		/// Notification:
		/// Your change request for your Canada-BC Job Grant agreement number<fileno> for <Streams.Name> with training starting between <TrainingPeriod.startdate>
		/// and<TrainingPeriod.enddate> has been denied.  Please login to the system at: <url> and review the explanation for this assessment.
		/// </summary>
		/// <param name="reason"></param>
		private void OnDenyChangeRequest(string reason)
		{
			try
			{
				if (!_grantApplication.CanDenyChangeRequest())
					throw new InvalidOperationException($"All requested training providers must be for denial before denying the change request.");

				var changeRequests = _grantApplication.GetChangeRequests();
				changeRequests.ForEach(request => request.TrainingProviderState = TrainingProviderStates.Denied);
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ChangeRequestDenied;

				LogStateChanges(reason, reason);

				UpdateGrantApplication();

				_logger.Info($"Grant application {_grantApplication.Id} change request has been denied");
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// Submit the specified claim so that it can be assessed.
		/// This will adjust the financial statements and disable participant reporting.
		/// </summary>
		/// <param name="claim"></param>
		private void OnSubmitClaim(Claim claim)
		{
			try
			{
				claim.DateSubmitted = AppDateTime.UtcNow;
				claim.ClaimState = ClaimState.Unassessed;

				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimSubmitted;
				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.SubmitClaim);

				// Close participant reporting on claim submission
				_grantApplication.DisableApplicantParticipantReporting();
				_grantApplication.DisableParticipantReporting();

				var validationResults = _grantOpeningService.Validate(claim);
				if (validationResults.Any())
				{
					throw new DbEntityValidationException(validationResults.GetErrorMessages(Environment.NewLine));
				}

				// Associated participants with claim.
				_grantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim).ForEach(pf =>
				{
					claim.ParticipantForms.Add(pf);
					pf.ClaimReported = true;
				});

				LogStateChanges($"Applicant submitted claim number {claim.ClaimNumber}", addReason: false);

				UpdateGrantApplication();
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		/// <summary>
		/// Reverse the assumed success of the claim by restoring the assessed commitment value for the grant file(expecting a potential successful claim following the returned claim).  This would adjust the claim management dashboard to record this grant file as it was before a claim was submitted.
		/// Reduce the current claims by the amount claimed:
		/// 	•	G.CurrentClaims - = Claims.TotalClaimReimbursement
		/// 	•	G.CurrentClaimsCount -= 1
		/// Increase the Outstanding Commitments by the original approved agreement maximum amount.
		/// 	•	G.OutstandingCommitments + = TrainingPrograms.AgreedCommitment
		/// 	•	G.OutstandingCommitmentsCount += 1
		/// There are no other tallies for management and slippage reporting when a claim is returned to an applicant.
		/// Notes Log Entry:
		/// Changed to “Claim Returned to Applicant” from “<Previous>”\n
		/// Claim<ClaimVersion> withdrawn by applicant\n
		/// Reason: <ApplicantWithdrawReason>
		/// Restore the grant file for participant and claim reporting:
		/// 	•	Generate a new participant invitation link and enable it for participant reporting.
		/// 	•	State change enables application administrator access to functions for participant and claim reporting.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="reason"></param>
		/// <param name="service"></param>
		private void OnWithdrawClaim(Claim claim, string reason, IClaimService service)
		{
			claim.ClaimState = ClaimState.Incomplete;
			service.RemoveEligibleCostsAddedByAssessor(claim);

			_grantApplication.EnableParticipantReporting();

			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimReturned;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.WithdrawClaim);

			LogStateChanges(reason, reason, suffix: $"\nClaim {claim.ClaimNumber} withdrawn by applicant");

			// Remove the participants from the claim.
			RemoveParticipantsFromClaim(claim);

			UpdateGrantApplication();
		}

		private void OnSelectClaimForAssessment(Claim claim)
		{
			// Need to copy all of the ClaimEligibleCosts.ClaimValues into the ClaimEligibleCosts.AssessedValues
			claim.CopyClaimValuesToAssessed();

			LogStateChanges(suffix: $" for claim {claim.ClaimNumber}");
			UpdateGrantApplication();
		}

		private void OnRemoveClaimFromAssessment(Claim claim)
		{
			LogStateChanges(suffix: $" for claim {claim.ClaimNumber}");
			UpdateGrantApplication();
		}

		private void OnAssessClaimReimbursement(Claim claim)
		{
			_grantApplication.RemoveAssignedAssessor();

			LogStateChanges(suffix: $" for claim {claim.ClaimNumber}");
			UpdateGrantApplication();
		}

		private void OnAssessClaimEligibility(Claim claim)
		{
			if (_originalState == ApplicationStateInternal.ClaimAssessReimbursement)
				_grantApplication.RemoveAssignedAssessor();

			LogStateChanges(suffix: $" for claim {claim.ClaimNumber}");
			UpdateGrantApplication();
		}

		private void OnApproveClaim(Claim claim)
		{
			var priorApprovedClaimVersions = _dbContext.Claims
				.Where(c => c.Id == claim.Id && c.ClaimState == ClaimState.ClaimApproved).ToList();

			if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				foreach (var version in priorApprovedClaimVersions)
				{
					version.ClaimState = ClaimState.ClaimAmended;
					_dbContext.Entry(version).State = EntityState.Modified;
				}
			}

			if (claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
				_grantApplication.EnableParticipantReporting();

			claim.DateAssessed = AppDateTime.UtcNow;
			claim.ClaimState = ClaimState.ClaimApproved;
			claim.Assessor = _userService.GetInternalUser(_httpContext.User.GetUserId().Value);
			claim.AssessorId = _httpContext.User.GetUserId();
			claim.LockParticipants();

			claim.IsFinalClaim = true;

			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimApproved;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ApproveClaim);

			LogStateChanges(stateChangeReason: claim.ClaimAssessmentNotes, suffix: $" for claim {claim.ClaimNumber}");

			UpdateGrantApplication();

			//OnCloseClaimReporting();
		}

		private void OnDenyClaim(Claim claim)
		{
			claim.DateAssessed = AppDateTime.UtcNow;
			claim.ResetAssessment();
			claim.Assessor = _userService.GetInternalUser(_httpContext.User.GetUserId().Value);
			claim.AssessorId = _httpContext.User.GetUserId();
			claim.ClaimState = ClaimState.ClaimDenied;
			claim.LockParticipants();

			_grantApplication.EnableParticipantReporting();
			_grantApplication.RemoveAssignedAssessor();
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimDenied;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.DenyClaim);

			LogStateChanges(stateChangeReason: claim.ClaimAssessmentNotes, suffix: $" for claim {claim.ClaimNumber}");

			UpdateGrantApplication();
		}

		private void OnReturnClaimToApplicant(Claim claim, string reason, IClaimService service)
		{
			service.RemoveEligibleCostsAddedByAssessor(claim);
			claim.ResetAssessment();
			claim.DateSubmitted = null;
			var numberOfParticipants = claim.GrantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
			foreach (var claimEligibleCost in claim.EligibleCosts)
			{
				if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited
					&& claim.GrantApplication.GetProgramType() == ProgramTypes.WDAService)
				{
					if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
					{
						claimEligibleCost.UpdateUpToMaxClaimParticipants(numberOfParticipants);
					}
				}
			}
			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();
			claim.ClaimState = ClaimState.Incomplete;

			_grantApplication.EnableParticipantReporting();

			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimReturned;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReturnClaimToApplicant);

			var noteLogReason = $"Explanation to Applicant: {claim.ClaimAssessmentNotes}\n" +
								$"Eligibility Notes: {claim.EligibilityAssessmentNotes}\n" +
								$"Reimbursement Notes: {claim.ReimbursementAssessmentNotes}";

			LogStateChanges(noteLogReason, claim.ClaimAssessmentNotes, $" for claim {claim.ClaimNumber}", addReason: false);

			// Remove participants from the claim.
			RemoveParticipantsFromClaim(claim);

			UpdateGrantApplication();
		}

		private void OnReturnClaimToNew(Claim claim, IClaimService service)
		{
			try
			{
				claim.DateSubmitted = AppDateTime.UtcNow;
				claim.ClaimState = ClaimState.Unassessed;

				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimSubmitted;
				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReverseClaimReturnedToApplicant);

				// Close participant reporting on claim submission
				_grantApplication.DisableApplicantParticipantReporting();
				_grantApplication.DisableParticipantReporting();

				var validationResults = _grantOpeningService.Validate(claim).ToList();
				if (validationResults.Any())
					throw new DbEntityValidationException(validationResults.GetErrorMessages(Environment.NewLine));

				// Associated participants with claim.
				_grantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim).ForEach(pf =>
				{
					claim.ParticipantForms.Add(pf);
					pf.ClaimReported = true;
				});

				LogStateChanges($"Applicant submitted claim number {claim.ClaimNumber}", stateChangeReason: "Reversed 'Claim Returned to Applicant'", addReason: false);

				UpdateGrantApplication();
			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		private void OnReverseClaimDenied(Claim claim, IClaimService service)
		{
			try
			{
				claim.DateAssessed = AppDateTime.UtcNow;

				claim.CopyClaimValuesToAssessed();
				claim.ClaimState = ClaimState.Unassessed;
				claim.UnlockParticipants();

				_grantApplication.DisableParticipantReporting();
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimSubmitted;

				_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.ReverseClaimDenied);

				LogStateChanges($"Claim number {claim.ClaimNumber} Denial reversed", stateChangeReason: "Reversed 'Claim Denied'", addReason: false);

				UpdateGrantApplication();

			}
			catch (NotificationException e)
			{
				_logger.Error(e, "Notification failed for grant application Id: {0}", _grantApplication?.Id);
			}
		}

		private void OnInitiateClaimAmendment(Claim claim)
		{
			try
			{
				_grantApplication.EnableParticipantReporting();

				if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					_grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

				LogStateChanges(stateChangeReason: claim.ClaimAssessmentNotes, suffix: $"\nWith claim {claim.ClaimNumber}\n");

				UpdateGrantApplication();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't initiate claim amendment for claim Id: ", claim.Id);
				throw;
			}
		}

		private void OnCloseClaimReporting()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ReportCompletion;
			_grantApplication.DisableParticipantReporting();

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.CloseClaimReporting);

			LogStateChanges();
			UpdateGrantApplication();
		}

		private void OnEnableClaimReporting()
		{
			_grantApplication.EnableParticipantReporting();

			if (_grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

			if (_grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
				_grantApplication.ApplicationStateExternal = ApplicationStateExternal.Approved;

			_grantOpeningService.AdjustFinancialStatements(_grantApplication, _originalState, ApplicationWorkflowTrigger.EnableClaimReporting);

			LogStateChanges();
			UpdateGrantApplication();
		}

		private void OnEnableCompletionReporting()
		{
			_grantApplication.ApplicationStateExternal = ApplicationStateExternal.ReportCompletion;
			_grantApplication.DisableParticipantReporting();

			LogStateChanges();
			UpdateGrantApplication();
		}
		#endregion

		#region Trigger events
		public void SubmitApplication()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SubmitApplication);
			_stateMachine.Fire(ApplicationWorkflowTrigger.SubmitApplication);
		}

		public void SelectForAssessment()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SelectForAssessment);
			_stateMachine.Fire(ApplicationWorkflowTrigger.SelectForAssessment);
		}

		public void ReturnUnfundedApplications()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnUnfundedApplications);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnUnfundedApplications);
		}

		public void ReturnApplicationUnassessed()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnUnassessed);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnUnassessed);
		}

		public void ReturnApplicationUnassessedToNew()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnUnassessedToNew);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnUnassessedToNew);
		}

		public void RemoveFromAssessment()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RemoveFromAssessment);
			_stateMachine.Fire(ApplicationWorkflowTrigger.RemoveFromAssessment);
		}

		public void BeginAssessment(InternalUser internalUser)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.BeginAssessment);
			_stateMachine.Fire(_beginAssessmentTrigger, internalUser);
		}

		public void RecommendForApproval(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RecommendForApproval);
			_stateMachine.Fire(_recommendForApprovalTrigger, reason);
		}

		public void RecommendForDenial(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RecommendForDenial);
			_stateMachine.Fire(_recommendForDenialTrigger, reason);
		}

		public void IssueOffer()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.IssueOffer);
			_stateMachine.Fire(ApplicationWorkflowTrigger.IssueOffer);
		}

		public void ReturnToAssessment(string reason = null)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnToAssessment);
			_stateMachine.Fire(_returnToAssessmentTrigger, reason);
		}

		public void ReturnUnderAssessmentToDraft()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft);
		}

		public void ReturnOfferToAssessment()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnOfferToAssessment);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnOfferToAssessment);
		}

		public void DenyApplication(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.DenyApplication);
			_stateMachine.Fire(_denyApplicationTrigger, reason);
		}

		public void WithdrawOffer(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.WithdrawOffer);
			_stateMachine.Fire(_withdrawOfferTrigger, reason);
		}

		public void ReturnWithdrawnOfferToAssessment()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment);
		}

		public void AcceptGrantAgreement()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.AcceptGrantAgreement);
			_stateMachine.Fire(ApplicationWorkflowTrigger.AcceptGrantAgreement);
		}

		public void RejectGrantAgreement(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RejectGrantAgreement);
			_stateMachine.Fire(_rejectGrantAgreementTrigger, reason);
		}

		public void CancelAgreementMinistry(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.CancelAgreementMinistry);
			_stateMachine.Fire(_cancelAgreementMinistryTrigger, reason);
		}

		public void ReverseCancelledAgreementMinistry()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry);
		}

		public void CloseGrantFile(string reason = null)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.Close);
			_stateMachine.Fire(_closeGrantFileTrigger, reason);
		}

		public void SubmitCompletionReportToCloseGrantFile()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SubmitCompletionReport);
			_stateMachine.Fire(ApplicationWorkflowTrigger.SubmitCompletionReport);
		}

		public void CancelAgreementHolder(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.CancelAgreementHolder);
			_stateMachine.Fire(_cancelAgreementHolderTrigger, reason);
		}

		public void WithdrawApplication(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.WithdrawApplication);
			_stateMachine.Fire(_withdrawApplicationTrigger, reason);
		}

		public void SubmitChangeRequest(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SubmitChangeRequest);
			_stateMachine.Fire(_submitChangeRequestTrigger, reason);
		}

		public void RecommendChangeForApproval()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RecommendChangeForApproval);
			_stateMachine.Fire(ApplicationWorkflowTrigger.RecommendChangeForApproval);
		}

		public void RecommendChangeForDenial(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RecommendChangeForDenial);
			_stateMachine.Fire(_recommendChangeForDenialTrigger, reason);
		}

		public void ApproveChangeRequest()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ApproveChangeRequest);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ApproveChangeRequest);
		}

		public void SubmitClaim(Claim claim)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SubmitClaim);
			_stateMachine.Fire(_submitClaimTrigger, claim);
		}

		public void WithdrawClaim(Claim claim, string reason, IClaimService service)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.WithdrawClaim);
			_stateMachine.Fire(_withdrawClaimTrigger, claim, reason, service);
		}

		public void SelectClaimForAssessment(Claim claim)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.SelectClaimForAssessment);
			_stateMachine.Fire(_selectClaimForAssessmentTrigger, claim);
		}

		public void RemoveClaimFromAssessment(Claim claim)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.RemoveClaimFromAssessment);
			_stateMachine.Fire(_removeClaimFromAssessmentTrigger, claim);
		}

		public void AssessClaimReimbursement(Claim claim)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.AssessReimbursement);
			_stateMachine.Fire(_assessClaimReimbursementTrigger, claim);
		}

		public void AssessClaimEligibility(Claim claim)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.AssessEligibility);
			_stateMachine.Fire(_assessClaimEligibilityTrigger, claim);
		}

		public void ApproveClaim(Claim claim)
		{
			if (string.IsNullOrWhiteSpace(claim.ClaimAssessmentNotes))
				throw new InvalidOperationException("You must provide an explanation to applicant before approving the claim.");

			CanPerformTrigger(ApplicationWorkflowTrigger.ApproveClaim);
			_stateMachine.Fire(_approveClaimTrigger, claim);
		}

		public void DenyClaim(Claim claim)
		{
			if (string.IsNullOrWhiteSpace(claim.ClaimAssessmentNotes))
				throw new InvalidOperationException("You must provide an explanation to applicant before denying the claim.");

			CanPerformTrigger(ApplicationWorkflowTrigger.DenyClaim);
			_stateMachine.Fire(_denyClaimTrigger, claim);
		}

		public void AmendClaim(Claim claim, Func<GrantApplication, Claim> action)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.AmendClaim);
			action(claim.GrantApplication);
			_stateMachine.Fire(_initiateClaimAmendmentTrigger, claim);
		}

		public void ReturnClaimToApplicant(Claim claim, string reason, IClaimService service)
		{
			if (string.IsNullOrWhiteSpace(reason))
				throw new InvalidOperationException("You must provide a reason to return the claim the the applicant.");

			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnClaimToApplicant);
			_stateMachine.Fire(_returnClaimToApplicantTrigger, claim, reason, service);
		}

		public void ReturnClaimToNew(Claim claim, IClaimService service)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReverseClaimReturnedToApplicant);
			_stateMachine.Fire(_returnClaimToNewTrigger, claim, service);
		}

		public void ReverseClaimDenied(Claim claim, IClaimService service)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReverseClaimDenied);
			_stateMachine.Fire(_reverseClaimDeniedTrigger, claim, service);
		}

		public void ReturnChangeToAssessment(string reason = null)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnChangeToAssessment);
			_stateMachine.Fire(_returnChangeToAssessmentTrigger, reason);
		}

		public void DenyChangeRequest(string reason)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.DenyChangeRequest);
			_stateMachine.Fire(_denyChangeRequestTrigger, reason);
		}

		public void CloseClaimReporting(Action<GrantApplication> action)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.CloseClaimReporting);
			action?.Invoke(_grantApplication);
			_stateMachine.Fire(ApplicationWorkflowTrigger.CloseClaimReporting);
		}

		public void EnableClaimReporting(Func<GrantApplication, Claim> action)
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.EnableClaimReporting);
			action?.Invoke(_grantApplication);
			_stateMachine.Fire(ApplicationWorkflowTrigger.EnableClaimReporting);
		}

		public void EnableCompletionReporting()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.EnableCompletionReporting);
			_stateMachine.Fire(ApplicationWorkflowTrigger.EnableCompletionReporting);
		}

		#endregion

		#region Helpers
		public IEnumerable<ApplicationWorkflowTrigger> GetPermittedTriggers()
		{
			return _stateMachine.PermittedTriggers;
		}

		/// <summary>
		/// If the currently logged in user is internal get them.
		/// </summary>
		/// <returns></returns>
		private InternalUser GetInternalUser()
		{
			return _httpContext.User.GetAccountType() == AccountTypes.Internal ? _userService.GetInternalUser(_httpContext.User.GetUserId().Value) : null;
		}

		/// <summary>
		/// If the currently logged in user is external get them.
		/// </summary>
		/// <returns></returns>
		private User GetUser()
		{
			return _httpContext.User.GetAccountType() == AccountTypes.External ? _userService.GetUser(_httpContext.User.GetUserId().Value) : null;
		}

		/// <summary>
		/// Add a note for each grant application being selected for assessment.
		/// </summary>
		/// <param name="reservedApplicationIds"></param>
		private void LogStateChangesForReservedApplications(List<int> reservedApplicationIds)
		{
			try
			{
				var ids = reservedApplicationIds.ToArray();
				var reservedGrantApplications = _dbContext.GrantApplications.Where(ga => ids.Contains(ga.Id));
				foreach (var reservedGrantApplication in reservedGrantApplications)
				{
					RecordStateChange(reservedGrantApplication, ApplicationStateInternal.New, GetInternalUser(), GetUser(), string.Empty);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}

			LogStateChanges();
		}

		/// <summary>
		/// Add a note to the application containing the change that occured.
		/// </summary>
		/// <param name="noteReason"></param>
		/// <param name="stateChangeReason"></param>
		/// <param name="suffix"></param>
		/// <param name="addReason"></param>
		private void LogStateChanges(string noteReason = "", string stateChangeReason = "", string suffix = null, bool addReason = true)
		{
			var noteContent = $"Changed from \"{_originalState.GetDescription()}\" to \"{_grantApplication.ApplicationStateInternal.GetDescription()}\"{suffix}";

			switch (_grantApplication.ApplicationStateInternal)
			{
				case ApplicationStateInternal.New:
					if (_originalState == ApplicationStateInternal.ReturnedUnassessed)
					{
						noteContent = $"Unassessed application returned to New and assigned file number: {_grantApplication.FileNumber}";
						break;
					}

					if (_originalState != ApplicationStateInternal.PendingAssessment)
					{
						noteContent = $"New application submitted and assigned file number: {_grantApplication.FileNumber}";
					}
					break;

				case ApplicationStateInternal.ApplicationWithdrawn:
					noteContent = $"{_grantApplication.FileNumber} {noteContent}";
					break;

				default:
					break;
			}

			if (!string.IsNullOrEmpty(noteReason))
			{
				if (addReason)
					noteReason = $"Reason: {noteReason}";

				noteContent += $"{Environment.NewLine}{noteReason}";
			}

			_noteService.AddWorkflowNote(_grantApplication, noteContent);

			RecordStateChange(_grantApplication, _originalState, GetInternalUser(), GetUser(), stateChangeReason);
		}

		/// <summary>
		/// Add a state change record to the datasrouce.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="previousState"></param>
		/// <param name="internalUser"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		private void RecordStateChange(GrantApplication grantApplication, ApplicationStateInternal previousState, InternalUser internalUser, User user, string reason)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var stateChange = internalUser != null
				? new GrantApplicationStateChange(grantApplication, previousState, grantApplication.ApplicationStateInternal, internalUser, reason)
				: new GrantApplicationStateChange(grantApplication, previousState, grantApplication.ApplicationStateInternal, user, reason);

			_dbContext.GrantApplicationStateChanges.Add(stateChange);
		}

		/// <summary>
		/// Remove eligible costs that were added by the assessor.
		/// </summary>
		/// <param name="grantApplication"></param>
		private void RemoveEligibleCostAddedByAssessor(GrantApplication grantApplication)
		{
			foreach (var eligibleCost in grantApplication.TrainingCost.EligibleCosts.ToArray())
			{
				if (eligibleCost.AddedByAssessor)
				{
					foreach (var breakdown in eligibleCost.Breakdowns.ToArray())
					{
						if (breakdown.TrainingPrograms.Any())
						{
							foreach (var trainingProgram in breakdown.TrainingPrograms.ToArray())
							{
								foreach (var trainingProvider in trainingProgram.TrainingProviders.ToArray())
								{
									_dbContext.ApplicationAddresses.Remove(trainingProvider.TrainingAddress);
									if (trainingProvider.ProofOfQualificationsDocument != null) _dbContext.Attachments.Remove(trainingProvider.ProofOfQualificationsDocument);
									if (trainingProvider.CourseOutlineDocument != null) _dbContext.Attachments.Remove(trainingProvider.CourseOutlineDocument);
									if (trainingProvider.BusinessCaseDocument != null) _dbContext.Attachments.Remove(trainingProvider.BusinessCaseDocument);
									_dbContext.TrainingProviders.Remove(trainingProvider);
								}
								if (trainingProgram.CourseOutlineDocument != null)
									_dbContext.Attachments.Remove(trainingProgram.CourseOutlineDocument);
								trainingProgram.DeliveryMethods.Clear();
								trainingProgram.UnderRepresentedGroups.Clear();
								_dbContext.TrainingPrograms.Remove(trainingProgram);
							}
						}
						_dbContext.EligibleCostBreakdowns.Remove(breakdown);
					}
					_dbContext.EligibleCosts.Remove(eligibleCost);
				}
				else
				{
					foreach (var breakdown in eligibleCost.Breakdowns.Where(b => b.AddedByAssessor).ToArray())
					{
						if (breakdown.TrainingPrograms.Any())
						{
							foreach (var trainingProgram in breakdown.TrainingPrograms.ToArray())
							{
								foreach (var trainingProvider in trainingProgram.TrainingProviders.ToArray())
								{
									_dbContext.ApplicationAddresses.Remove(trainingProvider.TrainingAddress);
									if (trainingProvider.ProofOfQualificationsDocument != null) _dbContext.Attachments.Remove(trainingProvider.ProofOfQualificationsDocument);
									if (trainingProvider.CourseOutlineDocument != null) _dbContext.Attachments.Remove(trainingProvider.CourseOutlineDocument);
									if (trainingProvider.BusinessCaseDocument != null) _dbContext.Attachments.Remove(trainingProvider.BusinessCaseDocument);
									_dbContext.TrainingProviders.Remove(trainingProvider);
								}
								if (trainingProgram.CourseOutlineDocument != null)
									_dbContext.Attachments.Remove(trainingProgram.CourseOutlineDocument);
								trainingProgram.DeliveryMethods.Clear();
								trainingProgram.UnderRepresentedGroups.Clear();
								_dbContext.TrainingPrograms.Remove(trainingProgram);
							}
						}
						eligibleCost.Breakdowns.Remove(breakdown);
						_dbContext.EligibleCostBreakdowns.Remove(breakdown);
					}

					// Re-calculate the breakdowns for skills training components.
					if (eligibleCost.EligibleExpenseType?.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining)
					{
						eligibleCost.EstimatedCost = eligibleCost.CalculateEstimateCost();
					}
				}
			}

			grantApplication.TrainingCost.ResetEstimatedCosts();
		}

		/// <summary>
		/// Remove the participants from the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		private void RemoveParticipantsFromClaim(Claim claim)
		{
			claim.ParticipantForms.ForEach(pf =>
			{
				pf.ClaimReported = pf.Claims.Any(c => c.Id != claim.Id && c.ClaimVersion != claim.ClaimVersion);
			});
			claim.ParticipantForms.Clear();
		}

		/// <summary>
		/// Add a notification to the queue for every grant program notification type that is valid for this state transition.
		/// </summary>
		private void HandleWorkflowNotifications()
		{
			// Send notifications
			var notificationTypes = _grantApplication.GrantOpening.GrantStream.GrantProgram
				.GrantProgramNotificationTypes
				.Where(t => t.IsActive)
				.Where(t => t.NotificationType.IsActive)
				.Where(t => t.NotificationType.NotificationTriggerId == NotificationTriggerTypes.Workflow);

			foreach (var grantProgramNotificationType in notificationTypes)
			{
				if (_notificationService.CheckNotificationWorkflow(grantProgramNotificationType, _grantApplication, _originalState))
					_notificationService.HandleWorkflowNotification(_grantApplication, grantProgramNotificationType);
			}
		}
		#endregion
		#endregion
	}
}
