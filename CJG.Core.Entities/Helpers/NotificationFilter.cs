namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationFilter struct, provides an ORM for internal user filters.
	/// </summary>
	public struct NotificationFilter
	{
		#region Properties
		public string Organization { get; }
		public string NotificationType { get; }
		public string Status { get; }
		public string Caption { get; }
		public string Search { get;  }
		public NotificationTriggerTypes? TriggerType { get; }
		public string OrderBy { get; }
		#endregion

		#region Constructors
		public NotificationFilter(string organization, string notificationType, string status, string caption, NotificationTriggerTypes triggerType, string orderBy)
		{
			this.Search = null;
			this.Organization = organization;
			this.NotificationType = notificationType;
			this.Status = status;
			this.Caption = caption;
			this.TriggerType = triggerType;
			this.OrderBy = orderBy;
		}

		public NotificationFilter(string search, string status, string orderBy)
		{
			this.Search = search;
			this.Organization = null;
			this.NotificationType = null;
			this.Status = status;
			this.Caption = null;
			this.TriggerType = null;
			this.OrderBy = orderBy;
		}
		#endregion
	}
}
