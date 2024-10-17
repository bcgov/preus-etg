using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class EnableNotificationViewModel : BaseViewModel
	{
		#region Properties

		public string RowVersion { get; set; }

		public bool ScheduledNotificationsEnabled { get; set; }
		#endregion

		#region Constructors
		public EnableNotificationViewModel() { }

		public EnableNotificationViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			Utilities.MapProperties(grantApplication, this);
		}
		#endregion
	}
}
