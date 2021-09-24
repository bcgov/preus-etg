using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using System;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class ClaimAssessedViewModel : BaseClaimViewModel
	{
		#region Properties
		public int? AssessorId { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public ClaimModel Claim { get; set; } // TODO: Remove business model and replace with view model.
		public ClaimAttachmentsViewModel Attachments { get; set; }
		#endregion

		#region Constructors
		public ClaimAssessedViewModel()
		{

		}

		public ClaimAssessedViewModel(Claim claim, IPrincipal user) : base(claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.ClaimType = claim.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;
			this.Claim = new ClaimModel(claim)
			{
				IsEditable = user.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim)
			};
			if (ClaimType == ClaimTypes.MultipleClaimsWithoutAmendments) { 
				this.Claim.Participants = claim.ParticipantForms.Select(x => new ParticipantFormModel(x)).ToList();
			}
			this.Attachments = new ClaimAttachmentsViewModel(claim);
		}
		#endregion
	}
}