using System;
using System.Security.Principal;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class ClaimReportViewModel : BaseClaimViewModel
	{
		#region Properties
		public ClaimTypes ClaimType { get; set; }
		public ClaimModel Claim { get; set; }
		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		#endregion

		#region Constructors
		public ClaimReportViewModel()
		{

		}

		public ClaimReportViewModel(Claim claim, IPrincipal user) : base(claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			var grantApplication = claim.GrantApplication;
			ClaimType = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;
			Claim = new ClaimModel(claim)
			{
				IsEditable = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditClaim)
			};
			ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication, false);
		}
		#endregion
	}
}
