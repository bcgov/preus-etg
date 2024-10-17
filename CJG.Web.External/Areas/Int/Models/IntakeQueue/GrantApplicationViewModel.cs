using System;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.IntakeQueue
{
	public class GrantApplicationViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public int? AssessorId { get; set; }
		public string Assessor { get; set; }
		public string Applicant { get; set; }
		public DateTime DateSubmitted { get; set; }
		public DateTime StartDate { get; set; }
		public int PrioritizationScore { get; set; }
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
			DateSubmitted = grantApplication.DateSubmitted.Value;
			StartDate = grantApplication.StartDate;
			PrioritizationScore = grantApplication.PrioritizationScore;
			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
		}
	}
}