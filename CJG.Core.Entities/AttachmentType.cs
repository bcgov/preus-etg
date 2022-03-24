using System.ComponentModel;

namespace CJG.Core.Entities
{
	public enum AttachmentType
	{
		[Description("Applicant Attachment")]
		Attachment = 0,

		[Description("Ministry Attachment")]
		Document = 1
	}
}