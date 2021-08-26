using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="Address"/> class, provides the ORM with a way to manage address information for users and organizations.
    /// </summary>
    public class Address : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The address line one.
        /// </summary>
        [Required, MaxLength(250)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// get/set The address line two.
        /// </summary>
        [MaxLength(250)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// get/set - The city.
        /// </summary>
        [Required, MaxLength(250)]
        public string City { get; set; }

        /// <summary>
        /// get/set - The postal code.
        /// </summary>
        [Required, MaxLength(20)]
        [RegularExpression(Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
        public string PostalCode { get; set; }

        /// <summary>
        /// get/set - The foreign key to the region (province/state)
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
        [Required, MaxLength(20), DefaultValue("CA")]
        [ForeignKey(nameof(Region)), Column(Order = 2)]
        public string CountryId { get; set; } = "CA";

        /// <summary>
        /// get/set - The country.
        /// </summary>
        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="Address"/> object.
        /// </summary>
        public Address()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Address"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="city"></param>
        /// <param name="postalCode"></param>
        /// <param name="region"></param>
        public Address(string addressLine1, string addressLine2, string city, string postalCode, Region region)
        {
            if (String.IsNullOrEmpty(addressLine1))
                throw new ArgumentException("Address line 1 must not be null or empty.", nameof(addressLine1));

            if (String.IsNullOrEmpty(city))
                throw new ArgumentException("City must not be null or empty.", nameof(city));

            if (String.IsNullOrEmpty(postalCode))
                throw new ArgumentException("Postal Code (or ZIP) must not be null or empty.", nameof(postalCode));

            if (region == null)
                throw new ArgumentNullException("Province (or State) must not be null or empty.", nameof(region));

            if (region.Country == null)
                throw new ArgumentException("Country must not be null or empty.", nameof(region.Country));

            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.City = city;
            this.PostalCode = postalCode;
            this.Region = region;
            this.RegionId = region.Id;
            this.Country = region.Country;
            this.CountryId = region.CountryId;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Address"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="city"></param>
        /// <param name="postalCode"></param>
        /// <param name="regionId"></param>
        /// <param name="countryId"></param>
        public Address(string addressLine1, string addressLine2, string city, string postalCode, string regionId, string countryId)
        {
            if (String.IsNullOrEmpty(addressLine1))
                throw new ArgumentException("Address line 1 must not be null or empty.", nameof(addressLine1));

            if (String.IsNullOrEmpty(city))
                throw new ArgumentException("City must not be null or empty.", nameof(city));

            if (String.IsNullOrEmpty(postalCode))
                throw new ArgumentException("Postal Code (or ZIP) must not be null or empty.", nameof(postalCode));

            if (String.IsNullOrEmpty(regionId))
                throw new ArgumentNullException("Province (or State) must not be null or empty.", nameof(regionId));

            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentException("Country must not be null or empty.", nameof(countryId));

            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.City = city;
            this.PostalCode = postalCode;
            this.RegionId = regionId;
            this.CountryId = countryId;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a formated address.
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
                this.Country.Name;
        }
        #endregion
    }
}
