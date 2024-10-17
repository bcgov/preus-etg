using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Reconciliation
{
	public class ReconciliationReportViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public DateTime DateRun { get; set; }
		public DateTime PeriodFrom { get; set; }
		public DateTime PeriodTo { get; set; }
		public string Requestor { get; set; }
		public string ReportedBy { get; set; }
		public bool IsReconciled { get; set; }
		public IEnumerable<ReconciliationPaymentViewModel> Payments { get; set; }
		#endregion

		#region Constructors
		public ReconciliationReportViewModel() { }

		public ReconciliationReportViewModel(ReconciliationReport report)
		{
			if (report == null) throw new ArgumentNullException(nameof(report));

			this.Id = report.Id;
			this.RowVersion = Convert.ToBase64String(report.RowVersion);
			this.DateRun = report.DateRun.ToLocalTime();
			this.PeriodFrom = report.PeriodFrom.ToLocalTime();
			this.PeriodTo = report.PeriodTo.ToLocalTime();
			this.Requestor = report.Requestor;
			this.ReportedBy = $"{report.Creator.FirstName} {report.Creator.LastName}";
			this.IsReconciled = report.IsReconciled;

			var groups = report.Payments.Select(p => new ReconciliationPaymentReportModel(p, p.PaymentRequest)).Distinct().ToArray();

			this.Payments = groups.Select(pr =>
			{
				if (pr.PaymentRequest == null) return new ReconciliationPaymentViewModel(pr.ReconciliationPayment);
				if (pr.PaymentRequest.ReconciliationPayments.Count() == 1) return new ReconciliationPaymentViewModel(pr.ReconciliationPayment);

				var recPayments = pr.PaymentRequest.ReconciliationPayments.Where(r => r.FromCAS);
				if (recPayments.Count() == 1) return new ReconciliationPaymentViewModel(recPayments.First());
				return new ReconciliationPaymentViewModel(pr.PaymentRequest);
			}).ToArray();
		}
		#endregion
	}
}