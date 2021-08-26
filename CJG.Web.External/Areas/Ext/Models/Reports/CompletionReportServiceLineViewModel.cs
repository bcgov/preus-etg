using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
	public class CompletionReportServiceLineViewModel : BaseViewModel
	{
		#region Properties
		public int EligibleCostBreakdownId { get; set; }
		public string Caption { get; set; }
		#endregion

		#region Constructors
		public CompletionReportServiceLineViewModel()
		{
		}

		public CompletionReportServiceLineViewModel(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (eligibleCostBreakdown == null) throw new ArgumentNullException(nameof(eligibleCostBreakdown));

			Utilities.MapProperties(eligibleCostBreakdown.EligibleExpenseBreakdown.ServiceLine, this);
			this.EligibleCostBreakdownId = eligibleCostBreakdown.Id;
		}
		#endregion
	}
}
