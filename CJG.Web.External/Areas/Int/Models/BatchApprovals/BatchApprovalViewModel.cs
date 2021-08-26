using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.BatchApprovals
{
	public class BatchApprovalViewModel : BaseViewModel
	{
		#region Properties
		public bool SelectAll { get; set; }
		public int? AssessorId { get; set; }

		public int? FiscalYearId { get; set; }

		public int? GrantProgramId { get; set; }

		public int? GrantStreamId { get; set; }

		public int? TrainingPeriodId { get; set; }

		public int Total { get; set; }

		public IEnumerable<IssueOfferViewModel> GrantApplications { get; set; } = new List<IssueOfferViewModel>();
		#endregion

		#region Constructors
		public BatchApprovalViewModel() { }
		#endregion

		#region Methods
		public ApplicationFilter GetFilter()
		{
			var states = new[] { new StateFilter<ApplicationStateInternal>(ApplicationStateInternal.RecommendedForApproval) };
			return new ApplicationFilter(states, this.AssessorId, this.FiscalYearId, this.TrainingPeriodId, this.GrantProgramId, this.GrantStreamId);
		}
		#endregion
	}
}