using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationBusinessLicensesModel : BaseViewModel
	{
		public int OrgId { get; set; }
		public IEnumerable<AttachmentViewModel> BusinessLicenseDocumentAttachments { get; set; }

		public OrganizationBusinessLicensesModel()
		{
		}

		public OrganizationBusinessLicensesModel(Organization organization)
		{
			OrgId = organization.Id;
			BusinessLicenseDocumentAttachments = organization.BusinessLicenseDocuments.Select(a => new AttachmentViewModel(a));
		}
	}
}