using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicationWorkflowViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }
		public string OrganizationLegalName { get; set; }
		public int? AssessorId { get; set; }
		public string ReasonToDeny { get; set; }
		public string ReasonToCancel { get; set; }
		public string ReasonToWithdraw { get; set; }
		public string ReasonToDenyChangeRequest { get; set; }
		public string ReasonToReassess { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		#endregion

		#region Constructors
		public ApplicationWorkflowViewModel()
		{
		}

		public ApplicationWorkflowViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.FileNumber = grantApplication.FileNumber;
			this.FileName = grantApplication.GetFileName();
			this.OrganizationLegalName = grantApplication.Organization.LegalName;
			this.AssessorId = grantApplication.AssessorId;
			this.ReasonToDeny = grantApplication.GetDeniedReason();
			this.ReasonToCancel = grantApplication.GetCancelledReason();
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
		}
		#endregion
	}
}