using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
	public class RequestedTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public int OriginalTrainingProviderId { get; set; }

		[Required(ErrorMessage = "Change request reason is required.")]
		public string ChangeRequestReason { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Training provider type is required.")]
		public int? TrainingProviderTypeId { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; }

		public int? EligibleCostId { get; set; }
		public int? GrantApplicationId { get; set; }
		public int? TrainingProgramId { get; set; }
		public ApplicationAddressViewModel TrainingAddress { get; set; }

		[Required(ErrorMessage = "Training outside BC is required.")]
		public bool? TrainingOutsideBC { get; set; }

		public string BusinessCase { get; set; }

		[Required(ErrorMessage = "Contact first name is required.")]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "Contact last name is required.")]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "Contact email is required.")]
		public string ContactEmail { get; set; }

		[Required(ErrorMessage = "Contact phone number is required.")]
		public string ContactPhone { get; set; }

		public string ContactPhoneAreaCode { get; set; }

		public string ContactPhoneExchange { get; set; }

		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }

		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public int? ProofOfQualificationsDocumentId { get; set; }
		public TrainingProviderAttachmentViewModel ProofOfQualificationsDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.ProofOfQualifications);
		public int? BusinessCaseDocumentId { get; set; }
		public TrainingProviderAttachmentViewModel BusinessCaseDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.BusinessCase);
		public int? CourseOutlineDocumentId { get; set; }
		public TrainingProviderAttachmentViewModel CourseOutlineDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.CourseOutline);
		public int MaxUploadSize { get; set; }
		#endregion

		#region Constructors
		public RequestedTrainingProviderViewModel() { }

		public RequestedTrainingProviderViewModel(TrainingProvider originalTrainingProvider)
		{
			if (originalTrainingProvider == null) throw new ArgumentNullException(nameof(originalTrainingProvider));

			var requestedTrainingProvider = originalTrainingProvider.RequestedTrainingProvider ?? new TrainingProvider(originalTrainingProvider);

			Utilities.MapProperties(requestedTrainingProvider, this);

			this.TrainingProgramId = originalTrainingProvider.TrainingPrograms.FirstOrDefault()?.Id;

			this.ContactPhone = requestedTrainingProvider.ContactPhoneNumber;
			this.ContactPhoneAreaCode = requestedTrainingProvider.ContactPhoneNumber.GetPhoneAreaCode();
			this.ContactPhoneExchange = requestedTrainingProvider.ContactPhoneNumber.GetPhoneExchange();
			this.ContactPhoneNumber = requestedTrainingProvider.ContactPhoneNumber.GetPhoneNumber();

			this.TrainingAddress = requestedTrainingProvider.TrainingAddress != null ? new ApplicationAddressViewModel(requestedTrainingProvider.TrainingAddress) : new ApplicationAddressViewModel();

			this.TrainingProviderTypeId = requestedTrainingProvider.Id == 0 ? (int?)null : requestedTrainingProvider.TrainingProviderTypeId;
			this.TrainingOutsideBC = requestedTrainingProvider.Id == 0 ? (bool?)null : requestedTrainingProvider.TrainingOutsideBC;
			this.BusinessCase = requestedTrainingProvider.BusinessCase;
			this.PrivateSectorValidationType = requestedTrainingProvider.TrainingProviderType != null ? requestedTrainingProvider.TrainingProviderType.PrivateSectorValidationType : TrainingProviderPrivateSectorValidationTypes.Never;
			this.BusinessCaseDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.BusinessCaseDocument, this.Id, TrainingProviderAttachmentTypes.BusinessCase, this.RowVersion);
			this.ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.ProofOfQualificationsDocument, this.Id, TrainingProviderAttachmentTypes.ProofOfQualifications, this.RowVersion);
			this.CourseOutlineDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.CourseOutlineDocument, this.Id, TrainingProviderAttachmentTypes.CourseOutline, this.RowVersion);

			this.MaxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
		}
		#endregion

		#region Methods
		public TrainingProvider MapProperties(HttpPostedFileBase[] files, ITrainingProviderService trainingProviderService)
		{
			if (trainingProviderService == null) throw new ArgumentNullException(nameof(trainingProviderService));

			var originalTrainingProvider = trainingProviderService.Get(this.OriginalTrainingProviderId);
			var trainingProvider = this.Id == 0 ? new TrainingProvider(originalTrainingProvider) : trainingProviderService.Get(this.Id);

			Utilities.MapProperties(this, trainingProvider);

			if (trainingProvider.TrainingAddress == null) trainingProvider.TrainingAddress = new ApplicationAddress();
			Utilities.MapProperties(this.TrainingAddress, trainingProvider.TrainingAddress);

			trainingProvider.ContactPhoneNumber = this.ContactPhone;

			trainingProvider.TrainingProviderType = trainingProviderService.Get<TrainingProviderType>(trainingProvider.TrainingProviderTypeId);

			if (files != null)
			{
				if (this.BusinessCaseDocument.Index.HasValue && files.Count() > this.BusinessCaseDocument.Index.Value)
				{
					var file = files[this.BusinessCaseDocument.Index.Value];
					if (trainingProvider.BusinessCaseDocument != null)
					{
						trainingProvider.BusinessCaseDocument.CreateNewVersion(file, null);
					}
					else
					{
						trainingProvider.BusinessCaseDocument = file.UploadFile(null, file.FileName);
					}
				}
				if (this.CourseOutlineDocument.Index.HasValue && files.Count() > this.CourseOutlineDocument.Index.Value)
				{
					var file = files[this.CourseOutlineDocument.Index.Value];
					if (trainingProvider.CourseOutlineDocument != null)
					{
						trainingProvider.CourseOutlineDocument.CreateNewVersion(file, null);
					}
					else
					{
						trainingProvider.CourseOutlineDocument = file.UploadFile(null, file.FileName);
					}
				}
				if (this.ProofOfQualificationsDocument.Index.HasValue && files.Count() > this.ProofOfQualificationsDocument.Index.Value)
				{
					var file = files[this.ProofOfQualificationsDocument.Index.Value];
					if (trainingProvider.ProofOfQualificationsDocument != null)
					{
						trainingProvider.ProofOfQualificationsDocument.CreateNewVersion(file, null);
					}
					else
					{
						trainingProvider.ProofOfQualificationsDocument = file.UploadFile(null, file.FileName);
					}
				}
			}

			if (!this.TrainingAddress.IsCanadianAddress)
			{
				trainingProvider.BusinessCaseDocumentId = null;
				trainingProvider.BusinessCaseDocument = null;
			}

			if (!trainingProvider.IsPrivateSectorType(AppDateTime.Now))
			{
				trainingProvider.CourseOutlineDocumentId = null;
				trainingProvider.CourseOutlineDocument = null;
				trainingProvider.ProofOfQualificationsDocumentId = null;
				trainingProvider.ProofOfQualificationsDocument = null;
			}

			return trainingProvider;
		}
		#endregion
	}
}