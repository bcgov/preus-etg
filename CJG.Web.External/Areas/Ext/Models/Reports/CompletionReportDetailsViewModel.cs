using System;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared.Reports;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
    public class CompletionReportDetailsViewModel : CompletionReportBaseViewModel
	{
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }

		public CompletionReportDetailsViewModel()
		{
		}

		public CompletionReportDetailsViewModel(GrantApplication grantApplication, ICompletionReportService completionReportService) : base(grantApplication, completionReportService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));

			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
		}
	}
}
