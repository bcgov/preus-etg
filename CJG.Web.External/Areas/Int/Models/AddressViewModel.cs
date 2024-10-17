using System;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class AddressViewModel: AddressSharedViewModel
	{
		public AddressViewModel() { }

		public AddressViewModel(ApplicationAddress address) : base(address) { }

		public AddressViewModel(Address address)
		{
			if (address == null)
				throw new ArgumentNullException(nameof(address));

			AddressLine1 = address.AddressLine1;
			AddressLine2 = address.AddressLine2;
			City = address.City;
			RegionId = address.RegionId;
			Region = address.Region?.Name;
			PostalCode = address.PostalCode;
			CountryId = address.CountryId;
			Country = address.Country?.Name;
			RowVersion = Convert.ToBase64String(address.RowVersion);
		}

		public void MapToApplicationAddress(ApplicationAddress address, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (address == null)
				throw new ArgumentNullException(nameof(address));

			if (staticDataService == null)
				throw new ArgumentNullException(nameof(staticDataService));

			if (applicationAddressService == null)
				throw new ArgumentNullException(nameof(applicationAddressService));

			address.RowVersion = RowVersion != null ? Convert.FromBase64String(RowVersion) : null;

			Region region;
			var country = staticDataService.GetCountry(CountryId);
			if (IsCanadianAddress)
			{
				CountryId = Constants.CanadaCountryId;
				region = staticDataService.GetRegion(CountryId, RegionId);
				address.PostalCode = PostalCode;
			}
			else
			{
				region = applicationAddressService.VerifyOrCreateRegion(OtherRegion, CountryId);
				address.PostalCode = OtherZipCode;
			}

			address.AddressLine1 = AddressLine1;
			address.AddressLine2 = AddressLine2;
			address.City = City;
			address.RegionId = region.Id;
			address.Region = region;
			address.CountryId = country.Id;
			address.Country = country;
		}
	}
}