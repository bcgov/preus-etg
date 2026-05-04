using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeListViewModel : BaseViewModel
	{
		public IEnumerable<NotificationTypeViewModel> NotificationTypes { get; set; }

		public NotificationTypeListViewModel() { }

		public NotificationTypeListViewModel(IEnumerable<NotificationType> notificationTypes)
		{
			if (notificationTypes == null)
				throw new ArgumentNullException("Notification types cannot be empty.");

			NotificationTypes = notificationTypes.Select(n => new NotificationTypeViewModel(n)).ToArray();
		}
	}
}