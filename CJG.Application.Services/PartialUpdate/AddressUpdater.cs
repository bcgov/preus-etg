using CJG.Core.Entities;
using System;
using System.Text;

namespace CJG.Application.Services.PartialUpdate
{
    public class AddressFields
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string RegionId { get; set; }
        public Region Region { get; set; }
        public string CountryId { get; set; }
        public Country Country { get; set; }
        public string PostalCode { get; set; }
        public bool IsEmptyAddress
        { get; set; }
    }

    internal class AddressUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, AddressFields> _setValue;
        private readonly Func<string, string, Region> _getRegionById;
        private readonly Func<string, Country> _getCountryById;
        private AddressFields _address;

        public AddressUpdater(string systemNoteText, Action<TEntity, AddressFields> setValue, 
            Func<TEntity, string> convertEntityToText, Func<string, string, Region> getRegionById,
            Func<string, Country> getCountryById,
            Action<TEntity> ensureExists = null) : base(systemNoteText, convertEntityToText, ensureExists)
        {
            _setValue = setValue;
            _getRegionById = getRegionById;
            _getCountryById = getCountryById;
        }
        
        public override void UpdateValue(TEntity entity, string[] arr)
        {
            _address = GetOrCreateCachedAddress(arr);
            
            ValidateAddressFields(_address);

            _setValue(entity, _address);
        }

        private void ValidateAddressFields(AddressFields address)
        {
            string invalidAddressField = null;

            if (address.IsEmptyAddress) return;

            if (string.IsNullOrWhiteSpace(address.AddressLine1))
            {
                invalidAddressField = "Address line 1";
            }

            if (invalidAddressField == null && string.IsNullOrWhiteSpace(address.City))
            {
                invalidAddressField = "City";
            }

            if (invalidAddressField == null && string.IsNullOrWhiteSpace(address.PostalCode))
            {
                invalidAddressField = "Postal code";
            }

            if (invalidAddressField == null && string.IsNullOrWhiteSpace(address.CountryId))
            {
                invalidAddressField = "Country";
            }

            if (invalidAddressField == null && address.Region == null)
            {
                invalidAddressField = "Region";
            }

            if (invalidAddressField != null)
            {
                throw new ApplicationException($"{invalidAddressField} is required");
            }
        }

        public override string GetNewValueText(string[] arr)
        {
            return ConvertAddressToFormattedText(GetOrCreateCachedAddress(arr));
        }

        internal AddressFields GetOrCreateCachedAddress(string[] arr)
        {
            if (_address != null) return _address;

            if (arr.Length > 6 && bool.Parse(arr[6]))
            {
                _address = new AddressFields { IsEmptyAddress = true };
            }
            else
            {
                var country = _getCountryById(arr[5]);

                var region = _getRegionById(arr[5], arr[3]);

                _address = new AddressFields
                {
                    AddressLine1 = arr[0],
                    AddressLine2 = arr[1],
                    City = arr[2],
                    RegionId = region.Id,
                    Region = region,
                    PostalCode = country.Id == "CA" ? Utilities.FormatCanadianPostalCode(arr[4]) : arr[4],
                    CountryId = country.Id,
                    Country = country,
                    IsEmptyAddress = false
                };
            }

            return _address;
        }

        internal static string ConvertAddressToFormattedText(ApplicationAddress applicationAddress)
        {
            return ConvertAddressToFormattedText(ConvertApplicationAddressToAddressFields(applicationAddress));
        }

        private static AddressFields ConvertApplicationAddressToAddressFields(ApplicationAddress applicationAddress)
        {
            return new AddressFields()
            {
                AddressLine1 = applicationAddress.AddressLine1,
                AddressLine2 = applicationAddress.AddressLine2,
                City = applicationAddress.City,
                RegionId = applicationAddress.Region.Id,
                Region = applicationAddress.Region,
                PostalCode = applicationAddress.PostalCode,
                CountryId = applicationAddress.CountryId,
                Country = applicationAddress.Country
            };
        }

        internal static string ConvertAddressToFormattedText(AddressFields applicationAddress)
        {
            if (applicationAddress == null || applicationAddress.IsEmptyAddress)
            {
                return "None";
            }

            var str = new StringBuilder();

            str.AppendLine();

            str.Append(applicationAddress.AddressLine1);

            if (string.IsNullOrWhiteSpace(applicationAddress.AddressLine2))
            {
                str.Append(' ');
                str.Append(applicationAddress.AddressLine1);
            }

            str.AppendLine();
            str.Append(applicationAddress.City);

            str.Append(' ');
            str.Append(applicationAddress.Region.Name);

            str.Append(' ');
            str.Append(applicationAddress.PostalCode);

            str.Append(' ');
            str.Append(applicationAddress.Country.Name);

            str.AppendLine();

            return str.ToString() ;
        }
    }
}