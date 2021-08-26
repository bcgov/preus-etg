using System;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="TrainingProgramExtensions"/> static class, provides extension methods for <typeparamref name="TrainingProgram"/> objects.
	/// </summary>
	public static class TrainingProgramExtensions
	{

		/// <summary>
		/// The Training Program start date must be between the grant application start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasStartDateWithinDeliveryDates(this TrainingProgram trainingProgram)
		{
			return trainingProgram.StartDate.ToLocalTime().Date >= trainingProgram.GrantApplication.StartDate.ToLocalTime().Date
				&& trainingProgram.StartDate.ToLocalTime().Date <= trainingProgram.GrantApplication.EndDate.ToLocalTime().Date;
		}

		/// <summary>
		/// The Training Program end date must be between the grant application start and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasEndDateWithinDeliveryDates(this TrainingProgram trainingProgram)
		{
			return trainingProgram.EndDate.ToLocalTime().Date >= trainingProgram.GrantApplication.StartDate.ToLocalTime().Date
				&& trainingProgram.EndDate.ToLocalTime().Date <= trainingProgram.GrantApplication.EndDate.ToLocalTime().Date;
		}

		/// <summary>
		/// Validates both the start date and end dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static bool HasValidDates(this TrainingProgram trainingProgram)
		{
			return HasStartDateWithinDeliveryDates(trainingProgram) && HasEndDateWithinDeliveryDates(trainingProgram);
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
	}
}
