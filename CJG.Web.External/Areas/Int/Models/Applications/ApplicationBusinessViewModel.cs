using System.Collections.Generic;
using CJG.Web.External.Areas.Int.Models.Attachments;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ApplicationBusinessViewModel
	{
		public string BusinessWebsite { get; set; }
		public string BusinessDescription { get; set; }
		public IEnumerable<AttachmentViewModel> BusinessLicenseDocumentAttachments { get; set; }
	}
}