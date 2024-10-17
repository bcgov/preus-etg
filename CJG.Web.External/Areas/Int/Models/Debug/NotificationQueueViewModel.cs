using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.Debug
{
	public class NotificationQueueViewModel
	{
		#region Properties
		public string StateCaption { get; set; }
		public string FileNumber { get; set; }
		public string NotificationTypeCaption { get; set; }
		public string NotificationTypeTriggerCaption { get; set; }
		public string OrganizationLegalName { get; set; }
		public string BatchNumber { get; set; }
		public string EmailSubject { get; set; }
		public string EmailBody { get; set; }
		public string ErrorMessage { get; set; }
		public DateTime? SendDate { get; set; }
		public DateTime? SentDate { get; set; }
		#endregion

		#region Constructors
		public NotificationQueueViewModel()
		{

		}

		public NotificationQueueViewModel(NotificationQueue notificationQueue)
		{
			if (notificationQueue == null) throw new ArgumentNullException(nameof(notificationQueue));

			this.StateCaption = notificationQueue.State.GetDescription();
			this.FileNumber = notificationQueue.GrantApplication?.FileNumber;
			this.NotificationTypeCaption = notificationQueue.NotificationType?.Caption;
			this.NotificationTypeTriggerCaption = notificationQueue.NotificationType?.NotificationTrigger?.Caption;
			this.OrganizationLegalName = notificationQueue.Organization.LegalName;
			this.BatchNumber = notificationQueue.BatchNumber;
			this.EmailSubject = notificationQueue.EmailSubject;
			this.EmailBody = notificationQueue.EmailBody;
			this.ErrorMessage = notificationQueue.ErrorMessage;
			this.SendDate = notificationQueue.SendDate;
			this.SentDate = notificationQueue.State == NotificationState.Sent ? notificationQueue.DateUpdated : null;
		}
		#endregion
	}
}