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
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;

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
			var notifications = _notificationService.SendScheduledNotifications(currentDate).ToList();
			foreach (var notification in notifications)
			{
				var notificationTypeCaption = notification.NotificationType?.Caption ?? string.Empty;
				switch (notification.State)
				{
					case NotificationState.Sent:
						_logger.Info("Successfully sent notification for Application Id: {0} / Notification Type: {1}", notification.GrantApplicationId, notificationTypeCaption);
						break;
					case NotificationState.Failed:
						_logger.Error("Failed to send notification for Application Id: {0} / Notification Type: {1}", notification.GrantApplicationId, notificationTypeCaption);
						break;
					case NotificationState.Queued:
						_logger.Info("Notification for Application Id: {0} / Notification Type: {1} is marked as 'Queued'", notification.GrantApplicationId, notificationTypeCaption);
						break;
					default:
						_logger.Error("Notification for Application Id: {0} / Notification Type: {1} has an invalid state", notification.GrantApplicationId, notificationTypeCaption);
						break;
				}
			}

			_logger.Info("Processed {0} notifications - Sent: {1}, Failed: {2}, Queued: {3}",
				notifications.Count,
				notifications.Count(n => n.State == NotificationState.Sent),
				notifications.Count(n => n.State == NotificationState.Failed),
				notifications.Count(n => n.State == NotificationState.Queued));

			return SystemExitCode.Success;
		}
	}
}
