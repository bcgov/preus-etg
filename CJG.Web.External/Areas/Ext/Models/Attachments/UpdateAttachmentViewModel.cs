using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class UpdateAttachmentViewModel : AttachmentViewModel
	{
		#region Properties
		public int? Index { get; set; }
		public bool Delete { get; set; }
		#endregion

		#region Constructors
		public UpdateAttachmentViewModel() { }

		public UpdateAttachmentViewModel(Attachment attachment) : base(attachment)
		{
		}
		#endregion

		#region Methods
		public void MapToEntity(Attachment attachment)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			attachment.Id = this.Id;
			attachment.RowVersion = !String.IsNullOrWhiteSpace(this.RowVersion) ? Convert.FromBase64String(this.RowVersion) : null;
			attachment.FileName = this.FileName;
			attachment.Description = this.Description;
			attachment.FileExtension = this.FileExtension;
		}
		#endregion
	}
}
