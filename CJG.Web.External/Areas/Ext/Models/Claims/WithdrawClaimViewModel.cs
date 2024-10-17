using CJG.Web.External.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class WithdrawClaimViewModel : BaseViewModel
	{
		#region Properties
		public int ClaimId { get; set; }

		public int ClaimVersion { get; set; }

		public string RowVersion { get; set; }

		[Required(ErrorMessage = "Reason for withdrawing is required")]
		public string WithdrawReason { get; set; }
		#endregion
	}
}