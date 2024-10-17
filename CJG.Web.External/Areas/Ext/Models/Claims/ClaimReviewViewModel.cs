using System.Linq;
using CJG.Application.Business.Models;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
    public class ClaimReviewViewModel : BaseClaimViewModel
	{
		public ClaimTypes ClaimType { get; set; }
		public ClaimModel Claim { get; set; }
		public ClaimAttachmentsViewModel Attachments { get; set; }
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }

		public ClaimReviewViewModel()
		{
		}

		public ClaimReviewViewModel(Claim claim) : base(claim)
		{
			ClaimType = claim.GrantApplication.GetClaimType();
			Claim = new ClaimModel(claim)
			{
				IsEditable = false
			};

			if (ClaimType == ClaimTypes.MultipleClaimsWithoutAmendments)
			{
				if (claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
				{
					Claim.Participants = claim.GrantApplication.ParticipantForms
						.Where(pf => !pf.IsExcludedFromClaim)
						.Select(pf => new ParticipantFormModel(pf))
						.ToList();
				}
				else
				{
					Claim.Participants = claim.ParticipantForms
						.Select(pf => new ParticipantFormModel(pf))
						.ToList();
				}
			}

			Attachments = new ClaimAttachmentsViewModel(claim);
			ProgramTitleLabel = new ProgramTitleLabelViewModel(claim.GrantApplication, false);
		}
	}
}