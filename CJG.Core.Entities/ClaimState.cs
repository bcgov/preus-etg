using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="ClaimState"/> enum, provides all possible states for individual Claims and their versions.
    /// </summary>
    public enum ClaimState
    {
        /// <summary>
        /// Incomplete - A newly started Claim which in incomplete.
        /// </summary>
        [Description("Incomplete")]
        Incomplete = 0,
        /// <summary>
        /// Complete - A Claim that is ready to be submitted.
        /// </summary>
        [Description("Complete")]
        Complete = 1,
        /// <summary>
        /// NewClaim - A claim is submitted and claim assessment is the next action for assessors.
        /// Assessors assess and approve claims without director approval.
        /// The external state is "Claim Submitted"
        /// </summary>
        [Description("Unassessed")]
        Unassessed = 21,
        /// <summary>
        /// ClaimDenied - The Claim has been denied and has a zero dollar assessment outcome and will show externally in the
        /// claim assessment block with the outcome of the assessment.
        /// The external state is "Claim Denied"
        /// </summary>
        [Description("Claim Denied")]
        ClaimDenied = 24,
        /// <summary>
        /// ClaimApproved - The Claim has been approved and will now show externally in the claim assessment block with the outcome of the assessment.
        /// The external state is "Claim Approved"
        /// </summary>
        [Description("Claim Approved")]
        ClaimApproved = 25,
        /// <summary>
        /// NOTE: This is not an grant file state and needs to be recorded somewhere as a child claim state
        /// PaymentRequested - The Claim has a payment request generated (printed) for it and the payment request was sent to the
        /// government's payment department to have a cheque printed and sent to the application administrator.
        /// </summary>
        [Description("Payment Requested")]
        PaymentRequested = 26,
        /// <summary>
        /// NOTE: This is not an grant file state and needs to be recorded somewhere as a child claim state
        /// AmountOwing - The assessment of a claim (version 2 or more) resulted in an amount owing from the applicant because the assessment of
        /// claim version 1 reimbursed too much money.  This state begins the collection process for the amount owing.
        /// </summary>
        [Description("Amount Owing")]
        AmountOwing = 27,
        /// <summary>
        /// NOTE: This is not an grant file state and needs to be recorded somewhere as a child claim state
        /// ClaimPaid - The system payment request for this claim has been reconciled against the payment transactions in the government's
        /// accounts payable system and the payment request has be confirmed as paid.
        /// </summary>
        [Description("Claim Paid")]
        ClaimPaid = 28,
        /// <summary>
        /// NOTE: This is not an grant file state and needs to be recorded somewhere as a child claim state
        /// AmountReceived - The amount owing for a claim has been received from the applicant to the ministry and is confirmed in the
        /// government's accounts receivable system as paid.  No amount is owing any more.
        /// </summary>
        [Description("Amount Received")]
        AmountReceived = 29,
        /// <summary>
        /// ClaimAmended - This claim is approved, but there has been a new claim version that has been approved.
        /// </summary>
        [Description("Claim Amended")]
        ClaimAmended = 30
    }
}
