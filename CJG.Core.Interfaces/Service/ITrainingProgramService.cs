using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface ITrainingProgramService : IService
	{
		TrainingProgram Get(int id);
		TrainingProgram Add(TrainingProgram trainingProgram);
		TrainingProgram Update(TrainingProgram trainingProgram);

		void UpdateProgramDates(TrainingProgram trainingProgram);
		void Delete(TrainingProgram trainingProgram);
		void UpdateDeliveryMethods(TrainingProgram trainingProgram, int[] selectedDeliveryMethodIds);

		void ChangeEligibility(TrainingProgram trainingProgram);
	}
}
