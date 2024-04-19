using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared.Reports
{
    public class CompletionReportGroupDetailsViewModel : BaseViewModel
	{
		public string Title { get; set; }

		public IEnumerable<CompletionReportQuestionDetailsViewModel> Questions { get; set; }

		public CompletionReportGroupDetailsViewModel()
		{
		}

		public CompletionReportGroupDetailsViewModel(GrantApplication grantApplication, CompletionReportGroup completionReportGroup, IEnumerable<ParticipantForm> participants)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (completionReportGroup == null)
				throw new ArgumentNullException(nameof(completionReportGroup));

			Utilities.MapProperties(completionReportGroup, this);
			Questions = completionReportGroup.Questions
				.Where(x => x.CompletionReportId == grantApplication.CompletionReportId)
				.OrderBy(x => !x.IsActive)
				.ThenBy(x => x.Sequence)   // Because we are adding a CWRG report, we need to start using Sequence
				.ThenBy(x => x.Id)          // The original ETG report did not apply sequence everywhere so order by Id as well.
				.Select(o => new CompletionReportQuestionDetailsViewModel(grantApplication, o, participants))
				.ToArray();
		}
	}
}
