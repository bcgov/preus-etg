using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationQueueListViewModel : BaseViewModel
	{
		#region Properties
		public PageList<NotificationQueueViewModel> Notifications { get; set; }
		#endregion

		#region Constructors
		public NotificationQueueListViewModel() { }

		public NotificationQueueListViewModel(PageList<NotificationQueueViewModel> pageListOfNotifications)
		{
			this.Notifications = pageListOfNotifications ?? throw new ArgumentNullException("Page list of notifications cannot be null.");
		}
		#endregion
	}	
}