using CJG.Core.Entities;
using Constants = CJG.Core.Entities.Constants;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class AlternateAddressViewModel : AddressSharedViewModel
	{
		public AlternateAddressViewModel() : base()
		{
		}

		public AlternateAddressViewModel(ApplicationAddress address) : base(address)
		{
		}

		public void MapToApplicationAddress(ApplicationAddress address, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (address == null) throw new ArgumentNullException(nameof(address));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));

			address.RowVersion = this.RowVersion != null ? Convert.FromBase64String(this.RowVersion) : null;

			Region region = null;
			var country = staticDataService.GetCountry(this.CountryId);
			if (this.IsCanadianAddress)
			{
				this.CountryId = Constants.CanadaCountryId;
				region = staticDataService.GetRegion(this.CountryId, this.RegionId);
				address.PostalCode = this.PostalCode;
			}
			else
			{
				region = applicationAddressService.VerifyOrCreateRegion(this.OtherRegion, this.CountryId);
				address.PostalCode = this.OtherZipCode;
			}
			address.AddressLine1 = this.AddressLine1;
			address.AddressLine2 = this.AddressLine2;
			address.City = this.City;
			address.RegionId = region.Id;
			address.Region = region;
			address.CountryId = country.Id;
			address.Country = country;
		}
	}
}