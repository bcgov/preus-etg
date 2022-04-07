using System;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.TrainingPrograms
{
    public class TrainingProgramExtraInfoViewModel : BaseViewModel
	{
		public string BusinessTrainingRelevance { get; set; }

		public TrainingProgramExtraInfoViewModel() { }

		public TrainingProgramExtraInfoViewModel(TrainingProgram trainingProgram)
		{
			Id = trainingProgram?.Id ?? throw new ArgumentNullException(nameof(trainingProgram));
			BusinessTrainingRelevance = trainingProgram.BusinessTrainingRelevance;
		}
	}
}