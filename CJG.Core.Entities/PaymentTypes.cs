using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// PaymentTypes enum, provides a valid list of payment types.
    /// </summary>
    public enum PaymentTypes
    {
        /// <summary>
        /// "If the payment request is issued in the fiscal year that contains the training start date of the training program, then the payment type is 'Normal'"
        /// </summary>
        [Description("Normal")]
        Normal = 0,
        /// <summary>
        /// "If the payment request is issued in a fiscal year following the fiscal year that contains the training start date of the training program, then the payment type is 'Accrual'"
        /// </summary>
        [Description("Accrual")]
        Accrual = 1,
        /// <summary>
        /// A claim with ClaimState=”ClaimApproved” and with **payment amount** == zero will generate a PaymentRequest with a value of zero and a type of 'None'
        /// </summary>
        [Description("None")]
        None = 2
    }
}
