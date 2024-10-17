using CJG.Application.Services;
using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationViewModel
	{
		#region Properties
		public int NotificationTypeId { get; set; }

		public string RowVersion { get; set; }

		public bool IsActive { get; set; }

		public bool IsDisabled { get; set; }

		public string Caption { get; set; }

		public string Description { get; set; }

		public string MilestoneDateName { get; set; }

		public int MilestoneDateOffset { get; set; }
		#endregion

		#region Constructors
		public GrantProgramNotificationViewModel() { }

		public GrantProgramNotificationViewModel(GrantProgramNotificationType notification)
		{
			if (notification == null) throw new ArgumentNullException(nameof(notification));

			Utilities.MapProperties(notification.NotificationType, this);
			Utilities.MapProperties(notification, this);
			this.IsDisabled = !notification.NotificationType.IsActive;
		}
		#endregion
	}
}