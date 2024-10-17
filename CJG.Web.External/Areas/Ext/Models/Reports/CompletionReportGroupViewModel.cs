using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
    public class CompletionReportGroupViewModel : BaseViewModel
	{
		public string Title { get; set; }
		public int GrantApplicationId { get; set; }
		public bool SaveOnly { get; set; } = false;

		public IEnumerable<CompletionReportQuestionViewModel> Questions { get; set; }

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
			GrantApplicationId = grantApplication.Id;

			Questions = completionReportGroup.Questions
				.Where(x => x.CompletionReportId == grantApplication.CompletionReportId)
				.Where(x => x.IsActive)
				.OrderBy(x => x.Sequence)	// Because we are adding a CWRG report, we need to start using Sequence
				.ThenBy(x => x.Id)			// The original ETG report did not apply sequence everywhere so order by Id as well.
				.Select(o => new CompletionReportQuestionViewModel(grantApplication, o, naIndustryClassificationSystemService, nationalOccupationalClassificationService))
				.ToArray();
		}
	}
}
