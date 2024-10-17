using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class TrainingProgramTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public string CourseTitle { get; set; }
		public string TrainingProviderName { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public decimal AssessedCost { get; set; }
		#endregion

		#region Constructors
		public TrainingProgramTemplateModel()
		{
		}

		public TrainingProgramTemplateModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			this.Id = trainingProgram.Id;
			this.CourseTitle = trainingProgram.CourseTitle;
			this.TrainingProviderName = trainingProgram.TrainingProvider?.Name;
			this.StartDate = trainingProgram.StartDate.ToStringLocalTime();
			this.EndDate = trainingProgram.EndDate.ToStringLocalTime();
			if (trainingProgram.EligibleCostBreakdownId != null)
				this.AssessedCost = trainingProgram.EligibleCostBreakdown.AssessedCost;
		}
		#endregion
	}
}
