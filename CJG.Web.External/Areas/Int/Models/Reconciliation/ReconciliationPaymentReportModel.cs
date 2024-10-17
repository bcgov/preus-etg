using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Reconciliation
{
	class ReconciliationPaymentReportModel
	{
		#region Properties
		public ReconciliationPayment ReconciliationPayment { get; }
		public PaymentRequest PaymentRequest { get; }
		#endregion

		#region Constructors
		public ReconciliationPaymentReportModel(ReconciliationPayment reconciliationPayment, PaymentRequest paymentRequest)
		{
			this.ReconciliationPayment = reconciliationPayment;
			this.PaymentRequest = paymentRequest;
		}
		#endregion

		#region Methods
		public override int GetHashCode()
		{
			return this.PaymentRequest != null
				? (this.PaymentRequest.PaymentRequestBatchId.GetHashCode()
					+ this.PaymentRequest.GrantApplicationId.GetHashCode()
					+ this.PaymentRequest.ClaimId.GetHashCode())
				: this.ReconciliationPayment.Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ReconciliationPaymentReportModel reconciliationPaymentReport)) return false;
			return this.PaymentRequest != null
				? this.PaymentRequest.PaymentRequestBatchId == reconciliationPaymentReport.PaymentRequest?.PaymentRequestBatchId
					&& this.PaymentRequest.GrantApplicationId == reconciliationPaymentReport.PaymentRequest?.GrantApplicationId
					&& this.PaymentRequest.ClaimId == reconciliationPaymentReport.PaymentRequest?.ClaimId
				: this.ReconciliationPayment.Id == reconciliationPaymentReport.ReconciliationPayment.Id;
		}
		#endregion
	}
}