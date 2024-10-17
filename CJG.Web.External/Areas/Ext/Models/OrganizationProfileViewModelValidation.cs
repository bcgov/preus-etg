using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.OrganizationProfile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using CJG.Web.External.Areas.Ext.Models.Attachments;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class OrganizationProfileViewModelValidation
    {
        public static ValidationResult ValidateYearEstablished(int? yearEstablished, ValidationContext context)
        {
            var model = context.ObjectInstance as OrganizationProfileViewNewModel;
            if (model == null)
	            throw new ArgumentNullException();

            var result = ValidationResult.Success;
            if (!yearEstablished.HasValue || yearEstablished < 1800 || yearEstablished > AppDateTime.UtcNow.Year)
            {
                result = new ValidationResult($"Year established must be between 1800 and {AppDateTime.UtcNow.Year}");
            }
            return result;
        }

        public static ValidationResult ValidateNumberOfEmployeesInBC(int? NumberOfEmployeesInBC, ValidationContext context)
        {
            var model = context.ObjectInstance as OrganizationProfileViewNewModel;
            if (model == null)
	            throw new ArgumentNullException();

            var result = ValidationResult.Success;
            if (!NumberOfEmployeesInBC.HasValue || NumberOfEmployeesInBC > model.NumberOfEmployeesWorldwide)
            {
                result = new ValidationResult("Number of employees in BC can't be greater than number of employees worldwide");
            }

            return result;
        }

        public static ValidationResult ValidateBusinessLicenseDocuments(IEnumerable<AttachmentViewModel> BusinessLicenseDocumentAttachments,  ValidationContext context)
        {
            var model = context.ObjectInstance as OrganizationProfileViewNewModel;
            if (model == null)
	            throw new ArgumentNullException();

            if (BusinessLicenseDocumentAttachments == null)
	            return new ValidationResult("Business information document is required");

			if (!BusinessLicenseDocumentAttachments.Any())
				return new ValidationResult("Business information document is required");

			// Do we have a new document being uploaded? Assume it's current.
			if (BusinessLicenseDocumentAttachments.Any(b => b.Id == 0))
				return ValidationResult.Success;

			// Do we have existing documents? Check if they are newer than 12 months old.
			var businessLicenseExpiry = AppDateTime.UtcNow.AddMonths(-12);
			if (BusinessLicenseDocumentAttachments
				.Where(b => b.Id > 0)
				.All(b => b.DateAdded < businessLicenseExpiry || b.DateUpdated <= businessLicenseExpiry))
			{
				return new ValidationResult("Please upload a recent business information document");
			}

			return ValidationResult.Success;
        }

        public static ValidationResult ValidateBusinessUrl(string businessUrl, ValidationContext context)
        {
            var model = context.ObjectInstance as OrganizationProfileViewNewModel;
            if (model == null)
	            throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(businessUrl))
	            return ValidationResult.Success;

            var urlAttribute = new UrlAttribute();
            if (!urlAttribute.IsValid(businessUrl))
				return new ValidationResult("Business website must be a valid, fully-qualified http or https URL.");
			
			return ValidationResult.Success;
        }
    } 
}
