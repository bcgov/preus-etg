using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewApplicantContact
	{
		public string ApplicantSalutation { get; set; }
		public string ApplicantFirstName { get; set; }
		public string ApplicantLastName { get; set; }
		public string ApplicantEmail { get; set; }
		public string ApplicantPhoneNumber { get; set; }
		public string ApplicantPhoneExtension { get; set; }
		public string ApplicantJobTitle { get; set; }
		public bool HasPhysicalAddress { get; set; }
		public string ApplicantPhysicalAddressLine1 { get; set; }
		public string ApplicantPhysicalAddressLine2 { get; set; }
		public string ApplicantPhysicalCity { get; set; }
		public string ApplicantPhysicalRegion { get; set; }
		public string ApplicantPhysicalPostalCode { get; set; }
		public string ApplicantPhysicalCountry { get; set; }
		public bool HasMailingAddress { get; set; }
		public string ApplicantMailingAddressLine1 { get; set; }
		public string ApplicantMailingAddressLine2 { get; set; }
		public string ApplicantMailingCity { get; set; }
		public string ApplicantMailingRegion { get; set; }
		public string ApplicantMailingPostalCode { get; set; }
		public string ApplicantMailingCountry { get; set; }
		public bool MailingAddressSameAsPhysical { get; set; }
		public bool IsAlternateContact { get; set; }

		public string AlternateSalutation { get; set; }
		public string AlternateFirstName { get; set; }
		public string AlternateLastName { get; set; }
		public string AlternateEmail { get; set; }
		public string AlternatePhoneNumber { get; set; }
		public string AlternatePhoneExtension { get; set; }
		public string AlternateJobTitle { get; set; }
		public bool HasAlternatePhysicalAddress { get; set; }
		public string AlternatePhysicalAddressLine1 { get; set; }
		public string AlternatePhysicalAddressLine2 { get; set; }
		public string AlternatePhysicalCity { get; set; }
		public string AlternatePhysicalRegion { get; set; }
		public string AlternatePhysicalPostalCode { get; set; }
		public string AlternatePhysicalCountry { get; set; }
		public bool HasAlternateMailingAddress { get; set; }
		public string AlternateMailingAddressLine1 { get; set; }
		public string AlternateMailingAddressLine2 { get; set; }
		public string AlternateMailingCity { get; set; }
		public string AlternateMailingRegion { get; set; }
		public string AlternateMailingPostalCode { get; set; }
		public string AlternateMailingCountry { get; set; }
		public bool AlternateMailingAddressSameAsPhysical { get; set; }

		public OverviewApplicantContact(GrantApplication grantApplication, User user)
		{
			this.ApplicantSalutation = grantApplication.ApplicantSalutation;
			this.ApplicantFirstName = grantApplication.ApplicantFirstName;
			this.ApplicantLastName = grantApplication.ApplicantLastName;
			this.ApplicantPhoneNumber = grantApplication.ApplicantPhoneNumber;
			this.ApplicantPhoneExtension = grantApplication.ApplicantPhoneExtension;
			this.ApplicantJobTitle = grantApplication.ApplicantJobTitle;


			this.HasPhysicalAddress = grantApplication.ApplicantPhysicalAddress.Id > 0;

			this.IsAlternateContact = grantApplication.IsAlternateContact == null ? false : grantApplication.IsAlternateContact.Value;
			this.AlternateSalutation = grantApplication.AlternateSalutation;
			this.AlternateFirstName = grantApplication.AlternateFirstName;
			this.AlternateLastName = grantApplication.AlternateLastName;
			this.AlternatePhoneNumber = grantApplication.AlternatePhoneNumber;
			this.AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
			this.AlternateJobTitle = grantApplication.AlternateJobTitle;
			this.AlternateEmail = grantApplication.AlternateEmail;

			this.HasPhysicalAddress = grantApplication.ApplicantPhysicalAddress.Id > 0;

			if (grantApplication.ApplicantPhysicalAddress != null)
			{
				this.ApplicantPhysicalAddressLine1 = grantApplication.ApplicantPhysicalAddress.AddressLine1;
				this.ApplicantPhysicalAddressLine2 = grantApplication.ApplicantPhysicalAddress.AddressLine2;
				this.ApplicantPhysicalCity = grantApplication.ApplicantPhysicalAddress.City;
				this.ApplicantPhysicalRegion = grantApplication.ApplicantPhysicalAddress.Region == null ? string.Empty : grantApplication.ApplicantPhysicalAddress.Region.Name;
				this.ApplicantPhysicalPostalCode = grantApplication.ApplicantPhysicalAddress.PostalCode;
				this.ApplicantPhysicalCountry = grantApplication.ApplicantPhysicalAddress.Country.Name;
			}
			this.MailingAddressSameAsPhysical = grantApplication.ApplicantPhysicalAddressId == grantApplication.ApplicantMailingAddressId;

			this.HasMailingAddress = grantApplication.ApplicantMailingAddressId > 0;
			if (!this.MailingAddressSameAsPhysical && grantApplication.ApplicantMailingAddress != null)
			{
				this.ApplicantMailingAddressLine1 = grantApplication.ApplicantMailingAddress.AddressLine1;
				this.ApplicantMailingAddressLine2 = grantApplication.ApplicantMailingAddress.AddressLine2;
				this.ApplicantMailingCity = grantApplication.ApplicantMailingAddress.City;
				this.ApplicantMailingRegion = grantApplication.ApplicantMailingAddress.Region == null ? string.Empty : grantApplication.ApplicantMailingAddress.Region.Name;
				this.ApplicantMailingPostalCode = grantApplication.ApplicantMailingAddress.PostalCode;
				this.ApplicantMailingCountry = grantApplication.ApplicantMailingAddress.Country.Name;
			}

			//if (grantApplication.AlternatePhysicalAddress != null)
			//{
			//	this.AlternatePhysicalAddressLine1 = grantApplication.AlternatePhysicalAddress.AddressLine1;
			//	this.AlternatePhysicalAddressLine2 = grantApplication.AlternatePhysicalAddress.AddressLine2;
			//	this.AlternatePhysicalCity = grantApplication.AlternatePhysicalAddress.City;
			//	this.AlternatePhysicalRegion = grantApplication.AlternatePhysicalAddress.Region == null ? string.Empty : grantApplication.AlternatePhysicalAddress.Region.Name;
			//	this.AlternatePhysicalPostalCode = grantApplication.AlternatePhysicalAddress.PostalCode;
			//	this.AlternatePhysicalCountry = grantApplication.AlternatePhysicalAddress.Country.Name;
			//}
			//this.MailingAddressSameAsPhysical = grantApplication.AlternatePhysicalAddressId == grantApplication.AlternateMailingAddressId;

			//this.HasMailingAddress = grantApplication.AlternateMailingAddressId > 0;
			//if (!this.MailingAddressSameAsPhysical && grantApplication.AlternateMailingAddress != null)
			//{
			//	this.AlternateMailingAddressLine1 = grantApplication.AlternateMailingAddress.AddressLine1;
			//	this.AlternateMailingAddressLine2 = grantApplication.AlternateMailingAddress.AddressLine2;
			//	this.AlternateMailingCity = grantApplication.AlternateMailingAddress.City;
			//	this.AlternateMailingRegion = grantApplication.AlternateMailingAddress.Region == null ? string.Empty : grantApplication.AlternateMailingAddress.Region.Name;
			//	this.AlternateMailingPostalCode = grantApplication.AlternateMailingAddress.PostalCode;
			//	this.AlternateMailingCountry = grantApplication.AlternateMailingAddress.Country.Name;
			//}

			if (user != null)
			{
				this.ApplicantEmail = user.EmailAddress;
			}
		}
	}
}