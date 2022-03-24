using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Attachments
{
	public class AttachmentViewModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string FileExtension { get; set; }
		public int VersionNumber { get; set; }
		public AttachmentType AttachmentType { get; set; }
		public DateTime DateAdded { get; set; }
		public DateTime? DateUpdated { get; set; }

		public AttachmentViewModel() { }

		public AttachmentViewModel(Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			Id = attachment.Id;
			RowVersion = Convert.ToBase64String(attachment.RowVersion);
			FileName = attachment.FileName;
			Description = attachment.Description;
			FileExtension = attachment.FileExtension;
			VersionNumber = attachment.VersionNumber;
			AttachmentType = attachment.AttachmentType;
			DateAdded = attachment.DateAdded;
			DateUpdated = attachment.DateUpdated;
		}
	}
}