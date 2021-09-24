namespace CJG.Core.Entities
{
	/// <summary>
	/// PaymentRequestExtensions static class, provides extension methods to help with payment requests.
	/// </summary>
	public static class PaymentRequestExtensions
	{
		/// <summary>
		/// Determine the payment type for the specified payment request.
		/// If the payment amount = $0, the payment type will be 'None'
		/// If the payment belongs to a grant application that began before the current fiscal year, the payment type will be 'Accrual'.
		/// If the payment belongs to a grant application that began in the current fiscal year, the payment type will be 'Normal'.
		/// </summary>
		/// <param name="paymentRequest"></param>
		/// <returns></returns>
		public static PaymentTypes DeterminePaymentType(this PaymentRequest paymentRequest)
		{
			var isAccrual = paymentRequest.Claim.GrantApplication.GrantOpening.TrainingPeriod.FiscalYear.EndDate < paymentRequest.DateAdded;
			return paymentRequest.PaymentAmount == 0 ? PaymentTypes.None : isAccrual ? PaymentTypes.Accrual : PaymentTypes.Normal;
		}

		/// <summary>
		/// Determine the payment type for the specified reconciliation payment.
		/// If the payment amount = $0, the payment type will be 'None'
		/// If the payment belongs to a grant application that began before the current fiscal year, the payment type will be 'Accrual'.
		/// If the payment belongs to a grant application that began in the current fiscal year, the payment type will be 'Normal'.
		/// </summary>
		/// <param name="reconciliationPayment"></param>
		/// <returns></returns>
		public static PaymentTypes DeterminePaymentType(this ReconciliationPayment reconciliationPayment)
		{
			var isAccrual = reconciliationPayment?.GrantApplication?.GrantOpening?.TrainingPeriod?.FiscalYear?.EndDate < reconciliationPayment.DateCreated;
			return reconciliationPayment.Amount == 0 ? PaymentTypes.None : isAccrual ? PaymentTypes.Accrual : PaymentTypes.Normal;
		}
	}
}
