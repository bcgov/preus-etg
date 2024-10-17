using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.ProgramNotifications
{
	public class ProgramNotificationTemplateViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string Caption { get; set; }
		public string EmailSubject { get; set; }
		public string EmailBody { get; set; }
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public ProgramNotificationTemplateViewModel() { }

		public ProgramNotificationTemplateViewModel(NotificationTemplate notificationTemplate)
		{
			if (notificationTemplate == null) throw new ArgumentNullException(nameof(notificationTemplate));

			Utilities.MapProperties(notificationTemplate, this);
		}
		#endregion
	}
}
