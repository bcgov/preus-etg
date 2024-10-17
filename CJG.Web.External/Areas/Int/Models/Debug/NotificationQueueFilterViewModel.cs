using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Debug
{
	public class NotificationQueueFilterViewModel
	{
		#region Properties
		public int Page { get; set; }
		public int Quantity { get; set; }
		public string Search { get; set; }
		public string Status { get; set; } = "Queued,Failed";
		public string OrderBy { get; set; } = "State";
		#endregion

		#region Constructors
		#endregion

		#region Methods
		public NotificationFilter GenerateFilter()
		{
			return new NotificationFilter(this.Search, this.Status, this.OrderBy);
		}
		#endregion
	}
}