using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class AgreementTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public int? GrantApplicationId { get; set; }

		public int? TrainingProgramId { get; set; }

		public string Name { get; set; }

		public AgreementTrainingProviderViewModel RequestedTrainingProvider { get; set; }

		public string ChangeRequestReason { get; set; }
		#endregion

		#region Constructors
		public AgreementTrainingProviderViewModel() { }

		public AgreementTrainingProviderViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.Id = trainingProvider.Id;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.GrantApplicationId = trainingProvider.GrantApplicationId;
			this.TrainingProgramId = trainingProvider.TrainingPrograms.FirstOrDefault()?.Id;
			this.Name = trainingProvider.Name;
		}

		public AgreementTrainingProviderViewModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));

			var trainingProvider = trainingProgram.TrainingProvider ?? throw new ArgumentNullException($"{nameof(trainingProgram)}.{trainingProgram.TrainingProvider}");

			this.Id = trainingProvider.Id;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.TrainingProgramId = trainingProgram.TrainingProvider.Id;
			this.Name = trainingProvider.Name;
			if (trainingProvider.RequestedTrainingProvider != null)
			{
				this.RequestedTrainingProvider = new AgreementTrainingProviderViewModel(trainingProvider.RequestedTrainingProvider);
				this.ChangeRequestReason = trainingProvider.RequestedTrainingProvider.ChangeRequestReason;
			}
		}
		#endregion
	}
}