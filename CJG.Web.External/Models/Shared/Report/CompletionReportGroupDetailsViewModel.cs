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
	public class CompletionReportGroupDetailsViewModel : BaseViewModel
	{
		#region Properties
		public string Title { get; set; }

		public IEnumerable<CompletionReportQuestionDetailsViewModel> Questions { get; set; }
		#endregion

		#region Constructors
		public CompletionReportGroupDetailsViewModel()
		{
		}

		public CompletionReportGroupDetailsViewModel(GrantApplication grantApplication,
			CompletionReportGroup completionReportGroup,
			IEnumerable<ParticipantForm> participants)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportGroup == null) throw new ArgumentNullException(nameof(completionReportGroup));

			Utilities.MapProperties(completionReportGroup, this);
			this.Questions = completionReportGroup.Questions.Select(o =>
			new CompletionReportQuestionDetailsViewModel(grantApplication, o, participants)).ToArray();
		}
		#endregion
	}
}
