using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.Reconciliation
{
	public class ReconciliationReportsViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public DateTime DateRun { get; set; }
		public DateTime PeriodFrom { get; set; }
		public DateTime PeriodTo { get; set; }
		public string Requestor { get; set; }
		public bool IsReconciled { get; set; }
		public bool IsDirector { get; set; }
		#endregion

		#region Constructors
		public ReconciliationReportsViewModel() { }

		public ReconciliationReportsViewModel(ReconciliationReport report, IPrincipal user)
		{
			if (report == null) throw new ArgumentNullException(nameof(report));

			this.Id = report.Id;
			this.RowVersion = Convert.ToBase64String(report.RowVersion);
			this.DateRun = report.DateRun.ToLocalTime();
			this.PeriodFrom = report.PeriodFrom.ToLocalTime();
			this.PeriodTo = report.PeriodTo.ToLocalTime();
			this.Requestor = report.Requestor;
			this.IsReconciled = report.IsReconciled;
			this.IsDirector = user.HasPrivilege(Privilege.AM4);
		}
		#endregion
	}
}