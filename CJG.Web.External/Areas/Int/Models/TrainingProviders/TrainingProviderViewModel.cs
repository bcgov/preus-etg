using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Int.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProviderViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string Name { get; set; }
		public string RowVersion { get; set; }
		public TrainingProviderType TrainingProviderType { get; set; }
		public int? TrainingProviderInventoryId { get; set; }
		public int MaxUploadSizeKB { get; set; }
		public AttachmentViewModel CourseOutlineDocument { get; set; }
		public AttachmentViewModel ProofOfQualificationsDocument { get; set; }
		public AttachmentViewModel BusinessCaseDocument { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; }
		public TrainingTrainerDetailsListViewModel TrainingTrainerDetailsListViewModel { get; set; }
		public AddressViewModel TrainingLocationListViewModel { get; set; }

		public AddressViewModel TrainingProviderLocationListViewModel { get; set; }
		public TrainingOutsideBCListViewModel TrainingOutsideBcListViewModel { get; set; }
		public bool RequiresTrainingProviderValidation { get; set; }

		public string AlternativeTrainingOptions { get; set; }
		public string ChoiceOfTrainerOrProgram { get; set; }

		public bool CanValidateTrainingProvider { get; set; }
		public ProgramTypes ProgramType { get; set; }

		public int[] SelectedDeliveryMethodIds { get; set; }


		public TrainingProviderViewModel()
		{
		}

		public TrainingProviderViewModel(TrainingProvider trainingProvider, IPrincipal user)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var grantApplication = trainingProvider.GetGrantApplication();

			GrantApplicationId = grantApplication.Id;
			Id = trainingProvider.Id;
			Name = trainingProvider.Name;
			RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			TrainingProviderType = trainingProvider.TrainingProviderType;
			TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			TrainingProviderState = trainingProvider.TrainingProviderState;
			MaxUploadSizeKB = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			RequiresTrainingProviderValidation = trainingProvider.TrainingProviderInventoryId == null;
			CanValidateTrainingProvider = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);

			AlternativeTrainingOptions = trainingProvider.AlternativeTrainingOptions;
			ChoiceOfTrainerOrProgram = trainingProvider.ChoiceOfTrainerOrProgram;
			
			if (trainingProvider.CourseOutlineDocument != null)
				CourseOutlineDocument = new AttachmentViewModel(trainingProvider.CourseOutlineDocument);

			if (trainingProvider.ProofOfQualificationsDocument != null)
				ProofOfQualificationsDocument = new AttachmentViewModel(trainingProvider.ProofOfQualificationsDocument);

			if (trainingProvider.BusinessCaseDocument != null)
				BusinessCaseDocument = new AttachmentViewModel(trainingProvider.BusinessCaseDocument);

			TrainingTrainerDetailsListViewModel = new TrainingTrainerDetailsListViewModel(trainingProvider);

			if (trainingProvider.TrainingAddress != null)
				TrainingLocationListViewModel = new AddressViewModel(trainingProvider.TrainingAddress);

			if (trainingProvider.TrainingProviderAddress != null)
				TrainingProviderLocationListViewModel = new AddressViewModel(trainingProvider.TrainingProviderAddress);

			TrainingOutsideBcListViewModel = new TrainingOutsideBCListViewModel(trainingProvider);

			ProgramType = grantApplication.GetProgramType();

			if (grantApplication.GetProgramType() == ProgramTypes.WDAService && trainingProvider.TrainingPrograms.Count == 0)
			{
				// Handle Employment Assistance Service Provider case internally
				SelectedDeliveryMethodIds = trainingProvider.GrantApplication.TrainingPrograms?.FirstOrDefault()
					?.DeliveryMethods.Select(dm => dm.Id).ToArray();
			}
			else
			{
				SelectedDeliveryMethodIds = trainingProvider.TrainingProgram?.DeliveryMethods?.Select(dm => dm.Id).ToArray();
			}
		}
	}
}