using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.ProgramNotifications
{
	public class ProgramNotificationRecipientViewModel
	{
		#region Properties
		public int ProgramNotificationId { get; set; }
		public int GrantProgramId { get; set; }
		public bool ApplicantOnly { get; set; }
		public bool SubscriberOnly { get; set; }
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public ProgramNotificationRecipientViewModel() { }

		public ProgramNotificationRecipientViewModel(ProgramNotificationRecipient notificationRecipient)
		{
			if (notificationRecipient == null) throw new ArgumentNullException(nameof(notificationRecipient));

			Utilities.MapProperties(notificationRecipient, this);
		}
		#endregion
	}
}
