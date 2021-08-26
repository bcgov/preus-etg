using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicantViewModel : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "Organization Legal Name is required", AllowEmptyStrings = false)]
		public string OrganizationLegalName { get; set; }

		[Required(ErrorMessage = "Organization Type is required")]
		public int OrganizationTypeId { get; set; }

		[Required(ErrorMessage = "Legal Structure is required")]
		public int LegalStructureId { get; set; }

		[Required(ErrorMessage = "Year Established is required")]
		public int? OrganizationYearEstablished { get; set; }

		[Required(ErrorMessage = "Number of Employees Worldwide (including BC) is required")]
		public int? OrganizationNumberOfEmployeesWorldwide { get; set; }

		[Required(ErrorMessage = "Number of Employees in BC is required")]
		public int? OrganizationNumberOfEmployeesBC { get; set; }

		[Required(ErrorMessage = "Average Spending on Training Annually is required")]
		public decimal? OrganizationAnnualTrainingBudget { get; set; }

		[Required(ErrorMessage = "Average Number of Employees Trained Annually is required")]
		public int? OrganizationAnnualEmployeesTrained { get; set; }

		[StringLength(20, ErrorMessage = "The field must be a string with a maximum length of 20 characters")]
		public string BusinessLicenseNumber { get; set; }

		public string BusinessNumber { get; set; }

		public bool? BusinessNumberVerified { get; set; }

		public string StatementOfRegistrationNumber { get; set; }

		public string JurisdictionOfIncorporation { get; set; }

		public string IncorporationNumber { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250 characters")]
		public string AddressLine1 { get; set; }

		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250 characters")]
		public string City { get; set; }

		public string RegionId { get; set; }

		[Required(ErrorMessage = "Region is required")]
		[StringLength(250, MinimumLength = 2, ErrorMessage = "Region length needs to be in-between 2 and 250 characters")]
		public string Region { get; set; }

		[Required(ErrorMessage = "Postal Code is required")]
		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10 characters")]
		public string PostalCode { get; set; }

		public string CountryId { get; set; }

		[Required(ErrorMessage = "Country is required")]
		public string Country { get; set; }

		public bool HasAppliedForGrantBefore { get; set; }

		public bool WouldTrainEmployeesWithoutGrant { get; set; }

		[Required(ErrorMessage = "NAICS level 1 is required.")]
		public int? NAICSLevel1Id { get; set; }

		[Required(ErrorMessage = "NAICS level 2 is required.")]
		public int? NAICSLevel2Id { get; set; }

		[Required(ErrorMessage = "NAICS level 3 is required.")]
		public int? NAICSLevel3Id { get; set; }

		public int? NAICSLevel4Id { get; set; }

		public int? NAICSLevel5Id { get; set; }

		public string GrantProgramName { get; set; }

		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public ApplicantViewModel() { }

		public ApplicantViewModel(GrantApplication grantApplication, INaIndustryClassificationSystemService naIndustryClassificationSystemService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (naIndustryClassificationSystemService == null) throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.OrganizationLegalName = grantApplication.OrganizationLegalName;
			this.OrganizationTypeId = grantApplication.OrganizationTypeId.Value;
			this.LegalStructureId = grantApplication.OrganizationLegalStructureId.Value;
			this.OrganizationYearEstablished = grantApplication.OrganizationYearEstablished;
			this.OrganizationNumberOfEmployeesWorldwide = grantApplication.OrganizationNumberOfEmployeesWorldwide;
			this.OrganizationNumberOfEmployeesBC = grantApplication.OrganizationNumberOfEmployeesInBC;
			this.OrganizationAnnualTrainingBudget = grantApplication.OrganizationAnnualTrainingBudget.Value;
			this.OrganizationAnnualEmployeesTrained = grantApplication.OrganizationAnnualEmployeesTrained;
			this.BusinessNumber = grantApplication.Organization.BusinessNumber;
			this.BusinessNumberVerified = grantApplication.Organization.BusinessNumberVerified;
			this.StatementOfRegistrationNumber = grantApplication.Organization.StatementOfRegistrationNumber;
			this.JurisdictionOfIncorporation = grantApplication.Organization.JurisdictionOfIncorporation;
			this.IncorporationNumber = grantApplication.Organization.IncorporationNumber;
			this.AddressLine1 = grantApplication.OrganizationAddress.AddressLine1;
			this.AddressLine2 = grantApplication.OrganizationAddress.AddressLine2;
			this.City = grantApplication.OrganizationAddress.City;
			this.RegionId = grantApplication.OrganizationAddress.Region == null ? "BC" : grantApplication.OrganizationAddress.Region.Id;
			this.Region = grantApplication.OrganizationAddress.Region.Name;
			this.PostalCode = grantApplication.OrganizationAddress.PostalCode;
			this.CountryId = grantApplication.OrganizationAddress.CountryId;
			this.Country = grantApplication.OrganizationAddress.Country.Name;
			this.HasAppliedForGrantBefore = grantApplication.HasAppliedForGrantBefore;
			this.WouldTrainEmployeesWithoutGrant = grantApplication.WouldTrainEmployeesWithoutGrant;
			var naics = grantApplication.NAICSId.HasValue ? naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(grantApplication.NAICSId.Value) : null;
			this.NAICSLevel1Id = naics.FirstOrDefault(n => n.Level == 1)?.Id;
			this.NAICSLevel2Id = naics.FirstOrDefault(n => n.Level == 2)?.Id;
			this.NAICSLevel3Id = naics.FirstOrDefault(n => n.Level == 3)?.Id;
			this.NAICSLevel4Id = naics.FirstOrDefault(n => n.Level == 4)?.Id;
			this.NAICSLevel5Id = naics.FirstOrDefault(n => n.Level == 5)?.Id;
			this.BusinessLicenseNumber = grantApplication.OrganizationBusinessLicenseNumber;
		}
		#endregion

		#region Methods
		public void MapTo(GrantApplication grantApplication, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));

			var country = staticDataService.GetCountry(this.CountryId);
			var region = (country.Id == Constants.CanadaCountryId) ? staticDataService.GetRegion(this.CountryId, this.RegionId) : applicationAddressService.VerifyOrCreateRegion(this.Region, this.CountryId);

			grantApplication.RowVersion = Convert.FromBase64String(this.RowVersion);
			grantApplication.OrganizationLegalName = this.OrganizationLegalName;
			grantApplication.OrganizationTypeId = this.OrganizationTypeId;
			grantApplication.OrganizationLegalStructureId = this.LegalStructureId;
			grantApplication.OrganizationYearEstablished = this.OrganizationYearEstablished;
			grantApplication.OrganizationNumberOfEmployeesWorldwide = this.OrganizationNumberOfEmployeesWorldwide;
			grantApplication.OrganizationNumberOfEmployeesInBC = this.OrganizationNumberOfEmployeesBC;
			grantApplication.OrganizationAnnualTrainingBudget = this.OrganizationAnnualTrainingBudget;
			grantApplication.OrganizationAnnualEmployeesTrained = this.OrganizationAnnualEmployeesTrained;
			grantApplication.OrganizationBusinessLicenseNumber = this.BusinessLicenseNumber;
			grantApplication.Organization.BusinessNumber = this.BusinessNumber;
			grantApplication.OrganizationAddress.AddressLine1 = this.AddressLine1;
			grantApplication.OrganizationAddress.AddressLine2 = this.AddressLine2;
			grantApplication.OrganizationAddress.City = this.City;
			grantApplication.OrganizationAddress.RegionId = region.Id;
			grantApplication.OrganizationAddress.Region = region;
			grantApplication.OrganizationAddress.PostalCode = this.PostalCode;
			grantApplication.OrganizationAddress.CountryId = country.Id;
			grantApplication.OrganizationAddress.Country = country;
			grantApplication.HasAppliedForGrantBefore = this.HasAppliedForGrantBefore;
			grantApplication.WouldTrainEmployeesWithoutGrant = this.WouldTrainEmployeesWithoutGrant;
			grantApplication.NAICSId = this.NAICSLevel5Id ?? this.NAICSLevel4Id ?? this.NAICSLevel3Id ?? this.NAICSLevel2Id ?? this.NAICSLevel1Id;
		}
		#endregion
	}
}