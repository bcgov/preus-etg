using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.OrganizationProfile
{
	public class OrganizationProfileViewNewModel : BaseViewModel
	{
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

		public string HeadOfficeAddressBlob { get; set; }
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

		public bool RequiresBusinessLicenseDocuments { get; set; }

		[MaxLength(2000, ErrorMessage = "Business website cannot exceed 2000 characters.")]
		[CustomValidation(typeof(OrganizationProfileViewModelValidation), "ValidateBusinessUrl")]
		public string BusinessWebsite { get; set; }

		[AllowHtml]
		[Required(ErrorMessage = "Business description is required")]
		[MaxLength(150000, ErrorMessage = "Business description cannot exceed 150000 characters.")]
		public string BusinessDescription { get; set; }

		[CustomValidation(typeof(OrganizationProfileViewModelValidation), "ValidateBusinessLicenseDocuments")]
		public IEnumerable<AttachmentViewModel> BusinessLicenseDocumentAttachments { get; set; }

		public string RowVersion { get; set; }

		public OrganizationProfileViewNewModel()
		{
		}

		public OrganizationProfileViewNewModel(Organization organization, INaIndustryClassificationSystemService naIndustryClassificationSystemService)
		{
			if (organization == null)
				throw new ArgumentNullException(nameof(organization));

			if (naIndustryClassificationSystemService == null)
				throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));

			Utilities.MapProperties(organization, this);

			HeadOfficeAddress = organization.HeadOfficeAddress != null ? new AddressViewModel(organization.HeadOfficeAddress) : new AddressViewModel();
			OrganizationTypeId = organization.OrganizationTypeId ?? (int)OrganizationTypeCodes.Default;

			var adminUserInfo = organization.Users.Where(u => u.IsOrganizationProfileAdministrator)
												  .Select(u => new { AdminUserName = u.FirstName + " " + u.LastName, AdminUserEmailAddress = u.EmailAddress })
												  .FirstOrDefault();
			AdminUserName = adminUserInfo?.AdminUserName;
			AdminUserEmailAddress = adminUserInfo?.AdminUserEmailAddress;

			BusinessLicenseDocumentAttachments = organization.BusinessLicenseDocuments.Select(a => new AttachmentViewModel(a));

			var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(organization.NaicsId);
			naics.ForEach(item =>
			{
				var property = GetType().GetProperty($"Naics{item.Level}Id");
				property?.SetValue(this, item.Id);
			});
		}

		public void UpdateOrganization(IUserService userService, ISiteMinderService siteMinderService, IOrganizationService organizationService)
		{
			if (userService == null) throw
				new ArgumentNullException(nameof(userService));

			if (siteMinderService == null)
				throw new ArgumentNullException(nameof(siteMinderService));

			if (organizationService == null)
				throw new ArgumentNullException(nameof(organizationService));

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
				organization.RowVersion = Convert.FromBase64String(RowVersion);
			}

			organization.DoingBusinessAs = DoingBusinessAs;
			organization.OrganizationType = (OrganizationTypeCodes)OrganizationTypeId == OrganizationTypeCodes.Default ? organizationService.GetDefaultOrganizationType() : organizationService.GetOrganizationType(OrganizationTypeId);
			organization.YearEstablished = YearEstablished.Value;
			organization.NumberOfEmployeesWorldwide = NumberOfEmployeesWorldwide.Value;
			organization.AnnualTrainingBudget = AnnualTrainingBudget.Value;
			organization.AnnualEmployeesTrained = AnnualEmployeesTrained.Value;
			organization.NumberOfEmployeesInBC = NumberOfEmployeesInBC.Value;

			if (organization.HeadOfficeAddress == null)
				organization.HeadOfficeAddress = new Address();

			organization.HeadOfficeAddress.AddressLine1 = HeadOfficeAddress.AddressLine1;
			organization.HeadOfficeAddress.AddressLine2 = HeadOfficeAddress.AddressLine2;
			organization.HeadOfficeAddress.City = HeadOfficeAddress.City;
			organization.HeadOfficeAddress.PostalCode = HeadOfficeAddress.PostalCode;
			organization.HeadOfficeAddress.RegionId = HeadOfficeAddress.RegionId;
			organization.HeadOfficeAddress.CountryId = HeadOfficeAddress.CountryId;

			organization.LegalStructureId = LegalStructureId;
			organization.OtherLegalStructure = organization.LegalStructureId == 10 ? OtherLegalStructure : null;
			organization.BusinessLicenseNumber = BusinessLicenseNumber;

			organization.NaicsId = NaicsId;
			organization.IsNaicsUpdated = true;

			organization.BusinessWebsite = BusinessWebsite;
			organization.BusinessDescription = BusinessDescription;

			userService.Update(currentUser);

			RowVersion = Convert.ToBase64String(organization.RowVersion);
		}

		public void UpdateOrganizationBusinessLicenses(IUserService userService, ISiteMinderService siteMinderService, IAttachmentService attachmentService,
			HttpPostedFileBase[] files, IEnumerable<UpdateAttachmentViewModel> data)
		{
			if (userService == null)
				throw new ArgumentNullException(nameof(userService));

			if (siteMinderService == null)
				throw new ArgumentNullException(nameof(siteMinderService));

			var currentUser = userService.GetUser(siteMinderService.CurrentUserGuid);
			var organization = currentUser.Organization;

			if (organization == null)
			{
				var bcUser = userService.GetBCeIDUser(currentUser.BCeIDGuid);
				organization = bcUser.Organization ?? new Organization();
				currentUser.Organization = organization;
				if (organization.Id == 0)
					currentUser.IsOrganizationProfileAdministrator = true;
			}
			else
			{
				organization.RowVersion = Convert.FromBase64String(RowVersion);
			}

			foreach (var attachment in data)
			{
				if (attachment.Delete) // Delete
				{
					var existing = attachmentService.Get(attachment.Id);
					existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
					organization.BusinessLicenseDocuments.Remove(existing);

					attachmentService.Delete(existing);
				}
				else if (attachment.Index.HasValue == false) // Update data only
				{
					var existing = attachmentService.Get(attachment.Id);
					existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
					attachment.MapToEntity(existing);
					attachmentService.Update(existing, true);
				}
				else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id == 0) // Add
				{
					var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
					organization.BusinessLicenseDocuments.Add(file);

					attachmentService.Add(file, true);
				}
				else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id != 0) // Update with file
				{
					var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
					var existing = attachmentService.Get(attachment.Id);
					existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);

					attachment.MapToEntity(existing);
					existing.AttachmentData = file.AttachmentData;
					attachmentService.Update(existing, true);
				}
			}
		}
	}
}
