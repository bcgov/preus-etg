using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OrganizationViewModel
	{
		#region Properties
		public int Id { get; set; }

		[Required]
		public Guid BCeIDGuid { get; set; }

		[Required(ErrorMessage = "Organization legal name is required"), MaxLength(500)]
		public string LegalName { get; set; }

		[Required(ErrorMessage = "Organization type is required")]
		[DefaultValue(OrganizationTypeCodes.Default)]
		public OrganizationTypeCodes OrganizationTypeCode { get; set; } = OrganizationTypeCodes.Default;

		public int? HeadOfficeAddressId { get; set; }

		public virtual AddressViewModel HeadOfficeAddress { get; set; }

		public int? OrganizationTypeId { get; set; }

		[NotMapped]
		public virtual OrganizationType OrganizationType { get; set; }

		[Required(ErrorMessage = "Legal structure is required")]
		public int? LegalStructureId { get; set; }

		[NotMapped]
		public virtual LegalStructure LegalStructure { get; set; }

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

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Annual Trained budget is required")]
		[Range(0, 9999999, ErrorMessage = "The Annual Trained budget must be between 0 and $9,999,99")]
		public decimal? AnnualTrainingBudget { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Required(ErrorMessage = "Number of employees trained annually is required")]
		[Range(0, 999999, ErrorMessage = "The Number of employees trained must be between 0 and 999,999")]
		public int? AnnualEmployeesTrained { get; set; }

		[MaxLength(500)]
		public string DoingBusinessAs { get; set; }

		[NameValidation]
		public String AdminUserName { get; set; }

		public String AdminUserEmailAddress { get; set; }

		public int? NaicsId { get; set; }

		[NotMapped]
		public virtual NaIndustryClassificationSystem Naics { get; set; }

		public string StatementOfRegistrationNumber { get; set; }

		public string BusinessLicenseNumber { get; set; }


		public byte[] RowVersion { get; set; }
		#endregion

		#region Constructors
		public OrganizationViewModel()
		{

		}

		public OrganizationViewModel(Organization organization, GrantApplication grantApplication = null)
		{
			if (organization == null)
				throw new ArgumentNullException(nameof(organization));

			this.Id = grantApplication != null ? grantApplication.OrganizationId : organization.Id;
			this.BCeIDGuid = (grantApplication != null && grantApplication.OrganizationBCeID.HasValue) ? grantApplication.OrganizationBCeID.Value : organization.BCeIDGuid;
			this.LegalName = grantApplication != null ? grantApplication.OrganizationLegalName : organization.LegalName;
			this.StatementOfRegistrationNumber = grantApplication.Organization.StatementOfRegistrationNumber;

			this.HeadOfficeAddressId = grantApplication != null ? grantApplication.OrganizationAddressId : organization.HeadOfficeAddressId;
			this.HeadOfficeAddress = grantApplication != null ? new AddressViewModel(grantApplication.OrganizationAddress) : (organization.HeadOfficeAddress != null ? new AddressViewModel(organization.HeadOfficeAddress) : null);
			this.OrganizationTypeId = grantApplication != null ? grantApplication.OrganizationTypeId : organization.OrganizationTypeId;
			this.OrganizationType = grantApplication != null ? grantApplication.OrganizationType : organization.OrganizationType;

			this.OrganizationTypeCode = this.OrganizationTypeId == null ? OrganizationTypeCodes.Default : (OrganizationTypeCodes)this.OrganizationTypeId;

			this.LegalStructureId = grantApplication != null ? grantApplication.OrganizationLegalStructureId : organization.LegalStructureId;
			this.LegalStructure = grantApplication != null ? grantApplication.OrganizationLegalStructure : organization.LegalStructure;
			this.OtherLegalStructure = organization.OtherLegalStructure;
			this.YearEstablished = grantApplication != null ? grantApplication.OrganizationYearEstablished : (organization.Id == 0 ? null : (int?)organization.YearEstablished);
			this.NumberOfEmployeesWorldwide = grantApplication != null ? grantApplication.OrganizationNumberOfEmployeesWorldwide : (organization.Id == 0 ? null : (int?)organization.NumberOfEmployeesWorldwide);
			this.NumberOfEmployeesInBC = grantApplication != null ? grantApplication.OrganizationNumberOfEmployeesInBC : (organization.Id == 0 ? null : (int?)organization.NumberOfEmployeesInBC);
			this.AnnualTrainingBudget = grantApplication != null ? grantApplication.OrganizationAnnualTrainingBudget : (organization.Id == 0 ? null : (int?)organization.AnnualTrainingBudget);
			this.AnnualEmployeesTrained = grantApplication != null ? grantApplication.OrganizationAnnualEmployeesTrained : (organization.Id == 0 ? null : (int?)organization.AnnualEmployeesTrained);
			this.DoingBusinessAs = organization.DoingBusinessAs;
			this.Naics = grantApplication != null ? grantApplication.NAICS : organization.Naics;
			this.NaicsId = grantApplication != null ? grantApplication.NAICSId : organization.NaicsId;
			this.RowVersion = organization.RowVersion;
			var adminUserInfo = organization.Users.Where(u => u.IsOrganizationProfileAdministrator == true).Select(u => new { AdminUserName = u.FirstName + " " + u.LastName, AdminUserEmailAddress = u.EmailAddress }).FirstOrDefault();
			this.AdminUserName = adminUserInfo?.AdminUserName;
			this.AdminUserEmailAddress = adminUserInfo?.AdminUserEmailAddress;
			this.BusinessLicenseNumber = grantApplication?.OrganizationBusinessLicenseNumber ?? organization.BusinessLicenseNumber;
		}
		#endregion
	}
}
