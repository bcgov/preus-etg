using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class ClaimAttachmentDetailViewModel : BaseViewModel
	{
		public int MaximumNumberOfAttachmentsAllowed { get; set; }
		public string RowVersion { get; set; }
		public int MaxUploadSize { get; set; }
		public IEnumerable<AttachmentViewModel> ClaimAttachments { get; set; }

		public ClaimAttachmentDetailViewModel()
		{
		}

		public ClaimAttachmentDetailViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			Id = claim.Id;
			ClaimAttachments = claim.Receipts.Select(attachment => new AttachmentViewModel(attachment)).ToArray();
			MaximumNumberOfAttachmentsAllowed = Constants.MaximumNumberOfAttachmentsPerClaim;
			RowVersion = Convert.ToBase64String(claim.RowVersion);
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			MaxUploadSize = maxUploadSize / 1024 / 1024;
		}
	}
}