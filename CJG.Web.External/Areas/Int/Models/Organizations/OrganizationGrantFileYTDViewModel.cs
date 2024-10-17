using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationGrantFileYTDViewModel : BaseViewModel
	{
		public decimal TotalRequested { get; set; }
		public decimal TotalApproved { get; set; }
		public decimal TotalPaid { get; set; }
	}
}
