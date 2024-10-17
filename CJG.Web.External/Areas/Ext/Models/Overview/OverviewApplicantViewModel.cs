using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Overview
{
	// TODO: Rename file.
	public class OverviewApplicantViewModel
	{
		public string LegalName { get; set; }
		public string DoingBusinessAs { get; set; }
		public string EmployerTypeDescription { get; set; }
		public string LegalStructure { get; set; }
		public string OtherLegalStructure { get; set; }
		public string BusinessLicenseNumber { get; set; }
		public int? YearEstablished { get; set; }
		public int? NumberOfEmployeesWorldwide { get; set; }
		public int? NumberOfEmployeesInBC { get; set; }
		public decimal? AnnualTrainingBudget { get; set; }
		public int? AnnualEmployeesTrained { get; set; }
		public string Naics { get; set; }
		public string HeadOfficeAddress { get; set; }

		public OverviewApplicantViewModel(GrantApplication grantApplication)
		{
			var organization = grantApplication.Organization;
			if (organization == null)
				throw new ArgumentNullException(nameof(organization));

			LegalName = grantApplication != null ? grantApplication.OrganizationLegalName : organization.LegalName;
			DoingBusinessAs = grantApplication?.OrganizationDoingBusinessAs ?? organization.DoingBusinessAs;
			EmployerTypeDescription = grantApplication != null ? grantApplication.OrganizationType.Id.ResolveEmployerTypeCode<OrganizationTypeCodes>() : organization.OrganizationType.Id.ResolveEmployerTypeCode<OrganizationTypeCodes>();
			LegalStructure = grantApplication != null ? grantApplication.OrganizationLegalStructure.Caption : organization.LegalStructure.Caption;
			OtherLegalStructure = organization.OtherLegalStructure;
			BusinessLicenseNumber = grantApplication.OrganizationBusinessLicenseNumber;
			YearEstablished = grantApplication != null ? grantApplication.OrganizationYearEstablished : (organization.Id == 0 ? null : organization.YearEstablished);
			NumberOfEmployeesWorldwide = grantApplication != null ? grantApplication.OrganizationNumberOfEmployeesWorldwide : (organization.Id == 0 ? null : organization.NumberOfEmployeesWorldwide);
			NumberOfEmployeesInBC = grantApplication != null ? grantApplication.OrganizationNumberOfEmployeesInBC : (organization.Id == 0 ? null : organization.NumberOfEmployeesInBC);
			AnnualTrainingBudget = grantApplication != null ? grantApplication.OrganizationAnnualTrainingBudget : (organization.Id == 0 ? null : (int?)organization.AnnualTrainingBudget);
			AnnualEmployeesTrained = grantApplication != null ? grantApplication.OrganizationAnnualEmployeesTrained : (organization.Id == 0 ? null : organization.AnnualEmployeesTrained);
			Naics = grantApplication != null ? (grantApplication.NAICS != null ? grantApplication.NAICS.ToString() : "") : (organization.Naics != null ? organization.Naics.ToString() : "");
			var headOfficeAddress = grantApplication != null ? new AddressViewModel(grantApplication.OrganizationAddress) : (organization.HeadOfficeAddress != null ? new AddressViewModel(organization.HeadOfficeAddress) : null);
			HeadOfficeAddress = headOfficeAddress != null ? headOfficeAddress.ToString() : "n/a";
		}
	}
}
