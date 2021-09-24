using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.OrganizationProfile
{
	public class OrganizationProfileViewNewModel : BaseViewModel
	{
		#region Properties
		public bool IsOrganizationProfileAdministrator { get; set; }
		public bool CreateOrganizationProfile { get; set; }
		public string BackURL { get; set; }

		[Required(ErrorMessage = "NAICS sector code is required")]
		public int? Naics1Id { get; set; }
		[Required(ErrorMessage = "NAICS sector code is required")]
		public int? Naics2Id { get; set; }
		[Required(ErrorMessage = "NAICS sector code is required")]
		public int? Naics3Id { get; set; }
		public int? Naics4Id { get; set; }
		public int? Naics5Id { get; set; }

		public int? NaicsId { get { return Naics5Id ?? Naics4Id ?? Naics3Id ?? Naics2Id ?? Naics1Id; } }

		[Required]
		public Guid BCeIDGuid { get; set; }

		[Required(ErrorMessage = "Organization legal name is required"), MaxLength(500)]
		public string LegalName { get; set; }

		public int? HeadOfficeAddressId { get; set; }

		public AddressViewModel HeadOfficeAddress { get; set; }

		public int OrganizationTypeId { get; set; }

		[Required(ErrorMessage = "Legal structure is required")]
		public int? LegalStructureId { get; set; }

		[MaxLength(250)]
		public string OtherLegalStructure { get; set; }

		[RegularExpression("^[0-9]{4,4}$", ErrorMessage = "Value must be in YYYY format")]
		[Required(ErrorMessage = "Year Established is required")]
		[CustomValidation(typeof(OrganizationProfileViewModelValidation), "ValidateYearEstablished")]
		public int? YearEstablished { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Number of Worldwide Employees is required")]
		[Range(1, 999999, ErrorMessage = "The Number of Worldwide Employees must be between 1 and 999,999")]
		public int? NumberOfEmployeesWorldwide { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Number of employees in BC is required")]
		[Range(1, 999999, ErrorMessage = "The Number of employees in BC must be between 1 and 999,999, and not less than the number worldwide")]
		[CustomValidation(typeof(OrganizationProfileViewModelValidation), "ValidateNumberOfEmployeesInBC")]
		public int? NumberOfEmployeesInBC { get; set; }

		[RegularExpression("^[0-9]*(.00)?$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Annual Trained budget is required")]
		[Range(0, 9999999, ErrorMessage = "The Annual Trained budget must be between 0 and $9,999,99")]
		public decimal? AnnualTrainingBudget { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Number of employees trained annually is required")]
		[Range(0, 999999, ErrorMessage = "The Number of employees trained must be between 0 and 999,999")]
		public int? AnnualEmployeesTrained { get; set; }

		[MaxLength(500)]
		public string DoingBusinessAs { get; set; }

		[MaxLength(500)]
		public string DoingBusinessAsMinistry { get; set; }

		public string AdminUserName { get; set; }

		public string AdminUserEmailAddress { get; set; }

		public string StatementOfRegistrationNumber { get; set; }

		public string BusinessLicenseNumber { get; set; }


		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public OrganizationProfileViewNewModel()
		{
		}

		public OrganizationProfileViewNewModel(Organization organization, INaIndustryClassificationSystemService naIndustryClassificationSystemService)
		{
			if (organization == null) throw new ArgumentNullException(nameof(organization));
			if (naIndustryClassificationSystemService == null) throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));

			Utilities.MapProperties(organization, this);

			this.HeadOfficeAddress = organization.HeadOfficeAddress != null ? new AddressViewModel(organization.HeadOfficeAddress) : new AddressViewModel();
			this.OrganizationTypeId = organization.OrganizationTypeId ?? (int)OrganizationTypeCodes.Default;

			var adminUserInfo = organization.Users.Where(u => u.IsOrganizationProfileAdministrator == true)
												  .Select(u => new { AdminUserName = u.FirstName + " " + u.LastName, AdminUserEmailAddress = u.EmailAddress })
												  .FirstOrDefault();
			this.AdminUserName = adminUserInfo?.AdminUserName;
			this.AdminUserEmailAddress = adminUserInfo?.AdminUserEmailAddress;

			var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(organization.NaicsId);
			naics.ForEach(item =>
			{
				var property = this.GetType().GetProperty($"Naics{item.Level}Id");
				property?.SetValue(this, item.Id);
			});
		}

		public void UpdateOrganization(IUserService userService, ISiteMinderService siteMinderService, IOrganizationService organizationService)
		{
			if (userService == null) throw new ArgumentNullException(nameof(userService));
			if (siteMinderService == null) throw new ArgumentNullException(nameof(siteMinderService));
			if (organizationService == null) throw new ArgumentNullException(nameof(organizationService));

			var currentUser = userService.GetUser(siteMinderService.CurrentUserGuid);
			var organization = currentUser.Organization;

			if (organization == null)
			{
				var bcUser = userService.GetBCeIDUser(currentUser.BCeIDGuid);
				organization = bcUser.Organization ?? new Organization();
				currentUser.Organization = organization;
				if (organization.Id == 0) currentUser.IsOrganizationProfileAdministrator = true;
			}
			else
			{
				organization.RowVersion = Convert.FromBase64String(this.RowVersion);
			}

			organization.DoingBusinessAs = this.DoingBusinessAs;
			organization.OrganizationType = (OrganizationTypeCodes)this.OrganizationTypeId == OrganizationTypeCodes.Default ? organizationService.GetDefaultOrganizationType() : organizationService.GetOrganizationType(this.OrganizationTypeId);
			organization.YearEstablished = this.YearEstablished.Value;
			organization.NumberOfEmployeesWorldwide = this.NumberOfEmployeesWorldwide.Value;
			organization.AnnualTrainingBudget = this.AnnualTrainingBudget.Value;
			organization.AnnualEmployeesTrained = this.AnnualEmployeesTrained.Value;
			organization.NumberOfEmployeesInBC = this.NumberOfEmployeesInBC.Value;

			if (organization.HeadOfficeAddress == null)
			{
				organization.HeadOfficeAddress = new Address();
			}

			organization.HeadOfficeAddress.AddressLine1 = this.HeadOfficeAddress.AddressLine1;
			organization.HeadOfficeAddress.AddressLine2 = this.HeadOfficeAddress.AddressLine2;
			organization.HeadOfficeAddress.City = this.HeadOfficeAddress.City;
			organization.HeadOfficeAddress.PostalCode = this.HeadOfficeAddress.PostalCode;
			organization.HeadOfficeAddress.RegionId = this.HeadOfficeAddress.RegionId;
			organization.HeadOfficeAddress.CountryId = this.HeadOfficeAddress.CountryId;

			organization.LegalStructureId = this.LegalStructureId;
			organization.OtherLegalStructure = organization.LegalStructureId == 10 ? this.OtherLegalStructure : null;
			organization.BusinessLicenseNumber = this.BusinessLicenseNumber;

			organization.NaicsId = this.NaicsId;
			organization.IsNaicsUpdated = true;

			userService.Update(currentUser);

			this.RowVersion = Convert.ToBase64String(organization.RowVersion);
		}
		#endregion
	}
}
