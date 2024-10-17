using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Debug;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models
{
	public class DebugNotificationsViewModel : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "The run as date is required.")]
		public DateTime? RunAsDate { get; set; }

		public int AddedToQueue { get; set; }

		public int NotificationsInQueue { get; set; }

		public IEnumerable<NotificationQueueViewModel> Queue { get; set; }
		#endregion

		#region Constructors
		public DebugNotificationsViewModel()
		{
			this.RunAsDate = AppDateTime.UtcNow;
		}

		public DebugNotificationsViewModel(int addedToQueue, IEnumerable<NotificationQueueViewModel> queue) : this()
		{
			this.AddedToQueue = addedToQueue;
			this.Queue = queue;
		}
		#endregion
	}
}