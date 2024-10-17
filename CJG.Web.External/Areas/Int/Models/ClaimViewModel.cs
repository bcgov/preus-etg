using System;
using System.ComponentModel;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ClaimViewModel : BaseViewModel
    {
        #region Properties
        public int ClaimVersion { get; set; }
        public int GrantApplicationId { get; set; }
        public bool IsNewClaim { get; set; }
        public bool HasPaymentRequest { get; set; }
        public decimal TotalClaimReimbursement { get; set; }
        public decimal TotalAssessedReimbursement { get; set; }
        public decimal ClaimAmount { get; set; }
        public string ClaimStateDescription { get; set; }
        public string PaymentRequestDocumentNumber { get; set; }
        public string DateAssessed { get; set; }
        public string DateSubmitted { get; set; }
        [DefaultValue(ClaimState.Incomplete)]
        public ClaimState ClaimState { get; set; } = ClaimState.Incomplete;
        public string RowVersion { get; set; }
        #endregion

        #region Constructors
        public ClaimViewModel()
        {

        }

        public ClaimViewModel(Claim claim)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            this.Id = claim.Id;
            this.ClaimVersion = claim.ClaimVersion;
            this.GrantApplicationId = claim.GrantApplicationId;
            this.TotalClaimReimbursement = claim.TotalClaimReimbursement;
			this.TotalAssessedReimbursement = claim.TotalAssessedReimbursement;
			this.DateSubmitted = claim.DateSubmitted?.ToLocalMorning().ToString("yyyy-MM-dd") ?? "";
			this.DateAssessed = claim.DateAssessed?.ToLocalMorning().ToString("yyyy-MM-dd") ?? "";
			this.ClaimState = claim.ClaimState;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.IsNewClaim = claim.GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.NewClaim)
				   && claim.ClaimState.In(ClaimState.Unassessed);

			var claimRaymentRequest = claim.PaymentRequests.FirstOrDefault();
			this.HasPaymentRequest = (claimRaymentRequest != null);
			this.ClaimAmount = (claim.ClaimState == ClaimState.ClaimApproved ? claim.AmountPaidOrOwing()
                                                : (claimRaymentRequest?.PaymentAmount ?? 0));
            this.ClaimStateDescription = claim.GetClaimStateDescription();
            this.PaymentRequestDocumentNumber = claimRaymentRequest?.DocumentNumber;
        }
        #endregion
    }
}
