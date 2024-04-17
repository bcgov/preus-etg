namespace CJG.Core.Entities
{
	public interface IAmAnAddress
	{
		string AddressLine1 { get; set; }
		string AddressLine2 { get; set; }
		string City { get; set; }
		string PostalCode { get; set; }
		string RegionId { get; set; }
		string CountryId { get; set; }
	}
}