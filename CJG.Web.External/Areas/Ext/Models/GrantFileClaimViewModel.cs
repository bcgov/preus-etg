using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantFileClaimViewModel
	{
		#region Properties
		public int ClaimId { get; set; }
		public int ClaimVersion { get; set; }
		public ClaimState ClaimState { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public DateTime? DateAssessed { get; set; }
		public string Description { get; set; }
		public string DocumentNumber { get; set; }
		public string AmountStyle { get; set; }
		public decimal? DisplayAmount { get; set; }
		public DateTime? DateUpdated { get; set; }
		#endregion

		#region Constructors
		public GrantFileClaimViewModel()
		{
		}
		public GrantFileClaimViewModel(Claim claim, ref Decimal approved)
		{
			this.ClaimId = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.ClaimState = claim.ClaimState;
			this.DateSubmitted = claim.DateSubmitted?.ToLocalMorning();
			this.DateAssessed = claim.DateAssessed?.ToLocalMorning();
			this.DateUpdated = claim.DateUpdated;
			this.Description = claim.GetClaimStateDescription();
			this.DocumentNumber = claim.PaymentRequests.FirstOrDefault()?.DocumentNumber;
			var amount = claim.IsApproved() ?
				claim.TotalAssessedReimbursement - approved
				: claim.TotalClaimReimbursement - approved;

			if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				approved += claim.IsApproved() ? claim.TotalAssessedReimbursement - approved : 0;
			}
			else
			{
				approved = 0;
			}
			if (claim.ClaimState.In(ClaimState.ClaimPaid, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.AmountReceived))
			{
				this.DisplayAmount = claim.PaymentRequests.FirstOrDefault()?.PaymentAmount ?? 0;
			}
			else if (claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete, ClaimState.Unassessed))
			{
				this.DisplayAmount = claim.TotalClaimReimbursement;
			}
			else if (!claim.ClaimState.In(ClaimState.ClaimAmended, ClaimState.ClaimDenied))
			{
				this.DisplayAmount = amount;
			}
		}
		#endregion
	}
}