using System;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class DeliveryDateViewModel : BaseViewModel
	{
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

		public DeliveryDateViewModel()
		{

		}
		public DeliveryDateViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			StartDate = grantApplication.StartDate.ToLocalTime();
			EndDate = grantApplication.EndDate.ToLocalTime();
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			TermStartDate = grantApplication.GrantAgreement.StartDate.ToLocalTime();
			TermEndDate = grantApplication.GrantAgreement.ConvertEndDateToLocalTime();
			ParticipantReportingDueDate = grantApplication.GrantAgreement.ParticipantReportingDueDate.ToLocalTime();
			ReimbursementClaimDueDate = grantApplication.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime();
			MaxEndDate = MaxEndDateGetter(grantApplication.GrantAgreement.StartDate.ToLocalTime());
		}
	}
}