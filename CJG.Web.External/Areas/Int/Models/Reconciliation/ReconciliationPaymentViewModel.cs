using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Reconciliation
{
	public class ReconciliationPaymentViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int? PaymentRequestBatchId { get; set; }
		public int? GrantApplicationId { get; set; }
		public int? ClaimId { get; set; }
		public int? ClaimVersion { get; set; }
		public string BatchName { get; set; }
		public decimal Amount { get; set; }
		public decimal PaymentAmount { get; set; }
		public DateTime? DateCreated { get; set; }
		public string DocumentNumber { get; set; }
		public PaymentTypes PaymentType { get; set; }
		public string PaymentTypeCaption { get; set; }
		public ReconciliationStates State { get; set; }
		public string StateCaption { get; set; }
		public string SupplierName { get; set; }
		public string SupplierNumber { get; set; }
		public bool FromCAS { get; set; }
		public DateTime? IssuedDate { get; set; }
		public IEnumerable<ReconciliationPaymentViewModel> Payments { get; set; }
		#endregion

		#region Constructors
		public ReconciliationPaymentViewModel() { }

		public ReconciliationPaymentViewModel(ReconciliationPayment payment)
		{
			if (payment == null) throw new ArgumentNullException(nameof(payment));

			this.Id = payment.Id;
			this.RowVersion = Convert.ToBase64String(payment.RowVersion);

			this.PaymentRequestBatchId = payment.PaymentRequestBatchId;
			this.GrantApplicationId = payment.GrantApplicationId;
			this.ClaimId = payment.PaymentRequest?.ClaimId;
			this.ClaimVersion = payment.PaymentRequest?.ClaimVersion;

			this.BatchName = payment.BatchName;
			this.Amount = payment.Amount;
			this.PaymentAmount = payment.PaymentRequest?.PaymentAmount ?? 0;
			this.DateCreated = payment.DateCreated.HasValue ? payment.DateCreated.Value.ToLocalTime() : (DateTime?)null;
			this.DocumentNumber = payment.DocumentNumber;
			this.PaymentType = payment.PaymentType;
			this.PaymentTypeCaption = payment.PaymentType.GetDescription();
			this.State = payment.ReconcilationState;
			this.StateCaption = payment.ReconcilationState.GetDescription();
			this.SupplierName = payment.SupplierName;
			this.SupplierNumber = payment.SupplierNumber;
			this.FromCAS = payment.FromCAS;
			this.IssuedDate = payment.PaymentRequest?.PaymentRequestBatch?.IssuedDate;
		}

		public ReconciliationPaymentViewModel(PaymentRequest paymentRequest)
		{
			if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));

			this.Id = 0;
			this.RowVersion = null;
			this.PaymentRequestBatchId = paymentRequest.PaymentRequestBatchId;
			this.GrantApplicationId = paymentRequest.GrantApplicationId;
			this.ClaimId = paymentRequest.ClaimId;
			this.ClaimVersion = paymentRequest.ClaimVersion;

			var recPayment = paymentRequest.ReconciliationPayments.OrderByDescending(rp => rp.DateCreated).ThenByDescending(rp => rp.DateAdded).First();

			this.BatchName = recPayment.BatchName;
			this.Amount = paymentRequest.ReconciliationPayments.Sum(p => p.Amount);
			this.PaymentAmount = paymentRequest.PaymentAmount;
			this.DateCreated = recPayment.DateCreated.HasValue ? recPayment.DateCreated.Value.ToLocalTime() : (DateTime?)null;
			this.DocumentNumber = paymentRequest.DocumentNumber;
			this.PaymentType = paymentRequest.PaymentType;
			this.PaymentTypeCaption = paymentRequest.PaymentType.GetDescription();
			this.State = recPayment.ReconcilationState;
			this.StateCaption = recPayment.ReconcilationState.GetDescription();
			this.SupplierName = paymentRequest.GrantApplication.OrganizationLegalName;
			this.SupplierNumber = paymentRequest.RecipientBusinessNumber;

			this.Payments = paymentRequest.ReconciliationPayments.Where(rp => rp.FromCAS).OrderBy(p => p.DateCreated).ThenByDescending(rp => rp.DateAdded).Select(p => new ReconciliationPaymentViewModel(p)).ToArray();
			this.FromCAS = paymentRequest.ReconciliationPayments.Any(p => p.FromCAS);
			this.IssuedDate = paymentRequest.PaymentRequestBatch?.IssuedDate;
		}
		#endregion
	}
}