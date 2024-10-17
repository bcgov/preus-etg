using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTemplateViewModel : BaseViewModel
	{
		#region Properties
		[Required]
		public string Caption { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "The Subject field is required."), MaxLength(500)]
		public string EmailSubject { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "The Body field is required.")]
		public string EmailBody { get; set; }
		#endregion

		#region Constructors	
		public NotificationTemplateViewModel() { }

		public NotificationTemplateViewModel(NotificationTemplate notificationTemplate)
		{
			if (notificationTemplate == null) throw new ArgumentNullException("Notification template cannot be null.");

			this.Id = notificationTemplate.Id;
			this.Caption = notificationTemplate.Caption;
			this.EmailSubject = notificationTemplate.EmailSubject;
			this.EmailBody = notificationTemplate.EmailBody;
		}
		#endregion
	}	
}