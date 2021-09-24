using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Ext.Models.TrainingProviders
{
	public class TrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string GrantApplicationRowVersion { get; set; }
		public string RowVersion { get; set; }
		public int? EligibleCostId { get; set; }

		[Required(ErrorMessage = "Training Provider Name is required"), MaxLength(500)]
		public string Name { get; set; }
		public string ChangeRequestReason { get; set; }
		public int? GrantApplicationId { get; set; }
		public int? TrainingProgramId { get; set; }

		[Required(ErrorMessage = "Training Provider Type is required"), Range(1, int.MaxValue, ErrorMessage = "Training Provider Type is required")]
		public int? TrainingProviderTypeId { get; set; }

		public int? ProofOfInstructorQualifications { get; set; }
		public int? CourseOutline { get; set; }

		public int? TrainingAddressId { get; set; }

		public int? TrainingProviderAddressId { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; } = TrainingProviderStates.Incomplete;

		public int? TrainingProviderInventoryId { get; set; }
		public TrainingProviderInventory TrainingProviderInventory { get; set; }

		[Required(ErrorMessage = "Contact First Name is required")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Invalid Format")]
		[MaxLength(128)]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "Contact Last Name is required")]
		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		[MaxLength(128)]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "Email Address is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		[RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Email Address is not valid")]
		public string ContactEmail { get; set; }

		[Required(ErrorMessage = "Contact phone number is required")]
		[RegularExpression("^\\D*(\\d\\D*){10}", ErrorMessage = "Contact phone number must be 10-digit number")]
		public string ContactPhone { get; set; }
		public string ContactPhoneAreaCode { get; set; }
		public string ContactPhoneExchange { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "City must be entered in English without accents or special characters")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string City { get; set; }

		[Required(ErrorMessage = "Training Provider Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1TrainingProvider { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2TrainingProvider { get; set; }

		[Required(ErrorMessage = "Training Provider City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "Training Provider City must be entered in English without accents or special characters")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string CityTrainingProvider { get; set; }

		private string _postalCode;
		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
		public string PostalCode
		{
			get
			{
				var postalcode = _postalCode;
				if (!string.IsNullOrEmpty(postalcode))
				{
					postalcode = postalcode.ToUpper().Replace(" ", "");
					if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
					else postalcode = _postalCode; //no change
				}
				return postalcode;
			}
			set
			{
				_postalCode = value;
			}
		}

		private string _postalCodeTrainingProvider;
		[Required(ErrorMessage = "Training Provider Postal Code is required")]
		[RegularExpression(Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
		public string PostalCodeTrainingProvider
		{
			get
			{
				var postalcode = _postalCodeTrainingProvider;
				if (!string.IsNullOrEmpty(postalcode))
				{
					postalcode = postalcode.ToUpper().Replace(" ", "");
					if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
					else postalcode = _postalCodeTrainingProvider; //no change
				}
				return postalcode;
			}
			set
			{
				_postalCodeTrainingProvider = value;
			}
		}

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code is required for international addresses")]
		[DisplayName("Zip Code")]
		public string OtherZipCode { get; set; }

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code of the Training Provider is required for international addresses")]
		[DisplayName("Zip Code")]
		public string OtherZipCodeTrainingProvider { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string RegionId { get; set; } = "BC";
		public string Region { get; set; } = "British Columbia";

		[Required(ErrorMessage = "Province of the Training Provider is required")]
		public string RegionIdTrainingProvider { get; set; } = "BC";

		public string RegionTrainingProvider { get; set; } = "British Columbia";

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region is required for international addresses")]
		[RegularExpression(Constants.ProvinceNameValidationRegEx, ErrorMessage = "State / Region must be entered in English without accents or special characters")]
		[DisplayName("State / Region")]
		public string OtherRegion { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region of the Training Provider is required for international addresses")]
		[RegularExpression(Constants.ProvinceNameValidationRegEx, ErrorMessage = "Training Provider State / Region must be entered in English without accents or special characters")]
		[DisplayName("State / Region")]
		public string OtherRegionTrainingProvider { get; set; }

		[Required(ErrorMessage = "Country is required for international addresses")]
		[DefaultValue(Constants.CanadaCountryId)]
		[DisplayName("Country")]
		public string CountryId { get; set; } = Constants.CanadaCountryId;
		public string Country { get; set; } = "Canada";

		[Required(ErrorMessage = "Training Provider Country is required for international addresses")]
		[DefaultValue(Constants.CanadaCountryId)]
		[DisplayName("Country")]
		public string CountryIdTrainingProvider { get; set; } = Constants.CanadaCountryId;
		public string CountryTrainingProvider { get; set; } = "Canada";

		public bool IsCanadianAddress { get; set; } = true;
		public bool IsCanadianAddressTrainingProvider { get; set; } = true;

		public int? ParentProviderId { get; set; }
		public bool IsServiceProvider { get; set; }

		public string BusinessCase { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public TrainingProviderAttachmentViewModel ProofOfQualificationsDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.ProofOfQualifications);
		public TrainingProviderAttachmentViewModel BusinessCaseDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.BusinessCase);
		public TrainingProviderAttachmentViewModel CourseOutlineDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.CourseOutline);

		public int[] SelectedDeliveryMethodIds { get; set; }
		#endregion

		#region Constructors
		public TrainingProviderViewModel() { }

		public TrainingProviderViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			Id = trainingProvider.Id;
			RowVersion = trainingProvider.RowVersion != null ? Convert.ToBase64String(trainingProvider.RowVersion) : null;
			GrantApplicationId = trainingProvider.GrantApplicationId;
			var grantApplication = trainingProvider.GetGrantApplication();
			GrantApplicationRowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			TrainingProgramId = trainingProvider.TrainingPrograms.FirstOrDefault()?.Id;
			SelectedDeliveryMethodIds = trainingProvider.TrainingPrograms.FirstOrDefault()?.DeliveryMethods.Select(dm => dm.Id).ToArray();
			EligibleCostId = trainingProvider.EligibleCostId;
			Name = trainingProvider.Name;
			ChangeRequestReason = trainingProvider.ChangeRequestReason;
			TrainingProviderState = trainingProvider.TrainingProviderState;
			TrainingProviderTypeId = trainingProvider.Id == 0 ? (int?)null : trainingProvider.TrainingProviderTypeId;
			ContactPhone = trainingProvider.ContactPhoneNumber;
			ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber.GetPhoneAreaCode();
			ContactPhoneExchange = trainingProvider.ContactPhoneNumber.GetPhoneExchange();
			ContactPhoneNumber = trainingProvider.ContactPhoneNumber.GetPhoneNumber();
			ContactPhoneExtension = trainingProvider.ContactPhoneExtension;
			ContactEmail = trainingProvider.ContactEmail;
			ContactFirstName = trainingProvider.ContactFirstName;
			ContactLastName = trainingProvider.ContactLastName;
			TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			TrainingProviderInventory = trainingProvider.TrainingProviderInventory;
			TrainingAddressId = trainingProvider.TrainingAddressId;
			if (trainingProvider.TrainingAddress != null)
			{
				var address = trainingProvider.TrainingAddress;
				AddressLine1 = address.AddressLine1;
				AddressLine2 = address.AddressLine2;
				City = address.City;
				PostalCode = address.PostalCode;
				RegionId = address.RegionId;
				Region = address.Region?.Name;
				CountryId = address.CountryId;
				Country = address.Country?.Name;

				IsCanadianAddress = address.CountryId == Constants.CanadaCountryId;

				if (!IsCanadianAddress)
				{
					OtherRegion = Region;
					OtherZipCode = PostalCode;
				}
			}
			TrainingProviderAddressId = trainingProvider.TrainingProviderAddressId;
			if (trainingProvider.TrainingProviderAddress != null)
			{
				var addressTrainingProvider = trainingProvider.TrainingProviderAddress;
				AddressLine1TrainingProvider = addressTrainingProvider.AddressLine1;
				AddressLine2TrainingProvider = addressTrainingProvider.AddressLine2;
				CityTrainingProvider = addressTrainingProvider.City;
				PostalCodeTrainingProvider = addressTrainingProvider.PostalCode;
				RegionIdTrainingProvider = addressTrainingProvider.RegionId;
				RegionTrainingProvider = addressTrainingProvider.Region?.Name;
				CountryIdTrainingProvider = addressTrainingProvider.CountryId;
				CountryTrainingProvider = addressTrainingProvider.Country?.Name;

				IsCanadianAddressTrainingProvider = addressTrainingProvider.CountryId == Constants.CanadaCountryId;

				if (!IsCanadianAddressTrainingProvider)
				{
					OtherRegionTrainingProvider = RegionTrainingProvider;
					OtherZipCodeTrainingProvider = PostalCodeTrainingProvider;
				}
			}


			ParentProviderId = trainingProvider.OriginalTrainingProviderId;
			BusinessCase = trainingProvider.BusinessCase;
			PrivateSectorValidationType = trainingProvider.TrainingProviderType != null ? trainingProvider.TrainingProviderType.PrivateSectorValidationType : TrainingProviderPrivateSectorValidationTypes.Never;
			if (trainingProvider.TrainingProviderType != null)
			{
				ProofOfInstructorQualifications = trainingProvider.TrainingProviderType.ProofOfInstructorQualifications;
				CourseOutline = trainingProvider.TrainingProviderType.CourseOutline;
			}
			BusinessCaseDocument = new TrainingProviderAttachmentViewModel(trainingProvider.BusinessCaseDocument, Id, TrainingProviderAttachmentTypes.BusinessCase, RowVersion);
			ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument, Id, TrainingProviderAttachmentTypes.ProofOfQualifications, RowVersion);
			CourseOutlineDocument = new TrainingProviderAttachmentViewModel(trainingProvider.CourseOutlineDocument, Id, TrainingProviderAttachmentTypes.CourseOutline, RowVersion);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Map the model to the appropriate training provider.
		/// Also Add/Update the attachments associated with this training provider.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProvider MapProperties(
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService,
			IStaticDataService staticDataService,
			IPrincipal user,
			HttpPostedFileBase[] files = null)
		{
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (trainingProgramService == null) throw new ArgumentNullException(nameof(trainingProgramService));
			if (trainingProviderService == null) throw new ArgumentNullException(nameof(trainingProviderService));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var create = Id == 0;
			var grantApplication = GrantApplicationId.HasValue ? grantApplicationService.Get(GrantApplicationId.Value) : null;
			TrainingProvider trainingProvider;

			if (create)
			{
				if (ParentProviderId > 0)
				{
					// This is a change request if it's a new service provider.
					var parentProvider = trainingProviderService.Get(ParentProviderId.Value);
					trainingProvider = new TrainingProvider(parentProvider.OriginalTrainingProvider ?? parentProvider);

					parentProvider.TrainingProgram.TrainingProviders.Add(trainingProvider);
				}
				else if (TrainingProgramId.HasValue)
				{
					var trainingProgram = trainingProgramService.Get(TrainingProgramId.Value);
					trainingProvider = new TrainingProvider(trainingProgram);
				}
				else if (grantApplication?.TrainingPrograms.Any() ?? false)
				{
					var trainingProgram = grantApplication.TrainingPrograms.First();
					trainingProvider = new TrainingProvider(trainingProgram);
				}
				else
				{
					trainingProvider = new TrainingProvider(grantApplication);
				}
			}
			else
			{
				trainingProvider = trainingProviderService.Get(Id);
				grantApplication = trainingProvider.GetGrantApplication();
			}

			if (!create)
				trainingProvider.RowVersion = Convert.FromBase64String(RowVersion);

			trainingProvider.Name = Name;
			trainingProvider.ChangeRequestReason = ChangeRequestReason;
			trainingProvider.TrainingProviderTypeId = TrainingProviderTypeId.Value;

			if (!trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
			{
				trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			}
			if (trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
			{
				var associatedTrainingProgram = trainingProvider.TrainingPrograms.First();
				trainingProvider.TrainingProviderState = associatedTrainingProgram.TrainingProvider?.Id == trainingProvider.Id ? TrainingProviderStates.Complete : TrainingProviderStates.Requested;
			}

			trainingProvider.ContactPhoneNumber = ContactPhone;
			trainingProvider.ContactPhoneExtension = ContactPhoneExtension;
			trainingProvider.ContactEmail = ContactEmail;
			trainingProvider.ContactFirstName = ContactFirstName;
			trainingProvider.ContactLastName = ContactLastName;

			if (trainingProvider.TrainingProviderAddress == null)
				trainingProvider.TrainingProviderAddress = new ApplicationAddress();

			trainingProvider.TrainingProviderAddress.AddressLine1 = AddressLine1TrainingProvider;
			trainingProvider.TrainingProviderAddress.AddressLine2 = AddressLine2TrainingProvider;
			trainingProvider.TrainingProviderAddress.City = CityTrainingProvider;

			Region nonCanadianRegion = null;
			if (IsCanadianAddressTrainingProvider)
			{
				trainingProvider.TrainingProviderAddress.RegionId = RegionIdTrainingProvider;
			}
			else
			{
				nonCanadianRegion = applicationAddressService.VerifyOrCreateRegion(OtherRegionTrainingProvider, CountryIdTrainingProvider);
				trainingProvider.TrainingProviderAddress.RegionId = nonCanadianRegion.Id;
			}

			trainingProvider.TrainingProviderAddress.PostalCode = IsCanadianAddressTrainingProvider ? PostalCodeTrainingProvider : OtherZipCodeTrainingProvider;
			trainingProvider.TrainingProviderAddress.CountryId = CountryIdTrainingProvider;

			if (SelectedDeliveryMethodIds != null
			    && (SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom) || SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace)))
			{
				if (trainingProvider.TrainingAddress == null)
					trainingProvider.TrainingAddress = new ApplicationAddress();

				trainingProvider.TrainingAddress.AddressLine1 = AddressLine1;
				trainingProvider.TrainingAddress.AddressLine2 = AddressLine2;
				trainingProvider.TrainingAddress.City = City;

				if (IsCanadianAddress)
				{
					trainingProvider.TrainingAddress.RegionId = RegionId;
				}
				else
				{
					// We have to get the existing region if it's the same for both addresses, otherwise we get an EF error
					var region = GetInternationalRegion(CountryId, OtherRegion,
						CountryIdTrainingProvider, OtherRegionTrainingProvider,
						nonCanadianRegion, applicationAddressService);
					trainingProvider.TrainingAddress.RegionId = region.Id;
				}

				trainingProvider.TrainingAddress.PostalCode = IsCanadianAddress ? PostalCode : OtherZipCode;
				trainingProvider.TrainingAddress.CountryId = CountryId;
			}

			if (!trainingProvider.TrainingOutsideBC)
			{
				BusinessCaseDocument.Id = 0;
				BusinessCase = "";
			}


			trainingProvider.BusinessCase = BusinessCase;

			UpdateAttachments(trainingProvider, attachmentService, files);

			if ((grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
				|| grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotAccepted)
			{
				grantApplication.RowVersion = Convert.FromBase64String(GrantApplicationRowVersion);

				// This GrantApplication was withdrawn or returned as not accepted, and can now be editted for resubmission
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.Draft;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;

				// remove the filenumber
				grantApplication.FileNumber = string.Empty;

				// also remove the assigned assessor
				grantApplication.Assessor = null;
			}

			return trainingProvider;
		}

		private Region GetInternationalRegion(string targetCountry, string targetRegion, string sourceCountry, string sourceRegion, Region existingRegion, IApplicationAddressService applicationAddressService)
		{
			if (targetCountry == sourceCountry && targetRegion.Equals(sourceRegion, StringComparison.CurrentCultureIgnoreCase))
				return existingRegion;

			return applicationAddressService.VerifyOrCreateRegion(targetRegion, targetCountry);
		}

		/// <summary>
		/// Add/update/remove attachments associated with the specific properties of the training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="attachmentService"></param>
		/// <param name="files"></param>
		private void UpdateAttachments(TrainingProvider trainingProvider, IAttachmentService attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));

			// If files were provided add/update the attachments for the specified properties.
			if (files != null && files.Any())
			{
				if (CourseOutlineDocument != null && CourseOutlineDocument.Index.HasValue && files.Count() > CourseOutlineDocument.Index)
				{
					var attachment = files[CourseOutlineDocument.Index.Value].UploadFile(CourseOutlineDocument.Description, CourseOutlineDocument.FileName);
					attachment.Id = CourseOutlineDocument.Id;
					if (CourseOutlineDocument.Id == 0)
					{
						trainingProvider.CourseOutlineDocument = attachment;
						attachmentService.Add(attachment);
						CourseOutlineDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(CourseOutlineDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
				if (ProofOfQualificationsDocument != null && ProofOfQualificationsDocument.Index.HasValue && files.Count() > ProofOfQualificationsDocument.Index)
				{
					var attachment = files[ProofOfQualificationsDocument.Index.Value].UploadFile(ProofOfQualificationsDocument.Description, ProofOfQualificationsDocument.FileName);
					attachment.Id = ProofOfQualificationsDocument.Id;
					if (ProofOfQualificationsDocument.Id == 0)
					{
						trainingProvider.ProofOfQualificationsDocument = attachment;
						attachmentService.Add(attachment);
						ProofOfQualificationsDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(ProofOfQualificationsDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
				if (BusinessCaseDocument != null && BusinessCaseDocument.Index.HasValue && files.Count() > BusinessCaseDocument.Index)
				{
					var attachment = files[BusinessCaseDocument.Index.Value].UploadFile(BusinessCaseDocument.Description, BusinessCaseDocument.FileName);
					attachment.Id = BusinessCaseDocument.Id;
					if (BusinessCaseDocument.Id == 0)
					{
						trainingProvider.BusinessCaseDocument = attachment;
						attachmentService.Add(attachment);
						BusinessCaseDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(BusinessCaseDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
			}

			if (trainingProvider.BusinessCaseDocumentId.HasValue && trainingProvider.BusinessCaseDocumentId != BusinessCaseDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.BusinessCaseDocumentId.Value);
				trainingProvider.BusinessCaseDocumentId = null;
				attachmentService.Remove(attachment);
			}

			if (trainingProvider.ProofOfQualificationsDocumentId.HasValue && trainingProvider.ProofOfQualificationsDocumentId != ProofOfQualificationsDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.ProofOfQualificationsDocumentId.Value);
				trainingProvider.ProofOfQualificationsDocumentId = null;
				attachmentService.Remove(attachment);
			}

			if (trainingProvider.CourseOutlineDocumentId.HasValue && trainingProvider.CourseOutlineDocumentId != CourseOutlineDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.CourseOutlineDocumentId.Value);
				trainingProvider.CourseOutlineDocumentId = null;
				attachmentService.Remove(attachment);
			}

			trainingProvider.BusinessCaseDocumentId = trainingProvider.BusinessCaseDocument?.Id;
			trainingProvider.ProofOfQualificationsDocumentId = trainingProvider.ProofOfQualificationsDocument?.Id;
			trainingProvider.CourseOutlineDocumentId = trainingProvider.CourseOutlineDocument?.Id;
		}
		#endregion
	}
}