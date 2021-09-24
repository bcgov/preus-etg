using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.TrainingPrograms
{
	public class GrantApplicationAttachmentDetailViewModel : BaseViewModel
	{
		#region Properties
		public bool AllowAttachment { get; set; }
		public int AttachmentMaximum { get; set; }
		public string AttachmentHeader { get; set; }
		public string AttachmentGuidance { get; set; }
		public IEnumerable<GrantApplicationAttachmentViewModel> GrantApplicationAttachments { get; set; }
		#endregion

		#region Constructors
		public GrantApplicationAttachmentDetailViewModel()
		{

		}

		public GrantApplicationAttachmentDetailViewModel(GrantApplication grantApplication, IEnumerable<AttachmentModel> attachments)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (grantApplication?.GrantOpening?.GrantStream == null) throw new ArgumentNullException(nameof(grantApplication), "GrantStream cannot be null.");

			this.AllowAttachment = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;
			this.AttachmentHeader = grantApplication.GrantOpening.GrantStream.AttachmentsHeader;
			this.AttachmentGuidance = grantApplication.GrantOpening.GrantStream.AttachmentsUserGuidance;
			this.AttachmentMaximum = grantApplication.GrantOpening.GrantStream.AttachmentsMaximum;
			this.GrantApplicationAttachments = attachments.Select(y => new GrantApplicationAttachmentViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				Guid = Guid.NewGuid().ToString(),
				Attachment = y
			}).ToArray();
		}
		#endregion

	}
}
