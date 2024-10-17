using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
    public class ClaimAttachmentsViewModel : BaseViewModel
	{
		public int ClaimVersion { get; set; }
		public string Title { get; set; }
		public int MaxUploadSize { get; set; }
		public int MaximumNumberOfAttachmentsAllowed { get; set; }
		public string RowVersion { get; set; }

		public bool? ParticipantsPaidForExpenses { get; set; }
		public bool? ParticipantsHaveBeenReimbursed { get; set; }

		public IEnumerable<AttachmentViewModel> Attachments { get; set; }

		public ClaimAttachmentsViewModel() { }

		public ClaimAttachmentsViewModel(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);

			Id = claim.Id;
			ClaimVersion = claim.ClaimVersion;
			Title = "Proof of Payment Documents";
			MaxUploadSize = maxUploadSize / 1024 / 1024;
			MaximumNumberOfAttachmentsAllowed = Constants.MaximumNumberOfAttachmentsPerClaim;
			RowVersion = Convert.ToBase64String(claim.RowVersion);

			ParticipantsPaidForExpenses = claim.ParticipantsPaidForExpenses;
			ParticipantsHaveBeenReimbursed = claim.ParticipantsHaveBeenReimbursed;

			// When a claim has been updated before (ie: a fresh new Claim) but is still incomplete, default the Paid Question to false if it's null.
			if (claim.DateUpdated.HasValue && claim.ClaimState == ClaimState.Incomplete)
				ParticipantsPaidForExpenses = claim.ParticipantsPaidForExpenses ?? false;

			Attachments = claim.Receipts.Select(a => new AttachmentViewModel(a));
		}
	}
}
