using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class IncludeParticipantsViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public int[] ParticipantFormIds { get; set; }
		public bool Include { get; set; }
		public string ClaimRowVersion { get; set; }
		#endregion

		#region Constructors
		public IncludeParticipantsViewModel() { }
		#endregion
	}
}