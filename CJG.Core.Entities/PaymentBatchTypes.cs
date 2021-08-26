using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// PaymentBatchTypes enum, provides a valid list of payment batch types.
    /// </summary>
    public enum PaymentBatchTypes
    {
        /// <summary>
        /// Payment requests are sent to FASB.
        /// </summary>
        [Description("Payment Request")]
        PaymentRequest = 0,
        /// <summary>
        /// Amount Owing requests are sent to the Applicant Contact.
        /// </summary>
        [Description("Amount Owing")]
        AmountOwing = 1
    }
}
