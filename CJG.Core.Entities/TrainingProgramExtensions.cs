namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="TrainingProgramExtensions"/> static class, provides extension methods for <typeparamref name="TrainingProgram"/> objects.
	/// </summary>
	public static class TrainingProgramExtensions
	{
		//TODO: There are no more usages of these extension methods. Consider removal.

		/// <summary>
		/// The Training Program start date must be between the grant application's intake period start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasStartDateWithinIntakeDates(this TrainingProgram trainingProgram)
		{
			return trainingProgram.StartDate.ToLocalTime().Date >= trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime().Date
				&& trainingProgram.StartDate.ToLocalTime().Date <= trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime().Date;
		}

		/// <summary>
		/// The Training Program end date must be between the grant application's intake period start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasEndDateWithinIntakeDates(this TrainingProgram trainingProgram)
		{
			return trainingProgram.EndDate.ToLocalTime().Date >= trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime().Date
				&& trainingProgram.EndDate.ToLocalTime().Date <= trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime().Date;
		}

		/// <summary>
		/// Validates both the start date and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasValidDates(this TrainingProgram trainingProgram)
		{
			return HasStartDateWithinIntakeDates(trainingProgram) && HasEndDateWithinIntakeDates(trainingProgram);
		}

		/* No longer in use - possibly remove

		/// <summary>
		/// The Training Program start date must be between the grant application start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasStartDateWithin30Days(this TrainingProgram trainingProgram)
		{
			bool isEarliestDeliveryDate = true;

			if (trainingProgram.IsSkillsTraining)
			{
				isEarliestDeliveryDate = (trainingProgram.StartDate.ToLocalTime().Date - trainingProgram.GrantApplication.StartDate.ToLocalTime().Date).TotalDays >= 30;
			}
			return isEarliestDeliveryDate;
		}

		/// <summary>
		/// The Training Program end date must be between the grant application start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasEndDateWithin30Days(this TrainingProgram trainingProgram)
		{
			bool isEarliestDeliveryDate = true;
			if (trainingProgram.IsSkillsTraining)
			{
				isEarliestDeliveryDate = ((trainingProgram.GrantApplication.EndDate.ToLocalTime().Date - trainingProgram.EndDate.ToLocalTime().Date).TotalDays >= 30);
			}

			return isEarliestDeliveryDate;
		}

		/// <summary>
		/// Validate whether the skills training component information is complete.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool SkillsTrainingConfirmed(this TrainingProgram trainingProgram)
		{
			return trainingProgram.EligibleCostBreakdown?.EstimatedCost > 0;
		}

		/// <summary>
		/// The Training Program start date must be within current fiscal year.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasStartDateWithinCurrentFiscalYear(this TrainingProgram trainingProgram)
		{
			return trainingProgram.StartDate.ToLocalTime().Date >= AppDateTime.CurrentFYStartDateMorning.Date
				&& trainingProgram.StartDate.ToLocalTime().Date <= AppDateTime.CurrentFYEndDateMidnight.Date;
		}
		*/
	}
}
