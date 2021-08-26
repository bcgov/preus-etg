namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="NotificationState"/> enum, provides a way to identify the state of the notification.
	/// </summary>
	public enum NotificationState
	{
		/// <summary>
		/// Queued - Notification is queued to be sent.
		/// </summary>
		Queued = 0,
		/// <summary>
		/// Sent - Notification was sent.
		/// </summary>
		Sent = 1,
		/// <summary>
		/// Failed - Notification failed to be sent.
		/// </summary>
		Failed = 2
	}
}
