using CJG.Web.External.Areas.Int.Models.Applications;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramDocumentTemplatePreviewModel : ApplicationPreviewModel
	{
		#region Properties
		public string Template { get; set; }
		public string Body { get; set; }
		#endregion

		#region Constructors
		public GrantProgramDocumentTemplatePreviewModel() { }
		#endregion
	}
}
