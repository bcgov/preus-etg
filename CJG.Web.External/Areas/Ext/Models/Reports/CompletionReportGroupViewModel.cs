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
	public class CompletionReportGroupViewModel : BaseViewModel
	{
		#region Properties
		public string Title { get; set; }
		public int GrantApplicationId { get; set; }
		public bool SaveOnly { get; set; } = false;

		public IEnumerable<CompletionReportQuestionViewModel> Questions { get; set; }
		#endregion

		#region Constructors
		public CompletionReportGroupViewModel()
		{
		}

		public CompletionReportGroupViewModel(GrantApplication grantApplication,
			CompletionReportGroup completionReportGroup,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportGroup == null) throw new ArgumentNullException(nameof(completionReportGroup));

			Utilities.MapProperties(completionReportGroup, this);
			this.GrantApplicationId = grantApplication.Id;
			this.Questions = completionReportGroup.Questions.Select(o =>
			new CompletionReportQuestionViewModel(grantApplication, o, naIndustryClassificationSystemService, nationalOccupationalClassificationService)).ToArray();
		}
		#endregion
	}
}
