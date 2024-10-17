using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
	public class RequestedTrainingProviderViewModel : BaseViewModel
	{
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
		public ApplicationAddressViewModel TrainingProviderAddress { get; set; }

		public bool? TrainingOutsideBC { get; set; }

		public string OutOfProvinceLocationRationale { get; set; }

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

		public int ProofOfInstructorQualifications { get; set; }
		public int CourseOutline { get; set; }
		public ProgramTypes ProgramType { get; set; }

		public int[] SelectedDeliveryMethodIds { get; set; }
		public bool BlockChangeRequestSubmission { get; set; }

		public RequestedTrainingProviderViewModel() { }

		public RequestedTrainingProviderViewModel(TrainingProvider originalTrainingProvider)
		{
			if (originalTrainingProvider == null)
				throw new ArgumentNullException(nameof(originalTrainingProvider));

			var requestedTrainingProvider = originalTrainingProvider.RequestedTrainingProvider ?? new TrainingProvider(originalTrainingProvider);

			Utilities.MapProperties(requestedTrainingProvider, this);

			TrainingProgramId = originalTrainingProvider.TrainingPrograms.FirstOrDefault()?.Id;

			ContactPhone = requestedTrainingProvider.ContactPhoneNumber;
			ContactPhoneAreaCode = requestedTrainingProvider.ContactPhoneNumber.GetPhoneAreaCode();
			ContactPhoneExchange = requestedTrainingProvider.ContactPhoneNumber.GetPhoneExchange();
			ContactPhoneNumber = requestedTrainingProvider.ContactPhoneNumber.GetPhoneNumber();

			TrainingAddress = requestedTrainingProvider.TrainingAddress != null ? new ApplicationAddressViewModel(requestedTrainingProvider.TrainingAddress) : new ApplicationAddressViewModel();
			TrainingProviderAddress = requestedTrainingProvider.TrainingProviderAddress !=null ? new ApplicationAddressViewModel(requestedTrainingProvider.TrainingProviderAddress) : new ApplicationAddressViewModel();
			TrainingProviderTypeId = requestedTrainingProvider.Id == 0 ? (int?)null : requestedTrainingProvider.TrainingProviderTypeId;
			TrainingOutsideBC = requestedTrainingProvider.Id == 0 ? (bool?)null : requestedTrainingProvider.TrainingOutsideBC;
			BusinessCase = requestedTrainingProvider.BusinessCase;
			PrivateSectorValidationType = requestedTrainingProvider.TrainingProviderType?.PrivateSectorValidationType ?? TrainingProviderPrivateSectorValidationTypes.Never;
			BusinessCaseDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.BusinessCaseDocument, Id, TrainingProviderAttachmentTypes.BusinessCase, RowVersion);
			ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.ProofOfQualificationsDocument, Id, TrainingProviderAttachmentTypes.ProofOfQualifications, RowVersion);
			CourseOutlineDocument = new TrainingProviderAttachmentViewModel(requestedTrainingProvider.CourseOutlineDocument, Id, TrainingProviderAttachmentTypes.CourseOutline, RowVersion);
			ProofOfInstructorQualifications = requestedTrainingProvider.TrainingProviderType?.ProofOfInstructorQualifications ?? 0;
			CourseOutline = requestedTrainingProvider.TrainingProviderType?.CourseOutline ?? 0;

			SelectedDeliveryMethodIds = originalTrainingProvider.TrainingProviderType != null
				? originalTrainingProvider.TrainingProgram?.DeliveryMethods.Select(dm => dm.Id).ToArray()
				: new List<int>()
					.ToArray();

			var maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			MaxUploadSize = maxUploadSize / 1024 / 1024;

			BlockChangeRequestSubmission = false;

			if (originalTrainingProvider.TrainingProgram?.GrantApplication != null)
			{
				var grantApplication = originalTrainingProvider.TrainingProgram.GrantApplication;
				var hasSubmittedClaim = grantApplication.HasSubmittedAClaim();
				var trainingHasStarted = grantApplication.TrainingHasStarted();

				ProgramType = grantApplication.GetProgramType();
				BlockChangeRequestSubmission = hasSubmittedClaim || trainingHasStarted;
			}
		}

		public TrainingProvider MapProperties(HttpPostedFileBase[] files, ITrainingProviderService trainingProviderService, IApplicationAddressService applicationAddressService)
		{
			if (trainingProviderService == null)
				throw new ArgumentNullException(nameof(trainingProviderService));

			var originalTrainingProvider = trainingProviderService.Get(OriginalTrainingProviderId);
			var trainingProvider = Id == 0 ? new TrainingProvider(originalTrainingProvider) : trainingProviderService.Get(Id);

			Utilities.MapProperties(this, trainingProvider);

			Region nonCanadianRegion = null;

			if (TrainingProviderAddress != null)
			{ 
				if (trainingProvider.TrainingProviderAddress == null)
					trainingProvider.TrainingProviderAddress = new ApplicationAddress();

				trainingProvider.TrainingProviderAddress.AddressLine1 = TrainingProviderAddress.AddressLine1;
				trainingProvider.TrainingProviderAddress.AddressLine2 = TrainingProviderAddress.AddressLine2;
				trainingProvider.TrainingProviderAddress.City = TrainingProviderAddress.City;


				if (TrainingProviderAddress.IsCanadianAddress)
				{
					trainingProvider.TrainingProviderAddress.PostalCode = TrainingProviderAddress.PostalCode;
					trainingProvider.TrainingProviderAddress.RegionId = TrainingProviderAddress.RegionId;
					trainingProvider.TrainingProviderAddress.CountryId = Constants.CanadaCountryId;
				}
				else
				{
					nonCanadianRegion = applicationAddressService.VerifyOrCreateRegion(TrainingProviderAddress.OtherRegion, TrainingProviderAddress.CountryId);
					trainingProvider.TrainingProviderAddress.PostalCode = TrainingProviderAddress.OtherZipCode;
					trainingProvider.TrainingProviderAddress.RegionId = nonCanadianRegion.Id;
					trainingProvider.TrainingProviderAddress.CountryId = TrainingProviderAddress.CountryId;
				}
			}

			if (SelectedDeliveryMethodIds != null
			    && (SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom) || SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace)
					|| SelectedDeliveryMethodIds.Contains(0)))
			{
				if (trainingProvider.TrainingAddress == null)
					trainingProvider.TrainingAddress = new ApplicationAddress();

				trainingProvider.TrainingAddress.AddressLine1 = TrainingAddress.AddressLine1;
				trainingProvider.TrainingAddress.AddressLine2 = TrainingAddress.AddressLine2;
				trainingProvider.TrainingAddress.City = TrainingAddress.City;

				if (TrainingAddress.IsCanadianAddress)
				{
					trainingProvider.TrainingAddress.PostalCode = TrainingAddress.PostalCode;
					trainingProvider.TrainingAddress.RegionId = TrainingAddress.RegionId;
					trainingProvider.TrainingAddress.CountryId = Constants.CanadaCountryId;
				}
				else
				{
					// We have to get the existing region if it's the same for both addresses, otherwise we get an EF error
					var region = GetInternationalRegion(TrainingAddress, TrainingProviderAddress, nonCanadianRegion, applicationAddressService);
					trainingProvider.TrainingAddress.PostalCode = TrainingAddress.OtherZipCode;
					trainingProvider.TrainingAddress.RegionId = region.Id;
					trainingProvider.TrainingAddress.CountryId = TrainingAddress.CountryId;
				}
			}

			trainingProvider.ContactPhoneNumber = ContactPhone;

			trainingProvider.TrainingProviderType = trainingProviderService.Get<TrainingProviderType>(trainingProvider.TrainingProviderTypeId);

			if (files != null)
			{
				if (BusinessCaseDocument.Index.HasValue && files.Count() > BusinessCaseDocument.Index.Value)
				{
					var file = files[BusinessCaseDocument.Index.Value];
					if (trainingProvider.BusinessCaseDocument != null)
					{
						trainingProvider.BusinessCaseDocument.CreateNewVersion(file, null);
					}
					else
					{
						trainingProvider.BusinessCaseDocument = file.UploadFile(null, file.FileName);
					}
				}
				if (CourseOutlineDocument.Index.HasValue && files.Count() > CourseOutlineDocument.Index.Value)
				{
					var file = files[CourseOutlineDocument.Index.Value];
					if (trainingProvider.CourseOutlineDocument != null)
					{
						trainingProvider.CourseOutlineDocument.CreateNewVersion(file, null);
					}
					else
					{
						trainingProvider.CourseOutlineDocument = file.UploadFile(null, file.FileName);
					}
				}
				if (ProofOfQualificationsDocument.Index.HasValue && files.Count() > ProofOfQualificationsDocument.Index.Value)
				{
					var file = files[ProofOfQualificationsDocument.Index.Value];
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
            if (ProgramType == ProgramTypes.WDAService)
            {
                if (!SelectedDeliveryMethodIds.Equals(Constants.Delivery_Online))
                {
                    if (!TrainingAddress.IsCanadianAddress)
					{
						trainingProvider.BusinessCaseDocumentId = null;
						trainingProvider.BusinessCaseDocument = null;
					}
				}
			}

			return trainingProvider;
		}

		private Region GetInternationalRegion(AddressSharedViewModel trainingAddress, AddressSharedViewModel trainingProviderAddress, Region existingRegion, IApplicationAddressService applicationAddressService)
		{
			if (trainingAddress.CountryId == trainingProviderAddress.CountryId && trainingAddress.OtherRegion.Equals(trainingProviderAddress.OtherRegion, StringComparison.CurrentCultureIgnoreCase))
				return existingRegion;

			return applicationAddressService.VerifyOrCreateRegion(trainingAddress.OtherRegion, trainingAddress.CountryId);
		}
	}
}