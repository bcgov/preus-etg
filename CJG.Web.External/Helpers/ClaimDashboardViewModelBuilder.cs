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
				OutstandingCommintmentsAmount = array[0];
				ClaimsReceivedAmount = array[1];
				PaymentRequestsAmount = array[2];
				OutstandingCommintmentCount = Convert.ToInt32(array[3]);
				CurrentClaimCount = Convert.ToInt32(array[4]);
				ClaimsAssessedCount = Convert.ToInt32(array[5]);
				CurrentClaimAmount = Convert.ToInt32(array[6]);
				ClaimsAssessedAmount = array[7];
			}

			public decimal ClaimsAssessedAmount { get; set; }
			public decimal OutstandingCommintmentsAmount { get; set; }
			public decimal ClaimsReceivedAmount { get; set; }
			public decimal PaymentRequestsAmount { get; set; }
			public int OutstandingCommintmentCount { get; set; }
			public int CurrentClaimCount { get; set; }
			public decimal CurrentClaimAmount { get; set; }
			public int ClaimsAssessedCount { get; set; }
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

			FiscalYear fiscalYear;
			if (_model.SelectedFiscalYearId.HasValue)
			{
				fiscalYear = _staticDataService.GetFiscalYears().First(x => x.Id == _model.SelectedFiscalYearId.Value);
			}
			else
			{
				fiscalYear = _staticDataService.GetFiscalYears().First(x => x.StartDate <= AppDateTime.UtcNow && x.EndDate >= AppDateTime.UtcNow);
				_model.SelectedFiscalYearId = fiscalYear.Id;
			}

			var allZeroes = !_model.SelectedGrantStreamId.HasValue;

			var stream = _model.SelectedGrantStreamId.HasValue
				? _grantStreamService.GetGrantStreams(_model.SelectedFiscalYearId, _model.SelectedGrantProgramId).First(x => x.Id == _model.SelectedGrantStreamId.Value)
				: null;

			var program = model.SelectedGrantProgramId.HasValue
				? _grantProgramService.GetForFiscalYear(_model.SelectedFiscalYearId).First(x => x.Id == _model.SelectedGrantProgramId.Value)
				: null;

			_model.ClaimType = program?.ProgramConfiguration.ClaimTypeId;
			_model.AllowToSave = _user.HasPrivilege(Privilege.AM3) && fiscalYear != null && program != null && stream != null;

			// These need to be pulled from the ReportRates.
			var reportRate = _reportRateService.Get(_model.SelectedFiscalYearId ?? 0, _model.SelectedGrantProgramId ?? 0, _model.SelectedGrantStreamId ?? 0);
			_model.UnclaimedCancellationRate = reportRate?.AgreementCancellationRate ?? 0;
			_model.UnclaimedSlippageRate = reportRate?.AgreementSlippageRate ?? 0;
			_model.ClaimedSlippageRate = reportRate?.ClaimSlippageRate ?? 0;

			var trainingPeriods = _staticDataService.GetTrainingPeriodsForFiscalYear(fiscalYear.Id);

			var budgetAllocationAmounts = allZeroes
				? new Dictionary<int, decimal>()
				: _grantOpeningService.GetBudgetllocationAmountsInTrainingPeriods(fiscalYear.Id, program?.Id, stream?.Id);

			var grantOpeningFinancialsArray = allZeroes
				? new Dictionary<int, decimal[]>()
				: _grantOpeningService.GetGrantOpeningFinancialsInTrainingPeriods(fiscalYear.Id, program?.Id, stream?.Id);

			_model.DataColumns = new List<ClaimDashboardViewModel.DataColumnViewModel>();

			foreach (var trainingPeriod in trainingPeriods)
			{
				DefaultValueForTrainingPeriod(grantOpeningFinancialsArray, trainingPeriod, new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0 });
				DefaultValueForTrainingPeriod(budgetAllocationAmounts, trainingPeriod);

				var grantOpeningFinanicalsForPeriod = new GrantOpeningFinanicals(grantOpeningFinancialsArray[trainingPeriod.Id]);

				var totalNumberOfAgreementsForPeriod = grantOpeningFinanicalsForPeriod.OutstandingCommintmentCount +
					grantOpeningFinanicalsForPeriod.CurrentClaimCount +
					grantOpeningFinanicalsForPeriod.ClaimsAssessedCount;

				var columnViewModel = new ClaimDashboardViewModel.DataColumnViewModel
				{
					Id = trainingPeriod.Id,
					Name = trainingPeriod.Caption,
					TotalAgreementsCount = totalNumberOfAgreementsForPeriod,
					UnclaimedCommitments =
					{
						Amount = grantOpeningFinanicalsForPeriod.OutstandingCommintmentsAmount,
						Count = grantOpeningFinanicalsForPeriod.OutstandingCommintmentCount
					}
				};

				_model.DataColumns.Add(columnViewModel);

				columnViewModel.UnclaimedCommitments.Percent = totalNumberOfAgreementsForPeriod > 0
					? columnViewModel.UnclaimedCommitments.Count / (decimal)totalNumberOfAgreementsForPeriod
					: 0;


				columnViewModel.ClaimsReceivedAmount.Count = grantOpeningFinanicalsForPeriod.CurrentClaimCount;
				columnViewModel.ClaimsReceivedAmount.Percent = totalNumberOfAgreementsForPeriod > 0
					? columnViewModel.ClaimsReceivedAmount.Count / (decimal)totalNumberOfAgreementsForPeriod
					: 0;
				columnViewModel.ClaimsReceivedAmount.Amount = grantOpeningFinanicalsForPeriod.CurrentClaimAmount;

				columnViewModel.PaymentRequests.Count = grantOpeningFinanicalsForPeriod.ClaimsAssessedCount;
				columnViewModel.PaymentRequests.Percent = totalNumberOfAgreementsForPeriod > 0
					? columnViewModel.PaymentRequests.Count / (decimal)totalNumberOfAgreementsForPeriod
					: 0;
				columnViewModel.PaymentRequests.Amount = grantOpeningFinanicalsForPeriod.ClaimsAssessedAmount;

				columnViewModel.BudgetAllocationAmount = budgetAllocationAmounts.ContainsKey(trainingPeriod.Id)
					? budgetAllocationAmounts[trainingPeriod.Id]
					: 0;

			}

			_model.DataColumns.Add(new ClaimDashboardViewModel.DataColumnViewModel()
			{
				Id = fiscalYear.Id,
				Name = $"Fiscal Year {fiscalYear.Caption}",
			});

			return this;
		}

		private static void DefaultValueForTrainingPeriod<T>(IDictionary<int, T> dict, TrainingPeriod trainingPeriod, T defaultValue = default(T))
		{
			if (!dict.ContainsKey(trainingPeriod.Id))
			{
				dict.Add(trainingPeriod.Id, defaultValue);
			}
		}

		public ClaimDashboardViewModel Calculate(ClaimDashboardViewModel model = null)
		{
			if (model != null)
			{
				_model = model;
			}

			var trainingPeriodColumns = _model.DataColumns.Where(x => !x.Name.Contains("Fiscal")).ToList();

			foreach (var columnViewModel in trainingPeriodColumns)
			{
				columnViewModel.UnclaimedSlipageAmount = -1 * columnViewModel.UnclaimedCommitments.Amount *
										  ((decimal)_model.UnclaimedSlippageRate / 100);
				columnViewModel.UnclaimedCancellationAmount = -1 * columnViewModel.UnclaimedCommitments.Amount *
												((decimal)_model.UnclaimedCancellationRate / 100);

				columnViewModel.ClaimsSlippageAmount = -1 * columnViewModel.ClaimsReceivedAmount.Amount * ((decimal)_model.ClaimedSlippageRate / 100);

				columnViewModel.ProjectionOfPerformanceAmount = columnViewModel.UnclaimedCommitments.Amount +
														columnViewModel.UnclaimedSlipageAmount + columnViewModel.UnclaimedCancellationAmount +
														columnViewModel.ClaimsReceivedAmount.Amount + columnViewModel.ClaimsSlippageAmount +
														columnViewModel.PaymentRequests.Amount;

				columnViewModel.OverUnderBudget.Amount = columnViewModel.ProjectionOfPerformanceAmount - columnViewModel.BudgetAllocationAmount;

				columnViewModel.OverUnderBudget.Percent = columnViewModel.BudgetAllocationAmount != 0 ? columnViewModel.OverUnderBudget.Amount / columnViewModel.BudgetAllocationAmount : 0;
			}

			var fiscalYearColumn = _model.DataColumns.FirstOrDefault(x => x.Name.Contains("Fiscal"));

			CalculateFiscalYearColumnViewModel(fiscalYearColumn, trainingPeriodColumns);

			return _model;
		}

		private static void CalculateFiscalYearColumnViewModel(ClaimDashboardViewModel.DataColumnViewModel targetColumn, IList<ClaimDashboardViewModel.DataColumnViewModel> sourceDataColumns)
		{
			targetColumn.TotalAgreementsCount = 0;
			targetColumn.BudgetAllocationAmount = 0;
			targetColumn.UnclaimedCancellationAmount = 0;
			targetColumn.UnclaimedSlipageAmount = 0;
			targetColumn.ClaimsSlippageAmount = 0;
			targetColumn.ProjectionOfPerformanceAmount = 0;
			targetColumn.UnclaimedCommitments = new ClaimDashboardViewModel.AmountPairViewModel();
			targetColumn.ClaimsReceivedAmount = new ClaimDashboardViewModel.AmountPairViewModel();
			targetColumn.PaymentRequests = new ClaimDashboardViewModel.AmountPairViewModel();
			targetColumn.OverUnderBudget = new ClaimDashboardViewModel.AmountPairViewModel();

			foreach (var sourceDataColumn in sourceDataColumns)
			{
				targetColumn.TotalAgreementsCount += sourceDataColumn.TotalAgreementsCount;
				IncrementClaimStats(targetColumn.UnclaimedCommitments, sourceDataColumn.UnclaimedCommitments);
				targetColumn.BudgetAllocationAmount += sourceDataColumn.BudgetAllocationAmount;
				targetColumn.UnclaimedCancellationAmount += sourceDataColumn.UnclaimedCancellationAmount;
				targetColumn.UnclaimedSlipageAmount += sourceDataColumn.UnclaimedSlipageAmount;
				targetColumn.ClaimsSlippageAmount += sourceDataColumn.ClaimsSlippageAmount;
				targetColumn.ProjectionOfPerformanceAmount += sourceDataColumn.ProjectionOfPerformanceAmount;
				IncrementClaimStats(targetColumn.ClaimsReceivedAmount, sourceDataColumn.ClaimsReceivedAmount);
				IncrementClaimStats(targetColumn.PaymentRequests, sourceDataColumn.PaymentRequests);
				IncrementClaimStats(targetColumn.OverUnderBudget, sourceDataColumn.OverUnderBudget);
			}

			if (targetColumn.TotalAgreementsCount > 0)
			{
				targetColumn.UnclaimedCommitments.Percent = targetColumn.UnclaimedCommitments.Count /
															   (decimal)targetColumn.TotalAgreementsCount;
				targetColumn.ClaimsReceivedAmount.Percent = targetColumn.ClaimsReceivedAmount.Count / (decimal)targetColumn.TotalAgreementsCount;
				targetColumn.PaymentRequests.Percent = targetColumn.PaymentRequests.Count /
														  (decimal)targetColumn.TotalAgreementsCount;
			}
			else
			{
				targetColumn.UnclaimedCommitments.Percent = 0;
				targetColumn.ClaimsReceivedAmount.Percent = 0;
				targetColumn.PaymentRequests.Percent = 0;
			}

			targetColumn.OverUnderBudget.Percent = targetColumn.BudgetAllocationAmount != 0 ? targetColumn.OverUnderBudget.Amount / targetColumn.BudgetAllocationAmount : 0;
		}

		private static void IncrementClaimStats(ClaimDashboardViewModel.AmountPairViewModel targetValue,
			ClaimDashboardViewModel.AmountPairViewModel sourceValue)
		{
			targetValue.Amount += sourceValue.Amount;
			targetValue.Count += sourceValue.Count;
		}
	}
}