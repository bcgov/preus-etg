using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeListViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<NotificationTypeViewModel> NotificationTypes { get; set; }
		#endregion

		#region Constructors
		public NotificationTypeListViewModel() { }

		public NotificationTypeListViewModel(IEnumerable<NotificationType> notificationTypes)
		{
			if (notificationTypes == null) throw new ArgumentNullException("Notification types cannot be empty.");

			this.NotificationTypes = notificationTypes.Select(n => new NotificationTypeViewModel(n)).ToArray();
		}
		#endregion
	}
}