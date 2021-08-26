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
        public OverviewApplicantContact(GrantApplication grantApplication, User user)
        {
            this.ApplicantSalutation = grantApplication.ApplicantSalutation;
            this.ApplicantFirstName = grantApplication.ApplicantFirstName;
            this.ApplicantLastName = grantApplication.ApplicantLastName;
            this.ApplicantPhoneNumber = grantApplication.ApplicantPhoneNumber;
            this.ApplicantPhoneExtension = grantApplication.ApplicantPhoneExtension;
            this.ApplicantJobTitle = grantApplication.ApplicantJobTitle;

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

            if (user != null)
            {
                this.ApplicantEmail = user.EmailAddress;
            }

        }
    }
}