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
		public bool AllowToSaveOverpayments { get; set; }

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
			public bool GrantOpeningExists { get; set; }
			public decimal TotalLine1_NoAgreements { get; set; }
			public decimal TotalLine2_CurrPayReq { get; set; }
			public decimal TotalLine3_PayProcessed { get; set; }
			public decimal TotalLine4_ProjSlippage { get; set; }
			public decimal TotalLine5_SlipToDate { get; set; }
			public decimal TotalLine5_SlipToDatePct { get; set; }
			public decimal TotalScheduleAAmount { get; set; }
			public decimal TotalLine6_Overpayments { get; set; }
			public decimal TotalLine7_CurrUnclmComm { get; set; }
		}
	}
}
