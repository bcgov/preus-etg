using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class UpdateTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string Name { get; set; }
		public string RowVersion { get; set; }
		public TrainingProviderType TrainingProviderType { get; set; }
		public int? TrainingProviderInventoryId { get; set; }
		public Attachments.UpdateAttachmentViewModel CourseOutlineDocument { get; set; }
		public Attachments.UpdateAttachmentViewModel ProofOfQualificationsDocument { get; set; }
		public Attachments.UpdateAttachmentViewModel BusinessCaseDocument { get; set; }
		public TrainingTrainerDetailsListViewModel TrainingTrainerDetailsListViewModel { get; set; }
		public AddressViewModel TrainingLocationListViewModel { get; set; }

		public AddressViewModel TrainingProviderLocationListViewModel { get; set; }
		public TrainingOutsideBCListViewModel TrainingOutsideBcListViewModel { get; set; }

		public int[] SelectedDeliveryMethodIds { get; set; }

		#endregion

		#region Constructors
		public UpdateTrainingProviderViewModel() { }

		public UpdateTrainingProviderViewModel(TrainingProvider trainingProvider, IPrincipal user)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = trainingProvider.GetGrantApplication();
			this.Id = trainingProvider.Id;
			this.Name = trainingProvider.Name;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.TrainingProviderType = trainingProvider.TrainingProviderType;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;

			if (trainingProvider.CourseOutlineDocument != null) this.CourseOutlineDocument = new Attachments.UpdateAttachmentViewModel(trainingProvider.CourseOutlineDocument);
			if (trainingProvider.ProofOfQualificationsDocument != null) this.ProofOfQualificationsDocument = new Attachments.UpdateAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument);
			if (trainingProvider.BusinessCaseDocument != null) this.BusinessCaseDocument = new Attachments.UpdateAttachmentViewModel(trainingProvider.BusinessCaseDocument);

			this.TrainingTrainerDetailsListViewModel = new TrainingTrainerDetailsListViewModel(trainingProvider);
			this.TrainingLocationListViewModel = new AddressViewModel(trainingProvider.TrainingAddress);
			this.TrainingProviderLocationListViewModel = new AddressViewModel(trainingProvider.TrainingProviderAddress);
			this.TrainingOutsideBcListViewModel = new TrainingOutsideBCListViewModel(trainingProvider);
			this.SelectedDeliveryMethodIds = trainingProvider.TrainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			
		}
		#endregion
	}
}