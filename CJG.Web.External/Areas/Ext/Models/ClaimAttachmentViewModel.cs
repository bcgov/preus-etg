using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ClaimAttachmentViewModel : BaseViewModel
	{
		#region Properties
		public int ClaimId { get; set; }
		public int ClaimVersion { get; set; }
		public string RowVersion { get; set; }
		public string Guid { get; set; }
		public AttachmentModel Attachment { get; set; } = new AttachmentModel();
		#endregion

		#region Constructors
		public ClaimAttachmentViewModel()
		{
			this.Guid = System.Guid.NewGuid().ToString();
		}

		public ClaimAttachmentViewModel(Claim claim, AttachmentModel attachment) : this()
		{
			this.ClaimId = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.Attachment = attachment ?? throw new ArgumentNullException(nameof(attachment));
		}
		#endregion
	}
}