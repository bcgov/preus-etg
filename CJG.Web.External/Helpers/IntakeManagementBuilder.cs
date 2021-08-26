using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Helpers
{
	/// <summary>
	/// IntakeManagementBuilder class, provides methods to initialize GrantApplication Intake.
	/// </summary>
	public class IntakeManagementBuilder
    {
        #region Variables
        private readonly IStaticDataService _staticDataService;
        private readonly IGrantProgramService _grantProgramService;
        private readonly IGrantStreamService _grantStreamService;
        private readonly IGrantOpeningService _grantOpeningService;
        #endregion

        #region Constructors
        public IntakeManagementBuilder(IStaticDataService staticDataService, IGrantProgramService grantProgramService, IGrantStreamService grantStreamService, IGrantOpeningService grantOpeningService)
        {
            _staticDataService = staticDataService;
            _grantProgramService = grantProgramService;
            _grantStreamService = grantStreamService;
            _grantOpeningService = grantOpeningService;
        }
        #endregion

        #region Methods
        public IntakeManagementViewModel Build(int? grantProgramId = null, int? grantStreamId = null, int? trainingPeriodId = null)
        {
            return Build(grantProgramId ?? GetDefaultGrantProgramId(), grantStreamId, trainingPeriodId, IntakeManagementViewModel.NavigationCommand.None);
        }

        public IntakeManagementViewModel Build(int? grantProgramId, int? grantStreamId, int? trainingPeriodId, IntakeManagementViewModel.NavigationCommand navigationCommand)
        {
            TrainingPeriod[] trainingPeriods;
            if (trainingPeriodId.HasValue)
            {
                int numberToShift;
                switch (navigationCommand)
                {
                    case IntakeManagementViewModel.NavigationCommand.Next:
                        numberToShift = 1;
                        break;
                    case IntakeManagementViewModel.NavigationCommand.Previous:
                        numberToShift = -1;
                        break;
                    default:
                        numberToShift = 0;
                        break;
                }

                trainingPeriods = GetTrainingPeriods(trainingPeriodId.Value,
                    numberToShift, 1);
            }
            else
            {
                trainingPeriods = GetTrainingPeriods(AppDateTime.UtcNow, 1);
            }
            var grantPrograms = GetAllGrantPrograms().ToList();

            var grantStreams = GetActiveGrantStreams(grantProgramId ?? 0).ToList();

            grantStreamId = GetDefaultGrantStreamId(grantStreams, grantStreamId);

            var periods = trainingPeriods.Select(x => LoadPeriod(x, grantStreamId.Value)).ToList();
            var middleIndex = (periods.Count - 1) / 2;

            return new IntakeManagementViewModel
            {
                TrainingPeriods = periods,
                GrantPrograms = grantPrograms,
                GrantStreams = grantStreams,
                GrantProgramId = grantProgramId,
                GrantStreamId = grantStreamId,
                TrainingPeriodId = periods != null && periods.Count() > 0 ? periods[middleIndex]?.Id : null
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

        private IntakeManagementViewModel.TrainingPeriodViewModel LoadPeriod(TrainingPeriod trainingPeriod, int grantStreamId)
        {
            var grantOpening = _grantOpeningService.GetGrantOpening(grantStreamId, trainingPeriod.Id);

            var trainingPeriodViewModel = new IntakeManagementViewModel.TrainingPeriodViewModel
            {
                Id = trainingPeriod.Id,
                FiscalYearName = trainingPeriod.FiscalYear.Caption,
                TrainingPeriodName = trainingPeriod.Caption,
                StartDate = trainingPeriod.StartDate,
                EndDate = trainingPeriod.EndDate,
                Status = grantOpening?.State.ToString(),
                GrantOpeningIntakes = LoadGrantOpeningIntakes(grantOpening),
                IntakeTargetAmt = grantOpening?.IntakeTargetAmt ?? 0
            };

            trainingPeriodViewModel.TotalApplicationsIntake =
                trainingPeriodViewModel.GrantOpeningIntakes.Where(x => !x.Value.StateName.Equals("Reductions")).Sum(x => x.Value.Number);

            trainingPeriodViewModel.TotalApplicationsIntakeAmt =
                trainingPeriodViewModel.GrantOpeningIntakes.Sum(x => x.Value.Value);

            trainingPeriodViewModel.OverUnderAmt = trainingPeriodViewModel.TotalApplicationsIntakeAmt -
                                                   trainingPeriodViewModel.IntakeTargetAmt;

            trainingPeriodViewModel.OverUnderPerc = trainingPeriodViewModel.OverUnderAmt != 0 ? (decimal?)(trainingPeriodViewModel.OverUnderAmt / trainingPeriodViewModel.IntakeTargetAmt) : null;

            trainingPeriodViewModel.CurrentReservations = grantOpening?.GrantOpeningFinancial.CurrentReservations;

            return trainingPeriodViewModel;
        }

        private static Dictionary<int, IntakeManagementViewModel.GrantOpeningIntakeViewModel> LoadGrantOpeningIntakes(GrantOpening grantOpening)
        {
            var openingIntakes = grantOpening?.GrantOpeningIntake;
            var openingFinancials = grantOpening?.GrantOpeningFinancial;

            return new Dictionary<int, IntakeManagementViewModel.GrantOpeningIntakeViewModel>
            {
                    {
                        1, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("New", openingIntakes?.NewCount ?? 0, openingIntakes?.NewAmt ?? 0)
                    },
                    {
                        2, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Pending Assessment", openingIntakes?.PendingAssessmentCount ?? 0, openingIntakes?.PendingAssessmentAmt ?? 0)
                    },
                    {
                        3, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Under Assessment", openingIntakes?.UnderAssessmentCount ?? 0, openingIntakes?.UnderAssessmentAmt ?? 0)
                    },
                    {
                        4, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Denied", openingIntakes?.DeniedCount ?? 0, openingIntakes?.DeniedAmt ?? 0)
                    },
                    {
                        5, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Withdrawn", openingIntakes?.WithdrawnCount ?? 0, openingIntakes?.WithdrawnAmt ?? 0)
                    },
                    {
                        6, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Reductions", 0, openingIntakes?.ReductionsAmt ?? 0)
                    },
                    {
                        7, new IntakeManagementViewModel.GrantOpeningIntakeViewModel("Commitments", openingFinancials?.AssessedCommitmentsCount ?? 0, openingFinancials?.AssessedCommitments ?? 0)
                    }
                };
        }

        public int? GetDefaultGrantProgramId()
        {
            var grantPrograms = GetAllGrantPrograms().ToList();

            if (grantPrograms == null || !grantPrograms.Any())
            {
                return null;
            }

            return grantPrograms.First().Key;
        }

        public int GetDefaultGrantStreamId(IEnumerable<KeyValuePair<int, string>> grantStreams, int? grantStreamId)
        {
            KeyValuePair<int, string> result;
            return (result = grantStreams.FirstOrDefault(o => o.Key == grantStreamId)).Equals(default(KeyValuePair<int, string>)) &&
                   (result = grantStreams.FirstOrDefault()).Equals(default(KeyValuePair<int, string>)) ?
                   0 : result.Key;
        }

        public TrainingPeriod[] GetTrainingPeriods(DateTime currentDate, int numberToTake)
        {
            List<TrainingPeriod> trainingPeriods;
            int index = SearchTrainingPeriodIndex(x => x.StartDate < currentDate && x.EndDate > currentDate, 0, out trainingPeriods);
            return FilterTrainingPeriods(trainingPeriods, index, numberToTake);
        }

        public TrainingPeriod[] GetTrainingPeriods(int trainingPeriodId, int numberToShift, int numberToTake)
        {
            List<TrainingPeriod> trainingPeriods;
            int index = SearchTrainingPeriodIndex(x => x.Id == trainingPeriodId, numberToShift, out trainingPeriods);
            return FilterTrainingPeriods(trainingPeriods, index, numberToTake);
        }

        internal int SearchTrainingPeriodIndex(Func<TrainingPeriod, bool> selectPeriod, int numberToShift, out List<TrainingPeriod> trainingPeriods)
        {
            trainingPeriods = _staticDataService.GetTrainingPeriods().OrderBy(x => x.StartDate).ToList();

            if (trainingPeriods == null || !trainingPeriods.Any())
                throw new ApplicationException("Cannot find any training periods.");

            var foundIndexedPeriod = trainingPeriods.Select((x, ind) => new { Index = ind, Value = x }).FirstOrDefault(x => selectPeriod(x.Value));

            return foundIndexedPeriod?.Index + numberToShift ?? 0;
        }

        internal static TrainingPeriod[] FilterTrainingPeriods(List<TrainingPeriod> trainingPeriods, int fromTrainingPeriodIndex, int numberToTake)
        {
            if (trainingPeriods == null)
                throw new ArgumentNullException(nameof(trainingPeriods));

            if (fromTrainingPeriodIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(fromTrainingPeriodIndex), fromTrainingPeriodIndex, "Training period index should be greater than zero.");

            if (numberToTake <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberToTake), fromTrainingPeriodIndex, "Training periods to take should be greater than zero.");

            var numPeriodsToTake = numberToTake * 2 + 1;

			// Find start index of training periods list
            var startIndex = fromTrainingPeriodIndex - numberToTake;
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            else if (startIndex > trainingPeriods.Count - numPeriodsToTake)
            {
                startIndex = trainingPeriods.Count - numPeriodsToTake;
            }

			// Find end index of training periods list
            var endIndex = fromTrainingPeriodIndex + numberToTake;
            if (endIndex < numPeriodsToTake - 1)
            {
                endIndex = numPeriodsToTake - 1;
            }
            else if (endIndex > trainingPeriods.Count - 1)
            {
                endIndex = trainingPeriods.Count - 1;
            }

            return trainingPeriods.Where((x, ind) => ind >= startIndex && ind <= endIndex).ToArray();
        }
        #endregion
    }
}