using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Applicants
{
	public class ChangeApplicantContactViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int ApplicantContactId { get; set; }
		#endregion
	}
}