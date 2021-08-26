using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.ClaimDashboard
{
	public class ClaimDashboardViewModel : BaseViewModel
	{
		public ClaimTypes? ClaimType { get; set; }
		public IList<DataColumnViewModel> DataColumns { get; set; } = new List<DataColumnViewModel>();
		public double UnclaimedSlippageRate { get; set; }
		public double UnclaimedCancellationRate { get; set; }
		public double ClaimedSlippageRate { get; set; }
		public int? SelectedFiscalYearId { get; set; }
		public int? SelectedGrantProgramId { get; set; }
		public int? SelectedGrantStreamId { get; set; }
		public bool AllowToSave { get; set; }

		public class AmountPairViewModel
		{
			public decimal Percent { get; set; }
			public decimal Amount { get; set; }
			public int Count { get; set; }
		}

		public class DataColumnViewModel
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public int TotalAgreementsCount { get; set; }
			public decimal UnclaimedSlipageAmount { get; set; }
			public decimal UnclaimedCancellationAmount { get; set; }
			public AmountPairViewModel UnclaimedCommitments { get; set; } = new AmountPairViewModel();
			public AmountPairViewModel ClaimsReceivedAmount { get; set; } = new AmountPairViewModel();
			public decimal ClaimsSlippageAmount { get; set; }
			public AmountPairViewModel PaymentRequests { get; set; } = new AmountPairViewModel();
			public decimal ProjectionOfPerformanceAmount { get; set; }
			public decimal BudgetAllocationAmount { get; set; }
			public AmountPairViewModel OverUnderBudget { get; set; } = new AmountPairViewModel();
		}
	}
}
