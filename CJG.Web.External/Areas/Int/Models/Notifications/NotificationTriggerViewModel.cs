using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTriggerViewModel : BaseViewModel
	{
		#region Properties
        [Required(ErrorMessage = "The Name field is required."), MaxLength(250)]
		public string Caption { get; set; }
		public string Description { get; set; }
		#endregion

		#region Constructors
		public NotificationTriggerViewModel() { }

		public NotificationTriggerViewModel(NotificationTrigger notificationTrigger)
		{
			if (notificationTrigger == null) throw new ArgumentNullException(nameof(notificationTrigger));

			Utilities.MapProperties(notificationTrigger, this);
		}
		#endregion
	}
}