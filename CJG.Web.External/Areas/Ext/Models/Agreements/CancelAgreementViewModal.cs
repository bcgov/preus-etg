using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class CancelAgreementViewModal : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "Please provide a reason for cancelling the agreement."), MaxLength(800)]
		public string CancelReason { get; set; }

		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public CancelAgreementViewModal()
		{

		}
		public CancelAgreementViewModal(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
		}
		#endregion
	}
}