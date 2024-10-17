using System;
using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicationWorkflowViewModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }
		public string OrganizationLegalName { get; set; }
		public int? AssessorId { get; set; }
		[AllowHtml]
		public string ReasonToApprove { get; set; }
		[AllowHtml]
		public string ReasonToDeny { get; set; }
		public string ReasonToCancel { get; set; }
		public string ReasonToWithdraw { get; set; }
		public string ReasonToDenyChangeRequest { get; set; }
		public string ReasonToReassess { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public bool RiskFlag { get; set; }

		public ApplicationWorkflowViewModel()
		{
		}

		public ApplicationWorkflowViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			FileNumber = grantApplication.FileNumber;
			FileName = grantApplication.GetFileName();
			OrganizationLegalName = grantApplication.Organization.LegalName;
			AssessorId = grantApplication.AssessorId;
			ReasonToApprove = grantApplication.GetApprovedReason();
			ReasonToDeny = grantApplication.GetDeniedReason();
			ReasonToCancel = grantApplication.GetCancelledReason();
			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			RiskFlag = grantApplication.Organization.RiskFlag;
		}
	}
}