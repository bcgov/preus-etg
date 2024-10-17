using System;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Prioritization
{
	public class GrantApplicationViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public int? AssessorId { get; set; }
		public string Assessor { get; set; }
		public string Applicant { get; set; }
		public string ContactPostalCode { get; set; }
		public string ContactAddress { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public string ApplicationStateInternalCaption => ApplicationStateInternal.GetDescription();

		public GrantApplicationViewModel() { }

		public GrantApplicationViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			FileNumber = grantApplication.FileNumber;
			AssessorId = grantApplication.AssessorId;
			Assessor = $"{grantApplication.Assessor?.FirstName} {grantApplication.Assessor?.LastName}";
			Applicant = grantApplication.OrganizationLegalName;
			ContactPostalCode = grantApplication.ApplicantPhysicalAddress.PostalCode;
			ContactAddress = FormatAddress(grantApplication.ApplicantPhysicalAddress);
			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
		}

		private string FormatAddress(ApplicationAddress applicationAddress)
		{
			var delimiter = ", ";

			return
				applicationAddress.AddressLine1 + delimiter +
				(string.IsNullOrWhiteSpace(applicationAddress.AddressLine2) ? string.Empty : applicationAddress.AddressLine2 + delimiter) +
				(string.IsNullOrWhiteSpace(applicationAddress.City) ? string.Empty : applicationAddress.City);
		}
	}
}