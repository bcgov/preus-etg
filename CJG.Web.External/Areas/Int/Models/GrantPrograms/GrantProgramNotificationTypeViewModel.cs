using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationTypeViewModel : BaseViewModel
	{
		#region Properties
		public int GrantProgramId { get; set; }
		public int NotificationTypeId { get; set; }
		public NotificationTemplateViewModel NotificationTemplate { get; set; } = new NotificationTemplateViewModel();
		public string Caption { get; set; }
		public string Description { get; set; }
		public string TriggerType { get; set; }
		public bool IsActive { get; set; }
		public bool ToBeDeleted { get; set; }

		#endregion

		#region Constructors
		public GrantProgramNotificationTypeViewModel() { }

		public GrantProgramNotificationTypeViewModel(GrantProgramNotificationType grantProgramNotificationType) {
			if (grantProgramNotificationType == null) throw new ArgumentNullException(nameof(grantProgramNotificationType));

			this.GrantProgramId = grantProgramNotificationType.GrantProgramId;
			this.NotificationTypeId = grantProgramNotificationType.NotificationTypeId;
			this.NotificationTemplate.Id = grantProgramNotificationType.NotificationTemplateId;
			this.NotificationTemplate.Caption = grantProgramNotificationType.NotificationTemplate.Caption;
			this.NotificationTemplate.EmailSubject = grantProgramNotificationType.NotificationTemplate.EmailSubject;
			this.NotificationTemplate.EmailBody = grantProgramNotificationType.NotificationTemplate.EmailBody;
			this.Caption = grantProgramNotificationType.NotificationType.Caption;
			this.Description = grantProgramNotificationType.NotificationType.Description;
			this.TriggerType = grantProgramNotificationType.NotificationType.NotificationTrigger.Caption;
			this.IsActive = grantProgramNotificationType.IsActive;
		}
		#endregion
	}
}