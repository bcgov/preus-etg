using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.Entities;
using Microsoft.CSharp.RuntimeBinder;
using NLog;
using RazorEngine.Templating;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="NotificationService"/> class, provides a way to manage notifications (emails).
	/// </summary>
	public class NotificationService : Service, INotificationService
	{
		private readonly IEmailSenderService _emailSender;
		private readonly INotificationSettings _notificationSettings;
		private readonly ISettingService _settingService;
		private readonly ICompletionReportService _completionReportService;

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationService"/> class.
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="emailSender"></param>
		/// <param name="settings"></param>
		/// <param name="httpContext"></param>
		/// <param name="settingsService"></param>
		/// <param name="completionReportService"></param>
		/// <param name="logger"></param>
		public NotificationService(
			IEmailSenderService emailSender,
			HttpContextBase httpContext,
			INotificationSettings settings,
			IDataContext dbContext,
			ISettingService settingsService,
			ICompletionReportService completionReportService,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			_emailSender = emailSender;
			_notificationSettings = settings;
			_settingService = settingsService;
			_completionReportService = completionReportService;
		}

		/// <summary>
		/// Returns the grant application notification for the specified 'notificationQueueId' or throw NoContentException if not found.
		/// </summary>
		/// <param name="notificationQueueId"></param>
		/// <returns></returns>
		public NotificationQueue GetApplicationNotification(int notificationQueueId)
		{
			return Get<NotificationQueue>(notificationQueueId);
		}

		/// <summary>
		/// Returns the grant program notifications for the specified 'filter'.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public PageList<NotificationQueue> GetGrantProgramNotifications(int grantProgramId, int page, int quantity, NotificationFilter filter)
		{
			var query = _dbContext.NotificationQueue.Where(nt => nt.GrantApplication.GrantOpening.GrantStream.GrantProgramId == grantProgramId);
			return GetNotifications(page, quantity, filter, query);
		}

		/// <summary>
		/// Returns the grant application notifications for the specified 'filter'.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public PageList<NotificationQueue> GetGrantApplicationNotifications(int grantApplicationId, int page, int quantity, string search, NotificationFilter filter)
		{
			var query = _dbContext.NotificationQueue.Where(nt => nt.GrantApplicationId == grantApplicationId &&
														  (string.IsNullOrEmpty(search) || nt.NotificationType.Caption.Contains(search)) &
														  (filter.TriggerType == 0 || nt.NotificationType.NotificationTriggerId == (NotificationTriggerTypes)filter.TriggerType));

			return GetNotifications(page, quantity, filter, query);
		}

		/// <summary>
		/// Returns the grant application notifications for the specified 'filter'.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public PageList<NotificationQueue> GetNotifications(int page, int quantity, NotificationFilter filter)
		{
			if (page <= 0)
				page = 1;

			if (quantity <= 0 || quantity > 100)
				quantity = 10;

			var query = _dbContext.NotificationQueue.Where(nt => true);

			// Filter
			if (!string.IsNullOrWhiteSpace(filter.NotificationType))
				query = query.Where(n => n.NotificationType.Caption == filter.NotificationType);

			if (!string.IsNullOrWhiteSpace(filter.Organization))
				query = query.Where(n => n.Organization.LegalName == filter.Organization);

			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				var aStatus = filter.Status.Split(',').Select(s => s.Trim()).ToArray();
				if (aStatus.Length > 1)
				{
					query = query.Where(n => aStatus.Contains(n.State.ToString()));
				}
				else
				{
					query = query.Where(n => n.State.ToString() == filter.Status);
				}
			}

			if (!string.IsNullOrWhiteSpace(filter.Search))
				query = query.Where(n => n.EmailSubject.Contains(filter.Search)
				                         || n.NotificationType.Caption.Contains(filter.Search)
				                         || n.Organization.LegalName.Contains(filter.Search));

			var total = query.Count();

			var orderBy = !string.IsNullOrWhiteSpace(filter.OrderBy)
				? filter.OrderBy
				: $"{nameof(NotificationQueue.SendDate)} desc";

			query = query.OrderByProperty(orderBy)
				.Skip((page - 1) * quantity)
				.Take(quantity);

			return new PageList<NotificationQueue>(page, quantity, total, query.ToArray()); ;
		}

		/// <summary>
		/// Returns the grant program notification organizations.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Organization> GetGrantProgramNotificationOrganizations(int grantProgramId)
		{
			var query = _dbContext.NotificationQueue.Where(nt => nt.GrantApplication.GrantOpening.GrantStream.GrantProgramId == grantProgramId);
			return query.Select(o => o.Organization)
				.Distinct()
				.ToArray();
		}

		/// <summary>
		/// Returns the grant program notification types.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<NotificationType> GetGrantProgramNotificationNotificationTypes(int grantProgramId)
		{
			var query = _dbContext.NotificationQueue.Where(nt => nt.GrantApplication.GrantOpening.GrantStream.GrantProgramId == grantProgramId);
			return query.Select(o => o.NotificationType)
				.Distinct()
				.ToArray();
		}

		public NotificationType GetPIFInvitationNotificationType()
		{
			var pifInvitationType = _dbContext.NotificationTypes
				.Where(nt => nt.NotificationTriggerId == NotificationTriggerTypes.EmailTemplate)
				.Where(nt => nt.Caption == "PIF Invitation")
				.FirstOrDefault();

			return pifInvitationType;
		}

		/// <summary>
		/// Attempts to send an email for the specified notification.
		/// This function does not update the datasource, you will need to call CommitTransaction().
		/// </summary>
		/// <param name="notification"></param>
		/// <param name="test"></param>
		/// <returns>Whether it was successfully sent.</returns>
		public bool SendNotification(NotificationQueue notification, bool test = false)
		{
			if (notification == null) throw new ArgumentNullException(nameof(notification));
			var sent = false;

			var setting = _settingService.Get("EnableEmails")?.Value ?? "true";
			var settingsEnableEmails = string.Compare(setting, "true", true) == 0;
			if (test
				|| (_notificationSettings.EnableEmails
				&& settingsEnableEmails
				&& (notification.NotificationType?.NotificationTriggerId != NotificationTriggerTypes.Scheduled || notification.GrantApplication?.ScheduledNotificationsEnabled != false)))
			{
				try
				{
#if Training || Support
					notification.EmailRecipients = System.Configuration.ConfigurationManager.AppSettings["ExternalUserOverrideEmail"];
					if (String.IsNullOrEmpty(notification.EmailRecipients))
						throw new InvalidOperationException("Within the Training or Support environments the AppSetting 'ExternalUserOverrideEmail' must be set to a valid email address.");
#endif
					_emailSender.Send(notification.EmailRecipients, notification.EmailSubject, notification.EmailBody, notification.Organization.LegalName, notification.EmailSender);
					notification.State = NotificationState.Sent;
					notification.ErrorMessage = null;
					sent = true;
				}
				catch (Exception ex)
				{
					notification.State = NotificationState.Failed;
					notification.ErrorMessage = ex.GetAllMessages();
					_logger.Error(notification.ErrorMessage, $"Error while sending email notification ID: {notification.Id}");
				}
			}
			else
			{
				notification.State = NotificationState.Failed;
				notification.ErrorMessage = "Notifications are currently disabled.";
			}

			if (notification.Id == 0)
			{
				_dbContext.NotificationQueue.Add(notification);
			}
			else
			{
				_dbContext.Update(notification);
			}

			return sent;
		}

		/// <summary>
		/// Attempts to send notifications for the specified grant application.
		/// This function updates the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="originalState"></param>
		/// <param name="appDate"></param>
		/// <returns></returns>
		public IEnumerable<NotificationQueue> SendNotifications(GrantApplication grantApplication, ApplicationStateInternal originalState, DateTime appDate = default)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (appDate == default)
				appDate = DateTime.UtcNow;

			// Loop through all the grant application notifications
			foreach (var notification in grantApplication.NotificationQueue.Where(q => q.State != NotificationState.Sent && (q.SendDate == null || q.SendDate <= appDate)).ToArray())
			{
				var recipientsEmail = notification.GrantApplication.BusinessContactRoles.FirstOrDefault().User.EmailAddress;
				if (recipientsEmail != notification.EmailRecipients)
					notification.GrantApplication.BusinessContactRoles.FirstOrDefault().User.EmailAddress = notification.EmailRecipients;

				SendNotification(notification);

				CommitTransaction();
			}

			return grantApplication.NotificationQueue.ToArray();
		}

		/// <summary>
		/// Based on the appDate determine what notifications need to be generated and added to the queue.
		/// </summary>
		/// <param name="appDate"></param>
		public int QueueScheduledNotifications(DateTime appDate = default)
		{
			if (appDate == default)
				appDate = AppDateTime.UtcNow;

			var count = 0;
			int failed = 0;
			var take = 100;

			var defaultGrantProgramId = GetDefaultGrantProgramId();

			// Find all implemented Grant Programs and their Grant Applications
			var implementedGrantPrograms = _dbContext.GrantPrograms
				.Where(g => g.Id == defaultGrantProgramId)
				.Where(g => g.DateImplemented != null)
				.ToArray();

			foreach (var grantProgram in implementedGrantPrograms)
			{
				// Find all active scheduled notification types.
				var grantProgramNotificationTypes = grantProgram.GrantProgramNotificationTypes
					.Where(t => t.IsActive)
					.Where(t => t.NotificationType.IsActive)
					.Where(t => t.NotificationType.NotificationTriggerId == NotificationTriggerTypes.Scheduled)
					.ToArray();

				var skip = 0;
				var query = _dbContext.GrantApplications.Where(g =>
					g.ApplicationStateInternal > ApplicationStateInternal.Draft
					&& g.ApplicationStateInternal != ApplicationStateInternal.Closed
					&& g.ApplicationStateInternal != ApplicationStateInternal.ApplicationDenied				// CJG-480:
					&& g.ApplicationStateInternal != ApplicationStateInternal.AgreementRejected             // Added ApplicationDenied, ApplicationDenied, ApplicationWithdrawn,
					&& g.ApplicationStateInternal != ApplicationStateInternal.ApplicationWithdrawn          // CancelledByMinistry, CancelledByAgreementHolder, Unfunded
					&& g.ApplicationStateInternal != ApplicationStateInternal.CancelledByMinistry
					&& g.ApplicationStateInternal != ApplicationStateInternal.CancelledByAgreementHolder
					&& g.ApplicationStateInternal != ApplicationStateInternal.Unfunded
					&& g.ScheduledNotificationsEnabled
					&& g.GrantOpening.GrantStream.GrantProgramId == grantProgram.Id);

				var total = query.Count();

				while (skip < total)
				{
					// Queue notifications for each active Grant Application
					foreach (var grantApplication in query.OrderByDescending(g => g.DateUpdated).Skip(skip).Take(take).ToArray())
					{
						foreach (var grantProgramNotificationType in grantProgramNotificationTypes)
						{
							try
							{
								if (!CheckNotificationWorkflow(grantProgramNotificationType, grantApplication, appDate))
									continue;

								// Create a notification for each user
								foreach (var user in GetRecipients(grantApplication.Id).ToArray())
								{
									grantApplication.NotificationQueue.Add(GenerateNotificationMessage(grantApplication, user, grantProgramNotificationType));
									count++;
								}
							}
							catch (Exception ex)
							{
								failed++;
								_logger.Error(ex, $"Failed to add grant program '{grantProgram.ProgramCode}' notification '{grantProgramNotificationType.NotificationType.Id}:{grantProgramNotificationType.NotificationType.Caption}' to queue.");
							}
						}
					}

					// We commit 100 at a time.
					CommitTransaction();
					skip += take;
				}
			}

			if (failed > 0)
				throw new NotificationException($"Notifications failed to be added to the queue ({failed} failures), review the log for additional information.");

			return count;
		}

		/// <summary>
		/// Attempts to send emails for all the notifications in the queue.
		/// </summary>
		/// <param name="appDate"></param>
		/// <returns></returns>
		public IEnumerable<NotificationQueue> SendScheduledNotifications(DateTime appDate = default)
		{
			if (appDate == default)
				appDate = AppDateTime.UtcNow;

			var queuedNotifications = _dbContext.NotificationQueue.Where(n => n.State != NotificationState.Sent && (n.SendDate == null || n.SendDate <= appDate)).ToArray();
			foreach (var notification in queuedNotifications)
			{
				SendNotification(notification);

				CommitTransaction();
			}

			return queuedNotifications;
		}

		/// <summary>
		/// Creates and sends notifications relating to grant application workflow changes
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="notificationType"></param>
		public void HandleWorkflowNotification(GrantApplication grantApplication, GrantProgramNotificationType notificationType)
		{
			// Create and send a notification to each user attached to the grant application
			foreach (var user in GetRecipients(grantApplication.Id).ToArray())
			{
				try
				{
					var notification = GenerateNotificationMessage(grantApplication, user, notificationType);

					SendNotification(notification);

					grantApplication.NotificationQueue.Add(notification);
				}
				catch (Exception ex)
				{
					_logger.Error(ex, $"The notification type '{notificationType.NotificationTypeId}:{notificationType.NotificationType.Caption}' with the template '{notificationType.NotificationTemplateId}:{notificationType.NotificationTemplate.Caption}' was not able to generate a message.");
				}
			}
		}

		/// <summary>
		/// Creates and sends notifications relating to grant application workflow changes
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="notificationType"></param>
		public void HandleWorkflowNotification(GrantApplication grantApplication, NotificationType notificationType)
		{
			// Create and send a notification to each user attached to the grant application
			foreach (var user in GetRecipients(grantApplication.Id).ToArray())
			{
				try
				{
					var notification = GenerateNotificationMessage(grantApplication, user, notificationType);

					SendNotification(notification);

					grantApplication.NotificationQueue.Add(notification);
				}
				catch (Exception ex)
				{
					_logger.Error(ex, $"The notification type '{notificationType.Id}:{notificationType.Caption}' with the template '{notificationType.NotificationTemplateId}:{notificationType.NotificationTemplate.Caption}' was not able to generate a message.");
				}
			}
		}

		/// <summary>
		/// Creates a new instance of a Notification object and generates the notification messages.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="user"></param>
		/// <param name="grantProgramNotificationType"></param>
		/// <returns></returns>
		public NotificationQueue GenerateNotificationMessage(GrantApplication grantApplication, User user, GrantProgramNotificationType grantProgramNotificationType)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (grantProgramNotificationType == null)
				throw new ArgumentNullException(nameof(grantProgramNotificationType));

			var template = grantProgramNotificationType.NotificationTemplate;
			var model = new NotificationViewModel(grantApplication, user, _httpContext);
			var body = ParseDocumentTemplate(model, template.EmailBody);
			var subject = ParseDocumentTemplate(model, template.EmailSubject);
			var sender = $"{_notificationSettings.DefaultSenderName} <{_notificationSettings.DefaultSenderAddress}>";
			var type = grantProgramNotificationType.NotificationType;

			// Decode the output if any encoded html is detected (ie: HTML Denial Reason)
			if (ContainsEncodedHtml(body))
				body = HttpUtility.HtmlDecode(body);

			return new NotificationQueue(grantApplication, user, sender, body, subject, type);
		}

		private static bool ContainsEncodedHtml(string body)
		{
			return body.Contains("&lt");
		}

		/// <summary>
		/// Creates a new instance of a Notification object and generates the notification messages.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="user"></param>
		/// <param name="notificationType"></param>
		/// <returns></returns>
		public NotificationQueue GenerateNotificationMessage(GrantApplication grantApplication, User user, NotificationType notificationType)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (notificationType == null)
				throw new ArgumentNullException(nameof(notificationType));

			try
			{
				var template = notificationType.NotificationTemplate;
				var model = new NotificationViewModel(grantApplication, user, _httpContext);
				var body = ParseDocumentTemplate(model, template.EmailBody);
				var subject = ParseDocumentTemplate(model, template.EmailSubject);
				var sender = $"{_notificationSettings.DefaultSenderName} <{_notificationSettings.DefaultSenderAddress}>";
				var type = notificationType;

				return new NotificationQueue(grantApplication, user, sender, body, subject, type);
			}
			catch (TemplateCompilationException ex)
			{
				throw new TemplateException("EmailBody", "Template", ex);
			}
			catch (TemplateParsingException ex)
			{
				throw new TemplateException("EmailBody", "Template", ex);
			}
			catch (RuntimeBinderException ex)
			{
				throw new TemplateException("EmailBody", "Template", ex);
			}
		}

		/// <summary>
		/// Check to see if the template model contains any invalid "@Models..."
		/// </summary>
		/// <param name="text"></param>
		/// <param name="excludedKeywords"></param>
		/// <returns>String</returns>
		public string ValidateModelKeywords(string text, string[] excludedKeywords = null)
		{
			var regex = @"[^@A-Za-z](@[A-Za-z\(\)]+\.[A-Za-z]+)|([\s\>]@[\s\<])|[^@A-Za-z](@[A-Za-z]+)|([A-Za-z]+@)[^@A-Za-z]|^(@[A-Za-z\(\)]+.[A-Za-z]+)|([A-Za-z\(\)]+.[A-Za-z]+@)$|^@|@$";
			var subjectVariablesKeywords = Regex.Matches(text, regex).Cast<Match>().Select(m => m.Groups[0].Value).Distinct().ToArray();
			var modelKeywords = typeof(NotificationViewModel).GetPropertiesAsKeyValuePairs(excludedKeywords).Select(v => "Model." + v.Key).ToArray();
			var invalidKeywords = new List<string>();

			foreach (var item in subjectVariablesKeywords) {
				if (!modelKeywords.Any(item.Contains))
					invalidKeywords.Add(item);
			}

			return string.Join(", ", invalidKeywords);
		}

		#region Helpers
		/// <summary>
		/// Returns the notifications for the specified 'filter'.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <param name="query"></param>
		/// <returns>PageList<NotificationQueue></returns>
		private PageList<NotificationQueue> GetNotifications(int page, int quantity, NotificationFilter filter, IQueryable<NotificationQueue> query)
		{
			if (page <= 0)
				page = 1;

			if (quantity <= 0 || quantity > 100)
				quantity = 10;

			// Filter
			if (!string.IsNullOrWhiteSpace(filter.NotificationType))
				query = query.Where(n => n.NotificationType.Caption == filter.NotificationType);

			if (!string.IsNullOrWhiteSpace(filter.Organization))
				query = query.Where(n => n.Organization.LegalName == filter.Organization);

			if (!string.IsNullOrWhiteSpace(filter.Status))
				query = query.Where(n => n.State.ToString() == filter.Status);

			var total = query.Count();

			// Order By
			var orderBy = !String.IsNullOrWhiteSpace(filter.OrderBy) ? filter.OrderBy : $"{nameof(NotificationQueue.SendDate)} desc";
			query = query.OrderByProperty(orderBy)
				.Skip((page - 1) * quantity)
				.Take(quantity);

			return new PageList<NotificationQueue>(page, quantity, total, query.ToArray());
		}

		/// <summary>
		/// Get the recipients for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		private IQueryable<User> GetRecipients(int grantApplicationId)
		{
			var businessContactRoles = _dbContext.BusinessContactRoles
				.Where(bcr => bcr.GrantApplicationId == grantApplicationId)
				.Include(x => x.User);

			return businessContactRoles.Select(x => x.User).Distinct();
		}

		/// <summary>
		/// Determine if the specified grant program notification type should be added to the notification queue.
		/// </summary>
		/// <param name="grantProgramNotificationType"></param>
		/// <param name="grantApplication"></param>
		/// <param name="appDate"></param>
		/// <returns></returns>
		public bool CheckNotificationWorkflow(GrantProgramNotificationType grantProgramNotificationType, GrantApplication grantApplication, DateTime appDate = default)
		{
			return CheckNotificationWorkflow(grantProgramNotificationType, grantApplication, null, appDate);
		}

		/// <summary>
		/// Determine if the specified grant program notification type should be added to the notification queue.
		/// </summary>
		/// <param name="grantProgramNotificationType"></param>
		/// <param name="grantApplication"></param>
		/// <param name="previousState"></param>
		/// <param name="appDate"></param>
		/// <returns></returns>
		public bool CheckNotificationWorkflow(GrantProgramNotificationType grantProgramNotificationType, GrantApplication grantApplication, ApplicationStateInternal? previousState = null, DateTime appDate = default)
		{
			if (appDate == default)
				appDate = AppDateTime.UtcNow;

			var type = grantProgramNotificationType.NotificationType;
			var milestoneDate = GetMilestoneDate(grantApplication, type.MilestoneDateName, appDate);

			return grantProgramNotificationType.IsActive
			       && type.IsActive
			       && type.NotificationTrigger.IsActive
			       && (type.PreviousApplicationState == null || type.PreviousApplicationState == (previousState ?? grantApplication.StateChanges.LastOrDefault()?.FromState))
			       && (type.CurrentApplicationState == null || type.CurrentApplicationState == grantApplication.ApplicationStateInternal)
			       && (milestoneDate != null && milestoneDate?.AddDays(type.MilestoneDateOffset) <= appDate) // If the milestone date returns null this should result in a false.
			       && (type.MilestoneDateExpires == 0 || milestoneDate?.AddDays(type.MilestoneDateExpires) >= appDate) // If the milestone date returns null this should result in a true.
			       && ApprovalRule(grantApplication, type.ApprovalRule)
			       && ResendRule(grantApplication, type, appDate)
			       && (type.ParticipantReportRule == NotificationParticipantReportRules.NotApplicable || ParticipantReportingRule(grantApplication) == type.ParticipantReportRule)
			       && (type.ClaimReportRule == NotificationClaimReportRules.NotApplicable || NotificationClaimReportingRule(grantApplication, type.ClaimReportRule) == type.ClaimReportRule)
			       && (type.CompletionReportRule == NotificationCompletionReportRules.NotApplicable || CompletionReportingRule(grantApplication) == type.CompletionReportRule);
		}

		/// <summary>
		/// Get the date for the specified milestone rule.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="milestoneDateName"></param>
		/// <param name="appDate"></param>
		/// <returns></returns>
		private static DateTime? GetMilestoneDate(GrantApplication grantApplication, string milestoneDateName, DateTime appDate = default)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (appDate == default)
				appDate = AppDateTime.UtcNow;

			Enum.TryParse(milestoneDateName, out NotificationMilestoneDateName milestoneDateNameEnum);

			switch (milestoneDateNameEnum)
			{
				case NotificationMilestoneDateName.DateSubmitted:
					return grantApplication.DateSubmitted;

				case NotificationMilestoneDateName.TrainingStartDate:
					return grantApplication.TrainingPrograms.FirstOrDefault()?.StartDate;

				case NotificationMilestoneDateName.TrainingEndDate:
					return grantApplication.TrainingPrograms.FirstOrDefault()?.EndDate;

				case NotificationMilestoneDateName.DateCancelled:
					return grantApplication.DateCancelled;

				case NotificationMilestoneDateName.DateAccepted:
					return grantApplication.DateAdded;

				case NotificationMilestoneDateName.ParticipantReportingDueDate:
					return grantApplication.GrantAgreement?.ParticipantReportingDueDate;

				case NotificationMilestoneDateName.ReimbursementClaimDueDate:
					return grantApplication.GrantAgreement?.ReimbursementClaimDueDate;

				case NotificationMilestoneDateName.CompletionReportingDueDate:
					return grantApplication.GrantAgreement?.CompletionReportingDueDate;

				case NotificationMilestoneDateName.DeliveryStartDate:
					return grantApplication.StartDate;

				case NotificationMilestoneDateName.DeliveryEndDate:
					return grantApplication.EndDate;

				case NotificationMilestoneDateName.AgreementIssuedDate:
					return grantApplication.GrantAgreement?.StartDate;

				case NotificationMilestoneDateName.TodaysDate:
					return appDate;

				case NotificationMilestoneDateName.ClaimReturnedToApplicantDate:
					return grantApplication.GetStateChange(ApplicationStateInternal.ClaimReturnedToApplicant)?.ChangedDate;

				default:
					return appDate;
			}
		}

		/// <summary>
		/// Determine if the participant reporting rule is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private static NotificationParticipantReportRules ParticipantReportingRule(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var participants = grantApplication.ParticipantForms.Count();
			var agreedAmountParticipants = grantApplication.TrainingCost.AgreedParticipants;

			if (participants == 0) return NotificationParticipantReportRules.NoneReported;

			if (participants > agreedAmountParticipants)
				return NotificationParticipantReportRules.GreaterThanAgreedReported;

			if (participants < agreedAmountParticipants)
				return NotificationParticipantReportRules.LessThanAgreedReported;

			if (participants == agreedAmountParticipants)
				return NotificationParticipantReportRules.AllReported;

			return NotificationParticipantReportRules.NotApplicable;
		}

		/// <summary>
		/// Determine if the claim reporting rule is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="rule">The rule to be compared with.</param>
		/// <returns></returns>
		private static NotificationClaimReportRules NotificationClaimReportingRule(GrantApplication grantApplication, NotificationClaimReportRules rule)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			var currentClaim = grantApplication.GetCurrentClaim();
			var claims = grantApplication.Claims
				.Where(c => !c.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
				.ToList();
			var claimsTotal = claims.Sum(c => c.AmountPaidOrOwing());
			var agreedAmountCommitment = grantApplication.TrainingCost.AgreedCommitment;

			var anyClaims = claims.Any();

			if (!anyClaims)
				return NotificationClaimReportRules.NoneReported;

			if (rule == NotificationClaimReportRules.ClaimReported)
				return NotificationClaimReportRules.ClaimReported;

			if (currentClaim.ClaimState == ClaimState.PaymentRequested)
				return NotificationClaimReportRules.PaymentRequestIssued;

			if (currentClaim.ClaimState == ClaimState.ClaimPaid)
				return NotificationClaimReportRules.ClaimPaid;

			if (claimsTotal < agreedAmountCommitment)
				return NotificationClaimReportRules.AmountRemaining;

			if (claimsTotal > agreedAmountCommitment)
				return NotificationClaimReportRules.AmountOwing;

			if (grantApplication.Claims.Any(c => c.IsFinalClaim))
				return NotificationClaimReportRules.FinalClaimReported;

			return NotificationClaimReportRules.NotApplicable;
		}

		/// <summary>
		/// Determine if the completion reporting rule is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private NotificationCompletionReportRules CompletionReportingRule(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			var status = _completionReportService.GetCompletionReportStatus(grantApplication.Id);
			switch (status)
			{
				case "Submitted":
					return NotificationCompletionReportRules.Reported;
				case "Not Started":
				case "Incomplete":
				case "Complete":
					return NotificationCompletionReportRules.NotReported;
				default:
					return NotificationCompletionReportRules.NotApplicable;
			}
		}

		/// <summary>
		/// Determine if the resend rule is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="notificationType"></param>
		/// <param name="appDate"></param>
		/// <returns></returns>
		private bool ResendRule(GrantApplication grantApplication, NotificationType notificationType, DateTime appDate = default)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (appDate == default)
				appDate = AppDateTime.UtcNow;

			// Find all sent notifications of given notification type
			var resendRule = notificationType.ResendRule;
			var notifications = _dbContext.NotificationQueue
				.Where(q => q.GrantApplicationId == grantApplication.Id && q.NotificationTypeId == notificationType.Id)
				.ToList();

			if (!notifications.Any())
				return true;

			if (notifications.Any(n => n.State != NotificationState.Sent))
				return false; // Don't add a new notification to the queue if any are queued or failed presently.

			// If the notification type has a claim reporting rule, check if the current claim has already sent this notification.
			if (notificationType.ClaimReportRule.In(NotificationClaimReportRules.PaymentRequestIssued, NotificationClaimReportRules.ClaimPaid))
			{
				var currentClaim = grantApplication.GetCurrentClaim();
				if (currentClaim != null && notifications.Any(n => n.BatchNumber.Contains($"C:{currentClaim.Id}-CV:{currentClaim.ClaimVersion}"))) return false;
			}

			var latestNotificationDate = notifications.Max(n => n.DateUpdated);
			var resendDelay = notificationType.ResendDelayDays;

			switch (resendRule)
			{
				case NotificationResendRules.Always:
					if (resendDelay == 0)
						return true;

					// Don't queue if today is not after resend delay.
					return !latestNotificationDate.HasValue || latestNotificationDate.Value.AddDays(resendDelay) >= appDate;

				case NotificationResendRules.Never:
					return !notifications.Any();

				case NotificationResendRules.AgreementDateChanged:
					// If the last time this notification was sent was before the agreement was updated then send another.
					// A note for training dates changed highlights an agreement date change.
					var agreementUpdatedOn = _dbContext.Notes
						.OrderByDescending(n => n.DateAdded)
						.FirstOrDefault(n => n.GrantApplicationId == grantApplication.Id && n.NoteTypeId == NoteTypes.TD)?.DateAdded;
					return agreementUpdatedOn.HasValue && latestNotificationDate < agreementUpdatedOn;

				default: return true; // Default to send.
			}
		}

		/// <summary>
		/// Determine if the approval rule is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="approvalRule"></param>
		/// <returns></returns>
		private static bool ApprovalRule(GrantApplication grantApplication, NotificationApprovalRules approvalRule)
		{
			switch (approvalRule)
			{
				case NotificationApprovalRules.NotApplicable:
					return true;
				case NotificationApprovalRules.AgreementAccepted:
					return grantApplication?.GrantAgreement?.DateAccepted.HasValue ?? false;
				case NotificationApprovalRules.OfferIssued:
					return grantApplication.ApplicationStateInternal >= ApplicationStateInternal.OfferIssued
						&& grantApplication.ApplicationStateInternal != ApplicationStateInternal.OfferWithdrawn
						&& grantApplication.ApplicationStateInternal != ApplicationStateInternal.AgreementRejected
						&& grantApplication.ApplicationStateInternal != ApplicationStateInternal.CancelledByAgreementHolder
						&& grantApplication.ApplicationStateInternal != ApplicationStateInternal.CancelledByMinistry;
				default:
					return false;
			}
		}
		#endregion
	}
}