using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationQueueFilterViewModel : BaseViewModel
	{
		#region Properties
		public string Organization { get; set; }
		public string NotificationType { get; set; }
		public string Status { get; set; }
		public string Caption { get; set; }
		public NotificationTriggerTypes TriggerType { get; set; }
		public string OrderBy { get; set; }
		#endregion

		#region Constructors
		public NotificationQueueFilterViewModel() { }

		public NotificationFilter GetFilter() 
		{
			return new NotificationFilter(this.Organization, this.NotificationType, this.Status, this.Caption, this.TriggerType, this.OrderBy);
		}
		#endregion
	}
}