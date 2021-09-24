using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.BatchApprovals
{
	public class BatchApprovalFilterViewModel : BaseViewModel
	{
		public int? AssessorId { get; set; }
		public int? FiscalYearId { get; set; }
		public int? GrantProgramId { get; set; }
		public int? GrantStreamId { get; set; }
		public string TrainingPeriodCaption { get; set; }
		public string[] OrderBy { get; set; }

		public ApplicationFilter GetFilter()
		{
			var states = new[] { new StateFilter<ApplicationStateInternal>(ApplicationStateInternal.RecommendedForApproval) };
			return new ApplicationFilter(states, AssessorId, FiscalYearId, TrainingPeriodCaption, GrantProgramId, GrantStreamId, OrderBy);
		}
	}
}