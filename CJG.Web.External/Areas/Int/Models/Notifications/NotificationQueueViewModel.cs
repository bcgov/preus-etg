using System;
using System.ComponentModel.DataAnnotations;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
    public class NotificationQueueViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Status { get; set; }

		[Required]
		public int NotificationTypeId { get; set; }
		public string NotificationTypeCaption { get; set; }
		public int GrantApplicationId { get; set; }
		[Required]
		public int OrganizationId { get; set; }
		public string OrganizationCaption { get; set; }

		[Required]
		public string BatchNumber { get; set; }

		[Required]
		public string EmailSubject { get; set; }

		[Required]
		public string EmailBody { get; set; }

		[Required]
		public string EmailRecipients { get; set; }

		[Required]
		public string EmailSender { get; set; }

		[Required]
		public int State { get; set; }
		public string ErrorMessage { get; set; }
		public DateTime? SendDate { get; set; }
		public DateTime? SentOn { get; set; }

		public NotificationQueueViewModel() { }

		public NotificationQueueViewModel(NotificationQueue notification)
		{
			if (notification == null) throw new ArgumentNullException("Notification cannot be null.");

			Utilities.MapProperties(notification, this);

			Name = notification.NotificationType?.Caption;
			Description = notification.NotificationType?.Description;
			Status = notification.State.GetDescription();
			NotificationTypeCaption = notification.NotificationType?.Caption;
			OrganizationCaption = notification.Organization.LegalName;
			SentOn = notification.State == NotificationState.Sent ? notification.DateUpdated ?? notification.DateAdded : (DateTime?)null;
		}
	}
}