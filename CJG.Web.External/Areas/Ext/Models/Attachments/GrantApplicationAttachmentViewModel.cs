using System;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class GrantApplicationAttachmentViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string Guid { get; set; }
		public AttachmentModel Attachment { get; set; } = new AttachmentModel();

		public GrantApplicationAttachmentViewModel()
		{
			Guid = System.Guid.NewGuid().ToString();
		}

		public GrantApplicationAttachmentViewModel(GrantApplication grantApplication, Attachment attachment) : this(grantApplication)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			Attachment.Id = attachment.Id;
			Attachment.Name = attachment.FileName;
			Attachment.Description = attachment.Description;
			Attachment.RowVersion = attachment.RowVersion != null ? Convert.ToBase64String(attachment.RowVersion) : null;
			Attachment.AttachmentType = attachment.AttachmentType;
		}

		public GrantApplicationAttachmentViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			GrantApplicationId = grantApplication.Id;
		}
	}
}