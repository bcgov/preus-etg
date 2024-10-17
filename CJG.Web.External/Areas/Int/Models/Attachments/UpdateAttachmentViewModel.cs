using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Attachments
{
	public class UpdateAttachmentViewModel : AttachmentViewModel
	{
		public int? Index { get; set; }
		public bool Delete { get; set; }

		public UpdateAttachmentViewModel() { }

		public UpdateAttachmentViewModel(Attachment attachment) : base(attachment)
		{
		}

		public void MapToEntity(Attachment attachment)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			attachment.Id = Id;
			attachment.RowVersion = !String.IsNullOrWhiteSpace(RowVersion) ? Convert.FromBase64String(RowVersion) : null;
			attachment.FileName = FileName;
			attachment.Description = Description;
			attachment.FileExtension = FileExtension;
			attachment.AttachmentType = AttachmentType;
		}
	}
}