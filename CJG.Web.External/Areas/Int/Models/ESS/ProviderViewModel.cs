using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Int.Models.ESS
{
	public class ProviderViewModel : BaseViewModel
	{
		#region Properties
		public bool IsValidated { get; set; }
		public bool CanValidate { get; set; }
		public bool CanEdit { get; set; }
		public bool CanRemove { get; set; }
		public bool CanRecommendChangeRequest { get; set; }

		public string RowVersion { get; set; }
		public int? GrantApplicationId { get; set; }
		public int? TrainingProgramId { get; set; }
		public int? EligibleCostId { get; set; }

		[Required(ErrorMessage = "Training Provider Name is required"), MaxLength(500)]
		public string Name { get; set; }

		public string ChangeRequestReason { get; set; }

		[Required(ErrorMessage = "Training Provider Type is required")]
		public int? TrainingProviderTypeId { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; } = TrainingProviderStates.Incomplete;
		public int? TrainingProviderInventoryId { get; set; }
		public TrainingProviderInventory TrainingProviderInventory { get; set; }

		[Required(ErrorMessage = "Contact First Name is required")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Contact First Name Invalid Format")]
		[MaxLength(128)]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "Contact Last Name is required")]
		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Contact Last Name Invalid Format")]
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

		public int? TrainingAddressId { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "City must be entered in English without accents or special characters")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string City { get; set; }

		private string _postalCode;
		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(Constants.PostalCodeValidationRegEx, ErrorMessage = "PostalCode Invalid Format")]
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

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code is required for international addresses")]
		[DisplayName("Zip Code")]
		public string OtherZipCode { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string RegionId { get; set; }
		public string Region { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region is required for international addresses")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "State / Region must be entered in English without accents or special characters")]
		[DisplayName("State / Region")]
		public string OtherRegion { get; set; }

		[Required(ErrorMessage = "Country is required for international addresses")]
		[DefaultValue(Core.Entities.Constants.CanadaCountryId)]
		[DisplayName("Country")]
		public string CountryId { get; set; } = Constants.CanadaCountryId;
		public string Country { get; set; } = "Canada";
		public bool IsCanadianAddress { get; set; } = true;
		public int? BusinessCaseDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel BusinessCaseDocument { get; set; }

		public int? CourseOutlineDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel CourseOutlineDocument { get; set; }

		public int? ProofOfQualificationsDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel ProofOfQualificationsDocument { get; set; }
		#endregion

		#region Constructors
		public ProviderViewModel() { }

		public ProviderViewModel(TrainingProvider trainingProvider, IPrincipal user)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProvider.Id;
			this.RowVersion = trainingProvider.RowVersion != null ? Convert.ToBase64String(trainingProvider.RowVersion) : null;
			this.GrantApplicationId = trainingProvider.GrantApplicationId;
			this.TrainingProgramId = trainingProvider.TrainingProgram?.Id;
			this.EligibleCostId = trainingProvider.EligibleCostId;

			this.Name = trainingProvider.Name;
			this.ChangeRequestReason = trainingProvider.ChangeRequestReason;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
			this.PrivateSectorValidationType = trainingProvider.TrainingProviderType.PrivateSectorValidationType;
			this.TrainingProviderTypeId = trainingProvider.TrainingProviderTypeId;
			this.ContactPhone = trainingProvider.ContactPhoneNumber;
			this.ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber.GetPhoneAreaCode()?.ToString();
			this.ContactPhoneExchange = trainingProvider.ContactPhoneNumber.GetPhoneExchange()?.ToString();
			this.ContactPhoneNumber = trainingProvider.ContactPhoneNumber.GetPhoneNumber()?.ToString();
			this.ContactPhoneExtension = trainingProvider.ContactPhoneExtension;
			this.ContactEmail = trainingProvider.ContactEmail;
			this.ContactFirstName = trainingProvider.ContactFirstName;
			this.ContactLastName = trainingProvider.ContactLastName;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			this.TrainingProviderInventory = trainingProvider.TrainingProviderInventory;
			this.TrainingAddressId = trainingProvider.TrainingAddressId;
			if (trainingProvider.TrainingAddress != null)
			{
				var address = trainingProvider.TrainingAddress;
				this.AddressLine1 = address.AddressLine1;
				this.AddressLine2 = address.AddressLine2;
				this.City = address.City;
				this.PostalCode = address.PostalCode;
				this.RegionId = address.RegionId;
				this.Region = address.Region?.Name;
				this.CountryId = address.CountryId;
				this.Country = address.Country?.Name;

				this.IsCanadianAddress = address.CountryId == Core.Entities.Constants.CanadaCountryId;

				if (!IsCanadianAddress)
				{
					this.OtherRegion = Region;
					this.OtherZipCode = PostalCode;
				}
			}

			this.IsValidated = this.TrainingProviderInventoryId != null;
			this.CanValidate = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);
			this.CanEdit = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.CanRemove = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
			this.CanRecommendChangeRequest = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);

			this.BusinessCaseDocumentId = trainingProvider.BusinessCaseDocumentId;
			this.BusinessCaseDocument = trainingProvider.BusinessCaseDocumentId != null ? new Attachments.UpdateAttachmentViewModel(trainingProvider.BusinessCaseDocument) : null;
			this.CourseOutlineDocumentId = trainingProvider.CourseOutlineDocumentId;
			this.CourseOutlineDocument = trainingProvider.CourseOutlineDocumentId != null ? new Attachments.UpdateAttachmentViewModel(trainingProvider.CourseOutlineDocument) : null;
			this.ProofOfQualificationsDocumentId = trainingProvider.ProofOfQualificationsDocumentId;
			this.ProofOfQualificationsDocument = trainingProvider.ProofOfQualificationsDocumentId != null ? new Attachments.UpdateAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument) : null;
		}
		#endregion

		#region Methods
		public static explicit operator ServiceProviderDetailsViewModel(ProviderViewModel model)
		{
			var result = new ServiceProviderDetailsViewModel();
			Utilities.MapProperties(model, result);
			return result;
		}

		public static explicit operator TrainingProvider(ProviderViewModel model)
		{
			var trainingProvider = new TrainingProvider();
			Utilities.MapProperties(model, trainingProvider);
			trainingProvider.TrainingAddress = new ApplicationAddress();
			Utilities.MapProperties(model, trainingProvider.TrainingAddress);
			return trainingProvider;
		}
		#endregion
	}
}