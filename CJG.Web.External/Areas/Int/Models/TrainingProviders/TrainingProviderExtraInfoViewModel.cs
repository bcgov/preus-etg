using System;
using System.Security.Principal;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProviderExtraInfoViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }

		public string AlternativeTrainingOptions { get; set; }
		public string ChoiceOfTrainerOrProgram { get; set; }

		public TrainingProviderExtraInfoViewModel()
		{
		}

		public TrainingProviderExtraInfoViewModel(TrainingProvider trainingProvider, IPrincipal user)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var grantApplication = trainingProvider.GetGrantApplication();

			GrantApplicationId = grantApplication.Id;
			Id = trainingProvider.Id;

			AlternativeTrainingOptions = trainingProvider.AlternativeTrainingOptions;
			ChoiceOfTrainerOrProgram = trainingProvider.ChoiceOfTrainerOrProgram;
		}
	}
}