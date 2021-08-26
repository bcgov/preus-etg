using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Models.Shared.Reports
{
	public class CompletionReportServiceCategoryViewModel : BaseViewModel
	{
		#region Properties
		public string Caption { get; set; }
		public IEnumerable<CompletionReportServiceLineViewModel> ServiceLines { get; set; }
		#endregion

		#region Constructors
		public CompletionReportServiceCategoryViewModel()
		{
		}

		public CompletionReportServiceCategoryViewModel(EligibleCost eligibleCost)
		{
			if (eligibleCost == null) throw new ArgumentNullException(nameof(eligibleCost));

			Utilities.MapProperties(eligibleCost.EligibleExpenseType.ServiceCategory, this);
			this.ServiceLines = eligibleCost.Breakdowns.Select(o => new CompletionReportServiceLineViewModel(o)).ToArray();
		}
		#endregion
	}
}
