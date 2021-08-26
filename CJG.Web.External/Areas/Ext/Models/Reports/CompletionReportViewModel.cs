using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
	public class CompletionReportViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string ProgramName { get; set; }
		public int[] CompletionReportGroupIds { get; set; }
		public ICollection<ParticipantFormViewModel> Participants { get; set; }
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		public IEnumerable<CompletionReportGroupViewModel> CompletionReportGroups { get; set; } = new List<CompletionReportGroupViewModel>();
		#endregion

		#region Constructors
		public CompletionReportViewModel()
		{
		}

		public CompletionReportViewModel(GrantApplication grantApplication, ICompletionReportService completionReportService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));

			this.GrantApplicationId = grantApplication.Id;
			this.ProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.CompletionReportGroupIds = completionReportService.GetCompletionReportGroups(grantApplication.Id, o => o.Id).ToArray();
			this.Participants = grantApplication.ParticipantForms.OrderBy(o => o.LastName).Select(o => new ParticipantFormViewModel(o)).ToArray();
			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
		}
		#endregion
	}
}
