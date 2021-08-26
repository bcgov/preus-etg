using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.BatchApprovals
{
	public class BatchApprovalFilterViewModel : BaseViewModel
	{
		#region Properties
		public int? AssessorId { get; set; }

		public int? FiscalYearId { get; set; }

		public int? GrantProgramId { get; set; }

		public int? GrantStreamId { get; set; }

		public int? TrainingPeriodId { get; set; }

		public string[] OrderBy { get; set; }
		#endregion

		#region Constructors
		public BatchApprovalFilterViewModel() { }
		#endregion

		#region Methods
		public ApplicationFilter GetFilter()
		{
			var states = new[] { new StateFilter<ApplicationStateInternal>(ApplicationStateInternal.RecommendedForApproval) };
			return new ApplicationFilter(states, this.AssessorId, this.FiscalYearId, this.TrainingPeriodId, this.GrantProgramId, this.GrantStreamId, this.OrderBy);
		}
		#endregion
	}
}