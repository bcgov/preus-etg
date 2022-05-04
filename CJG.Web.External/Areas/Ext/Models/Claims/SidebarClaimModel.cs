using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class SidebarClaimModel
	{
		public string ClaimStateText { get; set; }
		public ClaimState? ClaimState { get; set; }
		public DateTime? ClaimStateDate { get; set; }

		public SidebarClaimModel(GrantApplication grantApplication)
		{
			var claim = grantApplication.GetCurrentClaim();

			if (claim == null)
				return;
			
			ClaimState = claim.ClaimState;
			ClaimStateText = claim.ClaimState.GetDescription();

			if (ClaimState == Core.Entities.ClaimState.ClaimApproved)
				ClaimStateDate = claim.GrantApplication.GetStateChange(ApplicationStateInternal.ClaimApproved).ChangedDate;

			if (ClaimState == Core.Entities.ClaimState.ClaimDenied)
				ClaimStateDate = claim.GrantApplication.GetStateChange(ApplicationStateInternal.ClaimDenied).ChangedDate;
		}
	}
}