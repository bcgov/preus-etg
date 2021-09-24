using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class ClaimAttachmentsViewModel : BaseViewModel
	{
		#region Properties
		public int ClaimVersion { get; set; }
		public bool IsWDAService { get; set; }
		public string Title { get; set; }
		public int MaxUploadSize { get; set; }
		public int MaximumNumberOfAttachmentsAllowed { get; set; }
		public string RowVersion { get; set; }

		public IEnumerable<AttachmentViewModel> Attachments { get; set; }
		#endregion

		#region Constructors
		public ClaimAttachmentsViewModel() { }

		public ClaimAttachmentsViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);

			this.Id = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.IsWDAService = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			this.Title = this.IsWDAService ? "Supporting Documentation" : "Proof of Payment Documents";
			this.MaxUploadSize = maxUploadSize / 1024 / 1024;
			this.MaximumNumberOfAttachmentsAllowed = Constants.MaximumNumberOfAttachmentsPerClaim;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);

			this.Attachments = claim.Receipts.Select(a => new AttachmentViewModel(a));
		}
		#endregion
	}
}
