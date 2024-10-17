using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Applications;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationQueuePreviewModel : ApplicationPreviewModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public NotificationTriggerTypes NotificationTriggerId { get; set; }

		public NotificationQueuePreviewModel() { }
	}
}
