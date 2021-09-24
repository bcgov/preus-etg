using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface ITrainingPeriodService
	{
		IEnumerable<TrainingPeriod> GetAllFor(int fiscalYearId, int grantProgramId);
		IEnumerable<TrainingPeriod> GetAllFor(int fiscalYearId, int grantProgramId, int grantStreamId);
		TrainingPeriod Get(int id);
		TrainingPeriod Add(TrainingPeriod trainingPeriod);
		TrainingPeriod Update(TrainingPeriod trainingPeriod);
		TrainingPeriod ToggleStatus(TrainingPeriod trainingPeriod);
		void Delete(TrainingPeriod trainingPeriod);
	}
}