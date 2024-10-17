using System;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationTypeViewModel : BaseViewModel
	{
		public int GrantProgramId { get; set; }
		public int NotificationTypeId { get; set; }
		public NotificationTemplateViewModel NotificationTemplate { get; set; } = new NotificationTemplateViewModel();
		public string Caption { get; set; }
		public string Description { get; set; }
		public string TriggerType { get; set; }
		public bool IsActive { get; set; }
		public bool ToBeDeleted { get; set; }

		public GrantProgramNotificationTypeViewModel()
		{
		}

		public GrantProgramNotificationTypeViewModel(GrantProgramNotificationType grantProgramNotificationType)
		{
			if (grantProgramNotificationType == null)
				throw new ArgumentNullException(nameof(grantProgramNotificationType));

			GrantProgramId = grantProgramNotificationType.GrantProgramId;
			NotificationTypeId = grantProgramNotificationType.NotificationTypeId;
			NotificationTemplate.Id = grantProgramNotificationType.NotificationTemplateId;
			NotificationTemplate.Caption = grantProgramNotificationType.NotificationTemplate.Caption;
			NotificationTemplate.EmailSubject = grantProgramNotificationType.NotificationTemplate.EmailSubject;
			NotificationTemplate.EmailBody = grantProgramNotificationType.NotificationTemplate.EmailBody;
			Caption = grantProgramNotificationType.NotificationType.Caption;
			Description = grantProgramNotificationType.NotificationType.Description;
			TriggerType = grantProgramNotificationType.NotificationType.NotificationTrigger.Caption;
			IsActive = grantProgramNotificationType.IsActive;
		}
	}
}