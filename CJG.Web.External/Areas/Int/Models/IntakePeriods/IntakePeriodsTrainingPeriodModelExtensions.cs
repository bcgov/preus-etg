using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.IntakePeriods
{
	public static class IntakePeriodsTrainingPeriodModelExtensions
	{
		public static void GetEmptyModel(this IntakePeriodsTrainingPeriodModel model, FiscalYear fiscalYear, int grantProgramId, int grantStreamId, int intakePeriods)
		{
			model.Id = 0;
			model.Caption = $"Intake Period {intakePeriods + 1}";
			model.IsActive = true;
			model.MinimumDate = fiscalYear.StartDate;
			model.MaximumDate = fiscalYear.EndDate;
			model.FiscalId = fiscalYear.Id;
			model.GrantProgramId = grantProgramId;
			model.GrantStreamId = grantStreamId;
			model.StartDateDisabled = false;
			model.EndDateDisabled = false;

			if (AppDateTime.UtcNow > fiscalYear.EndDate)
			{
				model.WarningMessage = "Intake periods in past fiscal years cannot be adjusted.";
				model.StartDateDisabled = true;
				model.EndDateDisabled = true;
			}
		}

		public static void LoadModel(this IntakePeriodsTrainingPeriodModel model, TrainingPeriod intakePeriod, FiscalYear fiscalYear)
		{
			model.Id = intakePeriod.Id;
			model.Caption = intakePeriod.Caption;
			model.IsActive = intakePeriod.IsActive;
			model.StartDate = intakePeriod.StartDate.ToLocalMorning();
			model.EndDate = intakePeriod.EndDate.ToLocalMidnight();
			model.FormattedStartDate = intakePeriod.StartDate.ToStringLocalTime();
			model.FormattedEndDate = intakePeriod.EndDate.ToStringLocalTime();

			model.FiscalId = intakePeriod.FiscalYearId;
			model.GrantProgramId = intakePeriod.GrantStream.GrantProgramId;
			model.GrantStreamId = intakePeriod.GrantStreamId ?? 0;

			if (AppDateTime.UtcNow > fiscalYear.EndDate)
			{
                model.WarningMessage = "Intake periods in past fiscal years cannot be adjusted.";
                model.StartDateDisabled = true;
                model.EndDateDisabled = true;

                return;
            }

			if (intakePeriod.HasClosedGrantOpenings())
			{
				model.WarningMessage = "This Intake Period is associated with a closed Grant Opening. Therefore the Start and End dates cannot be changed.";
				model.StartDateDisabled = true;
				model.EndDateDisabled = true;

				return;
			}

			if (intakePeriod.HasOpenGrantOpenings())
			{
				model.StartDateDisabled = true;
				model.WarningMessage = "Please note. Changing the Intake Period End Date will also change the associated Grant Opening Close Date.";
			}
		}

		/// <summary>
		/// Get a stripped down version of the model, suitable for binding the list via json
		/// </summary>
		/// <param name="trainingPeriod"></param>
		/// <returns></returns>
		public static IntakePeriodsTrainingPeriodModel ToListModel(this TrainingPeriod trainingPeriod)
		{
			return new IntakePeriodsTrainingPeriodModel
			{
				Id = trainingPeriod.Id,
				Caption = trainingPeriod.Caption,
				StartDate = trainingPeriod.StartDate.Date,
				EndDate = trainingPeriod.EndDate.Date,
				FormattedStartDate = trainingPeriod.StartDate.ToStringLocalTime(),
				FormattedEndDate = trainingPeriod.EndDate.ToStringLocalTime(),
				IsActive = trainingPeriod.IsActive
			};
		}

		public static TrainingPeriodStatusModel ToConfirmationDialogModel(this TrainingPeriod period)
		{
			var draftApplications = period.GrantStream.GrantOpenings.Sum(g => g.GrantApplications.Count(ga => ga.ApplicationStateInternal == ApplicationStateInternal.Draft));
			var targetStatus = period.IsActive ? "Disable" : "Enable";
			var grantProgramName = string.Empty;
			var grantStreamName = string.Empty;
			var hasOpenGrantOpenings = period.GrantStream != null && period.HasOpenGrantOpenings();

			if (period.IsActive)
			{
				if (hasOpenGrantOpenings)
				{
					grantProgramName = period.GrantStream.GrantProgram.Name;
					grantStreamName = period.GrantStream.Name;
				}
			}
			
			var cannotAffectDueToPreviousFiscal = AppDateTime.UtcNow > period.FiscalYear.EndDate;
			if (cannotAffectDueToPreviousFiscal)
			{
				return new TrainingPeriodStatusModel
				{
					TargetStatus = targetStatus,
					DialogMessage = "Intake periods in past fiscal years cannot be adjusted.",
					ShowYesAndNoButtons = false
				};
			}

			if (targetStatus == "Enable")
			{
				return new TrainingPeriodStatusModel
				{
					TargetStatus = targetStatus,
					DialogMessage = "Are you sure you want to enable this intake period?",
					ShowYesAndNoButtons = true
				};
			}

			if (hasOpenGrantOpenings)
			{
				return new TrainingPeriodStatusModel
				{
					TargetStatus = targetStatus,
					DialogMessage = $"This intake period is associated with the {grantProgramName}, {grantStreamName} grant opening. You must close the grant opening before you can disable this intake period.",
					ShowYesAndNoButtons = false
				};
			}

			if (draftApplications > 0)
			{
				return new TrainingPeriodStatusModel
				{
					TargetStatus = targetStatus,
					DialogMessage = $"There are {draftApplications} unsubmitted applications for this intake period. Are you sure you want to disable this intake period?",
					ShowYesAndNoButtons = true
				};


			}

			return new TrainingPeriodStatusModel
			{
				TargetStatus = targetStatus,
				DialogMessage = "Are you sure you want to disable this intake period?",
				ShowYesAndNoButtons = true
			};
		}
	}
}