using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Entities.Helpers
{
	public struct NotificationTypeFilter
	{
		#region Properties
		public NotificationTriggerTypes NotificationTriggerId { get; }
		public string Caption{ get; }
		public string OrderBy { get; }
		#endregion

		#region Constructors
		public NotificationTypeFilter(NotificationTriggerTypes triggerId, string caption, string orderBy = null)
		{
			NotificationTriggerId = triggerId;
			Caption = caption;
			OrderBy = orderBy;
		}
		#endregion
	}
}
