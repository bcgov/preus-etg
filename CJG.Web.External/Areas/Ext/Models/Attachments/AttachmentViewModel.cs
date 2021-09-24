using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class AttachmentViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string FileExtension { get; set; }
		public int VersionNumber { get; set; }
		public DateTime DateAdded { get; set; }
		public DateTime? DateUpdated { get; set; }
		#endregion

		#region Constructors
		public AttachmentViewModel() { }

		public AttachmentViewModel(Attachment attachment)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			this.Id = attachment.Id;
			this.RowVersion = Convert.ToBase64String(attachment.RowVersion);
			this.FileName = attachment.FileName;
			this.Description = attachment.Description;
			this.FileExtension = attachment.FileExtension;
			this.VersionNumber = attachment.VersionNumber;
			this.DateAdded = attachment.DateAdded;
			this.DateUpdated = attachment.DateUpdated;
		}
		#endregion
	}
}
