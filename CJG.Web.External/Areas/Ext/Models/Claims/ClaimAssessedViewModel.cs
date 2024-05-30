using System;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class ClaimAssessedViewModel : BaseClaimViewModel
	{
		public int? AssessorId { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public ClaimModel Claim { get; set; } // TODO: Remove business model and replace with view model.
		public ClaimAttachmentsViewModel Attachments { get; set; }

		public ClaimAssessedViewModel()
		{
		}

		public ClaimAssessedViewModel(Claim claim, IPrincipal user) : base(claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			ClaimType = claim.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;
			Claim = new ClaimModel(claim)
			{
				IsEditable = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim)
			};

			if (ClaimType == ClaimTypes.MultipleClaimsWithoutAmendments)
			{
				Claim.Participants = claim.ParticipantForms.Select(x => new ParticipantFormModel(x)).ToList();
			}

			Attachments = new ClaimAttachmentsViewModel(claim);
		}
	}
}