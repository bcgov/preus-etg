using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using Stateless;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ApplicationJobWorkflowStateMachine"/> class, provides valid workflow transitions and validation for the application.
	/// This is a cut down version of the ApplicationWorkFlowStateMachine the rest of the application uses.
	/// This was done to reduce the amount of dependencies that need to be resolved, and to bypass any user based checks
	/// </summary>
	public class ApplicationJobWorkflowStateMachine : Service
	{
		private readonly GrantApplication _grantApplication;
		private readonly ApplicationStateInternal _originalState;

		private readonly INotificationService _notificationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly INoteService _noteService;
		private readonly IUserService _userService;

		private readonly StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger> _stateMachine;

		public ApplicationJobWorkflowStateMachine(
			GrantApplication grantApplication,
			IDataContext dbContext,
			INotificationService notificationService,
			IGrantOpeningService grantOpeningService,
			INoteService noteService,
			IUserService userService,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			_grantApplication = grantApplication;
			_notificationService = notificationService;
			_grantOpeningService = grantOpeningService;
			_noteService = noteService;
			_userService = userService;

			// Instantiate state machine based on current Grant Application state
			_stateMachine = new StateMachine<ApplicationStateInternal, ApplicationWorkflowTrigger>(() => _grantApplication.ApplicationStateInternal,
																									s => _grantApplication.ApplicationStateInternal = s);
			_originalState = _stateMachine.State;

			ConfigureStateTransitions();
		}

		private void ConfigureStateTransitions()
		{
			_stateMachine.Configure(ApplicationStateInternal.New)
				.Permits(_grantApplication.ApplicationStateInternal);

			_stateMachine.Configure(ApplicationStateInternal.ReturnedUnassessed)
				.OnEntryFrom(ApplicationWorkflowTrigger.ReturnUnassessed, t => OnReturnApplicationUnassessed())
				.Permits(_grantApplication.ApplicationStateInternal);
		}

		private void UpdateGrantApplication(bool handleWorkFlowNotifications = true)
		{
			if (handleWorkFlowNotifications)
				HandleWorkflowNotifications();

			_dbContext.SetModified(_grantApplication);
			_dbContext.CommitTransaction();
		}

		private void CanPerformTrigger(ApplicationWorkflowTrigger trigger)
		{
			// We cannot check for user here since we are running as the system level user running the job

			var validTriggers = _grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers();
			if (validTriggers.Contains(trigger))
				return;

			throw new NotAuthorizedException($"The action '{trigger.GetDescription()}' is not valid for the current application state '{_grantApplication.ApplicationStateInternal.GetDescription()}'.");
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

		public void ReturnApplicationUnassessed()
		{
			CanPerformTrigger(ApplicationWorkflowTrigger.ReturnUnassessed);
			_stateMachine.Fire(ApplicationWorkflowTrigger.ReturnUnassessed);
		}

		public IEnumerable<ApplicationWorkflowTrigger> GetPermittedTriggers()
		{
			return _stateMachine.PermittedTriggers;
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
			}

			if (!string.IsNullOrEmpty(noteReason))
			{
				if (addReason)
					noteReason = $"Reason: {noteReason}";

				noteContent += $"{Environment.NewLine}{noteReason}";
			}

			_noteService.AddWorkflowNote(_grantApplication, noteContent);
			
			RecordStateChange(_grantApplication, _originalState, stateChangeReason);
		}

		/// <summary>
		/// Add a state change record to the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="previousState"></param>
		/// <param name="reason"></param>
		private void RecordStateChange(GrantApplication grantApplication, ApplicationStateInternal previousState, string reason)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var stateChange = new GrantApplicationStateChange(grantApplication, previousState, grantApplication.ApplicationStateInternal, reason);

			_dbContext.GrantApplicationStateChanges.Add(stateChange);
		}

		/// <summary>
		/// Add a notification to the queue for every grant program notification type that is valid for this state transition.
		/// </summary>
		private void HandleWorkflowNotifications()
		{
			// Send notifications
			var notificationTypes = _grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes
				.Where(t => t.IsActive)
				.Where(t => t.NotificationType.IsActive)
				.Where(t => t.NotificationType.NotificationTriggerId == NotificationTriggerTypes.Workflow);

			foreach (var grantProgramNotificationType in notificationTypes)
			{
				if (_notificationService.CheckNotificationWorkflow(grantProgramNotificationType, _grantApplication, _originalState))
					_notificationService.HandleWorkflowNotification(_grantApplication, grantProgramNotificationType);
			}
		}
	}
}
