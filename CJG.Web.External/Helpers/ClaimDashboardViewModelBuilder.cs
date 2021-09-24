using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.ClaimDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Helpers
{
	public class ClaimDashboardViewModelBuilder
	{
		public class GrantOpeningFinanicals
		{
			public GrantOpeningFinanicals(IReadOnlyList<decimal> array)
			{
				TotalLine1_NoAgreements = array[0];
				TotalLine2_CurrPayReq = array[1];
				TotalLine3_PayProcessed = array[2];
				TotalLine4_ProjSlippage = array[3];
				TotalScheduleAAmount = array[4];
				TotalLine5_SlipToDatePct = 0;
				TotalLine5_SlipToDate = array[5];
				TotalLine6_Overpayments = array[6];
				TotalLine7_CurrUnclmComm = array[7];
			}

			public int Id { get; set; }
			public string Name { get; set; }
			public decimal TotalLine1_NoAgreements { get; set; }
			public decimal TotalLine2_CurrPayReq { get; set; }
			public decimal TotalLine3_PayProcessed { get; set; }
			public decimal TotalLine4_ProjSlippage { get; set; }
			public decimal TotalLine5_SlipToDatePct { get; set; }
			public decimal TotalLine5_SlipToDate { get; set; }
			public decimal TotalLine6_Overpayments { get; set; }
			public decimal TotalLine7_CurrUnclmComm { get; set; }
			public decimal TotalScheduleAAmount { get; set; }
		}

		private readonly IStaticDataService _staticDataService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IReportRateService _reportRateService;
		private readonly IPrincipal _user;
		private ClaimDashboardViewModel _model;

		public ClaimDashboardViewModelBuilder(IStaticDataService staticDataService, IGrantOpeningService grantOpeningService, IGrantProgramService grantProgramService, IGrantStreamService grantStreamService, IReportRateService reportRateService, IPrincipal user)
		{
			_staticDataService = staticDataService;
			_grantOpeningService = grantOpeningService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_reportRateService = reportRateService;
			_user = user;
		}

		public ClaimDashboardViewModelBuilder Build(ClaimDashboardViewModel model)
		{
			_model = model;
			_model.DataColumns = new List<ClaimDashboardViewModel.DataColumnViewModel>();

			FiscalYear fiscalYear;
			if (_model.SelectedFiscalYearId.HasValue)
			{
				// TODO: Should probably use FiscalYearService.Get(_model.SelectedFiscalYearId.Value) (Will)
				fiscalYear = _staticDataService.GetFiscalYears().First(x => x.Id == _model.SelectedFiscalYearId.Value);
			}
			else
			{
				// If there is no fiscal year, this code will return only the current FY so that the dropdown
				// on the page has a default.
				// TODO: Should probably use FiscalYearService.GetFiscalYear(AppDateTime.UtcNow) (Will)
				fiscalYear = _staticDataService.GetFiscalYears().First(x => x.StartDate <= AppDateTime.UtcNow && x.EndDate >= AppDateTime.UtcNow);
				_model.SelectedFiscalYearId = fiscalYear.Id;
				return this;
			}

			var allZeroes = !_model.SelectedGrantStreamId.HasValue;

			var stream = _model.SelectedGrantStreamId.HasValue
				? _grantStreamService.GetGrantStreams(_model.SelectedFiscalYearId, _model.SelectedGrantProgramId).First(x => x.Id == _model.SelectedGrantStreamId.Value)
				: null;

			var program = model.SelectedGrantProgramId.HasValue
				? _grantProgramService.GetForFiscalYear(_model.SelectedFiscalYearId).First(x => x.Id == _model.SelectedGrantProgramId.Value)
				: null;

			// If there is no program or stream, the user have clicked the Display button without selecting those values.
			if (program == null || stream == null)
				return this;

			_model.ClaimType = program?.ProgramConfiguration.ClaimTypeId;
			// Save Rates (%) is for the director role (and includes Sysadmin as well).
			_model.AllowToSave = _user.HasPrivilege(Privilege.AM3) && fiscalYear != null && program != null && stream != null;
			// Save Overpayments is for the director role (and includes Sysadmin as well) and the Financial Clerk (the priv
			// test will allow Assessor and System Admin to access this as well).
			_model.AllowToSaveOverpayments = (_user.HasPrivilege(Privilege.AM3) || _user.HasPrivilege(Privilege.AM5)) && fiscalYear != null && program != null && stream != null;

			// These need to be pulled from the ReportRates.
			var reportRate = _reportRateService.Get(_model.SelectedFiscalYearId ?? 0, _model.SelectedGrantProgramId ?? 0, _model.SelectedGrantStreamId ?? 0);
			_model.UnclaimedCancellationRate = reportRate?.AgreementCancellationRate ?? 0;
			_model.UnclaimedSlippageRate = reportRate?.AgreementSlippageRate ?? 0;
			_model.ClaimedSlippageRate = reportRate?.ClaimSlippageRate ?? 0;

			var trainingPeriods = _staticDataService.GetTrainingPeriodsForFiscalYear(fiscalYear.Id, program.Id, stream.Id);

			var budgetAllocationAmounts = allZeroes
				? new Dictionary<int, decimal>()
				: _grantOpeningService.GetBudgetAllocationAmountsInTrainingPeriods(fiscalYear.Id, program.Id, stream.Id);

			var grantOpeningFinancialsArray = _grantOpeningService.GetGrantOpeningFinancialsInTrainingPeriods(fiscalYear.Id, program.Id, stream.Id,
					(decimal)_model.UnclaimedSlippageRate);

			_model.DataColumns.Add(new ClaimDashboardViewModel.DataColumnViewModel()
			{
				Id = 0,
				Name = $"Fiscal Year {fiscalYear.Caption}",
			});

			foreach (var trainingPeriod in trainingPeriods)
			{
				var grantOpeningExists = grantOpeningFinancialsArray.ContainsKey(trainingPeriod.Id);
				DefaultValueForTrainingPeriod(grantOpeningFinancialsArray, trainingPeriod, new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0 });
				DefaultValueForTrainingPeriod(budgetAllocationAmounts, trainingPeriod);

				var grantOpeningFinanicalsForPeriod = new GrantOpeningFinanicals(grantOpeningFinancialsArray[trainingPeriod.Id]);
				var columnViewModel = new ClaimDashboardViewModel.DataColumnViewModel
				{
					Id = trainingPeriod.Id,
					Name = trainingPeriod.Caption,
					GrantOpeningExists = grantOpeningExists,
					TotalLine1_NoAgreements = grantOpeningFinanicalsForPeriod.TotalLine1_NoAgreements,
					TotalLine2_CurrPayReq = grantOpeningFinanicalsForPeriod.TotalLine2_CurrPayReq,
					TotalLine3_PayProcessed = grantOpeningFinanicalsForPeriod.TotalLine3_PayProcessed,
					TotalLine4_ProjSlippage = grantOpeningFinanicalsForPeriod.TotalLine4_ProjSlippage,
					TotalLine5_SlipToDate = grantOpeningFinanicalsForPeriod.TotalLine5_SlipToDate,
					TotalLine5_SlipToDatePct = grantOpeningFinanicalsForPeriod.TotalLine5_SlipToDatePct,
					TotalScheduleAAmount = grantOpeningFinanicalsForPeriod.TotalScheduleAAmount,
					TotalLine6_Overpayments = grantOpeningFinanicalsForPeriod.TotalLine6_Overpayments,
					TotalLine7_CurrUnclmComm = grantOpeningFinanicalsForPeriod.TotalLine7_CurrUnclmComm
				};
				_model.DataColumns.Add(columnViewModel);
			}
			return this;
		}

		private static void DefaultValueForTrainingPeriod<T>(IDictionary<int, T> dict, TrainingPeriod trainingPeriod, T defaultValue = default(T))
		{
			if (dict.ContainsKey(trainingPeriod.Id))
				return;

			dict.Add(trainingPeriod.Id, defaultValue);
		}

		public ClaimDashboardViewModel Calculate(ClaimDashboardViewModel model = null)
		{
			if (model != null)
			{
				_model = model;
			}

			var trainingPeriodColumns = _model.DataColumns.Where(x => !x.Name.Contains("Fiscal")).ToList();
			if (trainingPeriodColumns.Count == 0)   // If no fiscal year has been selected yet
				return _model;

			foreach (var columnViewModel in trainingPeriodColumns)
			{
				columnViewModel.TotalLine5_SlipToDatePct = columnViewModel.TotalScheduleAAmount == 0 ? 0 :
					columnViewModel.TotalLine5_SlipToDate / columnViewModel.TotalScheduleAAmount;
			}

			var fiscalYearColumn = _model.DataColumns.FirstOrDefault(x => x.Name.Contains("Fiscal"));
			CalculateFiscalYearColumnViewModel(fiscalYearColumn, trainingPeriodColumns);
			return _model;
		}

		private static void CalculateFiscalYearColumnViewModel(ClaimDashboardViewModel.DataColumnViewModel targetColumn, IList<ClaimDashboardViewModel.DataColumnViewModel> sourceDataColumns)
		{
			targetColumn.TotalLine1_NoAgreements = 0;
			targetColumn.TotalLine2_CurrPayReq = 0;
			targetColumn.TotalLine3_PayProcessed = 0;
			targetColumn.TotalLine4_ProjSlippage = 0;
			targetColumn.TotalLine5_SlipToDatePct = 0;
			targetColumn.TotalScheduleAAmount = 0;
			targetColumn.TotalLine5_SlipToDate = 0;
			targetColumn.TotalLine6_Overpayments = 0;
			targetColumn.TotalLine7_CurrUnclmComm = 0;

			foreach (var sourceDataColumn in sourceDataColumns)
			{
				targetColumn.TotalLine1_NoAgreements += sourceDataColumn.TotalLine1_NoAgreements;
				targetColumn.TotalLine2_CurrPayReq += sourceDataColumn.TotalLine2_CurrPayReq;
				targetColumn.TotalLine3_PayProcessed += sourceDataColumn.TotalLine3_PayProcessed;
				targetColumn.TotalLine4_ProjSlippage += sourceDataColumn.TotalLine4_ProjSlippage;
				targetColumn.TotalScheduleAAmount += sourceDataColumn.TotalScheduleAAmount;
				targetColumn.TotalLine5_SlipToDate += sourceDataColumn.TotalLine5_SlipToDate;
				targetColumn.TotalLine6_Overpayments += sourceDataColumn.TotalLine6_Overpayments;
				targetColumn.TotalLine7_CurrUnclmComm += sourceDataColumn.TotalLine7_CurrUnclmComm;
			}

			// Percentage for the target column after all the additions are complete.
			targetColumn.TotalLine5_SlipToDatePct = targetColumn.TotalScheduleAAmount == 0 ? 0 :
				targetColumn.TotalLine5_SlipToDate / targetColumn.TotalScheduleAAmount;
		}

		private static void IncrementClaimStats(ClaimDashboardViewModel.AmountPairViewModel targetValue, ClaimDashboardViewModel.AmountPairViewModel sourceValue)
		{
			targetValue.Amount += sourceValue.Amount;
			targetValue.Count += sourceValue.Count;
		}
	}
}