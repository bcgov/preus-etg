using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using NLog;
using System;
using System.Linq;

namespace CJG.Infrastructure.NotificationService
{
	/// <summary>
	/// <typeparamref name="NotificationJob"/> class, provides the process notification schedule queue
	/// </summary>
	internal class NotificationJob : INotificationJob
	{
		#region Variables
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationJob"/> object.
		/// </summary>
		/// <param name="notificationService"></param>
		/// <param name="logger"></param>
		public NotificationJob(INotificationService notificationService, ILogger logger)
		{
			_notificationService = notificationService;
			_logger = logger;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Start the notification schedule queue service process.
		/// </summary>
		/// <param name="currentDate"></param>
		public SystemExitCode StartNotificationService(DateTime currentDate)
		{
			_logger.Debug($"Starting '{nameof(NotificationJob)}' on {currentDate:G}");

			// Create notifications
			_notificationService.QueueScheduledNotifications(currentDate);

			// Send notifications
			var notifications = _notificationService.SendScheduledNotifications(currentDate);
			foreach (var notification in notifications) {
				switch (notification.State) {
					case NotificationState.Sent:
						_logger.Info("Successfully sent notification for Application Id: {0} / Notification Type: {1}", notification.GrantApplicationId, notification.NotificationType.Caption);
						break;
					case NotificationState.Failed:
						_logger.Error("Failed to send notification for Application Id: {0} / Notification Type: {1}", notification.GrantApplicationId, notification.NotificationType.Caption);
						break;
					case NotificationState.Queued:
						_logger.Info("Notification for Application Id: {0} / Notification Type: {1} is marked as 'Queued'", notification.GrantApplicationId, notification.NotificationType.Caption);
						break;
					default:
						_logger.Error("Notification for Application Id: {0} / Notification Type: {1} has an invalid state", notification.GrantApplicationId, notification.NotificationType.Caption);
						break;
				}
			}

			_logger.Info("Processed {0} notifications - Sent: {1}, Failed: {2}, Queued: {3}",
				notifications.Count(),
				notifications.Where(n => n.State == NotificationState.Sent).Count(),
				notifications.Where(n => n.State == NotificationState.Failed).Count(),
				notifications.Where(n => n.State == NotificationState.Queued).Count());

			return SystemExitCode.Success;
		}
		#endregion
	}
}
