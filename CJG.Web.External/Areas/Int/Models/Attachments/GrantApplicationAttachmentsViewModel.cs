using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Attachments
{
	public class GrantApplicationAttachmentsViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public string AttachmentsHeader { get; set; }
		public bool AttachmentsIsEnabled { get; set; }
		public bool AttachmentsRequired { get; set; }
		public int AttachmentsMaximum { get; set; }
		public string AttachmentsUserGuidance { get; set; }
		public IEnumerable<AttachmentViewModel> Attachments { get; set; }
		#endregion

		#region Constructors
		public GrantApplicationAttachmentsViewModel() { }

		public GrantApplicationAttachmentsViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.AttachmentsHeader = grantApplication.GrantOpening.GrantStream.AttachmentsHeader;
			this.AttachmentsIsEnabled = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;
			this.AttachmentsRequired = grantApplication.GrantOpening.GrantStream.AttachmentsRequired;
			this.AttachmentsMaximum = grantApplication.GrantOpening.GrantStream.AttachmentsMaximum;
			this.AttachmentsUserGuidance = grantApplication.GrantOpening.GrantStream.AttachmentsUserGuidance;

			this.Attachments = grantApplication.Attachments.Select(a => new AttachmentViewModel(a));
		}
		#endregion
	}
}