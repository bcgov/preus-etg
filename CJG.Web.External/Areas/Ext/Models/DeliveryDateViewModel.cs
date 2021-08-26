using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class DeliveryDateViewModel : BaseViewModel
	{
		#region Properties
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string RowVersion { get; set; }
		public DateTime TermStartDate { get; set; }
		public DateTime TermEndDate { get; set; }
		public DateTime ParticipantReportingDueDate { get; set; }
		public DateTime ReimbursementClaimDueDate { get; set; }
		#endregion

		#region Constructors
		public DeliveryDateViewModel()
		{

		}
		public DeliveryDateViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			this.Id = grantApplication.Id;
			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.TermStartDate = grantApplication.GrantAgreement.StartDate.ToLocalTime();
			this.TermEndDate = grantApplication.GrantAgreement.EndDate.ToLocalTime();
			this.ParticipantReportingDueDate = grantApplication.GrantAgreement.ParticipantReportingDueDate.ToLocalTime();
			this.ReimbursementClaimDueDate = grantApplication.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime();

		}
		#endregion
	}
}