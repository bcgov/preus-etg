using System;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class AttachmentModel
	{
		public int Id { get; set; }
		public int Sequence { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		public AttachmentType AttachmentType { get; set; }

		public AttachmentModel()
		{
		}

		public AttachmentModel(Attachment attachment, int sequence)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			Id = attachment.Id;
			Name = attachment.FileName;
			Description = attachment.Description;
			AttachmentType = attachment.AttachmentType;
			RowVersion = attachment.RowVersion != null ? Convert.ToBase64String(attachment.RowVersion) : null;
			Sequence = sequence;
		}
	}
}
