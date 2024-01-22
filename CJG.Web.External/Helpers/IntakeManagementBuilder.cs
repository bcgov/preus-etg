using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// IntakeManagementBuilder class, provides methods to initialize GrantApplication Intake.
    /// </summary>
    public class IntakeManagementBuilder
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly ITrainingPeriodService _trainingPeriodService;
		private readonly IFinanceInformationService _financeInformationService;

		public IntakeManagementBuilder(IStaticDataService staticDataService, IGrantProgramService grantProgramService, IGrantStreamService grantStreamService, IGrantOpeningService grantOpeningService, ITrainingPeriodService trainingPeriodService, IFinanceInformationService financeInformationService)
		{
			_staticDataService = staticDataService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_grantOpeningService = grantOpeningService;
			_trainingPeriodService = trainingPeriodService;
			_financeInformationService = financeInformationService;
		}

		public IntakeManagementViewModel Build(int? fiscalYearId, int? grantStreamId, int? budgetTypeId)
		{
			var fiscalYears = _staticDataService
				.GetFiscalYears()
				.Select(fy => new KeyValuePair<int, string>(fy.Id, fy.Caption))
				.ToList();

			if (!fiscalYearId.HasValue)
				fiscalYearId = _staticDataService.GetFiscalYear(0).Id;

			var grantProgramId = _grantProgramService.GetDefaultGrantProgramId();

			var grantPrograms = GetAllGrantPrograms().ToList();
			var grantStreams = GetActiveGrantStreams(grantProgramId).ToList();
			var budgetTypes = GetBudgetTypes();
			grantStreamId = GetDefaultGrantStreamId(grantStreams, grantStreamId);

			var oldGrowthKnockoutChecked = budgetTypeId == (int)TrainingPeriodBudgetType.OldGrowthBudget;

			var trainingPeriods = _trainingPeriodService.GetAllFor(fiscalYearId.Value, grantProgramId, grantStreamId.Value);
			var periods = trainingPeriods.Select(x => LoadPeriod(x, grantStreamId.Value, oldGrowthKnockoutChecked)).ToList();

			return new IntakeManagementViewModel
			{
				TrainingPeriods = periods,
				FiscalYearId = fiscalYearId,
				FiscalYears = fiscalYears,
				GrantPrograms = grantPrograms,
				GrantStreams = grantStreams,
				GrantProgramId = grantProgramId,
				GrantStreamId = grantStreamId,
				BudgetTypes = budgetTypes,
				BudgetTypeId = budgetTypeId,
			};
		}

		private IEnumerable<KeyValuePair<int, string>> GetAllGrantPrograms()
		{
			return _grantProgramService.GetAll()
				.Select(x => new KeyValuePair<int, string>(x.Id, x.Name))
				.OrderBy(x => x.Value);
		}

		private IEnumerable<KeyValuePair<int, string>> GetActiveGrantStreams(int grantProgramId)
		{
			return _grantStreamService.GetAll()
				.Where(x => x.GrantProgramId == grantProgramId && x.IsActive)
				.Select(x => new KeyValuePair<int, string>(x.Id, x.Name))
				.OrderBy(x => x.Value);
		}

		private IEnumerable<KeyValuePair<int, string>> GetBudgetTypes()
		{
			return new List<KeyValuePair<int, string>>
			{
				new KeyValuePair<int, string>((int)TrainingPeriodBudgetType.BaseBudget, TrainingPeriodBudgetType.BaseBudget.GetDescription()),
				new KeyValuePair<int, string>((int)TrainingPeriodBudgetType.OldGrowthBudget, TrainingPeriodBudgetType.OldGrowthBudget.GetDescription())
			};
		}

		private IntakeManagementViewModel.TrainingPeriodViewModel LoadPeriod(TrainingPeriod trainingPeriod, int grantStreamId, bool filterOldGrowthApplication)
		{
			var grantOpening = _grantOpeningService.GetGrantOpeningWithApplications(grantStreamId, trainingPeriod.Id);
			var grantApplicationsInOpening = grantOpening?.GrantApplications ?? new List<GrantApplication>();

			var oldGrowthQuestionId = _grantStreamService.GetOldGrowthGrantStreamQuestion()?.Id ?? 0;
			var grantApplications = new List<GrantApplication>();

            if (filterOldGrowthApplication)
            {
                grantApplications = grantApplicationsInOpening
                    .Where(g => g.GrantStreamEligibilityAnswers.Any(a => a.GrantStreamEligibilityQuestionId == oldGrowthQuestionId && a.EligibilityAnswer))
                    .ToList();
            }

            if (!filterOldGrowthApplication)
            {
                grantApplications = grantApplicationsInOpening
                    .Where(g => !g.GrantStreamEligibilityAnswers.Any(a => a.GrantStreamEligibilityQuestionId == oldGrowthQuestionId && a.EligibilityAnswer))
                    .ToList();
            }

			var grantIntakes = LoadGrantOpeningIntakes(grantOpening, grantApplications);
			var intakeTotalApplications = grantIntakes.Values.Sum(a => a.Number);
			var intakeTotalAmount = grantIntakes.Values.Sum(a => a.Value);
			var budgetType = filterOldGrowthApplication ? TrainingPeriodBudgetType.OldGrowthBudget : TrainingPeriodBudgetType.BaseBudget;
			var trainingPeriodBudget = _trainingPeriodService.GetBudget(trainingPeriod, budgetType);
			var trainingPeriodViewModel = new IntakeManagementViewModel.TrainingPeriodViewModel
			{
				Id = trainingPeriod.Id,
				FiscalYearName = trainingPeriod.FiscalYear.Caption,
				TrainingPeriodName = trainingPeriod.Caption,
				StartDate = trainingPeriod.StartDate,
				EndDate = trainingPeriod.EndDate,
				Status = grantOpening?.State.ToString(),
				GrantOpeningIntakes = grantIntakes,
				TotalApplicationsIntake = intakeTotalApplications,
				TotalApplicationsIntakeAmt = intakeTotalAmount,
				RefusalRate = trainingPeriodBudget.RefusalRate * 100,
				WithdrawnRate = trainingPeriodBudget.WithdrawnRate * 100,
				SlippageApprovedAmount = trainingPeriodBudget.ApprovedSlippageRate * 100,
				SlippageClaimedAmount = trainingPeriodBudget.ClaimedSlippageRate * 100
			};

			return trainingPeriodViewModel;
		}

		private Dictionary<int, IntakeManagementViewModel.GrantOpeningIntakeViewModel> LoadGrantOpeningIntakes(GrantOpening grantOpening, List<GrantApplication> grantApplications)
		{
			var intakeModels = new Dictionary<int, IntakeManagementViewModel.GrantOpeningIntakeViewModel>
			{
				{
					1, GetIntakeModelFor(grantApplications, "New", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.New })
				},
				{
					2, GetIntakeModelFor(grantApplications, "Pending Assessment", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.PendingAssessment })
				},
				{
					3, GetIntakeModelFor(grantApplications, "Under Assessment", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.UnderAssessment, ApplicationStateInternal.ReturnedToAssessment })
				},
				{
					4, GetIntakeModelFor(grantApplications, "Denied", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.ApplicationDenied })
				},
				{
					5, GetIntakeModelFor(grantApplications, "Withdrawn", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.ApplicationWithdrawn })
				},
				{
					6, GetIntakeModelFor(grantApplications, "Cancelled by Applicant", CommitmentType.Agreed, new List<ApplicationStateInternal> { ApplicationStateInternal.CancelledByAgreementHolder })
				},
				{
					7, GetIntakeModelFor(grantApplications, "Cancelled by Ministry", CommitmentType.Agreed, new List<ApplicationStateInternal> { ApplicationStateInternal.CancelledByMinistry })
				},
				{
					8, GetIntakeModelFor(grantApplications, "Returned Unassessed", CommitmentType.Requested, new List<ApplicationStateInternal> { ApplicationStateInternal.ReturnedUnassessed })
				},
				{
					9, GetIntakeModelForYTD(grantApplications, "YTD Paid")
				},
				{
					10, GetIntakeModelForSubmittedClaims(grantApplications, "Claim Received")
				},
				{
					11, GetIntakeModelFor(grantApplications, "Claim Not Yet Received", CommitmentType.Agreed, new List<ApplicationStateInternal> { ApplicationStateInternal.AgreementAccepted })
				},
				{
					12, GetIntakeModelFor(grantApplications, "Commitments", CommitmentType.Agreed, new List<ApplicationStateInternal>
					{
						ApplicationStateInternal.AgreementAccepted,
						ApplicationStateInternal.NewClaim,
						ApplicationStateInternal.ClaimAssessEligibility,
						ApplicationStateInternal.ClaimAssessReimbursement,
						ApplicationStateInternal.ClaimApproved,
						ApplicationStateInternal.ClaimReturnedToApplicant,
						ApplicationStateInternal.CompletionReporting,
						ApplicationStateInternal.Closed
					})
				}
			};

			var newApplications = intakeModels[1];
			var pendingApplications = intakeModels[2];
			var underApplications = intakeModels[3];
			var committed = intakeModels[12];

			//Forecasted Expenditure would be the amount that we have already committed, plus the value of all the work awaiting assessment / under assessment,
			var forecastValue = newApplications.Value + pendingApplications.Value + underApplications.Value + committed.Value;

			var budget = grantOpening?.IntakeTargetAmt ?? 0;


			intakeModels.Add(25, GetIntakeModelInline("Forecasted Expenditure *", 0, forecastValue));
			// should be based off of forecasted expenditure vs budget
			var overUnderDollars = forecastValue - budget;
			var overUnderPercent = budget > 0 ? forecastValue / budget : 0;
			intakeModels.Add(30, GetIntakeModelInline("Over/Under $", 0, overUnderDollars));
			intakeModels.Add(31, GetIntakeModelInline("Over/Under %", 0, overUnderPercent, false));

			return intakeModels
				.OrderBy(i => i.Key)
				.ToDictionary(i => i.Key, i => i.Value);
		}

		private static IntakeManagementViewModel.GrantOpeningIntakeViewModel GetIntakeModelFor(IEnumerable<GrantApplication> grantApplications, string intakeGroupName, CommitmentType commitmentType, IEnumerable<ApplicationStateInternal> internalStates)
		{
			var applicationsWithStatus = grantApplications
				.Where(g => internalStates.Contains(g.ApplicationStateInternal))
				.ToList();

			var count = applicationsWithStatus.Count;
			var total = applicationsWithStatus.Sum(ga => commitmentType == CommitmentType.Requested ? ga.GetEstimatedReimbursement() : ga.GetAgreedCommitment());

			return new IntakeManagementViewModel.GrantOpeningIntakeViewModel(intakeGroupName, count, total);
		}

		private IntakeManagementViewModel.GrantOpeningIntakeViewModel GetIntakeModelForYTD(IEnumerable<GrantApplication> grantApplications, string intakeGroupName, bool valueIsMoney = true)
		{
			//var applicationsWithStatus = grantApplications
			//	.Where(g => internalStates.Contains(g.ApplicationStateInternal))
			//	.ToList();

			var ytdInfo = _financeInformationService.GetYearToDatePaidFor(grantApplications);

			var count = 0;
			var total = ytdInfo.TotalPaid;

			return new IntakeManagementViewModel.GrantOpeningIntakeViewModel(intakeGroupName, count, total, valueIsMoney);
		}

		private IntakeManagementViewModel.GrantOpeningIntakeViewModel GetIntakeModelInline(string intakeGroupName, int count, decimal total, bool valueIsMoney = true)
		{
			return new IntakeManagementViewModel.GrantOpeningIntakeViewModel(intakeGroupName, count, total, valueIsMoney);
		}

		private static IntakeManagementViewModel.GrantOpeningIntakeViewModel GetIntakeModelForSubmittedClaims(IEnumerable<GrantApplication> grantApplications, string intakeGroupName)
		{
			var applicationClaimStates = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.AgreementAccepted,
				ApplicationStateInternal.NewClaim,
				ApplicationStateInternal.ClaimAssessEligibility,
				ApplicationStateInternal.ClaimAssessReimbursement,
				ApplicationStateInternal.ClaimApproved,
				ApplicationStateInternal.ClaimReturnedToApplicant,
				ApplicationStateInternal.CompletionReporting,
				ApplicationStateInternal.Closed
			};

            var claimsForApplications = grantApplications
                .Where(g => applicationClaimStates.Contains(g.ApplicationStateInternal))
                .SelectMany(g => g.Claims)
                .Where(c => c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.ClaimPaid, ClaimState.AmountReceived))
                .ToList();

            var singleAmendable = claimsForApplications
	            .Where(c => c.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
	            .ToList();

            var notSingleAmendable = claimsForApplications
	            .Where(c => c.ClaimTypeId != ClaimTypes.SingleAmendableClaim)
	            .ToList();

            // Sum the two claim types, SingleAmendableClaim and the rest.
            decimal totalPaid = !singleAmendable.Any() ? 0 : singleAmendable.Sum(q => q.TotalAssessedReimbursement
                                                                                      - (q.GrantApplication.PaymentRequests.Where(o => o.ClaimVersion != q.ClaimVersion).Sum(o => o.PaymentAmount)));

            totalPaid += !notSingleAmendable.Any() ? 0 : notSingleAmendable.Sum(q => q.TotalAssessedReimbursement);

			var count = claimsForApplications.Count;
			var total = totalPaid;

			return new IntakeManagementViewModel.GrantOpeningIntakeViewModel(intakeGroupName, count, total);
		}

		public int GetDefaultGrantStreamId(IEnumerable<KeyValuePair<int, string>> grantStreams, int? grantStreamId)
		{
			KeyValuePair<int, string> result;
			return (result = grantStreams.FirstOrDefault(o => o.Key == grantStreamId)).Equals(default(KeyValuePair<int, string>)) &&
				   (result = grantStreams.FirstOrDefault()).Equals(default(KeyValuePair<int, string>)) ?
				   0 : result.Key;
		}

		public void SaveRates(IntakeManagementViewModel model)
		{
			if (!model.TrainingPeriods.Any())
				return;

			var budgetType = TrainingPeriodBudgetType.BaseBudget;
			if (model.BudgetTypeId != null)
				budgetType = (TrainingPeriodBudgetType)model.BudgetTypeId;

			foreach (var trainingPeriod in model.TrainingPeriods)
			{
				var budgetModel = new TrainingBudgetModel
				{
					TrainingPeriodId = trainingPeriod.Id,
					BudgetType = budgetType,
					RefusalRate = trainingPeriod.RefusalRate,
					WithdrawnRate = trainingPeriod.WithdrawnRate,
					ApprovedSlippageRate = trainingPeriod.SlippageApprovedAmount,
					ClaimedSlippageRate = trainingPeriod.SlippageClaimedAmount
				};

				_trainingPeriodService.SaveBudgetRates(budgetModel);
			}
		}
	}

	internal enum CommitmentType
	{
		Requested,
		Agreed
	}
}