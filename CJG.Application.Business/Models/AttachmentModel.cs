using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models
{
	public class AttachmentModel
	{
		#region Properties
		public int Id { get; set; }
		public int Sequence { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public AttachmentModel()
		{

		}

		public AttachmentModel(Attachment attachment, int sequence)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			this.Id = attachment.Id;
			this.Name = attachment.FileName;
			this.Description = attachment.Description;
			this.RowVersion = attachment.RowVersion != null ? Convert.ToBase64String(attachment.RowVersion) : null;
			this.Sequence = sequence;
		}
		#endregion
	}
}
