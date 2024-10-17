using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationSendStatus enum, provides a list of notificaiton send statuses.
	/// </summary>
	public enum NotificationSendStatus
	{
		/// <summary>
		/// Scheduled - The notificaiton is scheduled.
		/// </summary>
		Scheduled = 0,
		/// <summary>
		/// Sent - The notification has been sent.
		/// </summary>
		Sent = 1,
		/// <summary>
		/// FailedToSend - The notification failed to be sent.
		/// </summary>
		[Description("Failed To Send")]
		FailedToSend = 2,
		/// <summary>
		/// Invalid - The notification no longer is required to be sent.
		/// </summary>
		Invalid = 3
	}
}
