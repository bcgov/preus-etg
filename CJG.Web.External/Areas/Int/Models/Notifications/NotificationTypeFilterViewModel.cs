using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeFilterViewModel : BaseViewModel
	{
		#region Properties
		public NotificationTriggerTypes NotificationTriggerId { get; set; }
		public string Caption { get; set;  }
		public string OrderBy { get; set; }
		#endregion

		#region Constructors
		public NotificationTypeFilterViewModel() { }

		public NotificationTypeFilter GetFilter() 
		{
			return new NotificationTypeFilter(this.NotificationTriggerId, this.Caption, this.OrderBy);
		}
		#endregion
	}
}