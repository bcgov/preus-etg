using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class HoldPaymentRequestViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public bool HoldPaymentRequests { get; set; }
		#endregion

		#region Constructors
		public HoldPaymentRequestViewModel() { }

		public HoldPaymentRequestViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.HoldPaymentRequests = grantApplication.HoldPaymentRequests;
		}
		#endregion
	}
}