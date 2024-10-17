using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationProfileViewModel : BaseViewModel
	{
		public int SelectedUserId { get; set; }
		public string RowVersion { get; set; }
	}
}
