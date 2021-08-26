using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface ITrainingProviderInventoryService : IService
	{
		TrainingProviderInventory Get(int id);
		IEnumerable<TrainingProviderInventory> GetAll(int page, int quantity);
		IEnumerable<TrainingProviderInventory> Search(string name, int page, int quantity);
		TrainingProviderInventory Add(TrainingProviderInventory TrainingProviderInventory);
		TrainingProviderInventory Update(TrainingProviderInventory TrainingProviderInventory);
		void Delete(int id, ref string MsgType, ref string MsgText);
		TrainingProviderInventory GetTrainingProviderFromInventory(string name);
		PageList<TrainingProviderInventory> GetInventory(int page, int quantity, string search, bool? isActive = null);
		IEnumerable<TrainingProviderInventory> GetActiveTrainingProvidersFromInventory();
		bool IsTrainingProviderInventoryUsedInApplications(int vestedTrainingProviderId);
	}
}
