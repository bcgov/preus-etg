using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Models.Shared;
using System;
using System.Configuration;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string Name { get; set; }
		public string RowVersion { get; set; }
		public TrainingProviderType TrainingProviderType { get; set; }
		public int? TrainingProviderInventoryId { get; set; }
		public int MaxUploadSizeKB { get; set; }
		public Attachments.AttachmentViewModel CourseOutlineDocument { get; set; }
		public Attachments.AttachmentViewModel ProofOfQualificationsDocument { get; set; }
		public Attachments.AttachmentViewModel BusinessCaseDocument { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; }		
		public TrainingTrainerDetailsListViewModel TrainingTrainerDetailsListViewModel { get; set; }
		public AddressViewModel TrainingLocationListViewModel { get; set; }
		public TrainingOutsideBCListViewModel TrainingOutsideBcListViewModel { get; set; }
		public bool RequiresTrainingProviderValidation { get; set; }

		public bool CanValidateTrainingProvider { get; set; }
		#endregion

		#region Constructors
		public TrainingProviderViewModel() { }

		public TrainingProviderViewModel(TrainingProvider trainingProvider, IPrincipal user)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = trainingProvider.GetGrantApplication();
			this.GrantApplicationId = grantApplication.Id;
			this.Id = trainingProvider.Id;
			this.Name = trainingProvider.Name;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.TrainingProviderType = trainingProvider.TrainingProviderType;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
			this.MaxUploadSizeKB = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			this.RequiresTrainingProviderValidation = trainingProvider.TrainingProviderInventoryId == null;
			this.CanValidateTrainingProvider = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);

			if (trainingProvider.CourseOutlineDocument != null) this.CourseOutlineDocument = new Attachments.AttachmentViewModel(trainingProvider.CourseOutlineDocument);
			if (trainingProvider.ProofOfQualificationsDocument != null) this.ProofOfQualificationsDocument = new Attachments.AttachmentViewModel(trainingProvider.ProofOfQualificationsDocument);
			if (trainingProvider.BusinessCaseDocument != null) this.BusinessCaseDocument = new Attachments.AttachmentViewModel(trainingProvider.BusinessCaseDocument);

			this.TrainingTrainerDetailsListViewModel = new TrainingTrainerDetailsListViewModel(trainingProvider);
			this.TrainingLocationListViewModel = new AddressViewModel(trainingProvider.TrainingAddress);
			this.TrainingOutsideBcListViewModel = new TrainingOutsideBCListViewModel(trainingProvider);
		}
		#endregion
	}
}