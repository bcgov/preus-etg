using CJG.Core.Entities.Helpers;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationListViewModel : BaseViewModel
	{
		#region Properties
		public PageList<NotificationQueueViewModel> Notifications { get; set; }
		public List<string> NotificationTypes { get; set; }
		public List<string> Organizations { get; set; }
		#endregion

		#region Constructors
		public GrantProgramNotificationListViewModel() { }

		public GrantProgramNotificationListViewModel(PageList<NotificationQueueViewModel> pageListOfNotifications) {
			this.Notifications = pageListOfNotifications ?? throw new ArgumentNullException("Page list of notifications cannot be null.");
		}
		#endregion
	}
}