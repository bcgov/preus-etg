using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
	public class CompletionReportDetailsViewModel : CompletionReportBaseViewModel
	{
		#region Properties
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		#endregion

		#region Constructors
		public CompletionReportDetailsViewModel()
		{
		}

		public CompletionReportDetailsViewModel(GrantApplication grantApplication, ICompletionReportService completionReportService) : base(grantApplication, completionReportService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));

			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
		}
		#endregion
	}
}
