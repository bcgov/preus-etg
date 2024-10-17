using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.BatchApprovals
{
	public class GrantApplicationViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public int? AssessorId { get; set; }
		public string Assessor { get; set; }
		public string Applicant { get; set; }
		public decimal AgreedCommitment { get; set; }
		#endregion

		#region Constructors
		public GrantApplicationViewModel() { }

		public GrantApplicationViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.FileNumber = grantApplication.FileNumber;
			this.AssessorId = grantApplication.AssessorId;
			this.Assessor = $"{grantApplication.Assessor?.FirstName} {grantApplication.Assessor?.LastName}";
			this.Applicant = grantApplication.OrganizationLegalName;
			this.AgreedCommitment = grantApplication.TrainingCost.AgreedCommitment;
		}
		#endregion
	}
}