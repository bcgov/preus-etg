using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class ClaimReviewViewModel : BaseClaimViewModel
	{
		#region Properties
		public ClaimTypes ClaimType { get; set; }
		public bool IsWDAService { get; set; }
		public ClaimModel Claim { get; set; } // TODO: Replace model with view model.
		public ClaimAttachmentsViewModel Attachments { get; set; }
		public ProgramTitleLabelViewModel ProgramTitleLable { get; set; }
		#endregion

		#region Constructors
		public ClaimReviewViewModel()
		{
		}

		public ClaimReviewViewModel(Claim claim) : base(claim)
		{
			this.ClaimType = claim.GrantApplication.GetClaimType();
			this.IsWDAService = claim.GrantApplication.GetProgramType() == ProgramTypes.WDAService;
			this.Claim = new ClaimModel(claim)
			{
				IsEditable = false
			};

			if (ClaimType == ClaimTypes.MultipleClaimsWithoutAmendments)
			{
				if (claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
				{
					this.Claim.Participants = claim.GrantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim).Select(pf => new ParticipantFormModel(pf)).ToList();
				}
				else
				{
					this.Claim.Participants = claim.ParticipantForms.Select(pf => new ParticipantFormModel(pf)).ToList();
				}
			}

			this.Attachments = new ClaimAttachmentsViewModel(claim);
			this.ProgramTitleLable = new ProgramTitleLabelViewModel(claim.GrantApplication, false);
		}
		#endregion
	}
}