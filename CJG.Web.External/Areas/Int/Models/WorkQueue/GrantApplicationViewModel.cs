using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.WorkQueue
{
	public class GrantApplicationViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public int? AssessorId { get; set; }
		public string Assessor { get; set; }
		public string Applicant { get; set; }
		public DateTime DateSubmitted { get; set; }
		public DateTime StartDate { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public string ApplicationStateInternalCaption { get { return this.ApplicationStateInternal.GetDescription(); } }
		public DateTime StatusChanged { get; set; }
		public string GrantStreamName { get; set; }
		public bool RiskFlag { get; set; }
		public int OrgId { get; set; }
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
			this.DateSubmitted = grantApplication.DateSubmitted.Value;
			this.StartDate = grantApplication.StartDate;
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.StatusChanged = grantApplication.StateChanges.OrderByDescending(s => s.DateAdded).FirstOrDefault().ChangedDate.ToLocalTime();
			this.GrantStreamName = $"{grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramCode} - {grantApplication.GrantOpening.GrantStream.Name}";
			this.RiskFlag = grantApplication.Organization.RiskFlag;
			this.OrgId = grantApplication.Organization.Id;
		}
		#endregion
	}
}