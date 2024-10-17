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
	public class CompletionReportDynamicCheckboxViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<CompletionReportServiceCategoryViewModel> ServiceCategories { get; set; }
		#endregion

		#region Constructors
		public CompletionReportDynamicCheckboxViewModel()
		{
		}

		public CompletionReportDynamicCheckboxViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				var eligibleCosts = grantApplication.TrainingCost.EligibleCosts.Where(x =>
																		   x.EligibleExpenseType.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports
																		   && x.EligibleExpenseType.IsActive).ToArray();

				this.ServiceCategories = eligibleCosts.Where(o => o.Breakdowns.Any()).Select(o => new CompletionReportServiceCategoryViewModel(o)).ToArray();
			}
		}
		#endregion
	}
}
