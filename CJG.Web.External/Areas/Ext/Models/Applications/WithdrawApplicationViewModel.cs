using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class WithdrawApplicationViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		[Required(ErrorMessage = "Reason for withdrawing is required")]
		public string WithdrawReason { get; set; }

		#endregion

		#region Constructors
		public WithdrawApplicationViewModel() { }

		public WithdrawApplicationViewModel(GrantApplication grantApplication)
		{
			this.Id = grantApplication?.Id ?? throw new ArgumentNullException(nameof(grantApplication));
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.WithdrawReason = grantApplication.GetReason(ApplicationStateInternal.ApplicationWithdrawn);
		}
		#endregion
	}
}