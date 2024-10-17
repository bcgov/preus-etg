namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class GrantProgramNotificationViewModel
	{
		#region Properties
		public int GrantProgramId { get; set; }
		public bool ApplicantsOnly { get; set; }
		#endregion

		#region Constructors
		public GrantProgramNotificationViewModel() { }
		#endregion
	}
}