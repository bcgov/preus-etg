using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class GrantApplicationAttachmentViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string Guid { get; set; }
		public AttachmentModel Attachment { get; set; } = new AttachmentModel();
		#endregion

		#region Constructors
		public GrantApplicationAttachmentViewModel()
		{
			this.Guid = System.Guid.NewGuid().ToString();
		}

		public GrantApplicationAttachmentViewModel(GrantApplication grantApplication, Attachment attachment) : this(grantApplication)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			this.Attachment.Id = attachment.Id;
			this.Attachment.Name = attachment.FileName;
			this.Attachment.Description = attachment.Description;
			this.Attachment.RowVersion = attachment.RowVersion != null ? Convert.ToBase64String(attachment.RowVersion) : null;
		}

		public GrantApplicationAttachmentViewModel(GrantApplication grantApplication) : base()
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
		}
		#endregion
	}
}
