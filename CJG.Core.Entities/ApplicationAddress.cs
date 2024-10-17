using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ApplicationAddress"/> class, provides the ORM a way to manage address information for grant applications.
	/// All addresses need to be captured at the instant in time the contractual documents are created.
	/// </summary>
	public class ApplicationAddress : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Address line one.
		/// </summary>
		[Required(ErrorMessage = "Address line 1 is required"), MaxLength(250)]
		public string AddressLine1 { get; set; }

		/// <summary>
		/// get/set - Address line two.
		/// </summary>
		[MaxLength(250)]
		public string AddressLine2 { get; set; }

		/// <summary>
		/// get/set - The city.
		/// </summary>
		[Required(ErrorMessage = "City name is required"), MaxLength(250)]
		public string City { get; set; }

		/// <summary>
		/// get/set - The postal code.
		/// </summary>
		[Required(ErrorMessage = "Postal code is required"), MaxLength(10)]
		[CustomValidation(typeof(ApplicationAddress), "CountryPostalCodeValidator", ErrorMessage = "Invalid postal or ZIP code.")]
		[RegularExpression(Constants.InternationalPostalCodeValidationRegEx, ErrorMessage = "Only letters and numbers can be used for Postal Code")]
		public string PostalCode { get; set; }

		/// <summary>
		/// get/set - The foreign key to the region.
		/// </summary>
		[Required, MaxLength(10)]
		[ForeignKey(nameof(Region)), Column(Order = 1)]
		public string RegionId { get; set; }

		/// <summary>
		/// get/set - The region (province/state)
		/// </summary>
		public virtual Region Region { get; set; }

		/// <summary>
		/// get/set - The foreign key to the country.
		/// </summary>
		[Required, MaxLength(20)]
		[ForeignKey(nameof(Region)), Column(Order = 2)]
		public string CountryId { get; set; }

		/// <summary>
		/// get/set - The country.
		/// </summary>
		[ForeignKey(nameof(CountryId))]
		public virtual Country Country { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationAddress"/> object.
		/// </summary>
		public ApplicationAddress()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationAddress"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="address"></param>
		public ApplicationAddress(Address address)
		{
			if (address != null)
				CopyAddress(address);
		}
		public ApplicationAddress(ApplicationAddress address)
		{
			if (address != null)
				CopyAddress(address);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Copy the address properties into this application address object.
		/// </summary>
		/// <param name="address"><typeparamref name="Address"/> object to copy from.</param>
		public void CopyAddress(Address address)
		{
			if (address == null)
				throw new ArgumentNullException(nameof(address));

			AddressLine1 = address.AddressLine1;
			AddressLine2 = address.AddressLine2;
			City = address.City;
			Region = address.Region;
			RegionId = address.RegionId;
			PostalCode = address.PostalCode;
			Country = address.Country;
			CountryId = address.CountryId;
		}
		/// <summary>
		/// Copy the address properties into this application address object.
		/// </summary>
		/// <param name="address"><typeparamref name="Address"/> object to copy from.</param>
		public void CopyAddress(ApplicationAddress address)
		{
			if (address == null)
				throw new ArgumentNullException(nameof(address));

			AddressLine1 = address.AddressLine1;
			AddressLine2 = address.AddressLine2;
			City = address.City;
			Region = address.Region;
			RegionId = address.RegionId;
			PostalCode = address.PostalCode;
			Country = address.Country;
			CountryId = address.CountryId;
		}
		/// <summary>
		/// Outputs the address in a formatted string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return
				AddressLine1 + Environment.NewLine +
				(string.IsNullOrWhiteSpace(AddressLine2) ? string.Empty : AddressLine2 + Environment.NewLine) +
				(string.IsNullOrWhiteSpace(City) ? string.Empty : City + Environment.NewLine) +
				(Region == null ? string.Empty : Region.Name + Environment.NewLine) +
				(string.IsNullOrWhiteSpace(PostalCode) ? string.Empty : PostalCode + Environment.NewLine) + 
				Country?.Name;
		}

		/// <summary>
		/// Validate the postal code.
		/// </summary>
		/// <param name="postalCode"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static ValidationResult CountryPostalCodeValidator(string postalCode, ValidationContext context)
		{
			if (context == null) return ValidationResult.Success;
			if (postalCode == null) return new ValidationResult($"Invalid format for {context.DisplayName}");

			var address = context.ObjectInstance as ApplicationAddress;
			if (address == null)
			{
				throw new ApplicationException(
					$"{nameof(CountryPostalCodeValidator)} can be applied only to {nameof(ApplicationAddress)} instance");

			}

			return !IsPostalCodeValid(address)
				? new ValidationResult($"Invalid format for {context.DisplayName}")
				: ValidationResult.Success;
		}

		/// <summary>
		/// Is the PostalCode for Canada Valid. Bypasses check if not Canada.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public static bool IsPostalCodeValid(ApplicationAddress address)
		{
			var isCanada = address.CountryId == Constants.CanadaCountryId;

			if (isCanada)
				return Regex.IsMatch(address.PostalCode, Constants.PostalCodeValidationRegEx);

			return true;
		}

		/// <summary>
		/// Quick-and-dirty check to see if the addresses are the same
		/// </summary>
		/// <returns></returns>
		public bool IsSameAddress(ApplicationAddress address)
		{
			if (address == null) {
				return false;
			}

			return AddressLine1 == address.AddressLine1
			       && AddressLine2 == address.AddressLine2
			       && City == address.City
			       && Region == address.Region
			       && PostalCode == address.PostalCode
			       && Country == address.Country;
		}
		#endregion
	}
}
