using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class TrainingProviderTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; }
		#endregion

		#region Constructors
		public TrainingProviderTemplateModel()
		{
		}

		public TrainingProviderTemplateModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			this.Id = trainingProvider.Id;
			this.Name = trainingProvider.Name;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
		}
		#endregion
	}
}
