using CJG.Core.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class AddressViewModel
    {
        #region Properties
        [Required(ErrorMessage = "Address is required")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province/State/Region is required")]
        [DefaultValue("BC")]
        public string RegionId { get; set; } = "BC";

        [NotMapped]
        [DefaultValue("British Columbia")]
        public string Region { get; set; } = "British Columbia";

        [RegularExpression(CJG.Core.Entities.Constants.PostalCodeValidationRegEx, ErrorMessage = "Postal Code Invalid Format")]
        [Required(ErrorMessage = "Postal Code is required")]
        public string PostalCode { get; set; }

        [DefaultValue("CA")]
        public string CountryId { get; set; } = "CA";

        [NotMapped]
        [DefaultValue("Canada")]
        public string Country { get; set; } = "Canada";

        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        public AddressViewModel()
        {
        }

        public AddressViewModel(ApplicationAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            this.AddressLine1 = address.AddressLine1;
            this.AddressLine2 = address.AddressLine2;
            this.City = address.City;
            this.RegionId = address.RegionId;
            this.Region = address.Region?.Name;
            this.PostalCode = address.PostalCode;
            this.CountryId = address.CountryId;
            this.Country = address.Country?.Name;
            this.RowVersion = address.RowVersion;
        }

        public AddressViewModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            this.AddressLine1 = address.AddressLine1;
            this.AddressLine2 = address.AddressLine2;
            this.City = address.City;
            this.RegionId = address.RegionId;
            this.Region = address.Region?.Name;
            this.PostalCode = address.PostalCode;
            this.CountryId = address.CountryId;
            this.Country = address.Country?.Name;
            this.RowVersion = address.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator Address(AddressViewModel model)
        {
            var address = new Address();
            address.AddressLine1 = model.AddressLine1;
            address.AddressLine2 = model.AddressLine2;
            address.City = model.City;
            address.RegionId = model.RegionId;
            address.PostalCode = model.PostalCode;
            address.CountryId = model.CountryId;
            address.RowVersion = model.RowVersion;
            return address;
        }

        public override string ToString()
        {
            var addressLine2 = !string.IsNullOrEmpty(this.AddressLine2) ? $"{this.AddressLine2}<br />" : string.Empty;
            var region = !string.IsNullOrEmpty(this.Region) ? $"{this.Region}<br />" : string.Empty;
            var country = !string.IsNullOrEmpty(this.Country) ? $"{this.Country}<br />" : string.Empty;

            return $"{this.AddressLine1}<br />{addressLine2}{City}<br />{region}{PostalCode}<br />{country}";
        }
        #endregion

    }
}