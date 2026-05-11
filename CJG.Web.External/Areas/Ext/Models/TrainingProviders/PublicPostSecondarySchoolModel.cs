using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.TrainingProviders
{
	public class PublicPostSecondarySchoolModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }

		public string RegionId => "BC";
		public string CountryId => "CA";

		public PublicPostSecondarySchoolModel()
		{
		}

		public PublicPostSecondarySchoolModel(PublicPostSecondarySchool school)
		{
			Id = school.Id;
			Name = school.Name;
			AddressLine1 = school.AddressLine1;
			AddressLine2 = school.AddressLine2;
			City = school.City;
			PostalCode = school.PostalCode;
		}
	}
}