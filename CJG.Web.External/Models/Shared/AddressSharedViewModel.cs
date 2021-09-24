using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CJG.Web.External.Models.Shared
{
	public class AddressSharedViewModel: BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		[Required(ErrorMessage = "Address line 1 is required")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		public string AddressLine1 { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "City must be entered in English without accents or special characters")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		public string City { get; set; }

		private string _postalCode;

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[PostalCodeValidation(ErrorMessage = "A valid postal code is required for Canadian addresses")]
		[DisplayName("Postal code")]
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

		[RequiredForCanadaValidation(ErrorMessage = "Province is required for Canadian addresses")]
		[DisplayName("Province")]
		public string RegionId { get; set; } = "BC";

		[NotMapped]
		public string Region { get; set; } = "British Columbia";

		[RequiredForInternationalValidation(ErrorMessage = "Country is required for international addresses")]
		[DefaultValue(Constants.CanadaCountryId)]
		[DisplayName("Country")]
		public string CountryId { get; set; } = Constants.CanadaCountryId;

		[NotMapped]
		public string Country { get; set; } = "Canada";

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[RequiredForInternationalValidation(ErrorMessage = "Zip Code is required for international addresses")]
		[DisplayName("Zip Code")]
		public string OtherZipCode { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[RequiredForInternationalValidation(ErrorMessage = "State / Region is required for international addresses")]
		[DisplayName("State / Region")]
		public string OtherRegion { get; set; }

		public bool IsCanadianAddress { get; set; } = true;
		#endregion

		#region Constructors
		public AddressSharedViewModel()
		{
		}

		public AddressSharedViewModel(ApplicationAddress address)
		{
			this.Id = address.Id;
			this.RowVersion = address.RowVersion != null ? Convert.ToBase64String(address.RowVersion) : null;
			this.AddressLine1 = address.AddressLine1;
			this.AddressLine2 = address.AddressLine2;
			this.City = address.City;
			this.PostalCode = address.PostalCode;
			this.RegionId = address.RegionId;
			this.Region = address.Region?.Name;
			this.CountryId = address.CountryId;
			this.Country = address.Country?.Name;
			this.IsCanadianAddress = address.CountryId == Constants.CanadaCountryId;
			if (!this.IsCanadianAddress)
			{
				this.OtherRegion = Region;
				this.OtherZipCode = PostalCode;
			}
		}
		#endregion
	}

	public class PostalCodeValidationAttribute : ValidationAttribute, IClientValidatable
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (validationContext == null) return ValidationResult.Success;

			var address = validationContext.ObjectInstance as AddressSharedViewModel;

			if (address.IsCanadianAddress)
			{
				if (value == null)
				{
					return new ValidationResult($"{validationContext.DisplayName} is required for Canadian address");
				}
				else if ( !Regex.IsMatch(value.ToString(), Constants.PostalCodeValidationRegEx))
				{
					return new ValidationResult($"Invalid format for {validationContext.DisplayName}");
				}
			}

			return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			var rule = new ModelClientValidationRule();
			rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
			rule.ValidationParameters.Add("postalcoderegex", Constants.PostalCodeValidationRegEx);
			rule.ValidationType = "postalcodecheck";
			yield return rule;
		}
	}

	public class RequiredForCanadaValidationAttribute : ValidationAttribute, IClientValidatable
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (validationContext == null) return ValidationResult.Success;

			var address = validationContext.ObjectInstance as AddressSharedViewModel;

			if (address.IsCanadianAddress)
			{
				if (string.IsNullOrWhiteSpace(value.ToString()))
				{
					return new ValidationResult($"{validationContext.DisplayName} is required for Canadian address");
				}
			}

			return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			var rule = new ModelClientValidationRule();
			rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
			rule.ValidationType = "requiredforcanada";
			yield return rule;
		}
	}

	public class RequiredForInternationalValidationAttribute : ValidationAttribute, IClientValidatable
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (validationContext == null) return ValidationResult.Success;

			var address = validationContext.ObjectInstance as AddressSharedViewModel;

			if (!address.IsCanadianAddress)
			{
				if (string.IsNullOrWhiteSpace(value?.ToString()))
				{
					return new ValidationResult($"{validationContext.DisplayName} is required for international address");
				}
			}

			return ValidationResult.Success;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			var rule = new ModelClientValidationRule();
			rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
			rule.ValidationType = "requiredforinternational";
			yield return rule;
		}
	}
}