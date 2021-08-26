using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Application.Business.Models
{
	public class UserProfileDetailModel
	{
		#region Properties
		[Required(ErrorMessage = "Position/Title is required")]
		public string Title { get; set; }

		[NotMapped]
		[Required(ErrorMessage = "Preferred contact number is required")]
		[RegularExpression("^[0-9]{10}$", ErrorMessage = "Preferred contact number must be 10-digit number")]
		[ConvertMap(nameof(Phone), new[] { nameof(PhoneAreaCode), nameof(PhoneExchange), nameof(PhoneNumber) }, "{0}{1}{2}")]
		public string Phone { get; set; }

		public string PhoneAreaCode { get; set; }
		public string PhoneExchange { get; set; }
		public string PhoneNumber { get; set; }
		public string PhoneExtension { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string PhysicalAddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string PhysicalAddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression("^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "Invalid Format")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string PhysicalCity { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string PhysicalRegionId { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Provinces { get; set; }

		private string _physicalPostalCode;
		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(CJG.Core.Entities.Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
		public string PhysicalPostalCode
		{
			get
			{
				var postalcode = _physicalPostalCode;
				if (!string.IsNullOrEmpty(postalcode))
				{
					postalcode = postalcode.ToUpper().Replace(" ", "");
					if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
				}
				return postalcode;
			}
			set
			{
				_physicalPostalCode = value;
			}
		}

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string MailingAddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string MailingAddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression("^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "Invalid Format")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string MailingCity { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string MailingRegionId { get; set; }

		private string _mailingPostalCode;
		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(CJG.Core.Entities.Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
		public string MailingPostalCode
		{
			get
			{
				var postalcode = _mailingPostalCode;
				if (!string.IsNullOrEmpty(postalcode))
				{
					postalcode = postalcode.ToUpper().Replace(" ", "");
					if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
				}
				return postalcode;
			}
			set
			{
				_mailingPostalCode = value;
			}
		}

		public bool MailingAddressSameAsBusiness { get; set; }
		public bool Subscribe { get; set; }
		public string PhysicalCountryId { get; set; } = "CA";
		public string MailingCountryId { get; set; } = "CA";
		#endregion

		#region Constructors
		public UserProfileDetailModel() { }

		public UserProfileDetailModel(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));


			this.Title = user.JobTitle;
			user.PhoneNumber = $"({this.PhoneAreaCode}) {this.PhoneExchange}-{this.PhoneNumber}";
			this.PhoneExtension = user.PhoneExtension;
			this.Subscribe = user.IsSubscriberToEmail;

			this.PhysicalAddressLine1 = user.PhysicalAddress.AddressLine1;
			this.PhysicalAddressLine2 = user.PhysicalAddress.AddressLine1;
			this.PhysicalCity = user.PhysicalAddress.City;
			this.PhysicalPostalCode = user.PhysicalAddress.PostalCode;
			this.PhysicalRegionId = user.PhysicalAddress.RegionId;
			this.PhysicalCountryId = user.PhysicalAddress.CountryId;

			if (user.PhysicalAddressId != user.MailingAddressId)
			{
				this.MailingAddressLine1 = user.MailingAddress.AddressLine1;
				this.MailingAddressLine2 = user.MailingAddress.AddressLine1;
				this.MailingCity = user.MailingAddress.City;
				this.MailingPostalCode = user.MailingAddress.PostalCode;
				this.MailingRegionId = user.MailingAddress.RegionId;
				this.MailingCountryId = user.MailingAddress.CountryId;
			}
			else
			{
				this.MailingAddressSameAsBusiness = true;
			}
		}
		#endregion

		#region Methods
		public void BindBusinessUserToEntity(User user)
		{
			user.JobTitle = this.Title;
			user.PhoneNumber = $"({this.PhoneAreaCode}) {this.PhoneExchange}-{this.PhoneNumber}";
			user.PhoneExtension = this.PhoneExtension;
			user.IsSubscriberToEmail = this.Subscribe;
			user.PhysicalAddress = new Address(this.PhysicalAddressLine1,
											   this.PhysicalAddressLine2,
											   this.PhysicalCity,
											   this.PhysicalPostalCode,
											   this.PhysicalRegionId,
											   this.PhysicalCountryId);
			if (!this.MailingAddressSameAsBusiness)
			{
				user.MailingAddress = new Address(this.MailingAddressLine1,
												  this.MailingAddressLine2,
												  this.MailingCity,
												  this.MailingPostalCode,
												  this.MailingRegionId,
												  this.MailingCountryId);
			}
			else
			{
				user.MailingAddress = user.PhysicalAddress;
			}
		}
		#endregion
	}
}
