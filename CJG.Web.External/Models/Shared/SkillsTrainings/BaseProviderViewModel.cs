using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class BaseProviderViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int? EligibleCostId { get; set; }

		[Required(ErrorMessage = "Training Provider Name is required"), MaxLength(500)]
		public string Name { get; set; }
		public string ChangeRequestReason { get; set; }
		public int GrantApplicationId { get; set; }
		public int? TrainingProgramId { get; set; }

		[Required(ErrorMessage = "Training Provider Type is required"), Range(1, int.MaxValue, ErrorMessage = "Training Provider Type is required")]
		public int? TrainingProviderTypeId { get; set; }

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

		[Required(ErrorMessage = "Address Line 1 is required")]
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
		[Required(ErrorMessage = "Postal Code is required")]
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
		#endregion

		#region Constructors
		public BaseProviderViewModel()
		{

		}

		public BaseProviderViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.Id = trainingProvider.Id;
			this.RowVersion = trainingProvider.RowVersion != null ? Convert.ToBase64String(trainingProvider.RowVersion) : null;
			this.EligibleCostId = trainingProvider.EligibleCostId;
			this.Name = trainingProvider.Name;
			this.ChangeRequestReason = trainingProvider.ChangeRequestReason;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
			this.TrainingProviderTypeId = trainingProvider.TrainingProviderTypeId;
			this.ContactPhone = trainingProvider.ContactPhoneNumber;
			this.ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber.GetPhoneAreaCode();
			this.ContactPhoneExchange = trainingProvider.ContactPhoneNumber.GetPhoneExchange();
			this.ContactPhoneNumber = trainingProvider.ContactPhoneNumber.GetPhoneNumber();
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

				this.IsCanadianAddress = address.CountryId == Constants.CanadaCountryId;

				if (!IsCanadianAddress)
				{
					this.OtherRegion = Region;
					this.OtherZipCode = PostalCode;
				}
			}
			this.TrainingProviderAddressId = trainingProvider.TrainingProviderAddressId;
			if (trainingProvider.TrainingProviderAddress != null)
			{
				var addressTrainingProvider = trainingProvider.TrainingProviderAddress;
				this.AddressLine1TrainingProvider = addressTrainingProvider.AddressLine1;
				this.AddressLine2TrainingProvider = addressTrainingProvider.AddressLine2;
				this.CityTrainingProvider = addressTrainingProvider.City;
				this.PostalCodeTrainingProvider = addressTrainingProvider.PostalCode;
				this.RegionIdTrainingProvider = addressTrainingProvider.RegionId;
				this.RegionTrainingProvider = addressTrainingProvider.Region?.Name;
				this.CountryIdTrainingProvider = addressTrainingProvider.CountryId;
				this.CountryTrainingProvider = addressTrainingProvider.Country?.Name;

				this.IsCanadianAddressTrainingProvider = addressTrainingProvider.CountryId == Constants.CanadaCountryId;

				if (!IsCanadianAddressTrainingProvider)
				{
					this.OtherRegionTrainingProvider = RegionTrainingProvider;
					this.OtherZipCodeTrainingProvider = PostalCodeTrainingProvider;
				}
			}
		}
		#endregion
	}
}
