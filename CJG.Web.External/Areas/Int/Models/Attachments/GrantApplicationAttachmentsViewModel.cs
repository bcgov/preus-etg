using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Attachments
{
	public class GrantApplicationAttachmentsViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string AttachmentsHeader { get; set; }
		public bool AttachmentsIsEnabled { get; set; }
		public bool AttachmentsRequired { get; set; }
		public int AttachmentsMaximum { get; set; }
		public string AttachmentsUserGuidance { get; set; }
		public IEnumerable<AttachmentViewModel> Attachments { get; set; }

		public GrantApplicationAttachmentsViewModel() { }

		public GrantApplicationAttachmentsViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			AttachmentsHeader = grantApplication.GrantOpening.GrantStream.AttachmentsHeader;
			AttachmentsIsEnabled = true; // Grant Attachments are always on for Admin   grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;
			AttachmentsRequired = grantApplication.GrantOpening.GrantStream.AttachmentsRequired;
			AttachmentsMaximum = grantApplication.GrantOpening.GrantStream.AttachmentsMaximum;
			AttachmentsUserGuidance = grantApplication.GrantOpening.GrantStream.AttachmentsUserGuidance;

			Attachments = grantApplication.Attachments.Select(a => new AttachmentViewModel(a));
		}
	}
}