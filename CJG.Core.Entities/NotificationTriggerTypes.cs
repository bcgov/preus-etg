﻿using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationTriggerTypes enum, provides a way to identify notification trigger types.
	/// </summary>
	public enum NotificationTriggerTypes
	{
		/// <summary>
		/// Workflow - Notifications that are triggered when a workflow change occurs.
		/// </summary>
		Workflow = 1,

		/// <summary>
		/// Scheduled - Notifications that are set to be triggered during a scheduled date.
		/// </summary>
		Scheduled = 2,

		/// <summary>
		/// EmailTemplate - Notifications that are only used as an email template.
		/// </summary>
		[Description("Email Template")]
		EmailTemplate = 3
	}
}
