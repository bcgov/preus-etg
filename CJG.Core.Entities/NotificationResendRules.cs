using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationResendRules enum, provides a way to identify when to resend the notification.
	/// </summary>
	public enum NotificationResendRules
	{
		/// <summary>
		/// Never - Notification should be sent only once
		/// </summary>
		[Description("Never")]
		Never = 0,
		/// <summary>
		/// Always - Notification should always be resent.
		/// </summary>
		[Description("Always")]
		Always = 1,
		/// <summary>
		/// AgreementDateChanged - Notification should be resent on the agreement date.
		/// </summary>
		[Description("Agreement Date Changed")]
		AgreementDateChanged = 2
	}
}
