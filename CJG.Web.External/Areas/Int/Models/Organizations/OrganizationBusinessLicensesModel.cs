using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationBusinessLicensesModel : BaseViewModel
	{
		public int OrganizationId { get; set; }
		public bool CanAddBusinessLicenses { get; set; }
		public IEnumerable<AttachmentViewModel> BusinessLicenseDocumentAttachments { get; set; }

		public OrganizationBusinessLicensesModel()
		{
		}

		public OrganizationBusinessLicensesModel(Organization organization)
		{
			OrganizationId = organization.Id;
			CanAddBusinessLicenses = false;
			BusinessLicenseDocumentAttachments = organization.BusinessLicenseDocuments.Select(a => new AttachmentViewModel(a));
		}
	}
}