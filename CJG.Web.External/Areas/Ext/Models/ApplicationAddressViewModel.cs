using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationAddressViewModel: AddressSharedViewModel
	{
		#region Properties
		public byte[] RowVersion { get; set; }

		public IEnumerable<Region> Provinces { get; set; }

		public IEnumerable<Country> Countries { get; set; }

		#endregion

		#region Constructors

		public ApplicationAddressViewModel()
		{
		}

		public ApplicationAddressViewModel(ApplicationAddress address): base(address)
		{
		}

		#endregion

		#region Methods
		public static implicit operator ApplicationAddress(ApplicationAddressViewModel model)
		{
			if (model == null) return null;

			var applicationAddress = new ApplicationAddress();

			applicationAddress.Id = model.Id;
			applicationAddress.AddressLine1 = model.AddressLine1;
			applicationAddress.AddressLine2 = model.AddressLine2;
			applicationAddress.City = model.City;
			applicationAddress.PostalCode = model.PostalCode;
			applicationAddress.RegionId = model.RegionId;
			applicationAddress.CountryId = model.CountryId;
			applicationAddress.RowVersion = model.RowVersion;

			return applicationAddress;
		}

		#endregion
	}

	
}