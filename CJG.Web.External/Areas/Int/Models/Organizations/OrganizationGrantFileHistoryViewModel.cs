using System;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationGrantFileHistoryViewModel : BaseViewModel
	{
		public int OrgId { get; set; }
		public bool RiskFlag { get; set; }
		public string LegalName { get; set; }
		public string DoingBusinessAs { get; set; }
		public decimal YTDRequested { get; set; }
		public decimal YTDApproved { get; set; }
		public decimal YTDPaid { get; set; }
		public string Notes { get; set; }
		public string RowVersion { get; set; }
		public bool AllowDeleteOrganization { get; set; } = false;
		public string UrlReferrer { get; set; }
		public int? GrantProgramId { get; set; }

		public OrganizationGrantFileHistoryViewModel()
		{
		}

		public OrganizationGrantFileHistoryViewModel(Organization organization, IOrganizationService organizationService)
		{
			var ytdData = organizationService.GetOrganizationYTD(organization.Id);

			OrgId = organization.Id;
			RiskFlag = organization.RiskFlag;
			LegalName = organization.LegalName;
			DoingBusinessAs = organization.DoingBusinessAs;
			YTDRequested = ytdData.TotalRequested;
			YTDApproved = ytdData.TotalApproved;
			YTDPaid = ytdData.TotalPaid;
			Notes = organization.Notes;
			RowVersion = Convert.ToBase64String(organization.RowVersion);
		}
	}
}
