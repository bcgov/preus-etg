using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class ClaimAssessmentOutcomeViewModel
	{
		#region Properties
		public string GrantProgramName { get; set; }
		public string SummaryCSS { get; set; }
		public string SummaryDescription { get; set; }
		public bool ClaimProcessed { get; set; }
		public IEnumerable<GrantFileClaimViewModel> Claims { get; set; }
		#endregion

		#region Constructors
		public ClaimAssessmentOutcomeViewModel()
		{
		}
		public ClaimAssessmentOutcomeViewModel(GrantApplication grantApplication)
		{
			decimal approved = 0;
			this.Claims = grantApplication.Claims.Where(c => c.ClaimState.In(
					ClaimState.Unassessed,
					ClaimState.ClaimDenied,
					ClaimState.ClaimApproved,
					ClaimState.ClaimAmended,
					ClaimState.PaymentRequested,
					ClaimState.AmountOwing,
					ClaimState.ClaimPaid,
					ClaimState.AmountReceived))
				.OrderByDescending(c => c.Id).ThenByDescending(c => c.ClaimVersion).Select(x => new GrantFileClaimViewModel(x, ref approved)).ToArray();

			this.ClaimProcessed = this.Claims.Any(c => c.ClaimState.In(ClaimState.ClaimPaid, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.AmountReceived));
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			var summary = this.Claims.OrderByDescending(c => c.DateUpdated).FirstOrDefault();
			if (summary != null)
			{
				SetSummary(summary.ClaimState);
			}
		}
		#endregion

		#region Methods
		public void SetSummary(ClaimState claimState)
		{
			switch (claimState)
			{
				case ClaimState.Unassessed:
					this.SummaryCSS = "block--claim-outcome__success";
					this.SummaryDescription = @"Your claim has been submitted. A member of our team will review your claim for accuracy before reimbursements are made.
                                                You may return here to check the status of your claim anytime. We may contact you for additional information
                                                or to verify details of your claim.
                                                <b> Please respond to these requests as soon as possible.</b>
                                                If we are unable to reach you, your claim may be cancelled by the Ministry.";
					break;
				case ClaimState.ClaimApproved:
					this.SummaryCSS = "block--claim-outcome__success";
					this.SummaryDescription = @"Your Claim has been approved. Your reimbursement amount is shown below.
												<br />
												You may view the details of your claim assessment by clicking on it below.";
					break;
				case ClaimState.ClaimDenied:
					this.SummaryCSS = "block--claim-outcome__denied";
					this.SummaryDescription = @"Your claim has been denied.
												<br />
												You may view the details of your claim assessment by clicking on it below.";
					break;
				case ClaimState.PaymentRequested:
					this.SummaryCSS = "block--claim-outcome__success";
					this.SummaryDescription = @"A payment will be processed for your reimbursement.
												<br />
												You may view the details of your claim assessment by clicking on it below.";
					break;
				case ClaimState.ClaimPaid:
					this.SummaryCSS = "block--claim-outcome__success";
					this.SummaryDescription = @"Your claim has been paid.
												<br />
												Thank you for using the " + this.GrantProgramName + ".";
					break;
				case ClaimState.AmountOwing:
					this.SummaryCSS = "block--claim-outcome__warning";
					this.SummaryDescription = @"Your claim has been approved and an amount owing to the Ministry is shown below.
												<br />
												You may view the details of your claim assessment by clicking on it below.
												<br />
												<span style=""font-weight: bold;""> Instructions on how to make your payment will be sent to you by email.</span>";
					break;
				case ClaimState.AmountReceived:
					this.SummaryCSS = "block--claim-outcome__success";
					this.SummaryDescription = @"The amount owing has been received.
												<br />
												Thank you for using the " + this.GrantProgramName + ".";
					break;
			}
		}
		#endregion
	}
}