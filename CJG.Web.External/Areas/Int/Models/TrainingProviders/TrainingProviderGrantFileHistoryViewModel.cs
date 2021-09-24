using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProviderGrantFileHistoryViewModel : BaseViewModel
	{
		public int TrainingProviderInventoryId { get; set; }

		public string TrainingProviderInventoryName { get; set; }

		public string TrainingProviderInventoryAcronym { get; set; }

		public string TrainingProviderNotes { get; set; }

		public bool AllowDeleteTrainingProvider { get; set; } = false;

		public string UrlReferrer { get; set; }

		public string RowVersion { get; set; }

		public TrainingProviderGrantFileHistoryViewModel()
		{
		}

		public TrainingProviderGrantFileHistoryViewModel(TrainingProviderInventory trainingProviderInventory)
		{
			this.TrainingProviderInventoryId = trainingProviderInventory.Id;
			this.TrainingProviderInventoryName = trainingProviderInventory.Name;
			this.TrainingProviderInventoryAcronym = trainingProviderInventory.Acronym;
			this.TrainingProviderNotes = trainingProviderInventory.Notes;
			this.RowVersion = Convert.ToBase64String(trainingProviderInventory.RowVersion);
		}
	}
}
