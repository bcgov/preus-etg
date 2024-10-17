using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.ProgramNotifications
{
	public class ProgramNotificationListViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		#endregion

		#region Constructors
		public ProgramNotificationListViewModel() { }

		public ProgramNotificationListViewModel(ProgramNotification notification)
		{
			if (notification == null) throw new ArgumentNullException(nameof(notification));

			Utilities.MapProperties(notification, this);
		}
		#endregion
	}
}
