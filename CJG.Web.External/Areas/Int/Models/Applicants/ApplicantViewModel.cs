using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicantViewModel : BaseViewModel
	{
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

		public List<GrantStreamQuestionViewModel> StreamEligibilityQuestions { get; set; }

		public string RowVersion { get; set; }

		public ApplicantViewModel() { }

		public ApplicantViewModel(GrantApplication grantApplication, INaIndustryClassificationSystemService naIndustryClassificationSystemService, IGrantStreamService grantStreamService)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (naIndustryClassificationSystemService == null)
				throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			OrganizationLegalName = grantApplication.OrganizationLegalName;
			OrganizationTypeId = grantApplication.OrganizationTypeId.Value;
			LegalStructureId = grantApplication.OrganizationLegalStructureId.Value;
			OrganizationYearEstablished = grantApplication.OrganizationYearEstablished;
			OrganizationNumberOfEmployeesWorldwide = grantApplication.OrganizationNumberOfEmployeesWorldwide;
			OrganizationNumberOfEmployeesBC = grantApplication.OrganizationNumberOfEmployeesInBC;
			OrganizationAnnualTrainingBudget = grantApplication.OrganizationAnnualTrainingBudget.Value;
			OrganizationAnnualEmployeesTrained = grantApplication.OrganizationAnnualEmployeesTrained;
			BusinessNumber = grantApplication.Organization.BusinessNumber;
			BusinessNumberVerified = grantApplication.Organization.BusinessNumberVerified;
			StatementOfRegistrationNumber = grantApplication.Organization.StatementOfRegistrationNumber;
			JurisdictionOfIncorporation = grantApplication.Organization.JurisdictionOfIncorporation;
			IncorporationNumber = grantApplication.Organization.IncorporationNumber;
			AddressLine1 = grantApplication.OrganizationAddress.AddressLine1;
			AddressLine2 = grantApplication.OrganizationAddress.AddressLine2;
			City = grantApplication.OrganizationAddress.City;
			RegionId = grantApplication.OrganizationAddress.Region == null ? "BC" : grantApplication.OrganizationAddress.Region.Id;
			Region = grantApplication.OrganizationAddress.Region.Name;
			PostalCode = grantApplication.OrganizationAddress.PostalCode;
			CountryId = grantApplication.OrganizationAddress.CountryId;
			Country = grantApplication.OrganizationAddress.Country.Name;
			HasAppliedForGrantBefore = grantApplication.HasAppliedForGrantBefore;
			WouldTrainEmployeesWithoutGrant = grantApplication.WouldTrainEmployeesWithoutGrant;
			var naics = grantApplication.NAICSId.HasValue ? naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(grantApplication.NAICSId.Value).ToList() : null;
			if (naics != null)
			{
				NAICSLevel1Id = naics.FirstOrDefault(n => n.Level == 1)?.Id;
				NAICSLevel2Id = naics.FirstOrDefault(n => n.Level == 2)?.Id;
				NAICSLevel3Id = naics.FirstOrDefault(n => n.Level == 3)?.Id;
				NAICSLevel4Id = naics.FirstOrDefault(n => n.Level == 4)?.Id;
				NAICSLevel5Id = naics.FirstOrDefault(n => n.Level == 5)?.Id;
			}
			BusinessLicenseNumber = grantApplication.OrganizationBusinessLicenseNumber;

			var streamEligibilityQuestions = grantStreamService.GetGrantStreamQuestions(grantApplication.GrantOpening.GrantStreamId)
				.Where(l => l.IsActive)
				.Select(n => new GrantStreamQuestionViewModel(n))
				.ToList();

			var grantApplicationAnswers = grantStreamService.GetGrantStreamAnswers(grantApplication.Id).ToList();
			foreach (var quest in streamEligibilityQuestions)
			{
				var answer = grantApplicationAnswers.FirstOrDefault(a => a.GrantStreamEligibilityQuestionId == quest.Id);
				if (answer == null)
					continue;

				quest.EligibilityAnswer = answer.EligibilityAnswer;
				quest.RationaleAnswer = answer.RationaleAnswer;
			}
			StreamEligibilityQuestions = streamEligibilityQuestions;
		}

		public void MapTo(GrantApplication grantApplication, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (staticDataService == null)
				throw new ArgumentNullException(nameof(staticDataService));

			if (applicationAddressService == null)
				throw new ArgumentNullException(nameof(applicationAddressService));
			
			var country = staticDataService.GetCountry(CountryId);
			var region = country.Id == Constants.CanadaCountryId ? staticDataService.GetRegion(CountryId, RegionId) : applicationAddressService.VerifyOrCreateRegion(Region, CountryId);

			grantApplication.RowVersion = Convert.FromBase64String(RowVersion);
			grantApplication.OrganizationLegalName = OrganizationLegalName;
			grantApplication.OrganizationTypeId = OrganizationTypeId;
			grantApplication.OrganizationLegalStructureId = LegalStructureId;
			grantApplication.OrganizationYearEstablished = OrganizationYearEstablished;
			grantApplication.OrganizationNumberOfEmployeesWorldwide = OrganizationNumberOfEmployeesWorldwide;
			grantApplication.OrganizationNumberOfEmployeesInBC = OrganizationNumberOfEmployeesBC;
			grantApplication.OrganizationAnnualTrainingBudget = OrganizationAnnualTrainingBudget;
			grantApplication.OrganizationAnnualEmployeesTrained = OrganizationAnnualEmployeesTrained;
			grantApplication.OrganizationBusinessLicenseNumber = BusinessLicenseNumber;
			grantApplication.Organization.BusinessNumber = BusinessNumber;
			grantApplication.OrganizationAddress.AddressLine1 = AddressLine1;
			grantApplication.OrganizationAddress.AddressLine2 = AddressLine2;
			grantApplication.OrganizationAddress.City = City;
			grantApplication.OrganizationAddress.RegionId = region.Id;
			grantApplication.OrganizationAddress.Region = region;
			grantApplication.OrganizationAddress.PostalCode = PostalCode;
			grantApplication.OrganizationAddress.CountryId = country.Id;
			grantApplication.OrganizationAddress.Country = country;
			grantApplication.HasAppliedForGrantBefore = HasAppliedForGrantBefore;
			grantApplication.WouldTrainEmployeesWithoutGrant = WouldTrainEmployeesWithoutGrant;
			grantApplication.NAICSId = NAICSLevel5Id ?? NAICSLevel4Id ?? NAICSLevel3Id ?? NAICSLevel2Id ?? NAICSLevel1Id;

			foreach(var questionModel in StreamEligibilityQuestions)
			{
				var answer = grantApplication.GrantStreamEligibilityAnswers.FirstOrDefault(q => q.GrantStreamEligibilityQuestionId == questionModel.Id);
				if (answer != null)
				{
					answer.EligibilityAnswer = questionModel.EligibilityAnswer ?? false;
					answer.RationaleAnswer = questionModel.RationaleAnswer;
				}
			}
		}
	}
}