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
		public DateTime MaxEndDate { get; set; }

		private readonly Func<DateTime, DateTime> MaxEndDateGetter = startdate =>
		{
			if (startdate.Year > AppDateTime.Now.Year) // just in case start date is already 1 year ahead.
				return startdate;

			return startdate.AddYears(1);
		};

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
			this.TermEndDate = grantApplication.GrantAgreement.ConvertEndDateToLocalTime();
			this.ParticipantReportingDueDate = grantApplication.GrantAgreement.ParticipantReportingDueDate.ToLocalTime();
			this.ReimbursementClaimDueDate = grantApplication.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime();
			this.MaxEndDate = MaxEndDateGetter(grantApplication.GrantAgreement.StartDate.ToLocalTime());
		}
		#endregion
	}
}