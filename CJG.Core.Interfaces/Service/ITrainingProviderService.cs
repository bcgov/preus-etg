using CJG.Application.Business.Models;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface ITrainingProviderService : IService
	{
		TrainingProvider Get(int id);
		TrainingProvider Add(TrainingProvider trainingProvider);

		TrainingProvider Update(TrainingProvider trainingProvider);
		TrainingProviderType GetDefaultTrainingProviderType();
		TrainingProvider ValidateTrainingProvider(TrainingProvider trainingProvider, int trainingProviderInventoryId);
		void Delete(TrainingProvider trainingProvider);
		void DeleteRequestedTrainingProvider(TrainingProvider trainingProvider);
		void AddAttachment(TrainingProvider trainingProvider, Attachment attachment, TrainingProviderAttachmentTypes type);
	}
}
